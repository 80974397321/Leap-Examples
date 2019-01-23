using System;
using System.Windows.Forms;
using Leap;

namespace LeapSetup
{
    public partial class Form1 : Form
    {
        Controller controller;
        MyListener listener;
        Vector[][] fingerBonesPositions;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            controller = new Controller();
            listener = new MyListener();
            controller.AddListener(listener);

            dataGridView1.Rows.Add(FingerInfo.fingersInHand);
            dataGridView1.Rows[0].Cells[0].Value = "Великий";
            dataGridView1.Rows[1].Cells[0].Value = "Вказівний";
            dataGridView1.Rows[2].Cells[0].Value = "Середній";
            dataGridView1.Rows[3].Cells[0].Value = "Вказівний";
            dataGridView1.Rows[4].Cells[0].Value = "Мізинець";

            RefreshTimer.Start();
        }



        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            controller.RemoveListener(listener);
            RefreshTimer.Stop();
        }

        // Выводим зафиксированные данные о положении пальцев
        private void Refresh_Tick(object sender, EventArgs e)
        {
            fingerBonesPositions = new Vector[][] { FingerInfo.Thumb, FingerInfo.Index, FingerInfo.Middle, FingerInfo.Ring, FingerInfo.Pinky };
            for (int i = 0; i < fingerBonesPositions.Length; i++)
            {
                if (fingerBonesPositions[i] == null)
                    continue;
                
                for (int j = 0; j < fingerBonesPositions[i].Length; j++)
                {
                    lock(fingerBonesPositions[i])
                    {
                        dataGridView1.Rows[i].Cells[j + 1].Value = fingerBonesPositions[i][j];
                    }
                }
            }
        }
    }
}
