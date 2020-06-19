using States.Core.Common;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace States.Core.Commands
{
    public class CommandDatabaseDelegate : StateMachine<string, byte>
    {
        public CommandDatabaseDelegate(
            IStateGetService<string, byte> getService, 
            IStateSetService<string, byte> setService, 
            IStateNewService<string, byte> newService) : 
            base(getService, 
                setService, 
                newService)
        {

        }
    }
}
