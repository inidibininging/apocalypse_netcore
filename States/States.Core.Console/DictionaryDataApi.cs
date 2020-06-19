using States.Core.Commands;
using States.Core.Common;
using States.Core.Common.Storage;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace States.Core.Console
{
    public class DictionaryDataApi : InMemoryStateStorageService<string, IState<string,byte>>
    {
        public SharedKeyEnumerable<string> SharedKeyEnum { get; private set; }

        public DictionaryDataApi(IDictionary<string, IState<string, byte>> storageData,IEnumerable<string> code) : base(storageData)
        {
            SharedKeyEnum = new SharedKeyEnumerable<string>(code);
            AddCommands();
        }
        void AddCommands()
        {
            AddTrue();
            AddFalse();
            AddXorAll();
            AddAndAll();
            AddNot();
            AddMov();
            AddEAX();
            AddEBX();
            AddECX();
            AddEDX();
            AddClear();
            AddNewInstance();
            AddAlloc(SharedKeyEnum);
            AddSave();
            AddLoad(SharedKeyEnum);
            AddCopy(SharedKeyEnum);
            AddSaveLast();
            AddPrint();

        }
        private void AddTrue()
        {
            MemoryStorage.Add("true", new CommandStateDelegate<string, byte>(new Func<IStateMachine<string, byte>, byte>((stateMachine) =>
            {
                return (byte)(stateMachine.SharedContext = (byte)(stateMachine.SharedContext | CommandConstants.True));
            })));
        }
        void AddFalse()
        {
            MemoryStorage.Add("false", new CommandStateDelegate<string, byte>(new Func<IStateMachine<string, byte>, byte>((stateMachine) =>
            {
                return (byte)(stateMachine.SharedContext = (byte)(stateMachine.SharedContext | CommandConstants.False));
            })));
        }
        void AddAll()
        {
            MemoryStorage.Add("all", new CommandStateDelegate<string, byte>(new Func<IStateMachine<string, byte>, byte>((stateMachine) =>
            {
                return (byte)(stateMachine.SharedContext = (byte)(CommandConstants.All));
            })));
        }
        void AddXorAll()
        {
            MemoryStorage.Add("xorall", new CommandStateDelegate<string, byte>(new Func<IStateMachine<string, byte>, byte>((stateMachine) =>
            {
                return (byte)(stateMachine.SharedContext = (byte)(stateMachine.SharedContext ^ CommandConstants.All));
            })));
        }
        void AddAndAll()
        {
            MemoryStorage.Add("andall", new CommandStateDelegate<string, byte>(new Func<IStateMachine<string, byte>, byte>((stateMachine) =>
            {
                return (byte)(stateMachine.SharedContext = (byte)(stateMachine.SharedContext & CommandConstants.All));
            })));
        }
        void AddNot()
        {
            MemoryStorage.Add("not", new CommandStateDelegate<string, byte>(new Func<IStateMachine<string, byte>, byte>((stateMachine) =>
            {
                return (byte)(~stateMachine.SharedContext);
            })));
        }
        void AddClear()
        {
            var clear = new CommandStateDelegate<string, byte>(new Func<IStateMachine<string, byte>, byte>((stateMachine) =>
            {
                return (byte)(CommandConstants.None);
            }));
            MemoryStorage.Add(nameof(clear), clear);
        }

        void AddEAX()
        {
            MemoryStorage.Add("eax", new CommandStateDelegate<string, byte>(new Func<IStateMachine<string, byte>, byte>((stateMachine) =>
            {
                return (byte)(stateMachine.SharedContext = (byte)(stateMachine.SharedContext | CommandConstants.EAX));
            })));
        }
        void AddEBX()
        {
            MemoryStorage.Add("ebx", new CommandStateDelegate<string, byte>(new Func<IStateMachine<string, byte>, byte>((stateMachine) =>
            {
                return (byte)(stateMachine.SharedContext = (byte)(stateMachine.SharedContext | CommandConstants.EBX));
            })));
        }
        void AddECX()
        {
            MemoryStorage.Add("ecx", new CommandStateDelegate<string, byte>(new Func<IStateMachine<string, byte>, byte>((stateMachine) =>
            {
                return (byte)(stateMachine.SharedContext = (byte)(stateMachine.SharedContext | CommandConstants.ECX));
            })));
        }
        void AddEDX()
        {
            MemoryStorage.Add("edx", new CommandStateDelegate<string, byte>(new Func<IStateMachine<string, byte>, byte>((stateMachine) =>
            {
                return (byte)(stateMachine.SharedContext = (byte)(stateMachine.SharedContext | CommandConstants.EDX));
            })));
        }
        void AddMov()
        {
            var mov = new CommandStateDelegate<string, byte>(new Func<IStateMachine<string, byte>, byte>((stateMachine) =>
            {
                return (byte)(stateMachine.SharedContext = (byte)(stateMachine.SharedContext | CommandConstants.Move));
            }));
            MemoryStorage.Add(nameof(mov), mov);
        }

        void AddInterrupt()
        {
            var interrupt = new CommandStateDelegate<string, byte>(new Func<IStateMachine<string, byte>, byte>((stateMachine) =>
            {
                return (byte)(stateMachine.SharedContext = (byte)(stateMachine.SharedContext | CommandConstants.Interrupt));
            }));
            MemoryStorage.Add(nameof(interrupt), interrupt);
        }
        void AddSaveLast()
        {
            MemoryStorage.Add("save_to_last_new", new CommandStateDelegate<string, byte>((stateMachine) =>
            {
                var currentContext = stateMachine.SharedContext;
                var tkey = (MemoryStorage["newInstance"] as AllocateSharedContextStateDirty<SavedSharedContextState<string, byte>, string, byte>).NewKey;
                stateMachine.GetService.Get(tkey).Handle(stateMachine);
                return currentContext;
            }));
        }

        void AddCopy(SharedKeyEnumerable<string> codeEnum)
        {
            var copy = new CopyStateDirty<SavedSharedContextState<string, byte>, string, byte>();
            copy.SourceStream = codeEnum;
            MemoryStorage.Add(nameof(copy), copy);
        }

        void AddLoad(SharedKeyEnumerable<string> codeEnum)
        {
            var load = new LoadStateDirty<SavedSharedContextState<string, byte>, string, byte>();
            load.SourceStream = codeEnum;
            MemoryStorage.Add(nameof(load), load);
        }

        void AddSave()
        {
            var save = new SavedSharedContextState<string, byte>();
            MemoryStorage.Add(nameof(save), save);
        }

        void AddAlloc(SharedKeyEnumerable<string> codeEnum)
        {
            var alloc = new AllocateNamedSharedContextStateDirty<SavedSharedContextState<string, byte>, string, byte>();
            alloc.SourceStream = codeEnum;
            MemoryStorage.Add(nameof(alloc), alloc);
        }

        void AddNewInstance()
        {
            var newInstance = new AllocateSharedContextStateDirty<SavedSharedContextState<string, byte>, string, byte>();
            MemoryStorage.Add(nameof(newInstance), newInstance);
        }

        
        void AddPrint()
        {
            MemoryStorage.Add("print", new CommandStateDelegate<string, byte>((stateMachine) =>
            {
                var currentContext = stateMachine.SharedContext;
                System.Console.WriteLine(Convert.ToString(currentContext, 2));
                return currentContext;
            }));
            
        }
    }
}
