using OpenTK.Graphics.OpenGL4;

namespace Pinlty
{
    public class Mesh
    {
        public int Vao;
        public int VertexCount;
        public int Texture;

        public Mesh(float[] vertices, float[] normals, float[] uvs, int textureId = 0)
        {
            Vao = GL.GenVertexArray();
            GL.BindVertexArray(Vao);

            // Vertex positions
            int vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0);

            // Normals
            if (normals.Length > 0)
            {
                int nbo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, nbo);
                GL.BufferData(BufferTarget.ArrayBuffer, normals.Length * sizeof(float), normals, BufferUsageHint.StaticDraw);
                GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexAttribArray(1);
            }

            // UVs
            if (uvs.Length > 0)
            {
                int tbo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, tbo);
                GL.BufferData(BufferTarget.ArrayBuffer, uvs.Length * sizeof(float), uvs, BufferUsageHint.StaticDraw);
                GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexAttribArray(2);
            }

            GL.BindVertexArray(0);

            VertexCount = vertices.Length / 3;
            Texture = textureId;
        }

        public void Render()
        {
            if (Texture != 0) GL.BindTexture(TextureTarget.Texture2D, Texture);
            GL.BindVertexArray(Vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, VertexCount);
        }
    }
}