using ClashRoyale;
using ClashRoyale.Logic;
using ClashRoyale.Logic.Battle;
using ClashRoyale.Logic.Clan.StreamEntry.Entries;
using DotNetty.Buffers;
using ClashRoyale.Logic.Home.Decks.Items;
﻿using System.Collections.Generic;
using System.Linq;
using System;

namespace ClashRoyale.Protocol.Messages.Client.Alliance
{
    public class AcceptChallengeMessage : PiranhaMessage
    {
        public AcceptChallengeMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 14120;
        }

        public long EntryId { get; set; }

        public override void Decode()
        {
            EntryId = Reader.ReadLong();
        }

        public override async void Process()
        {
            var home = Device.Player.Home;
            var alliance = await Resources.Alliances.GetAllianceAsync(home.AllianceInfo.Id);
		
            if (!(alliance?.Stream.Find(e => e.Id == EntryId && e.StreamEntryType == 10) is ChallengeStreamEntry entry)
            ){
            return;
            }

            alliance.RemoveEntry(entry);

            var enemy = await Resources.Players.GetPlayerAsync(entry.SenderId);

            if (enemy.Device != null)
            {
                var battle = new LogicBattle(true, entry.Arena)
                {
                    Device.Player, enemy
                };
                
                if (entry.Message == "rnddeck")
                {
                    battle.player0deck = rnddeck();
                    battle.player1deck = rnddeck();
                    battle.player2deck = rnddeck();
                    battle.player3deck = rnddeck();
                    battle.customKingHitpoint = 2400;
                    battle.customPrincessHitpoint = 1400;
                }
                
                if (entry.Message == "lowelixir")
                {
                    battle.msbeforeregenmana = 5000;
                }
                
                if (entry.Message == "200hp")
                {
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

                Resources.Battles.Add(battle);

                Device.Player.Battle = battle;
                enemy.Battle = battle;

                battle.Start();
            }

            alliance.Save();

            // TODO: Update Entry + Battle Result + Card levels
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
