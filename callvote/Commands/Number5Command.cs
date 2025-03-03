﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandSystem;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using RemoteAdmin;
using UnityEngine;

namespace callvote.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    class Number5Command : ICommand
    {
        public string Command => "5";

        public string[] Aliases => new string[] { };

        public string Description => Plugin.Instance.Config.Localizations.DescriptionNumber5;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "";
			Player player = Server.GetPlayers<Player>().Where(sid => sid.UserId == ((CommandSender)sender).SenderId).First();
			if (sender is PlayerCommandSender)
            {
                var plr = sender as PlayerCommandSender;
                if (Plugin.Instance.Voting())
                {
                    response = Plugin.Instance.VoteHandler(player, 5);
                }
                else
                {
                    response = Plugin.Instance.Config.Localizations.NoVoteInProgress;
                }
            }
            return false;
        }
    }
}
