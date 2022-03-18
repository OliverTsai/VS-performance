using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

namespace WindowsFormsApp7
{
    public partial class MonitoringTools : Form
    {
        private Thread cpuThread;
        private Thread memoryThread;
        private double[] cpuArray = new double[60];
        private double[] memoryArray = new double[60];
        public MonitoringTools()
        {
            InitializeComponent();

            this.helloTab.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.helloTab.DrawItem += new DrawItemEventHandler(this.helloTab_DrawItem);

            chart1.Titles.Add("CLOUDFLARE");
            chart1.Series["Series1"].Points.AddXY("1", "10");
            chart1.Series["Series1"].Points.AddXY("2", "20");
            chart1.Series["Series1"].Points.AddXY("3", "30");

            cpuThread = new Thread(new ThreadStart(this.getPerformanceCounters));
            cpuThread.IsBackground = true;
            cpuThread.Start();

            memoryThread = new Thread(new ThreadStart(this.getPerformanceCounters));
            memoryThread.IsBackground = true;
            memoryThread.Start();
        }

        private void helloTab_DrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e)
        {
            Console.WriteLine("helloTab_DrawItem ENTER");

            TabPage CurrentTab = helloTab.TabPages[e.Index];
            Rectangle ItemRect = helloTab.GetTabRect(e.Index);
            SolidBrush FillBrush = new SolidBrush(Color.White);
            SolidBrush TextBrush = new SolidBrush(Color.FromArgb(255, 102, 0));
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;

            //If we are currently painting the Selected TabItem we'll
            //change the brush colors and inflate the rectangle.
            if (System.Convert.ToBoolean(e.State & DrawItemState.Selected))
            {
                FillBrush.Color = Color.FromArgb(255,102,0);
                TextBrush.Color = Color.White;
                ItemRect.Inflate(2, 2);
            }

            //Set up rotation for left and right aligned tabs
            if (helloTab.Alignment == TabAlignment.Left || helloTab.Alignment == TabAlignment.Right)
            {
                float RotateAngle = 90;
                if (helloTab.Alignment == TabAlignment.Left)
                    RotateAngle = 270;
                PointF cp = new PointF(ItemRect.Left + (ItemRect.Width / 2), ItemRect.Top + (ItemRect.Height / 2));
                e.Graphics.TranslateTransform(cp.X, cp.Y);
                e.Graphics.RotateTransform(RotateAngle);
                ItemRect = new Rectangle(-(ItemRect.Height / 2), -(ItemRect.Width / 2), ItemRect.Height, ItemRect.Width);
            }

            //Next we'll paint the TabItem with our Fill Brush
            e.Graphics.FillRectangle(FillBrush, ItemRect);

            //Now draw the text.
            e.Graphics.DrawString(CurrentTab.Text, e.Font, TextBrush, (RectangleF)ItemRect, sf);

            //Reset any Graphics rotation
            e.Graphics.ResetTransform();

            //Finally, we should Dispose of our brushes.
            FillBrush.Dispose();
            TextBrush.Dispose();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("CCTV");
        }

        private void pictureBox11_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Call Whatsapp ...");
        }

        private void pictureBox12_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Call Remote Tools");
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("CCTV");
        }
        private void getPerformanceCounters()
        {
            var cpuPerfCounter = new PerformanceCounter("Processor Information", "% Processor Time", "_Total");
            var memoryPerfCounter = new PerformanceCounter("Memory", "% Committed Bytes in Use");

            while (true)
            {
                cpuArray[cpuArray.Length - 1] = Math.Round(cpuPerfCounter.NextValue(), 0);
                memoryArray[memoryArray.Length - 1] = Math.Round(memoryPerfCounter.NextValue(), 0);

                Array.Copy(cpuArray, 1, cpuArray, 0, cpuArray.Length - 1);
                Array.Copy(memoryArray, 1,memoryArray, 0, memoryArray.Length -1);

                if (cpuChart.IsHandleCreated)
                {
                    this.Invoke((MethodInvoker)delegate { UpdateCpuChart(); });
                }
                else
                {
                    //......
                }

                if (memoryChart.IsHandleCreated)
                {
                    this.Invoke((MethodInvoker)delegate { UpdateMemoryChart(); });
                }
                else
                {
                    //......
                }

                Thread.Sleep(1000);
            }
        }

        private void UpdateCpuChart()
        {
            cpuChart.Series["Series1"].Points.Clear();

            for (int i = 0; i < cpuArray.Length - 1; ++i)
            {
                cpuChart.Series["Series1"].Points.AddY(cpuArray[i]);
            }
        }
        private void UpdateMemoryChart()
        {
            memoryChart.Series["Series2"].Points.Clear();

            for(int i = 0; i <= memoryArray.Length - 1; i++)
            {
                memoryChart.Series["Series2"].Points.AddY(memoryArray[i]);
            }
        }
    }
}
