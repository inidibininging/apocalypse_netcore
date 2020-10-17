using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Data;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apocalypse.Any.GameServer.States.Sector.Mechanics.PlayerMechanics
{
    public class AddDroppedItemsAsCurrencyToPlayersBankState : IState<string, IGameSectorLayerService>
    {
        public AddDroppedItemsAsCurrencyToPlayersBankState(ShortCounterThreshold threshold)
        {
            Threshold = threshold ?? throw new ArgumentNullException(nameof(threshold));
        }

        public ShortCounterThreshold Threshold { get; }

        public void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            SetCooldownTimeToThresholdIfNeeded(machine);

            if (Threshold.CooldownDeadline > machine.SharedContext.CurrentGameTime.ElapsedGameTime)
                return;

            Threshold.CooldownDeadline = TimeSpan.Zero;
            var bankItemsRelations = machine
                                .SharedContext
                                .DataLayer
                                .Layers
                                .OfType<DynamicRelationLayer<Item, IntBank>>()
                                .AsEnumerable<IGenericTypeDataLayer>()
                                .Union(
                                    machine
                                        .SharedContext
                                        .DataLayer
                                        .Layers
                                        .OfType<DynamicRelationLayer<IntBank, Item>>()
                                )
                                .SelectMany(l => l.DataAsEnumerable<DynamicRelation>());

            var banks = machine.SharedContext.DataLayer.GetLayersByType<IntBank>().SelectMany(l => l.DataAsEnumerable<IntBank>()).Where(br => bankItemsRelations
                                                                                                .Select(b => typeof(IntBank) == b.Entity1 ?
                                                                                                                b.Entity1Id :
                                                                                                                b.Entity2Id)
                                                                                                .Contains(br.Id));

            foreach (var g in bankItemsRelations
                                .GroupBy(g => g.Entity1 == typeof(IntBank) ? g.Entity1Id : g.Entity2Id)
                                .Select(g =>
                                (bank: banks.FirstOrDefault(b => b.Id == g.Key),
                                 items: g.Count())))
            {
                AdjustThresholdCounterBasedOnBankItemsCount(g);

                g.bank.Amount += g.items;
            }
        }

        private void SetCooldownTimeToThresholdIfNeeded(IStateMachine<string, IGameSectorLayerService> machine)
        {
            if (Threshold.CooldownDeadline == TimeSpan.Zero)
                Threshold.CooldownDeadline = machine.SharedContext.CurrentGameTime.ElapsedGameTime.Add(TimeSpan.FromSeconds(Threshold.Counter));
        }

        private void AdjustThresholdCounterBasedOnBankItemsCount((IntBank bank, int items) g)
        {
            if (Threshold.Counter < g.items)
                Threshold.Counter = g.items > byte.MaxValue ? byte.MaxValue : Convert.ToByte(g.items+1);
        }
    }
}
