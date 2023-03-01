using ClashRoyale.Logic;
using ClashRoyale.Protocol.Messages.Server;
using ClashRoyale.Protocol.Messages.Server.Home;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Messages.Client.Home
{
    public class AskForFriendListMessage : PiranhaMessage
    {
        public AskForFriendListMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 10104;
        }

        /*public override void Decode()
        {
            
        }*/

        /*public override async void Process()
        {
            //await new FriendListMessage(Device).SendAsync();
        }*/
    }
}
