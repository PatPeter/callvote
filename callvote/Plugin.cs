using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;

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

			EventHandlers = null;
		}

		public override void OnReloaded()
		{
			//This is only fired when you use the EXILED reload command, the reload command will call OnDisable, OnReload, reload the plugin, then OnEnable in that order. There is no GAC bypass, so if you are updating a plugin, it must have a unique assembly name, and you need to remove the old version from the plugins folder
		}

		public override string Name { get; } = "callvote";

		/*public void ReloadConfig()
		{
			List<string> allowedRoles = Config.GetStringList("callvote_allowed_roles");
			if (allowedRoles.Count != 0)
			{
				AllowedRoles = allowedRoles.ToArray();
			}
			else
			{
				AllowedRoles = new string[] { "owner", "admin", "moderator" };
			}
			VoteDuration = Config.GetInt("callvote_vote_duration", 30);
			VoteCooldown = Config.GetInt("callvote_vote_cooldown", 30);

			EnableKick = Config.GetBool("callvote_enable_kick", false);
			EnableKill = Config.GetBool("callvote_enable_kill", false);
			EnableNuke = Config.GetBool("callvote_enable_nuke", false);
			EnableRespawnWave = Config.GetBool("callvote_enable_respawnwave", false);
			EnableRestartRound = Config.GetBool("callvote_enable_restartround", false);

			ThresholdKick = Config.GetInt("callvote_threshold_kick", 80);
			ThresholdKill = Config.GetInt("callvote_threshold_kill", 80);
			ThresholdNuke = Config.GetInt("callvote_threshold_nuke", 80);
			ThresholdRespawnWave = Config.GetInt("callvote_threshold_respawnwave", 80);
			ThresholdRestartRound = Config.GetInt("callvote_threshold_restartround", 80);
		}*/

		public bool CanCallVotes(Player player)
		{
			if (player == null)
			{
				return false;
			}
			string playerGroupName = player.GroupName;
			string playerBadgeName = player.RankName;
			//UserGroup playerGroup = Player.GetRank(player);

			foreach (string role in Plugin.Instance.Config.AllowedRoles)
			{
				//if (playerGroup != null)
				//{
				if (String.Equals(role, playerGroupName, StringComparison.CurrentCultureIgnoreCase))
				{
					return true;
				}
				else if (String.Equals(role, playerBadgeName, StringComparison.CurrentCultureIgnoreCase))
				{
					return true;
				}
				//}
				//if (String.Equals(role, player.GetRankName(), StringComparison.CurrentCultureIgnoreCase))
				//{
				//	return true;
				//}
			}
			return false;
		}

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
			if (args.Length == 0)
			{
				//return new string[] { "callvote RestartRound", "callvote Kick <player>", "callvote <custom> [options]" };
				return "callvote Kick/Kill/<custom> <player>/[options]";
			}
			else
			{
				if (CurrentVote != null)
				{
					//return new string[] { "A vote is currently in progress." };
					return "A vote is currently in progress.";
				}
				else
				{
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

										StartVote(new Vote(playerNickname + " asks: Kick " + locatedPlayerName + "?", options), delegate (Vote vote)
										{
											int votePercent = (int)((float)vote.Counter[1] / (float)(Player.List.Count() - 1) * 100f);
											if (votePercent >= Plugin.Instance.Config.ThresholdKick)
											{
												Map.Broadcast(5, votePercent + "% voted yes. Kicking player " + locatedPlayerName + ".");
												locatedPlayer.Kick(votePercent + "% voted to kick you.");
											}
											else
											{
												Map.Broadcast(5, "Only " + votePercent + "% voted yes. " + Plugin.Instance.Config.ThresholdKick + "% was required to kick " + locatedPlayerName + ".");
											}
										});

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

										StartVote(new Vote(playerNickname + " asks: Kill " + locatedPlayerName + "?", options), delegate (Vote vote)
										{
											int votePercent = (int)((float)vote.Counter[1] / (float)(Player.List.Count() - 1) * 100f);
											if (votePercent >= Plugin.Instance.Config.ThresholdKill)
											{
												Map.Broadcast(5, votePercent + "% voted yes. Killing player " + locatedPlayerName + ".");
												locatedPlayer.Kill();
											}
											else
											{
												Map.Broadcast(5, "Only " + votePercent + "% voted yes. " + Plugin.Instance.Config.ThresholdKill + "% was required to kill " + locatedPlayerName + ".");
											}
										});

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

								StartVote(new Vote(playerNickname + " asks: NUKE THE FACILITY?!?", options), delegate (Vote vote)
								{
									int votePercent = (int)((float)vote.Counter[1] / (float)(Player.List.Count() - 1) * 100f);
									if (votePercent >= Plugin.Instance.Config.ThresholdNuke)
									{
										Map.Broadcast(5, votePercent + "% voted yes. Nuking the facility...");
										Exiled.API.Features.Warhead.Detonate();
									}
									else
									{
										Map.Broadcast(5, "Only " + votePercent + "% voted yes. " + Plugin.Instance.Config.ThresholdNuke + "% was required to nuke the facility.");
									}
								});
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

								StartVote(new Vote(playerNickname + " asks: Respawn the next wave?", options), delegate (Vote vote)
								{
									int votePercent = (int)((float)vote.Counter[1] / (float)(Player.List.Count() - 1) * 100f);
									int mtfVotePercent = (int)((float)vote.Counter[2] / (float)(Player.List.Count() - 1) * 100f);
									int ciVotePercent = (int)((float)vote.Counter[3] / (float)(Player.List.Count() - 1) * 100f);
									if (mtfVotePercent >= Plugin.Instance.Config.ThresholdRespawnWave)
									{
										Map.Broadcast(5, mtfVotePercent + "% voted yes. Respawning a wave of Nine-Tailed Fox...");
										//this.Server.Round.MTFRespawn(false);
										//Respawning.RespawnManager.Singleton.ForceSpawnTeam(Respawning.SpawnableTeamType.NineTailedFox);
										//PlayerManager.localPlayer.GetComponent<MTFRespawn>().nextWaveIsCI = false;
										//PlayerManager.localPlayer.GetComponent<MTFRespawn>().RespawnDeadPlayers();
										//Respawning.RespawnManager.Singleton.NextKnownTeam = Respawning.SpawnableTeamType.NineTailedFox;
										Respawning.RespawnManager.Singleton.ForceSpawnTeam(Respawning.SpawnableTeamType.NineTailedFox);

									}
									else if (ciVotePercent >= Plugin.Instance.Config.ThresholdRespawnWave)
									{
										Map.Broadcast(5, ciVotePercent + "% voted yes. Respawning a wave of Chaos Insurgency...");
										//this.Server.Round.MTFRespawn(true);
										//Respawning.RespawnManager.Singleton.ForceSpawnTeam(Respawning.SpawnableTeamType.ChaosInsurgency);
										//PlayerManager.localPlayer.GetComponent<MTFRespawn>().nextWaveIsCI = true;
										//PlayerManager.localPlayer.GetComponent<MTFRespawn>().RespawnDeadPlayers();
										//Respawning.RespawnManager.Singleton.NextKnownTeam = Respawning.SpawnableTeamType.ChaosInsurgency;
										Respawning.RespawnManager.Singleton.ForceSpawnTeam(Respawning.SpawnableTeamType.ChaosInsurgency);
									}
									else
									{
										Map.Broadcast(5, votePercent + "% voted no. " + Plugin.Instance.Config.ThresholdRespawnWave + "% was required to respawn the next wave.");
									}
								});
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

								StartVote(new Vote(playerNickname + " asks: Restart the round?", options), delegate (Vote vote)
								{
									int votePercent = (int)((float)vote.Counter[1] / (float)(Player.List.Count() - 1) * 100f);
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
								});
								break;
							}
							else
							{
								return "callvote RestartRound is not enabled.";
							}

						default:
							//voteInProgress = true;
							if (!CanCallVotes(player))
							{
								return "Your group, " + playerGroupName + "/" + playerBadgeName + ", is not allowed to call votes.";
							}

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
							StartVote(new Vote(playerNickname + " asks: " + args[0], options), null);
							break;
					}
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
			StartVote(newVote, null);
		}

		public bool StartVote(Vote newVote, CallvoteFunction callback)
		{
			if (CurrentVote != null)
			{
				return false;
			}

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
			Map.Broadcast(5, firstBroadcast);

			int timerCounter = 0;
			CurrentVote.Timer = new Timer
			{
				Interval = 5000,
				Enabled = true,
				AutoReset = true
			};
			CurrentVote.Timer.Elapsed += delegate
			{
				if (CurrentVote.Timer.Interval == 5000)
				{
					CurrentVote.Timer.Interval = 1000;
				}

				if (timerCounter >= Plugin.Instance.Config.VoteDuration + 1)
				{
					if (CurrentVote.Callback == null)
					{
						string timerBroadcast = "Final results:\n";
						foreach (KeyValuePair<int, string> kv in CurrentVote.Options)
						{
							timerBroadcast += CurrentVote.Options[kv.Key] + " (" + CurrentVote.Counter[kv.Key] + ") ";
						}
						Map.Broadcast(5, timerBroadcast);
					}
					else
					{
						CurrentVote.Callback.Invoke(CurrentVote);
					}

					CurrentVote.Timer.Enabled = false;
					CurrentVote = null;
				}
				else
				{
					string timerBroadcast = firstBroadcast + "\n";
					foreach (KeyValuePair<int, string> kv in CurrentVote.Options)
					{
						timerBroadcast += CurrentVote.Options[kv.Key] + " (" + CurrentVote.Counter[kv.Key] + ") ";
					}
					Map.Broadcast(1, timerBroadcast);
				}
				timerCounter++;
			};
			//return new string[] { "Vote has been started!" };
			return true;
		}

		public string StopvoteHandler(Player player)
		{
			string playerGroupName = player.GroupName;
			string playerBadgeName = player.RankName;
			if (!CanCallVotes(player))
			{
				return "Your group, " + playerGroupName + "/" + playerBadgeName + ", is not allowed to stop votes.";
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
			if (this.CurrentVote != null)
			{
				if (this.CurrentVote.Timer != null)
				{
					this.CurrentVote.Timer.Stop();
				}
				this.CurrentVote = null;
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
