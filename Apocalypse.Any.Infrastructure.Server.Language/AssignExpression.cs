using System.Linq;
using System.Text;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Infrastructure.Server.Language
{
    public class AssignExpression : VariableExpression
    {
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
                  machine.SharedContext.Current == LexiconSymbol.SkipMaterial ){

                if (machine.SharedContext.Current == LexiconSymbol.Letter && Left == null)
                {
                    Left = new IdentifierExpression();
                    Left.Handle(machine);
                }
                
                if (machine.SharedContext.Current == LexiconSymbol.TagIdentifier && Left != null)
                {
                    Right = new FactionExpression();
                    Right.Handle(machine);
                }

                if(!machine.SharedContext.MoveNext() || (Left != null && Right != null))
                    break;
            }
        }
    }
}