using System;
using System.Drawing;

namespace LeapTest
{

    // Вспомагательные методы, глобальные переменные и константы
    static class Utilities
    {
        // Цвет линий
        public static Color currentColor { get; set; }

        static Utilities()
        {
            currentColor = Color.Black;
        }

        public static double Distance(Point a, Point b)
        {
            return Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
        }

        // Перевод из системы координат плоскости рисование в экранные
        public static Point ToScreenCoords(float xCont, float yCont)
        {
            Point screenCoord = new Point();
            screenCoord.X = (int)((xCont - controllerMinimumX) / (controllerMaximumX - controllerMinimumX) * (screenMaximumX - screenMinimumX) + screenMinimumX);
            screenCoord.Y = (int)((controllerMaximumY - yCont) / (controllerMaximumY - controllerMinimumY) * (screenMaximumY - screenMinimumY) + screenMinimumY);
            return screenCoord;
        }

        // Размеры обрабатываемой области пространства
        public static readonly int controllerMinimumX = -400;
        public static readonly int controllerMaximumX = 400;
        public static readonly int controllerMinimumY = 20;
        public static readonly int controllerMaximumY = 350;

        // Размеры экранной области вывода изображения
        public static readonly int screenMinimumX = 0;
        public static readonly int screenMaximumX = 500;
        public static readonly int screenMinimumY = 0;
        public static readonly int screenMaximumY = 500;

        public static readonly int minimalViableDistance = 3;
        public static readonly int widthSections = 3;

        // Отклонение по оси Z при отслеживании
        public static readonly int maximalZToTrack = 0;
        public static readonly int minimalZToTrack = -200;

        public static DateTime LastRead { get; set; }
    }
}
