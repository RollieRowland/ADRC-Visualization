using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using ADRCVisualization.Class_Files;
using System.Drawing.Imaging;
using System.IO;
using ADRCVisualization.Class_Files.Mathematics;

namespace ADRCVisualization
{
    public partial class Visualizer : Form
    {
        private BackgroundWorker backgroundWorker;
        private DateTime dateTime;
        private bool correctionState;
        
        //FeedbackControllers
        private double maxOutput = 1000;
        private bool initializeFeedbackControllers = false;
        private double WaitTimeForCalculation = 5;
        private double RunTime = 60;

        //Active Disturbance Rejection Control Parameters
        private ADRC_PD adrc;
        private double r = 2000;//80
        private double c = 500;//500
        private double b = 2.875;//0.5   smoothing
        private double hModifier = 0.00085;//0005   overshoot

        //Timers for alternate threads and asynchronous calculations
        private System.Timers.Timer t1;

        //Fourier Transforms
        private float FourierTolerance = 1f;

        private Vector gravity =  new Vector(0, -9.81, 0);
        private Quadcopter quad = new Quadcopter(0.3, 45);

        private Vector targetPosition;
        private Vector targetRotation;

        public Visualizer()
        {
            InitializeComponent();
            
            dateTime = DateTime.Now;
            correctionState = false;
            
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += new DoWorkEventHandler(BackgroundWorker_CalculateFourierTransforms);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackgroundWorker_ChangeFourierTransforms);

            StartTimers();
            StopTimers();
            
            //Set current
            quad.SetCurrent(new Vector(0, 0, 0), new Vector(0, 0, 0));

            targetPosition = new Vector(1, -1, 1.2);
            targetRotation = new Vector(0, 0, 0);
            //Set target
            quad.SetTarget(targetPosition, targetRotation);

            SetTargets();
        }

        private async void SetTargets()
        {
            await Task.Delay(3000);

            //gravity.X = -0.5;
            //gravity.Z = 0.5;

            targetPosition = new Vector(-1, 0, 1.2);
            targetRotation = new Vector(60, 0, 0);
            Console.WriteLine("Target Set");
            
            await Task.Delay(3000);
            targetPosition = new Vector(1, 1, -1.2);
            targetRotation = new Vector(0, 60, 0);
            Console.WriteLine("Target Set");

            await Task.Delay(4500);

            targetPosition = new Vector(-1, -1, -1.2);
            targetRotation = new Vector(0, 0, 60);
            Console.WriteLine("Target Set");
        }


        private void SetChartPositions(Tuple<Vector, Vector, Vector, Vector> positions, Tuple<Vector, Vector, Vector,Vector> targetPositions, Vector centralPosition)
        {

            chart1.ChartAreas[0].AxisX.Maximum = 2;
            chart1.ChartAreas[0].AxisX.Minimum = -2;
            chart1.ChartAreas[0].AxisY.Maximum = 2;
            chart1.ChartAreas[0].AxisY.Minimum = -2;

            chart2.ChartAreas[0].AxisY.Maximum = 10;
            chart2.ChartAreas[0].AxisY.Minimum = -10;

            chart1.Series[0].Points.Clear();
            chart1.Series[1].Points.Clear();
            chart1.Series[2].Points.Clear();
            chart1.Series[3].Points.Clear();
            chart1.Series[4].Points.Clear();
            chart1.Series[5].Points.Clear();
            chart1.Series[6].Points.Clear();
            chart1.Series[7].Points.Clear();
            chart1.Series[8].Points.Clear();
        
            chart1.Series[1].MarkerColor = Color.BurlyWood;
            chart1.Series[2].MarkerColor = Color.BlueViolet;
            chart1.Series[3].MarkerColor = Color.ForestGreen;
            chart1.Series[4].MarkerColor = Color.MediumAquamarine;
            chart1.Series[5].MarkerColor = Color.BurlyWood;
            chart1.Series[6].MarkerColor = Color.BlueViolet;
            chart1.Series[7].MarkerColor = Color.ForestGreen;
            chart1.Series[8].MarkerColor = Color.MediumAquamarine;


            chart1.Series[0].Points.AddXY(centralPosition.X, centralPosition.Z);
            chart1.Series[1].Points.AddXY(positions.Item1.X, positions.Item1.Z);
            chart1.Series[2].Points.AddXY(positions.Item2.X, positions.Item2.Z);
            chart1.Series[3].Points.AddXY(positions.Item3.X, positions.Item3.Z);
            chart1.Series[4].Points.AddXY(positions.Item4.X, positions.Item4.Z);

            chart1.Series[0].MarkerSize = (int)centralPosition.Y + 5 * 2;
            chart1.Series[1].MarkerSize = (int)positions.Item1.Y + 5 * 2;
            chart1.Series[2].MarkerSize = (int)positions.Item2.Y + 5 * 2;
            chart1.Series[3].MarkerSize = (int)positions.Item3.Y + 5 * 2;
            chart1.Series[4].MarkerSize = (int)positions.Item4.Y + 5 * 2;

            chart1.Series[5].Points.AddXY(targetPositions.Item1.X, targetPositions.Item1.Z);
            chart1.Series[6].Points.AddXY(targetPositions.Item2.X, targetPositions.Item2.Z);
            chart1.Series[7].Points.AddXY(targetPositions.Item3.X, targetPositions.Item3.Z);
            chart1.Series[8].Points.AddXY(targetPositions.Item4.X, targetPositions.Item4.Z);

            chart2.Series[0].Points.Clear();

            chart2.Series[0].Points.Add(centralPosition.Y);
            chart2.Series[0].Points.Add(targetPosition.Y);
        }
        
        /// <summary>
        /// Starts alternate threads for calculation of the inverted pendulum and updating the display of the user interface for the FFTWs, pendulum, and graphs.
        /// </summary>
        private async void StartTimers()
        {
            await Task.Delay(50);

            this.BeginInvoke((Action)(() =>
            {
                t1 = new System.Timers.Timer
                {
                    Interval = 30, //In milliseconds here
                    AutoReset = true //Stops it from repeating
                };
                t1.Elapsed += new ElapsedEventHandler(Calculate);
                t1.Start();
            }));
        }

        /// <summary>
        /// Stops the secondary threads to end the calculation.
        /// </summary>
        private async void StopTimers()
        {
            await Task.Delay((int)RunTime * 1000);

            this.BeginInvoke((Action)(() =>
            {
                t1.Stop();
            }));
        }
        
        /// <summary>
        /// Updates the diplay of the quadcopter and the charts.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Calculate(object sender, ElapsedEventArgs e)
        {
            if (!(DateTime.Now.Subtract(dateTime).TotalSeconds > RunTime))
            {
                this.BeginInvoke((Action)(() =>
                {
                    //Calculate
                    //quad.CalculateIndividualThrustVectors();//Initial Solver
                    quad.CalculateCombinedThrustVector();//Secondary Solver

                    //Vector currentForce = quad.EstimateAcceleration(gravity);//force acting on quadcopter w/ quadcopter force
                    Vector currentAcceleration = quad.AccelerationNoGravity();//force from quadcopter
                    Vector currentPosition = quad.EstimatePosition(0.1);//time frame between movements
                    Vector currentRotation = quad.EstimateRotation(0.5);
                    //Tuple<Vector, Vector> positionAndRotation = quad.EstimatePositionAndRotation(0.1);

                    quad.SetTarget(targetPosition, targetRotation);
                    quad.SetCurrent(currentPosition, currentRotation);
                    //quad.SetCurrent(positionAndRotation.Item1, positionAndRotation.Item2);

                    //Console.Write("Target: " + targetRotation.ToString() + " ");
                    //Console.Write("Quad Position: " + currentPosition.ToString() + " ");
                    //Console.Write("Force: " + currentAcceleration + " ");

                    Tuple<Vector, Vector, Vector, Vector> motorPositions = quad.GetMotorPositions();
                    Tuple<Vector, Vector, Vector, Vector> motorOrientations = quad.GetMotorOrientations();
                    Tuple<Vector, Vector, Vector, Vector> targetPositions = quad.GetMotorTargetPositions();

                    SetChartPositions(motorPositions, targetPositions, currentPosition);

                    label1.Text = Vector.CalculateEuclideanDistance(motorPositions.Item1, targetPositions.Item1).ToString();
                    label2.Text = Vector.CalculateEuclideanDistance(motorPositions.Item2, targetPositions.Item2).ToString();
                    label3.Text = Vector.CalculateEuclideanDistance(motorPositions.Item3, targetPositions.Item3).ToString();
                    label4.Text = Vector.CalculateEuclideanDistance(motorPositions.Item4, targetPositions.Item4).ToString();

                    //Console.Write(motorPositions.Item1.ToString());
                    //Console.WriteLine();
                }));
            }
        }

        /// <summary>
        /// Calculates the fourier transforms and updates the axis scales.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackgroundWorker_CalculateFourierTransforms(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;
            

            this.BeginInvoke((Action)(() =>
            {

            }));

            //e.Result = new float[4][] { pidFFTW, adrcFFTW, pidAngleFFTW, adrcAngleFFTW };
        }

        /// <summary>
        /// Updates the fourier transform charts and the 2d memory displays.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackgroundWorker_ChangeFourierTransforms(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        private void Visualizer_FormClosing(object sender, FormClosingEventArgs e)
        {
            //t1.Stop();
        }
    }
}
