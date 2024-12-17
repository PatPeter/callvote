# Description
This is a plugin for [EXILED](https://github.com/Exiled-Team/EXILED) that allows calling Kick, RestartRound, or custom votes using console commands using the same format as the Source Engine (Left 4 Dead 2/Counter-Strike: Global Offensive).

## Configuration Settings
Setting Key | Value Type | Default Value | Description
--- | --- | --- | ---
callvote_allowed_roles | boolean | true | Roles allowed to call custom votes. If Kick or RestartRound are enabled, all players are allowed to call them.
callvote_vote_duration | integer | 30 | Number of seconds for a vote to last for.
callvote_enable_kick | boolean | false | Can all players use callvote Kick?
callvote_enable_restartround | boolean | false | Can all players use callvote RestartRound?
callvote_threshold_kick | integer | 80 | Percentage threshold required for a player to be kicked.
callvote_threshold_restartround | integer | 80 | Percentage threshold required for a round to be restarted.
vote_duration | integer | 30
vote_cooldown | integer | 10
max_amount_of_votes_per_round | float | 10
max_wait_general | float | 0
max_wait_kill | float | 12
max_wait_kick | float | 20
max_wait_nuke | float | 30
max_wait_respawn_wave | float | 40
max_wait_restart_round | float | 50
enable_kick true
enable_kill true
enable_nuke true
enable_respawn_wave true
enable_restart_round true
threshold_kick 30
threshold_kill 30
threshold_nuke 30
threshold_respawn_wave 30
hreshold_restart_round 30


## Permissions

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
