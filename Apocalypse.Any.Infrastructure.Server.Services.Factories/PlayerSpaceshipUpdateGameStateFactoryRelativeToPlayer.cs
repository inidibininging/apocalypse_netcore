﻿using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Server.Model;
using Apocalypse.Any.Domain.Server.Sector.Model;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apocalypse.Any.Infrastructure.Server.Services.Transformations
{
    public class PlayerSpaceshipUpdateGameStateFactoryRelativeToPlayer : CheckWithReflectionFactoryBase<GameStateData>
    {
        public int DrawingDistance { get; set; } = 768;
        public override bool CanUse<TParam>(TParam instance)
        {
            return CanUseByTType<TParam, GameStateData>();
        }

        public override List<Type> GetValidParameterTypes()
        {
            return new List<Type>() { typeof(GameStateData) };
        }

        protected override GameStateData UseConverter<TParam>(TParam parameter)
        {
            if (!(parameter is GameStateData gameStateData))
                return null;
            return ToGameState(gameStateData);
        }

        private GameStateData ToGameState(GameStateData gameSector)
        {
            if(gameSector.Screen == null){
                Console.WriteLine($"screen is null {DateTime.Now.ToString()}");
                return gameSector;
            }
                
            
            var relativePosition = new Vector2(gameSector.Screen.ScreenWidth / 2,
                                                gameSector.Screen.ScreenHeight / 2);
            gameSector.Images.ForEach(img =>
            {
                img.Position.X -= relativePosition.X;
                img.Position.Y -= relativePosition.Y;
            });
            return gameSector;
        }
    }
}