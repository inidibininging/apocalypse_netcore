using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Core.Services;
using Echse.Net.Domain;
using System;
using System.Collections.Generic;

namespace Apocalypse.Any.Core.Input.Translator
{
    public class ScreenVector2ToKeysTranslator : IInputTranslator<ScreenService, IEnumerable<string>>
    {
        private Func<IFullPositionHolder> GetFullPositionHolder { get; set; }

        public ScreenVector2ToKeysTranslator(Func<IFullPositionHolder> getFullPositionHolder)
        {
            if (getFullPositionHolder == null)
                throw new ArgumentNullException(nameof(getFullPositionHolder));
            GetFullPositionHolder = getFullPositionHolder;
        }

        public IEnumerable<string> Translate(ScreenService input)
        {
            // if( input.Item1.LeftScreenCamera != null || input.Item1.RightScreenCamera != null )
            //     throw new ArgumentException("ScreenVector2ToKeysTranslator cannot emit a command. Left or right screen camera is ");
            List<string> keys = new List<string>();
            var cameraPos = input.DefaultScreenCamera.Position.ToVector2();
            var positionHolder = GetFullPositionHolder();

            if (positionHolder == null)
            {
                Console.WriteLine("positionHolder => null");
                return keys;
            }

            var rotation = positionHolder.Rotation.Rotation;
            Console.WriteLine($"rotation => {rotation}");

            if (positionHolder.Position.X < cameraPos.X)
            {
                keys.Add(DefaultKeys.Left);
                return keys;
            }

            if (positionHolder.Position.X > cameraPos.X)
            {
                keys.Add(DefaultKeys.Right);
                return keys;
            }

            return keys;
        }
    }
}