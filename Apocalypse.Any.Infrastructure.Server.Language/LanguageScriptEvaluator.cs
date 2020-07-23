using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Server.Language
{
    public class LanguageScriptEvaluator
    {
        private string Content { get; set; }
        public string MainFunction { get; set; }
        public Interpreter Interpreter { get; set; }
        public LanguageScriptEvaluator(string content, string startupFunction, string runOperation)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));
            MainFunction = startupFunction ?? throw new ArgumentNullException(nameof(startupFunction));
            Interpreter = new Interpreter(runOperation);
        }
        public LanguageScriptEvaluator Evaluate(IStateMachine<string, IGameSectorLayerService> context)
        {
            if (string.IsNullOrWhiteSpace(Content))
                throw new ArgumentNullException("No mechanic specified");
            
            Interpreter.Context = context;
            Interpreter.Run(Content);
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
