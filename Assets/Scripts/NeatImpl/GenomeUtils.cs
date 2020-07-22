using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeatImpl
{
    public static class GenomeUtils
    {
        private static Random rand = new Random();

        public const int MAX_CONNECTION_NUMBER = 1024;

        public static float GetRandomWeight()
        {
            return rand.Next(-1500, 1500) * 0.01f;
        }

        public static float[] GetRandomColor3f()
        {
            float[] _color = new float[3];

            _color[0] = rand.Next(0, 100) * 0.01f;
            _color[1] = rand.Next(0, 100) * 0.01f;
            _color[2] = rand.Next(0, 100) * 0.01f;

            return _color;

        }
        public static float GetRandomNumber()
        {
            return rand.Next(-100, 100) * 0.01f;
        }
    }
}
