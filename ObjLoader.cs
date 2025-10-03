using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace Pinlty
{
    public static class ObjLoader
    {
        public static Mesh Load(string objPath)
        {
            List<Vector3> tempVertices = new();
            List<Vector3> tempNormals = new();
            List<Vector2> tempUVs = new();

            List<int> vertexIndices = new();
            List<int> normalIndices = new();
            List<int> uvIndices = new();

            string texturePath = "";

            foreach (var line in File.ReadLines(objPath))
            {
                string l = line.Trim();
                if (l.StartsWith("v ")) // vertex
                {
                    string[] parts = l.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    tempVertices.Add(new Vector3(
                        float.Parse(parts[1], CultureInfo.InvariantCulture),
                        float.Parse(parts[2], CultureInfo.InvariantCulture),
                        float.Parse(parts[3], CultureInfo.InvariantCulture)
                    ));
                }
                else if (l.StartsWith("vn ")) // normal
                {
                    string[] parts = l.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    tempNormals.Add(new Vector3(
                        float.Parse(parts[1], CultureInfo.InvariantCulture),
                        float.Parse(parts[2], CultureInfo.InvariantCulture),
                        float.Parse(parts[3], CultureInfo.InvariantCulture)
                    ));
                }
                else if (l.StartsWith("vt ")) // uv
                {
                    string[] parts = l.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    tempUVs.Add(new Vector2(
                        float.Parse(parts[1], CultureInfo.InvariantCulture),
                        1 - float.Parse(parts[2], CultureInfo.InvariantCulture) // invert Y
                    ));
                }
                else if (l.StartsWith("f ")) // face
                {
                    string[] parts = l.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var part in parts[1..])
                    {
                        string[] indices = part.Split('/');
                        vertexIndices.Add(int.Parse(indices[0]) - 1);
                        uvIndices.Add(indices.Length > 1 && indices[1] != "" ? int.Parse(indices[1]) - 1 : 0);
                        normalIndices.Add(indices.Length > 2 ? int.Parse(indices[2]) - 1 : 0);
                    }
                }
                else if (l.StartsWith("mtllib ")) // material file
                {
                    string mtlFile = l.Split(' ')[1];
                    string mtlPath = Path.Combine(Path.GetDirectoryName(objPath), mtlFile);
                    texturePath = ParseMTL(mtlPath); // get texture path
                }
            }

            // Build final arrays
            float[] vertices = new float[vertexIndices.Count * 3];
            float[] normals = new float[vertexIndices.Count * 3];
            float[] uvs = new float[vertexIndices.Count * 2];

            for (int i = 0; i < vertexIndices.Count; i++)
            {
                Vector3 v = tempVertices[vertexIndices[i]];
                Vector3 n = normalIndices[i] < tempNormals.Count ? tempNormals[normalIndices[i]] : Vector3.UnitY;
                Vector2 uv = uvIndices[i] < tempUVs.Count ? tempUVs[uvIndices[i]] : Vector2.Zero;

                vertices[i * 3 + 0] = v.X;
                vertices[i * 3 + 1] = v.Y;
                vertices[i * 3 + 2] = v.Z;

                normals[i * 3 + 0] = n.X;
                normals[i * 3 + 1] = n.Y;
                normals[i * 3 + 2] = n.Z;

                uvs[i * 2 + 0] = uv.X;
                uvs[i * 2 + 1] = uv.Y;
            }

            int texId = 0;
            if (!string.IsNullOrEmpty(texturePath))
                texId = LoadTexture(texturePath);

            return new Mesh(vertices, normals, uvs, texId);
        }

        private static string ParseMTL(string mtlPath)
        {
            foreach (var line in File.ReadLines(mtlPath))
            {
                string l = line.Trim();
                if (l.StartsWith("map_Kd ")) // diffuse texture
                {
                    string texFile = l.Split(' ')[1];
                    return Path.Combine(Path.GetDirectoryName(mtlPath), texFile);
                }
            }
            return "";
        }

        private static int LoadTexture(string path)
        {
            using var stream = File.OpenRead(path);
            var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

            int tex = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, tex);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            return tex;
        }
    }
}