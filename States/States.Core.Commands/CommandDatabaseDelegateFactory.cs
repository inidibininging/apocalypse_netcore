using States.Core.Infrastructure.Services;
using States.Core.Common;
using System;
using System.Collections.Generic;
using System.Text;
using States.Core.Common.Storage;
using System.IO;

namespace States.Core.Commands
{
    public class CommandDatabaseDelegateFactory
    {
        //public CommandDatabaseDelegate BuildDefault()
        //{
        //    IDictionary<string, IState<string, byte>> inMemoryDatabase = new Dictionary<string, IState<string, byte>>();

            
        //    inMemoryDatabase.Add("True",new CommandStateDelegate<string,byte>(new Func<IStateMachine<string,byte>,byte>((machine)=>
        //    {
        //        return (byte)(machine.SharedContext = (byte)(machine.SharedContext | CommandConstants.True));
        //    })));
        //    inMemoryDatabase.Add("False",new CommandStateDelegate<string,byte>(new Func<IStateMachine<string,byte>,byte>((machine)=>
        //    {
        //        return (byte)(machine.SharedContext = (byte)(machine.SharedContext | CommandConstants.False));
        //    })));
        //    inMemoryDatabase.Add("All",new CommandStateDelegate<string,byte>(new Func<IStateMachine<string,byte>,byte>((machine)=>
        //    {
        //        return (byte)(machine.SharedContext = (byte)(CommandConstants.All));
        //    })));
        //    inMemoryDatabase.Add("XorAll",new CommandStateDelegate<string,byte>(new Func<IStateMachine<string,byte>,byte>((machine)=>
        //    {
        //        return (byte)(machine.SharedContext = (byte)(machine.SharedContext ^ CommandConstants.All));
        //    })));
        //    inMemoryDatabase.Add("OrAll", new CommandStateDelegate<string, byte>(new Func<IStateMachine<string, byte>, byte>((machine) =>
        //    {
        //        return (byte)(machine.SharedContext = (byte)(machine.SharedContext | CommandConstants.All));
        //    })));
        //    inMemoryDatabase.Add("AndAll", new CommandStateDelegate<string, byte>(new Func<IStateMachine<string, byte>, byte>((machine) =>
        //    {
        //        return (byte)(machine.SharedContext = (byte)(machine.SharedContext & CommandConstants.All));
        //    })));
        //    inMemoryDatabase.Add("Not", new CommandStateDelegate<string, byte>(new Func<IStateMachine<string, byte>, byte>((machine) =>
        //    {
        //        return (byte)(~machine.SharedContext);
        //    })));
        //    inMemoryDatabase.Add("Push",new CommandStateDelegate<string,byte>(new Func<IStateMachine<string,byte>,byte>((machine)=>
        //    {
        //        return (byte)(machine.SharedContext = (byte)(machine.SharedContext | CommandConstants.Push));
        //    })));
        //    inMemoryDatabase.Add("Move",new CommandStateDelegate<string,byte>(new Func<IStateMachine<string,byte>,byte>((machine)=>
        //    {
        //        return (byte)(machine.SharedContext = (byte)(machine.SharedContext | CommandConstants.Move));
        //    })));
        //    inMemoryDatabase.Add("A",new CommandStateDelegate<string,byte>(new Func<IStateMachine<string,byte>,byte>((machine)=>
        //    {
        //        return (byte)(machine.SharedContext = (byte)(machine.SharedContext | CommandConstants.StateA));
        //    })));
        //    inMemoryDatabase.Add("B",new CommandStateDelegate<string,byte>(new Func<IStateMachine<string,byte>,byte>((machine)=>
        //    {
        //        return (byte)(machine.SharedContext = (byte)(machine.SharedContext | CommandConstants.StateB));
        //    })));
        //    inMemoryDatabase.Add("C",new CommandStateDelegate<string,byte>(new Func<IStateMachine<string,byte>,byte>((machine)=>
        //    {
        //        return (byte)(machine.SharedContext = (byte)(machine.SharedContext | CommandConstants.StateC));
        //    })));
        //    inMemoryDatabase.Add("D",new CommandStateDelegate<string,byte>(new Func<IStateMachine<string,byte>,byte>((machine)=>
        //    {
        //        return (byte)(machine.SharedContext = (byte)(machine.SharedContext | CommandConstants.StateD));
        //    })));

            
        //    inMemoryDatabase.Add("Load", new CommandStateDelegate<string, byte>(new Func<IStateMachine<string, byte>, byte>((machine) =>
        //    {
        //        Console.ForegroundColor = ConsoleColor.Yellow;
        //        Console.WriteLine("State:");
        //        var state = Console.ReadLine();
        //        Console.ForegroundColor = ConsoleColor.White;

        //        return (inMemoryDatabase[state] as SaveSharedContext<string, byte>).SharedContext;
        //    })));

        //    inMemoryDatabase.Add("Copy", new CommandStateDelegate<string, byte>(new Func<IStateMachine<string, byte>, byte>((machine) =>
        //    {
        //        var savedContext = machine.SharedContext;

        //        Console.ForegroundColor = ConsoleColor.Yellow;
        //        Console.WriteLine("StateFrom:");
        //        var stateFromName = Console.ReadLine();
        //        Console.ForegroundColor = ConsoleColor.White;


        //        Console.ForegroundColor = ConsoleColor.Yellow;
        //        Console.WriteLine("StateTo:");
        //        var stateToName = Console.ReadLine();
        //        Console.ForegroundColor = ConsoleColor.White;

        //        (inMemoryDatabase[stateToName] as LoadSharedContext<string, byte>).SharedContext = (inMemoryDatabase[stateFromName] as SaveSharedContext<string, byte>).SharedContext;

        //        return savedContext;
        //    })));

        //    inMemoryDatabase.Add("New", new CommandStateDelegate<string, byte>(new Func<IStateMachine<string, byte>, byte>((machine) =>
        //    {
        //        var keyName = machine.NewService.New(new SaveSharedContext<string, byte>());
        //        Console.ForegroundColor = ConsoleColor.Yellow;
        //        Console.WriteLine(keyName);
        //        Console.ForegroundColor = ConsoleColor.White;

        //        return machine.SharedContext;
        //    })));

        //    inMemoryDatabase.Add("Print",new CommandStateDelegate<string,byte>(new Func<IStateMachine<string,byte>,byte>((machine)=>
        //    {
        //        Console.ForegroundColor = ConsoleColor.Yellow;
        //        Console.WriteLine(Convert.ToString(machine.SharedContext, 2).PadLeft(8, '0'));
        //        Console.ForegroundColor = ConsoleColor.White;

        //        return machine.SharedContext;
        //    })));


        //    inMemoryDatabase.Add("Read",new CommandStateDelegate<string,byte>(new Func<IStateMachine<string,byte>,byte>((machine)=>
        //    {
        //        var input = String.Empty;
        //        while(input != "Exit")
        //        {
        //            input = Console.ReadLine();
        //            try
        //            {
        //                machine.GetService.Get(input).Handle(machine);
        //            }
        //            catch(Exception ex)
        //            {
        //                Console.WriteLine(ex.Message.ToString());
        //            }                    
        //        }
        //        return machine.SharedContext;
        //    })));

        //    //commands here plssss
        //    var commandDatabase = new CommandDatabase(inMemoryDatabase);

        //    var getService = new GetStateDelegationService<string, byte>(() => commandDatabase);
        //    var setService = new SetStateDelegationService<string, byte>(() => commandDatabase);
        //    var newService = new NewStateDelegationService<string, byte>(() => commandDatabase);

        //    var commandDatabaseDelegate = new CommandDatabaseDelegate(getService, setService, newService);
            
        //    return commandDatabaseDelegate;
        //}


    }
}
