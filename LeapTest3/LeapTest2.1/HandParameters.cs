using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeapTest2._1
{
    class HandParameters
    {
        private static List<Point3D[]> _bones;
        private static List<Point3D> _fingertips;
        private static Point3D _palm;

        public static List<Point3D[]> bones
        {
            get
            {
                return _bones;
            }
            set
            {
                _bones = value;
                foreach (Point3D[] item in _bones)
                {
                    for (int i = 0; i < item.Length; i++)
                    {
                        item[i] *= 0.1f;
                    }
                }
            }
        }
        public static List<Point3D> fingertips
        {
            get
            {
                return _fingertips;
            }
            set
            {
                _fingertips = value;
                foreach (Point3D item in _fingertips)
                {
                    item.X *= 0.1f;
                    item.Y *= 0.1f;
                    item.Z *= 0.1f;
                }
            }
        }
        public static Point3D palm
        {
            get
            {
                return _palm;
            }
            set
            {
                _palm = value * 0.1f;
            }
        }

        static HandParameters()
        {
            bones = new List<Point3D[]>();
            fingertips = new List<Point3D>();
        }

        public static Point3D[][] bonesArray()
        {
            lock (bones)
            {
                return bones.ToArray();
            }
        }

        
    }
}
