using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Currycomb.Gateway.ClientData
{
    public enum ConnectionState : int
    {
        Handshake,
        Status,
        Login,
        Play
    }
}
