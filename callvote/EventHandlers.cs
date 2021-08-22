using System.Collections.Generic;
using System.Text.RegularExpressions;
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
