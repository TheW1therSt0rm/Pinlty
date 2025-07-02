using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace Pinlty
{
    public class MainForm : GameWindow
    {
        private Projection projection;
        private int vertexShader = 0;
        private int fragmentShader = 0;
        private int shaderProgram = 0;
        private int vao = 0;
        private int vbo = 0;

        public MainForm() : base(GameWindowSettings.Default, new NativeWindowSettings
        {
            ClientSize = new Vector2i(800, 600),
            Title = "Pinlty Editor"
        })
        {
            projection = new Projection();
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);

            // Example projection logic
            System.Numerics.Vector3 point = new System.Numerics.Vector3(2f, 4f, 10f);
            System.Numerics.Vector2 projected = projection.Project(point, 300f);
            System.Console.WriteLine($"Projected: {projected}");

            // Load and compile vertex and fragment shaders
            vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, File.ReadAllText("Shaders/default.vert"));
            GL.CompileShader(vertexShader);

            fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, File.ReadAllText("Shaders/default.frag"));
            GL.CompileShader(fragmentShader);

            // Link into shader program
            shaderProgram = GL.CreateProgram();
            GL.AttachShader(shaderProgram, vertexShader);
            GL.AttachShader(shaderProgram, fragmentShader);
            GL.LinkProgram(shaderProgram);

            // Cleanup
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            float[] vertices = {
            // positions        // colors
            -0.5f, -0.5f, 0f,   1f, 0f, 0f,
            0.5f, -0.5f, 0f,   0f, 1f, 0f,
            0.0f,  0.5f, 0f,   0f, 0f, 1f
        };

            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();

            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            // position (0)
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // color (1)
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);
        }

        protected override void OnRenderFrame(OpenTK.Windowing.Common.FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.UseProgram(shaderProgram);
            GL.BindVertexArray(vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

            SwapBuffers();
        }

        public void OnUpdateFrame()
        {
            Input.Update(KeyboardState, MouseState);
        }
    }
}