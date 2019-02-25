using System;
using System.Text.RegularExpressions;
using Smod2;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;

namespace Callvote
{
	class CallvoteEvents : IEventHandlerCallCommand, IEventHandlerWaitingForPlayers
	{
		private readonly CallvotePlugin plugin;

		public CallvoteEvents(CallvotePlugin plugin)
		{
			this.plugin = plugin;
		}

		public void OnCallCommand(PlayerCallCommandEvent ev)
		{
			switch (ev.Command)
			{
				case "callvote":
					if (this.plugin.currentVote != null)
					{
						ev.ReturnMessage = "There is currently a vote in progress.";
					}
					else
					{
						MatchCollection quoteDelimitedArguments = new Regex("[^\\s\"\']+|\"([^\"]*)\"|\'([^\']*)\'").Matches(ev.Command);
						string[] args = new string[quoteDelimitedArguments.Count - 1];
						this.plugin.startVote(ev.Player, args);
						ev.ReturnMessage = "Vote started!";
					}
					break;

				case "stopvote":
					if (this.plugin.currentVote != null)
					{
						if (this.plugin.currentVote.timer != null)
						{
							this.plugin.currentVote.timer.Stop();
							this.plugin.currentVote = null;
							ev.ReturnMessage = "Vote stopped.";
						}
						else
						{
							this.plugin.currentVote = null;
							ev.ReturnMessage = "Vote stopped.";
						}
					}
					else
					{
						ev.ReturnMessage = "There is not a vote in progress.";
					}
					break;

				case "1":
				case "2":
				case "3":
				case "4":
				case "5":
				case "6":
				case "7":
				case "8":
				case "9":
				case "0":
					if (this.plugin.currentVote != null)
					{
						switch (ev.Command)
						{
							case "1":
								this.plugin.handleVote(ev.Player, 1);
								break;

							case "2":
								this.plugin.handleVote(ev.Player, 2);
								break;

							case "3":
								this.plugin.handleVote(ev.Player, 3);
								break;

							case "4":
								this.plugin.handleVote(ev.Player, 4);
								break;

							case "5":
								this.plugin.handleVote(ev.Player, 5);
								break;

							case "6":
								this.plugin.handleVote(ev.Player, 6);
								break;

							case "7":
								this.plugin.handleVote(ev.Player, 7);
								break;

							case "8":
								this.plugin.handleVote(ev.Player, 8);
								break;

							case "9":
								this.plugin.handleVote(ev.Player, 9);
								break;

							case "0":
								this.plugin.handleVote(ev.Player, 10);
								break;
						}
						ev.ReturnMessage = "Vote submitted!";
					}
					else
					{
						ev.ReturnMessage = "No vote is in progress.";
					}
					break;
			}
		}

		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			this.plugin.currentVote = null;
			//plugin.RefreshConfig();
		}
	}
}
