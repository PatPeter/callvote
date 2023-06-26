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
    class forceresultCommand : ICommand
    {
        public string Command => "forceresult";

        public string[] Aliases => null;

        public string Description => "screw pat for putting this off";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "";
			Player player = Server.GetPlayers<Player>().Where(sid => sid.UserId == ((CommandSender)sender).SenderId).First();
			if (sender is PlayerCommandSender)
            {
                var plr = sender as PlayerCommandSender;
                if (Misc.CheckPermission(sender, PlayerPermissions.ServerConsoleCommands))
                {
                    var args = arguments.Array;
                    if (args.Length > 0)
                    {
                        int option;
                        if (int.TryParse(args[0], out option))
                        {
                            response = Plugin.Instance.Rigging(option);
                        }
                    }
                }
                //response = Plugin.Instance.StopvoteHandler(player);
            }
            return false;
        }
    }
}
