# Description
This is a plugin for [Smod2](https://github.com/Grover-c13/Smod2) that allows calling Kick, RestartRound, or custom votes using console commands using the same format as the Source Engine (Left 4 Dead 2/Counter-Strike: Global Offensive).

## Configuration Setting
Setting Key | Value Type | Default Value | Description
--- | --- | --- | ---
callvote_allowed_roles | boolean | true | Roles allowed to call custom votes. If Kick or RestartRound are enabled, all players are allowed to call them.
callvote_vote_duration | integer | 30 | Number of seconds for a vote to last for.
callvote_enable_kick | boolean | false | Can all players use callvote Kick?
callvote_enable_restartround | boolean | false | Can all players use callvote RestartRound?
callvote_threshold_kick | integer | 80 | Percentage threshold required for a player to be kicked.
callvote_threshold_restartround | integer | 80 | Percentage threshold required for a round to be restarted.