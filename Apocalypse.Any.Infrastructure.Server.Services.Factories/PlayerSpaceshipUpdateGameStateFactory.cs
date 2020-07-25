using Apocalypse.Any.Core.Utilities;
using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Server.Model;
using Apocalypse.Any.Domain.Server.Sector.Model;
using Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces.Transformations;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apocalypse.Any.Infrastructure.Server.Services.Transformations
{
    public class PlayerSpaceshipUpdateGameStateFactory : CheckWithReflectionFactoryBase<GameStateData>
    {
        public int DrawingDistance { get; set; } = 1024;
        ISerializationAdapter SerializationAdapter { get; }
        ImageToRectangleTransformationService ImageToRectangleTransformationService { get; }

        public PlayerSpaceshipUpdateGameStateFactory(ISerializationAdapter serializationAdapter, 
                ImageToRectangleTransformationService imageToRectangleTransformationService)
        {
            SerializationAdapter = serializationAdapter ?? throw new ArgumentNullException(nameof(serializationAdapter));
            ImageToRectangleTransformationService = imageToRectangleTransformationService ?? throw new ArgumentNullException(nameof(imageToRectangleTransformationService));
        }
        public override bool CanUse<TParam>(TParam instance)
        {
            return CanUseByTType<TParam, GameSectorLoginTokenBag>();
        } 
        public override List<Type> GetValidParameterTypes()
        {
            return new List<Type>() { typeof(GameSectorLoginTokenBag) };
        }

        protected override GameStateData UseConverter<TParam>(TParam parameter)
        {
            var gameSectorLoginTokenBag = (parameter as GameSectorLoginTokenBag);
            var loginToken = gameSectorLoginTokenBag?.LoginToken;
            var gameSectorLayerService = gameSectorLoginTokenBag?.GameSectorLayerService;
            return ToGameState(gameSectorLayerService, loginToken);
        }

        private GameStateData ToGameState(IGameSectorLayerService gameSector,
                                    string loginToken)
        {
            
            var player = gameSector
                .DataLayer
                .Players
                .FirstOrDefault(somePlayer => somePlayer.LoginToken == loginToken);


            //var playerRectangle = new Rectangle(player.CurrentImage.Position.ToVector2().ToPoint(), new Point((int)MathF.Round(player.Stats.Aura * (int)MathF.Round(player.CurrentImage.Width * player.CurrentImage.Scale.X))));
            var playerRectangleWithAura = ImageToRectangleTransformationService.Transform(player.CurrentImage, (int)MathF.Round(player.CurrentImage.Width * player.CurrentImage.Scale.X * player.Stats.Aura));
            var playerRectangle = ImageToRectangleTransformationService.Transform(player.CurrentImage);

            var playerCanUseDialog = gameSector
                                    .DataLayer
                                    .Layers
                                    .Where(l => l.DataAsEnumerable<IdentifiableCircularLocation>().Any(
                                            location =>
                                            {
                                                var radiusAsVector2 = location.Radius.ToVector2();
                                                var locationRectangle =  ImageToRectangleTransformationService.Transform(location.Position,
                                                                                                1f.ToVector2(),
                                                                                                (int)MathF.Round(radiusAsVector2.X),
                                                                                                (int)MathF.Round(radiusAsVector2.Y));
                                                return locationRectangle.Intersects(playerRectangle);
                                            }))
                                    .Any();

            var playerNearEnemies = gameSector
                                    .DataLayer
                                    .Enemies
                                    .Where(enemy => ImageToRectangleTransformationService.Transform(enemy.CurrentImage)
                                                    .Intersects(playerRectangleWithAura))
                                    .Any();

            var unserializedPlayerMetadata = new PlayerMetadataBag()
            {
                Stats = player.Stats,
                GameSectorTag = gameSector.Tag,
                ChosenStat = player.ChosenStat,
                Items = gameSector.DataLayer.Items.Where(item => item.OwnerName == player.Name).ToList(),
                CurrentDialog = gameSector.PlayerDialogService.GetDialogNodeByLoginToken(player.LoginToken),
                ServerEventName = playerNearEnemies ? "Enemies" : playerCanUseDialog ? "Dialog" : string.Empty
            };

            var cache = new GameStateData
            {
                Commands = new List<string>(),
                LoginToken = player.LoginToken
            };

            //pass camera data
            cache.Screen = gameSector.IODataLayer.GetGameStateByLoginToken(loginToken)?.Screen;
            
            cache.Camera = new CameraData
            {
                Position = player.CurrentImage.Position,
                Rotation = player.CurrentImage.Rotation
            };

            //convert entities to images ( this is poor data, means only data needed )
            cache.Images?.Clear();
            cache.Images = new List<ImageData>();
            cache.Images.AddRange(gameSector
                .DataLayer
                .Players
                .Where(e => Vector2.Distance(
                                e.CurrentImage.Position.ToVector2(),
                                player.CurrentImage.Position.ToVector2()) <= DrawingDistance)
                                .Select(playerInfo => playerInfo.CurrentImage)
                                .ToList());

            cache.Images.AddRange(gameSector
                .DataLayer
                .Enemies
                .Where(e => Vector2.Distance(
                                e.CurrentImage.Position.ToVector2(),
                                player.CurrentImage.Position.ToVector2()) <= DrawingDistance)
                .Select(enemy => enemy.CurrentImage).ToList());

            cache.Images.AddRange(gameSector
                .DataLayer
                .Projectiles
                .Where(e => Vector2.Distance(
                                e.CurrentImage.Position.ToVector2(),
                                player.CurrentImage.Position.ToVector2()) <= DrawingDistance)
                .Select(projectile => projectile.CurrentImage).ToList());

            cache.Images.AddRange(gameSector
                .DataLayer
                .Items
                .Where(e => Vector2.Distance(
                                e.CurrentImage.Position.ToVector2(),
                                player.CurrentImage.Position.ToVector2()) <= DrawingDistance && !e.Taken)
                .Select(item => item.CurrentImage).ToList());

            cache.Images.AddRange(gameSector
                .DataLayer
                .GeneralCharacter
                .Where(e => Vector2.Distance(
                                e.CurrentImage.Position.ToVector2(),
                                player.CurrentImage.Position.ToVector2()) <= DrawingDistance)
                .Select(prop => prop.CurrentImage).ToList());

            cache.Images.AddRange(gameSector
                .DataLayer
                .ImageData
                .Where(e => Vector2.Distance(
                                e.Position.ToVector2(),
                                player.CurrentImage.Position.ToVector2()) <= DrawingDistance + 1024 || e.SelectedFrame.Contains("fog"))
                .ToList());

            //get selected item of player (just in case) / for now...
            var previousSelectedItem = gameSector
                                        .DataLayer
                                        .PlayerItems
                                        .FirstOrDefault(pItem => pItem.OwnerName == player.Name && pItem.Selected);


            unserializedPlayerMetadata.TimeStamp = DateTime.Now;

            //Position of the meta data is here now.. but it should be changed later on
            cache.Metadata = new IdentifiableNetworkCommand()
            {
                Id = player.CurrentImage.Id,
                CommandName = gameSector.Tag,
                CommandArgument = unserializedPlayerMetadata.GetType().FullName,
                Data = SerializationAdapter.SerializeObject(unserializedPlayerMetadata)
            };


            return cache;
        }
    }
}