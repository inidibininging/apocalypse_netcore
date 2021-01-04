using Apocalypse.Any.Core.Input;
using Apocalypse.Any.Domain.Common.Model.RPG;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Common.Services.Data
{
    /// <summary>
    /// Converts properties to a byte identifer
    /// </summary>
    public class DefaultPropertyToByteIdentifierTranslator<T> : IInputTranslator<string, byte>
    {
        private Dictionary<string, byte> DefaultList { get; set; } = new Dictionary<string, byte>();
        public DefaultPropertyToByteIdentifierTranslator()
        {
            InitializeList();
        }
        private void InitializeList()
        {
            
            byte identifier = 0;
            foreach(var property in typeof(T).GetProperties())
            {
                DefaultList.Add(property.Name, identifier);
                identifier++;
            }
        }
        public byte Translate(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentNullException($"String provided cannot be null: {nameof(input)}");
            if (!DefaultList.ContainsKey(input))
                throw new ArgumentNullException($"No byte identifier registered for {nameof(input)} as {input}");
            return DefaultList[input];
        }
    }
}
