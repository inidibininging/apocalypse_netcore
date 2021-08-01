using Apocalypse.Any.Domain.Common.Model.RPG;
using System;
using System.Collections.Generic;

namespace Apocalypse.Any.Domain.Common.Model
{
    /// <summary>
    /// Holds all relevant information for a client
    /// This can be seen as a "session bag"
    /// </summary>
    public class PlayerMetadataBag
    {
        public DateTime TimeStamp { get; set; }
        public int MoneyCount { get; set; }
        public List<Item> Items { get; set; }
        public CharacterSheet Stats { get; set; }
        public string ChosenStat { get; set; }
        public int GameSectorTag { get; set; }
        public string ClientEventName { get; set; }
        public string ServerEventName { get; set; }
        public DialogNode CurrentDialog { get; set; }
        //TODO: make a paginator, with tokens, like googles API, in order to scroll through information
    }
}