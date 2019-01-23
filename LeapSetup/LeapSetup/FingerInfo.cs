using Leap;

namespace LeapSetup
{
    // Класс-утилита для сохранения информации о пальце
    static class FingerInfo
    {
        public const int fingersInHand = 5;
        public const int pointNumber = 5;
        public const int tip = 0;
        public const int distal = 1;
        public const int intermediate = 2;
        public const int proximal = 3;
        public const int metacarpal = 4;

        public static Vector[] Thumb { get; set; }
        public static Vector[] Index { get; set; }
        public static Vector[] Middle { get; set; }
        public static Vector[] Ring { get; set; }
        public static Vector[] Pinky { get; set; }

        static FingerInfo()
        {
            Thumb = new Vector[pointNumber];
            Index = new Vector[pointNumber];
            Middle = new Vector[pointNumber];
            Ring = new Vector[pointNumber];
            Pinky = new Vector[pointNumber];
        }
    }
}
