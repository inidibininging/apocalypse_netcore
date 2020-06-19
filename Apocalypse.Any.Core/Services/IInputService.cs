using System.Collections.Generic;

namespace Apocalypse.Any.Core.Services
{
    public interface IInputService : IUpdateableLite
    {
        IEnumerable<string> InputBefore { get; }
        IEnumerable<string> InputNow { get; }

        IEnumerable<string> GetInput();
    }
}