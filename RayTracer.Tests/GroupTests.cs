using RayTracer.Shapes;
using Xunit;

namespace RayTracer.Tests
{
    public class GroupTests
    {
        [Fact]
        public void CreatingANewGroup()
        {
            // Arrange
            var g = new Group();
            
            // Act

            // Assert
            Assert.Equal(Matrix.Identity(4), g.Transform);
            Assert.Empty(g.Shapes);
        }

        [Fact]
        public void AddingAChildObjectToAGroup()
        {
            // Arrange
            var g = new Group();
            var s = new TestShape();
            
            // Act
            g.AddChild(s);

            // Assert
            Assert.NotEmpty(g.Shapes);
            Assert.Contains(s, g.Shapes);
            Assert.Same(g, s.Parent);
        }

        [Fact]
        public void AddingMultipleChildObjectsToAGroup()
        {
            // Arrange
            var g = new Group();
            var s1 = new TestShape();
            var s2 = new TestShape();
            var s3 = new TestShape();
            
            // Act
            g.AddChildren(new Shape[] {s1, s2, s3});

            // Assert
            Assert.Equal(3, g.Shapes.Count);
            Assert.Contains(s1, g.Shapes);
            Assert.Contains(s2, g.Shapes);
            Assert.Contains(s3, g.Shapes);
            Assert.Same(g, s1.Parent);
            Assert.Same(g, s2.Parent);
            Assert.Same(g, s3.Parent);
        }

        [Fact]
        public void IntersectingARayWithAnEmptyGroup()
        {
            // Arrange
            var g = new Group();
            var r = new Ray(new Point(0, 0, 0), new Vector(0, 0, 1));

            // Act
            var xs = g.LocalIntersects(r);

            // Assert
            Assert.Empty(xs);
        }

        [Fact]
        public void IntersectingARayWithANonemptyGroup()
        {
            // Arrange
            var g = new Group();
            var s1 = new Sphere();
            var s2 = new Sphere();
            s2.Transform = Transformations.Translation(0, 0, -3);
            var s3 = new Sphere();
            s3.Transform = Transformations.Translation(5, 0, 0);
            g.AddChild(s1);
            g.AddChild(s2);
            g.AddChild(s3);
            var r = new Ray(new Point(0, 0, -5), new Vector(0, 0, 1));

            // Act
            var xs = g.LocalIntersects(r);

            // Assert
            Assert.Equal(4, xs.Length);
            Assert.Equal(s2, xs[0].Obj);
            Assert.Equal(s2, xs[1].Obj);
            Assert.Equal(s1, xs[2].Obj);
            Assert.Equal(s1, xs[3].Obj);
        }

        [Fact]
        public void IntersectingARayWithATransformedGroup()
        {
            // Arrange
            var g = new Group();
            g.Transform = Transformations.Scaling(2, 2, 2);
            var s = new Sphere();
            s.Transform = Transformations.Translation(5, 0, 0);
            g.AddChild(s);
            var r = new Ray(new Point(10, 0, -10), new Vector(0, 0, 1));

            // Act
            var xs = g.Intersects(r);

            // Assert
            Assert.Equal(2, xs.Length);
        }

        [Fact]
        public void AGroupHasABoundingBoxWhichContainsItsChildren()
        {
            // Arrange
            var g = new Group();
            var s = new Sphere();
            s.Transform = Transformations.Translation(2, 5, -3) * Transformations.Scaling(2, 2, 2);
            g.AddChild(s);
            var c = new Cylinder();
            c.Minimum = -2;
            c.Maximum = 2;
            c.Transform = Transformations.Translation(-4, -1, 4) * Transformations.Scaling(0.5, 1, 0.5);
            g.AddChild(c);

            // Act
            var box = g.GetBoundingBox();

            // Assert
            Assert.Equal(new Point(-4.5, -3, -5), box.MinPoint);
            Assert.Equal(new Point(4, 7, 4.5), box.MaxPoint);
        }

        [Fact]
        public void IntersectingARayWithAGroupDoesNotTestChildrenIfBoundingBoxIsMissed()
        {
            // Arrange
            var g = new Group();
            var s1 = new TestShape();
            g.AddChild(s1);
            var ray = new Ray(new Point(0, 0, -5), new Vector(0, 1, 0));

            // Act
            var xs = g.Intersects(ray);

            // Assert
            Assert.Null(s1.SavedRay);
        }

        [Fact]
        public void IntersectingARayWithAGroupDoesTestChildrenIfBoundingBoxIsHit()
        {
            // Arrange
            var g = new Group();
            var s1 = new TestShape();
            g.AddChild(s1);
            var ray = new Ray(new Point(0, 0, -5), new Vector(0, 0, 1));

            // Act
            var xs = g.Intersects(ray);

            // Assert
            Assert.NotNull(s1.SavedRay);
        }

        [Fact]
        public void SetMaterialSetsTheMaterialOnTheGroup()
        {
            // Arrange
            var groupMaterial = new Material();
            groupMaterial.Colour = Colour.RED;
            var g = new Group();

            // Act
            g.SetMaterial(groupMaterial);

            // Assert
            Assert.Equal(groupMaterial, g.Material);
        }

        [Fact]
        public void SetMaterialSetsTheMaterialOnGroupChildObjects()
        {
            // Arrange
            var shapeMaterial = new Material();
            shapeMaterial.Colour = Colour.BLUE;
            var groupMaterial = new Material();
            groupMaterial.Colour = Colour.RED;

            var g = new Group();
            var s1 = new Cube();
            s1.Material = shapeMaterial;
            var s2 = new Sphere();
            s2.Material = shapeMaterial;

            g.AddChild(s1);
            g.AddChild(s2);

            // Act
            g.SetMaterial(groupMaterial);

            // Assert
            Assert.Equal(groupMaterial, s1.Material);
            Assert.Equal(groupMaterial, s2.Material);
        }

        [Fact]
        public void SetMaterialSetsTheMaterialOnGroupDescendantObjects()
        {
            // Arrange
            var shapeMaterial = new Material();
            shapeMaterial.Colour = Colour.BLUE;
            var groupMaterial = new Material();
            groupMaterial.Colour = Colour.RED;

            var g1 = new Group();
            var g2 = new Group();
            var s1 = new Cube();
            s1.Material = shapeMaterial;
            var s2 = new Sphere();
            s2.Material = shapeMaterial;
            var s3 = new Cone();
            s3.Material = shapeMaterial;

            g1.AddChild(s1);
            g2.AddChild(s2);
            g2.AddChild(s3);
            g1.AddChild(g2);

            // Act
            g1.SetMaterial(groupMaterial);

            // Assert
            Assert.Equal(groupMaterial, g1.Material);
            Assert.Equal(groupMaterial, s1.Material);
            Assert.Equal(groupMaterial, s2.Material);
            Assert.Equal(groupMaterial, g2.Material);
            Assert.Equal(groupMaterial, s3.Material);
        }

        [Fact]
        public void SetMaterialCanBeCalledAtAnyLevelOfAGroupHierarchy()
        {
            // Arrange
            var shapeMaterial = new Material();
            shapeMaterial.Colour = Colour.BLUE;
            var groupMaterial = new Material();
            groupMaterial.Colour = Colour.RED;
            var groupMaterial2 = new Material();
            groupMaterial2.Colour = Colour.GREEN;

            var g1 = new Group();
            var g2 = new Group();
            var s1 = new Cube();
            s1.Material = shapeMaterial;
            var s2 = new Sphere();
            s2.Material = shapeMaterial;
            var s3 = new Cone();
            s3.Material = shapeMaterial;

            g1.AddChild(s1);
            g2.AddChild(s2);
            g2.AddChild(s3);
            g1.AddChild(g2);

            // Act
            g1.SetMaterial(groupMaterial);
            g2.SetMaterial(groupMaterial2);

            // Assert
            Assert.Equal(groupMaterial, g1.Material);
            Assert.Equal(groupMaterial, s1.Material);
            Assert.Equal(groupMaterial2, s2.Material);
            Assert.Equal(groupMaterial2, g2.Material);
            Assert.Equal(groupMaterial2, s3.Material);
        }

        [Fact]
        public void PartitioningAGroupsChildObjects()
        {
            // Arrange
            var s1 = new Sphere();
            s1.Transform = Transformations.Translation(-2, 0, 0);
            var s2 = new Sphere();
            s2.Transform = Transformations.Translation(2, 0, 0);
            var s3 = new Sphere();
            var g = new Group();
            g.AddChildren(new Shape[] {s1, s2, s3});
            Group left;
            Group right;

            // Act
            g.PartitionChildObjects(out left, out right);

            // Assert
            Assert.Single(g.Shapes);
            Assert.Equal(s3, g.Shapes[0]);
            Assert.Single(left.Shapes);
            Assert.Equal(s1, left.Shapes[0]);
            Assert.Single(right.Shapes);
            Assert.Equal(s2, right.Shapes[0]);
        }

        [Fact]
        public void CreatingASubGroupFromAListOfChildObjects()
        {
            // Arrange
            var s1 = new Sphere();
            var s2 = new Sphere();
            var g = new Group();

            // Act
            g.CreateSubgroup(new Shape[]{s1, s2});

            // Assert
            Assert.Single(g.Shapes);
            Assert.Equal(2, ((Group)(g.Shapes[0])).Shapes.Count);
            Assert.Equal(s1, ((Group)(g.Shapes[0])).Shapes[0]);
            Assert.Equal(s2, ((Group)(g.Shapes[0])).Shapes[1]);
        }

        [Fact]
        public void SubdividingAGroupPartitionsItsChildObjects()
        {
            // Arrange
            var s1 = new Sphere();
            s1.Transform = Transformations.Translation(-2, -2, 0);
            var s2 = new Sphere();
            s2.Transform = Transformations.Translation(-2, 2, 0);
            var s3 = new Sphere();
            s3.Transform = Transformations.Scaling(4, 4, 4);
            var g = new Group();
            g.AddChildren(new Shape[] {s1, s2, s3});

            // Act
            Shape.Divide(g, 1);

            // Assert
            Assert.Equal(2, g.Shapes.Count);                    // Original group now only has 2 children...
            Assert.Equal(s3, g.Shapes[0]);                      // ...First child is s3...
            Assert.IsType<Group>(g.Shapes[1]);                  // ...Second child is a group
            var subGroup = (Group)g.Shapes[1];
            Assert.Equal(2, subGroup.Shapes.Count);             // The subgroup contains two children...
            Assert.IsType<Group>(subGroup.Shapes[0]);           // ...First child is a group...
            var subSubGroup1 = (Group)subGroup.Shapes[0];
            Assert.Single(subSubGroup1.Shapes);                 // ...Containing a single child...
            Assert.Equal(s1, subSubGroup1.Shapes[0]);           // ...s1
            Assert.IsType<Group>(subGroup.Shapes[1]);           // The second child of the original subgroup is also a group...
            var subSubGroup2 = (Group)subGroup.Shapes[1];
            Assert.Single(subSubGroup2.Shapes);                 // ...Containing a single child...
            Assert.Equal(s2, subSubGroup2.Shapes[0]);           // ...s2
        }

        [Fact]
        public void SubdividingAGroupWithTooFewChildObjects()
        {
            // Arrange
            var s1 = new Sphere();
            s1.Transform = Transformations.Translation(-2, 0, 0);
            var s2 = new Sphere();
            s2.Transform = Transformations.Translation(2, 1, 0);
            var s3 = new Sphere();
            s3.Transform = Transformations.Translation(2, -1, 0);
            var subGroup = new Group();
            subGroup.AddChildren(new Shape[] {s1, s2, s3});
            var s4 = new Sphere();
            var g = new Group();
            g.AddChildren(new Shape[] {subGroup, s4});

            // Act
            Shape.Divide(g, 3);

            // Assert
            Assert.Equal(2, g.Shapes.Count);
            Assert.Equal(subGroup, g.Shapes[0]);
            Assert.Equal(s4, g.Shapes[1]);
            Assert.Equal(2, subGroup.Shapes.Count);
            Assert.IsType<Group>(subGroup.Shapes[0]);
            var subSubGroup1 = (Group)subGroup.Shapes[0];
            Assert.Single(subSubGroup1.Shapes);
            Assert.Equal(s1, subSubGroup1.Shapes[0]);
            Assert.IsType<Group>(subGroup.Shapes[1]);
            var subSubGroup2 = (Group)subGroup.Shapes[1];
            Assert.Equal(2, subSubGroup2.Shapes.Count);
            Assert.Equal(s2, subSubGroup2.Shapes[0]);
            Assert.Equal(s3, subSubGroup2.Shapes[1]);
        }

        [Fact]
        public void CloningAGroup()
        {
            // Arrange
            var orig = new Group();

            // Act
            var clone = orig.Clone();

            // Assert
            Assert.Equal(orig.Origin, clone.Origin);
            Assert.Equal(orig.Material, clone.Material);
            Assert.Equal(orig.Transform, clone.Transform);
            Assert.Equal(orig.Parent, clone.Parent);
            Assert.Equal(orig.Shapes, clone.Shapes);
        }

        [Fact]
        public void StringRepresentation()
        {
            // Arrange
            var expected = "Type:RayTracer.Shapes.Group, Origin:[X:0, Y:0, Z:0, W:1]\nParent:null\nMaterial:[Colour:[R:1, G:1, B:1]\nAmb:0.1,Dif:0.9,Spec:0.9,Shin:200,Refl:0,Tran:0,Refr:1,Shad:True,\nPattern:null\n]\nTransform:[[1, 0, 0, 0,\n0, 1, 0, 0,\n0, 0, 1, 0,\n0, 0, 0, 1]]\nChildren:\n[Type:RayTracer.Tests.TestShape\nId:637603307156449547\nOrigin:[X:0, Y:0, Z:0, W:1]\nParent:637603307156427896\nMaterial:[Colour:[R:1, G:1, B:1]\nAmb:0.1,Dif:0.9,Spec:0.9,Shin:200,Refl:0,Tran:0,Refr:1,Shad:True,\nPattern:null\n]\nTransform:[[1, 0, 0, 0,\n0, 1, 0, 0,\n0, 0, 1, 0,\n0, 0, 0, 1]]\nSavedRay:false]\n[Type:RayTracer.Tests.TestShape\nId:637603307156449806\nOrigin:[X:0, Y:0, Z:0, W:1]\nParent:637603307156427896\nMaterial:[Colour:[R:1, G:1, B:1]\nAmb:0.1,Dif:0.9,Spec:0.9,Shin:200,Refl:0,Tran:0,Refr:1,Shad:True,\nPattern:null\n]\nTransform:[[1, 0, 0, 0,\n0, 1, 0, 0,\n0, 0, 1, 0,\n0, 0, 0, 1]]\nSavedRay:false]\n";
            var orig = new Group();
            var s1 = new TestShape();
            var s2 = new TestShape();
            orig.AddChildren(new Shape[] {s1, s2});

            // Act
            var result = orig.ToString();

            // Assert
            Assert.True(Utilities.ToStringEquals(expected, result));
        }
    }
}
