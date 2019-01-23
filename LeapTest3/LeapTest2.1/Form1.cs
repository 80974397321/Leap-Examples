using System;
using System.IO;
using System.Windows.Forms;
using Tao.DevIl;
using Tao.FreeGlut;
using Tao.OpenGl;
using Leap;

namespace LeapTest2._1
{
    public partial class Form1 : Form
    {
        const float distance = 1000;
        StreamWriter logger;

        Controller controller;
        MyListener listener;

        object thisLock = new object();

        uint textureObject;

        bool[] block = { false, false, false, false, true };
        char[] letters = { 'a', 'b', 'c', 'd' };

        // Инициализация OpenGl
        void DevIlInit()
        {
            Il.ilInit();
            Il.ilEnable(Il.IL_ORIGIN_SET);
        }

        void OpenGlInit()
        {
            Glut.glutInit();
            Glut.glutInitDisplayMode(Glut.GLUT_RGBA | Glut.GLUT_DOUBLE | Glut.GLUT_DEPTH);
            Gl.glClearColor(255, 255, 255, 1);
            Gl.glViewport(0, 0, Scene.Width, Scene.Height);
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Glu.gluPerspective(45, (float)Scene.Width / Scene.Height, 1, 2 * distance);

            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();

            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glEnable(Gl.GL_LIGHTING);
            Gl.glEnable(Gl.GL_LIGHT0);

            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            Gl.glHint(Gl.GL_LINE_SMOOTH_HINT, Gl.GL_NICEST);
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glEnable(Gl.GL_LINE_SMOOTH);
            Gl.glLineWidth(5.0f);
        }

        public Form1()
        {
            InitializeComponent();
            Scene.InitializeContexts();
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            OpenGlInit();
            DevIlInit();

            loadImage("img/2.jpg");

            AnimationTimer.Start();

            logger = new StreamWriter("logger.txt", true);
            listener = new MyListener();
            listener.output += (string s) => { logger.WriteLine(s); };

            controller = new Controller();
            controller.AddListener(listener);
        }

        // Циклическая отрисовка панели ввода и положения руки
        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            Gl.glLoadIdentity();
            Gl.glColor3f(1.0f, 0, 0);
            Gl.glPushMatrix();

            Gl.glTranslated(0, 0, -100);

            // Панелька
            Gl.glEnable(Gl.GL_TEXTURE_2D); // включаем режим текстурирования, указывая идентификатор mGlTextureObject
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, textureObject); // сохраняем состояние матрицы
            Gl.glPushMatrix(); // выполняем перемещение для более наглядного представления сцены
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glVertex3f(-16, 7, -16);
            Gl.glTexCoord2f(0, 0);
            Gl.glVertex3f(-16, 7, 16);
            Gl.glTexCoord2f(1, 0);
            Gl.glVertex3f(16, 7, 16);
            Gl.glTexCoord2f(1, 1);
            Gl.glVertex3f(16, 7, -16);
            Gl.glTexCoord2f(0, 1);
            Gl.glEnd();
            Gl.glPopMatrix();
            Gl.glDisable(Gl.GL_TEXTURE_2D);

            // Рука
            Point3D[][] bonesCopy = HandParameters.bonesArray();
            foreach (Point3D[] item in bonesCopy)
            {
                Gl.glBegin(Gl.GL_LINE_LOOP);
                for (int i = 0; i < item.Length; i++)
                {
                    Gl.glVertex3f(item[i].X, item[i].Y, item[i].Z);
                }
                Gl.glEnd();
            }

            Gl.glPopMatrix();
            Gl.glFlush();
            Scene.Invalidate();

            // Проверяем соприкосновение и производим ввод
            lock (HandParameters.fingertips)
            {
                foreach (var item in HandParameters.fingertips)
                {
                    if (item.Y <= 7 && item.Z >= -16 && item.Z <= 16)
                    {
                        int part = getPart(item.X);
                        if (!block[part])
                        {
                            textBox1.Text += letters[part];
                            block[part] = true;
                        }
                    }
                }

            }

            for (int i = 0; i < block.Length - 1; i++)
            {
                bool unlock = true;
                if(block[i])
                {
                    lock (HandParameters.fingertips)
                    {
                        foreach (var item in HandParameters.fingertips)
                        {
                            if (item.Y <= 7 && getPart(item.X) == i && item.Z >= -16 && item.Z <= 16)
                            {
                                unlock = false;
                            }
                                    
                        }
                    }
                    if (unlock) block[i] = false;
                }
            }
            


            if(HandParameters.palm != null)
                label1.Text = HandParameters.palm.ToString();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            controller.RemoveListener(listener);
            controller.Dispose();
            logger.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            block = new bool[] { false, false, false, false, true };
        }

        private void DrawLine(Point3D a, Point3D b)
        {

            Gl.glBegin(Gl.GL_LINES);

            Gl.glColor3f(0, 0, 1);
            Gl.glVertex3f(a.X, a.Y, a.Z);
            Gl.glVertex3f(b.X, b.Y, b.Z);
            Gl.glEnd();
        }

        private void loadImage(string path = "img/1.jpg")
        {
            // создаем изображение с идентификатором imageId
            int imageId, width, height;
            Il.ilGenImages(1, out imageId);
            Il.ilBindImage(imageId);
            Il.ilLoadImage(path);
            width = Il.ilGetInteger(Il.IL_IMAGE_WIDTH);
            height = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT);
            switch (Il.ilGetInteger(Il.IL_IMAGE_BITS_PER_PIXEL))
            {
                case 24:
                    textureObject = MakeGlTexture(Gl.GL_RGB, Il.ilGetData(), width, height);
                    break;
                case 32:
                    textureObject = MakeGlTexture(Gl.GL_RGBA, Il.ilGetData(), width, height);
                    break;
            }
            Il.ilDeleteImages(1, ref imageId);
        }

        private static uint MakeGlTexture(int Format, IntPtr pixels, int w, int h)
        {
            uint texObject;
            Gl.glGenTextures(1, out texObject);
            Gl.glPixelStorei( Gl.GL_UNPACK_ALIGNMENT, 1);
            Gl.glBindTexture( Gl.GL_TEXTURE_2D, texObject);
            Gl.glTexParameteri( Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT);
            Gl.glTexParameteri( Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT);
            Gl.glTexParameteri( Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
            Gl.glTexParameteri( Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
            Gl.glTexEnvf( Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_REPLACE); 
            switch (Format)
            {
                case Gl.GL_RGB:
                    Gl.glTexImage2D( Gl.GL_TEXTURE_2D, 0, Gl.GL_RGB, w, h, 0, Gl.GL_RGB, Gl.GL_UNSIGNED_BYTE, pixels);
                    break;
                case Gl.GL_RGBA:
                    Gl.glTexImage2D( Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA, w, h, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, pixels);
                    break;
            }
            return texObject;
        }

        private int getPart(float x)
        {
            if (x >= -16 && x <= -8) return 0;
            else if (x >= -8 && x <= 0) return 1;
            else if (x >= 0 && x <= 8) return 2;
            else if (x >= 8 && x <= 16) return 3;
            else return 5;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
