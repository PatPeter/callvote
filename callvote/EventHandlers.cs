using System.Collections.Generic;
using System.Text.RegularExpressions;
using MEC;
using System.Linq;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using static RoundSummary;

namespace callvote
{
    public class EventHandlers
	{
		public Plugin plugin;
		public EventHandlers(Plugin plugin) => this.plugin = plugin;

		[PluginEvent(PluginAPI.Enums.ServerEventType.WaitingForPlayers)]
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

		[PluginEvent(PluginAPI.Enums.ServerEventType.RoundEnd)]
		public void OnRoundEnded(LeadingTeam leadingTeam)
		{
			plugin.StopVote();
		}
	}
}
