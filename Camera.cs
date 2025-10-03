using System;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Pinlty
{
    public class Camera
    {
        public Vector3 Position;
        public float Pitch; // rotation around X axis
        public float Yaw;   // rotation around Y axis
        public float Speed = 0.025f;
        public float Sensitivity = 0.1f;

        private Vector2 _lastMousePos;
        private bool _firstMove = true;
        private Window _window;

        public Camera(Vector3 startPos, Window window)
        {
            Position = startPos;
            Pitch = 0f;
            Yaw = -90f; // look along -Z initially
            _window = window;
        }

        public Matrix4 GetViewMatrix()
        {
            Vector3 front;
            front.X = MathF.Cos(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));
            front.Y = MathF.Sin(MathHelper.DegreesToRadians(Pitch));
            front.Z = MathF.Sin(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));
            front = front.Normalized();

            return Matrix4.LookAt(Position, Position + front, Vector3.UnitY);
        }

        public void UpdateFrame(FrameEventArgs e, KeyboardState keyboard, MouseState mouse, Vector2 windowSize)
        {
            float delta = (float)e.Time;

            // ----- Mouse rotation -----
            Vector2 mousePos = new(mouse.X, mouse.Y);
            if (_firstMove)
            {
                _lastMousePos = mousePos;
                _window.MousePosition = new Vector2(windowSize.X / 2, windowSize.Y / 2);
                _firstMove = false;
            }
            Vector2 deltaMouse = mousePos - _lastMousePos;
            _lastMousePos = mousePos;

            Yaw += deltaMouse.X * Sensitivity;
            Pitch -= deltaMouse.Y * Sensitivity; // invert Y for typical FPS

            Pitch = MathHelper.Clamp(Pitch, -89f, 89f);

            // ----- Movement -----
            Vector3 forward;
            forward.X = MathF.Cos(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));
            forward.Y = 0; // keep movement horizontal
            forward.Z = MathF.Sin(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));
            forward = forward.Normalized();

            Vector3 right = Vector3.Cross(forward, Vector3.UnitY).Normalized();

            if (keyboard.IsKeyDown(Keys.W)) Position += forward * Speed * delta;
            if (keyboard.IsKeyDown(Keys.S)) Position -= forward * Speed * delta;
            if (keyboard.IsKeyDown(Keys.A)) Position -= right * Speed * delta;
            if (keyboard.IsKeyDown(Keys.D)) Position += right * Speed * delta;
            if (keyboard.IsKeyDown(Keys.E)) Position += Vector3.UnitY * Speed * delta;
            if (keyboard.IsKeyDown(Keys.Q)) Position -= Vector3.UnitY * Speed * delta;
        }
    }
}