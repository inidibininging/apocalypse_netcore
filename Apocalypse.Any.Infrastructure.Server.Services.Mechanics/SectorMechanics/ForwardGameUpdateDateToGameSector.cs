using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using System;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics.SectorMechanics
{
    public class ForwardGameUpdateDateToGameSector : IGenericTypeFactory<GameStateUpdateData>
    {
        public bool CanUse<TParam>(TParam parameter)
        {
            throw new NotImplementedException();
        }

        GameStateUpdateData IGenericTypeFactory<GameStateUpdateData>.Create<TParam>(TParam parameter)
        {
            throw new NotImplementedException();
        }
    }
}