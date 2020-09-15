/*
 * Copyright (c) 2019, Nicholas Solin a.k.a. PatPeter
 * All rights reserved.
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of the Universal Gaming Alliance nor the
 *       names of its contributors may be used to endorse or promote products
 *       derived from this software without specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL NICHOLAS SOLIN BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
using System;
using System.Text.RegularExpressions;
using Smod2;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;
using System.Linq;

namespace Callvote
{
	class EventHandlers : IEventHandlerCallCommand, IEventHandlerWaitingForPlayers
	{
		private readonly Plugin plugin;

		public EventHandlers(Plugin plugin)
		{
			this.plugin = plugin;
		}

		public void OnCallCommand(PlayerCallCommandEvent ev)
		{
			string command = ev.Command.Split(' ')[0];

			int option;
			if (int.TryParse(command, out option))
			{
				if (this.plugin.Voting())
				{

					ev.ReturnMessage = this.plugin.VoteHandler(ev.Player, option);
				}
				else
				{
					ev.ReturnMessage = "No vote is in progress.";
				}
			}
			else
			{

				switch (command)
				{
					case "callvote":
						string[] quotedArgs = Regex.Matches(string.Join(" ", ev.Command), "[^\\s\"\']+|\"([^\"]*)\"|\'([^\']*)\'")
							.Cast<Match>()
							.Select(m => m.Value)
							.ToArray()
							.Skip(1)
							.ToArray();
						ev.ReturnMessage = this.plugin.CallvoteHandler(ev.Player, quotedArgs);
						break;

					case "stopvote":
						ev.ReturnMessage = this.plugin.StopvoteHandler(ev.Player);
						break;

					case "yes":
						ev.ReturnMessage = this.plugin.VoteHandler(ev.Player, 1);
						break;

					case "no":
						ev.ReturnMessage = this.plugin.VoteHandler(ev.Player, 2);
						break;
				}
			}
		}

		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			plugin.ReloadConfig();
			if (this.plugin.CurrentVote != null && this.plugin.CurrentVote.Timer != null)
			{
				this.plugin.CurrentVote.Timer.Stop();
				this.plugin.CurrentVote.Timer.Dispose();
			}
			this.plugin.CurrentVote = null;
		}
	}
}
