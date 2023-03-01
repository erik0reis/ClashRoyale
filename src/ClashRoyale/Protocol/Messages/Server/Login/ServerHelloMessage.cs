using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClashRoyale.Logic;
using ClashRoyale.Utilities.Netty;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class ServerHelloMessage : PiranhaMessage
    {
        public ServerHelloMessage(Device device) : base(device)
        {
            Id = 20100;
        }

        public override void Decode()
        {
            
        }
        public override void Encode()
        {
            Writer.WriteBytes(new byte[24]);
        }
    }
}
