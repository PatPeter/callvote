using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using MEC;
using Exiled.Permissions.Extensions;

namespace callvote
{
	public class Plugin : Plugin<Config>
	{
		private static readonly Lazy<Plugin> LazyInstance = new Lazy<Plugin>(() => new Plugin());
		//Instance variable for eventhandlers
		public EventHandlers EventHandlers;

		private Plugin()
		{
		}

		/// <summary>
		/// Gets the lazy instance.
		/// </summary>
		public static Plugin Instance => LazyInstance.Value;

		//bool voteInProgress = false;
		internal Vote CurrentVote = null;

		public override void OnEnabled()
		{
			//ReloadConfig();
			try
			{
				Log.Debug("Initializing event handlers..");
				//Set instance varible to a new instance, this should be nulled again in OnDisable
				EventHandlers = new EventHandlers(this);
				//Hook the events you will be using in the plugin. You should hook all events you will be using here, all events should be unhooked in OnDisabled 
				Exiled.Events.Handlers.Server.SendingConsoleCommand += EventHandlers.OnConsoleCommand;
				Exiled.Events.Handlers.Server.WaitingForPlayers += EventHandlers.OnWaitingForPlayers;
				Exiled.Events.Handlers.Server.RoundEnded += EventHandlers.OnRoundEnded;
				Log.Info($"callvote loaded!");
			}
			catch (Exception e)
			{
				//This try catch is redundant, as EXILED will throw an error before this block can, but is here as an example of how to handle exceptions/errors
				Log.Error($"There was an error loading the plugin: {e}");
			}
		}

		public override void OnDisabled()
		{
			Exiled.Events.Handlers.Server.SendingConsoleCommand -= EventHandlers.OnConsoleCommand;
			Exiled.Events.Handlers.Server.WaitingForPlayers -= EventHandlers.OnWaitingForPlayers;
			Exiled.Events.Handlers.Server.RoundEnded -= EventHandlers.OnRoundEnded;

			EventHandlers = null;
		}

		public override void OnReloaded()
		{
			//This is only fired when you use the EXILED reload command, the reload command will call OnDisable, OnReload, reload the plugin, then OnEnable in that order. There is no GAC bypass, so if you are updating a plugin, it must have a unique assembly name, and you need to remove the old version from the plugins folder
		}

		public override string Name { get; } = "callvote";
		public Dictionary<int, int> DictionaryOfVotes = new Dictionary<int, int>();
		public int timeOfLastVote = 0;
		private CoroutineHandle voteCoroutine = new CoroutineHandle();
		public string CallvoteHandler(Player player, string[] args) // lowercase to match command
		{
			string playerNickname = player.Nickname;
			string playerGroupName = player.GroupName;
			string playerBadgeName = player.RankName;
			Log.Info(playerNickname + " called vote with arguments: ");
			for (int i = 0; i < args.Length; i++)
			{
				Log.Info("\t" + i + ": " + args[i]);
			}
			if (DictionaryOfVotes.ContainsKey(player.Id))
			{
				if(DictionaryOfVotes[player.Id] > Plugin.Instance.Config.MaxAmountOfVotesPerRound -1)
				{
					return "Max amounts of votes done this round";
				}
			}
				if (args.Length == 0)
			{
				//return new string[] { "callvote RestartRound", "callvote Kick <player>", "callvote <custom> [options]" };
				return "callvote Kick/Kill/<custom> <player>/[options]";
			}
			else
			{
				int timestamp = (int)(DateTime.Now - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
				if (CurrentVote != null || timestamp - timeOfLastVote < Plugin.Instance.Config.VoteCooldown)
				{
					//return new string[] { "A vote is currently in progress." };
					return "A vote is currently in progress or the vote is on cooldown";
				}
				else
				{
					if (DictionaryOfVotes.ContainsKey(player.Id))
					{
						DictionaryOfVotes[player.Id] = DictionaryOfVotes[player.Id] + 1;
					}
					else
					{
						DictionaryOfVotes.Add(player.Id,1);
					}
					Dictionary<int, string> options = new Dictionary<int, string>();
					switch (args[0].ToLower())
					{
						case "kick":
							if (Plugin.Instance.Config.EnableKick)
							{
								if (args.Length == 1)
								{
									return "callvote Kick <player>";
								}
								else
								{
									Log.Info("Vote called by " + playerNickname + " to " + args[0] + " player " + args[1]);

									List<Player> playerSearch = Player.List.Where(p => p.Nickname.Contains(args[1])).ToList();
									if (playerSearch.Count() > 1)
									{
										return "Multiple players have a name or partial name of " + args[1] + ". Please use a different search string.";
									}
									else if (playerSearch.Count() == 1)
									{
										Player locatedPlayer = playerSearch[0];
										string locatedPlayerName = locatedPlayer.Nickname;

										options[1] = "Yes";
										options[2] = "No";

										voteCoroutine = Timing.RunCoroutine(StartVoteCoroutine(new Vote(playerNickname + " asks: Kick " + locatedPlayerName + "?", options), delegate (Vote vote)
										{
											int votePercent = (int)((float)vote.Counter[1] / (float)(Player.List.Count()) * 100f);
											if (votePercent >= Plugin.Instance.Config.ThresholdKick)
											{
												Map.Broadcast(5, votePercent + "% voted yes. Kicking player " + locatedPlayerName + ".");
												if(!locatedPlayer.CheckPermission("cv.untouchable"))
												{
													locatedPlayer.Kick(votePercent + "% voted to kick you.");
												}
												
											}
											else
											{
												Map.Broadcast(5, "Only " + votePercent + "% voted yes. " + Plugin.Instance.Config.ThresholdKick + "% was required to kick " + locatedPlayerName + ".");
											}
										}));

										break;
									}
									else
									{
										return "Did not find any players with the name or partial name of " + args[1];
									}
								}
							}
							else
							{
								return "callvote Kick is not enabled.";
							}

						case "kill":
							if (Plugin.Instance.Config.EnableKill)
							{
								if (args.Length == 1)
								{
									return "callvote Kill <player>";
								}
								else
								{
									Log.Info("Vote called by " + playerNickname + " to " + args[0] + " player " + args[1]);

									List<Player> playerSearch = Player.List.Where(p => p.Nickname.Contains(args[1])).ToList();
									if (playerSearch.Count() > 1)
									{
										return "Multiple players have a name or partial name of " + args[1] + ". Please use a different search string.";
									}
									else if (playerSearch.Count() == 1)
									{
										Player locatedPlayer = playerSearch[0];
										string locatedPlayerName = locatedPlayer.Nickname;

										options[1] = "Yes";
										options[2] = "No";

										voteCoroutine = Timing.RunCoroutine(StartVoteCoroutine(new Vote(playerNickname + " asks: Kill " + locatedPlayerName + "?", options), delegate (Vote vote)
										{
											int votePercent = (int)((float)vote.Counter[1] / (float)(Player.List.Count()) * 100f);
											if (votePercent >= Plugin.Instance.Config.ThresholdKill)
											{
												Map.Broadcast(5, votePercent + "% voted yes. Killing player " + locatedPlayerName + ".");
												if (!locatedPlayer.CheckPermission("cv.untouchable"))
												{
													locatedPlayer.Kill();
												}
											}
											else
											{
												Map.Broadcast(5, "Only " + votePercent + "% voted yes. " + Plugin.Instance.Config.ThresholdKill + "% was required to kill " + locatedPlayerName + ".");
											}
										}));

										break;
									}
									else
									{
										return "Did not find any players with the name or partial name of " + args[1];
									}
								}
							}
							else
							{
								return "callvote Kill is not enabled.";
							}


						case "nuke":
							if (Plugin.Instance.Config.EnableNuke)
							{
								Log.Info("Vote called by " + playerNickname + " to " + args[0]);
								//return new string[] { "To be implemented." };

								options[1] = "Yes";
								options[2] = "No";

								voteCoroutine = Timing.RunCoroutine(StartVoteCoroutine(new Vote(playerNickname + " asks: NUKE THE FACILITY?!?", options), delegate (Vote vote)
								{
									int votePercent = (int)((float)vote.Counter[1] / (float)(Player.List.Count()) * 100f);
									if (votePercent >= Plugin.Instance.Config.ThresholdNuke)
									{
										Map.Broadcast(5, votePercent + "% voted yes. Nuking the facility...");
										Exiled.API.Features.Warhead.Start();
									}
									else
									{
										Map.Broadcast(5, "Only " + votePercent + "% voted yes. " + Plugin.Instance.Config.ThresholdNuke + "% was required to nuke the facility.");
									}
								}));
								break;
							}
							else
							{
								return "callvote Nuke is not enabled.";
							}

						case "respawnwave":
							if (Plugin.Instance.Config.EnableRespawnWave)
							{
								Log.Info("Vote called by " + playerNickname + " to " + args[0]);
								//return new string[] { "To be implemented." };

								options[1] = "No";
								options[2] = "MTF";
								options[3] = "CI";

								voteCoroutine = Timing.RunCoroutine(StartVoteCoroutine(new Vote(playerNickname + " asks: Respawn the next wave?", options), delegate (Vote vote)
								{
									int votePercent = (int)((float)vote.Counter[1] / (float)(Player.List.Count()) * 100f);
									int mtfVotePercent = (int)((float)vote.Counter[2] / (float)(Player.List.Count()) * 100f);
									int ciVotePercent = (int)((float)vote.Counter[3] / (float)(Player.List.Count()) * 100f);
									if (mtfVotePercent >= Plugin.Instance.Config.ThresholdRespawnWave)
									{
										Map.Broadcast(5, mtfVotePercent + "% voted yes. Respawning a wave of Nine-Tailed Fox...");
										Respawning.RespawnManager.Singleton.ForceSpawnTeam(Respawning.SpawnableTeamType.NineTailedFox);

									}
									else if (ciVotePercent >= Plugin.Instance.Config.ThresholdRespawnWave)
									{
										Map.Broadcast(5, ciVotePercent + "% voted yes. Respawning a wave of Chaos Insurgency...");
										Respawning.RespawnManager.Singleton.ForceSpawnTeam(Respawning.SpawnableTeamType.ChaosInsurgency);
									}
									else
									{
										Map.Broadcast(5, votePercent + "% voted no. " + Plugin.Instance.Config.ThresholdRespawnWave + "% was required to respawn the next wave.");
									}
								}));
								break;
							}
							else
							{
								return "callvote RespawnWave is not enabled.";
							}

						case "restartround":
							if (Plugin.Instance.Config.EnableRestartRound)
							{
								Log.Info("Vote called by " + playerNickname + " to " + args[0]);
								//return new string[] { "To be implemented." };

								options[1] = "Yes";
								options[2] = "No";

								voteCoroutine = Timing.RunCoroutine(StartVoteCoroutine(new Vote(playerNickname + " asks: Restart the round?", options), delegate (Vote vote)
								{
									int votePercent = (int)((float)vote.Counter[1] / (float)(Player.List.Count()) * 100f);
									if (votePercent >= Plugin.Instance.Config.ThresholdRestartRound)
									{
										Map.Broadcast(5, votePercent + "% voted yes. Restarting the round...");
										//this.Server.Round.RestartRound();
										PlayerManager.localPlayer.GetComponent<PlayerStats>().Roundrestart();

									}
									else
									{
										Map.Broadcast(5, "Only " + votePercent + "% voted yes. " + Plugin.Instance.Config.ThresholdRestartRound + "% was required to restart the round.");
									}
								}));
								break;
							}
							else
							{
								return "callvote RestartRound is not enabled.";
							}

						default:
							//voteInProgress = true;

							if (args.Length == 1)
							{
								Log.Info("Binary vote called by " + playerNickname + ": " + string.Join(" ", args));
								options[1] = "Yes";
								options[2] = "No";
							}
							else
							{
								Log.Info("Multiple-choice vote called by " + playerNickname + ": " + string.Join(" ", args));
								for (int i = 1; i < args.Length; i++)
								{
									options[i] = args[i];
								}
							}
							voteCoroutine = Timing.RunCoroutine(StartVoteCoroutine(new Vote(playerNickname + " asks: " + args[0], options), null));
							break;
					}
					timeOfLastVote = (int)(DateTime.Now - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
					return "Vote has been started!";
				}
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
			string firstBroadcast = CurrentVote.Question + " Press ~ and type ";
			int counter = 0;
			foreach (KeyValuePair<int, string> kv in CurrentVote.Options)
			{
				if (counter == CurrentVote.Options.Count - 1)
				{
					firstBroadcast += "or ." + kv.Key + " for " + kv.Value + " ";
				}
				else
				{
					firstBroadcast += "." + kv.Key + " for " + kv.Value + ", ";
				}
				counter++;
			}
			int textsize = firstBroadcast.Length / 10;
			Map.Broadcast(5, "<size="+(48-textsize).ToString()+">"+firstBroadcast+ "</size>");
			yield return Timing.WaitForSeconds(5f);
			for (; ; )
			{
				if (timerCounter >= Plugin.Instance.Config.VoteDuration + 1)
				{
					if (CurrentVote.Callback == null)
					{
						string timerBroadcast = "Final results:\n";
						foreach (KeyValuePair<int, string> kv in CurrentVote.Options)
						{
							timerBroadcast += CurrentVote.Options[kv.Key] + " (" + CurrentVote.Counter[kv.Key] + ") ";
							textsize = timerBroadcast.Length / 10;
						}
						Map.Broadcast(5, "<size=" + (48 - textsize).ToString() + ">" + timerBroadcast + "</size>");
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
					Map.Broadcast(1, "<size=" + (48 - textsize).ToString() + ">" + timerBroadcast + "</size>");
				}
				timerCounter++;
				yield return Timing.WaitForSeconds(1f);
			}
		}

		public string StopvoteHandler(Player player)
		{

			if (!player.CheckPermission("cv.stopvote"))
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
