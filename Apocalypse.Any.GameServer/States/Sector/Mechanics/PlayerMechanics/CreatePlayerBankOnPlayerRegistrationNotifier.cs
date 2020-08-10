using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Common.Model.PubSub;
using Apocalypse.Any.GameServer.Domain;
using Apocalypse.Any.Infrastructure.Server.Services.Data;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Net.Mime;
using System.Text;

namespace Apocalypse.Any.GameServer.States.Sector.Mechanics.PlayerMechanics
{
    class CreatePlayerBankOnPlayerRegistrationNotifier : IIdentifiableNotifiableModel
    {
        public string BankName { get; }
        private Func<IGameSectorLayerService> GetGameSector { get; set; }
        public string Id { get => EventName; set => EventName = value; }
        public string EventName { get; set; }
        public CreatePlayerBankOnPlayerRegistrationNotifier(string eventName, string bankName,Func<IGameSectorLayerService> getGameSector)
        {
            EventName = !string.IsNullOrWhiteSpace(eventName) ? eventName : throw new ArgumentNullException(nameof(eventName));
            BankName = !string.IsNullOrWhiteSpace(bankName) ? bankName : throw new ArgumentNullException(nameof(bankName)); ;
            GetGameSector = getGameSector ?? throw new ArgumentNullException(nameof(getGameSector));
        }

        public void Notify(string topic, EventQueueArgument content)
        {
            if (topic != EventName)
                return;
            var dataLayer = GetGameSector()
                                .DataLayer;
            var playerIdsFound = dataLayer
                                .Layers
                                .SelectMany(l => l.DataAsEnumerable<DynamicRelation>().Where(dr => dr.Id == content.DynamicRelationId))
                                .Select(relation => 
                                            !string.IsNullOrWhiteSpace(relation.Entity1Id) && 
                                            relation.Entity1 == typeof(PlayerSpaceship) ? relation.Entity1Id :
                                            !string.IsNullOrWhiteSpace(relation.Entity2Id) &&
                                            relation.Entity2 == typeof(PlayerSpaceship) ? relation.Entity2Id : 
                                            null)
                                .Where(pId => pId != null);

            if (!playerIdsFound.Any())
                throw new InvalidOperationException($"{nameof(CreatePlayerBankOnPlayerRegistrationNotifier)} cannot be used. No player ids found. This can be caused if there is another event consumer or the player left the game sector");


            foreach(var playerId in playerIdsFound)
            {
                //get the bank layers and look which belong to the player
                foreach (var bankLayer in dataLayer
                                            .Layers
                                            .Where(l => l.GetValidTypes().Any(t => t == typeof(IntBank))))
                {
                    var bank = bankLayer
                                .DataAsEnumerable<IntBank>()
                                .FirstOrDefault(b => b.OwnerName == playerId);
                    if (bank == null)
                    {

                    }
                }
            }
           

            //var playerRelatedRelations = eventQueues
            //                                .SelectMany(l => l.DataAsEnumerable<DynamicRelation>())
            //                                .DefaultIfEmpty()
            //                                .Where(dr => dr != null &&
            //                                             (dr.Entity1 == typeof(PlayerSpaceship) ||
            //                                              dr.Entity2 == typeof(PlayerSpaceship)));

            
            //eventQueues
            //    .DataAsEnumerable<DynamicRelation>()
            //    .Where()
        }
    }
}
