using System.Numerics;

namespace Pinlty
{
    public class Transform
    {
        public Vector3 Position = Vector3.Zero;
        public Vector3 Scale = Vector3.One;
        public Vector3 Rotation = Vector3.Zero;
    }

    public class GameObject
    {
        public required string Name;
        public Transform Transform = new();
        public List<System.ComponentModel.Component> Components = [];
    }
}