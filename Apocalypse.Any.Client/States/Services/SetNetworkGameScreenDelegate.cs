using Apocalypse.Any.Client.Screens;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;

namespace Apocalypse.Any.Client.States.Services
{
    public class SetNetworkGameScreenDelegate : IStateSetService<string, INetworkGameScreen>
    {
        private Func<Dictionary<string, IState<string, INetworkGameScreen>>> GetDelegate { get; set; }

        public SetNetworkGameScreenDelegate(Func<Dictionary<string, IState<string, INetworkGameScreen>>> getDelegate)
        {
            if (getDelegate == null)
                throw new ArgumentNullException(nameof(getDelegate));
            GetDelegate = getDelegate;
        }

        public bool Set(IState<string, INetworkGameScreen> state, string identifier)
        {
            var dictionary = GetDelegate();
            dictionary.Add(identifier, state);
            return true;
        }
    }
}