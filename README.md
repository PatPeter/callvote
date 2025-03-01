# Description
This is a plugin for [EXILED](https://github.com/ExMod-Team/EXILED) that allows calling Kick, RestartRound, Kill, RespawnWave, or custom votes using console commands in the same format as the Source Engine (Left 4 Dead 2/Counter-Strike: Global Offensive).

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
cv.callvote | .callvote (Parameter) | Allows players to use **.callvote**
cv.bypass | .callvote (Parameter) | Bypasses permissions requeriments and time
cv.unlimitedvotes | .callvote (Parameter) | Bypasses max_amount_of_votes_per_round
cv.callvotekick | .callvote kick | Gives permission to use **.callvote kick**
cv.callvotekill | .callvote kill | Gives permission to use **.callvote kill**
cv.callvotenuke | .callvote nuke | Gives permission to use **.callvote nuke**
cv.callvoterespawnwave | .callvote respawnwave | Gives permission to use **.callvote respawnwave**
cv.callvoterestartround | .callvote restartround | Gives permission to use **.callvote restartround**
cv.callvotecustom | .callvote "Custom Question" ... | Gives permission to use **.callvote "Custom Question" ...**
cv.stopvote | .stopvote | Stops current vote
cv.untouchable | .callvote kick/kill | Player cannot be kicked or killed
cv.superadmin+ | .callvote (Parameter) | Allows player to rig the system :trollface:


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


## Config File
```
callvote:
# Enable or disable the plugin. Defaults to true.
  is_enabled: true
  debug: false
  # Modules:
  enable_kick: true
  enable_kill: true
  enable_nuke: true
  enable_respawn_wave: true
  enable_restart_round: true
  # Durations:
  vote_duration: 30
  vote_cooldown: 5
  max_amount_of_votes_per_round: 10
  max_wait_kill: 0
  max_wait_kick: 0
  max_wait_nuke: 0
  max_wait_respawn_wave: 0
  max_wait_restart_round: 0
  # Thresholds:
  threshold_kick: 30
  threshold_kill: 30
  threshold_nuke: 30
  threshold_respawn_wave: 30
  threshold_restart_round: 30
```



## Translation File
```
callvote:
# %player%, %VotePercent%, %Offender%, %ThresholdKick%, %ThresholdRespawnWave%, %ThresholdNuke%, %ThresholdKill%, %ThresholdRestartRound%, %OptionKey%, %Option%, %Counter%, %Timer%, %Custom%
  max_vote: 'Max amounts of votes done this round'
  in_progress_vote: 'A vote is currently in progress.'
  players_with_same_name: 'Multiple players have a name or partial name of %Player%. Please use a different search string.'
  option_yes: '<color=green>YES</color>'
  option_no: '<color=red>NO</color>'
  player_getting_kicked: '%Player% asks: Kick %Offender% ?'
  asked_to_kill: '%Player% Asks: Kill %Offender% '
  untouchable: '%VotePercent%% Voted to kill you.'
  not_success_full_kick: '%VotePercent%% voted yes. %ThresholdKick%% was required to kill. %Offender%.'
  player_not_found: 'Did not find any players with the name or partial name of %Player%'
  no_option_available: 'Vote does not have that option.'
  already_voted: 'You''ve already voted.'
  vote_accepted: 'Vote accepted!'
  no_permission_to_vote: 'You do not have permission to run this command!'
  call_vote_ended: 'Vote stopped.'
  results: |
    Final results:
  option_and_counter: ' %Option% (%Counter%) '
  options: '.%OptionKey% for %Option% '
  asked_question: |
    %Question% 
     Press ~ and type 
  mtf: '<color=blue>MTF</color>'
  ci: '<color=green>CI</color>'
  ci_respawn: '%VotePercent%% voted <color=green>YES</color>. Forcing the reappering of CI..'
  mtf_respawn: '%VotePercent%% voted <color=green>YES</color>. Forcing the reappering of MTF..'
  no_success_full_respawn: '%VotePercent%% voted no. %ThresholdRespawnWave%% was required to respawn the next wave.'
  asked_to_respawn: '%Player% asks: Respawn the next wave?'
  asked_to_nuke: '%Player% asks: NUKE THE FACILITY?!??'
  foundation_nuked: '%VotePercent%% voted yes. Nuking the facility...'
  no_success_full_nuke: 'Only %VotePercent%% voted yes. %ThresholdNuke%% was required to nuke the facility.'
  no_success_full_kill: 'Only %VotePercent%% voted yes. + %ThresholdKill%% was required to kill locatedPlayerName'
  player_killed: '%VotePercent%% voted yes. Killing player %Offender%'
  vote_respawn_wave_disabled: 'callvote RespawnWave is disabled.'
  vote_kick_disabled: 'callvote kick is disabled.'
  vote_kill_disabled: 'callvote kill is disabled.'
  vote_nuke_disabled: 'callvote nuke is disabled.'
  vote_restart_round_disabled: 'callvote restartround is disabled.'
  asked_to_kick: '%Player% Asks: Kick %Offender%? '
  asked_to_restart: '%Player% asks: Restart the round?'
  round_restarting: '%VotePercent% voted yes. Restarting the round...'
  no_success_full_restart: 'Only %VotePercent%% voted yes. %ThresholdRestartRound%% was required to restart the round.'
  vote_started: 'Vote has been started!'
  no_call_vote_in_progress: 'There is no vote in progress.'
  wait_to_vote: 'You should wait %Timer%s before using this command.'
  custom_vote: '%Player% asks: %Custom%'
```

# Special thanks to: 

https://github.com/Playeroth for helping me with new logic and translations.

https://github.com/PatPeter for giving the permission to continue the development of callvote.
                   
