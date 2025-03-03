# callvote End-Of-Life Announcement
**callvote** now has a new home at [callvote](https://github.com/Unbistrackted/callvote) where its main contributor will be [Unbistrackted](https://github.com/Unbistrackted)! The original implementation of callvote was in ServerMod2, then Exiled, and finally NWAPI (so that we would not have to wait for EXILED updates and it would work immediately after each update). I shut down the Unigamia.com servers in September 2023 and therefore have no server to test new plugin versions on in order to maintain this repository. With the announcement that NWAPI is going to be deprecated in favor of LabAPI, I have decided that I will never code plugins for SCP:SL ever again.

# Description
This is a plugin for [EXILED](https://github.com/Exiled-Team/EXILED) that allows calling Kick, RestartRound, or custom votes using console commands using the same format as the Source Engine. For instance, in [Counter-Strike: Global Offensive](https://www.reddit.com/r/GlobalOffensive/comments/2zote3/callvote_command_syntax/) and [Left 4 Dead 2](https://www.reddit.com/r/l4d2/comments/1ny3gi/question_about_vote_console_commands/) the command syntax is the same. For anyone who wants to complain about the command syntax, bring it up with Valve.

## Configuration Settings
Setting Key | Value Type | Default Value | Description
--- | --- | --- | ---
callvote_allowed_roles | boolean | true | Roles allowed to call custom votes. If Kick or RestartRound are enabled, all players are allowed to call them.
callvote_vote_duration | integer | 30 | Number of seconds for a vote to last for.
callvote_enable_kick | boolean | false | Can all players use callvote Kick?
callvote_enable_restartround | boolean | false | Can all players use callvote RestartRound?
callvote_threshold_kick | integer | 80 | Percentage threshold required for a player to be kicked.
callvote_threshold_restartround | integer | 80 | Percentage threshold required for a round to be restarted.

## Commands
Server Command | Client Command | Parameters | Description
--- | --- | --- | ---
callvote | .callvote | "Custom Question" | Vote on a custom yes/no question.
callvote | .callvote | "Custom Question" "First Option" "Second Option" ... | Vote on a question with multiple options
callvote Kick | .callvote Kick | [player] | Vote to kick a player.
callvote RestartRound | .callvote RestartRound | [none] | Vote to restart a round.
stopvote | .stopvote | [none] | Stops a vote currently in progress
yes | .yes | [none] | Alias for 1
no | .no | [none] | Alias for 2
1 | .1 | [none] | Vote for the 1st option
2 | .2 | [none] | Vote for the 2nd option
3 | .3 | [none] | Vote for the 3rd option
4 | .4 | [none] | Vote for the 4th option
5 | .5 | [none] | Vote for the 5th option
6 | .6 | [none] | Vote for the 6th option
7 | .7 | [none] | Vote for the 7th option
8 | .8 | [none] | Vote for the 8th option
9 | .9 | [none] | Vote for the 9th option
0 | .0 | [none] | Vote for the 10th option
