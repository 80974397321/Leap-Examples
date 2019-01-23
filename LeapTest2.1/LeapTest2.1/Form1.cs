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
        const float distance = 200;
        StreamWriter logger;

        Controller controller;
        MyListener listener;

        delegate void AddText(string text);

        object thisLock = new object();

        // Потокобезопасный вывод текста
        void addText(string text)
        {
            if (label1.InvokeRequired)
            {
                AddText d = new AddText(addText);
                this.Invoke(d, text);
            }
            else
            {
                label1.Text = text;
            }
        }

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
            Gl.glLineWidth(1.0f);
        }

        public Form1()
        {
            InitializeComponent();
            Scene.InitializeContexts();
        }


        // Инициализация
        private void Form1_Load(object sender, EventArgs e)
        {
            OpenGlInit();
            DevIlInit();
            AnimationTimer.Start();

            logger = new StreamWriter("logger.txt", true);
            listener = new MyListener();
            listener.output += (string s) => { logger.WriteLine(s); };
                
            controller = new Controller();
            controller.AddListener(listener);
        }

        //Обработка преобразований и отрисока модели
        void Draw()
        {
            float xTranslation = TransformParameters.Translation[(int)Axis.x],
                  yTranslation = TransformParameters.Translation[(int)Axis.y],
                  zTranslation = TransformParameters.Translation[(int)Axis.z],
                  xRotation = TransformParameters.Rotation[(int)Axis.x],
                  yRotation = TransformParameters.Rotation[(int)Axis.y],
                  zRotation = TransformParameters.Rotation[(int)Axis.z],
                  xScaling = TransformParameters.Scaling[(int)Axis.x],
                  yScaling = TransformParameters.Scaling[(int)Axis.y],
                  zScaling = TransformParameters.Scaling[(int)Axis.z];

            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            Gl.glLoadIdentity();
            Gl.glColor3f(1.0f, 0, 0);
            Gl.glPushMatrix();
            Gl.glTranslated(xTranslation, yTranslation, zTranslation - 20);
            Gl.glRotated(xRotation, yRotation, zRotation, 1);
            Gl.glScaled(xScaling, yScaling, zScaling);
            Glut.glutSolidCube(2);
            Gl.glPopMatrix();
            Gl.glFlush();
            Scene.Invalidate();
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            Draw();
            TransformLock.Tick();
            label1.Text = PrepareString();

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            controller.RemoveListener(listener);
            controller.Dispose();
            logger.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            lock (thisLock)
            {
                for (int i = 0; i < TransformParameters.Translation.Length; i++)
                {
                    TransformParameters.Translation[i] = 0;
                    TransformParameters.Rotation[i] = 0;
                    TransformParameters.Scaling[i] = 1;
                }
            }
        }

        private string PrepareString()
        {
            string s = "Translation:{0}x: {1}; y: {2}; z: {3}{0}Rotations:{0}x: {4}; y: {5}; z: {6}{0}Scaling{0}x: {7}; y: {8}; z: {9}";
            s = string.Format(s, Environment.NewLine,
                TransformParameters.Translation[(int)Axis.x], TransformParameters.Translation[(int)Axis.y], TransformParameters.Translation[(int)Axis.z],
                TransformParameters.Rotation[(int)Axis.x], TransformParameters.Rotation[(int)Axis.y], TransformParameters.Rotation[(int)Axis.z],
                TransformParameters.Scaling[(int)Axis.x], TransformParameters.Scaling[(int)Axis.y], TransformParameters.Scaling[(int)Axis.y]);
            return s;
        }
    }
}
