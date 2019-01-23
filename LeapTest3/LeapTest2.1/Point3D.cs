using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeapTest2._1
{
    class Point3D
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Point3D(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static Point3D operator * (Point3D p, float t)
        {
            p.X *= t;
            p.Y *= t;
            p.Z *= t;
            return p;
        }
        public override string ToString()
        {
            return string.Format("x: {0}; y: {1}; z: {2}", X, Y, Z);
        }
    }
}
