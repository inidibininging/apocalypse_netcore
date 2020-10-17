using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Common.Model.Language;
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
        public CreateInstruction(Interpreter interpreter, CreateExpression createExpression, int functionIndex)
            : base(interpreter, createExpression, functionIndex)
        {
            Console.WriteLine("adding create instruction");
        }
        

        public override void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {

            var argumentAsVariable = Expression.Identifier.Name;
            if (Expression.Identifier is IdentifierExpression)
            {                
                argumentAsVariable = GetVariable(machine).Value;
            }

            if (machine.SharedContext.Factories.EnemyFactory.ContainsKey(Expression.Creator.Name))
            {
                var entity = machine.SharedContext.Factories.EnemyFactory[Expression.Creator.Name].Create(argumentAsVariable);
                machine.SharedContext.DataLayer.Enemies.Add(entity);
                return;
            }
            if (machine.SharedContext.Factories.GeneralCharacterFactory.ContainsKey(Expression.Creator.Name))
            {
                var entity = machine.SharedContext.Factories.GeneralCharacterFactory[Expression.Creator.Name].Create(argumentAsVariable);
                machine.SharedContext.DataLayer.GeneralCharacter.Add(entity);
                return;
            }
            if (machine.SharedContext.Factories.ItemFactory.ContainsKey(Expression.Creator.Name))
            {
                var entity = machine.SharedContext.Factories.ItemFactory[Expression.Creator.Name].Create(argumentAsVariable);
                machine.SharedContext.DataLayer.Items.Add(entity);
                return;
            }
        }

        private TagVariable GetVariable(IStateMachine<string, IGameSectorLayerService> machine)
        {
            //get the variable out of the function scope 
            var variable = machine
                .SharedContext
                .DataLayer
                .GetLayersByType<TagVariable>()
                .FirstOrDefault()
                ?.DataAsEnumerable<TagVariable>()
                .FirstOrDefault(t => t.Name == Expression.Identifier.Name &&
                                     t.Scope == Scope?.Expression.Name);

            var lastFn = Scope;
            var identifierName = Expression.Identifier.Name;
            while (variable == null)
            {
                var argumentIndex = lastFn.GetFunctionArgumentIndex(identifierName);
                variable = lastFn
                            .LastCaller
                            .GetVariableOfFunction(machine, argumentIndex);
                if (variable != null)
                    continue;
                identifierName = lastFn
                    .LastCaller
                    .Expression
                    .Arguments
                    .Arguments
                    .ElementAt(argumentIndex)
                    .Name;
                lastFn = lastFn.LastCaller.Scope;

            }
            if(variable.DataTypeSymbol != LexiconSymbol.TagDataType)
                throw new InvalidOperationException($"Syntax error. Cannot execute a modify instruction. Data type of variable is not a tag.");
            return variable;
        }
    }
}
