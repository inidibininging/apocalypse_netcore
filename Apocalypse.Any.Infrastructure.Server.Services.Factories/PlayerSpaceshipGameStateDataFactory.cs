using System;
using System.Collections.Generic;
using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Server.Model;

namespace Apocalypse.Any.Infrastructure.Server.Services.Factories
{
    public class PlayerSpaceshipGameStateDataFactory : CheckWithReflectionFactoryBase<GameStateData>
    {
        //and this is what i didn't want. reflection, hidden inside generics.
        //Dear reflection: you won. for now.
        public override bool CanUse<TParam>(TParam parameter) => typeof(TParam) == typeof(PlayerSpaceship);
        public override List<Type> GetValidParameterTypes()
        {
            return new List<Type>() { typeof(PlayerSpaceship) };
        }

        protected override GameStateData UseConverter<TParam>(TParam parameter)
        {
            var player = parameter as PlayerSpaceship ?? throw new ArgumentNullException(nameof(parameter));
            
            Console.WriteLine($"Creating game state for player {player.DisplayName}");
            return new GameStateData()
            {
                LoginToken = player.LoginToken,
                Id = Guid.NewGuid().ToString(),
                Camera = new CameraData()
                {
                    Position = player.CurrentImage.Position,
                    Rotation = player.CurrentImage.Rotation
                },
                Images = new List<ImageData>()
                {
                    player.CurrentImage
                },
                Commands = new List<string>()
            };
        }
    }
}