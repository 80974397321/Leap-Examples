using System;

namespace LeapTest2._1
{
    enum Axis
    {
        x, y, z
    }

    static class TransformParameters
    {
        private static int dimentions = 3;

        private static float[] rotations;
        private static float[] scaling;
        private static float[] translation;

        static TransformParameters()
        {
            Translation = new float[dimentions];
            Rotation = new float[dimentions];
            Scaling = new float[dimentions];
            for (int i = 0; i < Scaling.Length; i++)
            {
                Scaling[i] = 1;
            }
            for (int i = 0; i < Rotation.Length; i++)
            {
                Rotation[i] = 0;
            }
            for (int i = 0; i < Translation.Length; i++)
            {
                Translation[i] = 0;
            }
        }

        public static float[] Translation
        {
            get
            {
                CorrectNaNInArray(translation);
                return translation;
            }
            private set
            {
                translation = value;
            }
        }
        public static float[] Rotation
        {
            get
            {
                CorrectNaNInArray(rotations);
                return rotations;
            }
            private set
            {
                rotations = value;
            }
        }
        public static float[] Scaling
        {
            get
            {
                CorrectNaNInArray(scaling);
                return scaling;
            }
            private set
            {
                if (value.Length != dimentions) throw new ArgumentException("Array dimentions don't match with ones defined in class definitions");
                scaling = value;
            }
        }

        private static void CorrectNaNInArray(float[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (float.IsNaN(array[i]))
                    array[i] = 0;
            }
        }
    }
}
