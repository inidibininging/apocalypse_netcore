using System;
using System.Collections.Generic;
using System.Linq;
using Apocalypse.Any.Domain.Common.Model.Language;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Infrastructure.Server.Language
{
    public class AssignInstruction : AbstractInterpreterInstruction<AssignExpression>
    {
        public AssignInstruction(Interpreter interpreter, AssignExpression assignExpression, int functionIndex) : base(interpreter, functionIndex, assignExpression)
        {
        
        }

        public override void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {

            var assignmentIndex = Owner.Instructions.IndexOf(this as IAbstractInterpreterInstruction<IAbstractLanguageExpression>);
            if(assignmentIndex == -1)
                throw new InvalidOperationException("Assignment instruction not found. Code was modified");

            while ((Owner.Instructions[assignmentIndex] is FunctionInstruction) == false)
            {
                if (assignmentIndex < 0)
                    throw new InvalidOperationException("Function instruction cannot be found. Assignment must be in a function scope. Are you making an assignment inside a function?");
                assignmentIndex--;
            }
            var scopeOfAssignment = (Owner.Instructions[assignmentIndex] as FunctionInstruction)?.Expression.Name;

            var tagLayer = machine
                .SharedContext
                .DataLayer
                .GetLayersByType<TagVariable>()
                .FirstOrDefault();
            
            if(tagLayer == null)
                throw new ArgumentNullException(nameof(tagLayer),"Tags cannot be saved. Please check the data layer for language stored variables");
            
            //create or replace variable
            var variable = tagLayer
                .DataAsEnumerable<TagVariable>()
                .FirstOrDefault(t => t.Name == Expression.Left.Name &&
                                     t.Scope == scopeOfAssignment // no scope for now. scope must be implemented in the language first
                                );
            
            if (variable == null)
            {
                tagLayer.Add(new TagVariable()
                {
                    Name = Expression.Left.Name,
                    Value = Expression.Right.Name,
                    Scope = scopeOfAssignment
                });
                variable = tagLayer.DataAsEnumerable<TagVariable>().FirstOrDefault(t => t.Name == Expression.Left.Name &&
                                                                      t.Scope == scopeOfAssignment);
                if(variable == null)
                    throw new ArgumentNullException(nameof(tagLayer), $"Tag with name {Expression.Left.Name} cannot be created");
            }
            else
            {
                variable.Name = Expression.Left.Name;
                variable.Value = Expression.Right.Name;
            }

            
            base.Handle(machine);
        }
    }
}