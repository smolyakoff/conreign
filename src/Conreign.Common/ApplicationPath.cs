using System;
using System.IO;
using System.Reflection;

public static class ApplicationPath
{
    public static string CurrentExecutable
    {
        get
        {
            var assembly = Assembly.GetCallingAssembly();
            var uri = new Uri(assembly.CodeBase);
            return uri.LocalPath;
        }
    }

    public static string CurrentDirectory => Path.GetDirectoryName(CurrentExecutable);
}