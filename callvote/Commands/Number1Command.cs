using System;
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
    class Number1Command : ICommand
    {
        public string Command => "1";

        public string[] Aliases => new string[] { "yes", "Yes" };

        public string Description => Plugin.Instance.Config.Localizations.DescriptionNumber1;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "";
			Player player = Server.GetPlayers<Player>().Where(sid => sid.UserId == ((CommandSender)sender).SenderId).First();
			if (sender is PlayerCommandSender)
            {
                var plr = sender as PlayerCommandSender;
                if (Plugin.Instance.Voting())
                {
                    response = Plugin.Instance.VoteHandler(player, 1);
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
