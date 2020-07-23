using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

                        HandleExecute();
                        HandleFunction();
                        HandleWait();
                        HandleCreate();
                        HandleDestroy();
                        HandleMod();
                    }
                }
            }
        }

        private void HandleCreate()
        {
            if (SharedContext.Current == LexiconSymbol.Create)
            {
                // Console.WriteLine("Function found");
                var create = new CreateExpression();
                create.Handle(this);
                lock (Instructions)
                {
                    Instructions.Add(
                        new CreateInstruction(this, create)
                    );
                }
            }
        }

        private void HandleDestroy()
        {
            if (SharedContext.Current == LexiconSymbol.Destroy)
            {
                // Console.WriteLine("Function found");
                var destroy = new DestroyExpression();
                destroy.Handle(this);
                lock (Instructions)
                {
                    Instructions.Add(
                        new DestroyInstruction(this, destroy)
                    );
                }
            }
        }


        public List<AbstractInterpreterInstruction> Instructions { get; set; } = new List<AbstractInterpreterInstruction>();

        public IReadOnlyDictionary<string, TimeSpan> TimeLog => throw new NotImplementedException();

        private void HandleFunction()
        {
            if(SharedContext.Current == LexiconSymbol.FunctionIdentifier)
            {
                // Console.WriteLine("Function found");
                var function = new FunctionExpression();
                function.Handle(this);
                lock(Instructions)
                {
                    Instructions.Add(
                        new FunctionInstruction(this, function, Instructions.Count)
                    );
                }
            }
        }

        private void HandleExecute()
        {
            if(SharedContext.Current == LexiconSymbol.Execute)
            {
                // Console.WriteLine("execute found");
                var execute = new ExecuteExpression();
                execute.Handle(this);
                lock(Instructions)
                {
                    Instructions.Add(
                        new ExecuteInstruction(this, execute, Instructions.Count)
                    );
                }
            }
        }

        private void HandleWait()
        {
            if(SharedContext.Current == LexiconSymbol.Wait)
            {
                // Console.WriteLine("Wait found");
                var wait = new WaitExpression();
                wait.Handle(this);
                lock(Instructions)
                {
                    Instructions.Add(
                        new WaitInstruction(this, wait, Instructions.Count)
                    );
                }
            }
        }
        private void HandleMod()
        {
            if(SharedContext.Current == LexiconSymbol.Modify)
            {
                // Console.WriteLine("Mod found");
                var mod = new ModifyAttributeExpression();
                mod.Handle(this);
                // Console.WriteLine(mod.Identifier.Name);
                // Console.WriteLine(mod.Attribute.Name);
                // Console.WriteLine(mod.SignConverter.Polarity);
                // Console.WriteLine(mod.Number.NumberValue);

                Instructions.Add(new ModifyInstruction(this,mod));
            }
        }
    }
}
