using System;
using System.Collections.Generic;
using System.Linq;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.GameServer.States.Sector
{
    public class SectorCsvLoggerState : IState<string,IGameSectorLayerService>
    {
        private List<List<(string stateName, TimeSpan timeSpan)>> Rows =
            new List<List<(string stateName, TimeSpan timeSpan)>>();

        public int MaxRowsToAppend { get; set; } = 16000;
        public int CurrentRow { get; set; } = 0;
        public int FrameCounter { get; set; } = 0;
        public void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            if (CurrentRow > MaxRowsToAppend)
            {
                using(var sw = new System.IO.StreamWriter($"{machine.SharedContext.Tag}.csv", true))
                {
                    foreach(var row in Rows){
                        foreach (var rowState in row)
                        {
                            sw.WriteLine($"{FrameCounter};{rowState.stateName};{rowState.timeSpan.TotalMilliseconds}");    
                        }
                        FrameCounter++;
                    }
                }
                Rows.Clear();
                CurrentRow = 0;
            }
            else
            {
                Rows.Add(machine.TimeLog.Select(d => (d.Key,d.Value)).ToList());
                CurrentRow++;
            }
            
            
        }
    }
}