using System;
using ClashRoyale;
﻿using ClashRoyale.Logic;
using ClashRoyale.Logic.Battle;
using ClashRoyale.Protocol.Messages.Server;
using ClashRoyale.Utilities.Netty;
using DotNetty.Buffers;
using ClashRoyale.Logic.Home.Decks;
using ClashRoyale.Logic.Home.Decks.Items;
﻿using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace ClashRoyale.Protocol.Commands.Client
{
    public class StartTournamentMatchmakeCommand : LogicCommand
    {
        public StartTournamentMatchmakeCommand(Device device, IByteBuffer buffer) : base(device, buffer)
        {
        }

        public bool Is2V2 { get; set; }

        public override void Decode()
        {
            base.Decode();

            
            Reader.ReadVInt();
            Reader.ReadVInt();

            //Is2V2 = Reader.ReadBoolean();
        }

        public override void Process()
        {
            /*if (Is2V2)
            {
                //await new MatchmakeFailedMessage(Device).SendAsync();
                //await new CancelMatchmakeDoneMessage(Device).SendAsync();

                Device.Player.battlechannel = 1;
                var players = Resources.DuoBattles.Dequeue;
                if (players != null)
                {
                    players.Add(Device.Player);

                    var battle = new LogicBattle(false,Device.Player.Home.Arena.CurrentArena + 1, players);

                    Resources.DuoBattles.Add(battle);

                    foreach (var player in players)
                    {
                        player.Battle = battle;
                    }

                    battle.Start();
                }
                else
                {
                    Resources.DuoBattles.Enqueue(Device.Player);
                }
            }
            else
            {*/
                Player enemy = null;
                Device.Player.battlechannel = 1;
                Device.Player.Home.Deck.SwitchDeck(5);
                enemy = Resources.Battles.Dequeue(Device.Player.battlechannel);

                if (enemy != null)
                {
		    enemy.Home.Deck.SwitchDeck(5);
                    int currenttournamentindex = (int) ((DateTime.UtcNow).Subtract(new DateTime(Resources.Configuration.EventsStartDate[0], Resources.Configuration.EventsStartDate[1], Resources.Configuration.EventsStartDate[2])).TotalDays);
                    string currenttournamentinfo = Resources.Configuration.EventsGamemodes[currenttournamentindex];
                    string gamemode = currenttournamentinfo.Split(';')[0];
                    int arena = int.Parse(currenttournamentinfo.Split(';')[1]);
                    
                    
                    var battle = new LogicBattle(false, arena)
                    {
                        Device.Player, enemy
                    };
                    
                    Resources.Battles.Add(battle);

                    Device.Player.Battle = battle;
                    enemy.Battle = battle;

                    if (gamemode == "rnddeck"){
		    battle.player0deck = rnddeck();
                    battle.player1deck = rnddeck();
                    battle.player2deck = rnddeck();
                    battle.player3deck = rnddeck();
                    battle.customKingHitpoint = 2400;
                    battle.customPrincessHitpoint = 1400;
                    }
                    
                    
                    if (gamemode == "200hp") {
		    battle.customKingHitpoint = 200;
                    battle.customPrincessHitpoint = 200;
                    var deck = new List<Card>() {};
                    foreach (var card in Device.Player.Home.Deck.GetRange(0, 8))
                    {
                        var newcard = new Card(card.ClassId, card.InstanceId, false);
                        newcard.Level = 0;
                        if (newcard.ClassId == 28 || newcard.InstanceId == 32)
                        {
                            deck.Add(new Card(26, new Random().Next(30), false));
                        }
                        else
                        {
                            deck.Add(newcard);
                        }
                    }
                    battle.player0deck = deck.ToArray();
                    deck = new List<Card>() {};
                    foreach (var card in enemy.Home.Deck.GetRange(0, 8))
                    {
                        var newcard = new Card(card.ClassId, card.InstanceId, false);
                        newcard.Level = 0;
                        if (newcard.ClassId == 28 || newcard.InstanceId == 32)
                        {
                            deck.Add(new Card(26, new Random().Next(30), false));
                        }
                        else
                        {
                            deck.Add(newcard);
                        }
                    }
                    battle.player1deck = deck.ToArray();
                    }
                    
                    if (gamemode == "2.6") {
                         battle.player0deck = new Card[] { new Card(26, 21, false), new Card(26, 30, false), new Card(26, 38, false), new Card(26, 14, false), new Card(26, 10, false), new Card(27, 0, false), new Card(28, 0, false), new Card(28, 11, false)};
                        battle.player1deck = new Card[] { new Card(26, 21, false), new Card(26, 30, false), new Card(26, 38, false), new Card(26, 14, false), new Card(26, 10, false), new Card(27, 0, false), new Card(28, 0, false), new Card(28, 11, false)};
                        battle.customKingHitpoint = 1200;
                        battle.customPrincessHitpoint = 700;
                    }
                    
                    if (gamemode.StartsWith("firstcard-")){
                    int classid = int.Parse(gamemode.Split('-')[1]);
                    int instanceid = int.Parse(gamemode.Split('-')[2]);
                    var card = new Card(classid, instanceid, false);
                    card.Level = 0;
		    battle.player0deck = Device.Player.Home.Deck.GetRange(0, 8).ToArray();
		    battle.player0deck[0] = card;
                    battle.player1deck = enemy.Home.Deck.GetRange(0, 8).ToArray();
                    battle.player1deck[0] = card;
                    }
                    
                    
                    if (gamemode == "minideck") {
                         battle.player0deck = new Card[] { Device.Player.Home.Deck[0], Device.Player.Home.Deck[1], Device.Player.Home.Deck[2], Device.Player.Home.Deck[3], null, null, null, null};
                        battle.player1deck = new Card[] { enemy.Home.Deck[0], enemy.Home.Deck[1], enemy.Home.Deck[2], enemy.Home.Deck[3], null, null, null, null};
                    }
                    
                    if (gamemode == "megadeck") {
                        battle.player0deck = Cards.GetAllCards();
                        battle.player1deck = Cards.GetAllCards();
                    }
                    
                    battle.Start();
                }
                else
                {
                    Resources.Battles.Enqueue(Device.Player);
                }
            //}
        }
    
    
    Card[] rnddeck()
        {
            var deck = new List<Card>();
            deck.Add(chooserandomcard(new Card[] {
                new Card(26, 21, false),
                new Card(26, 24, false),
                new Card(26, 26, false),
                new Card(26, 32, false),
                new Card(26, 33, false),
                new Card(26, 36, false),
                new Card(26, 35, false),
                new Card(26, 43, false),
                new Card(26, 6, false),
                new Card(26, 56, false),
                new Card(28, 4, false),
                new Card(28, 10, false)
            }));
            deck.Add(chooserandomcard(new Card[] {
                new Card(26, 0, false),
                new Card(26, 3, false),
                new Card(26, 4, false),
                new Card(26, 9, false),
                new Card(26, 11, false),
                new Card(26, 29, false),
                new Card(26, 34, false)
            }));
            deck.Add(chooserandomcard(new Card[] {
                new Card(26, 1, false),
                new Card(26, 5, false),
                new Card(26, 7, false),
                new Card(26, 17, false),
                new Card(26, 19, false),
                new Card(26, 22, false),
                new Card(26, 23, false),
                new Card(26, 49, false),
                new Card(26, 55, false),
                new Card(26, 57, false)
            }));
            deck.Add(chooserandomcard(new Card[] {
                new Card(28, 1, false),
                new Card(28, 8, false),
                new Card(28, 11, false),
                new Card(28, 1, false)
            }));
            deck.Add(chooserandomcard(new Card[] {
                new Card(28, 0, false),
                new Card(28, 6, false),
                new Card(28, 10, false),
                new Card(28, 9, false),
                new Card(28, 9, false)
            }));
            deck.Add(chooserandomcard(new Card[] {
                new Card(26, 2, false),
                new Card(26, 8, false),
                new Card(26, 8, false),
                new Card(26, 10, false),
                new Card(26, 12, false),
                new Card(26, 13, false),
                new Card(26, 15, false),
                new Card(26, 16, false),
                new Card(26, 18, false),
                new Card(26, 27, false),
                new Card(26, 13, false),
                new Card(26, 28, false),
                new Card(26, 41, false)
            }));
            deck.Add(chooserandomcard(new Card[] {
                new Card(27, 0, false),
                new Card(27, 1, false),
                new Card(27, 2, false),
                new Card(27, 3, false),
                new Card(27, 4, false),
                new Card(27, 5, false),
                new Card(27, 6, false),
                new Card(27, 7, false),
                new Card(27, 8, false),
                new Card(27, 9, false),
                new Card(27, 10, false)
            }));
            deck.Add(chooserandomcard(new Card[] {
                new Card(28, 2, false),
                new Card(28, 5, false),
                new Card(28, 6, false),
                new Card(28, 13, false),
                new Card(28, 16, false)
            }));
            
            return deck.ToArray();
        }
        
        Card chooserandomcard(Card[] options)
        {
            var rnd = new Random();
            var card = options[rnd.Next(options.Length)];
            var cardlevel = 1;
            switch (card.GetRarityData.Name)
            {
                case "Common": { cardlevel = 12; break;}
                case "Rare": { cardlevel = 10; break;}
                case "Epic": { cardlevel = 7; break;}
                case "Legendary": { cardlevel = 4; break;}
            }
            card.Level = cardlevel;
            return card;
        }
    }
}
