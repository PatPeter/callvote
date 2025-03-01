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

    public class VoteCommand : CommandHandler, ICommand
    {
        public string Command { get; set; }

        public string[] Aliases { get; } = new string[0];

        public string Description { get; } = "";

        public override IEnumerable<ICommand> AllCommands => base.AllCommands;

        public override void ClearCommands()
        {
            base.ClearCommands();
        }

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            int x = arguments.Count();

            Player player = Player.Get(((CommandSender)sender).SenderId);

            if (Plugin.Instance.CurrentVote == null)
            {
                response = response = "No vote is in progress.";
                return true;
            }

            response = VoteHandler.Voting(player, Command);
            return true;
        }

        public override void LoadGeneratedCommands()
        {
            throw new NotImplementedException();
        }

        public override void RegisterCommand(ICommand command)
        {
            base.RegisterCommand(command);
        }

        public override bool TryGetCommand(string query, out ICommand command)
        {
            return base.TryGetCommand(query, out command);
        }

        public override void UnregisterCommand(ICommand command)
        {
            base.UnregisterCommand(command);
        }

        public VoteCommand(string command)
        {
            Command = command;
        }
    }
}
