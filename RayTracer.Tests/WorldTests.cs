using System;
using Xunit;

namespace RayTracer.Tests
{
    public class WorldTests
    {
        [Fact]
        public void CreatingAWorld()
        {
            // Arrange
            var w = new World();

            // Act

            // Assert
            Assert.Empty(w.Objects);
            Assert.Empty(w.Lights);
        }

        [Fact]
        public void TheDefaultWorld()
        {
            // Arrange
            var light = Light.PointLight(new Point(-10, 10, -10), new Colour(1, 1, 1));
            var s1 = new Sphere();
            s1.Material.Colour = new Colour(0.8, 1, 0.6);
            s1.Material.Diffuse = 0.7;
            s1.Material.Specular = 0.2;
            var s2= new Sphere();
            s2.Transform = Transformations.Scaling(0.5, 0.5, 0.5);

            // Act
            var w = World.DefaultWorld();

            // Assert
            Assert.Equal(light, w.Lights[0]);
            Assert.Contains<Shape>(s1, w.Objects);
            Assert.Contains<Shape>(s2, w.Objects);
        }

        [Fact]
        public void IntersectAWorldWithARay()
        {
            // Arrange
            var w = World.DefaultWorld();
            var r = new Ray(new Point(0, 0, -5), new Vector(0, 0, 1));

            // Act
            var xs = w.IntersectWorld(r);

            // Assert
            Assert.Equal(4, xs.Length);
            Assert.Equal(4, xs[0].T);
            Assert.Equal(4.5, xs[1].T);
            Assert.Equal(5.5, xs[2].T);
            Assert.Equal(6, xs[3].T);
        }

        [Fact]
        public void ShadingAnIntersection()
        {
            // Arrange
            var w = World.DefaultWorld();
            var r = new Ray(new Point(0, 0, -5), new Vector(0, 0, 1));
            var s = w.Objects[0];
            var i = new Intersection(4, s);

            // Act
            var comps = i.PrepareComputations(r);
            var c = w.ShadeHit(comps);

            // Assert
            Assert.Equal(new Colour(0.38066, 0.47583, 0.2855), c);
        }

        [Fact]
        public void ShadingAnIntersectionFromTheInside()
        {
            // Arrange
            var w = World.DefaultWorld();
            w.Lights[0] = Light.PointLight(new Point(0, 0.25, 0), new Colour(1, 1, 1));
            var r = new Ray(new Point(0, 0, 0), new Vector(0, 0, 1));
            var s = w.Objects[1];
            var i = new Intersection(0.5, s);

            // Act
            var comps = i.PrepareComputations(r);
            var c = w.ShadeHit(comps);

            // Assert
            Assert.Equal(new Colour(0.90498, 0.90498, 0.90498), c);
        }

        [Fact]
        public void TheColourWhenARayMisses()
        {
            // Arrange
            var w = World.DefaultWorld();
            var r = new Ray(new Point(0, 0, -5), new Vector(0, 1, 0));

            // Act
            var c = w.ColourAt(r);

            // Assert
            Assert.Equal(new Colour(0, 0, 0), c);
        }

        [Fact]
        public void TheColourWhenARayHits()
        {
            // Arrange
            var w = World.DefaultWorld();
            var r = new Ray(new Point(0, 0, -5), new Vector(0, 0, 1));

            // Act
            var c = w.ColourAt(r);

            // Assert
            Assert.Equal(new Colour(0.38066, 0.47583, 0.2855), c);
        }

        [Fact]
        public void TheColourWithAnIntersectionBehindTheRay()
        {
            // Arrange
            var w = World.DefaultWorld();
            var outer = w.Objects[0];
            outer.Material.Ambient = 1;
            var inner = w.Objects[1];
            inner.Material.Ambient = 1;
            var r = new Ray(new Point(0, 0, 0.75), new Vector(0, 0, -1));

            // Act
            var c = w.ColourAt(r);

            // Assert
            Assert.Equal(inner.Material.Colour, c);
        }

        [Fact]
        public void ThereIsNoShadowWhenNothingIsCollinearWithPointAndLight()
        {
            // Arrange
            var w = World.DefaultWorld();
            var p = new Point(0, 10, 0);

            // Act
            var result = w.IsShadowed(p);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ThereIsAShadowWhenObjectBetweenPointAndLight()
        {
            // Arrange
            var w = World.DefaultWorld();
            var p = new Point(10, -10, 10);

            // Act
            var result = w.IsShadowed(p);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ThereIsNoShadowWhenAnObjectIsBehindTheLight()
        {
            // Arrange
            var w = World.DefaultWorld();
            var p = new Point(-20, 20, -20);

            // Act
            var result = w.IsShadowed(p);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ThereIsNoShadowWhenPointIsBetweenObjectAndLight()
        {
            // Arrange
            var w = World.DefaultWorld();
            var p = new Point(-2, 2, -2);

            // Act
            var result = w.IsShadowed(p);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CorrectlyColouringAPointInShadow()
        {
            // Arrange
            var w = new World();
            w.Lights = new Light[] {Light.PointLight(new Point(0, 0, -10), new Colour(1, 1, 1))};
            var s1 = new Sphere();
            var s2 = new Sphere();
            s2.Transform = Transformations.Translation(0, 0, 10);
            w.Objects = new Sphere[] {s1, s2};
            var ray = new Ray(new Point(0, 0, 5), new Vector(0, 0, 1));
            var i = new Intersection(4, s2);
            var expected = new Colour(0.1, 0.1, 0.1);

            // Act
            var comps = i.PrepareComputations(ray);
            var c = w.ShadeHit(comps);

            // Assert
            Assert.Equal(expected, c);
        }

        [Fact]
        public void ReflectedColourForANonreflectiveMaterial()
        {
            // Arrange
            var w = World.DefaultWorld();
            var ray = new Ray(new Point(0, 0, 0), new Vector(0, 0, 1));
            var shape = w.Objects[1];
            shape.Material.Ambient = 1;
            var i = new Intersection(1, shape);
            var expected = Colour.BLACK;

            // Act
            var comps = i.PrepareComputations(ray);
            var c = w.ReflectedColour(comps);

            // Assert
            Assert.Equal(expected, c);
        }

        [Fact]
        public void ReflectedColourForAReflectiveMaterial()
        {
            // Arrange
            var halfSqrt2 = Math.Sqrt(2) / 2.0;
            var w = World.DefaultWorld();
            var shape = new Plane();
            shape.Transform = Transformations.Translation(0, -1, 0);
            shape.Material.Reflective = 0.5;
            w.AddShapeToWorld(shape);
            var ray = new Ray(new Point(0, 0, -3), new Vector(0, -halfSqrt2, halfSqrt2));
            var i = new Intersection(Math.Sqrt(2), shape);
            var expected = new Colour(0.19033, 0.23792, 0.14275);

            // Act
            var comps = i.PrepareComputations(ray);
            var c = w.ReflectedColour(comps);

            // Assert
            Assert.Equal(expected, c);
        }

        [Fact]
        public void TheShadeHitFunctionIncorporatesReflectiveMaterial()
        {
            // Arrange
            var halfSqrt2 = Math.Sqrt(2) / 2.0;
            var w = World.DefaultWorld();
            var shape = new Plane();
            shape.Transform = Transformations.Translation(0, -1, 0);
            shape.Material.Reflective = 0.5;
            w.AddShapeToWorld(shape);
            var ray = new Ray(new Point(0, 0, -3), new Vector(0, -halfSqrt2, halfSqrt2));
            var i = new Intersection(Math.Sqrt(2), shape);
            var expected = new Colour(0.87676, 0.92434, 0.82917);

            // Act
            var comps = i.PrepareComputations(ray);
            var c = w.ShadeHit(comps);

            // Assert
            Assert.Equal(expected, c);
        }

        [Fact]
        public void TheColourAtFunctionAvoidsInfiniteRecursion()
        {
            // Arrange
            var w = new World();
            w.AddLightToWorld(Light.PointLight(new Point(0, 0, 0), Colour.WHITE));
            var lower = new Plane();
            lower.Transform = Transformations.Translation(0, -1, 0);
            lower.Material.Reflective = 1;
            w.AddShapeToWorld(lower);
            var upper = new Plane();
            upper.Transform = Transformations.Translation(0, 1, 0);
            upper.Material.Reflective = 1;
            w.AddShapeToWorld(upper);
            var ray = new Ray(new Point(0, 0, 0), new Vector(0, 1, 0));

            // Act
            var c = w.ColourAt(ray);

            // Assert - execution should not get here if infinite recursion blows the stack
            Assert.Equal(1, 1);
        }

        [Fact]
        public void TheReflectedColourAtTheMaximumRecursiveDepth()
        {
            // Arrange
            var halfSqrt2 = Math.Sqrt(2) / 2.0;
            var w = World.DefaultWorld();
            var shape = new Plane();
            shape.Transform = Transformations.Translation(0, -1, 0);
            shape.Material.Reflective = 0.5;
            w.AddShapeToWorld(shape);
            var ray = new Ray(new Point(0, 0, -3), new Vector(0, -halfSqrt2, halfSqrt2));
            var i = new Intersection(Math.Sqrt(2), shape);
            var expected = Colour.BLACK;

            // Act
            var comps = i.PrepareComputations(ray);
            var c = w.ReflectedColour(comps, 0);

            // Assert
            Assert.Equal(expected, c);
        }

        [Fact]
        public void RefractedColourWithAnOpaqueSurface()
        {
            // Arrange
            var w = World.DefaultWorld();
            var s = w.Objects[0];
            var r = new Ray(new Point(0, 0, -5), new Vector(0, 0, 1));
            var xs = Intersection.Intersections(new Intersection(4, s), new Intersection(6, s));

            // Act
            var comps = xs[0].PrepareComputations(r, xs);
            var c = w.RefractedColour(comps, 5);

            // Assert
            Assert.Equal(Colour.BLACK, c);
        }

        [Fact]
        public void RefractedColourAtMaximumRecursiveDepth()
        {
            // Arrange
            var w = World.DefaultWorld();
            var s = w.Objects[0];
            s.Material.Transparency = 1.0;
            s.Material.RefractiveIndex = 1.5;
            var r = new Ray(new Point(0, 0, -5), new Vector(0, 0, 1));
            var xs = Intersection.Intersections(new Intersection(4, s), new Intersection(6, s));

            // Act
            var comps = xs[0].PrepareComputations(r, xs);
            var c = w.RefractedColour(comps, 0);

            // Assert
            Assert.Equal(Colour.BLACK, c);
        }

        [Fact]
        public void RefractedColourUnderTotalInternalReflection()
        {
            // Arrange
            var halfSqrt2 = Math.Sqrt(2) / 2.0;
            var w = World.DefaultWorld();
            var s = w.Objects[0];
            s.Material.Transparency = 1.0;
            s.Material.RefractiveIndex = 1.5;
            var r = new Ray(new Point(0, 0, halfSqrt2), new Vector(0, 1, 0));
            var xs = Intersection.Intersections(new Intersection(-halfSqrt2, s), new Intersection(halfSqrt2, s));

            // Act - as we're inside the sphere here, we need to examine the SECOND intersection xs[1], not xs[0]
            var comps = xs[1].PrepareComputations(r, xs);
            var c = w.RefractedColour(comps, 5);

            // Assert
            Assert.Equal(Colour.BLACK, c);
        }

        [Fact]
        public void RefractedColourWithARefractedRay()
        {
            // Arrange
            var w = World.DefaultWorld();
            var a = w.Objects[0];
            a.Material.Ambient = 1.0;
            a.Material.Pattern = new TestPattern(Colour.WHITE, Colour.BLACK);
            var b = w.Objects[1];
            b.Material.Transparency = 1.0;
            b.Material.RefractiveIndex = 1.5;
            var r = new Ray(new Point(0, 0, 0.1), new Vector(0, 1, 0));
            var xs = Intersection.Intersections(new Intersection(-0.9899, a), new Intersection(-0.4899, b), new Intersection(0.4899, b), new Intersection(0.9899, a));
            var expected = new Colour(0, 0.99887, 0.04722);

            // Act
            var comps = xs[2].PrepareComputations(r, xs);
            var c = w.RefractedColour(comps, 5);

            // Assert
            Assert.Equal(expected, c);
        }

        [Fact]
        public void HandlingRefractionWithinTheShadeHitFunction()
        {
            // Arrange
            var halfSqrt2 = Math.Sqrt(2) / 2.0;
            var w = World.DefaultWorld();
            var floor = new Plane();
            floor.Transform = Transformations.Translation(0, -1, 0);
            floor.Material.Transparency = 0.5;
            floor.Material.RefractiveIndex = 1.5;
            w.AddShapeToWorld(floor);
            var ball = new Sphere();
            ball.Transform = Transformations.Translation(0, -3.5, -0.5);
            ball.Material.Colour = new Colour(1, 0, 0);
            ball.Material.Ambient = 0.5;
            w.AddShapeToWorld(ball);
            var r = new Ray(new Point(0, 0, -3), new Vector(0, -halfSqrt2, halfSqrt2));
            var xs = Intersection.Intersections(new Intersection(Math.Sqrt(2), floor));
            var expected = new Colour(0.93642, 0.68642, 0.68642);

            // Act
            var comps = xs[0].PrepareComputations(r, xs);
            var c = w.ShadeHit(comps, 5);

            // Assert
            Assert.Equal(expected, c);
        }

        [Fact]
        public void HandlingReflectanceWithinTheShadeHitFunction()
        {
            // Arrange
            var halfSqrt2 = Math.Sqrt(2) / 2.0;
            var w = World.DefaultWorld();
            var floor = new Plane();
            floor.Transform = Transformations.Translation(0, -1, 0);
            floor.Material.Reflective = 0.5;
            floor.Material.Transparency = 0.5;
            floor.Material.RefractiveIndex = 1.5;
            w.AddShapeToWorld(floor);
            var ball = new Sphere();
            ball.Transform = Transformations.Translation(0, -3.5, -0.5);
            ball.Material.Colour = new Colour(1, 0, 0);
            ball.Material.Ambient = 0.5;
            w.AddShapeToWorld(ball);
            var r = new Ray(new Point(0, 0, -3), new Vector(0, -halfSqrt2, halfSqrt2));
            var xs = Intersection.Intersections(new Intersection(Math.Sqrt(2), floor));
            var expected = new Colour(0.93391, 0.69643, 0.69243);

            // Act
            var comps = xs[0].PrepareComputations(r, xs);
            var c = w.ShadeHit(comps, 5);

            // Assert
            Assert.Equal(expected, c);
        }
    }
}
