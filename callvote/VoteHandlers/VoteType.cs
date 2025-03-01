using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace callvote.VoteHandlers
{
    public class VoteType
    {
        public string Question; 
        public Dictionary<string, string> Options; //Option and Option Description
        public HashSet<string> Votes;
        public Dictionary<string, int> Counter; //Option and Votes to that Option
        public Timer Timer;
        public CallvoteFunction Callback;

        public VoteType(string question, Dictionary<string, string> options)
        {
            this.Question = question;
            this.Options = options;
            this.Votes = new HashSet<string>();
            this.Counter = new Dictionary<string, int>();
            foreach (string option in options.Keys)
            {
                Counter[option] = 0;
            }
        }

        // Allow Votes and Counter to be passed in and saved by reference for Event code
        public VoteType(string question, Dictionary<string, string> options, HashSet<string> votes, Dictionary<string, int> counter)
        {
            this.Question = question;
            this.Options = options;
            this.Votes = votes;
            this.Counter = counter;
            foreach (string option in options.Keys)
            {
                Counter[option] = 0;
            }
        }
    }
}
