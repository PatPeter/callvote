using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using RemoteAdmin;
using UnityEngine;
using System.Text.RegularExpressions;
using callvote.VoteHandlers;
using MEC;

namespace callvote.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    class ParentCallVoteCommand : ParentCommand
    {
        public ParentCallVoteCommand()
        {
            LoadGeneratedCommands();
        }
        public override string Command => "callvoteT";

        public override string[] Aliases => null;

        public override string Description => "";

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new KickCommand());
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "Sintaxe errada para o comando. Use .mvp (operação) ou .mvp (operação) (player)";
            return true;
        }

        public class KickCommand : ICommand
        {
            public string Command => "kick";

            public string[] Aliases => new string[] { "ki", "k" };

            public string Description => "";
            public bool Execute(ArraySegment<string> args, ICommandSender sender, out string response)
            {
                Dictionary<string, string> options = new Dictionary<string, string>();

                Player player = Player.Get((CommandSender)sender);
                if (!player.CheckPermission("cv.callvotekick") || !player.CheckPermission("cv.bypass"))
                {
                    response = Plugin.Instance.Translation.NoPermissionToVote;
                    return true;
                }

                if (args.Count == 0)
                {
                    response = "callvote Kick <player> (reason)";
                    return true;
                }

                if (args.Count == 1)
                {
                    response = "You need to pass a reason!";
                    return true;
                }

                if (Plugin.Instance.roundtimer < Plugin.Instance.Config.MaxWaitRestartRound || !player.CheckPermission("cv.bypass"))
                {

                    response = Plugin.Instance.Translation.WaitToVote.Replace("%Timer%", $"{Plugin.Instance.Config.MaxWaitKick - Plugin.Instance.roundtimer}");
                    return true;
                }

                if (!Plugin.Instance.Config.EnableKick || !player.CheckPermission("cv.bypass"))
                {
                    response = Plugin.Instance.Translation.VoteKickDisabled;
                    return true;
                }

                if (Player.Get(args.ToArray()[1]) == null)
                {
                    response = Plugin.Instance.Translation.PlayerNotFound.Replace("%Player", args.ToArray()[1]);
                    return true;
                }


                List<Player> playerSearch = Player.List.Where(p => p.Nickname.Contains(args.ToArray()[1])).ToList(); //To check if there are players with same name or not, kinda junky but whatever
                if (playerSearch.Count() < 0 || playerSearch.Count() > 1)
                {
                    response = Plugin.Instance.Translation.PlayersWithSameName.Replace("%Player%", args.ToArray()[1]);
                    return true;

                }

                Player locatedPlayer= Player.Get(args.ToArray()[1]);

                options.Add("yes", Plugin.Instance.Translation.OptionYes);
                options.Add("no", Plugin.Instance.Translation.OptionNo);

                VoteHandler.StartVote(Plugin.Instance.Translation.AskedToKick.Replace("%Player%", player.Nickname).Replace("%Offender%", locatedPlayer.Nickname), options, delegate (VoteType vote)
                {
                    int yesVotePercent = (int)((float)vote.Counter["yes"] / (float)(Player.List.Count()) * 100f);
                    int noVotePercent = (int)((float)vote.Counter["no"] / (float)(Player.List.Count()) * 100f); //Just so you know that it exists
                    if (yesVotePercent >= Plugin.Instance.Config.ThresholdKick)
                    {
                        Map.Broadcast(5, Plugin.Instance.Translation.PlayerGettingKicked
                            .Replace("%VotePercent%", yesVotePercent.ToString())
                            .Replace("%player%", locatedPlayer.Nickname)
                            .Replace("%Reason%", args.ToArray()[2]));

                        if (!locatedPlayer.CheckPermission("cv.untouchable"))
                        {
                            locatedPlayer.Kick(Plugin.Instance.Translation.Untouchable
                                .Replace("%VotePercent%", yesVotePercent.ToString()));
                        }

                    }
                    else
                    {
                        Map.Broadcast(5, Plugin.Instance.Translation.NotSuccessFullKick
                            .Replace("%VotePercent%", yesVotePercent.ToString())
                            .Replace("%ThresholdKick%", Plugin.Instance.Config.ThresholdKick.ToString())
                            .Replace("%Offender%", locatedPlayer.Nickname));
                    }
                });
                response = "Vote started.";
                return true;
            }
        }
    }
}

