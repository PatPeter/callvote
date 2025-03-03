using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using HarmonyLib;
using MEC;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;

namespace callvote
{
	public class Plugin
	{
		public static Plugin Instance { get; private set; } = new Plugin();

		[PluginConfig]
		public Config Config = new Config();

		public string Name { get; } = Callvote.AssemblyInfo.Name;
		public string Author { get; } = Callvote.AssemblyInfo.Author;
		public Version Version { get; } = new Version(Callvote.AssemblyInfo.Version);
		public string Prefix { get; } = Callvote.AssemblyInfo.LangFile;
		//public Version RequiredExiledVersion { get; } = new Version(5, 1, 3);
		//public PluginPriority Priority { get; } = PluginPriority.Default;

		//Instance variable for eventhandlers
		public EventHandlers EventHandlers;

		//bool voteInProgress = false;
		internal Vote CurrentVote = null;

		//[PluginPriority(LoadPriority.Lowest)]
		[PluginEntryPoint(Callvote.AssemblyInfo.Name, Callvote.AssemblyInfo.Version, Callvote.AssemblyInfo.Description, Callvote.AssemblyInfo.Author)]
		public void OnEnabled()
		{
			Instance = this;
			Config = new Config();

			try
			{
				Log.Debug("Initializing event handlers..");
				//Set instance varible to a new instance, this should be nulled again in OnDisable
				EventHandlers = new EventHandlers(this);
				EventManager.RegisterEvents(this, EventHandlers);

				Log.Info($"callvote loaded!");
			}
			catch (Exception e)
			{
				//This try catch is redundant, as EXILED will throw an error before this block can, but is here as an example of how to handle exceptions/errors
				Log.Error($"There was an error loading the plugin: {e}");
			}
		}

		[PluginUnload]
		public void OnDisabled()
		{

			EventHandlers = null;
		}

		[PluginReload]
		public void OnReloaded()
		{
			//This is only fired when you use the EXILED reload command, the reload command will call OnDisable, OnReload, reload the plugin, then OnEnable in that order. There is no GAC bypass, so if you are updating a plugin, it must have a unique assembly name, and you need to remove the old version from the plugins folder
		}
		
		public Dictionary<int, int> DictionaryOfVotes = new Dictionary<int, int>();
		public int timeOfLastVote = 0;
		private CoroutineHandle voteCoroutine = new CoroutineHandle();
		public string CallvoteHandler(Player player, string[] args) // lowercase to match command
		{
			int playerId = player.PlayerId;
			string playerUserId = player.UserId;
			string playerNickname = player.Nickname;
			//string playerGroupName = player.GroupName;
			//string playerBadgeName = player.RankName;
			Log.Info(playerNickname + " called vote with arguments: ");
			for (int i = 0; i < args.Length; i++)
			{
				Log.Info("\t" + i + ": " + args[i]);
			}
			if (DictionaryOfVotes.ContainsKey(playerId))
			{
				if(DictionaryOfVotes[playerId] > Plugin.Instance.Config.MaxAmountOfVotesPerRound -1) //  && !player.CheckPermission("cv.unlimitedvotes")
				{
					return Plugin.Instance.Config.Localizations.MaximumVotesReached;
				}
			}
				if (args.Length == 0)
			{
				//return new string[] { "callvote RestartRound", "callvote Kick <player>", "callvote <custom> [options]" };
				return Plugin.Instance.Config.Localizations.DescriptionCallvote;
			}
			else
			{
				int timestamp = (int)(DateTime.Now - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
				if (CurrentVote != null || timestamp - timeOfLastVote < Plugin.Instance.Config.VoteCooldown)
				{
					//return new string[] { "A vote is currently in progress." };
					return Plugin.Instance.Config.Localizations.VoteInProgress;
				}
				else
				{
					if (DictionaryOfVotes.ContainsKey(playerId))
					{
						DictionaryOfVotes[playerId] = DictionaryOfVotes[playerId] + 1;
					}
					else
					{
						DictionaryOfVotes.Add(playerId, 1);
					}
					Dictionary<int, string> options = new Dictionary<int, string>();

					if (args[0].ToLower().Equals(Plugin.Instance.Config.Localizations.SubcommandKick.ToLower()))
					{
						return CallvoteHandleKick(playerNickname, args, options);
					}
					else if (args[0].ToLower().Equals(Plugin.Instance.Config.Localizations.SubcommandKill.ToLower()))
					{
						return CallvoteHandleKill(playerNickname, args, options);
					}
					else if (args[0].ToLower().Equals(Plugin.Instance.Config.Localizations.SubcommandNuke.ToLower()))
					{
						return CallvoteHandleNuke(playerNickname, args, options);
					}
					else if (args[0].ToLower().Equals(Plugin.Instance.Config.Localizations.SubcommandRespawnWave.ToLower()))
					{
						return CallvoteHandleRespawnWave(playerNickname, args, options);
					}
					else if (args[0].ToLower().Equals(Plugin.Instance.Config.Localizations.SubcommandRestartRound.ToLower()))
					{
						return CallvoteHandleRestartRound(playerNickname, args, options);
					}
					else
					{
						return CallvoteHandleCustomVote(playerNickname, args, options);
					}
				}
			}
		}

		public string CallvoteHandleCustomVote(string playerNickname, string[] args, Dictionary<int, string> options)
		{
			if (Plugin.Instance.Config.ToggleCommands.Custom)
			{
				if (args.Length == 1)
				{
					Log.Info("Binary vote called by " + playerNickname + ": " + string.Join(" ", args));
					options[1] = Plugin.Instance.Config.Localizations.Yes;
					options[2] = Plugin.Instance.Config.Localizations.No;
				}
				else
				{
					Log.Info("Multiple-choice vote called by " + playerNickname + ": " + string.Join(" ", args));
					for (int i = 1; i < args.Length; i++)
					{
						options[i] = args[i];
					}
				}
				voteCoroutine = Timing.RunCoroutine(
					StartVoteCoroutine(
						new Vote(
							string.Format(Plugin.Instance.Config.Localizations.Prompt, playerNickname) + args[0],
							options
						)
						, null
					)
				);
				timeOfLastVote = (int)(DateTime.Now - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
				return Plugin.Instance.Config.Localizations.VoteStarted;
			}
			else
			{
				return string.Format(Plugin.Instance.Config.Localizations.SubcommandIsNotEnabled, "\"Custom Vote\"");
			}
		}

		public string CallvoteHandleKick(string playerNickname, string[] args, Dictionary<int, string> options)
		{
			if (Plugin.Instance.Config.ToggleCommands.Kick)
			{
				if (args.Length == 1)
				{
					return "callvote Kick <player>";
				}
				else
				{
					Log.Info("Vote called by " + playerNickname + " to " + args[0] + " player " + args[1]);

					List<Player> playerSearch = Server.GetPlayers<Player>().Where(p => p.Nickname.Contains(args[1])).ToList();
					if (playerSearch.Count() > 1)
					{
						return "Multiple players have a name or partial name of " + args[1] + ". Please use a different search string.";
					}
					else if (playerSearch.Count() == 1)
					{
						Player locatedPlayer = playerSearch[0];
						string locatedPlayerName = locatedPlayer.Nickname;

						options[1] = Plugin.Instance.Config.Localizations.Yes;
						options[2] = Plugin.Instance.Config.Localizations.No;

						voteCoroutine = Timing.RunCoroutine(
							StartVoteCoroutine(
								new Vote
								(
									string.Format(Plugin.Instance.Config.Localizations.Prompt, playerNickname)+ " Kick " + locatedPlayerName + "?",
									options
								),
								delegate (Vote vote)
								{
									int votePercent = (int)((float)vote.Counter[1] / (float)(Server.GetPlayers<Player>().Count()) * 100f);
									if (votePercent >= Plugin.Instance.Config.Thresholds.Kick)
									{
										Plugin.Instance.Broadcast(5, votePercent + "% voted yes. Kicking player " + locatedPlayerName + ".");
										//if(!locatedPlayer.CheckPermission("cv.untouchable"))
										//{
										locatedPlayer.Kick(votePercent + "% voted to kick you.");
										//}

									}
									else
									{
										Plugin.Instance.Broadcast(5, "Only " + votePercent + "% voted yes. " + Plugin.Instance.Config.Thresholds.Kick + "% was required to kick " + locatedPlayerName + ".");
									}
								}
							)
						);
						timeOfLastVote = (int)(DateTime.Now - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
						return Plugin.Instance.Config.Localizations.VoteStarted;
					}
					else
					{
						return "Did not find any players with the name or partial name of " + args[1];
					}
				}
			}
			else
			{
				return string.Format(Plugin.Instance.Config.Localizations.SubcommandIsNotEnabled, Plugin.Instance.Config.Localizations.SubcommandKick);
			}
		}

		public string CallvoteHandleKill(string playerNickname, string[] args, Dictionary<int, string> options)
		{
			if (Plugin.Instance.Config.ToggleCommands.Kill)
			{
				if (args.Length == 1)
				{
					return "callvote Kill <player>";
				}
				else
				{
					Log.Info("Vote called by " + playerNickname + " to " + args[0] + " player " + args[1]);

					List<Player> playerSearch = Server.GetPlayers<Player>().Where(p => p.Nickname.Contains(args[1])).ToList();
					if (playerSearch.Count() > 1)
					{
						return "Multiple players have a name or partial name of " + args[1] + ". Please use a different search string.";
					}
					else if (playerSearch.Count() == 1)
					{
						Player locatedPlayer = playerSearch[0];
						string locatedPlayerName = locatedPlayer.Nickname;

						options[1] = Plugin.Instance.Config.Localizations.Yes;
						options[2] = Plugin.Instance.Config.Localizations.No;

						voteCoroutine = Timing.RunCoroutine(
							StartVoteCoroutine(
								new Vote(
									string.Format(Plugin.Instance.Config.Localizations.Prompt, playerNickname) + " Kill " + locatedPlayerName + "?"
									, options
								),
								delegate (Vote vote)
								{
									int votePercent = (int)((float)vote.Counter[1] / (float)(Server.GetPlayers<Player>().Count()) * 100f);
									if (votePercent >= Plugin.Instance.Config.Thresholds.Kill)
									{
										Plugin.Instance.Broadcast(5, votePercent + "% voted yes. Killing player " + locatedPlayerName + ".");
										//if (!locatedPlayer.CheckPermission("cv.untouchable"))
										//{
										locatedPlayer.Kill("callvote");
										//}
									}
									else
									{
										Plugin.Instance.Broadcast(5, "Only " + votePercent + "% voted yes. " + Plugin.Instance.Config.Thresholds.Kill + "% was required to kill " + locatedPlayerName + ".");
									}
								}
							)
						);
						timeOfLastVote = (int)(DateTime.Now - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
						return Plugin.Instance.Config.Localizations.VoteStarted;
					}
					else
					{
						return "Did not find any players with the name or partial name of " + args[1];
					}
				}
			}
			else
			{
				return string.Format(Plugin.Instance.Config.Localizations.SubcommandIsNotEnabled, Plugin.Instance.Config.Localizations.SubcommandKill);
			}
		}

		public string CallvoteHandleNuke(string playerNickname, string[] args, Dictionary<int, string> options)
		{
			if (Plugin.Instance.Config.ToggleCommands.Nuke)
			{
				Log.Info("Vote called by " + playerNickname + " to " + args[0]);
				//return new string[] { "To be implemented." };

				options[1] = Plugin.Instance.Config.Localizations.Yes;
				options[2] = Plugin.Instance.Config.Localizations.No;

				voteCoroutine = Timing.RunCoroutine(
					StartVoteCoroutine(
						new Vote(
							string.Format(Plugin.Instance.Config.Localizations.Prompt, playerNickname) + " NUKE THE FACILITY?!?",
							options
						),
						delegate (Vote vote)
						{
							int votePercent = (int)((float)vote.Counter[1] / (float)(Server.GetPlayers<Player>().Count()) * 100f);
							if (votePercent >= Plugin.Instance.Config.Thresholds.Nuke)
							{
								Plugin.Instance.Broadcast(5, votePercent + "% voted yes. Nuking the facility...");
								//Exiled.API.Features.Warhead.Start();
								Warhead.Start();
							}
							else
							{
								Plugin.Instance.Broadcast(5, "Only " + votePercent + "% voted yes. " + Plugin.Instance.Config.Thresholds.Nuke + "% was required to nuke the facility.");
							}
						}
					)
				);
				timeOfLastVote = (int)(DateTime.Now - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
				return Plugin.Instance.Config.Localizations.VoteStarted;
			}
			else
			{
				return string.Format(Plugin.Instance.Config.Localizations.SubcommandIsNotEnabled, Plugin.Instance.Config.Localizations.SubcommandNuke);
			}
		}

		public string CallvoteHandleRespawnWave(string playerNickname, string[] args, Dictionary<int, string> options)
		{
			if (Plugin.Instance.Config.ToggleCommands.RespawnWave)
			{
				Log.Info("Vote called by " + playerNickname + " to " + args[0]);
				//return new string[] { "To be implemented." };

				options[1] = Plugin.Instance.Config.Localizations.No;
				options[2] = Plugin.Instance.Config.Localizations.MTF;
				options[3] = Plugin.Instance.Config.Localizations.CI;

				voteCoroutine = Timing.RunCoroutine(
					StartVoteCoroutine(
						new Vote(
							string.Format(Plugin.Instance.Config.Localizations.Prompt, playerNickname) + " Respawn the next wave?",
							options
						),
						delegate (Vote vote)
						{
							int votePercent = (int)((float)vote.Counter[1] / (float)(Server.GetPlayers<Player>().Count()) * 100f);
							int mtfVotePercent = (int)((float)vote.Counter[2] / (float)(Server.GetPlayers<Player>().Count()) * 100f);
							int ciVotePercent = (int)((float)vote.Counter[3] / (float)(Server.GetPlayers<Player>().Count()) * 100f);
							if (mtfVotePercent >= Plugin.Instance.Config.Thresholds.RespawnWave)
							{
								Plugin.Instance.Broadcast(5, mtfVotePercent + "% voted yes. Respawning a wave of Nine-Tailed Fox...");
								Respawning.RespawnManager.Singleton.ForceSpawnTeam(Respawning.SpawnableTeamType.NineTailedFox);

							}
							else if (ciVotePercent >= Plugin.Instance.Config.Thresholds.RespawnWave)
							{
								Plugin.Instance.Broadcast(5, ciVotePercent + "% voted yes. Respawning a wave of Chaos Insurgency...");
								Respawning.RespawnManager.Singleton.ForceSpawnTeam(Respawning.SpawnableTeamType.ChaosInsurgency);
							}
							else
							{
								Plugin.Instance.Broadcast(5, votePercent + "% voted no. " + Plugin.Instance.Config.Thresholds.RespawnWave + "% was required to respawn the next wave.");
							}
						}
					)
				);
				timeOfLastVote = (int)(DateTime.Now - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
				return Plugin.Instance.Config.Localizations.VoteStarted;
			}
			else
			{
				return string.Format(Plugin.Instance.Config.Localizations.SubcommandIsNotEnabled, Plugin.Instance.Config.Localizations.SubcommandRespawnWave);
			}
		}

		public string CallvoteHandleRestartRound(string playerNickname, string[] args, Dictionary<int, string> options)
		{
			if (Plugin.Instance.Config.ToggleCommands.RestartRound)
			{
				Log.Info("Vote called by " + playerNickname + " to " + args[0]);
				//return new string[] { "To be implemented." };

				options[1] = Plugin.Instance.Config.Localizations.Yes;
				options[2] = Plugin.Instance.Config.Localizations.No;

				voteCoroutine = Timing.RunCoroutine(
					StartVoteCoroutine(
						new Vote(
							string.Format(Plugin.Instance.Config.Localizations.Prompt, playerNickname) + " Restart the round?",
							options
						),
						delegate (Vote vote)
						{
							int votePercent = (int)((float)vote.Counter[1] / (float)(Server.GetPlayers<Player>().Count()) * 100f);
							if (votePercent >= Plugin.Instance.Config.Thresholds.RestartRound)
							{
								Plugin.Instance.Broadcast(5, votePercent + "% voted yes. Restarting the round...");
								//this.Server.Round.RestartRound();
								//PlayerManager.localPlayer.GetComponent<PlayerStats>().Roundrestart();
								//Round.Restart();
								RoundSummary.singleton.ForceEnd();

							}
							else
							{
								Plugin.Instance.Broadcast(5, "Only " + votePercent + "% voted yes. " + Plugin.Instance.Config.Thresholds.RestartRound + "% was required to restart the round.");
							}
						}
					)
				);
				timeOfLastVote = (int)(DateTime.Now - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
				return Plugin.Instance.Config.Localizations.VoteStarted;
			}
			else
			{
				return string.Format(Plugin.Instance.Config.Localizations.SubcommandIsNotEnabled, Plugin.Instance.Config.Localizations.SubcommandRestartRound);
			}
		}

		public bool Voting()
		{
			if (CurrentVote != null)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public string Rigging(int argument)
		{
			string response = "vote not active";
			if (CurrentVote != null)
			{
				response = "could not find option";
				if (CurrentVote.Options.ContainsKey(argument))
				{
					CurrentVote.Counter[argument]++;
					response = "vote added";
				}
			}
			return response;
		}

		public void OnStartVote(string question, Dictionary<int, string> options, HashSet<string> votes, Dictionary<int, int> counter)
		{
			Vote newVote = new Vote(question, options, votes, counter);
			voteCoroutine = Timing.RunCoroutine(StartVoteCoroutine(newVote, null));
		}

		public IEnumerator<float> StartVoteCoroutine(Vote newVote, CallvoteFunction callback)
		{
			int timerCounter = 0;
			CurrentVote = newVote;
			CurrentVote.Callback = callback;
			string firstBroadcast = CurrentVote.Question + " " + Plugin.Instance.Config.Localizations.Tutorial + " ";
			int counter = 0;
			foreach (KeyValuePair<int, string> kv in CurrentVote.Options)
			{
				if (counter == CurrentVote.Options.Count - 1)
				{
					firstBroadcast += Plugin.Instance.Config.Localizations.Or + " ." + kv.Key + " " + Plugin.Instance.Config.Localizations.For + " " + kv.Value + " ";
				}
				else
				{
					firstBroadcast += "." + kv.Key + " " + Plugin.Instance.Config.Localizations.For + " " + kv.Value + ", ";
				}
				counter++;
			}
			int textsize = firstBroadcast.Length / 10;
			Plugin.Instance.Broadcast(5, "<size="+(48-textsize).ToString()+">"+firstBroadcast+ "</size>");
			yield return Timing.WaitForSeconds(5f);
			for (; ; )
			{
				if (timerCounter >= Plugin.Instance.Config.VoteDuration + 1)
				{
					if (CurrentVote.Callback == null)
					{
						string timerBroadcast = Plugin.Instance.Config.Localizations.FinalResults + "\n";
						foreach (KeyValuePair<int, string> kv in CurrentVote.Options)
						{
							timerBroadcast += CurrentVote.Options[kv.Key] + " (" + CurrentVote.Counter[kv.Key] + ") ";
							textsize = timerBroadcast.Length / 10;
						}
						Plugin.Instance.Broadcast(5, "<size=" + (48 - textsize).ToString() + ">" + timerBroadcast + "</size>");
					}
					else
					{
						CurrentVote.Callback.Invoke(CurrentVote);
					}
					CurrentVote = null;
					yield break;
				}
				else
				{
					string timerBroadcast = firstBroadcast + "\n";
					foreach (KeyValuePair<int, string> kv in CurrentVote.Options)
					{
						timerBroadcast += CurrentVote.Options[kv.Key] + " (" + CurrentVote.Counter[kv.Key] + ") ";
						textsize = timerBroadcast.Length / 10;
					}
					Plugin.Instance.Broadcast(1, "<size=" + (48 - textsize).ToString() + ">" + timerBroadcast + "</size>");
				}
				timerCounter++;
				yield return Timing.WaitForSeconds(1f);
			}
		}

		public void Broadcast(ushort duration, string message)
		{
			Server.SendBroadcast(message, duration);
		}

		public void PlayerBroadcast(Player player, ushort duration, string message)
		{
			player.SendBroadcast(message, duration);
		}

		public string StopvoteHandler(Player player)
		{

			if (player.UserId != "76561197974998697") // !player.CheckPermission("cv.stopvote")
			{
				return "You do not have permission to run this command";
			}

			if (this.StopVote())
			{
				return "Vote stopped.";
			}
			else
			{
				return "There is not a vote in progress.";
			}
		}

		public bool StopVote()
		{
			if (voteCoroutine.IsRunning)
			{
				Timing.KillCoroutines(voteCoroutine);
				CurrentVote = null;
				return true;
			}
			else
			{
				return false;
			}
		}

		public string VoteHandler(Player player, int option)
		{
			string playerNickname = player.Nickname;
			string playerUserId = player.UserId;
			if (CurrentVote != null)
			{
				if (!CurrentVote.Votes.Contains(playerUserId))
				{
					if (CurrentVote.Options.ContainsKey(option))
					{
						CurrentVote.Counter[option]++;
						CurrentVote.Votes.Add(playerUserId);
						Log.Info("Player " + playerNickname + " voted " + CurrentVote.Options[option] + " bringing the counter to " + CurrentVote.Counter[option]);
						//return new string[] { "Vote accepted!" };
						return "Vote accepted!";
					}
					else
					{
						return "Vote does not have an option " + option + ".";
					}
				}
				else
				{
					return "You've already voted.";
				}
			}
			else
			{
				return "There is no vote in progress.";
			}
		}
	}

	public delegate void CallvoteFunction(Vote vote);

	public class Vote
	{
		public string Question;
		public Dictionary<int, string> Options;
		public HashSet<string> Votes;
		public Dictionary<int, int> Counter;
		public Timer Timer;
		public CallvoteFunction Callback;

		public Vote(string question, Dictionary<int, string> options)
		{
			this.Question = question;
			this.Options = options;
			this.Votes = new HashSet<string>();
			this.Counter = new Dictionary<int, int>();
			foreach (int option in options.Keys)
			{
				Counter[option] = 0;
			}
		}

		// Allow Votes and Counter to be passed in and saved by reference for Event code
		public Vote(string question, Dictionary<int, string> options, HashSet<string> votes, Dictionary<int, int> counter)
		{
			this.Question = question;
			this.Options = options;
			this.Votes = votes;
			this.Counter = counter;
			foreach (int option in options.Keys)
			{
				Counter[option] = 0;
			}
		}
	}
}
