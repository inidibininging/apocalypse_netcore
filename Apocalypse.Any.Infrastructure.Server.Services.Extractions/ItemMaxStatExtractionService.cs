using Apocalypse.Any.Domain.Common.Model;
using System;
using System.Linq;

namespace Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces.Data
{
    public class ItemMaxStatExtractionService
    {
        public Tuple<string, int> Extract(Item item)
        {
            return
            item
            .Stats
            .GetType()
            .GetProperties()
            .ToList()
            .Select(prop =>
            {
                var statVal = 0;
                try
                {
                    statVal = (int)prop.GetValue(item.Stats);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ups:{ex.Message}");
                }
                return new System.Tuple<string, int>
                (
                    prop.Name,
                    statVal
                );
            })
            .OrderByDescending(t => t.Item2)
            .First();
        }
    }
}