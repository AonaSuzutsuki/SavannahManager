
namespace CommonLib.Ini
{
    public class IniLoader
    {
        private string fileFullPath;

        public IniLoader(string fileFullPath)
        {
            this.fileFullPath = fileFullPath;
        }
        
        public string GetValue(string className, string keyName, string noValue)
        {
            return IniFileHandler.GetString(className, keyName, noValue, fileFullPath);
        }
        public int GetValue(string className, string keyName, int noValue)
        {
            return IniFileHandler.GetInteger(className, keyName, noValue, fileFullPath);
        }
        public bool GetValue(string className, string keyName, bool noValue)
        {
            return IniFileHandler.GetBoolean(className, keyName, noValue, fileFullPath);
        }

        public void SetValue<T>(string className, string keyName, T value)
        {
            IniFileHandler.WriteString(className, keyName, value.ToString(), fileFullPath);
        }
    }
}
