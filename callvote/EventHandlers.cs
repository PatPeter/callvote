using System.Collections.Generic;
using System.Text.RegularExpressions;
using Grenades;
using MEC;
using System.Linq;

namespace callvote
{
	using Exiled.Events.EventArgs;

	public class EventHandlers
	{
		public Plugin plugin;
		public EventHandlers(Plugin plugin) => this.plugin = plugin;

		public void OnConsoleCommand(SendingConsoleCommandEventArgs ev)
		{
			string command = ev.Name; // ev.Command.Split(' ')[0];

			int option;
			if (int.TryParse(command, out option))
			{
				if (this.plugin.Voting())
				{

					ev.ReturnMessage = this.plugin.VoteHandler(ev.Player, option);
				}
				else
				{
					ev.ReturnMessage = "No vote is in progress.";
				}
			}
			else
			{

				switch (command)
				{
					case "callvote":
						/*string[] quotedArgs = Regex.Matches(string.Join(" ", ev.Command), "[^\\s\"\']+|\"([^\"]*)\"|\'([^\']*)\'")
							.Cast<Match>()
							.Select(m => m.Value)
							.ToArray()
							.Skip(1)
							.ToArray();*/
						ev.ReturnMessage = this.plugin.CallvoteHandler(ev.Player, ev.Arguments.ToArray());
						break;

					case "stopvote":
						ev.ReturnMessage = this.plugin.StopvoteHandler(ev.Player);
						break;

					case "yes":
						ev.ReturnMessage = this.plugin.VoteHandler(ev.Player, 1);
						break;

					case "no":
						ev.ReturnMessage = this.plugin.VoteHandler(ev.Player, 2);
						break;
				}
			}
		}

		public void OnWaitingForPlayers()
		{
			//this.plugin.ReloadConfig();
			if (this.plugin.CurrentVote != null && this.plugin.CurrentVote.Timer != null)
			{
				this.plugin.CurrentVote.Timer.Stop();
				this.plugin.CurrentVote.Timer.Dispose();
			}
			this.plugin.CurrentVote = null;
		}
	}
}
