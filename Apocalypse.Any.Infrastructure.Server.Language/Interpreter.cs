using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Apocalypse.Any.Domain.Common.Model.Language;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Common;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Infrastructure.Server.Language
{
    public class Interpreter : IStateMachine<string,Tokenizer>
    {
        public static MemoryStream GenerateStreamFromString(string value)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(value ?? ""));
        }
        public string RunOperation { get; set; }
        public Interpreter(string runOperation)
        {
            RunOperation = runOperation;
        }
        public IStateMachine<string, IGameSectorLayerService> Context { get; set; }

        public IStateGetService<string, Tokenizer> GetService => throw new NotImplementedException();

        public IStateSetService<string, Tokenizer> SetService => throw new NotImplementedException();

        public IStateNewService<string, Tokenizer> NewService => throw new NotImplementedException();

        public string SharedIdentifier { get; private set;}

        public Tokenizer SharedContext { get; set; }

        public void Run(string key)
        {
            SharedIdentifier = key;
            Console.WriteLine($"running {key}");
            using(var codeStream = Interpreter.GenerateStreamFromString(key)){
                codeStream.Seek(0, SeekOrigin.Begin);
                using(var coreReader =  new StreamReader(codeStream))
                {
                    SharedContext  = new Tokenizer(coreReader);
                    while(SharedContext.MoveNext() &&
                        SharedContext.Current != LexiconSymbol.NA)
                    {
                        // Console.WriteLine(Enum.GetName(typeof(LexiconSymbol), SharedContext.Current));
                        if(SharedContext.Current == LexiconSymbol.NotFound)
                            continue;

                        HandleInstruction<IfExpression, IfInstruction>((s) => s == LexiconSymbol.If);
                        HandleInstruction<EndIfExpression, EndIfInstruction>((s) => s == LexiconSymbol.EndIf);
                        HandleInstruction<CreateExpression, CreateInstruction>((s) => s == LexiconSymbol.CreateWithFactory);
                        HandleInstruction<ApplyMechanicExpression, ApplyMechanicInstruction>((s) => s == LexiconSymbol.ApplyMechanic);
                        HandleInstruction<DestroyExpression, DestroyInstruction>((s) => s == LexiconSymbol.DestroyTag);
                        HandleInstruction<ExecuteExpression, ExecuteInstruction>((s) => s == LexiconSymbol.Execute); //TODO: Fix this 
                        HandleInstruction<FunctionExpression, FunctionInstruction>((s) => s == LexiconSymbol.FunctionIdentifier); //TODO: Fix this
                        HandleInstruction<AssignExpression, AssignInstruction>((s) => s == LexiconSymbol.Set);
                        HandleInstruction<WaitExpression, WaitInstruction>((s) => s == LexiconSymbol.Wait);
                        HandleInstruction<ModifyAttributeExpression, ModifyInstruction>((s) => s == LexiconSymbol.Modify);
                    }
                }
            }
        }

        private void HandleInstruction<TExpr, TInstr>(Func<LexiconSymbol, bool> symbolPredicate)
            where TExpr : AbstractLanguageExpression, new()
            where TInstr : AbstractInterpreterInstruction<TExpr>
        {
            if (!symbolPredicate(SharedContext.Current)) 
                return;
            var expr = new TExpr();
            expr.Handle(this);
            //TODO: switch argument ordering here!!!!
            Activator.CreateInstance(typeof(TInstr), this, expr, Instructions.Count);
        }
        
    


        public List<IAbstractInterpreterInstruction<IAbstractLanguageExpression>> Instructions { get; set; } = new List<IAbstractInterpreterInstruction<IAbstractLanguageExpression>>();

        public IReadOnlyDictionary<string, TimeSpan> TimeLog => throw new NotImplementedException();
    }
}
