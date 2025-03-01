using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using MEC;
using Exiled.Permissions.Extensions;
using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;
using GameCore;
using Log = Exiled.API.Features.Log;
using UnityEngine;
using PluginAPI;
using Exiled.API.Features.Hazards;
using Mirror;
using Respawning.Waves;
namespace callvote.VoteHandlers
{
    public class CallingVoteHandler
    {
        
        public int timeOfLastVote = 0;


        /*public static string CallVote(Player player, string[] args) // lowercase to match command
        {
            string playerNickname = player.Nickname;
            string playerGroupName = player.GroupName;
            string playerBadgeName = player.RankName;
            Log.Debug(playerNickname + " called vote with arguments: ");
            for (int i = 0; i < args.Length; i++)
            {
                Log.Debug("\t" + i + ": " + args[i]);
            }
            if (Plugin.Instance.DictionaryOfVotes.ContainsKey(player.Id))
            {
                if (Plugin.Instance.DictionaryOfVotes[player.Id] > Plugin.Instance.Config.MaxAmountOfVotesPerRound - 1 && !player.CheckPermission("cv.unlimitedvotes"))
                {
                    return Plugin.Instance.Translation.MaxVote;
                }
            }
            if (args.Length == 0)
            {
                //return new string[] { "callvote RestartRound", "callvote Kick <player>", "callvote <custom> [options]" };
                return "callvote Kick/Kill/Nuke/RespawnWave/RestartRound/\"Question\"/[player]/[options]/";
            }
            else
            {
                int timestamp = (int)(DateTime.Now - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
                int timestampGeneral = (int)(DateTime.Now - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
                if (Plugin.Instance.CurrentVote != null) { return Plugin.Instance.Translation.InProgressVote; }
                if (timestamp - Plugin.Instance.TimeOfLastVote < Plugin.Instance.Config.VoteCooldown) { return Plugin.Instance.Translation.WaitToVote.Replace("%Timer%", $"{Plugin.Instance.Config.VoteCooldown - (timestamp - Plugin.Instance.TimeOfLastVote)}"); }
                else
                {
                    if (Plugin.Instance.DictionaryOfVotes.ContainsKey(player.Id))
                    {
                        Plugin.Instance.DictionaryOfVotes[player.Id] =  Plugin.Instance.DictionaryOfVotes[player.Id] + 1;

                    }
                    else
                    {
                        Plugin.Instance.DictionaryOfVotes.Add(player.Id, 1);
                    }
                    Dictionary<string, string> options = new Dictionary<int, string>();
                    switch (args[0].ToLower())
                    {
                        case "kick":
                            if (!Plugin.Instance.Config.EnableKick && Plugin.Instance.roundtimer >= Plugin.Instance.Config.MaxWaitKick && !player.CheckPermission("cv.callvotekick") || !player.CheckPermission("cv.bypass"))
                            {
                                if (!player.CheckPermission("cv.callvotekick")) { return Plugin.Instance.Translation.NoPermissionToVote; }
                                if (Plugin.Instance.roundtimer < Plugin.Instance.Config.MaxWaitRestartRound) { return Plugin.Instance.Translation.WaitToVote.Replace("%Timer%", $"{Plugin.Instance.Config.MaxWaitKick - Plugin.Instance.roundtimer}"); }
                                return Plugin.Instance.Translation.VoteKickDisabled;
                            }
                            if (args.Length == 1)
                            {
                                return "callvote Kick <player>";
                            }
                            if (Player.Get(args[1]) == null)
                            {
                                return Plugin.Instance.Translation.PlayerNotFound.Replace("%Player", args[1]);
                            }
                            else
                            {
                                List<Player> playerSearch = Player.List.Where(p => p.Nickname.Contains(args[1])).ToList();
                                if (playerSearch.Count() > 1)
                                {
                                    return Plugin.Instance.Translation.PlayersWithSameName.Replace("%Player%", args[1]);

                                }
                                else if (playerSearch.Count() == 1)
                                {
                                    Player locatedPlayer = playerSearch[0];
                                    string locatedPlayerName = locatedPlayer.Nickname;

                                    options[1] = Plugin.Instance.Translation.OptionYes;
                                    options[2] = Plugin.Instance.Translation.OptionNo;

                                    Plugin.Instance.VoteCoroutine = Timing.RunCoroutine(VoteHandler.StartVoteCoroutine(new VoteType(Plugin.Instance.Translation.AskedToKick.Replace("%Player%", playerNickname).Replace("%Offender%", locatedPlayerName), 
                                        options), delegate (VoteType vote)
                                    {
                                        int votePercent = (int)((float)vote.Counter[1] / (float)(Player.List.Count()) * 100f);
                                        if (votePercent >= Plugin.Instance.Config.ThresholdKick)
                                        {
                                            Map.Broadcast(5, Plugin.Instance.Translation.PlayerGettingKicked.Replace("%VotePercent%", votePercent.ToString()).Replace("%player%", locatedPlayerName));
                                            if (!locatedPlayer.CheckPermission("cv.untouchable"))
                                            {
                                                locatedPlayer.Kick(Plugin.Instance.Translation.Untouchable.Replace("%VotePercent%", votePercent.ToString()));
                                            }

                                        }
                                        else
                                        {
                                            Map.Broadcast(5, Plugin.Instance.Translation.NotSuccessFullKick.Replace("%VotePercent%", votePercent.ToString()).Replace("%ThresholdKick%", Plugin.Instance.Config.ThresholdKick.ToString()).Replace("%Offender%", locatedPlayerName));
                                        }
                                    }));

                                    
                                }
                                break;
                            }


                        case "nuke":
                            if (Plugin.Instance.Config.EnableNuke == true && Plugin.Instance.roundtimer > Plugin.Instance.Config.MaxWaitNuke && player.CheckPermission("cv.callvotenuke") || player.CheckPermission("cv.bypass"))
                            {
                                Log.Debug("Vote called by " + playerNickname + " to " + args[0]);
                                //return new string[] { "To be implemented." };

                                options[1] = Plugin.Instance.Translation.OptionYes;
                                options[2] = Plugin.Instance.Translation.OptionNo;

                                Plugin.Instance.VoteCoroutine = Timing.RunCoroutine(VoteHandler.StartVoteCoroutine(new VoteType(Plugin.Instance.Translation.AskedToNuke.Replace("%Player%", playerNickname), options), delegate (VoteType vote)
                                {
                                    int votePercent = (int)((float)vote.Counter[1] / (float)(Player.List.Count()) * 100f);
                                    if (votePercent >= Plugin.Instance.Config.ThresholdNuke)
                                    {
                                        Map.Broadcast(5, Plugin.Instance.Translation.FoundationNuked.Replace("%VotePercent%", votePercent.ToString()));
                                        Exiled.API.Features.Warhead.Start();
                                    }
                                    else
                                    {
                                        Map.Broadcast(5, Plugin.Instance.Translation.NoSuccessFullNuke.Replace("%VotePercent%", votePercent.ToString()).Replace("%ThresholdNuke%", Plugin.Instance.Config.ThresholdNuke.ToString()));
                                    }

                                }));
                                break;
                            }
                            else
                            {
                                if (!player.CheckPermission("cv.callvotenuke")) { return Plugin.Instance.Translation.NoPermissionToVote; }
                                if (Plugin.Instance.roundtimer < Plugin.Instance.Config.MaxWaitNuke) { return Plugin.Instance.Translation.WaitToVote.Replace("%Timer%", $"{Plugin.Instance.Config.MaxWaitNuke - Plugin.Instance.roundtimer}"); }
                                return Plugin.Instance.Translation.VoteNukeDisabled;
                            }

                        case "respawnwave":
                            if (Plugin.Instance.Config.EnableRespawnWave && Plugin.Instance.roundtimer > Plugin.Instance.Config.MaxWaitRespawnWave && player.CheckPermission("cv.callvoterespawnwave") || player.CheckPermission("cv.bypass"))
                            {
                                Log.Debug("Vote called by " + playerNickname + " to " + args[0]);
                                //return new string[] { "To be implemented." };

                                options[1] = Plugin.Instance.Translation.OptionNo;
                                options[2] = Plugin.Instance.Translation.mtf;
                                options[3] = Plugin.Instance.Translation.ci;

                                Plugin.Instance.VoteCoroutine = Timing.RunCoroutine(VoteHandler.StartVoteCoroutine(new VoteType(Plugin.Instance.Translation.AskedToRespawn.Replace("%Player%", playerNickname), options), delegate (VoteType vote)
                                {
                                    int votePercent = (int)((float)vote.Counter[1] / (float)(Player.List.Count()) * 100f);
                                    int mtfVotePercent = (int)((float)vote.Counter[2] / (float)(Player.List.Count()) * 100f);
                                    int ciVotePercent = (int)((float)vote.Counter[3] / (float)(Player.List.Count()) * 100f);
                                    if (mtfVotePercent >= Plugin.Instance.Config.ThresholdRespawnWave)
                                    {
                                        Map.Broadcast(5, Plugin.Instance.Translation.mtfRespawn.Replace("%VotePercent%", mtfVotePercent.ToString() + "%"));
                                        Respawning.WaveManager.Spawn(Respawning.WaveManager.Waves[0]); //Gets MTF wave from List, [index] following implementation

                                    }
                                    else if (ciVotePercent >= Plugin.Instance.Config.ThresholdRespawnWave)
                                    {
                                        Map.Broadcast(5, Plugin.Instance.Translation.ciRespawn.Replace("%VotePercent%", ciVotePercent.ToString()));
                                        Respawning.WaveManager.Spawn(Respawning.WaveManager.Waves[1]); //Gets CI wave from List, [index] following implementation
                                    }
                                    else
                                    {
                                        Map.Broadcast(5, Plugin.Instance.Translation.NoSuccessFullRespawn.Replace("%VotePercent%", votePercent.ToString()).Replace("%ThresholdRespawnWave%", Plugin.Instance.Config.ThresholdRespawnWave.ToString()));
                                    }
                                }));
                                break;
                            }
                            else
                            {
                                if (!player.CheckPermission("cv.callvoterespawnwave")) { return Plugin.Instance.Translation.NoPermissionToVote; }
                                if (Plugin.Instance.roundtimer < Plugin.Instance.Config.MaxWaitRespawnWave) { return Plugin.Instance.Translation.WaitToVote.Replace("%Timer%", $"{Plugin.Instance.Config.MaxWaitRespawnWave - Plugin.Instance.roundtimer}"); }
                                return Plugin.Instance.Translation.VoteRespawnWaveDisabled;
                            }

                        case "restartround":
                            if (Plugin.Instance.Config.EnableRestartRound && Plugin.Instance.roundtimer > Plugin.Instance.Config.MaxWaitRestartRound && player.CheckPermission("cv.callvoterestartround") || player.CheckPermission("cv.bypass"))
                            {
                                Log.Debug("Vote called by " + playerNickname + " to " + args[0]);
                                //return new string[] { "To be implemented." };

                                options[1] = Plugin.Instance.Translation.OptionYes;
                                options[2] = Plugin.Instance.Translation.OptionNo;



                                Plugin.Instance.VoteCoroutine = Timing.RunCoroutine(VoteHandler.StartVoteCoroutine(new VoteType(Plugin.Instance.Translation.AskedToRestart.Replace("%Player%", playerNickname), options), delegate (VoteType vote)
                                {
                                    int votePercent = (int)((float)vote.Counter[1] / (float)(Player.List.Count()) * 100f);
                                    if (votePercent >= Plugin.Instance.Config.ThresholdRestartRound)
                                    {
                                        Map.Broadcast(5, Plugin.Instance.Translation.RoundRestarting.Replace("%VotePercent%", votePercent.ToString()));
                                        //this.Server.Round.RestartRound();
                                        //PlayerManager.localPlayer.GetComponent<PlayerStats>().Roundrestart();
                                        Round.Restart();

                                    }
                                    else
                                    {
                                        Map.Broadcast(5, Plugin.Instance.Translation.NoSuccessFullRestart.Replace("%VotePercent%", votePercent.ToString()).Replace("%ThresholdRestartRound%", Plugin.Instance.Config.ThresholdRestartRound.ToString()));
                                    }
                                }));
                                break;
                            }
                            else
                            {
                                if (!player.CheckPermission("cv.callvoterestartround")) { return Plugin.Instance.Translation.NoPermissionToVote; }
                                if (Plugin.Instance.roundtimer < Plugin.Instance.Config.MaxWaitRestartRound) { return Plugin.Instance.Translation.WaitToVote.Replace("%Timer%", $"{Plugin.Instance.Config.MaxWaitRestartRound - Plugin.Instance.roundtimer}"); }
                                return Plugin.Instance.Translation.VoteRestartRoundDisabled;
                            }

                        default:
                            //voteInProgress = true;
                            if (player.CheckPermission("cv.callvotecustom") || player.CheckPermission("cv.bypass"))
                            {
                                if (args.Length == 1)
                                {
                                    Log.Debug("Binary vote called by " + playerNickname + ": " + string.Join(" ", args));
                                    options[1] = Plugin.Instance.Translation.OptionYes;
                                    options[2] = Plugin.Instance.Translation.OptionNo;
                                }
                                else
                                {
                                    Log.Debug("Multiple-choice vote called by " + playerNickname + ": " + string.Join(" ", args));
                                    for (int i = 1; i < args.Length; i++)
                                    {
                                        options[i] = args[i];
                                    }
                                }
                                Plugin.Instance.VoteCoroutine = Timing.RunCoroutine(VoteHandler.StartVoteCoroutine(new VoteType(Plugin.Instance.Translation.AskedToRestart.Replace("%Player%", playerNickname).Replace("%Custom%", args[0]), options), null));
                                break;
                            }
                            else { return Plugin.Instance.Translation.NoPermissionToVote; }
                    }
                    Plugin.Instance.TimeOfLastVote = (int)(DateTime.Now - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
                    return Plugin.Instance.Translation.VoteStarted;
                }
            }
        }*/
    }
}
