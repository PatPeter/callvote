using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using callvote.VoteHandlers;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using RemoteAdmin;
using UnityEngine;

namespace callvote.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    class RigCommand : ICommand
    {
        public string Command => "rig";

        public string[] Aliases => null;

        public string Description => "";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            var args = arguments.Array;
            Player player = Player.Get(((CommandSender)sender).SenderId);

            if (!player.CheckPermission("cv.superadmin+"))
            {
                response = "You can't rig the system :trollface:";
                return false;
            }

            if (args.Length < 0)
            {
                response = "No arguments passed";
                return false;
            }

            if (!Plugin.Instance.CurrentVote.Options.ContainsKey(args[0]))
            {
                response = "Couldnt find Key";
            }

            VoteHandler.Rigging(args[0]);
            response = args[0];
            return true;
        }
    }
}
