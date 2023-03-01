using System;
using ClashRoyale;
﻿using ClashRoyale.Logic;
using ClashRoyale.Logic.Battle;
using ClashRoyale.Protocol.Messages.Server;
using ClashRoyale.Utilities.Netty;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Commands.Client
{
    public class JoinTournamentCommand : LogicCommand
    {
        public JoinTournamentCommand(Device device, IByteBuffer buffer) : base(device, buffer)
        {
        }

        public override void Decode()
        {
            base.Decode();

/*
            Logger.Log(Reader.ReadVInt(), null);
            Logger.Log(Reader.ReadVInt(), null);
            Logger.Log(Reader.ReadVInt(), null);
            Logger.Log(Reader.ReadVInt(), null);
            Logger.Log(Reader.ReadVInt(), null);
            Logger.Log(Reader.ReadVInt(), null);
            Logger.Log(Reader.ReadVInt(), null);
            Logger.Log(Reader.ReadVInt(), null);
            Logger.Log(Reader.ReadVInt(), null);
            Logger.Log(Reader.ReadVInt(), null);*/
        }

        public override void Encode()
        {
           
        }
    }
}
