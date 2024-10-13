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
using System.Text.RegularExpressions;

namespace callvote.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    class CallVoteCommand : ICommand
    {
        public string Command => "callvote";

        public string[] Aliases => null;

        public string Description => Plugin.Instance.Config.Localizations.CallvoteUsage;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "";
			Player player = Server.GetPlayers<Player>().Where(sid => sid.UserId == ((CommandSender)sender).SenderId).First();
			if (sender is PlayerCommandSender)
            {
                var plr = sender as PlayerCommandSender;
                if (Misc.CheckPermission(sender, PlayerPermissions.AdminChat))
                {
                    var args = arguments.Array.Skip(1).ToArray();
                    //var args = arguments.Array;
                    string[] quotedArgs = Regex.Matches(string.Join(" ", args), "[^\\s\"\']+|\"([^\"]*)\"|\'([^\']*)\'")
                        .Cast<Match>()
                        .Select(m => m.Value)
                        .ToArray()
                        //.Skip(1)
                        .ToArray();
                    response = Plugin.Instance.CallvoteHandler(player, quotedArgs);
                }
                else
                {
                    response = "You do not have permission to run this command";
                }
            }
            return false;
        }
    }
}
