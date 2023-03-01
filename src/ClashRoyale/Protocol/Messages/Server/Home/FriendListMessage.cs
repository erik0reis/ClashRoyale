using System;
using System.Collections.Generic;
using System.Linq;
using ClashRoyale.Logic;
using ClashRoyale.Utilities.Netty;
using ClashRoyale;
using ClashRoyale.Files;
using DotNetty.Buffers;
using SharpRaven.Data;
using ClashRoyale.Files.CsvLogic;
using ClashRoyale.Extensions;

namespace ClashRoyale.Protocol.Messages.Server.Home
{
    public class FriendListMessage : PiranhaMessage
    {
        public FriendListMessage(Device device) : base(device)
        {
            Id = 20105;
        }

        public override void Decode()
        {
            
        }

        public override void Encode()
        {
            var friends = Resources.Leaderboard.GlobalPlayerRanking;
            Writer.WriteVInt(friends.Count);
            Writer.WriteVInt(friends.Count);
            
            foreach (Player Friend in friends)
            {
                Writer.WriteLong(Friend.Home.Id);

                Writer.WriteBoolean(true);

                Writer.WriteLong(Friend.Home.Id);

                Writer.WriteScString(Friend.Home.Name);
                Writer.WriteScString("");
                Writer.WriteScString("");

                Writer.WriteVInt(Friend.Home.ExpLevel);
                Writer.WriteVInt(Friend.Home.Arena.Trophies);

                Writer.WriteBoolean(false); // Clan.

                // this.Data.AddString(null);
                // this.Data.AddString(null);

                // this.Data.AddVInt(1);
                
                Writer.WriteData(Friend.Home.Arena.GetCurrentArenaData());

                Writer.WriteScString(null);
                Writer.WriteScString(null);
                
                Writer.WriteVInt(-1);

                Writer.WriteInt(0);
                Writer.WriteInt(0);
            }
        }
    }
}

