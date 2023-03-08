using System.IO;
﻿using System.Text;
using ClashRoyale.Logic;
using ClashRoyale.Utilities.Netty;

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

            Writer.WriteVInt(0);
            
            Writer.WriteCompressedString(replay);
        }
    }
}
