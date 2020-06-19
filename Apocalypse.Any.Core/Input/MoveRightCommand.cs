using Apocalypse.Any.Core.Behaviour;

namespace Apocalypse.Any.Core.Input
{
    /// <summary>
    /// Simple command for moving the X position += 1
    /// </summary>
    internal class MoveRightCommand : ICommand<MovementBehaviour>
    {
        public bool CanExecute(MovementBehaviour parameters)
        {
            throw new System.NotImplementedException();
        }

        public void Execute(MovementBehaviour parameters)
        {
            parameters.X += 1;
        }
    }
}