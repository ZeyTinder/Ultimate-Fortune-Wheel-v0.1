using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UltimateFortuneWheel
{
    public partial class Form1 : Form
    {
        private const string APP_NAME = "Ultimate Fortune Wheel";
        private readonly string OUTPUT_DATA_PATH = $"{Environment.CurrentDirectory}\\dic1OutputData.json";
        public Dictionary<string, int> dic1 = new Dictionary<string, int>();
        public bool ifTrueDrawPieChart = false;
        float rotateAngle = 0.0f;
        string result;
        float startWinAngle;
        float endWinAngle;
        bool timerWork;


        public Form1()
        {
            InitializeComponent();
        }

        private Brush[] SliceBrushes =
        {
            Brushes.WhiteSmoke,
            Brushes.LightGreen,
            Brushes.Blue,
            Brushes.LightBlue,
            Brushes.Green,
            Brushes.Lime,
            Brushes.Orange,
            Brushes.Fuchsia,
            Brushes.Yellow,
            Brushes.Cyan,
        };
        public void PieChartDrawer(PaintEventArgs e, Rectangle rect, Brush[] brushes, string label_format, Font label_font, Brush label_brush)
        {
            if (ifTrueDrawPieChart == true)
            {
                Pen blackPen = new Pen(Color.Black, 2);
                SolidBrush redBrush = new SolidBrush(Color.Red);

                // Create start angles.
                float startAngle = 0f;
                // Create List of int values(percentages) from dic1
                List<int> arr = new List<int>(dic1.Values);
                // Create List of float values for List<> 
                List<float> floatDic1 = new List<float>();
                // Create List with Candidate names
                List<string> candidateName = new List<string>(dic1.Keys);
                // Create Final Sweep Angle for each value from float list floatDic1 
                List<float> floatSweepAngle = new List<float>();

                float sumOfArr = arr.Sum();

                foreach (var i in arr)
                {
                    float adder;
                    adder = i / sumOfArr;
                    floatDic1.Add(adder);
                }
                foreach (var i in floatDic1)
                {
                    float eachAngle;
                    eachAngle = i * 360;
                    floatSweepAngle.Add(eachAngle);
                }

                float sumOfAngle = floatSweepAngle.Sum();

                Dictionary<string, float> dic = candidateName.Zip(floatDic1, (k, v) => new { k, v })
                    .ToDictionary(x => x.k, x => x.v);

                int counter = 0;

                do
                {
                    if (dic1.Count < 0)
                    {
                        MessageBox.Show("No data found");
                        break;
                    }

                    e.Graphics.TranslateTransform((float)rect.Width / 2f, (float)rect.Height / 2f);
                    e.Graphics.RotateTransform(rotateAngle);
                    e.Graphics.TranslateTransform(-(float)rect.Width / 2f, -(float)rect.Height / 2f);

                    int a = 0;
                    foreach (var sweepAngle in floatSweepAngle)
                    {
                        a++;
                        e.Graphics.FillPie(brushes[a % brushes.Length], rect, startAngle, sweepAngle);
                        e.Graphics.DrawPie(blackPen, rect, startAngle, sweepAngle);
                        startAngle += sweepAngle;
                    }

                    //Draw a label for each pie
                    using (StringFormat string_format = new StringFormat())
                    {
                        // Center text.
                        string_format.Alignment = StringAlignment.Center;
                        string_format.LineAlignment = StringAlignment.Center;

                        // Find the center of the rectangle.
                        float cx = (rect.Left + rect.Right) / 2f;
                        float cy = (rect.Top + rect.Bottom) / 2f;
                        startAngle = 0;

                        // Place the label about 2/3 of the way out to the edge.
                        float radius = (rect.Width + rect.Height) / 2f * 0.33f;

                        foreach (KeyValuePair<string, float> i in dic)
                        {
                            var sweepAngle = i.Value * 360;
                            // Label the slice.
                            double label_angle =
                                Math.PI * (startAngle + sweepAngle / 2f) / 180f;
                            float x = cx + (float)(radius * Math.Cos(label_angle));
                            float y = cy + (float)(radius * Math.Sin(label_angle));
                            if (i.Value * 100 >= 5)
                            {
                                e.Graphics.DrawString(i.Key.ToString(), label_font, label_brush, x, y - 15, string_format);
                                e.Graphics.DrawString(i.Value.ToString(label_format),
                        label_font, label_brush, x, y, string_format);
                            }

                            if (result == i.Key)
                            {
                                startWinAngle = startAngle;
                                endWinAngle = startWinAngle + sweepAngle;
                            }
                            startAngle += sweepAngle;


                        }
                    }
                    counter++;

                } while (arr.Count == counter);
                using (GraphicsPath capPath = new GraphicsPath())
                {
                    e.Graphics.ResetTransform();
                    // A triangle
                    capPath.AddLine(-5, 0, 5, 0);
                    capPath.AddLine(-5, 0, 0, 5);
                    capPath.AddLine(0, 5, 5, 0);

                    blackPen.CustomEndCap = new System.Drawing.Drawing2D.CustomLineCap(null, capPath);

                    e.Graphics.DrawLine(blackPen, 0, 190, 1, 190);
                }

                counter = 0;
                ifTrueDrawPieChart = false;
            }

        }

        void Randomodf()
        {
            dic1.Add("test1", 1);
            dic1.Add("test2", 1);
            dic1.Add("test3", 1);
            dic1.Add("test4", 1);
            dic1.Add("test5", 1);
            dic1.Add("test6", 1);
            dic1.Add("test7", 1);
        }

        async void btnRoll_Click(object sender, EventArgs e)
        {
            timerWork = true;
            Randomizer();
            timer1.Start();
            timer1.Interval = 10;

            while(timerWork == true)
            {
                await Task.Delay(1);
            }

            DeleteFromDic1(result);

            Dic1ToJson();
        }

        public void btnAdd_Click(object sender, EventArgs e)
        {
            //changeDomainUD(10, 266);
            string pretenderName = textBox1.Text;
            string pretenderChance = textBox2.Text;
            int tempChance = 0;
            bool doesKeyAndValueAlreadyExist = false;

            // Check is there any data in textBox2, if yes Converts it to INT
            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("There is no data in here 2");
            }
            else if (textBox2.Text.Length != 0)
            {
                tempChance = Convert.ToInt32(pretenderChance);
            }

            int sum;

            // Check if Key in textBox1 already exist in dictionary(dic1)
            foreach (var dic1Keys in dic1)
            {
                if (pretenderName == dic1Keys.Key)
                {
                    //Adding value to the same key in dictionary
                    sum = dic1Keys.Value + tempChance;

                    dic1.Remove(dic1Keys.Key);
                    dic1.Add(pretenderName, sum);
                    chart1.Series["Series1"].Points.AddXY(pretenderName, tempChance);
                    RefreshChart1();
                    doesKeyAndValueAlreadyExist = true;
                    //MessageBox.Show("Thats name already exist");
                    break;
                }
            }
            // If Key in textBox1 does not exist in dic1, checks other conditions to have any names in textBox1, if exist add it to Dictionary and Chart
            if (doesKeyAndValueAlreadyExist == false)
            {
                if (textBox1.Text.Length != 0 && textBox2.Text.Length != 0)
                {
                    dic1.Add(pretenderName, tempChance);
                    chart1.Series["Series1"].Points.AddXY(pretenderName, tempChance);
                    //domainUpDown1 list create
                    foreach (KeyValuePair<string, int> exception in dic1)
                    {
                        if (!domainUpDown1.Items.Contains(exception.Key))
                            domainUpDown1.Items.Add(exception.Key);

                    }
                }
                else if (textBox1.Text.Length == 0 && textBox2.Text.Length == 0)
                {
                    MessageBox.Show("There is no data in here");
                }
                else
                {
                    MessageBox.Show("Something work wrong");
                }
            }
            redrawWheel();
            Dic1ToJson();
        }

        void DltBtn_Click(object sender, EventArgs e)
        {
            string delName = textBoxDelete.Text;

            DeleteFromDic1(delName);
            domainUpDown1.Items.Remove(delName);

            MessageBox.Show($"Deleted: {delName}");

        }

        void Form1_Load(object sender, EventArgs e)
        {
            this.Text = APP_NAME;

            try
            {
                JsonToDic1();
                Randomodf();
                DicToChart();
                CreatingDomainUDFromDic();

            }
            catch (Exception exTryCatch)
            {
                MessageBox.Show(exTryCatch.Message);
            }
            finally
            {

            }
        }

        void DicToChart()
        {
            chart1.Titles.Add("Pretenders");

            // Creates Pie Chart from data in dic1
            RefreshChart1();

            chart1.Series["Series1"].Label = "#AXISLABEL \n #PERCENT";
            chart1.Series["Series1"].IsVisibleInLegend = false;
        }

        public void Randomizer()
        {
            Random winner = new Random();
            int participants = dic1.Count;

            result = "";
            var totalWeight = 0;

            foreach (var dicNumber in dic1)
                totalWeight += dicNumber.Value;

            var randNumber = winner.Next(totalWeight);

            foreach (var dicNumber in dic1)
            {
                var value = dicNumber.Value;

                if (randNumber >= value)
                {
                    randNumber -= value;
                }
                else
                {
                    result = dicNumber.Key;
                    break;
                }
            }

            if (dic1.ContainsKey(result))
            {
                int lastWinner = dic1[result];

                //Foreach delete winner from dictionary/chart

                DomainUpDownTextBoxSelect(participants, result);

                MessageBox.Show($"Winner is: {result} with chance of: {lastWinner}%");

            }
        }

        void Dic1ToJson()
        {
            File.WriteAllText(OUTPUT_DATA_PATH, JsonConvert.SerializeObject(dic1));
        }

        void JsonToDic1()
        {
            var data = File.ReadAllText(OUTPUT_DATA_PATH);

            data = Regex.Unescape(data);
            data = data.Replace("\n", "");
            data = data.Replace("\r", "");
            data = data.TrimStart('\"');
            data = data.TrimEnd('\"');
            data = data.Replace("\\", "");

            dic1 = JsonConvert.DeserializeObject<Dictionary<string, int>>(data);
        }

        public void RefreshChart1()
        {
            chart1.Series["Series1"].Points.Clear();

            foreach (KeyValuePair<string, int> exception in dic1)
                chart1.Series["Series1"].Points.AddXY(exception.Key, exception.Value);
        }

        void DeleteFromDic1(string keyToDelete)
        {
            foreach (var del in dic1)
            {
                if (del.Key == keyToDelete)
                {
                    dic1.Remove(del.Key);
                    //domainUpDown1.Items.Remove(del.Key);
                    RefreshChart1();
                    break;
                }
            }
        }

        void DomainUpDownTextBoxSelect(int allKeys, string winner)
        {
            if (domainUpDown1.Items.Count >= allKeys)
            {
                int totalDomainItems = domainUpDown1.Items.Count;

                for (int j = 0; j < totalDomainItems; j++)
                    if (domainUpDown1.Items[j].ToString() == winner)
                    {
                        domainUpDown1.SelectedIndex = j;
                        domainUpDown1.Items.Remove(winner);
                        break;
                    }
            }
        }

        void CreatingDomainUDFromDic()
        {
            if (dic1.Count >= 1)
            {
                foreach (KeyValuePair<string, int> exception in dic1)
                {
                    if (!domainUpDown1.Items.Contains(exception.Key))
                        domainUpDown1.Items.Add(exception.Key);

                }
            }
        }

        void changeDomainUD(int X, int Y)
        {
            Point Point1 = new Point(X, Y);

            domainUpDown1.Location = Point1;
        }
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Rectangle rect = new Rectangle(
        0, 0, 380, 380);

            PieChartDrawer(e, rect, SliceBrushes, "0.00%", Font, Brushes.Black);
        }

        private void DltDomainButton_Click(object sender, EventArgs e)
        {
            string str = domainUpDown1.Text;
            DeleteFromDic1(str);
            domainUpDown1.Items.Remove(str);
            redrawWheel();
        }
        private void redrawWheel()
        {
            ifTrueDrawPieChart = true;
            pictureBox1.Refresh();
        }

        private float RandomSweep(float sweep)
        {
            Random random = new Random();
            int i = random.Next(0, 7);
            float t = (float)i;
            return sweep * 2 / t;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            redrawWheel();
            float reshala = (endWinAngle - startWinAngle) / 2;
            float middleWinAngle = (360f - endWinAngle) + 180 + reshala;
            float endMid = middleWinAngle + reshala;
            float startMid = middleWinAngle - reshala;
            float valueOfRotate= 0.0f;
            valueOfRotate += 4.0f;
            if (rotateAngle <= endMid - 200.0f && rotateAngle >= 50.0f)
            {
                valueOfRotate -= 1.3f;
            }
            else if(rotateAngle <= endMid - 150.0f && rotateAngle >= 60.0f)
            {
                valueOfRotate -= 1.4f;

            }
            else if(rotateAngle <= endMid - 100.0f && rotateAngle >= 100.0f)
            {
                valueOfRotate -= 1.5f;

            }
            else if(rotateAngle <= endMid && rotateAngle >= 150.0f)
            {
                valueOfRotate -= 1.6f;
            }
            rotateAngle += valueOfRotate;
            if (rotateAngle >= 3600f)
            {
                rotateAngle = 0f;
                timer1.Stop();
            }

            if (rotateAngle <= endMid && rotateAngle >= startMid + RandomSweep(reshala))
            {
                timer1.Stop();
                ifTrueDrawPieChart = false;
                rotateAngle = 0f;
                timerWork = false;
            }
        }

    }
}