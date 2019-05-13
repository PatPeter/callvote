/*
 * Copyright (c) 2019, Nicholas Solin a.k.a. PatPeter
 * All rights reserved.
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of the Universal Gaming Alliance nor the
 *       names of its contributors may be used to endorse or promote products
 *       derived from this software without specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL NICHOLAS SOLIN BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
using Smod2; 
using Smod2.API;
using Smod2.Config;
using Smod2.Attributes;
using Smod2.EventHandlers;
using Smod2.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Timers;
using System.Linq;
using Smod2.Piping;

namespace Callvote
{
	[PluginDetails(
		author = AssemblyInfo.Author,
		name = AssemblyInfo.Name,
		description = AssemblyInfo.Description,
		id = AssemblyInfo.Id,
		configPrefix = AssemblyInfo.ConfigPrefix,
		langFile = AssemblyInfo.LangFile,
		version = AssemblyInfo.Version,
		SmodMajor = 3,
		SmodMinor = 4,
		SmodRevision = 0
		)]
	class CallvotePlugin : Plugin
	{
		//bool voteInProgress = false;
		internal Vote CurrentVote = null;

		internal string[] AllowedRoles = { "owner", "admin", "moderator" };
		internal int VoteDuration = 30;

		internal bool EnableKick = false;
		internal bool EnableKill = false;
		internal bool EnableNuke = false;
		internal bool EnableRespawnWave = false;
		internal bool EnableRestartRound = false;

		internal int ThresholdKick = 80;
		internal int ThresholdKill = 80;
		internal int ThresholdNuke = 80;
		internal int ThresholdRespawnWave = 80;
		internal int ThresholdRestartRound = 80;

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
			AllowedRoles = this.GetConfigList("callvote_allowed_roles");
			VoteDuration = this.GetConfigInt("callvote_vote_duration");

			EnableKick = this.GetConfigBool("callvote_enable_kick");
			EnableKill = this.GetConfigBool("callvote_enable_kill");
			EnableNuke = this.GetConfigBool("callvote_enable_nuke");
			EnableRespawnWave = this.GetConfigBool("callvote_enable_respawnwave");
			EnableRestartRound = this.GetConfigBool("callvote_enable_restartround");

			ThresholdKick = this.GetConfigInt("callvote_threshold_kick");
			ThresholdKill = this.GetConfigInt("callvote_threshold_kill");
			ThresholdNuke = this.GetConfigInt("callvote_threshold_nuke");
			ThresholdRespawnWave = this.GetConfigInt("callvote_threshold_respawnwave");
			ThresholdRestartRound = this.GetConfigInt("callvote_threshold_restartround");
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
			this.AddConfig(new Smod2.Config.ConfigSetting("callvote_allowed_roles", new string[] { "owner", "admin", "moderator" }, true, "List of role allowed to call custom votes."));
			this.AddConfig(new Smod2.Config.ConfigSetting("callvote_vote_duration", 30, true, "Number of seconds for a vote to last for."));

			this.AddConfig(new Smod2.Config.ConfigSetting("callvote_enable_kick", false, true, "Enable callvote Kick."));
			this.AddConfig(new Smod2.Config.ConfigSetting("callvote_enable_kill", false, true, "Enable callvote Kill."));
			this.AddConfig(new Smod2.Config.ConfigSetting("callvote_enable_nuke", false, true, "Enable callvote Nuke."));
			this.AddConfig(new Smod2.Config.ConfigSetting("callvote_enable_respawnwave", false, true, "Enable callvote RespawnWave."));
			this.AddConfig(new Smod2.Config.ConfigSetting("callvote_enable_restartround", false, true, "Enable callvote RestartRound."));

			this.AddConfig(new Smod2.Config.ConfigSetting("callvote_threshold_kick", 80, true, "The percentage needed to kick a user."));
			this.AddConfig(new Smod2.Config.ConfigSetting("callvote_threshold_kill", 80, true, "The percentage needed to kill a user."));
			this.AddConfig(new Smod2.Config.ConfigSetting("callvote_threshold_nuke", 80, true, "The percentage needed to nuke the facility."));
			this.AddConfig(new Smod2.Config.ConfigSetting("callvote_threshold_respawnwave", 80, true, "The percentage needed to respawn a wave."));
			this.AddConfig(new Smod2.Config.ConfigSetting("callvote_threshold_restartround", 80, true, "The percentage needed to restart a round."));
		}

		[PipeMethod]
		public bool CanCallVotes(Player player)
		{
			if (player == null)
			{
				return false;
			}

			foreach (string role in AllowedRoles)
			{
				if (player.GetUserGroup() != null)
				{
					if (String.Equals(role, player.GetUserGroup().Name, StringComparison.CurrentCultureIgnoreCase))
					{
						return true;
					}
					else if (String.Equals(role, player.GetUserGroup().BadgeText, StringComparison.CurrentCultureIgnoreCase))
					{
						return true;
					}
				}
				if (String.Equals(role, player.GetRankName(), StringComparison.CurrentCultureIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		public string CallvoteHandler(Player player, string[] args) // lowercase to match command
		{
			this.Info(player.Name + " called vote with arguments: ");
			for (int i = 0; i < args.Length; i++)
			{
				this.Info("\t" + i + ": " + args[i]);
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
							if (this.EnableKick)
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

										StartVote(new Vote(player.Name + " asks: Kick " + locatedPlayer.Name + "?", options), delegate(Vote vote)
										{
											int votePercent = (int) ((float)vote.Counter[1] / (float)(this.Server.NumPlayers - 1) * 100f);
											if (votePercent >= this.ThresholdKick)
											{
												this.Server.Map.Broadcast(5, votePercent + "% voted yes. Kicking player " + locatedPlayer.Name + ".", false);
												locatedPlayer.Ban(0);
											}
											else
											{
												this.Server.Map.Broadcast(5, "Only " + votePercent + "% voted yes. " + this.ThresholdKick + "% was required to kick " + locatedPlayer.Name + ".", false);
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
							if (this.EnableKill)
							{
								if (args.Length == 1)
								{
									return "callvote Kill <player>";
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

										StartVote(new Vote(player.Name + " asks: Kill " + locatedPlayer.Name + "?", options), delegate (Vote vote)
										{
											int votePercent = (int)((float)vote.Counter[1] / (float)(this.Server.NumPlayers - 1) * 100f);
											if (votePercent >= this.ThresholdKill)
											{
												this.Server.Map.Broadcast(5, votePercent + "% voted yes. Killing player " + locatedPlayer.Name + ".", false);
												locatedPlayer.Kill();
											}
											else
											{
												this.Server.Map.Broadcast(5, "Only " + votePercent + "% voted yes. " + this.ThresholdKill + "% was required to kill " + locatedPlayer.Name + ".", false);
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
							if (EnableNuke)
							{
								this.Info("Vote called by " + player.Name + " to " + args[0]);
								//return new string[] { "To be implemented." };

								options[1] = "Yes";
								options[2] = "No";

								StartVote(new Vote(player.Name + " asks: NUKE THE FACILITY?!?", options), delegate (Vote vote)
								{
									int votePercent = (int)((float)vote.Counter[1] / (float)(this.Server.NumPlayers - 1) * 100f);
									if (votePercent >= this.ThresholdNuke)
									{
										this.Server.Map.Broadcast(5, votePercent + "% voted yes. Nuking the facility...", false);
										this.Server.Map.DetonateWarhead();
									}
									else
									{
										this.Server.Map.Broadcast(5, "Only " + votePercent + "% voted yes. " + this.ThresholdNuke + "% was required to nuke the facility.", false);
									}
								});
								break;
							}
							else
							{
								return "callvote Nuke is not enabled.";
							}

						case "respawnwave":
							if (EnableRespawnWave)
							{
								this.Info("Vote called by " + player.Name + " to " + args[0]);
								//return new string[] { "To be implemented." };

								options[1] = "No";
								options[2] = "MTF";
								options[3] = "CI";

								StartVote(new Vote(player.Name + " asks: Respawn the next wave?", options), delegate (Vote vote)
								{
									int votePercent = (int)((float)vote.Counter[1] / (float)(this.Server.NumPlayers - 1) * 100f);
									int mtfVotePercent = (int)((float)vote.Counter[2] / (float)(this.Server.NumPlayers - 1) * 100f);
									int ciVotePercent = (int)((float)vote.Counter[3] / (float)(this.Server.NumPlayers - 1) * 100f);
									if (mtfVotePercent >= this.ThresholdRespawnWave)
									{
										this.Server.Map.Broadcast(5, mtfVotePercent + "% voted yes. Respawning a wave of Nine-Tailed Fox...", false);
										this.Server.Round.MTFRespawn(false);
									}
									else if (ciVotePercent >= this.ThresholdRespawnWave)
									{
										this.Server.Map.Broadcast(5, ciVotePercent + "% voted yes. Respawning a wave of Chaos Insurgency...", false);
										this.Server.Round.MTFRespawn(true);
									}
									else
									{
										this.Server.Map.Broadcast(5, votePercent + "% voted no. " + this.ThresholdRespawnWave + "% was required to respawn the next wave.", false);
									}
								});
								break;
							}
							else
							{
								return "callvote RespawnWave is not enabled.";
							}

						case "restartround":
							if (EnableRestartRound)
							{
								this.Info("Vote called by " + player.Name + " to " + args[0]);
								//return new string[] { "To be implemented." };

								options[1] = "Yes";
								options[2] = "No";

								StartVote(new Vote(player.Name + " asks: Restart the round?", options), delegate (Vote vote)
								{
									int votePercent = (int)((float)vote.Counter[1] / (float)(this.Server.NumPlayers - 1) * 100f);
									if (votePercent >= this.ThresholdRestartRound)
									{
										this.Server.Map.Broadcast(5, votePercent + "% voted yes. Restarting the round...", false);
										this.Server.Round.RestartRound();
									}
									else
									{
										this.Server.Map.Broadcast(5, "Only " + votePercent + "% voted yes. " + this.ThresholdRestartRound + "% was required to restart the round.", false);
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
								return "Your group, " + (player.GetUserGroup() != null ? player.GetUserGroup().Name + "/" + player.GetUserGroup().BadgeText + "/" : "\"" + player.GetRankName() + "\"") + ", is not allowed to call votes.";
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
							StartVote(new Vote(player.Name + " asks: " + args[0], options), null);
							break;
					}
					return "Vote has been started!";
				}
			}
		}

		[PipeMethod]
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

		[PipeEvent("patpeter.callvote.OnStartVote")]
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
			this.Server.Map.Broadcast(5, firstBroadcast, false);

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

				if (timerCounter >= this.VoteDuration + 1)
				{
					if (CurrentVote.Callback == null)
					{
						string timerBroadcast = "Final results:\n";
						foreach (KeyValuePair<int, string> kv in CurrentVote.Options)
						{
							timerBroadcast += CurrentVote.Options[kv.Key] + " (" + CurrentVote.Counter[kv.Key] + ") ";
						}
						this.Server.Map.Broadcast(5, timerBroadcast, false);
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
					this.Server.Map.Broadcast(1, timerBroadcast, false);
				}
				timerCounter++;
			};
			//return new string[] { "Vote has been started!" };
			return true;
		}
		
		public string StopvoteHandler(Player player)
		{
			if (!CanCallVotes(player))
			{
				return "Your group, " + (player.GetUserGroup() != null ? player.GetUserGroup().Name + "/" + player.GetUserGroup().BadgeText + "/" : "\"" + player.GetRankName() + "\"") + ", is not allowed to stop votes.";
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

		[PipeMethod]
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
			if (CurrentVote != null)
			{
				if (!CurrentVote.Votes.Contains(player.SteamId))
				{
					if (CurrentVote.Options.ContainsKey(option))
					{
						CurrentVote.Counter[option]++;
						CurrentVote.Votes.Add(player.SteamId);
						this.Info("Player " + player.Name + " voted " + CurrentVote.Options[option] + " bringing the counter to " + CurrentVote.Counter[option]);
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
