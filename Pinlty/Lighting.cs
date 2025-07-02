using System.Numerics;

namespace Pinlty
{
    public class Lighting : Shader
    {
        List<List<List<float>>> LightMatrix = new List<List<List<float>>>();
        Shader light = new();
        
        public List<List<List<float>>> boxLighting(Vector3 pos, Vector3 size, Vector3 lightpos, float bright)
        {
            for (int i = 0; i >= pos.X - size.X; i++)
            {
                for (int j = 0; j >= pos.Y - size.Y; j++)
                {
                    for (int k = 0; k >= pos.Z - size.Z; k++)
                    {
                        LightMatrix[i][j][k] = light.Lighting(pos, lightpos, bright, 0, 255).Y;
                    }
                }
            }
            return LightMatrix;
        }
    }
}