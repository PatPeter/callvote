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
using Smod2.Commands;
using Smod2.API;
using System.Text.RegularExpressions;
using System.Linq;

namespace Callvote
{
	class CallvoteCommand : ICommandHandler
	{
		private readonly CallvotePlugin plugin;

		public CallvoteCommand(CallvotePlugin plugin)
		{
			//Constructor passing plugin refrence to this class
			this.plugin = plugin;
		}

		public string GetCommandDescription()
		{
			// This prints when someone types HELP HELLO
			return "Starts a vote.";
		}

		public string GetUsage()
		{
			// This prints when someone types HELP HELLO
			return "CALLVOTE Kick/RestartRound/<custom> <player>/[options]";
		}

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			//Will be null if command was called by server console
			Player caller = sender as Player;
			// This will print 3 lines in console.
			string[] quotedArgs = Regex.Matches(string.Join(" ", args), "[^\\s\"\']+|\"([^\"]*)\"|\'([^\']*)\'")
				.Cast<Match>()
				.Select(m => m.Value)
				.ToArray();
			//string[] quotedArgs = new string[quoteDelimitedArguments.Count - 1];
			return new string[] { this.plugin.callvoteHandler(caller, quotedArgs) };
		}
	}
	class StopvoteCommand : ICommandHandler
	{
		private readonly CallvotePlugin plugin;

		public StopvoteCommand(CallvotePlugin plugin)
		{
			//Constructor passing plugin refrence to this class
			this.plugin = plugin;
		}

		public string GetCommandDescription()
		{
			// This prints when someone types HELP HELLO
			return "Ends a vote.";
		}

		public string GetUsage()
		{
			// This prints when someone types HELP HELLO
			return "STOPVOTE";
		}

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			//Will be null if command was called by server console
			Player caller = sender as Player;
			// This will print 3 lines in console.
			return new string[] { this.plugin.stopvoteHandler(caller) };
		}
	}

	class Vote1Command : ICommandHandler
	{
		private readonly CallvotePlugin plugin;

		public Vote1Command(CallvotePlugin plugin)
		{
			//Constructor passing plugin refrence to this class
			this.plugin = plugin;
		}

		public string GetCommandDescription()
		{
			// This prints when someone types HELP HELLO
			return "Vote for option 1.";
		}

		public string GetUsage()
		{
			// This prints when someone types HELP HELLO
			return "1";
		}

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			//Will be null if command was called by server console
			Player caller = sender as Player;
			// This will print 3 lines in console.
			return new string[] { this.plugin.HandleVote(caller, 1) };
		}
	}

	class Vote2Command : ICommandHandler
	{
		private readonly CallvotePlugin plugin;

		public Vote2Command(CallvotePlugin plugin)
		{
			//Constructor passing plugin refrence to this class
			this.plugin = plugin;
		}

		public string GetCommandDescription()
		{
			// This prints when someone types HELP HELLO
			return "Vote for option 2.";
		}

		public string GetUsage()
		{
			// This prints when someone types HELP HELLO
			return "2";
		}

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			//Will be null if command was called by server console
			Player caller = sender as Player;
			// This will print 3 lines in console.
			return new string[] { this.plugin.HandleVote(caller, 2) };
		}
	}

	class Vote3Command : ICommandHandler
	{
		private readonly CallvotePlugin plugin;

		public Vote3Command(CallvotePlugin plugin)
		{
			//Constructor passing plugin refrence to this class
			this.plugin = plugin;
		}

		public string GetCommandDescription()
		{
			// This prints when someone types HELP HELLO
			return "Vote for option 3.";
		}

		public string GetUsage()
		{
			// This prints when someone types HELP HELLO
			return "3";
		}

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			//Will be null if command was called by server console
			Player caller = sender as Player;
			// This will print 3 lines in console.
			return new string[] { this.plugin.HandleVote(caller, 3) };
		}
	}

	class Vote4Command : ICommandHandler
	{
		private readonly CallvotePlugin plugin;

		public Vote4Command(CallvotePlugin plugin)
		{
			//Constructor passing plugin refrence to this class
			this.plugin = plugin;
		}

		public string GetCommandDescription()
		{
			// This prints when someone types HELP HELLO
			return "Vote for option 4.";
		}

		public string GetUsage()
		{
			// This prints when someone types HELP HELLO
			return "4";
		}

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			//Will be null if command was called by server console
			Player caller = sender as Player;
			// This will print 3 lines in console.
			return new string[] { this.plugin.HandleVote(caller, 4) };
		}
	}

	class Vote5Command : ICommandHandler
	{
		private readonly CallvotePlugin plugin;

		public Vote5Command(CallvotePlugin plugin)
		{
			//Constructor passing plugin refrence to this class
			this.plugin = plugin;
		}

		public string GetCommandDescription()
		{
			// This prints when someone types HELP HELLO
			return "Vote for option 5.";
		}

		public string GetUsage()
		{
			// This prints when someone types HELP HELLO
			return "5";
		}

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			//Will be null if command was called by server console
			Player caller = sender as Player;
			// This will print 3 lines in console.
			return new string[] { this.plugin.HandleVote(caller, 5) };
		}
	}

	class Vote6Command : ICommandHandler
	{
		private readonly CallvotePlugin plugin;

		public Vote6Command(CallvotePlugin plugin)
		{
			//Constructor passing plugin refrence to this class
			this.plugin = plugin;
		}

		public string GetCommandDescription()
		{
			// This prints when someone types HELP HELLO
			return "Vote for option 6.";
		}

		public string GetUsage()
		{
			// This prints when someone types HELP HELLO
			return "6";
		}

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			//Will be null if command was called by server console
			Player caller = sender as Player;
			// This will print 3 lines in console.
			return new string[] { this.plugin.HandleVote(caller, 6) };
		}
	}

	class Vote7Command : ICommandHandler
	{
		private readonly CallvotePlugin plugin;

		public Vote7Command(CallvotePlugin plugin)
		{
			//Constructor passing plugin refrence to this class
			this.plugin = plugin;
		}

		public string GetCommandDescription()
		{
			// This prints when someone types HELP HELLO
			return "Vote for option 7.";
		}

		public string GetUsage()
		{
			// This prints when someone types HELP HELLO
			return "7";
		}

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			//Will be null if command was called by server console
			Player caller = sender as Player;
			// This will print 3 lines in console.
			return new string[] { this.plugin.HandleVote(caller, 7) };
		}
	}

	class Vote8Command : ICommandHandler
	{
		private readonly CallvotePlugin plugin;

		public Vote8Command(CallvotePlugin plugin)
		{
			//Constructor passing plugin refrence to this class
			this.plugin = plugin;
		}

		public string GetCommandDescription()
		{
			// This prints when someone types HELP HELLO
			return "Vote for option 8.";
		}

		public string GetUsage()
		{
			// This prints when someone types HELP HELLO
			return "8";
		}

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			//Will be null if command was called by server console
			Player caller = sender as Player;
			// This will print 3 lines in console.
			return new string[] { this.plugin.HandleVote(caller, 8) };
		}
	}

	class Vote9Command : ICommandHandler
	{
		private readonly CallvotePlugin plugin;

		public Vote9Command(CallvotePlugin plugin)
		{
			//Constructor passing plugin refrence to this class
			this.plugin = plugin;
		}

		public string GetCommandDescription()
		{
			// This prints when someone types HELP HELLO
			return "Vote for option 9.";
		}

		public string GetUsage()
		{
			// This prints when someone types HELP HELLO
			return "9";
		}

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			//Will be null if command was called by server console
			Player caller = sender as Player;
			// This will print 3 lines in console.
			return new string[] { this.plugin.HandleVote(caller, 9) };
		}
	}

	class Vote0Command : ICommandHandler
	{
		private readonly CallvotePlugin plugin;

		public Vote0Command(CallvotePlugin plugin)
		{
			//Constructor passing plugin refrence to this class
			this.plugin = plugin;
		}

		public string GetCommandDescription()
		{
			// This prints when someone types HELP HELLO
			return "Vote for option 0.";
		}

		public string GetUsage()
		{
			// This prints when someone types HELP HELLO
			return "0";
		}

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			//Will be null if command was called by server console
			Player caller = sender as Player;
			// This will print 3 lines in console.
			return new string[] { this.plugin.HandleVote(caller, 10) };
		}
	}
}
