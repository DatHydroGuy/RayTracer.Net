using System;
using System.Linq;
using System.Collections.Generic;
using RayTracer.Shapes;

namespace RayTracer
{
    public class ObjParser
    {
        public int IgnoredLines { get; private set; }

        public List<Point> Vertices { get; }

        public List<Vector> Normals { get; }

        public List<Shape> Faces { get; }

        public Group RootGroup { get; }

        public ObjParser()
        {
            IgnoredLines = 0;
            Faces = new List<Shape>();
            RootGroup = new Group();
            Vertices = new List<Point> { null }; // 0th element is always unused
            Normals = new List<Vector> { null }; // 0th element is always unused
        }

        public void ReadFile(string objFile)
        {
            var rawLines = System.IO.File.ReadAllLines(objFile).ToList();
            ParseFile(rawLines);
        }

        public Group ObjToGroup()
        {
            return RootGroup;
        }

        public void ParseFile(List<string> rawLines)
        {
            var lines = rawLines.Where(x => x.StartsWith('v') || x.StartsWith('f') || x.StartsWith('g')).ToList();
            IgnoredLines = rawLines.Count - lines.Count;
            ProcessDataLines(lines);
            var minPt = RootGroup.GetBoundingBox().MinPoint;
            var maxPt = RootGroup.GetBoundingBox().MaxPoint;
            Console.WriteLine($"Bounding box of imported object\r\n===============================\r\n\t\tX\t\tY\t\tZ\r\nMinimums:{minPt.X,16:F8}{minPt.Y,16:F8}{minPt.Z,16:F8}\r\nMaximums:{maxPt.X,16:F8}{maxPt.Y,16:F8}{maxPt.Z,16:F8}");
        }

        private void ProcessDataLines(IEnumerable<string> dataLines)
        {
            var currentGroup = RootGroup;
            foreach (var dataLine in dataLines)
            {
                var lineData = dataLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                switch (lineData[0])
                {
                    case "g":
                        currentGroup = new Group();
                        RootGroup.AddChild(currentGroup);
                        break;
                    case "f":
                    {
                        int[] vertexIndices;
                        int[] vertexNormals = null;
                        var vertexIndexData = lineData.Skip(1).ToList();
                        if (vertexIndexData.FindAll(x => x.Contains('/')).Count > 0)
                        {
                            vertexIndices = new int[vertexIndexData.Count];
                            var vertexTextures = new int[vertexIndexData.Count];
                            vertexNormals = new int[vertexIndexData.Count];

                            for (var i = 0; i < vertexIndexData.Count; i++)
                            {
                                var components = vertexIndexData[i].Split('/');
                                if (int.TryParse(components[0], out var temp))
                                    vertexIndices[i] = temp;
                                if (int.TryParse(components[1], out temp))
                                    vertexTextures[i] = temp;
                                if (int.TryParse(components[2], out temp))
                                    vertexNormals[i] = temp;
                            }
                        }
                        else
                        {
                            vertexIndices = lineData.Skip(1).Select(int.Parse).ToArray();
                        }
                        FanTriangulate(vertexIndices, currentGroup, vertexNormals);
                        break;
                    }
                    case "v":
                        Vertices.Add(new Point(double.Parse(lineData[1]), double.Parse(lineData[2]), double.Parse(lineData[3])));
                        break;
                    case "vn":
                        Normals.Add(new Vector(double.Parse(lineData[1]), double.Parse(lineData[2]), double.Parse(lineData[3])));
                        break;
                }
            }
        }

        private void FanTriangulate(int[] vertices, Group currGroup, int[] normals = null)
        {
            for (var i = 2; i < vertices.Length; i++)
            {
                Shape newFace;
                if (normals == null)
                {
                    newFace = new Triangle(Vertices[vertices[0]], Vertices[vertices[i - 1]], Vertices[vertices[i]]);
                }
                else
                {
                    newFace = new SmoothTriangle(Vertices[vertices[0]], Vertices[vertices[i - 1]], Vertices[vertices[i]],
                                                 Normals[normals[0]], Normals[normals[i - 1]], Normals[normals[i]]);
                }
                currGroup.AddChild(newFace);
                Faces.Add(newFace);
            }
        }
    }
}
