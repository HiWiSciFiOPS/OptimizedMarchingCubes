using UnityEngine;

namespace MarchingCubes
{
    public struct GridPoint
    {
        public float v { get; set; }
        public Color c { get; set; }

        public override string ToString()
        {
            return v + "." + c.r + ":" + c.g + ":" + c.b;
        }
    }
}