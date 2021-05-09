using System.Linq;
using System.Collections.Generic;
using RayTracer;

namespace RayTracerApp
{
    public class YamlReader
    {
        private List<string> materialList = new List<string>{"ambient", "diffuse", "specular", "shininess", "reflective", "transparency", "refractive-index", "casts-shadow", "pattern"};
        private List<string> transformList = new List<string>{"translate", "scale", "rotate-x", "rotate-y", "rotate-z"};
        private Dictionary<string, List<string>> _definitions;

        public YamlReader(string yamlFile)
        {
            if (!System.IO.File.Exists(yamlFile))
                System.Console.WriteLine($"YAML file {yamlFile} not found.");

            var rawLines = System.IO.File.ReadAllLines(yamlFile).ToList<string>();
            var lines = rawLines.Where(x => !x.StartsWith('#') && x.Length > 0).ToList();
            var rawList = new List<List<string>>();
            var currList = new List<string>();
            foreach (var line in lines)
            {
                var hashIndex = line.Contains('#') ? line.IndexOf('#') : line.Length;
                if (line.StartsWith("- "))
                {
                    rawList.Add(currList);
                    currList = new List<string>();
                }
                currList.Add(line.Substring(0, hashIndex));
            }
            rawList.Add(currList);
            rawList.RemoveAt(0);
            ProcessRawObjectData(rawList);
        }

        private void ProcessRawObjectData(List<List<string>> rawList)
        {
            var addList = rawList.Where(x => x[0].StartsWith("- add")).ToList();
            var defineList = rawList.Where(x => x[0].StartsWith("- define")).ToList();
            var outFile = TrimFromColonPosition(rawList.Where(x => x[0].StartsWith("- file")).ToList()[0][0]);
            if (System.Diagnostics.Debugger.IsAttached)
                outFile = "..\\" + outFile;
            outFile = System.IO.Path.GetFullPath(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), outFile));
            System.Console.WriteLine($"Writing to file: {outFile}");
            _definitions = ProcessDefineList(defineList);
            var canvas = ProcessAddList(addList);
            canvas.WritePpmFile(outFile);
            System.Console.WriteLine("Image rendering complete");
        }

        private string TrimToColonPosition(string textToScan)
        {
            int colonPos = System.Math.Max(0, textToScan.IndexOf(':'));
            return textToScan.Substring(0, colonPos).Trim().ToLower();
        }

        private string TrimFromColonPosition(string textToScan)
        {
            int colonPos = textToScan.LastIndexOf(':');
            return textToScan.Substring(colonPos + 1).Trim().ToLower();
        }

        private string TrimLeadingHyphen(string textToScan)
        {
            int hyphenPos = System.Math.Max(0, textToScan.IndexOf('-'));
            return textToScan.Substring(hyphenPos + 1).Trim().ToLower();
        }

        private Dictionary<string, List<string>> ProcessDefineList(List<List<string>> defList)
        {
            var pattern = new System.Text.RegularExpressions.Regex(@"[\[\]\s]");
            var definitions = new Dictionary<string, List<string>>();
            foreach (var definition in defList)
            {
                var valuesList = new List<string>();
                var name = TrimFromColonPosition(definition[0]);
                var extendIndex = definition.FindIndex(x => x.TrimStart().ToLower().StartsWith("extend"));
                if (extendIndex > -1)
                {
                    var extension = TrimFromColonPosition(definition[extendIndex]);
                    valuesList.AddRange(definitions[extension]);
                }

                var valueIndex = definition.FindIndex(x => x.TrimStart().ToLower().StartsWith("value"));
                var valueLines = definition.Select((e, i) => new {Element = e.Trim(), Index = i}).Where(x => x.Index > valueIndex).ToList();
                for (int i = 0; i < valueLines.Count; i++)
                {
                    var currValue = valueLines[i].Element;
                    if (currValue.StartsWith("-"))
                    {
                        // Probably a transform definition.  Need to check it for pre-existing definitions
                        if (currValue.Contains('[') && currValue.Contains(']'))
                        {
                            // Transformation operation
                            valuesList.Add(currValue);
                        }
                        else
                        {
                            // Pre-existing definition, so perform a lookup & replace
                            valuesList.AddRange(definitions[TrimLeadingHyphen(currValue)]);
                        }
                    }
                    else
                    {
                        valuesList.Add(valueLines[i].Element);
                    }
                }
                definitions.Add(name, valuesList);
            }
            return definitions;
        }

        private Canvas ProcessAddList(List<List<string>> objList)
        {
            var worldObjects = objList.Where(x => TrimFromColonPosition(x[0]) != "camera").ToList();
            var cameraObject = objList.Where(x => TrimFromColonPosition(x[0]) == "camera").First();
            var camera = ProcessCameraObject(cameraObject);
            var world = new World();
            foreach (var obj in worldObjects)
            {
                ProcessWorldObject(world, obj);
            }
            return camera.Render(world, showProgress: true);
        }

        private Camera ProcessCameraObject(List<string> objData)
        {
            int width = 200;
            int height = 200;
            double fieldOfView = 0.785;
            var from = new Point();
            var to = new Point();
            var up = new Vector();

            for (int i = 1; i < objData.Count; i++)
            {
                var temp = objData[i].ToLower().Trim();
                if (temp.StartsWith("width"))
                {
                    width = ExtractInteger(TrimFromColonPosition(temp), "Camera width");
                }
                else if (temp.StartsWith("height"))
                {
                    height = ExtractInteger(TrimFromColonPosition(temp), "Camera height");
                }
                else if (temp.StartsWith("field-of-view"))
                {
                    fieldOfView = ExtractDouble(TrimFromColonPosition(temp), "Camera FoV");
                }
                else if (temp.StartsWith("from"))
                {
                    var tuple = ExtractTuple(TrimFromColonPosition(temp), "Camera from");
                    from = new Point(tuple[0], tuple[1], tuple[2]);
                }
                else if (temp.StartsWith("to"))
                {
                    var tuple = ExtractTuple(TrimFromColonPosition(temp), "Camera to");
                    to = new Point(tuple[0], tuple[1], tuple[2]);
                }
                else if (temp.StartsWith("up"))
                {
                    var tuple = ExtractTuple(TrimFromColonPosition(temp), "Camera up");
                    up = new Vector(tuple[0], tuple[1], tuple[2]);
                }
                else
                {
                    throw new System.Exception($"Unknown camera attribute: {temp}");
                }
            }

            var camera = new Camera(width, height, fieldOfView);
            camera.Transform = Transformations.ViewTransform(from, to, up);
            return camera;
        }

        private void ProcessWorldObject(World world, List<string> objData)
        {
            switch (TrimFromColonPosition(objData[0]))
            {
                case "light":
                    world.AddLightToWorld(ProcessLight(objData));
                    break;
                case "plane":
                case "sphere":
                case "cube":
                case "cone":
                case "cylinder":
                case "triangle":
                case "smooth-triangle":
                    world.AddShapeToWorld(ProcessShape(objData));
                    break;
                default:
                    break;
            }
        }

        private Shape ProcessShape(List<string> shapeData)
        {
            var shapeObject = DetermineShape(TrimFromColonPosition(shapeData[0]));
            var indentLevels = GetIndentLevels(shapeData);
            var indentLevel1s = indentLevels.Select((e, i) => new {Element = e, Index = i}).Where(x => x.Element == 1);
            var indentLevel1Indices = indentLevel1s.Select(x => x.Index).Append(shapeData.Count).ToList();
            var index = 0;
            foreach (var level1Index in indentLevel1Indices)
            {
                if (shapeData.Count - level1Index > 0 || index < indentLevel1Indices.Count - 1)
                {
                    var level2Lines = shapeData.Skip(level1Index + 1).SkipLast(shapeData.Count - indentLevel1Indices[index + 1]).ToList();
                    var temp = shapeData[level1Index].Trim();
                    if (temp.StartsWith("material"))
                    {
                        var fullList = new List<string>();
                        if (_definitions.Keys.Contains(TrimFromColonPosition(temp)))
                        {
                            fullList = _definitions[TrimFromColonPosition(temp)];
                        }
                        // if (level2Lines.Count == 0)
                        if (level2Lines.Count > 0)
                        {
                            // shapeObject.Material = ProcessMaterial(_definitions[TrimFromColonPosition(temp)]);
                            fullList.AddRange(level2Lines);
                        }
                        // else
                        // {
                        //     shapeObject.Material = ProcessMaterial(level2Lines);
                        // }
                        shapeObject.Material = ProcessMaterial(fullList);
                    }
                    else if (temp.StartsWith("transform"))
                    {
                        shapeObject.Transform = ProcessTransform(level2Lines);;
                    }
                    else
                    {
                        throw new System.Exception("Unknown shape attribute in YAML file");
                    }
                }
                index += 1;
            }
            return shapeObject;
        }

        private List<int> GetIndentLevels(List<string> shapeData)
        {
            var indentLevels = shapeData.Select(y => System.Array.FindIndex(y.ToCharArray(), 1, x => !char.IsWhiteSpace(x))).ToList();
            var distinctIndentLevels = indentLevels.Distinct().ToList();
            distinctIndentLevels.Sort();
            distinctIndentLevels.Insert(0, 0);
            var indexLookup = indentLevels.Select(x => distinctIndentLevels.IndexOf(x)).ToList();
            indexLookup[0] = 0;
            return indexLookup;
        }

        private Shape DetermineShape(string shapeName)
        {
            switch (shapeName)
            {
                case "plane":
                    return new Plane();
                case "sphere":
                    return new Sphere();
                case "cube":
                    return new Cube();
                case "cone":
                    return new Cone();
                case "cylinder":
                    return new Cylinder();
                case "triangle":
                    return new Triangle(null, null, null);
                case "smooth-triangle":
                    return new SmoothTriangle(null, null, null, null, null, null);
                default:
                    throw new System.Exception("Unknown Shape type in YAML file");
            }
        }

        private Matrix ProcessTransform(List<string> transformData)
        {
            var rotation = Matrix.Identity(4);
            var scaling = Matrix.Identity(4);
            var translation = Matrix.Identity(4);
            var definedTransform = Matrix.Identity(4);

            foreach (var transformInstruction in transformData)
            {
                var coords = TrimLeadingHyphen(transformInstruction).Replace("[", "").Replace("]", "").Split(',');
                var temp = coords[0].Trim().ToLower();
                if (_definitions.ContainsKey(temp))
                {
                    definedTransform = ProcessTransform(_definitions[temp]);
                }
                else
                {
                    if (temp.Contains("rotate-x") || temp.Contains("rotatex"))
                    {
                        rotation = Transformations.RotationX(ExtractDouble(coords[1], "Rotation-X")) * rotation;
                    }
                    else if (temp.Contains("rotate-y") || temp.Contains("rotatey"))
                    {
                        rotation = Transformations.RotationY(ExtractDouble(coords[1], "Rotation-Y")) * rotation;
                    }
                    else if (temp.Contains("rotate-z") || temp.Contains("rotatez"))
                    {
                        rotation = Transformations.RotationZ(ExtractDouble(coords[1], "Rotation-Z")) * rotation;
                    }
                    else if (temp.Contains("scale"))
                    {
                        var scaleX = ExtractDouble(coords[1], "Scale (x)");
                        var scaleY = ExtractDouble(coords[2], "Scale (y)");
                        var scaleZ = ExtractDouble(coords[3], "Scale (z)");
                        scaling = Transformations.Scaling(scaleX, scaleY, scaleZ) * scaling;
                    }
                    else if (temp.Contains("translate"))
                    {
                        var translateX = ExtractDouble(coords[1], "Translation (x)");
                        var translateY = ExtractDouble(coords[2], "Translation (y)");
                        var translateZ = ExtractDouble(coords[3], "Translation (z)");
                        translation = Transformations.Translation(translateX, translateY, translateZ) * translation;
                    }
                    else
                    {
                        throw new System.Exception("Unknown transform type in transform data");
                    }
                }
            }

            return translation * scaling * rotation * definedTransform;
        }

        private Material ProcessMaterial(List<string> materialData)
        {
            var material = new Material();

            for (int i = 0; i < materialData.Count; i++)
            {
                var temp = materialData[i].Trim();
                if (temp.StartsWith("color") || temp.StartsWith("colour"))
                {
                    var materialColour = ExtractTuple(temp, "material colour");
                    material.Colour = new Colour(materialColour[0], materialColour[1], materialColour[2]);
                }
                else if (temp.StartsWith("ambient"))
                {
                    material.Ambient = ExtractDouble(temp, "material ambient");
                }
                else if (temp.StartsWith("diffuse"))
                {
                    material.Diffuse = ExtractDouble(temp, "material diffuse");
                }
                else if (temp.StartsWith("specular"))
                {
                    material.Specular = ExtractDouble(temp, "material specular");
                }
                else if (temp.StartsWith("shininess"))
                {
                    material.Shininess = ExtractDouble(temp, "material shininess");
                }
                else if (temp.StartsWith("reflective"))
                {
                    material.Reflective = ExtractDouble(temp, "material reflective");
                }
                else if (temp.StartsWith("transparency"))
                {
                    material.Transparency = ExtractDouble(temp, "material transparency");
                }
                else if (temp.StartsWith("refractive-index"))
                {
                    material.RefractiveIndex = ExtractDouble(temp, "material refractive-index");
                }
                else if (temp.StartsWith("casts-shadow"))
                {
                    material.CastsShadow = ExtractBoolean(temp, "material casts-shadow");
                }
                else if (temp.StartsWith("pattern"))
                {
                    var indentLevels = GetIndentLevels(materialData);
                    // var indentLevel2s = indentLevels.Select((e, i) => new {Element = e, Index = i}).Where(x => x.Element == 2);
                    // var indentLevel1s = indentLevels.Select((e, i) => new {Element = e, Index = i}).Where(x => x.Element == 1 && x.Index > indentLevel2s.Last().Index);
                    // var lastIndex = indentLevel1s.Count() > 0 ? indentLevel1s.First().Index : materialData.Count;
                    // var level2Lines = materialData.Skip(indentLevel2s.First().Index).SkipLast(materialData.Count - lastIndex).ToList();
                    // material.Pattern = ProcessPattern(level2Lines);
                    // i += level2Lines.Count;

                    var currentIndentLevels = indentLevels.Select((e, i) => new {Element = e, Index = i}).Where(x => x.Element == indentLevels[i]);
                    var nextIndentLevels = indentLevels.Select((e, i) => new {Element = e, Index = i}).Where(x => x.Element == indentLevels[i + 1]);
                    var nextIndexAtCurrentLevel = i < currentIndentLevels.Last().Index ? currentIndentLevels.Where(x => x.Index > i).First().Index : materialData.Count;
                    var currLevelLines = materialData.Skip(nextIndentLevels.Where(x => x.Index > i).First().Index).SkipLast(materialData.Count - nextIndexAtCurrentLevel).ToList();
                    material.Pattern = ProcessPattern(currLevelLines);
                    i += currLevelLines.Count;
                }
                else
                {
                    throw new System.Exception("Unknown material attribute in YAML file");
                }
            }

            return material;
        }

        private Pattern ProcessPattern(List<string> patternData)
        {
            Pattern pattern = new StripePattern(Colour.BLACK, Colour.WHITE);
            var indentLevels = GetIndentLevels(patternData);

            for (int i = 0; i < patternData.Count; i++)
            {
                var currentIndentLevels = indentLevels.Select((e, i) => new {Element = e, Index = i}).Where(x => x.Element == indentLevels[i]);
                var nextIndentLevels = indentLevels.Select((e, i) => new {Element = e, Index = i}).Where(x => x.Element == indentLevels[i + 1]);
                var nextIndexAtCurrentLevel = i < currentIndentLevels.Last().Index ? currentIndentLevels.Where(x => x.Index > i).First().Index : patternData.Count;
                var currLevelLines = patternData.Skip(nextIndentLevels.Where(x => x.Index > i).First().Index).SkipLast(patternData.Count - nextIndexAtCurrentLevel).ToList();

                var patternKey = patternData[i].Trim();
                if (patternKey.StartsWith("type"))
                {
                    var patternType = TrimFromColonPosition(patternKey);
                    if (patternType == "blended" || patternType == "nested")
                    {
                        Pattern parentPattern;
                        Pattern patternA;
                        Pattern patternB;
                        Matrix transform;
                        var requiredIndents = nextIndentLevels.Select(x => x.Index - 1).Append(currLevelLines.Count()).ToList();
                        ProcessSubPatterns(currLevelLines, requiredIndents, out parentPattern, out patternA, out patternB, out transform);
                        pattern = patternType == "nested" ? new NestedPattern(parentPattern, patternA, patternB) : new BlendedPattern(patternA, patternB);
                        pattern.Transform = transform;
                        i += currLevelLines.Count;
                    }
                    else if (patternType == "perturbed")
                    {
                        // perturbed
                    }
                    else
                    {
                        pattern = CreatePattern(patternType);
                    }
                }
                else if (patternKey.StartsWith("colors") || patternKey.StartsWith("colours"))
                {
                    var colourATuple = TrimLeadingHyphen(currLevelLines[0]);
                    var colourBTuple = TrimLeadingHyphen(currLevelLines[1]);
                    var patternColourA = ExtractTuple(colourATuple, "pattern colour");
                    var patternColourB = ExtractTuple(colourBTuple, "pattern colour");
                    pattern.ColourA = new Colour(patternColourA[0], patternColourA[1], patternColourA[2]);
                    pattern.ColourB = new Colour(patternColourB[0], patternColourB[1], patternColourB[2]);
                    i += currLevelLines.Count;
                }
                else if (patternKey.StartsWith("transform"))
                {
                    pattern.Transform = ProcessTransform(currLevelLines);
                    i += currLevelLines.Count;
                }
                else
                {
                    throw new System.Exception("Unknown pattern attribute in YAML file");
                }
            }

            return pattern;
        }

        private Pattern CreatePattern(string patternType)
        {
            switch (patternType)
            {
                case "checker":
                    return new CheckerPattern(Colour.BLACK, Colour.WHITE);
                case "double-gradient":
                    return new DoubleGradientPattern(Colour.BLACK, Colour.WHITE);
                case "double-gradient-ring":
                    return new DoubleGradientRingPattern(Colour.BLACK, Colour.WHITE);
                case "gradient":
                    return new GradientPattern(Colour.BLACK, Colour.WHITE);
                case "gradient-ring":
                    return new GradientRingPattern(Colour.BLACK, Colour.WHITE);
                case "ring":
                    return new RingPattern(Colour.BLACK, Colour.WHITE);
                case "stripe":
                    return new StripePattern(Colour.BLACK, Colour.WHITE);
                default:
                    throw new System.Exception($"Unknown Pattern type {patternType}");
            }
        }

        private void ProcessSubPatterns(List<string> subPatternData, List<int> indentLevels, out Pattern parentPattern, out Pattern patternA, out Pattern patternB, out Matrix transform)
        {
            parentPattern = null;
            patternA = null;
            patternB = null;
            transform = null;
            // var parentPatternFind = subPatternData.Select((e, i) => new {Element = e, Index = i}).Where(x => x.Element.Contains("parent-pattern"));
            // var parentPatternLine = parentPatternFind.Count() > 0 ? parentPatternFind.First().Index : -1;
            for (int i = 0; i < indentLevels.Count - 1; i++)
            {
                var objectName = TrimToColonPosition(subPatternData[indentLevels[i]]);
                var objectData = subPatternData.Skip(indentLevels[i] + 1).Take(indentLevels[i + 1] - indentLevels[i] - 1).ToList();
                switch (objectName)
                {
                    case "parent-pattern":
                        parentPattern = ProcessPattern(objectData);
                        break;
                    case "patterns":
                        System.Console.WriteLine(objectName);
                        var childIndentLevels = GetIndentLevels(objectData);
                        var childPatternHeadings = childIndentLevels.Select((e, i) => new {Element = e, Index = i}).Where(x => x.Element < childIndentLevels[1]).ToList();
                        childPatternHeadings = childPatternHeadings.Append(new {Element = childPatternHeadings.Count(), Index = childIndentLevels.Count}).ToList();
                        int lineIndex;
                        for (int j = 0; j < childPatternHeadings.Count() - 1; j++)
                        {
                            lineIndex = childPatternHeadings[j].Index;
                            if (objectData[lineIndex].Contains("pattern-a"))
                            {
                                patternA = ProcessPattern(objectData.Skip(lineIndex + 1).Take(childPatternHeadings[j + 1].Index - childPatternHeadings[j].Index - 1).ToList());
                            }
                            else if (objectData[lineIndex].Contains("pattern-b"))
                            {
                                patternB = ProcessPattern(objectData.Skip(lineIndex + 1).Take(childPatternHeadings[j + 1].Index - childPatternHeadings[j].Index - 1).ToList());
                            }
                            else
                            {
                                throw new System.Exception($"Unknown sub-pattern heading: {objectData[lineIndex]}");
                            }
                        }
                        break;
                    case "transform":
                        transform = ProcessTransform(objectData);
                        break;
                    default:
                        throw new System.Exception($"Unknown type in sub-pattern data: {objectName}");
                }
            }
            // var patternsLine = subPatternData.Select((e, i) => new {Element = e, Index = i}).Where(x => x.Element.Contains("patterns")).First().Index;
            // // var patternsLineIndentLevel = indentLevels[patternsLine + 1];
            // var patternALine = subPatternData.Select((e, i) => new {Element = e, Index = i}).Where(x => x.Element.Contains("pattern-a")).First().Index;
            // // var patternALineIndentLevel = indentLevels[patternALine + 1];
            // var patternBLine = subPatternData.Select((e, i) => new {Element = e, Index = i}).Where(x => x.Element.Contains("pattern-b")).First().Index;
            // // var patternBLineIndentLevel = indentLevels[patternBLine + 1];
            // var transformLine = 0;//subPatternData.Select((e, i) => new {Element = e, Index = i}).Where(x => x.Element.Contains("transform") && indentLevels[x.Index + 1] == patternsLineIndentLevel).First().Index;
            // if (patternsLine < transformLine)
            // {
            //     if (patternALine < patternBLine)
            //     {
            //         patternA = ProcessPattern(subPatternData.Skip(patternALine + 1).Take(patternBLine - patternALine - 1).ToList());
            //         patternB = ProcessPattern(subPatternData.Skip(patternBLine + 1).Take(transformLine - patternBLine - 1).ToList());
            //     }
            //     else
            //     {
            //         patternB = ProcessPattern(subPatternData.Skip(patternBLine + 1).Take(patternALine - patternBLine - 1).ToList());
            //         patternA = ProcessPattern(subPatternData.Skip(patternALine + 1).Take(transformLine - patternALine - 1).ToList());
            //     }
            //     transform = ProcessTransform(subPatternData.Skip(transformLine + 1).ToList());
            // }
            // else
            // {
            //     if (patternALine < patternBLine)
            //     {
            //         transform = ProcessTransform(subPatternData.Skip(transformLine + 1).Take(patternALine - transformLine - 1).ToList());
            //         patternA = ProcessPattern(subPatternData.Skip(patternALine + 1).Take(patternBLine - patternALine - 1).ToList());
            //         patternB = ProcessPattern(subPatternData.Skip(patternBLine + 1).ToList());
            //     }
            //     else
            //     {
            //         transform = ProcessTransform(subPatternData.Skip(transformLine + 1).Take(patternBLine - transformLine - 1).ToList());
            //         patternB = ProcessPattern(subPatternData.Skip(patternBLine + 1).Take(patternALine - patternBLine - 1).ToList());
            //         patternA = ProcessPattern(subPatternData.Skip(patternALine + 1).ToList());
            //     }
            // }
        }

        private Light ProcessLight(List<string> lightData)
        {
            Point lightPosition = null;
            Colour lightIntensity = null;
            for (int i = 1; i < lightData.Count; i++)
            {
                var temp = lightData[i].Trim();
                if (temp.StartsWith("at"))
                {
                    var positionCoords = ExtractTuple(lightData[i], "light position");
                    lightPosition = new Point(positionCoords[0], positionCoords[1], positionCoords[2]);
                }
                else if (temp.StartsWith("intensity"))
                {
                    var colourLevels = ExtractTuple(lightData[i], "light intensity");
                    lightIntensity = new Colour(colourLevels[0], colourLevels[1], colourLevels[2]);
                }
                else
                {
                    throw new System.Exception("Unknown light attribute in YAML file");
                }
            }
            return new Light(lightPosition, lightIntensity);
        }

        private List<double> ExtractTuple(string tupleData, string description)
        {
            var coords = TrimFromColonPosition(tupleData).Replace("[", "").Replace("]", "").Split(',');
            var xyz = new List<double>();
            foreach (var coord in coords)
            {
                xyz.Add(ExtractDouble(coord, description));
            }
            return xyz;
        }

        private double ExtractDouble(string data, string description)
        {
            double doubleValue;
            if (double.TryParse(TrimFromColonPosition(data), out doubleValue))
            {
                return doubleValue;
            }
            else
            {
                throw new System.Exception($"Unable to parse {description} data.");
            }
        }

        private int ExtractInteger(string data, string description)
        {
            int intValue;
            if (int.TryParse(TrimFromColonPosition(data), out intValue))
            {
                return intValue;
            }
            else
            {
                throw new System.Exception($"Unable to parse {description} data.");
            }
        }

        private bool ExtractBoolean(string data, string description)
        {
            bool boolValue;
            if (bool.TryParse(TrimFromColonPosition(data), out boolValue))
            {
                return boolValue;
            }
            else
            {
                throw new System.Exception($"Unable to parse {description} data.");
            }
        }
    }
}
