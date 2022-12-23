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
    class StopVoteCommand : ICommand
    {
        public string Command => "stopvote";

        public string[] Aliases => null;

        public string Description => "screw pat for putting this off";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "";
			Player player = Server.GetPlayers<Player>().Where(sid => sid.UserId == ((CommandSender)sender).SenderId).First();
			if (sender is PlayerCommandSender)
            {
                var plr = sender as PlayerCommandSender;
                response = Plugin.Instance.StopvoteHandler(player);
            }
            return false;
        }
    }
}
