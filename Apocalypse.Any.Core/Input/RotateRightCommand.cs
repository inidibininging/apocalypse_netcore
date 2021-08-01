using Apocalypse.Any.Core.Behaviour;

namespace Apocalypse.Any.Core.Input
{
    /// <summary>
    /// Simple command for changing the rotation by -1 (to the right)
    /// </summary>
    public class RotateRightCommand : ICommand<RotationBehaviour>
    {
        public bool CanExecute(RotationBehaviour parameters)
        {
            throw new System.NotImplementedException();
        }

        public void Execute(RotationBehaviour parameters)
        {
            //var rotation = parameters.Rotation;

            //if ((rotation + 1) > 360)
            //    parameters.Rotation = 360 - rotation;
            //parameters.Rotation -= 1;

            parameters.Rotation += 0.05f;//0.001f;
        }
    }
}