using System.Collections.Generic;
using System.Text.RegularExpressions;
using Grenades;
using MEC;
using System.Linq;

namespace callvote
{
	using Exiled.Events.EventArgs;
    using Exiled.Permissions.Extensions;

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
					ev.IsAllowed = false;
				}
				else
				{
					ev.ReturnMessage = "No vote is in progress.";
					ev.IsAllowed = false;
				}
			}
			else
			{

				switch (command)
				{

					case "callvote":
						if (ev.Player.CheckPermission("cv.callvote"))
						{
							string[] quotedArgs = Regex.Matches(string.Join(" ", ev.Arguments), "[^\\s\"\']+|\"([^\"]*)\"|\'([^\']*)\'")
								.Cast<Match>()
								.Select(m => m.Value)
								.ToArray()
								//.Skip(1)
								.ToArray();
							ev.ReturnMessage = this.plugin.CallvoteHandler(ev.Player, quotedArgs);
							ev.IsAllowed = false;
						}
						else
						{
							ev.ReturnMessage = "You do not have permission to run this command";
							ev.IsAllowed = false;
						}
						break;

					case "stopvote":

						ev.ReturnMessage = this.plugin.StopvoteHandler(ev.Player);
						ev.IsAllowed = false;
						break;

					case "yes":
						ev.ReturnMessage = this.plugin.VoteHandler(ev.Player, 1);
						ev.IsAllowed = false;
						break;

					case "no":
						ev.ReturnMessage = this.plugin.VoteHandler(ev.Player, 2);
						ev.IsAllowed = false;
						break;
					case "forceresult":
						if (ev.Player.CheckPermission("cv.superadmin+"))
						{ 
							if (ev.Arguments.Count > 0)
							{
								if (int.TryParse(ev.Arguments[0], out option))
								{
									ev.ReturnMessage = this.plugin.Rigging(option);
								}
							}
						}
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
			Plugin.Instance.DictionaryOfVotes.Clear();
			this.plugin.CurrentVote = null;
		}

		public void OnRoundEnded(RoundEndedEventArgs ev)
		{
			plugin.StopVote();
		}
	}
}
