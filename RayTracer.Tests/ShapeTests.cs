using System;
using RayTracer.Shapes;
using Xunit;

namespace RayTracer.Tests
{
    public class TestShape: Shape
    {
        private Ray _savedRay;
        public Ray SavedRay
        {
            get { return _savedRay; }
            set { _savedRay = value; }
        }
        
        public override Intersection[] LocalIntersects(Ray ray)
        {
            SavedRay = ray;
            return new Intersection[] {new Intersection(double.NaN, null)};
        }

        public override Vector LocalNormalAt(Point objectPoint, Intersection intersect = null)
        {
            return (objectPoint - new Point(0, 0, 0)).Normalise();
        }

        public override BoundingBox GetBoundingBox()
        {
            return new BoundingBox(new Point(-1, -1, -1), new Point(1, 1, 1));
        }

        public override TestShape Clone()
        {
            return new TestShape
            {
                Origin = Origin.Clone(),
                Material = Material.Clone(),
                Transform = Transform.Clone(),
                Parent = Parent,
                SavedRay = SavedRay
            };
        }

        public override string ToString()
        {
            var savedRay = SavedRay != null ? "true" : "false";
            return $"[{base.ToString()}SavedRay:{savedRay}]";
        }
    }

    public class ShapeTests
    {
        [Fact]
        public void AShapeHasAParentAttribute()
        {
            // Arrange
            var s = new TestShape();
            
            // Act

            // Assert
            Assert.Null(s.Parent);
        }

        [Fact]
        public void TheDefaultTransformationMatrix()
        {
            // Arrange
            var s = new TestShape();

            // Act

            // Assert
            Assert.Equal(Matrix.Identity(4), s.Transform);
        }

        [Fact]
        public void AssigningATransformationMatrix()
        {
            // Arrange
            var s = new TestShape();
            var trans = Transformations.Translation(2, 3, 4);

            // Act
            s.Transform = trans;

            // Assert
            Assert.Equal(trans, s.Transform);
        }

        [Fact]
        public void TheDefaultMaterial()
        {
            // Arrange
            var s = new TestShape();

            // Act

            // Assert
            Assert.Equal(new Material(), s.Material);
        }

        [Fact]
        public void AssigningAMaterial()
        {
            // Arrange
            var s = new TestShape();
            var mat = new Material();
            mat.Ambient = 1;

            // Act
            s.Material = mat;

            // Assert
            Assert.Equal(mat, s.Material);
        }

        [Fact]
        public void IntersectingAScaledShapeWithARay()
        {
            // Arrange
            var ray = new Ray(new Point(0, 0, -5), new Vector(0, 0, 1));
            var s = new TestShape();
            var expectedOrigin = new Point(0, 0, -2.5);
            var expectedDirection = new Vector(0, 0, 0.5);

            // Act
            s.Transform = Transformations.Scaling(2, 2, 2);
            var xs = s.Intersects(ray);

            // Assert
            Assert.Equal(expectedOrigin, s.SavedRay.Origin);
            Assert.Equal(expectedDirection, s.SavedRay.Direction);
        }

        [Fact]
        public void IntersectingATranslatedShapeWithARay()
        {
            // Arrange
            var ray = new Ray(new Point(0, 0, -5), new Vector(0, 0, 1));
            var s = new TestShape();
            var expectedOrigin = new Point(-5, 0, -5);
            var expectedDirection = new Vector(0, 0, 1);

            // Act
            s.Transform = Transformations.Translation(5, 0, 0);
            var xs = s.Intersects(ray);

            // Assert
            Assert.Equal(expectedOrigin, s.SavedRay.Origin);
            Assert.Equal(expectedDirection, s.SavedRay.Direction);
        }

        [Fact]
        public void ComputeTheNormalOnATranslatedShape()
        {
            // Arrange
            var oneOverSqrtTwo = 1.0 / Math.Sqrt(2);
            var s = new TestShape();
            s.Transform = Transformations.Translation(0, 1, 0);
            var expected = new Vector(0, oneOverSqrtTwo, -oneOverSqrtTwo);

            // Act
            var result = s.NormalAt(new Point(0, 1 + oneOverSqrtTwo, -oneOverSqrtTwo));

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ComputeTheNormalOnATransformedShape()
        {
            // Arrange
            var halfSqrtTwo = Math.Sqrt(2) / 2.0;
            var s = new TestShape();
            var m = Transformations.Scaling(1, 0.5, 1) * Transformations.RotationZ(Math.PI / 5.0);
            s.Transform = m;
            var expected = new Vector(0, 0.97014, -0.24254);

            // Act
            var result = s.NormalAt(new Point(0, halfSqrtTwo, -halfSqrtTwo));

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ConvertingAPointFromWorldSpaceToObjectSpace()
        {
            // Arrange
            var g1 = new Group();
            g1.Transform = Transformations.RotationY(Math.PI / 2.0);
            var g2 = new Group();
            g2.Transform = Transformations.Scaling(2, 2, 2);
            g1.AddChild(g2);
            var s = new Sphere();
            s.Transform = Transformations.Translation(5, 0, 0);
            g2.AddChild(s);
            var expected = new Point(0, 0, -1);

            // Act
            var result = s.WorldToObject(new Point(-2, 0, -10));

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ConvertingANormalVectorFromObjectSpaceToWorldSpace()
        {
            // Arrange
            var thirdSqrt3 = Math.Sqrt(3) / 3.0;
            var g1 = new Group();
            g1.Transform = Transformations.RotationY(Math.PI / 2.0);
            var g2 = new Group();
            g2.Transform = Transformations.Scaling(1, 2, 3);
            g1.AddChild(g2);
            var s = new Sphere();
            s.Transform = Transformations.Translation(5, 0, 0);
            g2.AddChild(s);
            var expected = new Vector(0.28571, 0.42857, -0.85714);

            // Act
            var result = s.NormalToWorld(new Vector(thirdSqrt3, thirdSqrt3, thirdSqrt3));

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void FindingTheNormalOnAChildObjectInWorldSpace()
        {
            // Arrange
            var g1 = new Group();
            g1.Transform = Transformations.RotationY(Math.PI / 2.0);
            var g2 = new Group();
            g2.Transform = Transformations.Scaling(1, 2, 3);
            g1.AddChild(g2);
            var s = new Sphere();
            s.Transform = Transformations.Translation(5, 0, 0);
            g2.AddChild(s);
            var expected = new Vector(0.2857, 0.42854, -0.85716);

            // Act
            var result = s.NormalAt(new Point(1.7321, 1.1547, -5.5774));

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void TheTestShapeHasAnArbitraryBoundingBox()
        {
            // Arrange
            var s = new TestShape();

            // Act
            var boundingBox = s.GetBoundingBox();

            // Assert
            Assert.Equal(new Point(-1, -1, -1), boundingBox.MinPoint);
            Assert.Equal(new Point(1, 1, 1), boundingBox.MaxPoint);
        }

        [Fact]
        public void QueryingAShapesBoundingBoxInItsParentsSpace()
        {
            // Arrange
            var s = new Sphere();
            s.Transform = Transformations.Translation(1, -3, 5) * Transformations.Scaling(0.5, 2, 4);

            // Act
            var boundingBox = s.GetParentSpaceBoundingBox();

            // Assert
            Assert.Equal(new Point(0.5, -5, 1), boundingBox.MinPoint);
            Assert.Equal(new Point(1.5, -1, 9), boundingBox.MaxPoint);
        }

        [Fact]
        public void SubdividingAPrimitiveDoesNothing()
        {
            // Arrange
            var s = new Sphere();

            // Act
            Shape.Divide(s, 1);

            // Assert
            Assert.IsType<Sphere>(s);
        }

        [Fact]
        public void CloningAShape()
        {
            // Arrange
            var orig = new TestShape();

            // Act
            var clone = orig.Clone();

            // Assert
            Assert.Equal(orig.Origin, clone.Origin);
            Assert.Equal(orig.Material, clone.Material);
            Assert.Equal(orig.Transform, clone.Transform);
            Assert.Equal(orig.Parent, clone.Parent);
            Assert.Equal(orig.SavedRay, clone.SavedRay);
        }

        [Fact]
        public void StringRepresentation()
        {
            // Arrange
            var expected = "[Type:RayTracer.Tests.TestShape\nId:637602294772396341\nOrigin:[X:0, Y:0, Z:0, W:1]\nParent:null\nMaterial:[Colour:[R:1, G:1, B:1]\nAmb:0.1,Dif:0.9,Spec:0.9,Shin:200,Refl:0,Tran:0,Refr:1,Shad:True,\nPattern:null\n]\nTransform:[[1, 0, 0, 0,\n0, 1, 0, 0,\n0, 0, 1, 0,\n0, 0, 0, 1]]\nSavedRay:false]";
            var orig = new TestShape();

            // Act
            var result = orig.ToString();

            // Assert
            Assert.True(TestUtilities.ToStringEquals(expected, result));
        }
    }
}
