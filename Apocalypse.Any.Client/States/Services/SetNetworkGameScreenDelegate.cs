using System;
using System.Collections.Generic;
using Apocalypse.Any.Client.Screens;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Client.States.Services
{
    public class SetNetworkGameScreenDelegate : IStateSetService<string, INetworkGameScreen>
    {
        public SetNetworkGameScreenDelegate(Func<Dictionary<string, IState<string, INetworkGameScreen>>> getDelegate)
        {
            if (getDelegate == null)
                throw new ArgumentNullException(nameof(getDelegate));
            GetDelegate = getDelegate;
        }

        private Func<Dictionary<string, IState<string, INetworkGameScreen>>> GetDelegate { get; }

        public bool Set(IState<string, INetworkGameScreen> state, string identifier)
        {
            var dictionary = GetDelegate();
            dictionary.Add(identifier, state);
            return true;
        }
    }
}