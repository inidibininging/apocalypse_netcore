using Echse.Net.Domain;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Apocalypse.Any.Core.Input.Translator
{
    public class GenericMouseDeviceToKeysTranslator :
        IInputTranslator<MouseState, IEnumerable<string>>
    {
        private int scrollWheelValueRaw { get; set; }
        private int scrollWheelValueBefore { get; set; }
        
        public IEnumerable<string> Translate(MouseState input)
        {
            var commandInput = new List<string>();

            if (input.LeftButton == ButtonState.Pressed)
                commandInput.Add(DefaultKeys.Shoot);

            if (input.RightButton == ButtonState.Pressed)
                commandInput.Add(DefaultKeys.AltShoot);

            if (input.MiddleButton == ButtonState.Pressed)
                commandInput.Add(DefaultKeys.Boost);

            //mouse scroll
            scrollWheelValueBefore = scrollWheelValueRaw;
            scrollWheelValueRaw = Mouse.GetState().ScrollWheelValue;

            float delta = scrollWheelValueRaw - scrollWheelValueBefore;
            if (delta == 0) return commandInput;
            
            if (delta > 0)
                commandInput.Add(DefaultKeys.ZoomIn);
            if (delta < 0)
                commandInput.Add(DefaultKeys.ZoomOut);
            
            return commandInput;
        }


    }
}