using System;
using System.Linq;
using ClashRoyale.Database;
using ClashRoyale.Logic;
using ClashRoyale.Logic.Clan.StreamEntry.Entries;
using ClashRoyale.Logic.Home.Decks;
using ClashRoyale.Protocol.Messages.Server;
using ClashRoyale.Utilities;
using ClashRoyale.Utilities.Netty;
using DotNetty.Buffers;
using System.IO;

namespace ClashRoyale.Protocol.Messages.Client.Alliance
{
    public class ChatToAllianceStreamMessage : PiranhaMessage
    {
        public ChatToAllianceStreamMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 14315;
        }

        public string Message { get; set; }

        public override void Decode()
        {
            Message = Reader.ReadScString();
        }

        public override async void Process()
        {
            var info = Device.Player.Home.AllianceInfo;
            if (!info.HasAlliance) return;

            var alliance = await Resources.Alliances.GetAllianceAsync(info.Id);
            if (alliance == null) return;
            File.WriteAllText("lastlog.txt", "INFO: player " + Device.Player.Home.Name + " (" + Device.Player.Home.Id + ") sent a message (" + Message + ") to clan " + Device.Player.Home.AllianceInfo.Name + "(" + info.Id + ")");
            if (Message.StartsWith('/'))
            {
                var cmd = Message.Split(' ');
                var cmdType = cmd[0];
                var cmdValue = 0;

                if (cmd.Length > 1)
                    if (Message.Split(' ')[1].Any(char.IsDigit))
                        int.TryParse(Message.Split(' ')[1], out cmdValue);

                switch (cmdType)
                {
                    /*case "/max":
                    {
                        var deck = Device.Player.Home.Deck;

                        //foreach (var card in Cards.GetAllCards())
                        //{
                            //deck.Add(card);
                            bool exclusive = false;
                            foreach (var deckcard in deck)
                            {     
		                foreach (var exclusivecard in Resources.Configuration.exclusiveCards)
		                {
	                            if (int.Parse(exclusivecard.Split("-")[0]) == deckcard.ClassId && int.Parse(exclusivecard.Split("-")[1]) == deckcard.InstanceId)
		                    {
                                        exclusive = true;
	                            }
	                        }
	                        if (!exclusive)
	                        {
                	            for (var i = 0; i < 100; i++)
                	            {
                	                deck.UpgradeCard(deckcard.ClassId, deckcard.InstanceId, true);
                	            }
	                        }
                            }
                        //}

                        await new ServerErrorMessage(Device)
                        {
                            Message = "Upgraded Cards"
                        }.SendAsync();

                        break;
                    }

                    case "/unlock":
                    {
                        var deck = Device.Player.Home.Deck;

                        foreach (var card in Cards.GetAllCards())
                        {
                            bool exclusive = false; 
                            foreach (var exclusivecard in Resources.Configuration.exclusiveCards)
                            {
                                if (int.Parse(exclusivecard.Split("-")[0]) == card.ClassId && int.Parse(exclusivecard.Split("-")[1]) == card.InstanceId)
                                {
                                    exclusive = true;
                                }
                            }
                            if (!exclusive)
                            {
                                deck.Add(card);
                            }
                        }

                        await new ServerErrorMessage(Device)
                        {
                            Message = "Added all cards"
                        }.SendAsync();

                        break;
                    }

                    case "/gold":
                    {
                        Device.Player.Home.Gold += cmdValue;
                        Device.Disconnect();
                        break;
                    }*/

                    case "/status":
                    {
                        await new ServerErrorMessage(Device)
                        {
                            Message =
                                $"Online Players: {Resources.Players.Count}\nTotal Players: {await PlayerDb.CountAsync()}\n1v1 Battles: {Resources.Battles.Count}\n2v2 Battles: {Resources.DuoBattles.Count}\nTotal Clans: {await AllianceDb.CountAsync()}\nUptime: {DateTime.UtcNow.Subtract(Resources.StartTime).ToReadableString()}"
                        }.SendAsync();

                        break;
                    }

                    /*case "/free":
                    {
                        Device.Player.Home.FreeChestTime = Device.Player.Home.FreeChestTime.Subtract(TimeSpan.FromMinutes(245));
                        Device.Disconnect();
                        break;
                    }*/

                        case "/replay":
                        {
                            await new HomeBattleReplayDataMessage(Device).SendAsync();
                            break;
                        }

                        /*case "/trophies":
                        {
                            if (cmdValue >= 0)
                                Device.Player.Home.Arena.AddTrophies(cmdValue);
                            else if (cmdValue < 0)
                                Device.Player.Home.Arena.RemoveTrophies(cmdValue);

                            Device.Disconnect();
                            break;
                        }*/
                }
            }
            else
            {
                var entry = new ChatStreamEntry
                {
                    Message = Message
                };

                entry.SetSender(Device.Player);

                alliance.AddEntry(entry);
            }
        }
    }
}
