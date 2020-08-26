using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Common.Model.PubSub;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using Apocalypse.Any.Infrastructure.Server.PubSub.Interfaces;
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
        public Func<IGenericTypeFactory<IntBank>> BankFactory { get; }
        private Func<IGameSectorLayerService> GetGameSector { get; set; }
        public string Id { get => EventName; set => EventName = value; }
        public string EventName { get; set; }


        public CreatePlayerBankOnPlayerRegistrationNotifier(string eventName, string bankName, Func<IGenericTypeFactory<IntBank>> bankFactory, Func<IGameSectorLayerService> getGameSector)
        {
            EventName = !string.IsNullOrWhiteSpace(eventName) ? eventName : throw new ArgumentNullException(nameof(eventName));
            BankName = !string.IsNullOrWhiteSpace(bankName) ? bankName : throw new ArgumentNullException(nameof(bankName)); ;
            BankFactory = bankFactory ?? throw new ArgumentNullException(nameof(bankFactory));
            GetGameSector = getGameSector ?? throw new ArgumentNullException(nameof(getGameSector));
        }

        public void Notify(string topic, EventQueueArgument content)
        {
            if (topic != EventName)
                return;
            var dataLayer = GetGameSector()?
                                .DataLayer;

            if (dataLayer == null)
                throw new InvalidOperationException($"Data layer not found. Cannot continue with notification of event {Id} - {nameof(CreatePlayerBankOnPlayerRegistrationNotifier)} for {BankName}");
            
            //var playerIdsFound = dataLayer
            //                    .Layers
            //                    .SelectMany(l => l.DataAsEnumerable<DynamicRelation>().Where(dr => dr.Id == content.RefereneceId))
            //                    .Select(relation => 
            //                                (!string.IsNullOrWhiteSpace(relation.Entity1Id) && 
            //                                relation.Entity1 == typeof(PlayerSpaceship) ? relation.Entity1Id :
            //                                (!string.IsNullOrWhiteSpace(relation.Entity2Id) &&
            //                                relation.Entity2 == typeof(PlayerSpaceship) ? relation.Entity2Id :
            //                                null)));

            if (string.IsNullOrWhiteSpace(content.ReferenceId) || content.ReferenceType != typeof(PlayerSpaceship))
                throw new InvalidOperationException($"{nameof(CreatePlayerBankOnPlayerRegistrationNotifier)} cannot be used. No relations to player id found. This can be caused if there is another event consumer or the player left the game sector");

            var bankFactory = BankFactory() ?? throw new InvalidOperationException($"Cannot continue {nameof(CreatePlayerBankOnPlayerRegistrationNotifier)} for {EventName}. Bank factory delegate returns null");


            //foreach (var playerId in playerIdsFound)
            //{
            //get the bank layers and look which belong to the player
            foreach (var bankLayer in dataLayer
                                            .Layers
                                            .Where(l => l.GetValidTypes().Any(t => t == typeof(IntBank))))
                {
                    var bank = bankLayer
                                .DataAsEnumerable<IntBank>()
                                .FirstOrDefault(b => b.OwnerName == content.ReferenceId);
                    if (bank == null)
                    {
                        var playersBank = bankFactory.Create(content.ReferenceId) ?? throw new InvalidOperationException($"The players bank cannot be created. The bank factory cannot create a bank");
                        bankLayer.Add(playersBank);
                    }
                }
            //}
           

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
