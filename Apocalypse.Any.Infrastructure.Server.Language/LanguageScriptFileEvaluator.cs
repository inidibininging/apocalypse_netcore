using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Server.Language
{
    public class LanguageScriptFileEvaluator
    {
        private string FilePath { get; set; }
        public string MainFunction { get; set; }
        public Interpreter Interpreter { get; set; }
        public LanguageScriptFileEvaluator(string filePath, string startupFunction, string runOperation)
        {
            FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            MainFunction = startupFunction ?? throw new ArgumentNullException(nameof(startupFunction));
            Interpreter = new Interpreter(runOperation);
        }
        public LanguageScriptFileEvaluator Evaluate(IStateMachine<string, IGameSectorLayerService> context)
        {
            if (!System.IO.File.Exists(FilePath))
                throw new FileNotFoundException("No mechanic specified", FilePath);
            var scriptFileContent = File.ReadAllText(FilePath);
            Interpreter.Context = context;
            Interpreter.Run(scriptFileContent);
            Interpreter.Context = null;
            return this;
        }
        public void Run(IStateMachine<string, IGameSectorLayerService> context)
        {
            try
            {
                context.GetService.Get(MainFunction).Handle(context);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Interpreter.Context = context;
                Interpreter.Run(MainFunction);
                Interpreter.Context = null;
            }
        } 
    }

}
