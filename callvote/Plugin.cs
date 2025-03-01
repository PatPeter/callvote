using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using MEC;
using Exiled.Permissions.Extensions;
using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;
using GameCore;
using Log = Exiled.API.Features.Log;
using UnityEngine;
using PluginAPI;
using Exiled.API.Features.Hazards;
using Mirror;
using Respawning.Waves;
using callvote.VoteHandlers;


namespace callvote
{
    public class Plugin : Plugin<Config, Translation>
    {
        public static Plugin Instance;

        public override string Name { get; } = Callvote.AssemblyInfo.Name;
        public override string Author { get; } = Callvote.AssemblyInfo.Author;
        public override System.Version Version { get; } = new System.Version(Callvote.AssemblyInfo.Version);
        public override string Prefix { get; } = Callvote.AssemblyInfo.LangFile;
        public override System.Version RequiredExiledVersion { get; } = new System.Version(5, 1, 3);
        public override PluginPriority Priority { get; } = PluginPriority.Default;

        //Instance variable for eventhandlers
        public EventHandlers EventHandlers;

        public VoteType CurrentVote = null;

        public int roundtimer = 0;




        public Dictionary<int, int> DictionaryOfVotes = new Dictionary<int, int>();
        public int TimeOfLastVote = 0;
        public CoroutineHandle VoteCoroutine = new CoroutineHandle();



        public override void OnEnabled()
        {
            try
            {
                Log.Debug("Initializing event handlers..");
                //Set instance varible to a new instance, this should be nulled again in OnDisable
                EventHandlers = new EventHandlers(this);
                Instance = this;
                //Hook the events you will be using in the plugin. You should hook all events you will be using here, all events should be unhooked in OnDisabled 
                Exiled.Events.Handlers.Server.WaitingForPlayers += EventHandlers.OnWaitingForPlayers;
                Exiled.Events.Handlers.Server.RoundEnded += EventHandlers.OnRoundEnded;
                Exiled.Events.Handlers.Server.RoundStarted += EventHandlers.OnRoundStarted;
                Exiled.Events.Handlers.Server.RoundStarted += EventHandlers.OnRoundStarted;
                
                Log.Debug($"callvote loaded!");
            }
            catch (Exception e)
            {
                //This try catch is redundant, as EXILED will throw an error before this block can, but is here as an example of how to handle exceptions/errors
                Log.Error($"There was an error loading the plugin: {e}");
            }
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Server.RoundStarted -= EventHandlers.OnRoundStarted;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= EventHandlers.OnWaitingForPlayers;
            Exiled.Events.Handlers.Server.RoundEnded -= EventHandlers.OnRoundEnded;
            Instance = null;
            EventHandlers = null;
        }

        public override void OnReloaded()
        {
            //This is only fired when you use the EXILED reload command, the reload command will call OnDisable, OnReload, reload the plugin, then OnEnable in that order. There is no GAC bypass, so if you are updating a plugin, it must have a unique assembly name, and you need to remove the old version from the plugins folder
        }
    }
}
