namespace _7dtd_svmanager_fix_mvvm.Views.PlayerController.Pages
{
    public class AddType
    {
        public enum Type
        {
            Admin,
            Whitelist
        }

        private Type type;
        public AddType(Type type)
        {
            this.type = type;
        }

        public string ToCommand()
        {
            string command = default;
            switch (type)
            {
                case Type.Admin:
                    command = "admin";
                    break;
                case Type.Whitelist:
                    command = "whitelist";
                    break;
            }

            return command;
        }
    }
}
