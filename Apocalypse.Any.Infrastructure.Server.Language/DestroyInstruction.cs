using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Server.Language
{
    public class DestroyInstruction : AbstractInterpreterInstruction
    {
        public DestroyInstruction(Interpreter interpreter, DestroyExpression createExpression) : base(interpreter)
        {
            Console.WriteLine("adding destroy instruction");
            DestroyExpression = createExpression;
        }
        private DestroyExpression DestroyExpression { get; set; }

        public override void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            if (DestroyExpression == null)
                throw new ArgumentNullException(nameof(DestroyExpression));

            var factionToDestroy = DestroyExpression.Identifier.Name;
            GetAllValidCharacters(machine)
                .Where(character => character.Factions.Contains(factionToDestroy))
                .ToList()
                .ForEach(character => character.Factions.Remove(factionToDestroy));

            //commented out for now. users should not be created through script for now.            
            //if (machine.SharedContext.Factories.PlayerFactory.ContainsKey(CreateExpression.Creator.Name))
            //{
            //    return;
            //}
        }
        private static IEnumerable<CharacterEntity> GetAllValidCharacters(IStateMachine<string, IGameSectorLayerService> machine) =>
           machine.SharedContext.DataLayer.Players.Cast<CharacterEntity>()
           .Concat(machine.SharedContext.DataLayer.Enemies.Cast<CharacterEntity>())
           .Concat(machine.SharedContext.DataLayer.Items.Cast<CharacterEntity>())
           .Concat(machine.SharedContext.DataLayer.GeneralCharacter.Cast<CharacterEntity>());

    }
}
