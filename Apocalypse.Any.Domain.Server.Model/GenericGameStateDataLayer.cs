using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Server.DataLayer;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Domain.Server.Model
{
    public class GenericGameStateDataLayer : 
        GameStateDataLayer,
        IExpandedGameSectorDataLayer<
            PlayerSpaceship,
            EnemySpaceship,
            Item,
            Projectile,
            CharacterEntity,
            CharacterEntity,
            ImageData>
    {
        public ConcurrentBag<IGenericTypeDataLayer> Layers { get; set; } = new ConcurrentBag<IGenericTypeDataLayer>();
    }
}
