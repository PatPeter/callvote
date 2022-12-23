using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using UnityEngine;

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
		[Description("")]
		public int VoteDuration { get; set; } = 30;

		/// <inheritdoc/>
		[Description("")]
		public int VoteCooldown { get; set; } = 60;

		/// <inheritdoc/>
		[Description("")]
		public int MaxAmountOfVotesPerRound { get; set; } = 10;

		/// <inheritdoc/>
		[Description("")]
		public bool EnableKick { get; set; } = false;

		/// <inheritdoc/>
		[Description("")]
		public bool EnableKill { get; set; } = false;

		/// <inheritdoc/>
		[Description("")]
		public bool EnableNuke { get; set; } = false;

		/// <inheritdoc/>
		[Description("")]
		public bool EnableRespawnWave { get; set; } = false;

		/// <inheritdoc/>
		[Description("")]
		public bool EnableRestartRound { get; set; } = false;


		/// <inheritdoc/>
		[Description("")]
		public int ThresholdKick { get; set; } = 30;

		/// <inheritdoc/>
		[Description("")]
		public int ThresholdKill { get; set; } = 30;

		/// <inheritdoc/>
		[Description("")]
		public int ThresholdNuke { get; set; } = 30;

		/// <inheritdoc/>
		[Description("")]
		public int ThresholdRespawnWave { get; set; } = 30;

		/// <inheritdoc/>
		[Description("")]
		public int ThresholdRestartRound { get; set; } = 30;
	}
}
