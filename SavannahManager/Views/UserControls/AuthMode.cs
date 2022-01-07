using System;

namespace _7dtd_svmanager_fix_mvvm.Views.UserControls;

public enum AuthMode
{
    Password,
    Key
}

public static class AuthModeExtensions
{
    public static int ToInt(this AuthMode authMode)
    {
        return (int)authMode;
    }

    public static AuthMode FromInt(this int value)
    {
        return value switch
        {
            0 => AuthMode.Password,
            1 => AuthMode.Key,
            _ => AuthMode.Password
        };
    }
}