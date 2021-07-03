using RayTracer.Shapes;
using Xunit;

namespace RayTracer.Tests
{
    public class CSGTests
    {
        [Fact]
        public void CreatingACSGWithAnOperationAnd2Shapes()
        {
            // Arrange
            var s = new Sphere();
            var c = new Cube();

            // Act
            var csg = new Csg(CsgOperation.Union, s, c);

            // Assert
            Assert.Equal(CsgOperation.Union, csg.Operation);
            Assert.Equal(s, csg.Left);
            Assert.Equal(c, csg.Right);
            Assert.Equal(csg, s.Parent);
            Assert.Equal(csg, c.Parent);
        }

        [Fact]
        public void EvaluatingTheRuleForCSGUnionOperation()
        {
            // Arrange
            var csg = new Csg(CsgOperation.Union, new Cube(), new Cube());
            var leftHits = new bool[] {true, true, true, true, false, false, false, false};
            var insideLefts = new bool[] {true, true, false, false, true, true, false, false};
            var insideRights = new bool[] {true, false, true, false, true, false, true, false};
            var expecteds = new bool[] {false, true, false, true, false, false, true, true};
            var allTestsPass = true;

            for (int i = 0; i < expecteds.Length; i++)
            {
                // Act
                var result = csg.IntersectionAllowed(leftHits[i], insideLefts[i], insideRights[i]);
                allTestsPass &= result == expecteds[i];
            }

            // Assert
            Assert.True(allTestsPass);
        }

        [Fact]
        public void EvaluatingTheRuleForCSGIntersectOperation()
        {
            // Arrange
            var csg = new Csg(CsgOperation.Intersect, new Cube(), new Cube());
            var leftHits = new bool[] {true, true, true, true, false, false, false, false};
            var insideLefts = new bool[] {true, true, false, false, true, true, false, false};
            var insideRights = new bool[] {true, false, true, false, true, false, true, false};
            var expecteds = new bool[] {true, false, true, false, true, true, false, false};
            var allTestsPass = true;

            for (int i = 0; i < expecteds.Length; i++)
            {
                // Act
                var result = csg.IntersectionAllowed(leftHits[i], insideLefts[i], insideRights[i]);
                allTestsPass &= result == expecteds[i];
            }

            // Assert
            Assert.True(allTestsPass);
        }

        [Fact]
        public void EvaluatingTheRuleForCSGDifferenceOperation()
        {
            // Arrange
            var csg = new Csg(CsgOperation.Difference, new Cube(), new Cube());
            var leftHits = new bool[] {true, true, true, true, false, false, false, false};
            var insideLefts = new bool[] {true, true, false, false, true, true, false, false};
            var insideRights = new bool[] {true, false, true, false, true, false, true, false};
            var expecteds = new bool[] {false, true, false, true, true, true, false, false};
            var allTestsPass = true;

            for (int i = 0; i < expecteds.Length; i++)
            {
                // Act
                var result = csg.IntersectionAllowed(leftHits[i], insideLefts[i], insideRights[i]);
                allTestsPass &= result == expecteds[i];
            }

            // Assert
            Assert.True(allTestsPass);
        }

        [Fact]
        public void FilteringAListOfIntersections()
        {
            // Arrange
            var s = new Sphere();
            var c = new Cube();
            var operations = new CsgOperation[] {CsgOperation.Union, CsgOperation.Intersect, CsgOperation.Difference};
            var expectedX0s = new int[] {0, 1, 0};
            var expectedX1s = new int[] {3, 2, 1};
            var allTestsPass = true;

            for (int i = 0; i < expectedX0s.Length; i++)
            {
                // Act
                var csg = new Csg(operations[i], s, c);
                var xs = Intersection.Intersections(new Intersection[] {new Intersection(1, s), new Intersection(2, c), new Intersection(3, s), new Intersection(4, c)});
                var result = csg.FilterIntersections(xs);
                allTestsPass &= result.Length == 2;
                allTestsPass &= result[0] == xs[expectedX0s[i]];
                allTestsPass &= result[1] == xs[expectedX1s[i]];
            }

            // Assert
            Assert.True(allTestsPass);
        }

        [Fact]
        public void ARayMissesACSGObject()
        {
            // Arrange
            var csg = new Csg(CsgOperation.Union, new Sphere(), new Cube());
            var ray = new Ray(new Point(0, 2, -5), new Vector(0, 0, 1));

            // Act
            var xs = csg.LocalIntersects(ray);

            // Assert
            Assert.Empty(xs);
        }

        [Fact]
        public void ARayHitsACSGObject()
        {
            // Arrange
            var s1 = new Sphere();
            var s2 = new Sphere();
            s2.Transform = Transformations.Translation(0, 0, 0.5);
            var csg = new Csg(CsgOperation.Union, s1, s2);
            var ray = new Ray(new Point(0, 0, -5), new Vector(0, 0, 1));

            // Act
            var xs = csg.LocalIntersects(ray);

            // Assert
            Assert.Equal(2, xs.Length);
            Assert.Equal(4, xs[0].T);
            Assert.Equal(s1, xs[0].Obj);
            Assert.Equal(6.5, xs[1].T);
            Assert.Equal(s2, xs[1].Obj);
        }

        [Fact]
        public void ACSGHasABoundingBoxContainingAllOfItsChildObjects()
        {
            // Arrange
            var s1 = new Sphere();
            var s2 = new Sphere();
            s2.Transform = Transformations.Translation(2, 3, 4);
            var csg = new Csg(CsgOperation.Difference, s1, s2);

            // Act
            var box = csg.GetBoundingBox();

            // Assert
            Assert.Equal(new Point(-1, -1, -1), box.MinPoint);
            Assert.Equal(new Point(3, 4, 5), box.MaxPoint);
        }

        [Fact]
        public void IntersectingARayWithACSGDoesntTestChildObjectsIfBoundingBoxIsMissed()
        {
            // Arrange
            var left = new TestShape();
            var right = new TestShape();
            var csg = new Csg(CsgOperation.Difference, left, right);
            var ray = new Ray(new Point(0, 0, -5), new Vector(0, 1, 0));

            // Act
            var xs = csg.Intersects(ray);

            // Assert
            Assert.Null(left.SavedRay);
            Assert.Null(right.SavedRay);
        }

        [Fact]
        public void IntersectingARayWithACSGTestsChildObjectsIfBoundingBoxIsHit()
        {
            // Arrange
            var left = new TestShape();
            var right = new TestShape();
            var csg = new Csg(CsgOperation.Difference, left, right);
            var ray = new Ray(new Point(0, 0, -5), new Vector(0, 0, 1));

            // Act
            var xs = csg.Intersects(ray);

            // Assert
            Assert.NotNull(((TestShape)(csg.Left)).SavedRay);
            Assert.NotNull(((TestShape)(csg.Right)).SavedRay);
        }

        [Fact]
        public void SubdividingACSGPartitionsItsChildObjects()
        {
            // Arrange
            var s1 = new Sphere();
            s1.Transform = Transformations.Translation(-1.5, 0, 0);
            var s2 = new Sphere();
            s2.Transform = Transformations.Translation(1.5, 0, 0);
            var left = new Group();
            left.AddChildren(new Shape[] {s1, s2});

            var s3 = new Sphere();
            s3.Transform = Transformations.Translation(0, 0, -1.5);
            var s4 = new Sphere();
            s4.Transform = Transformations.Translation(0, 0, 1.5);
            var right = new Group();
            right.AddChildren(new Shape[] {s3, s4});

            var shape = new Csg(CsgOperation.Difference, left, right);
            left = ((Group)(shape.Left));
            right = ((Group)(shape.Right));

            // Act
            Shape.Divide(shape, 1);

            // Assert
            Assert.Equal(2, left.Shapes.Count);
            Assert.IsType<Group>(left.Shapes[0]);
            var leftSubGroup1 = (Group)left.Shapes[0];
            Assert.Single(leftSubGroup1.Shapes);
            Assert.Equal(s1, leftSubGroup1.Shapes[0]);
            Assert.IsType<Group>(left.Shapes[1]);
            var leftSubGroup2 = (Group)left.Shapes[1];
            Assert.Single(leftSubGroup2.Shapes);
            Assert.Equal(s2, leftSubGroup2.Shapes[0]);
            Assert.Equal(2, right.Shapes.Count);
            Assert.IsType<Group>(right.Shapes[0]);
            var rightSubGroup1 = (Group)right.Shapes[0];
            Assert.Single(rightSubGroup1.Shapes);
            Assert.Equal(s3, rightSubGroup1.Shapes[0]);
            Assert.IsType<Group>(right.Shapes[1]);
            var rightSubGroup2 = (Group)right.Shapes[1];
            Assert.Single(rightSubGroup2.Shapes);
            Assert.Equal(s4, rightSubGroup2.Shapes[0]);
        }

        [Fact]
        public void CloningACSG()
        {
            // Arrange
            var left = new TestShape();
            var right = new TestShape();
            var orig = new Csg(CsgOperation.Difference, left, right);

            // Act
            var result = orig.Clone();

            // Assert
            Assert.Equal(orig.Aabb, result.Aabb);
            Assert.Equal(orig.Left, result.Left);
            Assert.Equal(orig.Material, result.Material);
            Assert.Equal(orig.Operation, result.Operation);
            Assert.Equal(orig.Origin, result.Origin);
            Assert.Equal(orig.Parent, result.Parent);
            Assert.Equal(orig.Right, result.Right);
            Assert.Equal(orig.Shapes, result.Shapes);
            Assert.Equal(orig.Transform, result.Transform);
        }

        [Fact]
        public void StringRepresentation()
        {
            // Arrange
            var expected = "[Type:RayTracer.Shapes.Csg, Origin:[X:0, Y:0, Z:0, W:1]\nParent:null\nOp:Intersect\nMaterial:[Colour:[R:1, G:1, B:1]\nAmb:0.1,Dif:0.9,Spec:0.9,Shin:200,Refl:0,Tran:0,Refr:1,Shad:True,\nPattern:null\n]\nTransform:[[1, 0, 0, 0,\n0, 1, 0, 0,\n0, 0, 1, 0,\n0, 0, 0, 1]]\nChildren:\n[Type:RayTracer.Tests.TestShape\nId:637603259314317791\nOrigin:[X:0, Y:0, Z:0, W:1]\nParent:637603259314339500\nMaterial:[Colour:[R:1, G:1, B:1]\nAmb:0.1,Dif:0.9,Spec:0.9,Shin:200,Refl:0,Tran:0,Refr:1,Shad:True,\nPattern:null\n]\nTransform:[[1, 0, 0, 0,\n0, 1, 0, 0,\n0, 0, 1, 0,\n0, 0, 0, 1]]\nSavedRay:false]\n[Type:RayTracer.Tests.TestShape\nId:637603259314337745\nOrigin:[X:0, Y:0, Z:0, W:1]\nParent:637603259314339500\nMaterial:[Colour:[R:1, G:1, B:1]\nAmb:0.1,Dif:0.9,Spec:0.9,Shin:200,Refl:0,Tran:0,Refr:1,Shad:True,\nPattern:null\n]\nTransform:[[1, 0, 0, 0,\n0, 1, 0, 0,\n0, 0, 1, 0,\n0, 0, 0, 1]]\nSavedRay:false]\n]";
            var orig = new Csg(CsgOperation.Intersect, new TestShape(), new TestShape());

            // Act
            var result = orig.ToString();

            // Assert
            Assert.True(Utilities.ToStringEquals(expected, result));
        }
    }
}
