using Common.Network;
using Common.Network.Extension;
using GameServer.Tool;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Service
{
    public class ClientSession
    {
        private System.Net.Sockets.Socket _socket;

        public event EventHandler<ObjectDisposedException?>? SessionClosed;
        public event EventHandler<Packet>? PacketReceived;
        public event EventHandler<Exception>? ErrorOccur;


        public ClientSession(System.Net.Sockets.Socket socket)
        {
            _socket = socket;
        }

        public void Start()
        {
            Task.Run(async () =>
            {
                try
                {
                    while (true)
                    {
                        var size = await _socket.ReadInt32Async();
                        Debug.Assert(size > 0 && size < NetConfig.MaxPacketSize);
                        var buffer = await _socket.ReadAsync(size);
                        PacketReceived?.Invoke(this, new(buffer));
                    }
                }
                catch (ObjectDisposedException ex)
                {
                    SessionClosed?.Invoke(this, ex);
                }
                catch (Exception ex)
                {
                    ErrorOccur?.Invoke(this, ex);
                }
            });
        }


        public void Close()
        {
            try
            {
                _socket.Shutdown(SocketShutdown.Both);
            }
            catch (ObjectDisposedException)
            {
                Global.Logger.Warn($"尝试关闭已经关闭的socket!");
                return;
            }
            catch (Exception ex)
            {
                ErrorOccur?.Invoke(this, ex);
            }
            finally
            {
                _socket.Close();

            }
        }
    }
}
