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
        private double RunTime = 20;

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
        
        public Visualizer()
        {
            InitializeComponent();

            Hide();
            
            dateTime = DateTime.Now;
            correctionState = false;
            
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += new DoWorkEventHandler(BackgroundWorker_CalculateFourierTransforms);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackgroundWorker_ChangeFourierTransforms);

            //StartTimers();
            //StopTimers();

            Vector point = new Vector(110, -50, 60);
            Matrix rotation = new Matrix(point);

            //rotation.ReadjustMatrix();

            rotation.Rotate(new Vector(60, 60, 30));

            /*

            rotation.RotateX(60);// single axis rotation works, multiple axis rotation does not
            
            Console.WriteLine(rotation.ToString());

            Console.WriteLine(rotation.ConvertCoordinateToVector().ToString());

            rotation.ReadjustMatrix();

            rotation.RotateY(60);

            Console.WriteLine(rotation.ToString());

            Console.WriteLine(rotation.ConvertCoordinateToVector().ToString());

            rotation.ReadjustMatrix();

            rotation.RotateZ(30);
            */

            Console.WriteLine(rotation.ToString());

            Console.WriteLine(rotation.ConvertCoordinateToVector().ToString());
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
                    Interval = 60, //In milliseconds here
                    AutoReset = true //Stops it from repeating
                };
                t1.Elapsed += new ElapsedEventHandler(SetInvertedPendulumAngle);
                //t1.Start();
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
        /// Why FFT?
        /// -Displays change of frequency of pendulum, slowing/speeding up
        /// -Displays noise effectively from output
        /// -Displays switching frequency of feedback controller
        /// 
        /// Updates the Fourier Transform charts and 2d memory displays.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void UpdateFourierTransforms(object sender, ElapsedEventArgs e)
        {
            if (!(DateTime.Now.Subtract(dateTime).TotalSeconds > RunTime))
            {
                this.BeginInvoke((Action)(() =>
                {

                }));
            }
        }
        
        /// <summary>
        /// Changes the angle of the pendulums and calculates the corrections for the feedback controllers.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ChangeAngle(object sender, ElapsedEventArgs e)
        {
            if (DateTime.Now.Subtract(dateTime).TotalSeconds > WaitTimeForCalculation)
            {

            }
        }
        
        /// <summary>
        /// Updates the diplay of the inverted pendulums and the charts.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SetInvertedPendulumAngle(object sender, ElapsedEventArgs e)
        {
            if (!(DateTime.Now.Subtract(dateTime).TotalSeconds > RunTime))
            {
                this.BeginInvoke((Action)(() =>
                {

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
