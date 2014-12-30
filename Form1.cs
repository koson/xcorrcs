using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MathNet.Numerics.Statistics;
using MathNet.Numerics.UnitTests.StatisticsTests;
using ZedGraph;
namespace xcorrcs
{
    public partial class Form1 : Form
    {
        List<double> x = new List<double>();
        List<double> y = new List<double>();
        List<double> t = new List<double>();

        GraphPane myPane = new GraphPane();

        int lag;

        // poing pair lists
        PointPairList listPointsOne = new PointPairList();
        PointPairList listPointsTwo = new PointPairList();
        PointPairList listPointsX = new PointPairList();
        PointPairList listPointsY = new PointPairList();

        // line item
        StickItem myCurveOne;
        readonly IDictionary<string, StatTestData> _data = new Dictionary<string, StatTestData>();


        public Form1()
        {
            InitializeComponent();
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            listPointsOne.Clear();
            listPointsX.Clear();
            listPointsY.Clear();


            x.Clear();
            x.Capacity = 100;
            for (int i = 0; i < y.Count; i++) // Loop through List with for
            {
                if (i < lag)
                    x.Add(2);
                else
                    x.Add(y[i - lag]);
            }
            for (int i = 0; i < x.Count; i++)
            {
                ListViewItem itm;
                double rxy0 = cal_rxy(i);
                //Console.WriteLine("rxy{0} = {1}", i, rxy0);
                string[] arr = new string[2];
                arr[0] = i.ToString();
                arr[1] = rxy0.ToString();
                itm = new ListViewItem(arr);
                listView1.Items.Add(itm);
                listPointsOne.Add(i, rxy0);
                listPointsX.Add(i, x[i]);
                listPointsY.Add(i, y[i]);
            }

            myPane.CurveList.Clear();
            // Add a smoothed curve
            LineItem curve1 = myPane.AddStick("correlation (lag)", listPointsOne, Color.Red);
            LineItem curve2 =   myPane.AddCurve("X", listPointsX, Color.Green);
            LineItem curve3 =   myPane.AddCurve("Y", listPointsY, Color.Blue);
            //curve1.Symbol.Fill = new Fill(Color.White);
            //curve1.Symbol.Size = 5;
            // activate the cardinal spline smoothing
            //curve1.Line.IsSmooth = true;
            //curve1.Line.SmoothTension = 0.5F;

            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
            zedGraphControl1.Refresh();

        }

        private double cal_rxy(int lag)
        {
            double rxy = 0;
            t = y.GetRange(0, y.Count);
            for (int i = 0; i < lag; i++) // Loop through List with for
            {
                t.Insert(i, 0);
            }

            while (t.Count > 100)
            {
                t.RemoveAt(100);
            }

            //for (int i = 0; i < t.Count; i++) // Loop through List with for
            //{
            //    Console.WriteLine(t[i]);
            //}

            for (int i = 0; i < x.Count; i++) // Loop through List with for
            {
                //                Console.WriteLine("x{0} * y{1} = {2}", i, i, x[i] * y[i]);
                rxy += x[i] * t[i];
            }

            var dataX = x;
            var dataY = t;
            var corr = Correlation.Pearson(dataY, dataX);
            //Console.WriteLine(corr.ToString());
            return corr;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lag = 30;
            //numericUpDown1.Value = lag;

            for (int i = 0; i < 100; i++)
            {

                if (i < 5)
                    y.Add(2);
                if (i >= 5 && i < 10)
                    y.Add(4);
                if (i >= 10)
                    y.Add(2);
            }
            for (int i = 0; i < 100; i++)
            {
                if (i < (5 + lag))
                    x.Add(2);
                if (i >= (5 + lag) && i < (10 + lag))
                    x.Add(4);
                if (i >= (10 + lag))
                    x.Add(2);
            }

            for (int i = 0; i < x.Count; i++)
            {
                ListViewItem itm;
                string[] arr = new string[3];
                arr[0] = i.ToString();
                arr[1] = x[i].ToString();
                arr[2] = y[i].ToString();
                itm = new ListViewItem(arr);
                listPointsTwo.Add(x[i], y[i]);
                listView2.Items.Add(itm);
            }

            myPane = zedGraphControl1.GraphPane;

            // set a title
            myPane.Title.Text = "Lag plot";

            // set X and Y axis titles
            myPane.XAxis.Title.Text = "X Axis";
            myPane.YAxis.Title.Text = "Y Axis";

            zedGraphControl1.IsEnableVZoom = false;
            zedGraphControl1.IsEnableVPan = false;
            // ---- CURVE ONE ----
            // draw a sin curve
            //for (int i = 0; i < 100; i++)
            //{
            //    listPointsOne.Add(i, Math.Sin(i));
            //}

            // set lineitem to list of points
          //  myCurveOne = myPane.AddStick("Some Sticks", listPointsOne, Color.Blue);


            // ---------------------

            // delegate to draw
            //zedGraphControl1.AxisChange();
            label1.Text = "Lag : 0";
            button1_Click(null, null);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //CreateChart(zedGraphControl2);
        }

        // Call this method from the Form_Load method, passing your ZedGraphControl
        public void CreateChart(ZedGraphControl zgc)
        {
            GraphPane myPane = zgc.GraphPane;

            // Set the titles
            myPane.Title.Text = "Scatter Plot Demo";
            myPane.XAxis.Title.Text = "Pressure, Atm";
            myPane.YAxis.Title.Text = "Flow Rate, cc/hr";

            // Get a random number generator
            Random rand = new Random();

            // Populate a PointPairList with a log-based function and some random variability
            PointPairList list = new PointPairList();
            for (int i = 0; i < 200; i++)
            {
                double x = rand.NextDouble() * 20.0 + 1;
                double y = Math.Log(10.0 * (x - 1.0) + 1.0) * (rand.NextDouble() * 0.2 + 0.9);
                list.Add(x, y);
            }

            // Add the curve
            LineItem myCurve = myPane.AddCurve("Performance", listPointsTwo, Color.Black);
            // Don't display the line (This makes a scatter plot)
            myCurve.Line.IsVisible = false;
            // Hide the symbol outline
            //myCurve.Symbol.Border.IsVisible = false;
            // Fill the symbol interior with color
            //myCurve.Symbol.Fill = new Fill(Color.Firebrick);

            // Fill the background of the chart rect and pane
            myPane.Chart.Fill = new Fill(Color.White, Color.LightGoldenrodYellow, 45.0f);
            myPane.Fill = new Fill(Color.White, Color.SlateGray, 45.0f);

            zgc.AxisChange();
            zgc.Invalidate();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var lottery = new StatTestData("./data/NIST/Lottery.dat");
            _data.Add("lottery", lottery);
            var lew = new StatTestData("./data/NIST/Lew.dat");
            _data.Add("lew", lew);
            PearsonCorrelationTest();

        }

        /// <summary>
        /// Pearson correlation test.
        /// </summary>

        public void PearsonCorrelationTest()
        {
            var dataA = _data["lottery"].Data.Take(200);
            var dataB = _data["lew"].Data.Take(200);

            var corr = Correlation.Pearson(dataA, dataB);

        }

        private void button4_Click(object sender, EventArgs e)
        {

            for (int i = 0; i < x.Count; i++)
            {
                ListViewItem itm;

                var dataX = x;
                var dataY = y;
                var corr = Correlation.Pearson(dataX, dataY);


                //double rxy0 = cal_rxy(i);
                //Console.WriteLine("rxy{0} = {1}", i, rxy0);

                string[] arr = new string[2];
                arr[0] = i.ToString();
                arr[1] = corr.ToString();
                itm = new ListViewItem(arr);
                listView1.Items.Add(itm);
                listPointsOne.Add(i, corr);
                //listPointsX.Add(i, x[i]);
                //listPointsY.Add(i, y[i]);


            }


            // Enter some arbitrary data
            double[] a = { 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000 };
            double[] b = { 20, 10, 50, 25, 35, 75, 90, 40, 33, 50 };

            // Add a smoothed curve
            LineItem curve = myPane.AddCurve("Smooth (Tension=0.5)", listPointsOne, Color.Red);
            //myPane.AddCurve("Smooth (Tension=0.5)", listPointsX, Color.Green);
            //myPane.AddCurve("Smooth (Tension=0.5)", listPointsY, Color.Blue);
            //curve.Symbol.Fill = new Fill(Color.White);
            //curve.Symbol.Size = 5;
            // activate the cardinal spline smoothing
            curve.Line.IsSmooth = true;
            curve.Line.SmoothTension = 0.5F;

            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            //lag = Convert.ToInt32(numericUpDown1.Value);

            //y.Clear();
            //y = x.GetRange(lag, x.Count);

            //myPane.CurveList.Clear();
            //button1_Click(null, null);
        }

        private void numericUpDown1_ValueChanged_1(object sender, EventArgs e)
        {
            //lag = Convert.ToInt32(numericUpDown1.Value);
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            lag = trackBar1.Value;
            label1.Text = "Lag : " + trackBar1.Value.ToString();
            button1_Click(null, null);
        }
    }
}
