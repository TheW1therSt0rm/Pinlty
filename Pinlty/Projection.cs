namespace Pinlty
{
    public class Projection
    {
        public System.Numerics.Vector2 Project(System.Numerics.Vector3 pos, float focalLength)
        {
            return new (pos.X / (focalLength + pos.Z) * focalLength, pos.Y / (focalLength + pos.Z) * focalLength);
        }
    }
}