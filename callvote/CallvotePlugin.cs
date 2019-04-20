using Smod2;
using Smod2.API;
using Smod2.Config;
using Smod2.Attributes;
using Smod2.EventHandlers;
using Smod2.Events;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Linq;

namespace Callvote
{
	[PluginDetails(
		author = "PatPeter",
		name = "callvote",
		description = "callvote command like in the Source engine. Vote to kick users, restart round, or make your own custom votes.",
		id = "patpeter.callvote",
		version = "1.1.0.14",
		// 3.4.0 is not compatible with SettingType
		SmodMajor = 3,
		SmodMinor = 3,
		SmodRevision = 0
		)]
	class CallvotePlugin : Plugin
	{
		//bool voteInProgress = false;
		public Vote currentVote = null;

		public string[] allowedRoles = { "owner", "admin", "moderator" };
		public int voteDuration = 30;
		public bool enableKick = false;
		public bool enableRestartRound = false;
		public int thresholdKick = 80;
		public int thresholdRestartRound = 80;

		public override void OnDisable()
		{
			this.Info(this.Details.name + " was disabled ):");
		}

		public override void OnEnable()
		{
			this.Info(this.Details.name + " has loaded :)");
		}

		public void ReloadConfig()
		{
			allowedRoles = this.GetConfigList("callvote_allowed_roles");
			voteDuration = this.GetConfigInt("callvote_vote_duration");
			enableKick = this.GetConfigBool("callvote_enable_kick");
			enableRestartRound = this.GetConfigBool("callvote_enable_restartround");
			thresholdKick = this.GetConfigInt("callvote_threshold_kick");
			thresholdRestartRound = this.GetConfigInt("callvote_threshold_restartround");
		}

		public override void Register()
		{
			// Register multiple events
			//this.AddEventHandlers(new RoundEventHandler(this));
			//Register multiple events with Low Priority
			this.AddEventHandlers(new CallvoteEvents(this), Priority.Normal);
			// Register single event with priority (need to specify the handler type)
			//this.AddEventHandler(typeof(IEventHandlerPlayerPickupItem), new LottoItemHandler(this), Priority.High);
			// Register Command(s)
			this.AddCommand("callvote", new CallvoteCommand(this));
			this.AddCommand("stopvote", new StopvoteCommand(this));
			this.AddCommand("yes", new Vote1Command(this));
			this.AddCommand("no", new Vote2Command(this));
			this.AddCommand("1", new Vote1Command(this));
			this.AddCommand("2", new Vote2Command(this));
			this.AddCommand("3", new Vote3Command(this));
			this.AddCommand("4", new Vote4Command(this));
			this.AddCommand("5", new Vote5Command(this));
			this.AddCommand("6", new Vote6Command(this));
			this.AddCommand("7", new Vote7Command(this));
			this.AddCommand("8", new Vote8Command(this));
			this.AddCommand("9", new Vote9Command(this));
			this.AddCommand("0", new Vote0Command(this));
			// Register config setting(s)
			this.AddConfig(new Smod2.Config.ConfigSetting("callvote_allowed_roles", new string[] { "owner", "admin", "moderator" }, SettingType.LIST, true, "List of role allowed "));
			this.AddConfig(new Smod2.Config.ConfigSetting("callvote_vote_duration", 30, SettingType.NUMERIC, true, ""));
			this.AddConfig(new Smod2.Config.ConfigSetting("callvote_enable_kick", false, SettingType.BOOL, true, "Enable callvote Kick."));
			this.AddConfig(new Smod2.Config.ConfigSetting("callvote_enable_restartround", false, SettingType.BOOL, true, "Enable callvote RestartRound."));
			this.AddConfig(new Smod2.Config.ConfigSetting("callvote_threshold_kick", 80, SettingType.NUMERIC, true, "The percentage needed to kick a user."));
			this.AddConfig(new Smod2.Config.ConfigSetting("callvote_threshold_restartround", 80, SettingType.NUMERIC, true, "The percentage needed to restart a round."));
		}

		public bool canCallVotes(Player player)
		{
			foreach (string role in allowedRoles)
			{
				if (String.Equals(role, player.GetUserGroup().Name, StringComparison.CurrentCultureIgnoreCase) || String.Equals(role, player.GetRankName(), StringComparison.CurrentCultureIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		public string startVote(Player player, string[] args)
		{

			this.Info(player.Name + " called vote with arguments: ");
			for (int i = 0; i < args.Length; i++)
			{
				this.Info("\t" + i + ": " + args[i]);
			}
			if (args.Length == 0)
			{
				//return new string[] { "callvote RestartRound", "callvote Kick <player>", "callvote <custom> [options]" };
				return "callvote RestartRound/Kick/<custom> <player>/[options]";
			}
			else
			{
				if (currentVote != null)
				{
					//return new string[] { "A vote is currently in progress." };
					return "A vote is currently in progress.";
				}
				else
				{
					Dictionary<int, string> options = new Dictionary<int, string>();
					switch (args[0])
					{
						case "RestartRound":
							if (enableRestartRound)
							{
								this.Info("Vote called by " + player.Name + " to " + args[0]);
								//return new string[] { "To be implemented." };

								options[1] = "Yes";
								options[2] = "No";

								currentVote = new Vote("Restart the round?", options);
								currentVote.response = delegate(Vote vote)
								{
									int votePercent = (int) ((float)vote.counter[1] / (float)this.Server.NumPlayers * 100f);
									if (votePercent >= this.thresholdRestartRound)
									{
										this.Server.Map.Broadcast(5, votePercent + " voted yes. Restarting the round...", false);
										this.Server.Round.RestartRound();
									}
									else
									{
										this.Server.Map.Broadcast(5, "Only " + votePercent + "% voted yes. " + this.thresholdRestartRound + "% was required.", false);
									}
								};
								break;
							}
							else
							{
								return "callvote RestartRound is not enabled.";
							}
							

						case "Kick":
							if (this.enableKick)
							{
								if (args.Length == 1)
								{
									return "callvote Kick <player>";
								}
								else
								{
									this.Info("Vote called by " + player.Name + " to " + args[0] + " player " + args[1]);

									List<Player> playerSearch = this.Server.GetPlayers().Where(p => p.Name.Contains(args[1])).ToList();
									if (playerSearch.Count() > 1)
									{
										return "Multiple players have a name or partial name of " + args[1] + ". Please use a different search string.";
									}
									else if (playerSearch.Count() == 1)
									{
										Player locatedPlayer = playerSearch[0];

										options[1] = "Yes";
										options[2] = "No";

										currentVote = new Vote("Should the player " + locatedPlayer.Name + " be kicked?", options);

										currentVote.response = delegate(Vote vote)
										{
											int votePercent = (int) ((float)vote.counter[1] / (float)this.Server.NumPlayers * 100f);
											if (votePercent >= this.thresholdKick)
											{
												this.Server.Map.Broadcast(5, votePercent + " voted yes. Kicking player " + locatedPlayer.Name + ".", false);
												locatedPlayer.Ban(0);
											}
											else
											{
												this.Server.Map.Broadcast(5, "Only " + votePercent + "% voted yes. " + this.thresholdKick + "% was required to kick " + locatedPlayer.Name + ".", false);
											}
										};

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

						default:
							//voteInProgress = true;
							if (!canCallVotes(player))
							{
								return "You cannot call votes.";
							}
							
							if (args.Length == 1)
							{
								this.Info("Binary vote called by " + player.Name + ": " + string.Join(" ", args));
								options[1] = "Yes";
								options[2] = "No";
							}
							else
							{
								this.Info("Multiple-choice vote called by " + player.Name + ": " + string.Join(" ", args));
								for (int i = 1; i < args.Length; i++)
								{
									options[i] = args[i];
								}
							}
							currentVote = new Vote(args[0], options);
							break;
					}

					string firstBroadcast = currentVote.question + " Press ~ and type ";
					int counter = 0;
					foreach (KeyValuePair<int, string> kv in currentVote.options)
					{
						if (counter == currentVote.options.Count - 1)
						{
							firstBroadcast += "or ." + kv.Key + " for " + kv.Value + " ";
						}
						else
						{
							firstBroadcast += "." + kv.Key + " for " + kv.Value + ", ";
						}
						counter++;
					}
					this.Server.Map.Broadcast(5, firstBroadcast, false);

					int timerCounter = 0;
					currentVote.timer = new Timer
					{
						Interval = 5000,
						Enabled = true,
						AutoReset = true
					};
					currentVote.timer.Elapsed += delegate
					{
						if (currentVote.timer.Interval == 5000)
						{
							currentVote.timer.Interval = 1000;
						}

						if (timerCounter >= this.voteDuration + 1)
						{
							if (currentVote.response == null)
							{
								string timerBroadcast = "Final results:\n";
								foreach (KeyValuePair<int, string> kv in currentVote.options)
								{
									timerBroadcast += currentVote.options[kv.Key] + " (" + currentVote.counter[kv.Key] + ") ";
								}
								this.Server.Map.Broadcast(5, timerBroadcast, false);
							}
							else
							{
								currentVote.response.Invoke(currentVote);
							}

							currentVote.timer.Enabled = false;
							currentVote = null;
						}
						else
						{
							string timerBroadcast = firstBroadcast + "\n";
							foreach (KeyValuePair<int, string> kv in currentVote.options)
							{
								timerBroadcast += currentVote.options[kv.Key] + " (" + currentVote.counter[kv.Key] + ") ";
							}
							this.Server.Map.Broadcast(1, timerBroadcast, false);
						}
						timerCounter++;
					};
					//return new string[] { "Vote has been started!" };
					return "Vote has been started!";
				}
			}
		}

		public string stopVote(Player player)
		{
			if (!canCallVotes(player))
			{
				return "You cannot stop votes.";
			}

			if (this.currentVote != null)
			{
				if (this.currentVote.timer != null)
				{
					this.currentVote.timer.Stop();
					this.currentVote = null;
					return "Vote stopped.";
				}
				else
				{
					this.currentVote = null;
					return "Vote stopped.";
				}
			}
			else
			{
				return "There is not a vote in progress.";
			}
		}

		public string handleVote(Player player, int option)
		{
			if (currentVote != null)
			{
				if (!currentVote.votes.Contains(player.SteamId))
				{
					if (currentVote.options.ContainsKey(option))
					{
						currentVote.counter[option]++;
						currentVote.votes.Add(player.SteamId);
						this.Info("Player " + player.Name + " voted " + currentVote.options[option] + " bringing the counter to " + currentVote.counter[option]);
						//return new string[] { "Vote accepted!" };
						return "Vote accepted!";
					} else
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

	delegate void CallvoteFunction(Vote vote);

	class Vote
	{
		public string question;
		public Dictionary<int, string> options = new Dictionary<int, string>();
		public HashSet<string> votes = new HashSet<string>();
		public Dictionary<int, int> counter = new Dictionary<int, int>();
		public Timer timer;
		public CallvoteFunction response;

		public Vote(string question, Dictionary<int, string> options)
		{
			this.question = question;
			this.options = options;
			foreach (int option in options.Keys)
			{
				counter[option] = 0;
			}
		}
	}
}
