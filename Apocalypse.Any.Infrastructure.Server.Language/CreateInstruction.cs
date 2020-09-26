using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Server.Language
{
    public class CreateInstruction : AbstractInterpreterInstruction<CreateExpression>
    {
        public CreateInstruction(Interpreter interpreter, CreateExpression createExpression, int functionIndex) : base(interpreter, functionIndex, createExpression)
        {
            Console.WriteLine("adding create instruction");
        }

        public override void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {

            var creatorName = Expression.Creator.Name;
            if (machine.SharedContext.Factories.EnemyFactory.ContainsKey(Expression.Creator.Name))
            {
                var entity = machine.SharedContext.Factories.EnemyFactory[creatorName].Create(Expression.Identifier.Name);
                machine.SharedContext.DataLayer.Enemies.Add(entity);
                return;
            }
            if (machine.SharedContext.Factories.GeneralCharacterFactory.ContainsKey(Expression.Creator.Name))
            {
                var entity = machine.SharedContext.Factories.GeneralCharacterFactory[creatorName].Create(Expression.Identifier.Name);
                machine.SharedContext.DataLayer.GeneralCharacter.Add(entity);
                return;
            }
            if(machine.SharedContext.Factories.ItemFactory.ContainsKey(Expression.Creator.Name))
            {
                var entity = machine.SharedContext.Factories.ItemFactory[creatorName].Create(Expression.Identifier.Name);
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
