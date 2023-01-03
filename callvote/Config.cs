	using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace callvote
{
	using System.Collections.Generic;
	using System.ComponentModel;

	using Exiled.API.Interfaces;

	public sealed class Config : IConfig
	{
		//internal List<string> _AllowedRoles = new List<string>() { "owner", "admin", "moderator" };
		/*internal int _VoteDuration = 30;
		internal int _VoteCooldown = 30;

		internal bool _EnableKick = false;
		internal bool _EnableKill = false;
		internal bool _EnableNuke = false;
		internal bool _EnableRespawnWave = false;
		internal bool _EnableRestartRound = false;

		internal int _ThresholdKick = 80;
		internal int _ThresholdKill = 80;
		internal int _ThresholdNuke = 80;
		internal int _ThresholdRespawnWave = 80;
		internal int _ThresholdRestartRound = 80;*/

		/// <inheritdoc/>
		[Description("Enable or disable the plugin. Defaults to true.")]
		public bool IsEnabled { get; set; } = true;
		public bool Debug { get; set; } = false;
		public int VoteDuration { get; set; } = 30;
		public int VoteCooldown { get; set; } = 60;
		public int MaxAmountOfVotesPerRound { get; set; } = 10;
		public bool EnableKick { get; set; } = false;
		public bool EnableKill { get; set; } = false;
		public bool EnableNuke { get; set; } = false;
		public bool EnableRespawnWave { get; set; } = false;
		public bool EnableRestartRound { get; set; } = false;

		public int ThresholdKick { get; set; } = 30;
		public int ThresholdKill { get; set; } = 30;
		public int ThresholdNuke { get; set; } = 30;
		public int ThresholdRespawnWave { get; set; } = 30;
		public int ThresholdRestartRound { get; set; } = 30;
	}
}
