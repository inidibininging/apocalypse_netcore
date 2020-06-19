namespace Apocalypse.Any.Core.Behaviour
{
    /// <summary>
    /// This class changes the alpha property of an object.
    /// </summary>
    public class AlphaBehaviour // : Behaviour
    {
        /*public AlphaBehaviour(IGameObject target) : base(target)
        {
        }*/

        public float Alpha { get; set; } = 1.0f;

        public static implicit operator float(AlphaBehaviour behaviour)
        {
            return behaviour.Alpha;
        }
    }
}