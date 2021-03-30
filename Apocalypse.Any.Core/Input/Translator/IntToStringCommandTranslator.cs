using System;

namespace Apocalypse.Any.Core.Input.Translator
{
    public class IntToStringCommandTranslator 
        : IInputTranslator<int, string>, IInputTranslator<string, int>
    {
public string Translate(int input)
        {
            // Console.WriteLine($"string to int input to translate:{input}");
            switch (input)
            {
                case 0:
                    return DefaultKeys.Up;
                case 1:
                    return DefaultKeys.Down;
                case 2:
                    return DefaultKeys.Left;
                case 3:
                    return DefaultKeys.Right;
                case 4:
                    return DefaultKeys.Shoot;
                case 5:
                    return DefaultKeys.Boost;
                case 6:
                    return DefaultKeys.AltShoot;
                // case 7:
                //     return DefaultKeys.Up;
                // case 8:
                //     return DefaultKeys.Down;
                // case 9:
                //     return DefaultKeys.Left;
                // case 10:
                //     return DefaultKeys.Right;
                // case 11:
                //     return DefaultKeys.Shoot;
                // case 12:
                //     return DefaultKeys.Boost;
                // case 13:
                //     return DefaultKeys.AltShoot;
                default:
                    return string.Empty;
            }
        }

        public int Translate(string input)
        {
            //Problem. I need to check if the is pressed or not. This should be done by recording the last key pressed and when the input is nothing (-1) -> send a release
            //The behaviour is similar to ProcessPressReleaseState
            //Solved, see CommandPressReleaseTranslator
            // Console.WriteLine($"int to string input to translate:{input}");
            switch (input)
            {
                case DefaultKeys.Up:
                    return 0;
                case DefaultKeys.Down:
                    return 1;
                case DefaultKeys.Left:
                    return 2 ;
                case DefaultKeys.Right:
                    return 3;
                case DefaultKeys.Shoot:
                    return 4;
                case DefaultKeys.Boost:
                    return 5;
                case DefaultKeys.AltShoot:
                    return 6;
                // case DefaultKeys.Up:
                //     return 7;
                // case DefaultKeys.Down:
                //     return 8;
                // case DefaultKeys.Left:
                //     return 9;
                // case DefaultKeys.Right:
                //     return 10;
                // case DefaultKeys.Shoot:
                //     return 11;
                // case DefaultKeys.Boost:
                //     return 12;
                // case DefaultKeys.AltShoot:
                //     return 13;
                default:
                    Console.WriteLine("no translation found");
                    return -1;
            }
        }
    }
}
