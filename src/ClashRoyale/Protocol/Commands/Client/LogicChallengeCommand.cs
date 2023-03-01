﻿//using System;
using ClashRoyale.Logic;
using ClashRoyale.Logic.Clan.StreamEntry.Entries;
using ClashRoyale.Protocol.Messages.Server;
using ClashRoyale.Utilities.Netty;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Commands.Client
{
    public class LogicChallengeCommand : LogicCommand
    {
        public LogicChallengeCommand(Device device, IByteBuffer buffer) : base(device, buffer)
        {
        }

        public string Message { get; set; }
        public int Arena { get; set; }
        public int GameMode { get; set; }
        public int GameModeEvent { get; set; }

        public override void Decode()
        {
            Message = Reader.ReadScString();
            Reader.ReadBoolean();

            Reader.ReadVInt(); // ClassId
            GameMode = Reader.ReadVInt(); // InstanceId

            Reader.ReadVInt();
            Reader.ReadVInt();

            Reader.ReadVInt();
            Reader.ReadVInt();

            Reader.ReadVInt();

            Arena = Reader.ReadVInt();
            
            Reader.ReadVInt();
            Reader.ReadVInt();
            Reader.ReadVInt();
            Reader.ReadVInt();
            GameModeEvent = Reader.ReadVInt();
        }

        public override async void Process()
        {
            if (GameMode == 0)
            {
                var home = Device.Player.Home;
                var alliance = await Resources.Alliances.GetAllianceAsync(home.AllianceInfo.Id);

                if (alliance == null) return;
                
                var GameModeEvents = new string[] {
                    "rnddeck", "200hp", "lowelixir", ""
                };
                
                var entry = new ChallengeStreamEntry
                {
                    Message = (GameModeEvent == -1 ? Message : GameModeEvents[GameModeEvent - 5000]),
                    Arena = Arena + 1
                };

                entry.SetSender(Device.Player);
                alliance.AddEntry(entry);
            }
            else
            {
                await new MatchmakeFailedMessage(Device).SendAsync();
                await new CancelMatchmakeDoneMessage(Device).SendAsync();
            }
        }
    }
}
