
namespace CommonLib.Ini
{
    public class IniLoader
    {
        private string fileFullPath;

        public IniLoader(string fileFullPath)
        {
            this.fileFullPath = fileFullPath;
        }

        public string GetValue(string ClassName, string KeyName, string NoValue)
        {
            return IniFileHandler.GetString(ClassName, KeyName, NoValue, fileFullPath);
        }
        public int GetValue(string ClassName, string KeyName, int NoValue)
        {
            return IniFileHandler.GetInteger(ClassName, KeyName, NoValue, fileFullPath);
        }
        public bool GetValue(string ClassName, string KeyName, bool NoValue)
        {
            return IniFileHandler.GetBoolean(ClassName, KeyName, NoValue, fileFullPath);
        }

        public void SetValue(string ClassName, string KeyName, object BodyName)
        {
            IniFileHandler.WriteString(ClassName, KeyName, BodyName.ToString(), fileFullPath);
        }
    }
}
