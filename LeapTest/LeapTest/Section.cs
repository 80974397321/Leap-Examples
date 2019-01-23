using System.Drawing;

namespace LeapTest
{
    // Простой класс отрезка
    class Section
    {
        private Color black = Color.Black;
        public Point Start { get; set; }
        public Point End { get; set; }
        public Color Color { get; set; }
        public int LineWidth { get; set; }

        public Section(Point s, Point e, Color c, int w = 1)
        {
            Start = s;
            End = e;

            if (c == null)
                Color = Color.Black;
            else
                Color = c;

            LineWidth = w;
        }
    }
}
