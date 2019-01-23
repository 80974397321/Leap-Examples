using System;
using System.Collections.Generic;
using System.Drawing;

namespace LeapTest
{
    // Контейнер для отрезков, выделяемых в кадрах контроллера
    static class MyLines
    {
        public static List<Section> Lines { get; set; }
        public static Nullable<Point> LastPoint { get; set; }

        public static bool Break { get; set; }
        static MyLines()
        {
            Lines = new List<Section>();
            LastPoint = null;
            Break = false;
        }

    }
}
