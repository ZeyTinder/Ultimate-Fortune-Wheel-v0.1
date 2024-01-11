using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms;

namespace KolesoFirstTry
{
    public partial class PieChart : Form1
    {

        Form1 f1 = new Form1();

        public void DrawPieFloat(PaintEventArgs e)
        {

            //if (ifTrueDrawPieChart == true)
            //{
                PieChartDrawer(e);
                chart1.Width = 200;
            //}
        }


        public void PieChartDrawer(PaintEventArgs e)
        {
            // Create pen.
            Pen blackPen = new Pen(Color.Black, 3);

            // Create location and size of ellipse.
            int x = 290;
            int y = 50;
            int width = 360;
            int height = 360;

            // Create start and sweep angles.
            int startAngle = 0;
            int sweepAngle = 0;

            List<int> arr = new List<int>(dic1.Values);

            // Draw pie to screen.
            e.Graphics.DrawPie(blackPen, x, y, width, height, startAngle, sweepAngle);

            int totalDegree = 360;
            //float totalDegree = (width*2)-startAngle;

            //float kal = startAngle - (sweepAngle / 2);
            int counter = 0;
            do
            {
                if (dic1.Count > 0)
                {

                    foreach (var i in arr)
                    {
                        //totalDegree -= sweepAngle;
                        startAngle += sweepAngle;
                        sweepAngle = i;

                        if (startAngle + sweepAngle >= totalDegree)
                        {
                            totalDegree -= startAngle;
                            e.Graphics.DrawPie(blackPen, x, y, width, height, startAngle, totalDegree);

                            break;
                        }
                        e.Graphics.DrawPie(blackPen, x, y, width, height, startAngle, sweepAngle);
                    }
                }
                else if (dic1.Count <= 0)
                {
                    MessageBox.Show("No data found");
                    break;
                }
                else
                {
                    MessageBox.Show("Error");
                    break;
                }

                counter++;

            } while (dic1.Count == counter && startAngle <= totalDegree);

            counter = 0;
        }


    }
}