using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Currycomb.AuthService
{
    class Program
    {
        static async Task Main(string[] args)
        {
            TcpListener listener = new(IPAddress.Any, 10001);

            while(true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();

            }
        }
    }

    class AuthService
    {

    }
}
