using ClashRoyale.Logic;
using ClashRoyale.Protocol.Commands.Server;
using ClashRoyale.Protocol.Messages.Server;
using ClashRoyale.Utilities.Netty;
using DotNetty.Buffers;
using System.IO;

namespace ClashRoyale.Protocol.Messages.Client.Home
{
    public class ChangeAvatarNameMessage : PiranhaMessage
    {
        public ChangeAvatarNameMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 10212;
        }

        public string Name { get; set; }

        public override void Decode()
        {
            Name = Reader.ReadScString();
        }

        public override async void Process()
        {
            if (string.IsNullOrEmpty(Name)) return;
            if (Name.Length < 2 || Name.Length > 15) return;

            var home = Device.Player.Home;
            var oldname = home.Name;
            //if (home.NameSet >= 2) return;

            home.Name = Name;

            var info = Device.Player.Home.AllianceInfo;

            if (info.HasAlliance)
            {
                var alliance = await Resources.Alliances.GetAllianceAsync(info.Id);

                alliance.GetMember(home.Id).Name = Name;

                alliance.Save();
            }

            File.WriteAllText("lastlog.txt", "INFO: player " + oldname + " (" + Device.Player.Home.Id + ") has changed his name to " + Name);
            
            await new AvailableServerCommand(Device)
            {
                Command = new LogicChangeNameCommand(Device)
                {
                    NameSet = home.NameSet
                }
            }.SendAsync();

            home.NameSet++;

            Device.Player.Save();
        }
    }
}
