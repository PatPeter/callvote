using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API;
using Exiled.API.Interfaces;
using System.ComponentModel;

namespace callvote
{
    public class Translation : ITranslation
    {
        [Description("%player%, %VotePercent%, %Offender%, %ThresholdKick%, %ThresholdRespawnWave%, %ThresholdNuke%, %ThresholdKill%, %ThresholdRestartRound%, %OptionKey%, %Option%, %Counter%")]
        //%player%, %VotePercent%, %offender%, %ThresholdKick%
        public string MaxVote { get; private set; } = "Max amounts of votes done this round";
        public string InProgressVote { get; private set; } = "A vote is currently in progress.";
        public string PlayersWithSameName { get; private set; } = "Multiple players have a name or partial name of %Player%. Please use a different search string.";
        public string OptionYes { get; private set; } = "<color=green>YES</color>";
        public string OptionNo { get; private set; } = "<color=red>NO</color>";
        public string PlayerGettingKicked { get; private set; } = "%Player% asks: Kick %Offender% ?";
        public string AskedToKill { get; private set; } = "%Player% Asks: Kill %Offender% ";
        public string Untouchable { get; private set; } = "%VotePercent%% Voted to kill you.";
        public string NotSuccessFullKick { get; private set; } = "%VotePercent%% voted yes. %ThresholdKick%% was required to kill. %Offender%.";
        public string PlayerNotFound { get; private set; } = "Did not find any players with the name or partial name of %Player%";
        public string NoOptionAvailable { get; private set; } = "Vote does not have that option.";
        public string AlreadyVoted { get; private set; } = "You've already voted.";
        public string VoteAccepted { get; private set; } = "Vote accepted!";
        public string NoPermissionToVote { get; private set; } = "You do not have permission to run this command!";
        public string CallVoteEnded { get; private set; } = "Vote stopped.";
        public string Results { get; private set; } = "Final results:\n";
        public string OptionAndCounter { get; private set; } = " %Option% (%Counter%) ";
        public string Options { get; private set; } = ".%OptionKey% for %Option% ";
        public string AskedQuestion { get; private set; } = "%Question% \n Press ~ and type \n";
        public string mtf { get; private set; } = "<color=blue>MTF</color>";
        public string ci { get; private set; } = "<color=green>CI</color>";
        public string ciRespawn { get; private set; } = "%VotePercent%% voted <color=green>YES</color>. Forcing the reappering of CI..";
        public string mtfRespawn { get; private set; } = "%VotePercent%% voted <color=green>YES</color>. Forcing the reappering of MTF..";
        public string NoSuccessFullRespawn { get; private set; } = "%VotePercent%% voted no. %ThresholdRespawnWave%% was required to respawn the next wave.";
        public string AskedToRespawn { get; private set; } = "%Player% asks: Respawn the next wave?";
        public string AskedToNuke { get; private set; } = "%Player% asks: NUKE THE FACILITY?!??";
        public string FoundationNuked { get; private set; } = "%VotePercent%% voted yes. Nuking the facility...";
        public string NoSuccessFullNuke { get; private set; } = "Only %VotePercent%% voted yes. %ThresholdNuke%% was required to nuke the facility.";
        public string NoSuccessFullKill { get; private set; } = "Only %VotePercent%% voted yes. + %ThresholdKill%% was required to kill locatedPlayerName";
        public string PlayerKilled { get; private set; } = "%VotePercent%% voted yes. Killing player %Offender%";
        public string VoteRespawnWaveDisabled { get; private set; } = "callvote RespawnWave is disabled.";
        public string VoteKickDisabled { get; private set; } = "callvote kick is disabled.";
        public string VoteKillDisabled { get; private set; } = "callvote kill is disabled.";
        public string VoteNukeDisabled { get; private set; } = "callvote nuke is disabled.";
        public string VoteRestartRoundDisabled { get; private set; } = "callvote restartround is disabled.";
        public string AskedToKick { get; private set; } = "%Player% Asks: Kick %Offender%? ";
        public string AskedToRestart { get; private set; } = "%Player% asks: Restart the round?";
        public string RoundRestarting { get; private set; } = "%VotePercent% voted yes. Restarting the round...";
        public string NoSuccessFullRestart { get; private set; } = "Only %VotePercent%% voted yes. %ThresholdRestartRound%% was required to restart the round.";
        public string VoteStarted { get; private set; } = "Vote has been started!";
        public string NoCallVoteInProgress { get; private set; } = "There is no vote in progress.";
        public string WaitToVote { get; private set; } = "You should wait %Timer%s before using this command.";

    }
}
