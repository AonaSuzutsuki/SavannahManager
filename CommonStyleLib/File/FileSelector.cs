using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KimamaLib.File
{
    public class FileSelector
    {
        public enum FileSelectorType
        {
            Read,
            Write
        }

        public static string GetFilePath(string InitialDirectory, string Filter, string FileName, FileSelectorType type)
        {
            if (type == FileSelectorType.Read)
                return GetReadFilePath(InitialDirectory, Filter, FileName);
            else
                return GetWriteFilePath(InitialDirectory, Filter, FileName);

        }

        private static string GetReadFilePath(string InitialDirectory, string Filter, string FileName)
        {
            var openFileDialog = new OpenFileDialog()
            {
                FilterIndex = 1,
                FileName = FileName,
                Filter = Filter,
                InitialDirectory = InitialDirectory
            };
            bool? result = openFileDialog.ShowDialog();
            if (result == true)
                return openFileDialog.FileName;

            return string.Empty;
        }
        private static string GetWriteFilePath(string InitialDirectory, string Filter, string FileName)
        {
            var openFileDialog = new SaveFileDialog()
            {
                FilterIndex = 1,
                FileName = FileName,
                Filter = Filter,
                InitialDirectory = InitialDirectory
            };
            bool? result = openFileDialog.ShowDialog();
            if (result == true)
                return openFileDialog.FileName;

            return string.Empty;
        }
    }
}
