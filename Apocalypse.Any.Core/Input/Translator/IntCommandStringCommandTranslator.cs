namespace Apocalypse.Any.Core.Input.Translator
{
    public class IntCommandStringCommandTranslator : 
        IInputTranslator<int, string>, 
        IInputTranslator<string, int>
    {
        public string Translate(int input)
        {
            switch (input)
            {
                case 0:
                    return DefaultKeys.Up + DefaultKeys.Press;
                case 1:
                    return DefaultKeys.Down + DefaultKeys.Press;
                case 2:
                    return DefaultKeys.Left + DefaultKeys.Press;
                case 3:
                    return DefaultKeys.Right + DefaultKeys.Press;
                case 4:
                    return DefaultKeys.Shoot + DefaultKeys.Press;
                case 5:
                    return DefaultKeys.Boost + DefaultKeys.Press;
                case 6:
                    return DefaultKeys.AltShoot + DefaultKeys.Press;
                case 7:
                    return DefaultKeys.Up + DefaultKeys.Release;
                case 8:
                    return DefaultKeys.Down + DefaultKeys.Release;
                case 9:
                    return DefaultKeys.Left + DefaultKeys.Release;
                case 10:
                    return DefaultKeys.Right + DefaultKeys.Release;
                case 11:
                    return DefaultKeys.Shoot + DefaultKeys.Release;
                case 12:
                    return DefaultKeys.Boost + DefaultKeys.Release;
                case 13:
                    return DefaultKeys.AltShoot + DefaultKeys.Release;
                default:
                    return string.Empty;
            }
        }

        public int Translate(string input)
        {
            switch (input)
            {
                case DefaultKeys.Up + DefaultKeys.Press:
                    return 0;
                case DefaultKeys.Down + DefaultKeys.Press:
                    return 1;
                case DefaultKeys.Left + DefaultKeys.Press:
                    return 2 ;
                case DefaultKeys.Right + DefaultKeys.Press:
                    return 3;
                case DefaultKeys.Shoot + DefaultKeys.Press:
                    return 4;
                case DefaultKeys.Boost + DefaultKeys.Press:
                    return 5;
                case DefaultKeys.AltShoot + DefaultKeys.Press:
                    return 6;
                case DefaultKeys.Up + DefaultKeys.Release:
                    return 7;
                case DefaultKeys.Down + DefaultKeys.Release:
                    return 8;
                case DefaultKeys.Left + DefaultKeys.Release:
                    return 9;
                case DefaultKeys.Right + DefaultKeys.Release:
                    return 10;
                case DefaultKeys.Shoot + DefaultKeys.Release:
                    return 11;
                case DefaultKeys.Boost + DefaultKeys.Release:
                    return 12;
                case DefaultKeys.AltShoot + DefaultKeys.Release:
                    return 13;
                default:
                    return -1;
            }
        }
    }
}