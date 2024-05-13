using System;
using System.IO;
using System.Reflection;

namespace GameServer.Tool
{
    public static class ResourceHelper
    {
        public static string LoadFile(string path)
        {
            string exePath = Assembly.GetExecutingAssembly().Location;
            string exeDirectory = Path.GetDirectoryName(exePath);
            string content = File.ReadAllText(Path.Join(exeDirectory, path));
            return content;
        }
    }
}
