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
using System.Windows.Media;
using SavannahManagerStyleLib.Models.Image;

namespace SavannahManagerStyleLib.Models.SshFileSelector
{
    public class SftpFileDetailInfo : SftpServerConnector.SftpFileInfo
    {
        public ImageSource ImageSource { get; set; }
        public string Name { get; set; }
        public string DateString { get; set; }
    }

    public class SftpOpenStreamItem
    {
        public Stream Stream { get; }

        public SftpOpenStreamItem(Stream stream)
        {
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

        public Action<SftpOpenStreamItem> OpenCallbackAction { get; set; }
        public Func<byte[]> SaveDataFunction { get; set; }

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

            _sftpServerConnector.ChangeDirectory(path);
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

        public void DoOpenAction(string path)
        {
            if (OpenCallbackAction == null)
                return;

            var stream = DownloadFile(path);
            OpenCallbackAction?.Invoke(new SftpOpenStreamItem(stream));
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
                    Path = sftpFileInfo.Path,
                    Name = Path.GetFileName(sftpFileInfo.Path),
                    IsDirectory = true
                };
                FileList.Add(item);
            }

            foreach (var sftpFileInfo in files.Where(x => !x.IsDirectory && Path.GetExtension(x.Path) == ".xml"))
            {
                var item = new SftpFileDetailInfo
                {
                    ImageSource = fileImage,
                    Path = sftpFileInfo.Path,
                    Name = Path.GetFileName(sftpFileInfo.Path)
                };
                FileList.Add(item);
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
