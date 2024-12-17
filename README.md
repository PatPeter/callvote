# Description
This is a plugin for [EXILED](https://github.com/Exiled-Team/EXILED) that allows calling Kick, RestartRound, Kill, RespawnWave or custom votes using console commands using the same format as the Source Engine (Left 4 Dead 2/Counter-Strike: Global Offensive).

## Configuration Settings
Setting Key | Value Type | Default Value | Description
--- | --- | --- | ---
vote_duration | integer | 30 | Number of seconds for a vote to last for.
vote_cooldown | integer | 5 | Cooldown (in seconds) between each callvote.
max_amount_of_votes_per_round | float | 10 | Maximum amount of **callvotes** in a round
max_wait_kill | float | 0 | Time (in seconds) after the round starts to the command **callvote kill** be available
max_wait_kick | float | 0 | Time (in seconds) after the round starts to the command **callvote kick** be available
max_wait_nuke | float | 0 | Time (in seconds) after the round starts to the command **callvote nuke** be available
max_wait_respawn_wave | float | 0 | Time (in seconds) after the round starts to the command **callvote respawnwave** be available
max_wait_restart_round | float | 0 | Time (in seconds) after the round starts to the command **callvote restartround** be available
enable_kick | boolean | true | Can players use **callvote kick**?
enable_kill | boolean | true | Can players use **callvote kill**?
enable_nuke | boolean | true | Can players use **callvote nuke**?
enable_respawn_wave | boolean | true | Can players use **callvote respawnwave**?
enable_restart_round | boolean | true | Can players use **callvote restartround**?
threshold_kick | integer | 30 | Percentage threshold required for a player to be kicked.
threshold_kill | integer | 30 | Percentage threshold required for a player to be killed.
threshold_nuke | integer | 30 | Percentage threshold required to explode ALPHA WARHEAD.
threshold_respawn_wave | integer | 30 | Percentage threshold required to respawn a MTF or CI wave.
threshold_restart_round | integer | 30 | Percentage threshold required to restart the round.


## Permissions
Permission | Command | Description
--- | --- | ---
cv.callvote | .callvote <parameter> | Allows players to use **.callvote**
cv.bypass | .callvote <parameter> | Bypasses permissions requeriments and time
cv.unlimitedvotes | .callvote <parameter> | Bypasses max_amount_of_votes_per_round
cv.callvotekick | .callvote kick | Gives permission to use **.callvote kick**
cv.callvotekill | .callvote kill | Gives permission to use **.callvote kill**
cv.callvotenuke | .callvote nuke | Gives permission to use **.callvote nuke**
cv.callvoterespawnwave | .callvote respawnwave | Gives permission to use **.callvote respawnwave**
cv.callvoterestartround | .callvote restartround | Gives permission to use **.callvote restartround**
cv.callvotecustom | .callvote "Custom Question" ... | Gives permission to use **.callvote "Custom Question" ...**
cv.stopvote | .stopvote | Stops current vote
cv.untouchable | .callvote kick/kill | Player cannot be kicked or killed
cv.superadmin+ | .callvote kick/kill | Allows player to rig the system
cv.bypass | .callvote <parameter> | Bypasses permissions requeriments and time


## Commands
Server Command | Client Command | Parameters | Description
--- | --- | --- | ---
callvote | .callvote | "Custom Question" | Vote on a custom yes/no question.
callvote | .callvote | "Custom Question" "First Option" "Second Option" ... | Vote on a question with multiple options
callvote kick | .callvote kick | [player] | Vote to kick a player.
callvote kill | .callvote kill | [player] | Vote to kill a player.
callvote nuke | .callvote nuke | [none] | Vote to nuke the facility.
callvote respawnwave | .callvote respawnwave | [none] | Vote to restart a round.
callvote restartround | .callvote restartround | [none] | Vote to restart a round.
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


Special thanks to: https://github.com/Playeroth
