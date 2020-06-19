using System;
using System.Collections.Generic;

namespace Apocalypse.Any.Core
{
    public interface IInputArgs
    {
        DateTime RecordedOn { get; }
        List<string> PressedKeys { get; }
        //MouseState MouseStates { get; }
    }
}