namespace SshConsole;

public interface IProgram : IDisposable
{
    bool Parse(string cmd);
}