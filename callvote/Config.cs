using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Exiled.API.Interfaces;
using System.Threading.Tasks;

namespace callvote
{

    public class Config : IConfig
    {
        /// <inheritdoc/>
        [Description("Enable or disable the plugin. Defaults to true.")]
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        [Description("Modules:")]
        public bool EnableKick { get; set; } = true;
        public bool EnableKill { get; set; } = true;
        public bool EnableNuke { get; set; } = true;
        public bool EnableRespawnWave { get; set; } = true;
        public bool EnableRestartRound { get; set; } = true;
        [Description("Durations:")]
        public int VoteDuration { get; set; } = 30;
        public int VoteCooldown { get; set; } = 5;
        public int MaxAmountOfVotesPerRound { get; set; } = 10;
        public float MaxWaitKill { get; set; } = 0;
        public float MaxWaitKick { get; set; } = 0;
        public float MaxWaitNuke { get; set; } = 0;
        public float MaxWaitRespawnWave { get; set; } = 0;
        public float MaxWaitRestartRound { get; set; } = 0;
        [Description("Thresholds:")]
        public int ThresholdKick { get; set; } = 30;
        public int ThresholdKill { get; set; } = 30;
        public int ThresholdNuke { get; set; } = 30;
        public int ThresholdRespawnWave { get; set; } = 30;
        public int ThresholdRestartRound { get; set; } = 30;
    }
}
