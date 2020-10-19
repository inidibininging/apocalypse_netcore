using Apocalypse.Any.Domain.Common.Model.Language;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Common;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Server.Language
{
    public class ApplyMechanicInstruction : AbstractInterpreterInstruction<ApplyMechanicExpression>
    {
        private string Id { get; set; }
        public string Tag { get; private set; }
        public ApplyMechanicInstruction(Interpreter interpreter, ApplyMechanicExpression expression, int functionIndex) 
            : base(interpreter, expression, functionIndex)
        {

        }

        public override void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            if (string.IsNullOrWhiteSpace(Tag))
            {
                var argumentAsVariable = Expression.Identifier.Name;
                if (Expression.Identifier is IdentifierExpression)
                {
                    Tag = GetVariable(machine).Value;
                }
                Id = machine.NewService.New(Expression.Mechanic.Name, this);

                var runOperation = machine.GetService.Get(Owner.RunOperation) as RoutineState<string, IGameSectorLayerService>;
                if(runOperation == null)
                {                    
                    return;
                }
                if (!runOperation.Operations.Contains(Id))
                {
                    runOperation.Operations = runOperation.Operations.Append(Id).ToList();
                }
                
            }


            if (machine.SharedContext.SingularMechanics.EnemyMechanics.ContainsKey(Expression.Mechanic.Name))
            {
                var entities = machine.SharedContext.DataLayer.Enemies.Where(entity => entity.Tags.Contains(Expression.Mechanic.Name));
                foreach (var e in entities)
                    machine.SharedContext.SingularMechanics.EnemyMechanics[Expression.Mechanic.Name].Update(e);
            }
            if (machine.SharedContext.SingularMechanics.PlayerMechanics.ContainsKey(Expression.Mechanic.Name))
            {
                var entities = machine.SharedContext.DataLayer.Players.Where(entity => entity.Tags.Contains(Expression.Mechanic.Name));
                foreach (var e in entities)
                    machine.SharedContext.SingularMechanics.PlayerMechanics[Expression.Mechanic.Name].Update(e);
            }
            if (machine.SharedContext.SingularMechanics.ItemMechanics.ContainsKey(Expression.Mechanic.Name))
            {
                var entities = machine.SharedContext.DataLayer.Items.Where(entity => entity.Tags.Contains(Expression.Mechanic.Name));
                foreach (var e in entities)
                    machine.SharedContext.SingularMechanics.ItemMechanics[Expression.Mechanic.Name].Update(e);
            }
            if (machine.SharedContext.SingularMechanics.PropsMechanics.ContainsKey(Expression.Mechanic.Name))
            {
                var entities = machine.SharedContext.DataLayer.GeneralCharacter.Where(entity => entity.Tags.Contains(Expression.Mechanic.Name));
                foreach (var e in entities)
                    machine.SharedContext.SingularMechanics.PropsMechanics[Expression.Mechanic.Name].Update(e);
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
            if (variable.DataTypeSymbol != LexiconSymbol.TagDataType)
                throw new InvalidOperationException($"Syntax error. Cannot execute a modify instruction. Data type of variable is not a tag.");
            return variable;
        }
    }
}
