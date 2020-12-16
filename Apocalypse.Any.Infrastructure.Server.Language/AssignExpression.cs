using System;
using System.Linq;
using System.Text;
using Apocalypse.Any.Domain.Common.Model.Language;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Infrastructure.Server.Language
{
    public class AssignExpression : VariableExpression
    {
        public DataTypeExpression DataType { get; set; }
        public VariableExpression Left { get; private set; }
        public VariableExpression Right { get; private set; }
        public override void Handle(IStateMachine<string, Tokenizer> machine)
        {
            while(
                  machine.SharedContext.Current == LexiconSymbol.Set || 
                  machine.SharedContext.Current == LexiconSymbol.Letter || 
                  machine.SharedContext.Current == LexiconSymbol.Identifier || 
                  machine.SharedContext.Current == LexiconSymbol.Assign ||
                  machine.SharedContext.Current == LexiconSymbol.TagIdentifier ||
                  machine.SharedContext.Current == LexiconSymbol.TagLetter ||
                  machine.SharedContext.Current == LexiconSymbol.TagDataType ||
                  machine.SharedContext.Current == LexiconSymbol.NumberDataType ||
                  machine.SharedContext.Current == LexiconSymbol.SkipMaterial ){

                //This is for now the way I can verify if DataType is not set
                if (DataType == null) {
                    DataType = new DataTypeExpression();
                    DataType.Handle(machine);
                }

                if (machine.SharedContext.Current == LexiconSymbol.Letter && Left == null)
                {
                    Left = new IdentifierExpression();
                    Left.Handle(machine);
                }
                
                if (machine.SharedContext.Current == LexiconSymbol.TagIdentifier && Left != null)
                {
                    Right = new TagExpression();
                    Right.Handle(machine);
                }
                
                if (machine.SharedContext.Current == LexiconSymbol.Letter && Left != null)
                {
                    Right = new IdentifierExpression();
                    Right.Handle(machine);
                }

                if(!machine.SharedContext.MoveNext() || (Left != null && Right != null))
                    break;
            }
            if (Left == null)
                throw new InvalidOperationException($"Syntax error: ${nameof(Left)} side is not implemented near {machine.SharedContext.CurrentBuffer}");
            if (Right == null)
                throw new InvalidOperationException($"Syntax error: ${nameof(Right)} side is not implemented near {machine.SharedContext.CurrentBuffer}");
            if (DataType == null)
                throw new InvalidOperationException($"Syntax error: ${nameof(DataType)} side is not implemented near {machine.SharedContext.CurrentBuffer}");
        }
    }
}