using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADRCVisualization.Class_Files.Mathematics;
using ADRCVisualization.Class_Files;
using System.Windows.Media;
using System.Windows.Media.Media3D;

using HelixToolkit.Wpf;

namespace ADRCVisualization
{
    class QuadModel
    {
        public Vector assemblyOffset = new Vector(0, 0, 0);

        private Model3D main;
        private Model3D innerB;
        private Model3D outerB;

        private Vector innerBPrevious;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        public QuadModel()
        {
            StLReader stLReader = new StLReader();

            //main = stLReader.Read(@"..\..\Resources\Main.stl");
            innerB = stLReader.Read(@"..\..\Resources\Inner.stl");
            //outerB = stLReader.Read(@"..\..\Resources\Outer.stl");

            innerBPrevious = new Vector(0, 0, 0);
            
            //UpdateModel();
        }

        public void UpdateTransformation(Quadcopter quadcopter)
        {
            //rotate inner rotations
            //rotate outer rotations
            //transform entire quad

            //Vector test = quadcopter.CurrentRotation;

            //this.Model.

            Vector rotation = quadcopter.CurrentRotation;

            Vector test = rotation.Subtract(innerBPrevious);

            TransformModel(ref innerB, new Vector(test.X, test.Y, 0), new Vector(-1638, 28, 1638), new Vector(0, 0, 0));
            //TransformModel(ref outerB, new Vector(0, test.Y, test.Z), new Vector(-1638, 28, 1638), new Vector(0, 0, 0));

            innerBPrevious = rotation;

            //Console.WriteLine(quadcopter.CurrentRotation.X + " " + quadcopter.CurrentRotation.Subtract(innerBPrevious).X); 

            //UpdateModel();
        }

        private void TransformModel(ref Model3D model, Vector rotation, Vector pointOfRotation, Vector translation)
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

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>The model.</value>
        public Model3D Model { get; set; }
    }
}
