using Apocalypse.Any.Domain.Common.Model.Language;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Server.Language
{
    public class DataTypeExpression : TerminalExpression
    {
        public LexiconSymbol DataType { get; set; }
        public override void Handle(IStateMachine<string, Tokenizer> machine)
        {
            while (DataType == LexiconSymbol._0)
            {
                
                if (machine.SharedContext.Current == LexiconSymbol.TagDataType ||
                    machine.SharedContext.Current == LexiconSymbol.NumberDataType)
                    DataType = machine.SharedContext.Current;
                if (!machine.SharedContext.MoveNext())
                    break;
            }
            if (DataType == LexiconSymbol._0)
                throw new InvalidOperationException($"Syntax error: DataType cannot be determined near '{machine.SharedContext.CurrentBuffer}'");
        }
    }
}
