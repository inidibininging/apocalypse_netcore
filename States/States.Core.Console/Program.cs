using States.Core.Commands;
using States.Core.Common;
using States.Core.Common.Delegation;
using States.Core.Common.Storage;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
namespace States.Core.Console
{
    class Program
    {
        private static IDictionary<string, IState<string, byte>> InMemoryDatabase = new Dictionary<string, IState<string, byte>>();

        static void Main(string[] args)
        {
            args.ToList().ForEach(arg => System.Console.WriteLine(arg));
            
            System.Console.WriteLine("-------------------");
            //create some code
            var someCode = new List<string>()
            {
                "clear",

                "mov",
                "eax",
                "true",
                "print",
                
                "alloc",
                "foo",
                "print",

                "clear",

                "mov",
                "ebx",
                "true",

                "alloc",
                "bar",
                "print",

                "clear",

                "load",
                "foo",
                "print",

                "load",
                "bar",
                "print",
                // ---> 
            };

            var dictionaryStringDataApi = new DictionaryDataApi(InMemoryDatabase,someCode);

            var memoryAccessorGet = new GetStateDelegationService<string, byte>(() => dictionaryStringDataApi);
            var memoryAccessorSet = new SetStateDelegationService<string, byte>(() => dictionaryStringDataApi);
            var memoryAccessorNew = new NewStateDelegationService<string, byte>(() => dictionaryStringDataApi);
            var stateMachine = new StateMachine<string, byte>(memoryAccessorGet, memoryAccessorSet, memoryAccessorNew);
            stateMachine.Run(stateMachine, dictionaryStringDataApi.SharedKeyEnum);
            
            
            System.Console.WriteLine("-------------------");
        }


    }
}
