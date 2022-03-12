namespace SavannahManagerStyleLib.ViewModels.SshFileSelector;

public class ConnectionInformation
{
    public string Address { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public bool IsPassword { get; set; }
    public string Password { get; set; }
    public string KeyPath { get; set; }
    public string PassPhrase { get; set; }
    public string DefaultWorkingDirectory { get; set; }
}