# Description
This is a plugin for [Smod2](https://github.com/Grover-c13/Smod2) that allows calling Kick, RestartRound, or custom votes using console commands using the same format as the Source Engine (Left 4 Dead 2/Counter-Strike: Global Offensive).

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
yes | .yes | Alias for 1
no | .no | Alias for 2
1 | .1 | Vote for the 1st option
2 | .2 | Vote for the 2nd option
3 | .3 | Vote for the 3rd option
4 | .4 | Vote for the 4th option
5 | .5 | Vote for the 5th option
6 | .6 | Vote for the 6th option
7 | .7 | Vote for the 7th option
8 | .8 | Vote for the 8th option
9 | .9 | Vote for the 9th option
0 | .0 | Vote for the 10th option
