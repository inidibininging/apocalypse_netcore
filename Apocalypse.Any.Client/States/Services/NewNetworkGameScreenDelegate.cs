using Apocalypse.Any.Client.Screens;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;

namespace Apocalypse.Any.Client.States.Services
{
    public class NewNetworkGameScreenDelegate : IStateNewService<string, INetworkGameScreen>
    {
        private Func<Dictionary<string, IState<string, INetworkGameScreen>>> GetDelegate { get; set; }

        public NewNetworkGameScreenDelegate(Func<Dictionary<string, IState<string, INetworkGameScreen>>> getDelegate)
        {
            if (getDelegate == null)
                throw new ArgumentNullException(nameof(getDelegate));
            GetDelegate = getDelegate;
        }

        public string New(IState<string, INetworkGameScreen> state)
        {
            var dictionary = GetDelegate();
            var identifier = Guid.NewGuid().ToString();
            dictionary.Add(identifier, state);
            Console.WriteLine($"{identifier} created");
            return identifier;
        }

        public string New(string identifier, IState<string, INetworkGameScreen> state)
        {
            var dictionary = GetDelegate();
            dictionary.Add(identifier, state);
            return identifier;
        }
    }
}