using ClashRoyale.Logic;
using ClashRoyale.Utilities.Netty;
using ClashRoyale.Utilities.Utils;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class TournamentListMessage : PiranhaMessage
    {
        public TournamentListMessage(Device device) : base(device)
        {
            Id = 26101;
        }
        
        public override void Encode()
        {
            Writer.WriteVInt(0);
        }
    }
}
