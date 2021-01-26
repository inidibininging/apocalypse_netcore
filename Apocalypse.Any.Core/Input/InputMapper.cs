using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Core.Input.Translator;
using System;
using System.Collections.Generic;

namespace Apocalypse.Any.Core.Input
{
    /// <summary>
    /// This class is for a level of abstraction. Every input processed by the game should be mapped to a string that can be interpreted as a movement command.
    /// This means that every game object should be listening to movement commands and not to the input intself.
    /// </summary>
    [Serializable]
    public class InputMapper
    {
        private static IList<ISingleKeyToMovementCommandTranslator> _defaultMap;
        private static IList<ISingleKeyToRotationCommandTranslator> _defaultRotationMap;

        public static IList<ISingleKeyToMovementCommandTranslator> DefaultMovementMap
            => _defaultMap ?? (_defaultMap = MovementMap());

        public static IList<ISingleKeyToRotationCommandTranslator> DefaultRotationMap
            => _defaultRotationMap ?? (_defaultRotationMap = RotationMap());

        public void ReadMapping(string map)
        {
        }

        private static IList<ISingleKeyToMovementCommandTranslator> MovementMap()
        {
            var map = new List<ISingleKeyToMovementCommandTranslator>()
            {
                new SingleKeyToMovementDelegateCommandTranslator(DefaultKeys.Up, new List<ICommand<MovementBehaviour>>() { new MoveUpCommand() }),
                new SingleKeyToMovementDelegateCommandTranslator(DefaultKeys.Down, new List<ICommand<MovementBehaviour>>() { new MoveDownCommand() }),
                new SingleKeyToMovementDelegateCommandTranslator(DefaultKeys.Left, new List<ICommand<MovementBehaviour>>() { new MoveLeftCommand() }),
                new SingleKeyToMovementDelegateCommandTranslator(DefaultKeys.Right, new List<ICommand<MovementBehaviour>>() { new MoveRightCommand() })
            };
            return map;
        }

        private static IList<ISingleKeyToRotationCommandTranslator> RotationMap()
        {
            var map = new List<ISingleKeyToRotationCommandTranslator>()
            {
                new SingleKeyToRotationDelegateCommandTranslator(DefaultKeys.Left, new List<ICommand<RotationBehaviour>>() { new RotateLeftCommand() }),
                new SingleKeyToRotationDelegateCommandTranslator(DefaultKeys.Right, new List<ICommand<RotationBehaviour>>() { new RotateRightCommand() })
            };
            return map;
        }

        
    }
}