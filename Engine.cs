using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using System.Drawing.Drawing2D;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pinlty
{
    public class Engine
    {
        private readonly Window _window;
        private readonly float _targetFrameRate;
        public readonly Shader _shader;

        public Engine(Window window, float targetFrameRate)
        {
            _window = window;
            _targetFrameRate = targetFrameRate;
            _shader = new Shader("shaders/text/text");
            _window._shader = _shader;
        }

        public void Run()
        {
            double targetDelta = 1.0 / _targetFrameRate;
            var stopwatch = new System.Diagnostics.Stopwatch();
            float[] vertices = [
                // positions        // colors
                0.0f,  0.5f, 1.0f, 1.0f, 0.0f, 0.0f,
                -0.5f, -0.5f, 1.0f, 0.0f, 1.0f, 0.0f,
                0.5f, -0.5f, 1.0f, 0.0f, 0.0f, 1.0f,
                0.25f, 0.0f, 0.0f, 0.5f, 0.0f, 0.5f,
                -0.25f, 0.0f, 0.0f, 0.5f, 0.5f, 0.0f,
                0.0f, -0.5f, 0.0f, 0.0f, 0.5f, 0.5f
            ];

            uint[] indices = {
                0, 4, 3,   // top-mid triangle
                4, 1, 5,   // bottom left triangle
                3, 5, 2    // bottom right triangle
            };

            Vector3 cameraPos = new(0.0f, 0.0f, 3.0f);
            Camera camera = new(cameraPos, _window);

            Mesh cottage = ObjLoader.Load("cottage/cottage_obj.obj");

            Matrix4 model = Matrix4.Identity;
            Matrix4 view = Matrix4.CreateTranslation(cameraPos.X, cameraPos.Y, -cameraPos.Z);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), _window._size.X / _window._size.Y, 0.1f, 100.0f);

            int vao = _shader.GenObject(vertices, indices);
            int modelLoc = GL.GetUniformLocation(_shader._handle, "model");
            int viewLoc = GL.GetUniformLocation(_shader._handle, "view");
            int projectionLoc = GL.GetUniformLocation(_shader._handle, "proj");
            GL.ClearColor(0.07f, 0.13f, 0.17f, 1.0f);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less); // default, optional


            while (!_window.IsExiting)
            {
                stopwatch.Restart();

                _window.ProcessEvents(0);
                projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), _window._size.X / _window._size.Y, 0.1f, 100.0f);
                camera.UpdateFrame(new FrameEventArgs(targetDelta), _window.KeyboardState, _window.MouseState, _window._size);
                view = camera.GetViewMatrix();
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                GL.UseProgram(_shader._handle);
                GL.UniformMatrix4(modelLoc, false, ref model);
                GL.UniformMatrix4(viewLoc, false, ref view);
                GL.UniformMatrix4(projectionLoc, false, ref projection);

                //GL.BindVertexArray(vao); // <-- critical
                //DrawTrianglesBackToFront(vao, vertices, indices, model, view);
                cottage.Render();

                _window.SwapBuffers();
            }
        }

        /*public void RenderDepth(int[] vaos, float[][] vertices, uint[][] indices, Matrix4[] model, Matrix4[] view)
        {
            int[] oVaos = Order(, [.. vaos]);
        }*/

        public static List<T> Order<T>(List<float> originalDistances, List<T> relatedItems)
        {
            if (originalDistances.Count != relatedItems.Count)
                throw new ArgumentException("The distances and related items lists must be the same length.");

            // create ordered index list by distance ascending
            var orderedIndices = originalDistances
                .Select((d, i) => (dist: d, idx: i))
                .OrderBy(pair => pair.dist)
                .Select(pair => pair.idx)
                .ToList();

            var reordered = new List<T>(relatedItems.Count);
            foreach (var idx in orderedIndices)
                reordered.Add(relatedItems[idx]);

            return reordered;
        }

        public void DrawTrianglesBackToFront(int vao, float[] vertices, uint[] indices, Matrix4 model, Matrix4 view)
        {
            int triCount = indices.Length / 3;
            int[] order = new int[triCount];
            float[] depth = new float[triCount];

            for (int i = 0; i < triCount; i++)
            {
                int i0 = (int)indices[i * 3], i1 = (int)indices[i * 3 + 1], i2 = (int)indices[i * 3 + 2];
                Vector4 v0 = view * model * new Vector4(vertices[i0 * 6], vertices[i0 * 6 + 1], vertices[i0 * 6 + 2], 1);
                Vector4 v1 = view * model * new Vector4(vertices[i1 * 6], vertices[i1 * 6 + 1], vertices[i1 * 6 + 2], 1);
                Vector4 v2 = view * model * new Vector4(vertices[i2 * 6], vertices[i2 * 6 + 1], vertices[i2 * 6 + 2], 1);
                depth[i] = (v0.Z + v1.Z + v2.Z) / 3f;
                order[i] = i;
            }

            for (int i = 0; i < triCount - 1; i++)
                for (int j = 0; j < triCount - i - 1; j++)
                    if (depth[j] < depth[j + 1])
                    {
                        float tmpD = depth[j]; depth[j] = depth[j + 1]; depth[j + 1] = tmpD;
                        int tmpO = order[j]; order[j] = order[j + 1]; order[j + 1] = tmpO;
                    }

            GL.BindVertexArray(vao);
            for (int i = 0; i < triCount; i++)
                GL.DrawElements(PrimitiveType.Triangles, 3, DrawElementsType.UnsignedInt, (IntPtr)(order[i] * 3 * sizeof(uint)));
        }
    }
}