using CommonStyleLib.Models;
using SvManagerLibrary.Ssh;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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

    public class FileSelectorModel : ModelBase, IDisposable
    {
        private SftpServerConnector _sftpServerConnector;
        private Stack<string> _pageForwardHistory;
        private Stack<string> _pageBackHistory;

        private bool _canGoBack;
        private bool _canGoForward;
        private string _currentDirectory;
        private ObservableCollection<SftpFileDetailInfo> _fileList = new();

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

        public FileSelectorModel()
        {

        }

        public void Open()
        {
            _sftpServerConnector = new SftpServerConnector("");
            _sftpServerConnector.SetLoginInformation("", "");
            if (_sftpServerConnector.Connect())
            {
                _pageForwardHistory = new Stack<string>();
                _pageBackHistory = new Stack<string>();
                ResetDirectoryInfo();
            }
        }

        public void NewDirectory()
        {
            _pageForwardHistory = new Stack<string>();
            _pageBackHistory.Push(_sftpServerConnector.WorkingDirectory);
        }

        public void ChangeDirectory(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            _sftpServerConnector.ChangeDirectory(path);
            ResetDirectoryInfo();
        }

        public void DirectoryForward()
        {
            if (_pageForwardHistory.Count <= 0)
                return;

            var name = _pageForwardHistory.Pop();
            _pageBackHistory.Push(_sftpServerConnector.WorkingDirectory);
            ChangeDirectory(name);
        }

        //public void DirectoryBack()
        //{
        //    _pageForwardHistory.Push(_sftpServerConnector.WorkingDirectory);
        //    ChangeDirectory($"{_sftpServerConnector.WorkingDirectory}/..");
        //}

        public void DirectoryBack()
        {
            if (_pageBackHistory.Count <= 0)
                return;

            var name = _pageBackHistory.Pop();
            _pageForwardHistory.Push(_sftpServerConnector.WorkingDirectory);
            ChangeDirectory(name);
        }

        public void TraceBackPage()
        {
            _pageForwardHistory.Clear();
            _pageBackHistory.Push(_sftpServerConnector.WorkingDirectory);
            ChangeDirectory($"{_sftpServerConnector.WorkingDirectory}/..");
        }

        public void ResetDirectoryInfo()
        {
            var files = _sftpServerConnector.GetItems().Where(x =>
            {
                var name = Path.GetFileName(x.Path) ?? "";
                return !name.StartsWith(".");
            });

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
