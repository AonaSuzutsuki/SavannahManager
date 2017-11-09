using System.Text;
using System.Runtime.InteropServices;

namespace CommonLib.Ini
{
    public static class IniFileHandler
    {
        [DllImport("KERNEL32.DLL")]
        private static extern int GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault,
            StringBuilder lpReturnedString, int nSize, string lpFileName);

        [DllImport("KERNEL32.DLL", EntryPoint = "GetPrivateProfileStringA")]
        private static extern int GetPrivateProfileStringByByteArray(string lpAppName, string lpKeyName,
            string lpDefault, byte[] lpReturnedString, uint nSize, string lpFileName);

        [DllImport("KERNEL32.DLL")]
        private static extern int GetPrivateProfileInt(string lpAppName, string lpKeyName, int nDefault,
            string lpFileName);

        [DllImport("KERNEL32.DLL")]
        private static extern uint WritePrivateProfileString(string lpAppName, string lpKeyName, string lpString,
            string lpFileName);
        
        /// <summary>
        /// iniの値を文字列型で取得します。
        /// </summary>
        /// <param name="ClassName">クラス名</param>
        /// <param name="KeyName">キー名</param>
        /// <param name="NoStr">デフォルト値</param>
        /// <param name="PathName">iniファイルの場所</param>
        /// <returns>取得したiniの値</returns>
        public static string GetString(string ClassName, string KeyName, string NoStr, string PathName)
        {
            StringBuilder sb = new StringBuilder(1024);
            int returnInt;
            returnInt = GetPrivateProfileString(ClassName, KeyName, NoStr, sb, sb.Capacity, PathName);
            return sb.ToString();
        }

        /// <summary>
        /// iniの値を整数型で取得します。
        /// </summary>
        /// <param name="ClassName">クラス名</param>
        /// <param name="KeyName">キー名</param>
        /// <param name="NoStr">デフォルト値</param>
        /// <param name="PathName">iniファイルの場所</param>
        /// <returns>取得したiniの値</returns>
        public static int GetInteger(string ClassName, string KeyName, int NoStr, string PathName)
        {
            var str = GetString(ClassName, KeyName, NoStr.ToString(), PathName);

            int.TryParse(str, out int intStr);
            return intStr;
        }

        /// <summary>
        /// iniの値をbool型で取得します。
        /// </summary>
        /// <param name="ClassName">クラス名</param>
        /// <param name="KeyName">キー名</param>
        /// <param name="NoStr">デフォルト値</param>
        /// <param name="PathName">iniファイルの場所</param>
        /// <returns>取得したiniの値</returns>
        public static bool GetBoolean(string ClassName, string KeyName, bool NoStr, string PathName)
        {
            var str = GetString(ClassName, KeyName, NoStr.ToString(), PathName);

            bool.TryParse(str, out bool returnbool);
            return returnbool;
        }

        /// <summary>
        /// iniに値を書き込みます。
        /// </summary>
        /// <param name="ClassName">クラス名</param>
        /// <param name="KeyName">キー名</param>
        /// <param name="NoStr">デフォルト値</param>
        /// <param name="PathName">iniファイルの場所</param>
        public static void WriteString(string ClassName, string KeyName, string BodyName, string PathName)
        {
            WritePrivateProfileString(ClassName, KeyName, BodyName, PathName);
        }
    }
}
