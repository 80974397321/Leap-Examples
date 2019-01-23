using System;
using System.Windows.Forms;
using System.Drawing;
using Leap;

namespace LeapTest
{
    public partial class Form1 : Form
    {
        MyListener listener;
        Controller controller;

        Graphics g;

        int LastRememberedSections = 0;

        Bitmap image = new Bitmap(500, 561);

        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            // Инстанцировать контроллер и обработчик
            listener = new MyListener();
            controller = new Controller();

            controller.AddListener(listener);
            
            g = Graphics.FromImage(image);
            timer1.Start();
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            controller.RemoveListener(listener);
            controller.Dispose();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Дорисовать новые линии на изображении
            int tmpLen = MyLines.Lines.Count;
            if(LastRememberedSections != MyLines.Lines.Count)
            {
                for (int i = LastRememberedSections; i < MyLines.Lines.Count; i++)
                {
                    Draw(MyLines.Lines[i]);
                }
                LastRememberedSections = tmpLen;
                pictureBox1.BackgroundImage = new Bitmap(image);
            }

            if ((DateTime.Now - Utilities.LastRead).TotalSeconds > 5)
                panel1.BackColor = Color.Red;
            else
                panel1.BackColor = Color.Green;
        }

        private void Draw(Section s)
        {
            Pen p = new Pen(s.Color, s.LineWidth);
            g.DrawLine(p, s.Start, s.End);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(colorDialog1.ShowDialog() == DialogResult.OK)
            {
                Utilities.currentColor = colorDialog1.Color;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Очистка изображения
            MyLines.Lines.Clear();
            MyLines.LastPoint = null;
            g.Clear(Color.White);
            LastRememberedSections = 0;
            pictureBox1.BackgroundImage = new Bitmap(image);
        }
    }
}
