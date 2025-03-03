using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using UnityEngine;
using callvote.Configs;

namespace callvote
{
	public sealed class Config
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

		/// <inheritdoc/>
		[Description("Number of seconds for a vote to last for.")]
		public int VoteDuration { get; set; } = 30;

		/// <inheritdoc/>
		[Description("Number of seconds between votes to prevent vote spam.")]
		public int VoteCooldown { get; set; } = 60;

		/// <inheritdoc/>
		[Description("")]
		public int MaxAmountOfVotesPerRound { get; set; } = 10;

		public ToggleCommands ToggleCommands = new ToggleCommands();

		public Thresholds Thresholds = new Thresholds();

		public Localizations Localizations = new Localizations();
	}
}
