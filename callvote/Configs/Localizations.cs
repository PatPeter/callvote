using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace callvote.Configs
{
	public class Localizations
	{
		/**
		 * Localization strings used in broadcast output
		 */
		public string Prompt { get; set; } = "{0} asks: ";
		public string Tutorial { get; set; } = "Press ~ and type";
		public string Or { get; set; } = "or";
		public string For { get; set; } = "for";
		public string FinalResults { get; set; } = "Final results:";
		public string Yes { get; set; } = "Yes";
		public string No { get; set; } = "No";
		public string MTF { get; set; } = "MTF";
		public string CI { get; set; } = "CI";

		/**
		 * Localization strings used in command usage/return strings
		 */
		public string SubcommandKick { get; set; } = "Kick";
		public string SubcommandKill { get; set; } = "Kill";
		public string SubcommandNuke { get; set; } = "Nuke";
		public string SubcommandRespawnWave { get; set; } = "RespawnWave";
		public string SubcommandRestartRound { get; set; } = "RestartRound";
		public string SubcommandIsNotEnabled { get; set; } = "callvote {0} is not enabled.";
		public string NoPermission { get; set; } = "You do not have permission to run this command.";
		public string MaximumVotesReached { get; set; } = "The maximum amount of votes per round has been reached.";
		public string NoVoteInProgress { get; set; } = "No vote is in progress.";
		public string VoteInProgress { get; set; } = "A vote is currently in progress or the vote is on cooldown.";
		public string VoteStarted { get; set; } = "Vote has been started!";
		public string DescriptionCallvote { get; set; } = "callvote Kick/Kill/Nuke/RespawnWave/RestartRound/<custom> <player>/[options]";
		public string DescriptionStopvote { get; set; } = "Stops a vote that is currently in process.";
		public string DescriptionNumber1 { get; set; } = "Command to vote 1 to a multiple choice question.";
		public string DescriptionNumber2 { get; set; } = "Command to vote 2 to a multiple choice question.";
		public string DescriptionNumber3 { get; set; } = "Command to vote 3 to a multiple choice question.";
		public string DescriptionNumber4 { get; set; } = "Command to vote 4 to a multiple choice question.";
		public string DescriptionNumber5 { get; set; } = "Command to vote 5 to a multiple choice question.";
		public string DescriptionNumber6 { get; set; } = "Command to vote 6 to a multiple choice question.";
		public string DescriptionNumber7 { get; set; } = "Command to vote 7 to a multiple choice question.";
		public string DescriptionNumber8 { get; set; } = "Command to vote 8 to a multiple choice question.";
		public string DescriptionNumber9 { get; set; } = "Command to vote 9 to a multiple choice question.";
		public string DescriptionNumber10 { get; set; } = "Command to vote 10 to a multiple choice question.";
	}
}
