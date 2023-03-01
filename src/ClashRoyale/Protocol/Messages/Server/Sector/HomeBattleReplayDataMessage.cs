using System.IO;
﻿using System.Text;
using ClashRoyale.Logic;
using ClashRoyale.Utilities.Netty;
using ClashRoyale.Utilities.Compression.ZLib;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class HomeBattleReplayDataMessage : PiranhaMessage
    {
        public HomeBattleReplayDataMessage(Device device) : base(device)
        {
            Id = 24114;
            Device.CurrentState = Device.State.Battle;
        }

        public override void Encode()
        {
            var replay = File.ReadAllText("replay.json");
            var data = Encoding.UTF8.GetBytes(replay);
            var compressed = ZlibStream.CompressBuffer(data, CompressionLevel.Default);

            Writer.WriteVInt(0);
            
            Writer.WriteCompressedString(replay);
        }
    }
}
