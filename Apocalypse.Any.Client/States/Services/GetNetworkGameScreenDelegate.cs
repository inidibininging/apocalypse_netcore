using System;
using System.Collections.Generic;
using Apocalypse.Any.Client.Screens;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Client.States.Services
{
    public class GetNetworkGameScreenDelegate : IStateGetService<string, INetworkGameScreen>
    {
        public GetNetworkGameScreenDelegate(Func<Dictionary<string, IState<string, INetworkGameScreen>>> getDelegate)
        {
            if (getDelegate == null)
                throw new ArgumentNullException(nameof(getDelegate));
            GetDelegate = getDelegate;
        }

        private Func<Dictionary<string, IState<string, INetworkGameScreen>>> GetDelegate { get; }

        public IState<string, INetworkGameScreen> Get(string identifier)
        {
            return GetDelegate()[identifier];
        }

        public bool HasState(string identifier)
        {
            return GetDelegate().ContainsKey(identifier);
        }
    }
}