using Apocalypse.Any.Domain.Common.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Server.Services.Data
{
    public class PlayerDialogItemEventService : GenericInMemoryDataLayer<PlayerDialogEvent<string, Item>>
    {
        public PlayerDialogItemEventService(bool onlyUniques = false) : base(onlyUniques)
        {

        }

    }
}
