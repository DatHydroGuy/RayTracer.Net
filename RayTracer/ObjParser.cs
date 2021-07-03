using System;
using System.Linq;
using System.Collections.Generic;
using RayTracer.Shapes;

namespace RayTracer
{
    public class ObjParser
    {
        private int _ignoredLines;
        private List<Point> _vertices;
        private List<Vector> _normals;
        private List<Shape> _faces;
        private Group _rootGroup;

        public int IgnoredLines
        {
            get { return _ignoredLines; }
            set { _ignoredLines = value; }
        }
        public List<Point> Vertices
        {
            get { return _vertices; }
            set { _vertices = value; }
        }
        public List<Vector> Normals
        {
            get { return _normals; }
            set { _normals = value; }
        }
        public List<Shape> Faces
        {
            get { return _faces; }
            set { _faces = value; }
        }
        public Group RootGroup
        {
            get { return _rootGroup; }
            set { _rootGroup = value; }
        }

        public ObjParser()
        {
            IgnoredLines = 0;
            Faces = new List<Shape>();
            RootGroup = new Group();
            Vertices = new List<Point>();
            Vertices.Add(null);                                      // 0th element is always unused
            Normals = new List<Vector>();
            Normals.Add(null);                                       // 0th element is always unused
        }

        public void ReadFile(string objFile)
        {
            var rawLines = System.IO.File.ReadAllLines(objFile).ToList<string>();
            ParseFile(rawLines);
        }

        public Group ObjToGroup()
        {
            return RootGroup;
        }

        public void ParseFile(List<string> rawLines)
        {
            var lines = rawLines.Where(x => x.StartsWith('v') || x.StartsWith('f') || x.StartsWith('g'));
            IgnoredLines = rawLines.Count - lines.Count();
            ProcessDataLines(lines);
            var minPt = RootGroup.GetBoundingBox().MinPoint;
            var maxPt = RootGroup.GetBoundingBox().MaxPoint;
            Console.WriteLine($"Bounding box of imported object\r\n===============================\r\n\t\tX\t\tY\t\tZ\r\nMinimums:{minPt.X,16:F8}{minPt.Y,16:F8}{minPt.Z,16:F8}\r\nMaximums:{maxPt.X,16:F8}{maxPt.Y,16:F8}{maxPt.Z,16:F8}");
        }

        public void ProcessDataLines(IEnumerable<string> dataLines)
        {
            var currentGroup = RootGroup;
            foreach (var dataLine in dataLines)
            {
                string[] lineData = dataLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (lineData[0] == "g")
                {
                    currentGroup = new Group();
                    RootGroup.AddChild(currentGroup);
                }
                else if (lineData[0] == "f")
                {
                    int[] vertexIndices = null;
                    int[] vertexTextures = null;
                    int[] vertexNormals = null;
                    var vertexIndexData = lineData.Skip(1).ToList();
                    if (vertexIndexData.FindAll(x => x.Contains('/')).Count > 0)
                    {
                        vertexIndices = new int[vertexIndexData.Count];
                        vertexTextures = new int[vertexIndexData.Count];
                        vertexNormals = new int[vertexIndexData.Count];

                        for (int i = 0; i < vertexIndexData.Count; i++)
                        {
                            var components = vertexIndexData[i].Split('/');
                            int temp;
                            if (int.TryParse(components[0], out temp))
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
                }
                else if (lineData[0] == "v")
                {
                    Vertices.Add(new Point(double.Parse(lineData[1]), double.Parse(lineData[2]), double.Parse(lineData[3])));
                }
                else if (lineData[0] == "vn")
                {
                    Normals.Add(new Vector(double.Parse(lineData[1]), double.Parse(lineData[2]), double.Parse(lineData[3])));
                }
            }
        }

        public void FanTriangulate(int[] vertices, Group currGroup, int[] normals = null)
        {
            Shape newFace;
            for (int i = 2; i < vertices.Length; i++)
            {
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
