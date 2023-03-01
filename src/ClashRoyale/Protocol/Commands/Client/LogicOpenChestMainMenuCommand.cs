using System;
using System.Collections.Generic;
using System.Linq;
using ClashRoyale.Files;
using ClashRoyale.Files.CsvLogic;
using ClashRoyale.Logic;
using ClashRoyale.Logic.Home.Chests.Items;
using ClashRoyale.Protocol.Commands.Server;
using ClashRoyale.Protocol.Messages.Server;
using ClashRoyale.Logic.Home.Decks;
using ClashRoyale.Logic.Home.Decks.Items;
using DotNetty.Buffers;
using ClashRoyale.Logic.Home.Chests;
using ClashRoyale.Utilities.Netty;

namespace ClashRoyale.Protocol.Commands.Client
{
    public class LogicOpenChestMainMenuCommand : LogicCommand
    {
        public LogicOpenChestMainMenuCommand(Device device, IByteBuffer buffer) : base(device, buffer)
        {
        }
        
        public int slot { get; set; }

        public override void Decode()
        {
             
        }
        public override async void Process()
        {
            slot = 0;
            var home = Device.Player.Home;
            /*
            var chestcards = Cards.GetAllCards().ToList();
            for (int i = 0; i < chestcards.Count(); i++)
            {
                chestcards[i].Count = 999999;
            }
            */
            var _spellsCharacters = Csv.Tables.Get(Csv.Files.SpellsCharacters).GetDatas().Cast<SpellsCharacters>()
                .Where(s => !s.NotInUse).ToArray();
            List<Card> chestcardlist = new List<Card>() {};
            /*if (slot == 0)
            {
               chestcardlist.Add(new Card(26, _spellsCharacters[_spellsCharacters.Length - 1].GetInstanceId(), true));
            }
            if (slot == 1)
            {*/
                for (int i = 0; i < 4; i ++)
                {                
                    if (home.chestsExists[i])
                    {
                        home.chestsExists[i] = false;
                        break;
                    }
                }
                var rnd = new Random();
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        var card = Cards.RandomByArena(Card.Rarity.Common, home.Arena.GetChestArenaNames());
                        if (card != null)
                        {
                            card.Count = 20;
                            chestcardlist.Add(card);
                        }
                    }
                    for (int j = 0; j < 2; j++)
                    {
                        var card = Cards.RandomByArena(Card.Rarity.Rare, home.Arena.GetChestArenaNames());
                        if (card != null)
                        {
                            card.Count = 12;
                            chestcardlist.Add(card);
                        }
                    }
                    for (int j = 0; j < 1; j++)
                    {
                        var card = Cards.RandomByArena(Card.Rarity.Epic, home.Arena.GetChestArenaNames());
                        if (card != null)
                        {
                            card.Count = 5;
                            chestcardlist.Add(card);
                        }
                    }
                    for (int j = 0; j < rnd.Next(3); j++)
                    {
                        var card = Cards.RandomByArena(Card.Rarity.Legendary, home.Arena.GetChestArenaNames());
                        if (card != null)
                        {
                            card.Count = 1;
                            chestcardlist.Add(card);
                        }
                    }
                //}
            }
            
            await new AvailableServerCommand(Device)
            {
                Command = new ChestDataCommand(Device)
                {
                    Chest = home.Chests.BuyChest(1, Chest.ChestType.Shop, chestcardlist.ToArray())
                }
            }.SendAsync();
        }
    }
}
