using Apocalypse.Any.Domain.Common.Mechanics;
using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics.SectorMechanics
{
    /// <summary>
    /// Transfers players items, and bank account between sectors
    /// </summary>
    public class TransferPlayerStuffBetweenSectorsMechanic
        : ISingleUpdeatableMechanic<IGameSectorsOwner, IGameSectorsOwner>
    {
        public bool Active { get; set; } = true;
        public IGameSectorsOwner Update(IGameSectorsOwner entity)
        {
            if (!Active)
                return entity;

            foreach (var sector in entity
                                    .GameSectorLayerServices
                                    .Values
                                    .Where(s => s.SharedContext.CurrentStatus == Domain.Server.Model.Interfaces.GameSectorStatus.Running))
            {
                foreach (var player in sector
                                        .SharedContext
                                        .DataLayer
                                        .Players)
                {
                    foreach(var otherSector in entity
                                                .GameSectorLayerServices
                                                .Values
                                                .Where(s => s.SharedContext.Tag != sector.SharedContext.Tag))
                    {
                        var items = otherSector
                                        .SharedContext
                                        .DataLayer
                                        .Items
                                        .Where(i => i.OwnerName == player.Id);

                        if (items.Any())
                        {
                            otherSector
                                .SharedContext
                                .DataLayer
                                .Items = new ConcurrentBag<Item>(otherSector
                                                                    .SharedContext
                                                                    .DataLayer
                                                                    .Items
                                                                    .Except(items));
                            Console.WriteLine($"Transfering stuff of player {player.Id} from {otherSector.SharedContext.Tag} to {sector.SharedContext.Tag}");
                            foreach (var item in items)
                                sector
                                    .SharedContext
                                    .DataLayer
                                    .Items
                                    .Add(item);
                            
                        }
                            

                        


                    }
                }                
            }
            

            return entity;
        }
    }
}
