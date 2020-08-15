using Apocalypse.Any.Core.Input.Translator;
using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Infrastructure.Server.Services.Data;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Microsoft.Xna.Framework;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;

namespace Apocalypse.Any.GameServer.States.Sector.Mechanics.PlayerMechanics
{
    public class ProcessPlayerDialogsRequestsState : IState<string, IGameSectorLayerService>
    {
        public const string PlayerOnDialogEvent = nameof(PlayerOnDialogEvent);
        private const string PlayerOnGameEvent = nameof(PlayerOnGameEvent);
        public string RelationLayerName { get; }

        public ProcessPlayerDialogsRequestsState(string relationLayerName)
        {
            if (string.IsNullOrWhiteSpace(relationLayerName))
                throw new ArgumentNullException(nameof(relationLayerName));
             RelationLayerName = relationLayerName;
        }
        public void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            machine
                .SharedContext
                .DataLayer
                .Players
                .ToList()
                .ForEach(player =>
                {

                    machine
                    .SharedContext
                    .IODataLayer
                    .GetGameStateByLoginToken(player.LoginToken)?
                    .Commands.ForEach(cmd =>
                    {
                        if (string.IsNullOrWhiteSpace(cmd))
                            return;
                        if (cmd.Contains(DefaultKeys.OpenDialog) || 
                            cmd.Contains(DefaultKeys.Use))
                        {
                            //TODO: language interpreter for commands. if so, this needs speed and caching
                            
                            var commandAndArgument = cmd.Split(' ');


                            //look for any location, that can be triggered
                            //maybe this should be done with order by
                            var nearLocation = machine
                                                .SharedContext
                                                .DataLayer
                                                .Layers
                                                .Where(layer => layer.GetValidTypes().Any(t => t == typeof(IdentifiableCircularLocation)))
                                                .SelectMany(layer => layer.DataAsEnumerable<IdentifiableCircularLocation>())
                                                .FirstOrDefault(location => Vector2.Distance(player.CurrentImage.Position, location.Position) <= location.Radius);
                            if (nearLocation == null)
                                return;

                            //uses the command without a dialog argument. this means the player doesn't explicitly know what it is choosing
                            if (commandAndArgument.Length == 1)
                            {
                               

                                var nearLocationDialogRelation = machine
                                                    .SharedContext
                                                    .DataLayer
                                                    .Layers
                                                    .Where(layer => layer.DisplayName == RelationLayerName && 
                                                                                  layer.GetValidTypes().Any(t => t == typeof(DynamicRelation)))
                                                    .SelectMany(layer => layer.DataAsEnumerable<DynamicRelation>())
                                                    .FirstOrDefault(relation => (relation.Entity1 == typeof(IdentifiableCircularLocation) &&
                                                                                relation.Entity2 == typeof(DialogNode) &&
                                                                                relation.Entity1Id == nearLocation.Id) ||

                                                                                (relation.Entity1 == typeof(DialogNode) &&
                                                                                relation.Entity2 == typeof(IdentifiableCircularLocation) &&
                                                                                relation.Entity2Id == nearLocation.Id));

                                if (nearLocationDialogRelation == null)
                                    return;
                                

                                //find any dialog for this location                                
                                var locationDialog = machine
                                                    .SharedContext
                                                    .DataLayer
                                                    .Layers
                                                    .Where(layer => layer.GetValidTypes().Any(t => t == typeof(DialogNode)))
                                                    .SelectMany(layer => layer.DataAsEnumerable<DialogNode>())
                                                    .FirstOrDefault(dialog =>
                                                    (nearLocationDialogRelation.Entity1 == typeof(DialogNode) &&
                                                    nearLocationDialogRelation.Entity1Id == dialog.Id) ||
                                                    (nearLocationDialogRelation.Entity2 == typeof(DialogNode) &&
                                                    nearLocationDialogRelation.Entity2Id == dialog.Id));
                                    
                                //pass dialog to player dialog service if needed
                                if(locationDialog != null && (locationDialog.Requirement == null || player.Stats >= locationDialog.Requirement))
                                {                                        
                                    machine.SharedContext.PlayerDialogService.SwitchDialogNodeByLoginToken(player.LoginToken, locationDialog.Id);
                                    if (!player.Tags.Contains(PlayerOnDialogEvent))
                                        player.Tags.Add(PlayerOnDialogEvent);
                                }
                                //else
                                //{
                                //    machine.SharedContext.PlayerDialogService.SwitchDialogNodeByLoginToken(player.LoginToken, null);
                                //    if (player.Factions.Contains(PlayerOnDialogEvent))
                                //        player.Factions.Remove(PlayerOnDialogEvent);
                                //}
                            }

                            //commented for now. the player should not be capable of choosing the dialog

                            ////chooses dialog by id
                            if (commandAndArgument.Length >= 2)
                            {
                                //first argument should be the command itself. rest is the argument
                                var dialogId = string.Join(' ', commandAndArgument.Skip(1));
                                machine.SharedContext
                                    .PlayerDialogService
                                    .SwitchDialogNodeByLoginToken(player.LoginToken, dialogId);
                            }
                        }
                        if (cmd == DefaultKeys.CloseDialog)
                        {
                            if (player.Tags.Contains(PlayerOnDialogEvent))
                                player.Tags.Remove(PlayerOnDialogEvent);

                            machine.SharedContext
                                .PlayerDialogService
                                .SwitchDialogNodeByLoginToken(player.LoginToken, null);
                        }
                    });
                });
            }
    }
}
