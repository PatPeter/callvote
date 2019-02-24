using Smod2;
using Smod2.API;
using Smod2.Attributes;
using Smod2.EventHandlers;
using Smod2.Events;
using System.Collections.Generic;
using System.Timers;

namespace Callvote
{
	[PluginDetails(
		author = "PatPeter",
		name = "callvote",
		description = "callvote command like in the Source engine. Vote to kick users, restart round, or make your own custom votes.",
		id = "patpeter.callvote",
		version = "1.0.0.0",
		SmodMajor = 3,
		SmodMinor = 1,
		SmodRevision = 20
		)]
	class CallvotePlugin : Plugin
	{
		bool voteInProgress = false;
		Vote currentVote = null;

		public override void OnDisable()
		{
			this.Info(this.Details.name + " was disabled ):");
		}

		public override void OnEnable()
		{
			this.Info(this.Details.name + " has loaded :)");
			this.Info("Config value: " + this.GetConfigString("myConfigKey"));
		}

		public override void Register()
		{
			// Register multiple events
			//this.AddEventHandlers(new RoundEventHandler(this));
			//Register multiple events with Low Priority
			//this.AddEventHandlers(new MultipleEventsExample(this), Priority.Low);
			// Register single event with priority (need to specify the handler type)
			//this.AddEventHandler(typeof(IEventHandlerPlayerPickupItem), new LottoItemHandler(this), Priority.High);
			// Register Command(s)
			this.AddCommand("callvote", new CallvoteCommand(this));
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
			this.AddConfig(new Smod2.Config.ConfigSetting("callvote_enable_kick", false, Smod2.Config.SettingType.BOOL, true, ""));
			this.AddConfig(new Smod2.Config.ConfigSetting("callvote_enable_restartround", false, Smod2.Config.SettingType.BOOL, true, ""));
			this.AddConfig(new Smod2.Config.ConfigSetting("callvote_threshold_kick", 80, Smod2.Config.SettingType.NUMERIC, true, ""));
			this.AddConfig(new Smod2.Config.ConfigSetting("callvote_threshold_restartround", 80, Smod2.Config.SettingType.NUMERIC, true, ""));
		}

		public string[] startVote(Player player, string[] args)
		{
			if (args.Length == 0)
			{
				return new string[] { "callvote RestartRound", "callvote Kick <player>", "callvote <custom> [options]" };
			}
			else
			{
				if (voteInProgress)
				{
					return new string[] { "A vote is currently in progress." };
				}
				else
				{
					switch (args[0])
					{
						case "RestartRound":
							this.Info("Vote called by " + player.Name + " to " + args[1]);
							return new string[] { "To be implemented." };

						case "Kick":
							if (args.Length == 1)
							{
								return new string[] { "callvote Kick <player>" };
							}
							else
							{
								this.Info("Vote called by " + player.Name + " to " + args[1] + " player " + args[2]);
								return new string[] { "To be implemented." };
							}

						default:
							this.Info("Vote called by " + player.Name + ": " + string.Join(" ", args));
							voteInProgress = true;

							Dictionary<int, string> options = new Dictionary<int, string>();
							if (args.Length == 1)
							{
								options[1] = "yes";
								options[2] = "no";
							}
							else
							{
								this.Info("Vote called by " + player.Name + ": " + string.Join(" ", args));
								for (int i = 2; i < args.Length; i++)
								{
									options[i - 1] = args[i];
								}
							}

							{
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
							}

							int timerCounter = 0;
							currentVote = new Vote(args[1], options);
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
								else if (timerCounter >= 20)
								{
									currentVote.timer.Enabled = false;
								}
								else
								{
									string timerBroadcast = "";
									foreach (KeyValuePair<int, int> kv in currentVote.counter)
									{
										timerBroadcast += currentVote.options[kv.Key] + " (" + kv.Value + ") ";
									}
									this.Server.Map.Broadcast(1, timerBroadcast, false);
								}
							};
							return new string[] { "Vote has been started!" };
					}
				}
			}
		}

		public string[] handleVote(Player player, int option)
		{
			if (voteInProgress && currentVote != null)
			{
				if (!currentVote.votes.Contains(player.SteamId))
				{
					currentVote.counter[option]++;
					currentVote.votes.Add(player.SteamId);
					this.Info("Player " + player.Name + " voted " + currentVote.options[option] + " bringing the counter to " + currentVote.counter[option]);
					return new string[] { "Vote accepted!" };
				}
				else
				{
					return new string[] { "You've already voted." };
				}
			}
			else
			{
				return new string[] { "There is no vote in progress." };
			}
		}
	}

	/*enum OptionEnum {
		ONE = 1,
		TWO = 2,
		THREE = 3,
		FOUR = 4,
		FIVE = 5,
		SIX = 6,
		SEVEN = 7,
		EIGHT = 8,
		NINE = 9,
		TEN = 0
	}*/

	class Vote
	{
		public string question;
		public Dictionary<int, string> options = new Dictionary<int, string>();
		public HashSet<string> votes = new HashSet<string>();
		public Dictionary<int, int> counter = new Dictionary<int, int>();
		public Timer timer;
		//public Option[] options;

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

	/*class Option
	{
		string text;
		OptionEnum ordinal;

		public Option(string text, OptionEnum ordinal)
		{
			this.text = text;
			this.ordinal = ordinal;
		}
	}*/
}
