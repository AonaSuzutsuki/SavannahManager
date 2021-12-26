using System.Text;
using CommonExtensionLib.Extensions;
using SshConsole.Parser;

namespace SshConsole;

public abstract class AbstractProgram : IProgram
{
    protected Dictionary<string, Tuple<Action<CmdParser>, string, string>> _actionMap;
    protected Dictionary<string, Tuple<Func<CmdParser, bool>, string, string>> _funcMap;

    protected AbstractProgram()
    {
        _funcMap = new Dictionary<string, Tuple<Func<CmdParser, bool>, string, string>>()
        {
            { "exit", new Tuple<Func<CmdParser, bool>, string, string>((parser) => false, "[]", "Exit this program.") },
        };

        _actionMap = new Dictionary<string, Tuple<Action<CmdParser>, string, string>>()
        {
            { "gc", new Tuple<Action<CmdParser>, string, string>(parser => GC.Collect(), "[]", "Run GC.Collect.") },
            { "help", new Tuple<Action<CmdParser>, string, string>(ShowHelp, "[] / [comamnd]", "Show the help.") }
        };
    }

    protected void ShowHelp(CmdParser parser)
    {
        var sb = new StringBuilder();
        var command = parser.GetAttribute("command") ?? parser.GetAttribute(0);
        if (command != null)
        {
            if (_funcMap.ContainsKey(command))
            {
                var tuple = _funcMap[command];
                sb.AppendFormat("  {0}{1}{2}{3}{4}\n", command, command.MakeSpace(15), tuple.Item2, MakeSpace(tuple.Item2, 55), tuple.Item3);
            }
            if (_actionMap.ContainsKey(command))
            {
                var tuple = _actionMap[command];
                sb.AppendFormat("  {0}{1}{2}{3}{4}\n", command, command.MakeSpace(15), tuple.Item2, MakeSpace(tuple.Item2, 55), tuple.Item3);
            }
        }
        else
        {
            foreach (var tuple in _funcMap)
            {
                sb.AppendFormat("  {0}{1}{2}{3}{4}\n", tuple.Key, tuple.Key.MakeSpace(15), tuple.Value.Item2, tuple.Value.Item2.MakeSpace(55), tuple.Value.Item3);
            }

            foreach (var tuple in _actionMap)
            {
                sb.AppendFormat("  {0}{1}{2}{3}{4}\n", tuple.Key, tuple.Key.MakeSpace(15), tuple.Value.Item2, tuple.Value.Item2.MakeSpace(55), tuple.Value.Item3);
            }
        }

        Console.WriteLine(sb);
    }

    protected static string MakeSpace(string text, int max)
    {
        int count = max - text.Length;
        count = count < 0 ? 1 : count;
        var sb = new StringBuilder();
        for (int i = 0; i < count; i++)
        {
            sb.Append(" ");
        }
        return sb.ToString();
    }

    public virtual bool Parse(string cmd)
    {
        var parser = new CmdParser(cmd);
        var command = parser.Command;
        if (_funcMap.ContainsKey(command))
            return _funcMap[command].Item1(parser);
        if (_actionMap.ContainsKey(command))
            _actionMap[command].Item1(parser);
        return true;
    }

    public abstract void Dispose();
}