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


namespace callvote
{
	public class Plugin : Plugin<Config, Translation>
	{
		public static Plugin Instance;

		public override string Name { get; } = Callvote.AssemblyInfo.Name;
		public override string Author { get; } = Callvote.AssemblyInfo.Author;
		public override System.Version Version { get; } = new System.Version(Callvote.AssemblyInfo.Version);
		public override string Prefix { get; } = Callvote.AssemblyInfo.LangFile;
		public override System.Version RequiredExiledVersion { get; } = new System.Version(5, 1, 3);
		public override PluginPriority Priority { get; } = PluginPriority.Default;

		//Instance variable for eventhandlers
		public EventHandlers EventHandlers;


		//custom implementation
		
		//bool voteInProgress = false;
		internal Vote CurrentVote = null;
		// private IEnumerator<float> CallVoteTimer()
		// {
		// 	yield return Timing.WaitForSeconds(Config.MaxWaitGeneral);
		// 	//Timing.
		// 	//IsGeneralVoteEnabled = true;
		// }
		private int roundtimer = 0;
		private bool IsKillVoteEnabled = false;
		private IEnumerator<float> Update()
		{
			//var timerGeneral = 0f;
			while(true)
			{

				roundtimer = RoundStart.RoundLength.Seconds;
				yield return Timing.WaitForOneFrame;
			}
		}
		
		public override void OnEnabled()
		{
			try
			{
				Log.Debug("Initializing event handlers..");
				//Set instance varible to a new instance, this should be nulled again in OnDisable
				EventHandlers = new EventHandlers(this);
				Instance = this;
				//Hook the events you will be using in the plugin. You should hook all events you will be using here, all events should be unhooked in OnDisabled 
				Exiled.Events.Handlers.Server.WaitingForPlayers += EventHandlers.OnWaitingForPlayers;
				Exiled.Events.Handlers.Server.RoundEnded += EventHandlers.OnRoundEnded;
				Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
				if(RoundStart.RoundLength.Seconds > 0) {Timing.RunCoroutine(Update());};
				Log.Debug($"callvote loaded!");
			}
			catch (Exception e)
			{
				//This try catch is redundant, as EXILED will throw an error before this block can, but is here as an example of how to handle exceptions/errors
				Log.Error($"There was an error loading the plugin: {e}");
			}
		}
		private void OnRoundStarted()
		{
			Timing.RunCoroutine(Update());
		}
		public override void OnDisabled()
		{

			Exiled.Events.Handlers.Server.WaitingForPlayers -= EventHandlers.OnWaitingForPlayers;
			Exiled.Events.Handlers.Server.RoundEnded -= EventHandlers.OnRoundEnded;
			Instance = null;
			EventHandlers = null;
		}

		public override void OnReloaded()
		{
			//This is only fired when you use the EXILED reload command, the reload command will call OnDisable, OnReload, reload the plugin, then OnEnable in that order. There is no GAC bypass, so if you are updating a plugin, it must have a unique assembly name, and you need to remove the old version from the plugins folder
		}
		
		public Dictionary<int, int> DictionaryOfVotes = new Dictionary<int, int>();
		public int timeOfLastVote = 0;
		private CoroutineHandle voteCoroutine = new CoroutineHandle();
		public string CallvoteHandler(Player player, string[] args) // lowercase to match command
		{
			string playerNickname = player.Nickname;
			string playerGroupName = player.GroupName;
			string playerBadgeName = player.RankName;
			Log.Debug(playerNickname + " called vote with arguments: ");
			for (int i = 0; i < args.Length; i++)
			{
				Log.Debug("\t" + i + ": " + args[i]);
			}
			if (DictionaryOfVotes.ContainsKey(player.Id))
			{
				if(DictionaryOfVotes[player.Id] > Plugin.Instance.Config.MaxAmountOfVotesPerRound -1 && !player.CheckPermission("cv.unlimitedvotes"))
				{
					return Translation.MaxVote;
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
				if (CurrentVote != null) { return Translation.InProgressVote; }
				if (timestamp - timeOfLastVote < Plugin.Instance.Config.VoteCooldown){ return Translation.WaitToVote.Replace("%Timer%", $"{Config.VoteCooldown-(timestamp - timeOfLastVote)}"); }
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
					    //Lazy to change it in other ifs
						if (!Plugin.Instance.Config.EnableKick && roundtimer > Config.MaxWaitKick && player.CheckPermission("cv.callvotekick") || player.CheckPermission("cv.bypass"))
						{
							if (!player.CheckPermission("cv.callvotekick")) {return Translation.NoPermissionToVote;}
							if(roundtimer < Config.MaxWaitRestartRound) {return Translation.WaitToVote.Replace("%Timer%",$"{Config.MaxWaitKick-roundtimer}");}
							return Translation.VoteKickDisabled;
						}
						if (args.Length == 1)
						{
							return "callvote Kick <player>";
						}
						else
						{
						List<Player> playerSearch = Player.List.Where(p => p.Nickname.Contains(args[1])).ToList();
						if (playerSearch.Count() > 1)
						{
							return Translation.PlayersWithSameName.Replace("%Player%", args[1]);

						}
						else if (playerSearch.Count() == 1)
						{
							Player locatedPlayer = playerSearch[0];
							string locatedPlayerName = locatedPlayer.Nickname;

							options[1] = Translation.OptionYes;
							options[2] = Translation.OptionNo;

							voteCoroutine = Timing.RunCoroutine(StartVoteCoroutine(new Vote(Translation.AskedToKick.Replace("%Player%", playerNickname).Replace("%Offender%", locatedPlayerName), options), delegate (Vote vote)
							{
								int votePercent = (int)((float)vote.Counter[1] / (float)(Player.List.Count()) * 100f);
								if (votePercent >= Plugin.Instance.Config.ThresholdKick)
								{
									Map.Broadcast(5, Translation.PlayerGettingKicked.Replace("%VotePercent%", votePercent.ToString()).Replace("%player%", locatedPlayerName));
									if(!locatedPlayer.CheckPermission("cv.untouchable"))
									{
										locatedPlayer.Kick(Translation.Untouchable.Replace("%VotePercent%", votePercent.ToString()));
									}
												
								}
								else
								{
									Map.Broadcast(5, Translation.NotSuccessFullKick.Replace("%VotePercent%", votePercent.ToString()).Replace("%ThresholdKick%", Plugin.Instance.Config.ThresholdKick.ToString()).Replace("%Offender%", locatedPlayerName));
								}
							}));

							break;
						}
						else
						{
							return Translation.PlayerNotFound.Replace("%Player", args[1]);
						}
					}
						case "kill":
							if (Config.EnableKill == true && roundtimer > Config.MaxWaitKill && player.CheckPermission("cv.callvotekill")|| player.CheckPermission("cv.bypass"))
							{
								if (args.Length == 1)
								{
									return "callvote Kill <player>";
								}
								else
								{
									Log.Debug("Vote called by " + playerNickname + " to " + args[0] + " player " + args[1]);

									List<Player> playerSearch = Player.List.Where(p => p.Nickname.Contains(args[1])).ToList();
									if (playerSearch.Count() > 1)
									{
										return Translation.PlayersWithSameName.Replace("%Player%", args[1]);
									}
									else if (playerSearch.Count() == 1)
									{
										Player locatedPlayer = playerSearch[0];
										string locatedPlayerName = locatedPlayer.Nickname;

										options[1] = Translation.OptionYes;
										options[2] = Translation.OptionNo;

										voteCoroutine = Timing.RunCoroutine(StartVoteCoroutine(new Vote(Translation.AskedToKill.Replace("%Offender%", locatedPlayerName).Replace("%Player%", playerNickname), options), delegate (Vote vote)
										{
											int votePercent = (int)((float)vote.Counter[1] / (float)(Player.List.Count()) * 100f);
											if (votePercent >= Plugin.Instance.Config.ThresholdKill)
											{
												Map.Broadcast(5, Translation.PlayerKilled.Replace("%VotePercent%",votePercent.ToString()).Replace("%Offender%", locatedPlayerName));
												if (!locatedPlayer.CheckPermission("cv.untouchable"))
												{
													locatedPlayer.Kill("callvote");
												}
											}
											else
											{
												Map.Broadcast(5, Translation.NoSuccessFullKill.Replace("%VotePercent%", votePercent.ToString()).Replace("%ThresholdNuke%", Plugin.Instance.Config.ThresholdKill.ToString()).Replace("%Offender%", locatedPlayerName));
											}
										}));

										break;
									}
									else
									{
										return Translation.PlayerNotFound.Replace("%Player%", args[1]);
									}
								}
							}
							else
							{
								if (!player.CheckPermission("cv.callvotekill")) {return Translation.NoPermissionToVote;}
								if(roundtimer < Config.MaxWaitKill) {return Translation.WaitToVote.Replace("%Timer%",$"{Config.MaxWaitKill-roundtimer}");}
								return Translation.VoteKillDisabled;
							}


						case "nuke":
							if (Plugin.Instance.Config.EnableNuke == true && roundtimer > Config.MaxWaitNuke && player.CheckPermission("cv.callvotenuke")|| player.CheckPermission("cv.bypass"))
							{
								Log.Debug("Vote called by " + playerNickname + " to " + args[0]);
								//return new string[] { "To be implemented." };

								options[1] = Translation.OptionYes;
								options[2] = Translation.OptionNo;

								voteCoroutine = Timing.RunCoroutine(StartVoteCoroutine(new Vote(Translation.AskedToNuke.Replace("%Player%",playerNickname), options), delegate (Vote vote)
								{
									int votePercent = (int)((float)vote.Counter[1] / (float)(Player.List.Count()) * 100f);
									if (votePercent >= Plugin.Instance.Config.ThresholdNuke)
									{
										Map.Broadcast(5, Translation.FoundationNuked.Replace("%VotePercent%", votePercent.ToString()));
										Exiled.API.Features.Warhead.Start();
									}
									else
									{
										Map.Broadcast(5, Translation.NoSuccessFullNuke.Replace("%VotePercent%", votePercent.ToString()).Replace("%ThresholdNuke%", Plugin.Instance.Config.ThresholdNuke.ToString()));
									}

                                }));
								break;
							}
							else
							{
								if (!player.CheckPermission("cv.callvotenuke")) {return Translation.NoPermissionToVote;}
								if(roundtimer < Config.MaxWaitNuke) {return Translation.WaitToVote.Replace("%Timer%",$"{Config.MaxWaitNuke-roundtimer}");}
								return Translation.VoteNukeDisabled;
							}

						case "respawnwave":
							if (Plugin.Instance.Config.EnableRespawnWave && roundtimer > Config.MaxWaitRespawnWave && player.CheckPermission("cv.callvoterespawnwave") || player.CheckPermission("cv.bypass"))
							{
								Log.Debug("Vote called by " + playerNickname + " to " + args[0]);
								//return new string[] { "To be implemented." };

								options[1] = Translation.OptionNo;
								options[2] = Translation.mtf;
								options[3] = Translation.ci;

								voteCoroutine = Timing.RunCoroutine(StartVoteCoroutine(new Vote(Translation.AskedToRespawn.Replace("%Player%", playerNickname) , options), delegate (Vote vote)
								{
									int votePercent = (int)((float)vote.Counter[1] / (float)(Player.List.Count()) * 100f);
									int mtfVotePercent = (int)((float)vote.Counter[2] / (float)(Player.List.Count()) * 100f);
									int ciVotePercent = (int)((float)vote.Counter[3] / (float)(Player.List.Count()) * 100f);
									if (mtfVotePercent >= Plugin.Instance.Config.ThresholdRespawnWave)
									{
										Map.Broadcast(5, Translation.mtfRespawn.Replace("%VotePercent%", mtfVotePercent.ToString() + "%"));
										Respawning.RespawnManager.Singleton.ForceSpawnTeam(Respawning.SpawnableTeamType.NineTailedFox);

									}
									else if (ciVotePercent >= Plugin.Instance.Config.ThresholdRespawnWave)
									{
										Map.Broadcast(5, Translation.ciRespawn.Replace("%VotePercent%", ciVotePercent.ToString()));
										Respawning.RespawnManager.Singleton.ForceSpawnTeam(Respawning.SpawnableTeamType.ChaosInsurgency);
									}
									else
									{
										Map.Broadcast(5, Translation.NoSuccessFullRespawn.Replace("%VotePercent%",votePercent.ToString()).Replace("%ThresholdRespawnWave%", Plugin.Instance.Config.ThresholdRespawnWave.ToString()));
									}
								}));
								break;
							}
							else
							{
								if (!player.CheckPermission("cv.callvoterespawnwave")) {return Translation.NoPermissionToVote;}
								if(roundtimer < Config.MaxWaitRespawnWave) {return Translation.WaitToVote.Replace("%Timer%",$"{Config.MaxWaitRespawnWave-roundtimer}");}
								return Translation.VoteRespawnWaveDisabled;
							}

						case "restartround":
							if (Plugin.Instance.Config.EnableRestartRound && roundtimer > Config.MaxWaitRestartRound && player.CheckPermission("cv.callvoterestartround") || player.CheckPermission("cv.bypass"))
							{
								Log.Debug("Vote called by " + playerNickname + " to " + args[0]);
								//return new string[] { "To be implemented." };

								options[1] = Translation.OptionYes;
								options[2] = Translation.OptionNo;
								
								

								voteCoroutine = Timing.RunCoroutine(StartVoteCoroutine(new Vote(Translation.AskedToRestart.Replace("%Player%",playerNickname), options), delegate (Vote vote)
								{
									int votePercent = (int)((float)vote.Counter[1] / (float)(Player.List.Count()) * 100f);
									if (votePercent >= Plugin.Instance.Config.ThresholdRestartRound)
									{
										Map.Broadcast(5, Translation.RoundRestarting.Replace("%VotePercent%", votePercent.ToString()));
										//this.Server.Round.RestartRound();
										//PlayerManager.localPlayer.GetComponent<PlayerStats>().Roundrestart();
										Round.Restart();

									}
									else
									{
										Map.Broadcast(5, Translation.NoSuccessFullRestart.Replace("%VotePercent%",votePercent.ToString()).Replace("%ThresholdRestartRound%", Plugin.Instance.Config.ThresholdRestartRound.ToString()));
									}
								}));
								break;
							}
							else
							{
								if (!player.CheckPermission("cv.callvoterestartround")) {return Translation.NoPermissionToVote;}
								if(roundtimer < Config.MaxWaitRestartRound) {return Translation.WaitToVote.Replace("%Timer%",$"{Config.MaxWaitRestartRound-roundtimer}");}
								return Translation.VoteRestartRoundDisabled;
							}

						default:
							//voteInProgress = true;
							if (player.CheckPermission("cv.callvotecustom") || player.CheckPermission("cv.bypass"))
							{
								if (args.Length == 1)
								{
									Log.Debug("Binary vote called by " + playerNickname + ": " + string.Join(" ", args));
									options[1] = Translation.OptionYes;
									options[2] = Translation.OptionNo;
								}
								else
								{
									Log.Debug("Multiple-choice vote called by " + playerNickname + ": " + string.Join(" ", args));
									for (int i = 1; i < args.Length; i++)
									{
										options[i] = args[i];
									}
								}
								voteCoroutine = Timing.RunCoroutine(StartVoteCoroutine(new Vote(playerNickname + " asks: " + args[0], options), null));
								break;
							}
							else {return Translation.NoPermissionToVote; }
					}
					timeOfLastVote = (int)(DateTime.Now - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
					return Translation.VoteStarted;
				}
			}
		}

		public bool Voting()
		{
			return CurrentVote != null;
		}

		public string Rigging(int argument)
		{
			string response = "vote not active";
			if (CurrentVote != null)
			{
				response = Translation.NoOptionAvailable;
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
			string firstBroadcast = Translation.AskedQuestion.Replace("%Question%", CurrentVote.Question);
			int counter = 0;
			foreach (KeyValuePair<int, string> kv in CurrentVote.Options)
			{
				if (counter == CurrentVote.Options.Count - 1)
				{
					firstBroadcast += $", {Translation.Options.Replace("%OptionKey%", kv.Key.ToString()).Replace("%Option%", kv.Value)}";
				}
				else
				{
					firstBroadcast += $" {Translation.Options.Replace("%OptionKey%", kv.Key.ToString()).Replace("%Option%", kv.Value)}";
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
						string timerBroadcast = Translation.Results;
						foreach (KeyValuePair<int, string> kv in CurrentVote.Options)
						{
							timerBroadcast += Translation.OptionAndCounter.Replace("%Option%", kv.Value).Replace("%OptionKey%", kv.Key.ToString()).Replace("%Counter%", CurrentVote.Counter[kv.Key].ToString());
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
						timerBroadcast += Translation.OptionAndCounter.Replace("%Option%", kv.Value).Replace("%OptionKey%", kv.Key.ToString()).Replace("%Counter%", CurrentVote.Counter[kv.Key].ToString());
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
				return Translation.NoPermissionToVote;
			}

			if (this.StopVote())
			{
				return Translation.CallVoteEnded;
			}
			else
			{
				return Translation.NoCallVoteInProgress;
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
						Log.Debug("Player " + playerNickname + " voted " + CurrentVote.Options[option] + " bringing the counter to " + CurrentVote.Counter[option]);
						//return new string[] { "Vote accepted!" };
						return Translation.VoteAccepted;
					}
					else
					{
						return Translation.NoOptionAvailable;
					}
				}
				else
				{
					return Translation.AlreadyVoted;
				}
			}
			else
			{
				return Translation.NoCallVoteInProgress;
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
