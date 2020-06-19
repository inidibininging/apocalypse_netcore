using System;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Infrastructure.Server.Language
{
    public abstract class VariableExpression : TerminalExpression
    {
        public string Name { get; set; }
        protected VariableExpression()
        {
            Symbol = "#";
        }
    }
}
