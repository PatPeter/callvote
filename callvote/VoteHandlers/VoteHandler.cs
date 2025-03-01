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
using Callvote;
using callvote.VoteHandlers;
using CommandSystem;
using callvote.Commands;

namespace callvote.VoteHandlers
{
    public class VoteHandler
    {
        public static string Voting(Player player, string option)
        {
            string playerNickname = player.Nickname;
            string playerUserId = player.UserId;
            if (Plugin.Instance.CurrentVote == null)
            {
                return Plugin.Instance.Translation.NoCallVoteInProgress;
            }
            if (Plugin.Instance.CurrentVote.Votes.Contains(playerUserId))
            {
                return Plugin.Instance.Translation.AlreadyVoted;
            }
            if (!Plugin.Instance.CurrentVote.Options.ContainsKey(option))
            {
                return Plugin.Instance.Translation.NoOptionAvailable;
            }
            Plugin.Instance.CurrentVote.Counter[option]++;
            Plugin.Instance.CurrentVote.Votes.Add(playerUserId);
            Log.Debug("Player " + playerNickname + " voted " + Plugin.Instance.CurrentVote.Options[option] + " bringing the counter to " + Plugin.Instance.CurrentVote.Counter[option]);
            return Plugin.Instance.Translation.VoteAccepted.Replace("%Reason%", Plugin.Instance.CurrentVote.Options[option]);
        }



        public static void StartVote(string question, Dictionary<string, string> options, CallvoteFunction callback)
        {
            VoteType newVote = new VoteType(question, options);
            Plugin.Instance.VoteCoroutine = Timing.RunCoroutine(StartVoteCoroutine(newVote, callback));
            foreach (var kvp in options)
            {
                VoteCommand voteCommand = new VoteCommand(kvp.Key);
                voteCommand.RegisterCommand(voteCommand);
            }
        }

        public static string StopVote()
        {
            if (!Plugin.Instance.VoteCoroutine.IsRunning)
            {
                return Plugin.Instance.Translation.NoCallVoteInProgress;
            }
            Timing.KillCoroutines(Plugin.Instance.VoteCoroutine);
            foreach (var kvp in Plugin.Instance.CurrentVote.Options)
            {
                VoteCommand voteCommand = new VoteCommand(kvp.Key);
                voteCommand.UnregisterCommand(voteCommand);
            }
            Plugin.Instance.CurrentVote = null;
            return Plugin.Instance.Translation.CallVoteEnded;
        }


        public static IEnumerator<float> StartVoteCoroutine(VoteType newVote, CallvoteFunction callback)
        {
            int timerCounter = 0;
            Plugin.Instance.CurrentVote = newVote;
            Plugin.Instance.CurrentVote.Callback = callback;
            string firstBroadcast = Plugin.Instance.Translation.AskedQuestion.Replace("%Question%", Plugin.Instance.CurrentVote.Question);
            int counter = 0;
            foreach (KeyValuePair<string, string> kv in Plugin.Instance.CurrentVote.Options)
            {
                if (counter == Plugin.Instance.CurrentVote.Options.Count - 1)
                {
                    firstBroadcast += $", {Plugin.Instance.Translation.Options.Replace("%OptionKey%", kv.Key.ToString()).Replace("%Option%", kv.Value)}";
                }
                else
                {
                    firstBroadcast += $" {Plugin.Instance.Translation.Options.Replace("%OptionKey%", kv.Key.ToString()).Replace("%Option%", kv.Value)}";
                }
                counter++;
            }
            int textsize = firstBroadcast.Length / 10;
            Map.Broadcast(5, "<size=" + (48 - textsize).ToString() + ">" + firstBroadcast + "</size>");
            yield return Timing.WaitForSeconds(5f);
            for (; ; )
            {
                if (timerCounter >= Plugin.Instance.Config.VoteDuration + 1)
                {
                    if (Plugin.Instance.CurrentVote.Callback == null)
                    {
                        string timerBroadcast = Plugin.Instance.Translation.Results;
                        foreach (var kv in Plugin.Instance.CurrentVote.Options)
                        {
                            timerBroadcast += Plugin.Instance.Translation.OptionAndCounter.Replace("%Option%", kv.Value).Replace("%OptionKey%", kv.Key.ToString()).Replace("%Counter%", Plugin.Instance.CurrentVote.Counter[kv.Key].ToString());
                            textsize = timerBroadcast.Length / 10;
                        }
                        Map.Broadcast(5, "<size=" + (48 - textsize).ToString() + ">" + timerBroadcast + "</size>");
                    }
                    else
                    {
                        Plugin.Instance.CurrentVote.Callback.Invoke(Plugin.Instance.CurrentVote);
                    }
                    Plugin.Instance.CurrentVote = null;
                    yield break;
                }
                else
                {
                    string timerBroadcast = firstBroadcast + "\n";
                    foreach (KeyValuePair<string, string> kv in Plugin.Instance.CurrentVote.Options)
                    {
                        timerBroadcast += Plugin.Instance.Translation.OptionAndCounter.Replace("%Option%", kv.Value).Replace("%OptionKey%", kv.Key.ToString()).Replace("%Counter%", Plugin.Instance.CurrentVote.Counter[kv.Key].ToString());
                        textsize = timerBroadcast.Length / 10;
                    }
                    Map.Broadcast(1, "<size=" + (48 - textsize).ToString() + ">" + timerBroadcast + "</size>");
                }
                timerCounter++;
                yield return Timing.WaitForSeconds(1f);
            }
        }
        public static string Rigging(string argument)
        {
            if (Plugin.Instance.CurrentVote == null)
            {
                return "vote not active";
            }
            if (!Plugin.Instance.CurrentVote.Options.ContainsKey(argument))
            {
                return Plugin.Instance.Translation.NoOptionAvailable;
            }
            Plugin.Instance.CurrentVote.Counter[argument]++;
            return "vote added";
        }


        public static IEnumerator<float> UpdateRoundTimer()
        {
            while (true)
            {
                Plugin.Instance.roundtimer = RoundStart.RoundLength.Seconds;
                yield return Timing.WaitForOneFrame;
            }
        }
    }
}
public delegate void CallvoteFunction(VoteType vote);

