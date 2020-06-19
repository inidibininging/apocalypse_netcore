using Apocalypse.Any.Core.Behaviour;

namespace Apocalypse.Any.Core.Input
{
    /// <summary>
    /// Simple command for changing the rotation by -1 (to the left)
    /// </summary>
    public class RotateLeftCommand : ICommand<RotationBehaviour>
    {
        public bool CanExecute(RotationBehaviour parameters)
        {
            throw new System.NotImplementedException();
        }

        public void Execute(RotationBehaviour parameters)
        {
            var rotation = parameters.Rotation;
            if ((rotation - 1) < 0)
                parameters.Rotation = 360 - rotation;
            parameters.Rotation -= 1;
        }
    }
}