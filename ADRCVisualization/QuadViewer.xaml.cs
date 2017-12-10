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

        private ModelVisual3D main;
        private ModelVisual3D innerB;
        private ModelVisual3D outerB;

        private Vector innerBPrevious;
        private Vector innerBPrevious2;
        
        private Model3DGroup modelGroup;

        private ModelVisual3D testV3D;

        public QuadViewer()
        {
            InitializeComponent();

            ModelImporter import = new ModelImporter();

            Model3D model = import.Load(@"..\..\Resources\Inner.stl");
            innerB = new ModelVisual3D();
            innerB.Content = model;

            Model3D model2 = import.Load(@"..\..\Resources\Outer.stl");
            outerB = new ModelVisual3D();
            outerB.Content = model2;

            viewPort3D.Children.Add(innerB);
            viewPort3D.Children.Add(outerB);
            
            innerBPrevious = new Vector(0, 0, 0);

            modelGroup = new Model3DGroup();


            testV3D = new ModelVisual3D();
            

            viewPort3D.Children.Add(testV3D);

            StartTimers();
            //StopTimers();
        }
        
        /// <summary>
        /// Starts alternate threads for calculation of the inverted pendulum and updating the display of the user interface for the FFTWs, pendulum, and graphs.
        /// </summary>
        private void StartTimers()
        {
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
            Vector rotation = quadcopter.ThrusterB.CurrentRotation;

            Vector test = rotation.Subtract(innerBPrevious);

            Dispatcher.BeginInvoke((Action)(() =>
            {
                var matrix = innerB.Transform.Value;
                matrix.Translate(new Vector3D(1638, -1638, -28));
                matrix.Rotate(new Quaternion(new Vector3D(1, 0, 0), test.X));
                matrix.Rotate(new Quaternion(new Vector3D(0, Math.Sin(Misc.DegreesToRadians(rotation.X)) + Math.Cos(Misc.DegreesToRadians(rotation.X)),
                                                             Math.Cos(Misc.DegreesToRadians(rotation.X)) - Math.Sin(Misc.DegreesToRadians(rotation.X))), test.Z));
                matrix.Translate(new Vector3D(-1638, 1638, 28));

                innerB.Transform = new MatrixTransform3D(matrix);
                
                
                var matrix2 = modelGroup.Transform.Value;
                matrix2.Translate(new Vector3D(1638, -1638, -28));
                matrix2.Rotate(new Quaternion(new Vector3D(0, 1, 0), test.Z));
                matrix2.Translate(new Vector3D(-1638, 1638, 28));

                outerB.Transform = new MatrixTransform3D(matrix2);

                //innerB.Transform.Value.

                //TransformModel(ref innerB, new Vector(test.X, 0, 0), new Vector(-1638, 28, 1638), new Vector(0, 0, 0));
                //TransformModel(ref innerB, new Vector(0, 0, test.Z), new Vector(-1638, 28, 1638), new Vector(0, 0, 0));
                //TransformModel(ref outerB, new Vector(0, 0, test.Z), new Vector(-1638, 28, 1638), new Vector(0, 0, 0));
            }));

            //innerBPrevious2 = innerBPrevious;
            innerBPrevious = new Vector(rotation.X, rotation.Y, rotation.Z);
        }

        private void TransformModel(ref ModelVisual3D model, Vector rotation, Vector pointOfRotation, Vector translation)
        {
            TranslateTransform3D translate = new TranslateTransform3D(translation.X, translation.Z, translation.Y);

            //left hand rule, y and z are flipped
            RotateTransform3D rotateX, rotateY, rotateZ;

            Point3D point = new Point3D(pointOfRotation.X, pointOfRotation.Z, pointOfRotation.Y);

            //Console.WriteLine(test.ToString());

            rotateX = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), rotation.X), point);
            rotateY = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), rotation.Y), point);
            rotateZ = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), rotation.Z), point);

            Transform3D transform;

            transform = Transform3DHelper.CombineTransform(translate, rotateX);
            transform = Transform3DHelper.CombineTransform(transform, rotateY);
            transform = Transform3DHelper.CombineTransform(transform, rotateZ);

            model.Transform = Transform3DHelper.CombineTransform(model.Transform, transform);
        }
    }
}
