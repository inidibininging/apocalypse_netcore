using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Server.Language
{
    public class CreateInstruction : AbstractInterpreterInstruction
    {
        public CreateInstruction(Interpreter interpreter, CreateExpression createExpression) : base(interpreter)
        {
            Console.WriteLine("adding create instruction");
            CreateExpression = createExpression;
        }
        private CreateExpression CreateExpression { get; set; }

        public override void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            if (CreateExpression == null)
                throw new ArgumentNullException(nameof(CreateExpression));

            var creatorName = CreateExpression.Creator.Name;
            if (machine.SharedContext.Factories.EnemyFactory.ContainsKey(CreateExpression.Creator.Name))
            {
                var entity = machine.SharedContext.Factories.EnemyFactory[creatorName].Create(CreateExpression.Identifier.Name);
                machine.SharedContext.DataLayer.Enemies.Add(entity);
                return;
            }
            if (machine.SharedContext.Factories.GeneralCharacterFactory.ContainsKey(CreateExpression.Creator.Name))
            {
                var entity = machine.SharedContext.Factories.GeneralCharacterFactory[creatorName].Create(CreateExpression.Identifier.Name);
                machine.SharedContext.DataLayer.GeneralCharacter.Add(entity);
                return;
            }
            if(machine.SharedContext.Factories.ItemFactory.ContainsKey(CreateExpression.Creator.Name))
            {
                var entity = machine.SharedContext.Factories.ItemFactory[creatorName].Create(CreateExpression.Identifier.Name);
                machine.SharedContext.DataLayer.Items.Add(entity);
                return;
            }
            //commented out for now. users should not be created through script for now.            
            //if (machine.SharedContext.Factories.PlayerFactory.ContainsKey(CreateExpression.Creator.Name))
            //{
            //    return;
            //}
        }

    }
}
