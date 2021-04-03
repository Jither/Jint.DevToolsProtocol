using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Jint.DevToolsProtocol.Server
{
    public class ServerOptions
    {
        public string Name { get; private set; } = "JintDebugger";
        public string Version { get; private set; } = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public int Port { get; private set; } = 9222;
        public string HostName { get; private set; } = "127.0.0.1";
        public string FavIconUrl { get; private set; } = null;

        public string WebSocketHost => $"{HostName}:{Port}";
        public string WebSocketUri => $"ws://{WebSocketHost}/";
        public string HttpUri => $"http://{WebSocketHost}/";

        public ServerOptions UsePort(int port)
        {
            Port = port;
            return this;
        }

        public ServerOptions UseHostName(string hostName)
        {
            HostName = hostName;
            return this;
        }

        public ServerOptions UseName(string name, string version = null)
        {
            Name = name;
            if (version != null)
            {
                Version = version;
            }
            return this;
        }

        public ServerOptions UseFavIconUrl(string url)
        {
            FavIconUrl = url;
            return this;
        }
    }
}
