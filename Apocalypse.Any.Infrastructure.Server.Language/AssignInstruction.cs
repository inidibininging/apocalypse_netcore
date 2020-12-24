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
        public AssignInstruction(Interpreter interpreter, AssignExpression assignExpression, int functionIndex) 
            : base(interpreter, assignExpression, functionIndex)
        {
        
        }

        private TagVariable GetVariable(string name, IStateMachine<string, IGameSectorLayerService> machine)
        {
            //get the variable out of the function scope 
            var variable = machine
                .SharedContext
                .DataLayer
                .GetLayersByType<TagVariable>()
                .FirstOrDefault()
                ?.DataAsEnumerable<TagVariable>()
                .FirstOrDefault(t => t.Name == name &&
                                     t.Scope == Scope?.Expression.Name);

            var lastFn = Scope;
            var identifierName = name;
            while (variable == null)
            {
                var argumentIndex = lastFn.GetFunctionArgumentIndex(identifierName);
                
                if (argumentIndex < 0)
                    throw new ArgumentNullException(nameof(identifierName),
                        $"Tag with name {identifierName} not found inside {Scope.Expression.Name}");
                
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
            // Console.WriteLine($"Variable:{variable.Name} Current Value:{variable.Value}");
            // Console.WriteLine(System.Environment.NewLine);
            return variable;
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
                /*
                    if the value of tag variable is a tag. cool assign value.
                    if the value of tag variable is a variable itself, trace back variable value                     
                 */
                var tagValueAssigned = string.Empty;
                if (Expression.Right is IdentifierExpression)
                {
                    var tagVariable = GetVariable(Expression.Right.Name, machine);
                    if (tagVariable == null)
                        throw new ArgumentNullException(nameof(tagVariable),
                            $"Tag with name {Expression.Right.Name} not found inside {Scope.Expression.Name}");
                    tagValueAssigned = tagVariable.Value;
                }

                if (Expression.Right is TagExpression)
                {
                    tagValueAssigned = Expression.Right.Name;
                }

                if (string.IsNullOrWhiteSpace(tagValueAssigned))
                    throw new ArgumentNullException(nameof(tagValueAssigned), $"The value of Tag {Expression.Left.Name} is empty inside {Scope.Expression.Name}");
                
                tagLayer.Add(new TagVariable()
                {
                    Name = Expression.Left.Name,
                    Value = tagValueAssigned,
                    Scope = scopeOfAssignment,
                    DataTypeSymbol = Expression.DataType.DataType
                });
                
                variable = tagLayer.DataAsEnumerable<TagVariable>().FirstOrDefault(t => t.Name == Expression.Left.Name &&
                                                                      t.Scope == scopeOfAssignment);
                if(variable == null)
                    throw new ArgumentNullException(nameof(tagLayer), $"Tag with name {Expression.Left.Name} cannot be created inside {Scope.Expression.Name}");
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