using ClashRoyale.Logic;
using ClashRoyale.Utilities.Netty;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class InboxListMessage : PiranhaMessage
    {

        public InboxListMessage(Device device) : base(device)
        {
            Id = 24445;
        }

        public override void Encode()
        {

            Writer.WriteInt(2);

            Writer.WriteScString("https://56f230c6d142ad8a925f-b174a1d8fb2cf6907e1c742c46071d76.ssl.cf2.rackcdn.com/inbox/ClashRoyale_logo_small.png");
            Writer.WriteScString("<c4>Clash Royale MOD</c>!"); //Title
            Writer.WriteScString("");//Description
            Writer.WriteScString("Visit Discord");//Button Name
            Writer.WriteScString("https://discord.com/invite/epAUfNH3pj");//Button Link
            Writer.WriteScString("");//Unk
            Writer.WriteScString("");//Unk
            Writer.WriteScString("");//Unk
            
            Writer.WriteScString("https://56f230c6d142ad8a925f-b174a1d8fb2cf6907e1c742c46071d76.ssl.cf2.rackcdn.com/inbox/ClashRoyale_logo_small.png");
            Writer.WriteScString("<c6>Clash Royale MOD</c>!"); //Title
            Writer.WriteScString("");//Description
            Writer.WriteScString("Download");//Button Name
            Writer.WriteScString("http://crmod.ga");//Button Link
            Writer.WriteScString("");//Unk
            Writer.WriteScString("");//Unk
            Writer.WriteScString("");//Unk
        }


    }

}
