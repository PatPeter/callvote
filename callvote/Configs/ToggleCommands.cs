using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace callvote.Configs
{
	public class ToggleCommands
	{
		/// <inheritdoc/>
		[Description("Enable .callvote 'Custom Vote'")]
		public bool Custom { get; set; } = true;

		/// <inheritdoc/>
		[Description("Enable .callvote Kick <Player>")]
		public bool Kick { get; set; } = false;

		/// <inheritdoc/>
		[Description("Enable .callvote Kill <Player>")]
		public bool Kill { get; set; } = false;

		/// <inheritdoc/>
		[Description("Enable .callvote Nuke")]
		public bool Nuke { get; set; } = false;

		/// <inheritdoc/>
		[Description("Enable .callvote RespawnWave")]
		public bool RespawnWave { get; set; } = false;

		/// <inheritdoc/>
		[Description("Enable .callvote RestartRound")]
		public bool RestartRound { get; set; } = false;
	}
}
