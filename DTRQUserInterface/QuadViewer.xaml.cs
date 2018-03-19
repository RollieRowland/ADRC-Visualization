using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using DTRQCSInterface;

namespace ADRCVisualization
{
    /// <summary>
    /// Interaction logic for QuadViewer.xaml
    /// </summary>
    public partial class QuadViewer : UserControl
    {
        private SQuad quadcopter;

        private Timer t1;

        private ModelVisual3D main;

        private ModelVisual3D innerB;
        private ModelVisual3D outerB;

        private ModelVisual3D innerC;
        private ModelVisual3D outerC;

        private ModelVisual3D innerD;
        private ModelVisual3D outerD;

        private ModelVisual3D innerE;
        private ModelVisual3D outerE;

        private Vector mainPrevious;
        private Class_Files.Mathematics.Quaternion quadRotPrevious;

        private Vector bPrevious;
        private Vector cPrevious;
        private Vector dPrevious;
        private Vector ePrevious;
        private Matrix3D initialMainRot;


        public QuadViewer()
        {
            InitializeComponent();

            Console.WriteLine("Starting 3D Viewer...");

            ModelImporter import = new ModelImporter();

            string directory = @"C:\Users\steve\Documents\GitHub\Dual-Tilt-Rotor-Quadcopter\DTRQUserInterface\Resources\";
            //string directory = @"..\..\Resources\";

            import.DefaultMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.MediumSlateBlue));

            Model3D mainModel = import.Load(directory + "Main.stl");
            
            import.DefaultMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.BurlyWood));

            Model3D innerBModel = import.Load(directory + "InnerB.stl");
            Model3D outerBModel = import.Load(directory + "OuterB.stl");

            import.DefaultMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.BlueViolet));

            Model3D innerCModel = import.Load(directory + "InnerC.stl");
            Model3D outerCModel = import.Load(directory + "OuterC.stl");

            import.DefaultMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.ForestGreen));

            Model3D innerDModel = import.Load(directory + "InnerD.stl");
            Model3D outerDModel = import.Load(directory + "OuterD.stl");

            import.DefaultMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.MediumAquamarine));

            Model3D innerEModel = import.Load(directory + "InnerE.stl");
            Model3D outerEModel = import.Load(directory + "OuterE.stl");
            
            main = new ModelVisual3D();

            innerB = new ModelVisual3D();
            outerB = new ModelVisual3D();

            innerC = new ModelVisual3D();
            outerC = new ModelVisual3D();

            innerD = new ModelVisual3D();
            outerD = new ModelVisual3D();

            innerE = new ModelVisual3D();
            outerE = new ModelVisual3D();

            main.Content = mainModel;

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

            main.Children.Add(outerB);
            main.Children.Add(outerC);
            main.Children.Add(outerD);
            main.Children.Add(outerE);

            initialMainRot = main.Transform.Value;

            viewPort3D.Children.Add(main);
            
            mainPrevious = new Vector(0, 0, 0);

            bPrevious = new Vector(0, 0, 0);
            cPrevious = new Vector(0, 0, 0);
            dPrevious = new Vector(0, 0, 0);
            ePrevious = new Vector(0, 0, 0);

            quadRotPrevious = new Class_Files.Mathematics.Quaternion(1, 0, 0, 0);

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
            quadcopter = Program.UI.GetQuadcopter();

            //rotate inner rotations
            //rotate outer rotations
            //transform entire quad

            Class_Files.Mathematics.Quaternion quadRotCurrent = 
                Class_Files.Mathematics.Quaternion.DirectionAngleToQuaternion(
                    new DirectionAngle(
                        quadcopter.CurrentRotation.Rotation,
                        quadcopter.CurrentRotation.Direction.X,
                        quadcopter.CurrentRotation.Direction.Y,
                        quadcopter.CurrentRotation.Direction.Z
                    )
                );
            
            Vector mainRotation = (2 * (quadRotPrevious - quadRotCurrent) * quadRotCurrent.Conjugate() / quadcopter.dT).GetBiVector();

            Vector bRotation = new Vector(quadcopter.ThrusterB.CurrentRotation.X, quadcopter.ThrusterB.CurrentRotation.Y, quadcopter.ThrusterB.CurrentRotation.Z);
            Vector cRotation = new Vector(quadcopter.ThrusterC.CurrentRotation.X, quadcopter.ThrusterC.CurrentRotation.Y, quadcopter.ThrusterC.CurrentRotation.Z);
            Vector dRotation = new Vector(quadcopter.ThrusterD.CurrentRotation.X, quadcopter.ThrusterD.CurrentRotation.Y, quadcopter.ThrusterD.CurrentRotation.Z);
            Vector eRotation = new Vector(quadcopter.ThrusterE.CurrentRotation.X, quadcopter.ThrusterE.CurrentRotation.Y, quadcopter.ThrusterE.CurrentRotation.Z);

            Vector mainRotationRelative = mainRotation.Multiply(Math.PI).Multiply(new Vector(1, -1, -1));

            Vector bRelativeRotation = bRotation.Subtract(bPrevious);
            Vector cRelativeRotation = cRotation.Subtract(cPrevious);
            Vector dRelativeRotation = dRotation.Subtract(dPrevious);
            Vector eRelativeRotation = eRotation.Subtract(ePrevious);

            Dispatcher.BeginInvoke((Action)(() =>
            {
                Matrix3D mainMatrix = initialMainRot;

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
                innerBMatrix.Rotate(new System.Windows.Media.Media3D.Quaternion(new Vector3D(1, 0, 0), bRelativeRotation.Z));
                innerBMatrix.Translate(new Vector3D(1638, -1638, 28));
                innerB.Transform = new MatrixTransform3D(innerBMatrix);

                outerBMatrix.Translate(new Vector3D(1638, -1638, -28));
                outerBMatrix.Rotate(new System.Windows.Media.Media3D.Quaternion(new Vector3D(0, 1, 0), bRelativeRotation.X));
                outerBMatrix.Translate(new Vector3D(-1638, 1638, 28));
                outerB.Transform = new MatrixTransform3D(outerBMatrix);

                /// C ROTATION

                innerCMatrix.Translate(new Vector3D(1638, -1638, -28));
                innerCMatrix.Rotate(new System.Windows.Media.Media3D.Quaternion(new Vector3D(1, 0, 0), cRelativeRotation.Z));
                innerCMatrix.Translate(new Vector3D(-1638, 1638, 28));
                innerC.Transform = new MatrixTransform3D(innerCMatrix);

                outerCMatrix.Translate(new Vector3D(1638, -1638, -28));
                outerCMatrix.Rotate(new System.Windows.Media.Media3D.Quaternion(new Vector3D(0, 1, 0), cRelativeRotation.X));
                outerCMatrix.Translate(new Vector3D(-1638, 1638, 28));
                outerC.Transform = new MatrixTransform3D(outerCMatrix);

                /// D ROTATION
                
                innerDMatrix.Translate(new Vector3D(-1638, -1638, -28));
                innerDMatrix.Rotate(new System.Windows.Media.Media3D.Quaternion(new Vector3D(1, 0, 0), dRelativeRotation.Z));
                innerDMatrix.Translate(new Vector3D(1638, 1638, 28));
                innerD.Transform = new MatrixTransform3D(innerDMatrix);

                outerDMatrix.Translate(new Vector3D(-1638, -1638, -28));
                outerDMatrix.Rotate(new System.Windows.Media.Media3D.Quaternion(new Vector3D(0, 1, 0), dRelativeRotation.X));
                outerDMatrix.Translate(new Vector3D(1638, 1638, 28));
                outerD.Transform = new MatrixTransform3D(outerDMatrix);

                /// E ROTATION

                innerEMatrix.Translate(new Vector3D(1638, 1638, -28));
                innerEMatrix.Rotate(new System.Windows.Media.Media3D.Quaternion(new Vector3D(1, 0, 0), eRelativeRotation.Z));
                innerEMatrix.Translate(new Vector3D(-1638, -1638, 28));
                innerE.Transform = new MatrixTransform3D(innerEMatrix);

                outerEMatrix.Translate(new Vector3D(-1638, -1638, -28));
                outerEMatrix.Rotate(new System.Windows.Media.Media3D.Quaternion(new Vector3D(0, 1, 0), eRelativeRotation.X));
                outerEMatrix.Translate(new Vector3D(1638, 1638, 28));
                outerE.Transform = new MatrixTransform3D(outerEMatrix);

                /// MAIN ROTATION
                mainMatrix.Rotate(new System.Windows.Media.Media3D.Quaternion(quadRotCurrent.Z, -quadRotCurrent.X, -quadRotCurrent.Y, quadRotCurrent.W));
                main.Transform = new MatrixTransform3D(mainMatrix);
            }));
            
            mainPrevious = new Vector(mainRotation);
            quadRotPrevious = new Class_Files.Mathematics.Quaternion(quadRotCurrent);

            bPrevious = new Vector(bRotation);
            cPrevious = new Vector(cRotation);
            dPrevious = new Vector(dRotation);
            ePrevious = new Vector(eRotation);
        }
    }
}
