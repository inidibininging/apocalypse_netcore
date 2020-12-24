using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Core.Input.Translator;
using Apocalypse.Any.Domain.Common.DrawingOrder;
using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Server.Sector.Model;
using Apocalypse.Any.Infrastructure.Common.Services;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Microsoft.Xna.Framework;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Apocalypse.Any.Constants;

namespace Apocalypse.Any.GameServer.States.Sector.Mechanics.PlayerMechanics
{
    public class ProcessThrustForPlayerMechanicsState : IState<string, IGameSectorLayerService>
    {
        private const int BurstPositionDunnoWtf = (64 - 16);

        public DirectionVectorFactory DirectionVector { get; set; } = new DirectionVectorFactory();
        public List<CharacterPositionTimeTrigger> PlayerTriggers { get; set; } = new List<CharacterPositionTimeTrigger>();

        public ProcessThrustForPlayerMechanicsState()
        {

        }

        private void ProcessTriggerForPlayer(IEnumerable<CharacterEntity> characterEntities)
        {

            //PlayerTriggers
            //    .Where(plyrTrigger => plyrTrigger.MilisecondsCounter.TotalMilliseconds < plyrTrigger.MilisecondsToTrigger);
            //{
            //    var addedTimeSpan = (DateTime.Now.TimeOfDay - machine.SharedContext.CurrentGameTime.ElapsedGameTime);
            //    MilisecondsCounter = MilisecondsCounter.Add(addedTimeSpan);
            //    LerpIt = false;
            //}
            //else
            //{
            //    //MilisecondsCounter = TimeSpan.Zero;
            //    if (MilisecondsCounter.TotalMilliseconds > Miliseconds * 2)
            //    {
            //        LerpIt = false;
            //        MilisecondsCounter = TimeSpan.Zero;
            //    }
            //    else
            //    {
            //        LerpIt = true;
            //    }
            //}
        }
        public void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            
            machine
                .SharedContext
                .DataLayer
                .Players
                .Where(player => !string.IsNullOrWhiteSpace(player.LoginToken))
                .ToList()
                .ForEach(player =>
            {
                var playerGameState = machine.SharedContext.IODataLayer.GetGameStateByLoginToken(player.LoginToken);
                playerGameState.Commands.ForEach(cmd =>
                {
                    if (cmd == DefaultKeys.Boost)
                    {
                        //Fix for not using boost for players in a dialog
                        if (player.Tags.Contains(ProcessPlayerDialogsRequestsState.PlayerOnDialogEvent))
                        {
                            return;
                        }
                        
                        //add boost tag to player. for now this is needed by mechanics who need to know if the player is boosting or not
                        if(!player.Tags.Contains(DefaultKeys.Boost))
                            player.Tags.Add(DefaultKeys.Boost);
                        

                        var playerPositionBeforeThrust = new MovementBehaviour()
                        {
                            X = player.CurrentImage.Position.X,
                            Y = player.CurrentImage.Position.Y
                        };

                        for (int currentSpeedTime = 0; currentSpeedTime < player.Stats.Speed; currentSpeedTime++)
                        {
                            machine.SharedContext.SingularMechanics.PlayerMechanics["thrust_players"].Update(player);
                        }
                        machine.SharedContext.SingularMechanics.PlayerMechanics["thrust_players_zero"].Update(player);

                        var playerDirection = DirectionVector.Translate(player.CurrentImage) * BurstPositionDunnoWtf;//todo: fix this. this is due to the sourceRect.
                        
                        //make burst image for a thrust
                        machine.SharedContext.DataLayer.ImageData.Add(new ImageData()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Alpha = new AlphaBehaviour() { Alpha = 1.0f },
                            Path = ImagePaths.gamesheetExtended,
                            SelectedFrame = (ImagePaths.ThrustFrame, 6, 8), //TODO: make a burst image
                            Height = 32,
                            Width = 32,
                            Scale = new Vector2(2.5f, 2.5f),
                            Color = Color.Orange,

                            Position = new MovementBehaviour()
                            {
                                X = player.CurrentImage.Position.X + playerDirection.X * -1,
                                Y = player.CurrentImage.Position.Y + playerDirection.Y * -1,
                            },
                            Rotation = new RotationBehaviour()
                            {
                                Rotation = player.CurrentImage.Rotation.Rotation
                            },
                            LayerDepth = DrawingPlainOrder.EntitiesFX
                        });
                    }

                });
                
                if (player.Tags.Contains(DefaultKeys.Boost))
                {
                    player.Tags.Remove(DefaultKeys.Boost);
                }
                else
                {
                    machine.SharedContext.SingularMechanics.PlayerMechanics["thrust_players_zero"].Update(player);
                }
            });
            
        }
    }
}