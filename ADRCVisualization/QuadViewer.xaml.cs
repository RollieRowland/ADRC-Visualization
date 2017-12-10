using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ADRCVisualization.Class_Files;
using System.Timers;
using System.Windows.Threading;
using HelixToolkit.Wpf;
using System.Windows.Media.Media3D;
using ADRCVisualization.Class_Files.Mathematics;

namespace ADRCVisualization
{
    /// <summary>
    /// Interaction logic for QuadViewer.xaml
    /// </summary>
    public partial class QuadViewer : UserControl
    {
        private Quadcopter quadcopter;

        private Timer t1;

        private ModelVisual3D mainX;
        private ModelVisual3D mainY;
        private ModelVisual3D mainZ;

        private ModelVisual3D innerB;
        private ModelVisual3D outerB;

        private ModelVisual3D innerC;
        private ModelVisual3D outerC;

        private ModelVisual3D innerD;
        private ModelVisual3D outerD;

        private ModelVisual3D innerE;
        private ModelVisual3D outerE;

        private Vector mainPrevious;

        private Vector bPrevious;
        private Vector cPrevious;
        private Vector dPrevious;
        private Vector ePrevious;


        public QuadViewer()
        {
            InitializeComponent();

            ModelImporter import = new ModelImporter();

            string directory = @"C:\Users\steve\source\repos\ADRC-Visualization\ADRCVisualization\Resources\";
            //directory = @"..\..\Resources\";

            Model3D mainModel = import.Load(directory + "Main.stl");

            Model3D innerBModel = import.Load(directory + "InnerB.stl");
            Model3D outerBModel = import.Load(directory + "OuterB.stl");

            Model3D innerCModel = import.Load(directory + "InnerC.stl");
            Model3D outerCModel = import.Load(directory + "OuterC.stl");

            Model3D innerDModel = import.Load(directory + "InnerD.stl");
            Model3D outerDModel = import.Load(directory + "OuterD.stl");

            Model3D innerEModel = import.Load(directory + "InnerE.stl");
            Model3D outerEModel = import.Load(directory + "OuterE.stl");

            mainX = new ModelVisual3D();
            mainY = new ModelVisual3D();
            mainZ = new ModelVisual3D();

            innerB = new ModelVisual3D();
            outerB = new ModelVisual3D();

            innerC = new ModelVisual3D();
            outerC = new ModelVisual3D();

            innerD = new ModelVisual3D();
            outerD = new ModelVisual3D();

            innerE = new ModelVisual3D();
            outerE = new ModelVisual3D();

            mainX.Content = mainModel;

            innerB.Content = innerBModel;
            outerB.Content = outerBModel;

            innerC.Content = innerCModel;
            outerC.Content = outerCModel;

            innerD.Content = innerDModel;
            outerD.Content = outerDModel;
            
            innerE.Content = innerEModel;
            outerE.Content = outerEModel;

            outerB.Children.Add(innerB);
            outerC.Children.Add(innerC);
            outerD.Children.Add(innerD);
            outerE.Children.Add(innerE);

            mainX.Children.Add(outerB);
            mainX.Children.Add(outerC);
            mainX.Children.Add(outerD);
            mainX.Children.Add(outerE);

            mainY.Children.Add(mainX);
            mainZ.Children.Add(mainY);

            viewPort3D.Children.Add(mainZ);
            
            mainPrevious = new Vector(0, 0, 0);

            bPrevious = new Vector(0, 0, 0);
            cPrevious = new Vector(0, 0, 0);
            dPrevious = new Vector(0, 0, 0);
            ePrevious = new Vector(0, 0, 0);

            StartTimers();
            //StopTimers();
        }
        
        /// <summary>
        /// Starts alternate threads for calculation of the inverted pendulum and updating the display of the user interface for the FFTWs, pendulum, and graphs.
        /// </summary>
        private async void StartTimers()
        {
            await Task.Delay(250);

            t1 = new Timer
            {
                Interval = 30, //In milliseconds here
                AutoReset = true //Stops it from repeating
            };
            t1.Elapsed += new ElapsedEventHandler(UpdateTransformation);
            t1.Start();
        }

        /// <summary>
        /// Stops the secondary threads to end the calculation.
        /// </summary>
        private async void StopTimers()
        {
            await Task.Delay(60000);

            t1.Stop();
        }

        public void UpdateTransformation(object sender, ElapsedEventArgs e)
        {
            quadcopter = Program.Visualizer.GetQuadcopter();

            //rotate inner rotations
            //rotate outer rotations
            //transform entire quad

            Vector mainRotation = quadcopter.CurrentRotation;

            Vector bRotation = quadcopter.ThrusterB.CurrentRotation;
            Vector cRotation = quadcopter.ThrusterC.CurrentRotation;
            Vector dRotation = quadcopter.ThrusterD.CurrentRotation;
            Vector eRotation = quadcopter.ThrusterE.CurrentRotation;

            Vector mainRotationRelative = mainRotation.Subtract(mainPrevious);

            Vector bRelativeRotation = bRotation.Subtract(bPrevious);
            Vector cRelativeRotation = cRotation.Subtract(cPrevious);
            Vector dRelativeRotation = dRotation.Subtract(dPrevious);
            Vector eRelativeRotation = eRotation.Subtract(ePrevious);

            Dispatcher.BeginInvoke((Action)(() =>
            {
                Matrix3D mainXMatrix = mainX.Transform.Value;
                Matrix3D mainYMatrix = mainY.Transform.Value;
                Matrix3D mainZMatrix = mainZ.Transform.Value;

                Matrix3D innerBMatrix = innerB.Transform.Value;
                Matrix3D outerBMatrix = outerB.Transform.Value;

                Matrix3D innerCMatrix = innerC.Transform.Value;
                Matrix3D outerCMatrix = outerC.Transform.Value;

                Matrix3D innerDMatrix = innerD.Transform.Value;
                Matrix3D outerDMatrix = outerD.Transform.Value;

                Matrix3D innerEMatrix = innerE.Transform.Value;
                Matrix3D outerEMatrix = outerE.Transform.Value;

                /// B ROTATION

                innerBMatrix.Translate(new Vector3D(-1638, 1638, -28));
                innerBMatrix.Rotate(new Quaternion(new Vector3D(1, 0, 0), bRelativeRotation.Z));
                innerBMatrix.Translate(new Vector3D(1638, -1638, 28));
                innerB.Transform = new MatrixTransform3D(innerBMatrix);

                outerBMatrix.Translate(new Vector3D(1638, -1638, -28));
                outerBMatrix.Rotate(new Quaternion(new Vector3D(0, 1, 0), bRelativeRotation.X));
                outerBMatrix.Translate(new Vector3D(-1638, 1638, 28));
                outerB.Transform = new MatrixTransform3D(outerBMatrix);

                /// C ROTATION

                innerCMatrix.Translate(new Vector3D(1638, -1638, -28));
                innerCMatrix.Rotate(new Quaternion(new Vector3D(1, 0, 0), cRelativeRotation.Z));
                innerCMatrix.Translate(new Vector3D(-1638, 1638, 28));
                innerC.Transform = new MatrixTransform3D(innerCMatrix);

                outerCMatrix.Translate(new Vector3D(1638, -1638, -28));
                outerCMatrix.Rotate(new Quaternion(new Vector3D(0, 1, 0), cRelativeRotation.X));
                outerCMatrix.Translate(new Vector3D(-1638, 1638, 28));
                outerC.Transform = new MatrixTransform3D(outerCMatrix);

                /// D ROTATION
                
                innerDMatrix.Translate(new Vector3D(-1638, -1638, -28));
                innerDMatrix.Rotate(new Quaternion(new Vector3D(1, 0, 0), dRelativeRotation.Z));
                innerDMatrix.Translate(new Vector3D(1638, 1638, 28));
                innerD.Transform = new MatrixTransform3D(innerDMatrix);

                outerDMatrix.Translate(new Vector3D(-1638, -1638, -28));
                outerDMatrix.Rotate(new Quaternion(new Vector3D(0, 1, 0), dRelativeRotation.X));
                outerDMatrix.Translate(new Vector3D(1638, 1638, 28));
                outerD.Transform = new MatrixTransform3D(outerDMatrix);

                /// E ROTATION

                innerEMatrix.Translate(new Vector3D(1638, 1638, -28));
                innerEMatrix.Rotate(new Quaternion(new Vector3D(1, 0, 0), eRelativeRotation.Z));
                innerEMatrix.Translate(new Vector3D(-1638, -1638, 28));
                innerE.Transform = new MatrixTransform3D(innerEMatrix);

                outerEMatrix.Translate(new Vector3D(-1638, -1638, -28));
                outerEMatrix.Rotate(new Quaternion(new Vector3D(0, 1, 0), eRelativeRotation.X));
                outerEMatrix.Translate(new Vector3D(1638, 1638, 28));
                outerE.Transform = new MatrixTransform3D(outerEMatrix);

                /// MAIN ROTATION

                mainXMatrix.Rotate(new Quaternion(new Vector3D(1, 0, 0), mainRotationRelative.X));
                mainX.Transform = new MatrixTransform3D(mainXMatrix);

                mainYMatrix.Rotate(new Quaternion(new Vector3D(0, 0, 1), mainRotationRelative.Y));
                mainY.Transform = new MatrixTransform3D(mainYMatrix);

                mainZMatrix.Rotate(new Quaternion(new Vector3D(0, 1, 0), mainRotationRelative.Z));
                mainZ.Transform = new MatrixTransform3D(mainZMatrix);
            }));

            //innerBPrevious2 = innerBPrevious;
            mainPrevious = new Vector(mainRotation.X, mainRotation.Y, mainRotation.Z);

            bPrevious = new Vector(bRotation.X, bRotation.Y, bRotation.Z);
            cPrevious = new Vector(cRotation.X, cRotation.Y, cRotation.Z);
            dPrevious = new Vector(dRotation.X, dRotation.Y, dRotation.Z);
            ePrevious = new Vector(eRotation.X, eRotation.Y, eRotation.Z);
        }
    }
}
