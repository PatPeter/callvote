using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace callvote.Configs
{
	public class Thresholds
	{
		/// <inheritdoc/>
		[Description("Minimum percentage required in order for a Kick vote to succeed.")]
		public int Kick { get; set; } = 30;

		/// <inheritdoc/>
		[Description("Minimum percentage required in order for a Kill vote to succeed.")]
		public int Kill { get; set; } = 30;

		/// <inheritdoc/>
		[Description("Minimum percentage required in order for a Nuke vote to succeed.")]
		public int Nuke { get; set; } = 30;

		/// <inheritdoc/>
		[Description("Minimum percentage required in order for a RespawnWave vote to succeed.")]
		public int RespawnWave { get; set; } = 30;

		/// <inheritdoc/>
		[Description("Minimum percentage required in order for a RestartRound vote to succeed.")]
		public int RestartRound { get; set; } = 30;
	}
}
