using Smod2.Commands;
using Smod2.API;

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
			return "Prints hello world";
		}

		public string GetUsage()
		{
			// This prints when someone types HELP HELLO
			return "HELLO";
		}

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			//Will be null if command was called by server console
			Player caller = sender as Player;
			// This will print 3 lines in console.
			return new string[] { this.plugin.startVote(caller, args) };
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
			return new string[] { this.plugin.handleVote(caller, 1) };
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
			return new string[] { this.plugin.handleVote(caller, 2) };
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
			return new string[] { this.plugin.handleVote(caller, 3) };
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
			return new string[] { this.plugin.handleVote(caller, 4) };
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
			return new string[] { this.plugin.handleVote(caller, 5) };
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
			return new string[] { this.plugin.handleVote(caller, 6) };
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
			return new string[] { this.plugin.handleVote(caller, 7) };
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
			return new string[] { this.plugin.handleVote(caller, 8) };
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
			return new string[] { this.plugin.handleVote(caller, 9) };
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
			return new string[] { this.plugin.handleVote(caller, 10) };
		}
	}
}
