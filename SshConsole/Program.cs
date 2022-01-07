using System.Diagnostics;
using SshConsole.Parser;
using SvManagerLibrary.Ssh;

namespace SshConsole
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.Write("mode> ");
            var mode = Console.ReadLine() ?? "";
            IProgram program;

            switch (mode.ToLower())
            {
                case "ssh":
                    program = new SshProgram();
                    break;
                case "sftp":
                    program = new SftpProgram();
                    break;
                default:
                    return;
            }

            try
            {

                while (true)
                {
                    Console.Write("> ");
                    var cmd = Console.ReadLine() ?? "";
                    var sw = new Stopwatch();
                    sw.Start();

                    if (!program.Parse(cmd))
                        break;

                    sw.Stop();
                    var msec = sw.ElapsedMilliseconds;
                    Console.WriteLine($"{msec}ms");
                }
            }
            finally
            {
                program?.Dispose();
            }
        }
    }

    public class SftpProgram : AbstractProgram
    {

        private SftpServerConnector _connector;

        private bool _connected;
        private SshProgram.Mode _mode = SshProgram.Mode.Pass;

        public SftpProgram()
        {
            _actionMap = new Dictionary<string, Tuple<Action<CmdParser>, string, string>>(_actionMap)
            {
                { "auth", new Tuple<Action<CmdParser>, string, string>(ChangeMode, "[pass/key]", "default pass") },
                { "connect", new Tuple<Action<CmdParser>, string, string>(Connect, "[address] [port] [user] [pass] | [address] [port] [user] [pass] [key path]", "") },
                { "pwd", new Tuple<Action<CmdParser>, string, string>(ShowWorkingDirectory, "", "") },
                { "cd", new Tuple<Action<CmdParser>, string, string>(ChangeDirectory, "", "") },
                { "ls", new Tuple<Action<CmdParser>, string, string>(ShowFiles, "", "") }
            };
        }

        private void ChangeMode(CmdParser parser)
        {
            var mode = parser.GetAttribute(0) ?? "";
            switch (mode.ToLower())
            {
                case "pass":
                    _mode = SshProgram.Mode.Pass;
                    break;
                case "key":
                    _mode = SshProgram.Mode.Key;
                    break;
                default:
                    _mode = SshProgram.Mode.Pass;
                    break;
            }
        }

        private void Connect(CmdParser parser)
        {
            var address = parser.GetAttribute(0);
            var portText = parser.GetAttribute(1);
            var user = parser.GetAttribute(2);
            var pass = parser.GetAttribute(3);

            int.TryParse(portText, out var port);

            _connector = new SftpServerConnector(address, port);

            if (_mode == SshProgram.Mode.Pass)
            {
                _connector.SetLoginInformation(user, pass);
            }
            else
            {
                var keyPath = parser.GetAttribute(4);
                _connector.SetLoginInformation(user, pass, keyPath);
            }

            _connector.Connect();
            _connected = true;
        }

        private void ShowWorkingDirectory(CmdParser parser)
        {
            Console.WriteLine(_connector.WorkingDirectory);
        }

        private void ChangeDirectory(CmdParser parser)
        {
            var path = parser.GetAttribute(0) ?? "~/";
            _connector.ChangeDirectory(path);
        }

        private void ShowFiles(CmdParser parser)
        {
            var items = _connector.GetItems();
            foreach (var item in items.Where(x => x.IsDirectory))
            {
                Console.WriteLine($"{item.Path} {item.IsDirectory}");
            }
            foreach (var item in items.Where(x => !x.IsDirectory))
            {
                Console.WriteLine($"{item.Path} {item.IsDirectory}");
            }
        }

        public override void Dispose()
        {
            _connector?.Dispose();
        }
    }

    public class SshProgram : AbstractProgram
    {
        public enum Mode
        {
            Pass,
            Key
        }

        private bool _connected;

        private SshServerConnector _connector;
        private Mode _mode = Mode.Pass;

        public SshProgram()
        {
            _actionMap = new Dictionary<string, Tuple<Action<CmdParser>, string, string>>(_actionMap)
            {
                { "auth", new Tuple<Action<CmdParser>, string, string>(ChangeMode, "[pass/key]", "default pass") },
                { "connect", new Tuple<Action<CmdParser>, string, string>(Connect, "[address] [port] [user] [pass] | [address] [port] [user] [pass] [key path]", "") },
            };
        }

        public override bool Parse(string cmd)
        {
            if (!_connected || _funcMap.ContainsKey(cmd))
            {
                var parser = new CmdParser(cmd);
                var command = parser.Command;
                if (_funcMap.ContainsKey(command))
                    return _funcMap[command].Item1(parser);
                if (_actionMap.ContainsKey(command))
                    _actionMap[command].Item1(parser);
            }
            else
            {
                SendCommand(cmd);
            }
            return true;
        }

        private void SendCommand(string cmd)
        {
            if (_connector == null)
                return;
            
            _connector.WriteLine(cmd);
        }

        private void ChangeMode(CmdParser parser)
        {
            var mode = parser.GetAttribute(0) ?? "";
            switch (mode.ToLower())
            {
                case "pass":
                    _mode = Mode.Pass;
                    break;
                case "key":
                    _mode = Mode.Key;
                    break;
                default:
                    _mode = Mode.Pass;
                    break;
            }
        }

        private void Connect(CmdParser parser)
        {
            var address = parser.GetAttribute(0);
            var portText = parser.GetAttribute(1);
            var user = parser.GetAttribute(2);
            var pass = parser.GetAttribute(3);

            int.TryParse(portText, out var port);

            _connector = new SshServerConnector(address, port);
            _connector.SshDataReceived.Subscribe(stream =>
            {
                Console.WriteLine(stream.ReadToEnd());
            });

            if (_mode == Mode.Pass)
            {
                _connector.SetLoginInformation(user, pass);
            }
            else
            {
                var keyPath = parser.GetAttribute(4);
                _connector.SetLoginInformation(user, pass, keyPath);
            }

            _connector.Connect();
            _connected = true;
        }

        public override void Dispose()
        {
            _connector?.Dispose();
        }
    }
}