using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Apocalypse.Any.Core.Input;
using Apocalypse.Any.Core.Input.Translator;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.GameServer.States.Sector.Mechanics.PlayerMechanics
{
    /// <summary>
    /// This state is only for serverside client that get server authorative commands
    /// </summary>
    public class ProcessPressReleaseState : IState<string, IGameSectorLayerService>
    {
        public Dictionary<string, List<string>> KeyDownUp { get; set; } = new Dictionary<string, List<string>>();

        public void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            //TODO:
            //1. get key down and remember
            //2. if release pressed kill it and stop passing releases 
            //3. keep executing the pressed keys
            foreach (var player in machine.SharedContext.DataLayer.Players)
            {
                var playerGameStateData = machine.SharedContext.IODataLayer.GetGameStateByLoginToken(player.LoginToken);
                
                if (!KeyDownUp.ContainsKey(player.LoginToken) || KeyDownUp[player.LoginToken] == null)
                    KeyDownUp[player.LoginToken] = new List<string>();

                //get key down and remember  
                foreach (var cmd in playerGameStateData.Commands)
                {
                    Console.WriteLine(cmd);
                    
                    if (cmd.Contains(DefaultKeys.Press))
                    {
                        var pressKey = cmd.Replace(DefaultKeys.Press, "");
                        if (!KeyDownUp[player.LoginToken].Contains(cmd))
                            KeyDownUp[player.LoginToken].Add(pressKey);
                    }

                    if (!cmd.Contains(DefaultKeys.Release)) continue;
                    var releaseKey = cmd.Replace(DefaultKeys.Release, "");
                    
                    //if release pressed kill it and stop passing releases 
                    if (KeyDownUp[player.LoginToken].Contains(releaseKey))
                        KeyDownUp[player.LoginToken].Remove(releaseKey);
                }

                //keep executing the pressed keys as commands
                foreach (var cmd in KeyDownUp[player.LoginToken])
                {

                    
                    foreach (var mappedCommand in InputMapper.DefaultRotationMap)
                    {
                        mappedCommand
                            .Translate(cmd)?
                            .ToList()
                            .ForEach(foundCmd => foundCmd.Execute(player.CurrentImage.Rotation));
                    }

                    foreach (var mappedCommand in InputMapper.DefaultMovementMap)
                    {
                        mappedCommand
                            .Translate(cmd)?
                            .ToList()
                            .ForEach(foundCmd => foundCmd.Execute(player.CurrentImage.Position));
                    }

                }
            }
            
        }
    }
}