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
	class CallvoteEvents : IEventHandlerCallCommand, IEventHandlerWaitingForPlayers
	{
		private readonly CallvotePlugin plugin;

		public CallvoteEvents(CallvotePlugin plugin)
		{
			this.plugin = plugin;
		}

		public void OnCallCommand(PlayerCallCommandEvent ev)
		{
			string command = ev.Command.Split(' ')[0];
			switch (command)
			{
				case "callvote":
					string[] quotedArgs = Regex.Matches(string.Join(" ", ev.Command), "[^\\s\"\']+|\"([^\"]*)\"|\'([^\']*)\'")
						.Cast<Match>()
						.Select(m => m.Value)
						.ToArray()
						.Skip(1)
						.ToArray();
					ev.ReturnMessage = this.plugin.startVote(ev.Player, quotedArgs);
					break;

				case "stopvote":
					ev.ReturnMessage = this.plugin.stopVote(ev.Player);
					break;

				case "yes":
				case "no":
				case "1":
				case "2":
				case "3":
				case "4":
				case "5":
				case "6":
				case "7":
				case "8":
				case "9":
				case "0":
					if (this.plugin.currentVote != null)
					{
						switch (ev.Command)
						{
							case "yes":
								ev.ReturnMessage = this.plugin.handleVote(ev.Player, 1);
								break;

							case "no":
								ev.ReturnMessage = this.plugin.handleVote(ev.Player, 2);
								break;

							case "1":
								ev.ReturnMessage = this.plugin.handleVote(ev.Player, 1);
								break;

							case "2":
								ev.ReturnMessage = this.plugin.handleVote(ev.Player, 2);
								break;

							case "3":
								ev.ReturnMessage = this.plugin.handleVote(ev.Player, 3);
								break;

							case "4":
								ev.ReturnMessage = this.plugin.handleVote(ev.Player, 4);
								break;

							case "5":
								ev.ReturnMessage = this.plugin.handleVote(ev.Player, 5);
								break;

							case "6":
								ev.ReturnMessage = this.plugin.handleVote(ev.Player, 6);
								break;

							case "7":
								ev.ReturnMessage = this.plugin.handleVote(ev.Player, 7);
								break;

							case "8":
								ev.ReturnMessage = this.plugin.handleVote(ev.Player, 8);
								break;

							case "9":
								ev.ReturnMessage = this.plugin.handleVote(ev.Player, 9);
								break;

							case "0":
								ev.ReturnMessage = this.plugin.handleVote(ev.Player, 10);
								break;
						}
					}
					else
					{
						ev.ReturnMessage = "No vote is in progress.";
					}
					break;
			}
		}

		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			plugin.ReloadConfig();
			if (this.plugin.currentVote != null && this.plugin.currentVote.timer != null)
			{
				this.plugin.currentVote.timer.Stop();
			}
			this.plugin.currentVote = null;
		}
	}
}
