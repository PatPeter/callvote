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
    class Number3Command : ICommand
    {
        public string Command => "3";

        public string[] Aliases => new string[] { };

        public string Description => "";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "";
			Player player = Server.GetPlayers<Player>().Where(sid => sid.UserId == ((CommandSender)sender).SenderId).First();
			if (sender is PlayerCommandSender)
            {
                var plr = sender as PlayerCommandSender;
                if (Plugin.Instance.Voting())
                {
                    response = Plugin.Instance.VoteHandler(player, 3);
                }
                else
                {
                    response = "No vote is in progress.";
                }
            }
            return false;
        }
    }
}
