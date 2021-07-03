using System;
using System.Collections.Generic;
using RayTracer.Shapes;
using Xunit;

namespace RayTracer.Tests
{
    public class ObjParserTests
    {
        [Fact]
        public void ReadingAFile()
        {
            // Arrange
            var testFile = @"I:\Programming\DotNetCore\RayTracer\RayTracer\RayTracer.Tests\gibberish.obj";
            var parser = new ObjParser();

            // Act
            parser.ReadFile(testFile);

            // Assert
            Assert.True(parser.IgnoredLines > 0);
        }

        [Fact]
        public void IgnoringUnrecognisedLines()
        {
            // Arrange
            var testFile = @"I:\Programming\DotNetCore\RayTracer\RayTracer\RayTracer.Tests\gibberish.obj";
            var parser = new ObjParser();

            // Act
            parser.ReadFile(testFile);

            // Assert
            Assert.Equal(5, parser.IgnoredLines);
        }

        [Fact]
        public void ReadingVertices()
        {
            // Arrange
            var testFile = "v -1 1 0\r\nv -1.0000 0.5000 0.0000\r\nv 1 0 0\r\nv 1 1 0";
            var fileLines = new List<string>(testFile.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));
            var parser = new ObjParser();

            // Act
            parser.ParseFile(fileLines);

            // Assert
            Assert.Equal(new Point(-1, 1, 0), parser.Vertices[1]);
            Assert.Equal(new Point(-1, 0.5, 0), parser.Vertices[2]);
            Assert.Equal(new Point(1, 0, 0), parser.Vertices[3]);
            Assert.Equal(new Point(1, 1, 0), parser.Vertices[4]);
        }

        [Fact]
        public void ParsingTriangleFaces()
        {
            // Arrange
            var testFile = "v -1 1 0\r\nv -1 0 0\r\nv 1 0 0\r\nv 1 1 0\r\n\r\nf 1 2 3\r\nf 1 3 4";
            var fileLines = new List<string>(testFile.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));
            var expectedTriangle1 = new Triangle(new Point(-1, 1, 0), new Point(-1, 0, 0), new Point(1, 0, 0));
            var expectedTriangle2 = new Triangle(new Point(-1, 1, 0), new Point(1, 0, 0), new Point(1, 1, 0));
            var parser = new ObjParser();

            // Act
            parser.ParseFile(fileLines);
            var g = parser.RootGroup.Shapes;

            // Assert
            Assert.Equal(expectedTriangle1, g[0]);
            Assert.Equal(expectedTriangle2, g[1]);
            Assert.Equal(new Point(-1, 1, 0), ((Triangle)g[0]).Vertex1);
            Assert.Equal(new Point(-1, 0, 0), ((Triangle)g[0]).Vertex2);
            Assert.Equal(new Point(1, 0, 0), ((Triangle)g[0]).Vertex3);
            Assert.Equal(new Point(-1, 1, 0), ((Triangle)g[1]).Vertex1);
            Assert.Equal(new Point(1, 0, 0), ((Triangle)g[1]).Vertex2);
            Assert.Equal(new Point(1, 1, 0), ((Triangle)g[1]).Vertex3);
        }

        [Fact]
        public void TriangulatingPolygonFaces()
        {
            // Arrange
            var testFile = "v -1 1 0\r\nv -1 0 0\r\nv 1 0 0\r\nv 1 1 0\r\nv 0 2 0\r\n\r\nf 1 2 3 4 5";
            var fileLines = new List<string>(testFile.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));
            var expectedTriangle1 = new Triangle(new Point(-1, 1, 0), new Point(-1, 0, 0), new Point(1, 0, 0));
            var expectedTriangle2 = new Triangle(new Point(-1, 1, 0), new Point(1, 0, 0), new Point(1, 1, 0));
            var expectedTriangle3 = new Triangle(new Point(-1, 1, 0), new Point(1, 1, 0), new Point(0, 2, 0));
            var parser = new ObjParser();

            // Act
            parser.ParseFile(fileLines);
            var f = parser.Faces;

            // Assert
            Assert.Equal(expectedTriangle1, f[0]);
            Assert.Equal(expectedTriangle2, f[1]);
            Assert.Equal(expectedTriangle3, f[2]);
            Assert.Equal(new Point(-1, 1, 0), ((Triangle)f[0]).Vertex1);
            Assert.Equal(new Point(-1, 0, 0), ((Triangle)f[0]).Vertex2);
            Assert.Equal(new Point(1, 0, 0), ((Triangle)f[0]).Vertex3);
            Assert.Equal(new Point(-1, 1, 0), ((Triangle)f[1]).Vertex1);
            Assert.Equal(new Point(1, 0, 0), ((Triangle)f[1]).Vertex2);
            Assert.Equal(new Point(1, 1, 0), ((Triangle)f[1]).Vertex3);
            Assert.Equal(new Point(-1, 1, 0), ((Triangle)f[2]).Vertex1);
            Assert.Equal(new Point(1, 1, 0), ((Triangle)f[2]).Vertex2);
            Assert.Equal(new Point(0, 2, 0), ((Triangle)f[2]).Vertex3);
        }

        [Fact]
        public void ParsingNamedGroups()
        {
            // Arrange
            var testFile = "v -1 1 0\r\nv -1 0 0\r\nv 1 0 0\r\nv 1 1 0\r\n\r\ng FirstGroup\r\nf 1 2 3\r\ng SecondGroup\r\nf 1 3 4";
            var fileLines = new List<string>(testFile.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));
            var expectedTriangle1 = new Triangle(new Point(-1, 1, 0), new Point(-1, 0, 0), new Point(1, 0, 0));
            var expectedTriangle2 = new Triangle(new Point(-1, 1, 0), new Point(1, 0, 0), new Point(1, 1, 0));
            var parser = new ObjParser();

            // Act
            parser.ParseFile(fileLines);
            var g1 = ((Group)(parser.RootGroup.Shapes[0])).Shapes;
            var g2 = ((Group)(parser.RootGroup.Shapes[1])).Shapes;

            // Assert
            Assert.Equal(expectedTriangle1, g1[0]);
            Assert.Equal(expectedTriangle2, g2[0]);
            Assert.Equal(new Point(-1, 1, 0), ((Triangle)g1[0]).Vertex1);
            Assert.Equal(new Point(-1, 0, 0), ((Triangle)g1[0]).Vertex2);
            Assert.Equal(new Point(1, 0, 0), ((Triangle)g1[0]).Vertex3);
            Assert.Equal(new Point(-1, 1, 0), ((Triangle)g2[0]).Vertex1);
            Assert.Equal(new Point(1, 0, 0), ((Triangle)g2[0]).Vertex2);
            Assert.Equal(new Point(1, 1, 0), ((Triangle)g2[0]).Vertex3);
        }

        [Fact]
        public void ConvertingAnObjFileIntoAGroupObject()
        {
            // Arrange
            var testFile = "v -1 1 0\r\nv -1 0 0\r\nv 1 0 0\r\nv 1 1 0\r\n\r\ng FirstGroup\r\nf 1 2 3\r\ng SecondGroup\r\nf 1 3 4";
            var fileLines = new List<string>(testFile.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));
            var expectedTriangle1 = new Triangle(new Point(-1, 1, 0), new Point(-1, 0, 0), new Point(1, 0, 0));
            var expectedTriangle2 = new Triangle(new Point(-1, 1, 0), new Point(1, 0, 0), new Point(1, 1, 0));
            var parser = new ObjParser();
            parser.ParseFile(fileLines);

            // Act
            var g = parser.ObjToGroup();
            var g1 = ((Group)(parser.RootGroup.Shapes[0])).Shapes;
            var g2 = ((Group)(parser.RootGroup.Shapes[1])).Shapes;

            // Assert
            Assert.Contains<Shape>(expectedTriangle1, g1);
            Assert.Contains<Shape>(expectedTriangle2, g2);
        }

        [Fact]
        public void CorrectlyImportingVertexNormalData()
        {
            // Arrange
            var testFile = "vn 0 0 1\r\nvn 0.707 0 -0.707\r\nvn 1 2 3";
            var fileLines = new List<string>(testFile.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));
            var expectedNormal1 = new Vector(0, 0, 1);
            var expectedNormal2 = new Vector(0.707, 0, -0.707);
            var expectedNormal3 = new Vector(1, 2, 3);
            var parser = new ObjParser();

            // Act
            parser.ParseFile(fileLines);

            // Assert
            Assert.Equal(expectedNormal1, parser.Normals[1]);
            Assert.Equal(expectedNormal2, parser.Normals[2]);
            Assert.Equal(expectedNormal3, parser.Normals[3]);
        }

        [Fact]
        public void VertexNormalDataShouldBeCorrectlyAssociatedWithFaceData()
        {
            // Arrange
            var testFile = "v 0 1 0\r\nv -1 0 0\r\nv 1 0 0\r\n\r\nvn -1 0 0\r\nvn 1 0 0\r\nvn 0 1 0\r\n\r\nf 1//3 2//1 3//2\r\nf 1/0/3 2/102/1 3/14/2";
            var fileLines = new List<string>(testFile.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));
            var parser = new ObjParser();

            // Act
            parser.ParseFile(fileLines);
            var g = parser.ObjToGroup();
            var g1 = (SmoothTriangle)(g.Shapes[0]);
            var g2 = (SmoothTriangle)(g.Shapes[1]);

            // Assert
            Assert.Equal(g1.Vertex1, parser.Vertices[1]);
            Assert.Equal(g1.Vertex2, parser.Vertices[2]);
            Assert.Equal(g1.Vertex3, parser.Vertices[3]);
            Assert.Equal(g1.Normal1, parser.Normals[3]);
            Assert.Equal(g1.Normal2, parser.Normals[1]);
            Assert.Equal(g1.Normal3, parser.Normals[2]);
            Assert.Equal(g1, g2);
        }

        [Fact]
        public void BoundsOfTheImportedObjectAreCaptured()
        {
            // Arrange
            var testFile = "v 0.7 11 2.2\r\nv -14 0 -3.1\r\nv 1 -4.3 10\r\n\r\nvn -1 0 0\r\nvn 1 0 0\r\nvn 0 1 0\r\n\r\nf 1//3 2//1 3//2\r\nf 1/0/3 2/102/1 3/14/2";
            var fileLines = new List<string>(testFile.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));
            var parser = new ObjParser();
            var expectedMin = new Point(-14, -4.3, -3.1);
            var expectedMax = new Point(1, 11, 10);

            // Act
            parser.ParseFile(fileLines);
            var g = parser.ObjToGroup();
            
            // Assert
            Assert.Equal(expectedMin, g.GetBoundingBox().MinPoint);
            Assert.Equal(expectedMax, g.GetBoundingBox().MaxPoint);
        }
    }
}
