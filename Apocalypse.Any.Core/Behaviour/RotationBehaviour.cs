namespace Apocalypse.Any.Core.Behaviour
{
    /// <summary>
    /// Describes the actual representation of the rotation degree of an object.
    /// This behaviour can be used for velocity or anything that is meant to change the rotation or a bound position of a top down game object
    /// </summary>
    public class RotationBehaviour //: Behaviour
    {
        public float Rotation { get; set; } = 0;

        public float Delta { get; set; } = 0.05f; // WHY?? //TODO: verify this

        public static implicit operator float(RotationBehaviour p)
        {
            return p.Rotation * p.Delta;
        }
    }
}