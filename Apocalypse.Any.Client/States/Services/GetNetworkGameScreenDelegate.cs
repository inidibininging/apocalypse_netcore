using Apocalypse.Any.Client.Screens;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;

namespace Apocalypse.Any.Client.States.Services
{
    public class GetNetworkGameScreenDelegate : IStateGetService<string, INetworkGameScreen>
    {
        private Func<Dictionary<string, IState<string, INetworkGameScreen>>> GetDelegate { get; set; }

        public GetNetworkGameScreenDelegate(Func<Dictionary<string, IState<string, INetworkGameScreen>>> getDelegate)
        {
            if (getDelegate == null)
                throw new ArgumentNullException(nameof(getDelegate));
            GetDelegate = getDelegate;
        }

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