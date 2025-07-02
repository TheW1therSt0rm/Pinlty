using System.Numerics;

namespace Pinlty
{
    public class Shader
    {
        private Vector3 dist;
        private float distv;
        private float fbright;

        public Shader()
        {
            // Initizalize the shaders and varibles
            dist = new Vector3(0f);
            distv = 0f;
            fbright = 0f;
        }

        public Vector2 Lighting(Vector3 pos, Vector3 lightpos, float lbrightness, float brightness, float maxscale)
        {
            dist = new Vector3(lightpos.X - pos.X, lightpos.Y - pos.Y, lightpos.Z - pos.Z);
            distv = (dist.X + dist.Y + dist.Z) / 3f;
            fbright = (lbrightness - (lbrightness / distv)) * brightness;
            if (fbright < maxscale)
            {
                return new(fbright, lbrightness - (lbrightness / distv));
            }
            else
            {
                return new(maxscale, lbrightness - (lbrightness / distv));
            }
        }

        public float extbright(float extbright, float bright, float maxscale)
        {
            fbright = extbright * bright;
            if (fbright < maxscale)
            {
                return fbright;
            }
            else
            {
                return maxscale;
            }
        }

        public static Color color(float hue, float brightness, float saturation = 1.0f)
        {
            hue = hue % 360f;
            float c = brightness * saturation;
            float x = c * (1 - Math.Abs(hue / 60f % 2 - 1));
            float m = brightness - c;

            float r = 0, g = 0, b = 0;

            if (hue < 60)
                (r, g, b) = (c, x, 0);
            else if (hue < 120)
                (r, g, b) = (x, c, 0);
            else if (hue < 180)
                (r, g, b) = (0, c, x);
            else if (hue < 240)
                (r, g, b) = (0, x, c);
            else if (hue < 300)
                (r, g, b) = (x, 0, c);
            else
                (r, g, b) = (c, 0, x);

            return Color.FromArgb(
                (int)((r + m) * 255),
                (int)((g + m) * 255),
                (int)((b + m) * 255)
            );
        }
    }
}