namespace States.Core.Commands
{
    
    public static class CommandConstants
    {
        public const byte False       =    0b00000001;
        public const byte True        =    0b00000010;
        public const byte EAX         =    0b00000100;
        public const byte EBX         =    0b00001000;
        public const byte ECX         =    0b00010000;
        public const byte EDX         =    0b00100000;
        public const byte Interrupt   =    0b01000000;
        public const byte Move        =    0b10000000;

        public const byte All         =    0b11111111;
        public const byte None        =    0b00000000;
    }
}