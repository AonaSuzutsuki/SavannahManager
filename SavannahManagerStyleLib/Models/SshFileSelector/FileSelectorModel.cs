using CommonStyleLib.Models;
using SvManagerLibrary.Ssh;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using Prism.Commands;
using Prism.Mvvm;
using SavannahManagerStyleLib.Models.Image;
using SavannahManagerStyleLib.ViewModels.SshFileSelector;

namespace SavannahManagerStyleLib.Models.SshFileSelector
{
    public class SftpFileDetailInfo : BindableBase
    {
        #region Fields
        
        private string _fullPath;
        private string _name;
        private bool _isEditMode;

        #endregion

        public Func<SftpFileDetailInfo, Task> LostFocusAction { get; set; }

        public string Parent => Path.GetDirectoryName(FullPath)?.Replace("\\", "/").TrimEnd('/');

        public string FullPath
        {
            get => _fullPath;
            set => SetProperty(ref _fullPath, value);
        }
        public bool IsDirectory { get; set; }

        public ImageSource ImageSource { get; set; }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        public string DateString { get; set; }

        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }

        public ICommand EditTextBoxLostFocusCommand { get; set; }

        public SftpFileDetailInfo()
        {
            EditTextBoxLostFocusCommand = new DelegateCommand(EditTextBoxLostFocus);
        }

        public void EditTextBoxLostFocus()
        {
            LostFocusAction?.Invoke(this);
        }
    }

    public class SftpOpenStreamItem
    {
        public string FullPath { get; set; }
        public Stream Stream { get; }

        public SftpOpenStreamItem(Stream stream, string fullPath)
        {
            FullPath = fullPath;
            Stream = stream;
        }
    }

    public class FileSelectorModel : ModelBase, IDisposable
    {
        #region Fields

        private SftpServerConnector _sftpServerConnector;
        private Stack<string> _pageForwardHistory;
        private Stack<string> _pageBackHistory;

        private bool _canGoBack;
        private bool _canGoForward;
        private string _currentDirectory;
        private ObservableCollection<SftpFileDetailInfo> _fileList = new();

        #endregion

        #region Properties

        public HashSet<string> TargetExtensions { get; set; }

        public bool IsOpenStream { get; set; } = true;
        public Action<SftpOpenStreamItem> OpenCallbackAction { get; set; }
        public Func<byte[]> SaveDataFunction { get; set; }
        public FileDirectoryMode DirectoryMode { get; set; } = FileDirectoryMode.File;

        public bool CanGoBack
        {
            get => _canGoBack;
            set => SetProperty(ref _canGoBack, value);
        }

        public bool CanGoForward
        {
            get => _canGoForward;
            set => SetProperty(ref _canGoForward, value);
        }

        public string CurrentDirectory
        {
            get => _currentDirectory;
            set => SetProperty(ref _currentDirectory, value);
        }

        public ObservableCollection<SftpFileDetailInfo> FileList
        {
            get => _fileList;
            set => SetProperty(ref _fileList, value);
        }

        #endregion

        #region Events

        private readonly Subject<Exception> _errorOccurred = new();
        public IObservable<Exception> ErrorOccurred => _errorOccurred;

        private readonly Subject<Exception> _caughtErrorOccurred = new();
        public IObservable<Exception> CaughtOccurred => _caughtErrorOccurred;

        #endregion

        public void Open(string address, int port = 22)
        {
            _sftpServerConnector = new SftpServerConnector(address, port);
        }

        public async Task Connect(string userName, string password, string workingDirectory = null)
        {
            _sftpServerConnector.SetLoginInformation(userName, password);
            await Connect(workingDirectory);
        }

        public async Task Connect(string userName, string passPhrase, string keyPath, string workingDirectory = null)
        {
            _sftpServerConnector.SetLoginInformation(userName, passPhrase, keyPath);
            await Connect(workingDirectory);
        }

        private async Task Connect(string workingDirectory)
        {
            if (await Task.Factory.StartNew(() => _sftpServerConnector.Connect()))
            {
                if (!string.IsNullOrEmpty(workingDirectory))
                    _sftpServerConnector.ChangeDirectory(workingDirectory);

                _pageForwardHistory = new Stack<string>();
                _pageBackHistory = new Stack<string>();
                var files = await Task.Factory.StartNew(GetFileList);
                ResetDirectoryInfo(files);
            }
        }

        public void NewDirectory()
        {
            _pageForwardHistory = new Stack<string>();
            _pageBackHistory.Push(_sftpServerConnector.WorkingDirectory);
        }

        public async Task ChangeDirectory(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            try
            {
                _sftpServerConnector.ChangeDirectory(path);
            }
            catch (Renci.SshNet.Common.SftpPathNotFoundException e)
            {
                _caughtErrorOccurred.OnNext(e);
                return;
            }
            catch (Renci.SshNet.Common.SftpPermissionDeniedException e)
            {
                _caughtErrorOccurred.OnNext(e);
                return;
            }

            var files = await Task.Factory.StartNew(GetFileList);
            ResetDirectoryInfo(files);
        }

        public Stream DownloadFile(string path)
        {
            var ms = new MemoryStream();
            _sftpServerConnector.Download(path, ms);

            ms.Position = 0;
            return ms;
        }

        public void MakeDirectory()
        {
            FileList.Add(new SftpFileDetailInfo
            {
                ImageSource = ImageLoader.LoadFromResource("SavannahManagerStyleLib.Resources.Files.DirectoryIcon.png"),
                IsDirectory = true,
                IsEditMode = true,
                LostFocusAction = async info =>
                {
                    if (string.IsNullOrEmpty(info.Name))
                    {
                        FileList.Remove(info);
                        return;
                    }

                    _sftpServerConnector.MakeDirectory($"{CurrentDirectory}/{info.Name}");

                    var files = await Task.Factory.StartNew(GetFileList);
                    ResetDirectoryInfo(files);
                }
            });
        }

        public async Task Rename(SftpFileDetailInfo info)
        {
            if (_sftpServerConnector is not { IsConnected: true } || info.Parent == null)
                return;

            var oldPath = $"{info.Parent}/{Path.GetFileName(info.FullPath)?.Replace("\\", "/")}";
            var newPath = $"{info.Parent}/{info.Name}";

            _sftpServerConnector.Rename(oldPath, newPath);

            info.FullPath = newPath;
            info.Name = Path.GetFileName(newPath);
            var files = await Task.Factory.StartNew(GetFileList);
            ResetDirectoryInfo(files);
        }

        public async Task Delete(string path)
        {
            _sftpServerConnector.Delete(path);

            var files = await Task.Factory.StartNew(GetFileList);
            ResetDirectoryInfo(files);
        }

        public void DoOpenAction(string path)
        {
            if (OpenCallbackAction == null)
                return;

            var stream = IsOpenStream ? DownloadFile(path) : null;
            OpenCallbackAction?.Invoke(new SftpOpenStreamItem(stream, path));
        }

        public void DoSaveAction(string path)
        {
            var data = SaveDataFunction?.Invoke();
            if (data == null)
                return;

            using var stream = new MemoryStream(data);
            _sftpServerConnector.Upload(path, stream);
        }

        public async Task DirectoryForward()
        {
            if (_pageForwardHistory.Count <= 0)
                return;

            var name = _pageForwardHistory.Pop();
            _pageBackHistory.Push(_sftpServerConnector.WorkingDirectory);
            await ChangeDirectory(name);
        }

        public async Task DirectoryBack()
        {
            if (_pageBackHistory.Count <= 0)
                return;

            var name = _pageBackHistory.Pop();
            _pageForwardHistory.Push(_sftpServerConnector.WorkingDirectory);
            await ChangeDirectory(name);
        }

        public async Task TraceBackPage()
        {
            _pageForwardHistory.Clear();
            _pageBackHistory.Push(_sftpServerConnector.WorkingDirectory);
            await ChangeDirectory($"{_sftpServerConnector.WorkingDirectory}/..");
        }

        public string GetFullPath(string name)
        {
            if (name == null)
                return null;

            var directoryPath = CurrentDirectory.TrimEnd('/');
            return $"{directoryPath}/{name}";
        }

        private List<SftpServerConnector.SftpFileInfo> GetFileList()
        {
            var files = _sftpServerConnector.GetItems().Where(x =>
            {
                var name = Path.GetFileName(x.Path) ?? "";
                return !name.StartsWith(".");
            }).ToList();

            return files;
        }

        private void ResetDirectoryInfo(List<SftpServerConnector.SftpFileInfo> files)
        {
            var directoryImage = ImageLoader.LoadFromResource("SavannahManagerStyleLib.Resources.Files.DirectoryIcon.png");
            var fileImage = ImageLoader.LoadFromResource("SavannahManagerStyleLib.Resources.Files.FileIcon.png");
            
            FileList.Clear();

            foreach (var sftpFileInfo in files.Where(x => x.IsDirectory))
            {
                var item = new SftpFileDetailInfo
                {
                    ImageSource = directoryImage,
                    FullPath = sftpFileInfo.Path,
                    Name = Path.GetFileName(sftpFileInfo.Path),
                    IsDirectory = true,
                    LostFocusAction = async (info) => await Rename(info)
                };
                FileList.Add(item);
            }

            if (DirectoryMode == FileDirectoryMode.File)
            {

                IEnumerable<SftpServerConnector.SftpFileInfo> targetFiles;
                if (TargetExtensions != null && TargetExtensions.Any())
                {
                    targetFiles = files.Where(x => !x.IsDirectory && TargetExtensions.Contains(Path.GetExtension(x.Path)));
                }
                else
                {
                    targetFiles = files.Where(x => !x.IsDirectory);
                }

                foreach (var sftpFileInfo in targetFiles)
                {
                    var item = new SftpFileDetailInfo
                    {
                        ImageSource = fileImage,
                        FullPath = sftpFileInfo.Path,
                        Name = Path.GetFileName(sftpFileInfo.Path)
                    };
                    FileList.Add(item);
                }
            }

            CurrentDirectory = _sftpServerConnector.WorkingDirectory;
            CanGoBack = _pageBackHistory.Count > 0;
            CanGoForward = _pageForwardHistory.Count > 0;
        }

        ~FileSelectorModel()
        {
            Dispose();
        }

        public void Dispose()
        {
            _sftpServerConnector?.Dispose();
        }
    }
}
