using System;
using System.IO;
using System.Text;
using Apocalypse.Any.Domain.Common.Model.Language;
using Apocalypse.Any.Infrastructure.Server.Language;

namespace Apocalypse.Any.Language.CLI
{
    static class Program
    {
        public static void Main(string[] args)
        {
            while(Console.ReadKey().Key != ConsoleKey.Escape){
                var code = Console.ReadLine();
                Eval(code);
            }
        }
        public static void Eval(string code){
            var codeStream = Interpreter.GenerateStreamFromString(code);
            codeStream.Seek(0, SeekOrigin.Begin);
            var coreReader =  new StreamReader(codeStream);

            var tokenizer = new Tokenizer(coreReader);
            while(tokenizer.Current != LexiconSymbol.NA)
            {
                if (tokenizer.MoveNext())
                {
                    if(tokenizer.Current == LexiconSymbol.NotFound)
                        continue;
                    Console.WriteLine(Enum.GetName(typeof(LexiconSymbol), tokenizer.Current));
                }
                else
                {
                    break;
                }
            }
            tokenizer.Dispose();
        }
    }
}
