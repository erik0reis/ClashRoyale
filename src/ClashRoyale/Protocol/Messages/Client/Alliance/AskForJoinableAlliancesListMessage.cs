using System.Collections.Generic;
using ClashRoyale.Logic;
using ClashRoyale.Protocol.Messages.Server;
using DotNetty.Buffers;
using ClashRoyale;
using ClashRoyale.Database;

namespace ClashRoyale.Protocol.Messages.Client.Alliance
{
    public class AskForJoinableAlliancesListMessage : PiranhaMessage
    {
        public AskForJoinableAlliancesListMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 14303;
        }

        public override async void Process()
	{
	    long alliancecount = await AllianceDb.CountAsync();
	    List<Logic.Clan.Alliance> list = new List<Logic.Clan.Alliance>(0);
	    if (alliancecount > 0L)
	    {
	        list = Resources.Leaderboard.GlobalAllianceRanking;
	    }
            await new JoinableAllianceListMessage(Device)
            {
                Alliances = list
            }.SendAsync();
        }
    }
}
