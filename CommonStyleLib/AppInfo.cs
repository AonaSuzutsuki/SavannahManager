using System.IO;

namespace CommonLib
{
    public static class AppInfo
    {
        private static System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();

        private static string location;
        private static string version;

        /// <summary>
        /// 実行ファイルの現在の場所を取得します。
        /// </summary>
        /// <returns>実行ファイルのディレクトリ絶対パス</returns>
        public static string GetAppPath()
        {
            if (string.IsNullOrEmpty(location))
                location = Path.GetDirectoryName(asm.Location);

            return location;
        }

        /// <summary>
        /// アプリケーションの現在のバージョンを取得します。
        /// </summary>
        /// <returns>アプリケーションの現在のバージョン</returns>
        public static string GetVer()
        {
            if (string.IsNullOrEmpty(version))
                version = asm.GetName().Version.ToString();
            
            return version;
        }
    }
}
