using Apocalypse.Any.Domain.Server.Model;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Server.States.Interfaces;
using System;
using System.Collections.Generic;

namespace Apocalypse.Any.Infrastructure.Server.Services.Factories.Network
{
    public class DefaultNetworkLayerStateFactory<TWorld> : CheckWithReflectionFactoryBase<byte>
        where TWorld : IWorldGameStateDataIOLayer, IWorldGameSectorInputLayer
    {
        private Dictionary<byte, INetworkLayerState<TWorld>> DefaultStates { get; set; }

        public override bool CanUse<TParam>(TParam instance)
        {
            throw new NotImplementedException();
        }

        public override List<Type> GetValidParameterTypes()
        {
            throw new NotImplementedException();
        }

        //private void LoadDefaultGameStates()
        //{
        //    InternalGameStates.GetOrAdd((byte)ServerInternalGameStates.InitialGameState, new InitialGameState(new NetworkCommandToInitialGameState(WorldGameStateDataLayer)));
        //    InternalGameStates.GetOrAdd((byte)ServerInternalGameStates.LoginGameState, new LoginGameState(new NetworkCommandToLoginGameState(LoginService)));
        //    InternalGameStates.GetOrAdd((byte)ServerInternalGameStates.UpdateGameState, new UpdateGameState(new NetworkCommandToUpdateGameState(WorldGameStateDataLayer)));
        //    InternalGameStates.GetOrAdd((byte)ServerInternalGameStates.ErrorGameState, new ErrorGameState());
        //}

        protected override byte UseConverter<TParam>(TParam parameter)
        {
            throw new NotImplementedException();
        }
    }
}