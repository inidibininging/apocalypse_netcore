using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using System.Collections.Concurrent;
using System.Collections.Generic;

//TODO: change the namespace of the class to Apocalypse.Any.Domain.Server.Model.Layer
namespace Apocalypse.Any.Domain.Server.DataLayer
{
    public class GameStateDataLayer : IGameSectorDataLayer<
        PlayerSpaceship, 
        EnemySpaceship, 
        Item, 
        Projectile, 
        CharacterEntity, 
        CharacterEntity, 
        ImageData>
    {
        public ConcurrentBag<PlayerSpaceship> Players { get; set; } = new ConcurrentBag<PlayerSpaceship>();
        public ConcurrentBag<EnemySpaceship> Enemies { get; set; } = new ConcurrentBag<EnemySpaceship>();
        public ConcurrentBag<Projectile> Projectiles { get; set; } = new ConcurrentBag<Projectile>();
        public ConcurrentBag<Item> Items { get; set; } = new ConcurrentBag<Item>();
        public ConcurrentBag<CharacterEntity> GeneralCharacter { get; set; } = new ConcurrentBag<CharacterEntity>();
        public ConcurrentBag<ImageData> ImageData { get; set; } = new ConcurrentBag<ImageData>();
        public ConcurrentBag<Item> PlayerItems { get; set; } = new ConcurrentBag<Item>();
    }
}