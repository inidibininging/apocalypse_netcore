using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces
{
    public interface IWorldGameSectorInputLayer
    {
        /// <summary>
        /// Returns a sector by identifier
        /// </summary>
        /// <param name="sectorIdentifier"></param>
        /// <returns></returns>
        IGameSectorLayerService GetSector(string sectorIdentifier);
    }
}
