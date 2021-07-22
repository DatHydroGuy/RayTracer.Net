using System.Linq;
using System.Collections.Generic;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using RayTracer;

namespace RayTracerApp
{
    public class YamlReader
    {
        private readonly Dictionary<string, Dictionary<object, object>> _materialDefinitions = new();
        private readonly Dictionary<string, List<object>> _transformDefinitions = new();
        private readonly Dictionary<string, Group> _groupDefinitions = new();
        private readonly Dictionary<string, int> _renderingDetails = new() {{"reflectiondepth", 5}, {"antialiaslevel", 1}, {"showprogress", 0}};
        private static string _imagesFolder = "";
        private static string _objFolder = "";

        public YamlReader(string yamlFile)
        {
            if (!System.IO.File.Exists(yamlFile))
                System.Console.WriteLine($"YAML file {yamlFile} not found.");
            
            CreateHelperFolders();

            var rawLines = System.IO.File.ReadAllText(yamlFile);
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(HyphenatedNamingConvention.Instance)
                .Build();
            var rawObjects = deserializer.Deserialize<List<Dictionary<string, object>>>(rawLines.ToLower());
            ConvertToObjects(rawObjects);
        }

        private static void CreateHelperFolders()
        {
            var currDir = System.IO.Directory.GetCurrentDirectory();
            var parentDir = System.IO.Path.GetFullPath(System.IO.Path.Combine(currDir, ".."));
            var imageDir = System.IO.Path.GetFullPath(System.IO.Path.Combine(parentDir, "Images"));
            var objDir = System.IO.Path.GetFullPath(System.IO.Path.Combine(parentDir, "Obj"));
            foreach (var folder in new[]{imageDir, objDir})
            {
                if (!System.IO.Directory.Exists(folder))
                {
                    System.IO.Directory.CreateDirectory(folder);
                }
            }
            _imagesFolder = imageDir;
            _objFolder = objDir;
        }

        private void ConvertToObjects(IReadOnlyCollection<Dictionary<string, object>> objects)
        {
            var world = new World();
            var cameraInfo = objects.First(x => x.Keys.First() == "add" && x.Values.First().ToString() == "camera").Skip(1).ToList();
            var camera = ProcessCameraObject(cameraInfo);
            string outFile = null;

            foreach (var item in objects)
            {
                switch (item.Keys.First())
                {
                    case "add":
                    {
                        if (item.Values.First().ToString() != "camera")
                            ProcessWorldObject(world, item);
                        break;
                    }
                    case "define":
                        ProcessDefinitions(item);
                        break;
                    case "file":
                    {
                        outFile = item.Values.First().ToString();
                        outFile = outFile?.Split(System.IO.Path.DirectorySeparatorChar).Last();
                        outFile = System.IO.Path.GetFullPath(System.IO.Path.Combine(_imagesFolder, outFile ?? "default.ppm"));
                        System.Console.WriteLine($"Writing to file: {outFile}");
                        break;
                    }
                }
            }

            // Render the scene
            var canvas = camera.Render(world, reflectionDepth: _renderingDetails["reflectiondepth"],
                                       antialiasLevel: _renderingDetails["antialiaslevel"], showProgress: _renderingDetails["showprogress"] != 0);
            canvas.WritePpmFile(outFile);
            System.Console.WriteLine("Image rendering complete");
        }

        private void ProcessDefinitions(Dictionary<string, object> definitionData)
        {
            System.Console.WriteLine($"Defining: {definitionData.Values.First()}");
            var materialValuesList = new Dictionary<object, object>();
            var transformValuesList = new List<object>();
            var name = "";

            // Key is either define, extend, or value
            foreach (var (definitionKey, definitionValue) in definitionData)
            {
                switch (definitionKey)
                {
                    case "define":
                        name = definitionValue.ToString() ?? "";
                        break;
                    case "extend":
                        // Pre-existing value, so need to perform a lookup
                        LookupExtendItem(definitionKey, definitionValue, name, ref materialValuesList, ref transformValuesList);
                        break;
                    case "value":
                        switch (definitionValue)
                        {
                            case Dictionary<object, object> tempObject:
                                var (firstKey, firstValue) = tempObject.First();
                                switch (firstKey.ToString())
                                {
                                    case "add" when firstValue.ToString() == "group":
                                    {
                                        // Need to unpack tempObject and then pass it into ProcessShape()
                                        var g = new Group();
                                        var childData = (List<object>) tempObject.Values.Last();
                                        foreach (var childDictionary in childData)
                                        {
                                            var arguments = new List<KeyValuePair<string, object>>();

                                            foreach (var (tempKey, tempValue) in (Dictionary<object, object>) childDictionary)
                                            {
                                                arguments.Add(new KeyValuePair<string, object>(tempKey.ToString(), tempValue));
                                            }

                                            var s = ProcessShape(arguments);
                                            g.AddChild(s);
                                        }
                                        _groupDefinitions.Add(name, g);
                                        break;
                                    }
                                    case "add" when firstValue.ToString() == "csg":
                                    {
                                        // Need to unpack tempObject and then pass it into ProcessShape()
                                        var childData = tempObject.Skip(1).ToList();
                                        var opText = childData.Find(x => x.Key.ToString() == "operation").Value.ToString();
                                        var operation = opText switch
                                        {
                                            "difference" => CsgOperation.Difference,
                                            "intersect" => CsgOperation.Intersect,
                                            "union" => CsgOperation.Union,
                                            _ => CsgOperation.None
                                        };
                                        var children = childData.FindAll(x => x.Key.ToString() != "operation").ToList();
                                        // var left = (List<object>)childData.Find(x => x.Key.ToString() == "left").Value;
                                        // var right = (List<object>)childData.Find(x => x.Key.ToString() == "right").Value;
                                        var csgChildren = new List<Shape>(2) {null, null};
                                        foreach (var (childKey, childValue) in children)
                                        {
                                            var arguments = new List<KeyValuePair<string, object>>();
                                            var childList = ((List<object>) childValue).First();
                                            foreach (var (tempKey, tempValue) in (Dictionary<object, object>) childList)
                                            {
                                                arguments.Add(new KeyValuePair<string, object>(tempKey.ToString(), tempValue));
                                            }

                                            var s = ProcessShape(arguments);
                                            if (childKey.ToString() == "left")
                                            {
                                                csgChildren[0] = s;
                                            }
                                            else
                                            {
                                                csgChildren[1] = s;
                                            }
                                        }

                                        var csg = new CSG(operation, csgChildren[0], csgChildren[1]);
                                        _groupDefinitions.Add(name, csg);
                                        break;
                                    }
                                    case "add" when firstValue.ToString() == "obj":
                                    {
                                        // Need to locate obj file
                                        var (_, fileValue) = tempObject.Skip(1).First();
                                        var objFile = fileValue.ToString();
                                        fileValue = objFile?.Split(System.IO.Path.DirectorySeparatorChar).Last();
                                        objFile = System.IO.Path.GetFullPath(System.IO.Path.Combine(_objFolder, fileValue.ToString() ?? "default.ppm"));
                                        System.Console.WriteLine($"Loading .obj file: {objFile}");
                                        var parser = new ObjParser();
                                        parser.ReadFile(objFile.ToString());
                                        var obj = parser.ObjToGroup();
                                        // Create a dummy shape(sphere) to hold the transform & material for the object
                                        var arguments = new List<KeyValuePair<string, object>> {new("add", "sphere")};
                                        foreach (var (tempKey, tempValue) in tempObject.Skip(2))
                                        {
                                            arguments.Add(new KeyValuePair<string, object>(tempKey.ToString(), tempValue));
                                        }
                                        // Assign object transform & material to dummy shape
                                        var s = ProcessShape(arguments);
                                        // Copy (non-default) transform & material from dummy shape to object
                                        obj.Transform = s.Transform;
                                        if (s.Material != new Material())
                                        {
                                            obj.SetMaterial(s.Material);
                                        }
                                        _groupDefinitions.Add(name, obj);
                                        break;
                                    }
                                    default:
                                    {
                                        foreach (var (materialDefKey, materialDefValue) in tempObject.Where(kvp => !materialValuesList.TryAdd(kvp.Key, kvp.Value)))
                                        {
                                            materialValuesList[materialDefKey] = materialDefValue;    // overwrite any pre-existing values
                                        }
                                        _materialDefinitions.Add(name, materialValuesList);
                                        break;
                                    }
                                }
                                break;
                            case List<object> tempTransform:
                                DefineTransform(name, tempTransform, ref transformValuesList);
                                break;
                            default:
                                throw new System.Exception($"Unknown items in definition data: ({definitionData.First().Value} : {definitionKey})");
                        }
                        break;
                    default:
                        throw new System.Exception($"Unknown items in definition data: ({definitionKey} : {definitionValue})");
                }
            }
        }

        private void DefineTransform(string name, IEnumerable<object> tempTransform, ref List<object> transformValuesList)
        {
            foreach (var item in tempTransform)
            {
                transformValuesList.Add(item);
                // var itemStr = item.ToString() ?? "";
                // if (_transformDefinitions.ContainsKey(itemStr))
                // {
                //     transformValuesList.AddRange(_transformDefinitions[itemStr]);
                // }
                // else
                // {
                //     transformValuesList.Add(item);
                // }
            }                        
            _transformDefinitions.Add(name, transformValuesList);
        }

        private void LookupExtendItem(string definitionKey, object definitionValue, string name,
                                      ref Dictionary<object, object> materialValuesList, ref List<object> transformValuesList)
        {
            var itemString = definitionValue.ToString() ?? "";
            if (_materialDefinitions.ContainsKey(itemString))
                materialValuesList = materialValuesList.Concat(_materialDefinitions[itemString]).ToDictionary(x => x.Key, x => x.Value);
            else if (_transformDefinitions.ContainsKey(itemString))
                transformValuesList.AddRange(_transformDefinitions[name]);
            else
                throw new System.Exception($"Unknown items in definition data: ({definitionKey} : {definitionValue})");
        }

        private void ProcessWorldObject(World world, Dictionary<string, object> item)
        {
            System.Console.WriteLine($"Adding: {item.Values.First()}");

            switch (item.Values.First())
            {
                case "light":
                    world.AddLightToWorld(ProcessLight(item.Skip(1).ToList()));
                    break;
                case "plane":
                case "sphere":
                case "cube":
                case "cone":
                case "cylinder":
                case "triangle":
                case "smooth-triangle":
                    world.AddShapeToWorld(ProcessShape(item.ToList()));
                    break;
                case "group":
                    world.AddShapeToWorld(ProcessShape(item.Skip(1).ToList()));
                    break;
                default:
                    world.AddShapeToWorld(ProcessShape(item.ToList()));
                    break;
            }
        }

        private Shape DetermineShape(string shapeName)
        {
            return shapeName switch
            {
                "plane" => new Plane(),
                "sphere" => new Sphere(),
                "cube" => new Cube(),
                "cone" => new Cone(),
                "cylinder" => new Cylinder(),
                "triangle" => new Triangle(new Point(), new Point(), new Point()),
                "smooth-triangle" => new SmoothTriangle(new Point(), new Point(), new Point(), new Vector(), new Vector(), new Vector()),
                _ => CheckGroupDefinitions(shapeName)
            };
        }

        private Shape CheckGroupDefinitions(string shapeName)
        {
            if (_groupDefinitions.ContainsKey(shapeName))
            {
                return _groupDefinitions[shapeName].Clone();
            }

            throw new System.Exception($"Unknown Shape type in YAML file: {shapeName}");
        }

        private Shape ProcessShape(IReadOnlyList<KeyValuePair<string, object>> shapeData)
        {
            var shapeObject = DetermineShape(shapeData[0].Value.ToString());
            for (var i = 1; i < shapeData.Count; i++)
            {
                var (shapeKey, shapeValue) = shapeData[i];
                switch (shapeKey)
                {
                    case "material":
                    {
                        // Need to de-cipher material attributes
                        var attribValStr = shapeValue.ToString() ?? "";
                        var attributeDictionary = shapeValue as Dictionary<object, object>;
                        var tempMaterial = ProcessMaterial(attributeDictionary ?? _materialDefinitions[attribValStr]);
                        if (shapeObject is Group grp)
                        {
                            grp.SetMaterial(tempMaterial);
                        }
                        else
                        {
                            shapeObject.Material = tempMaterial;
                        }

                        break;
                    }
                    case "transform":
                    {
                        // Need to de-cipher transform attributes
                        var attribValStr = shapeValue.ToString() ?? "";
                        var attributeList = shapeValue as List<object>;
                        shapeObject.Transform = ProcessTransform(attributeList ?? _transformDefinitions[attribValStr]);
                        break;
                    }
                    case "minimum":
                        switch (shapeObject)
                        {
                            case Cylinder cylinder:
                                cylinder.Minimum = ExtractDouble(shapeValue.ToString(), "Cylinder minimum");
                                break;
                            case Cone cone:
                                cone.Minimum = ExtractDouble(shapeValue.ToString(), "Cone minimum");
                                break;
                            default:
                                throw new System.Exception($"Unknown shape type with minimum attribute: {shapeObject.GetType().Name}");
                        }
                        break;
                    case "maximum":
                        switch (shapeObject)
                        {
                            case Cylinder cylinder:
                                cylinder.Maximum = ExtractDouble(shapeValue.ToString(), "Cylinder maximum");
                                break;
                            case Cone cone:
                                cone.Maximum = ExtractDouble(shapeValue.ToString(), "Cone maximum");
                                break;
                            default:
                                throw new System.Exception($"Unknown shape type with maximum attribute: {shapeObject.GetType().Name}");
                        }
                        break;
                    case "closed":
                        switch (shapeObject)
                        {
                            case Cylinder cylinder:
                                cylinder.Closed = ExtractBoolean(shapeValue.ToString(), "Cylinder closed");
                                break;
                            case Cone cone:
                                cone.Closed = ExtractBoolean(shapeValue.ToString(), "Cone closed");
                                break;
                            default:
                                throw new System.Exception($"Unknown shape type with closed attribute: {shapeObject.GetType().Name}");
                        }
                        break;
                    case "vertices":
                    {
                        switch (shapeObject)
                        {
                            case Triangle triangle:
                            {
                                var vertices = (List<object>) shapeValue;
                                var p1 = ExtractTuple(vertices[0], "Triangle vertices");
                                var p2 = ExtractTuple(vertices[1], "Triangle vertices");
                                var p3 = ExtractTuple(vertices[2], "Triangle vertices");
                                triangle.Update(new Point(p1[0], p1[1], p1[2]),
                                    new Point(p2[0], p2[1], p2[2]),
                                    new Point(p3[0], p3[1], p3[2]));
                                break;
                            }
                            case SmoothTriangle smoothTriangle:
                            {
                                var vertices = (List<object>) shapeValue;
                                var p1 = ExtractTuple(vertices[0], "Triangle vertices");
                                var p2 = ExtractTuple(vertices[1], "Triangle vertices");
                                var p3 = ExtractTuple(vertices[2], "Triangle vertices");
                                smoothTriangle.UpdatePoints(new Point(p1[0], p1[1], p1[2]),
                                    new Point(p2[0], p2[1], p2[2]),
                                    new Point(p3[0], p3[1], p3[2]));
                                break;
                            }
                        }

                        break;
                    }
                    case "normals":
                    {
                        if (shapeObject is SmoothTriangle smoothTriangle)
                        {
                            var normals = (List<object>) shapeValue;
                            var n1 = ExtractTuple(normals[0], "Triangle vertices");
                            var n2 = ExtractTuple(normals[1], "Triangle vertices");
                            var n3 = ExtractTuple(normals[2], "Triangle vertices");
                            smoothTriangle.UpdateNormals(new Vector(n1[0], n1[1], n1[2]),
                                new Vector(n2[0], n2[1], n2[2]),
                                new Vector(n3[0], n3[1], n3[2]));
                        }

                        break;
                    }
                    default:
                        throw new System.Exception($"Unknown attribute in shape data: ({shapeKey}: {shapeValue})");
                }
            }
            return shapeObject;
        }

        private Material ProcessMaterial(Dictionary<object, object> materialData)
        {
            var material = new Material();

            foreach (var materialInstruction in materialData)
            {
                var instruction = materialInstruction.Key.ToString() ?? "";
                if (_transformDefinitions.ContainsKey(instruction))
                {
                    material = ProcessMaterial(_materialDefinitions[instruction]);
                }
                else
                {
                    if (instruction.Contains("colour") || instruction.Contains("color"))
                    {
                        var colour = ExtractTuple(materialInstruction.Value, "Material Colour");
                        material.Colour = new Colour(colour[0], colour[1], colour[2]);
                    }
                    else if (instruction.Contains("ambient"))
                    {
                        material.Ambient = ExtractDouble(materialInstruction.Value.ToString(), "Material Ambient");
                    }
                    else if (instruction.Contains("diffuse"))
                    {
                        material.Diffuse = ExtractDouble(materialInstruction.Value.ToString(), "Material Diffuse");
                    }
                    else if (instruction.Contains("specular"))
                    {
                        material.Specular = ExtractDouble(materialInstruction.Value.ToString(), "Material Specular");
                    }
                    else if (instruction.Contains("shininess"))
                    {
                        material.Shininess = ExtractDouble(materialInstruction.Value.ToString(), "Material Shininess");
                    }
                    else if (instruction.Contains("reflective"))
                    {
                        material.Reflective = ExtractDouble(materialInstruction.Value.ToString(), "Material Reflective");
                    }
                    else if (instruction.Contains("transparency"))
                    {
                        material.Transparency = ExtractDouble(materialInstruction.Value.ToString(), "Material Transparency");
                    }
                    else if (instruction.Contains("refractive-index") || instruction.Contains("refractiveindex"))
                    {
                        material.RefractiveIndex = ExtractDouble(materialInstruction.Value.ToString(), "Material Refractive Index");
                    }
                    else if (instruction.Contains("casts-shadow") || instruction.Contains("castsshadow"))
                    {
                        material.CastsShadow = ExtractBoolean(materialInstruction.Value.ToString(), "Material Casts Shadow");
                    }
                    else if (instruction.Contains("pattern"))
                    {
                        material.Pattern = ProcessPattern(materialInstruction.Value);
                    }
                }
            }

            return material;
        }

        private Pattern ProcessPattern(object patternData)
        {
            if (patternData is not Dictionary<object, object> patternInstructions)
            {
                throw new System.Exception($"Unknown type(s) in pattern data {patternData}");
            }

            Pattern pattern = null;
            foreach (var key in patternInstructions.Keys)
            {
                var instruction = key.ToString() ?? "";
                if (instruction.Contains("type"))
                {
                    pattern = GetEmptyPattern(patternInstructions[instruction].ToString());
                }
                else if (instruction.Contains("parent-pattern"))
                {
                    if (pattern is NestedPattern nested)
                        nested.ParentPattern = ProcessPattern(patternInstructions[instruction]);
                }
                else if (instruction.Contains("patterns"))
                {
                    switch (pattern)
                    {
                        case NestedPattern nestedPattern:
                        {
                            if (patternInstructions[instruction] is not Dictionary<object, object> subPatterns)
                            {
                                throw new System.Exception("Invalid sub-pattern data format");
                            }
                            nestedPattern.PatternA = ProcessPattern(subPatterns["pattern-a"]);
                            nestedPattern.PatternB = ProcessPattern(subPatterns["pattern-b"]);
                            break;
                        }
                        case BlendedPattern blendedPattern:
                        {
                            if (patternInstructions[instruction] is not Dictionary<object, object> subPatterns)
                            {
                                throw new System.Exception("Invalid sub-pattern data format");
                            }
                            blendedPattern.PatternA = ProcessPattern(subPatterns["pattern-a"]);
                            blendedPattern.PatternB = ProcessPattern(subPatterns["pattern-b"]);
                            break;
                        }
                        default:
                            throw new System.Exception("Invalid compound pattern type");
                    }
                }
                else if (instruction.Contains("transform"))
                {
                    if (patternInstructions[instruction] is not List<object> transform)
                    {
                        throw new System.Exception("Invalid transform format in pattern data");
                    }

                    if (pattern != null)
                        pattern.Transform = ProcessTransform(transform);
                }
                else if (instruction.Contains("colours") || instruction.Contains("colors"))
                {
                    var colList = ProcessPatternColours(patternInstructions, instruction);
                    pattern.ColourA = colList[0];
                    pattern.ColourB = colList[1];
                }
                else if (instruction.Contains("mapping"))
                {
                    if (pattern is UvPattern uv)
                    {
                        uv.UvPatternMapType = patternInstructions[instruction] switch
                        {
                            "spherical" => UvPatternMapType.Spherical,
                            "cylindrical" => UvPatternMapType.Cylindrical,
                            "planar" => UvPatternMapType.Planar,
                            "cubic" => UvPatternMapType.Cubic,
                            _ => throw new System.Exception($"Unknown UV Mapping Type: {patternInstructions[instruction]}")
                        };
                    }
                }
                else if (instruction.Contains("uv-pattern"))
                {
                    if (patternInstructions[instruction] is Dictionary<object, object> uvPatternData)
                        pattern = ProcessUvPatternData(((UvPattern)pattern).UvPatternMapType, uvPatternData);
                    else
                        throw new System.Exception($"Unknown type(s) in UV pattern data {patternInstructions[instruction]}");
                }
            }

            return pattern;
        }

        private static List<Colour> ProcessPatternColours(Dictionary<object, object> patternInstructions, string instruction)
        {
            if (patternInstructions[instruction] is not List<object> colours)
            {
                if (patternInstructions[instruction] is Dictionary<object, object> colourDict)
                    colours = colourDict.Values.ToList();
                else
                    throw new System.Exception("Invalid colour data format");
            }

            var colourList = new List<Colour>();

            foreach (var colour in colours)
            {
                var colourLevels = ExtractTuple(colour, "Pattern colour");
                colourList.Add(new Colour(colourLevels[0], colourLevels[1], colourLevels[2]));
            }

            return colourList;
        }

        private Pattern ProcessUvPatternData(UvPatternMapType uvPatternMapType, Dictionary<object,object> uvPatternData)
        {
            UvPattern uvPattern = null;
            foreach (var key in uvPatternData.Keys)
            {
                var instruction = key.ToString() ?? "";
                if (instruction.Contains("type"))
                {
                    uvPattern = GetEmptyUvPattern(uvPatternData[instruction].ToString());
                }
                else if (instruction.Contains("colours") || instruction.Contains("colors"))
                {
                    var colList = ProcessPatternColours(uvPatternData, instruction);
                    if (uvPattern is null)
                        continue;
                    if (colList.Count == 5)
                    {
                        ((AlignCheckPattern)uvPattern).MainColour = colList[0];
                        ((AlignCheckPattern)uvPattern).UpperLeftColour = colList[1];
                        ((AlignCheckPattern)uvPattern).UpperRightColour = colList[2];
                        ((AlignCheckPattern)uvPattern).LowerLeftColour = colList[3];
                        ((AlignCheckPattern)uvPattern).LowerRightColour = colList[4];
                    }
                    else
                    {
                        uvPattern.ColourA = colList[0];
                        uvPattern.ColourB = colList[1];
                    }
                }
                else if (instruction.Contains("width"))
                {
                    if (uvPattern is null)
                        continue;
                    uvPattern.Width = ExtractInteger(uvPatternData[instruction].ToString(), "UV pattern width");
                }
                else if (instruction.Contains("height"))
                {
                    if (uvPattern is null)
                        continue;
                    uvPattern.Height = ExtractInteger(uvPatternData[instruction].ToString(), "UV pattern height");
                }
            }

            if (uvPattern is not null)
                uvPattern.UvPatternMapType = uvPatternMapType;

            return uvPattern;
        }

        private static UvPattern GetEmptyUvPattern(string patternType)
        {
            return patternType switch
            {
                "checker" => new UvCheckerPattern(1, 1, Colour.BLACK, Colour.WHITE),
                "align-check" => new AlignCheckPattern(Colour.BLACK, Colour.WHITE, Colour.WHITE, Colour.WHITE, Colour.WHITE),
                _ => throw new System.Exception($"Unknown Pattern type {patternType}")
            };
        }

        private static Pattern GetEmptyPattern(string patternType)
        {
            return patternType switch
            {
                "blended" => new BlendedPattern(null, null),
                "checker" => new CheckerPattern(Colour.BLACK, Colour.WHITE),
                "double-gradient" => new DoubleGradientPattern(Colour.BLACK, Colour.WHITE),
                "double-gradient-ring" => new DoubleGradientRingPattern(Colour.BLACK, Colour.WHITE),
                "gradient" => new GradientPattern(Colour.BLACK, Colour.WHITE),
                "gradient-ring" => new GradientRingPattern(Colour.BLACK, Colour.WHITE),
                "nested" => new NestedPattern(null, null, null),
                "perturbed" => new PerturbedPattern(null),
                "ring" => new RingPattern(Colour.BLACK, Colour.WHITE),
                "stripe" => new StripePattern(Colour.BLACK, Colour.WHITE),
                "map" => new UvPattern(Colour.BLACK, Colour.WHITE),
                _ => throw new System.Exception($"Unknown Pattern type {patternType}")
            };
        }

        private Matrix ProcessTransform(IEnumerable<object> transformData)
        {
            var rotation = Matrix.Identity(4);
            var scaling = Matrix.Identity(4);
            var translation = Matrix.Identity(4);
            var definedTransform = Matrix.Identity(4);

            foreach (var transformInstruction in transformData)
            {
                var instructionStr = transformInstruction.ToString() ?? "";
                if (_transformDefinitions.ContainsKey(instructionStr))
                {
                    definedTransform = ProcessTransform(_transformDefinitions[instructionStr]);
                }
                else
                {
                    var transformItem = transformInstruction as List<object>;
                    if (transformItem == null)
                        continue;
                    
                    var instruction = transformItem[0].ToString() ?? "";

                    if (instruction.Contains("rotate-x") || instruction.Contains("rotatex"))
                    {
                        rotation = Transformations.RotationX(ExtractDouble(transformItem[1].ToString(), "Rotation-X")) * rotation;
                    }
                    else if (instruction.Contains("rotate-y") || instruction.Contains("rotatey"))
                    {
                        rotation = Transformations.RotationY(ExtractDouble(transformItem[1].ToString(), "Rotation-Y")) * rotation;
                    }
                    else if (instruction.Contains("rotate-z") || instruction.Contains("rotatez"))
                    {
                        rotation = Transformations.RotationZ(ExtractDouble(transformItem[1].ToString(), "Rotation-Z")) * rotation;
                    }
                    else if (instruction.Contains("scale"))
                    {
                        var scaleX = ExtractDouble(transformItem[1].ToString(), "Scale (x)");
                        var scaleY = ExtractDouble(transformItem[2].ToString(), "Scale (y)");
                        var scaleZ = ExtractDouble(transformItem[3].ToString(), "Scale (z)");
                        scaling = Transformations.Scaling(scaleX, scaleY, scaleZ) * scaling;
                    }
                    else if (instruction.Contains("translate"))
                    {
                        var translateX = ExtractDouble(transformItem[1].ToString(), "Translation (x)");
                        var translateY = ExtractDouble(transformItem[2].ToString(), "Translation (y)");
                        var translateZ = ExtractDouble(transformItem[3].ToString(), "Translation (z)");
                        translation = Transformations.Translation(translateX, translateY, translateZ) * translation;
                    }
                    else
                    {
                        throw new System.Exception("Unknown transform type in transform data: {instruction}");
                    }
                }
            }

            return translation * scaling * rotation * definedTransform;
        }

        private static Light ProcessLight(IEnumerable<KeyValuePair<string, object>> lightData)
        {
            Point lightPosition = null;
            Colour lightIntensity = null;
            foreach (var (lightKey, lightValue) in lightData)
            {
                if (lightKey.StartsWith("at"))
                {
                    var positionCoords = ExtractTuple(lightValue, "Light position");
                    lightPosition = new Point(positionCoords[0], positionCoords[1], positionCoords[2]);
                }
                else if (lightKey.StartsWith("intensity"))
                {
                    var colourLevels = ExtractTuple(lightValue, "Light intensity");
                    lightIntensity = new Colour(colourLevels[0], colourLevels[1], colourLevels[2]);
                }
                else
                {
                    throw new System.Exception("Unknown light attribute in YAML file: {lightKey}");
                }
            }
            return new Light(lightPosition, lightIntensity);
        }

        private Camera ProcessCameraObject(IEnumerable<KeyValuePair<string, object>> objData)
        {
            var width = 200;
            var height = 200;
            var fieldOfView = 0.785;
            var from = new Point();
            var to = new Point();
            var up = new Vector();
            System.Console.WriteLine("Adding: Camera");

            foreach (var (cameraKey, cameraValue) in objData)
            {
                var attributeValue = cameraValue.ToString();
                if (cameraKey.StartsWith("width"))
                {
                    width = ExtractInteger(attributeValue, "camera width");
                }
                else if (cameraKey.StartsWith("height"))
                {
                    height = ExtractInteger(attributeValue, "Camera height");
                }
                else if (cameraKey.StartsWith("field-of-view"))
                {
                    fieldOfView = ExtractDouble(attributeValue, "Camera FoV");
                }
                else if (cameraKey.StartsWith("from"))
                {
                    var tuple = ExtractTuple(cameraValue, "Camera from");
                    @from = new Point(tuple[0], tuple[1], tuple[2]);
                }
                else if (cameraKey.StartsWith("to"))
                {
                    var tuple = ExtractTuple(cameraValue, "Camera to");
                    to = new Point(tuple[0], tuple[1], tuple[2]);
                }
                else if (cameraKey.StartsWith("up"))
                {
                    var tuple = ExtractTuple(cameraValue, "Camera up");
                    up = new Vector(tuple[0], tuple[1], tuple[2]);
                }
                else if (cameraKey.StartsWith("reflection-depth"))
                {
                    _renderingDetails["reflectiondepth"] = ExtractInteger(attributeValue, "Camera reflection depth");
                }
                else if (cameraKey.StartsWith("antialias-level"))
                {
                    _renderingDetails["antialiaslevel"] = ExtractInteger(attributeValue, "Camera anti-alias level");
                }
                else if (cameraKey.StartsWith("show-progress"))
                {
                    _renderingDetails["showprogress"] = ExtractBoolean(attributeValue, "Camera show progress") ? 1 : 0;
                }
                else
                {
                    throw new System.Exception($"Unknown camera attribute: {cameraKey}");
                }
            }

            var camera = new Camera(width, height, fieldOfView)
            {
                Transform = Transformations.ViewTransform(@from, to, up)
            };
            return camera;
        }

        private static List<double> ExtractTuple(object tupleToConvert, string description)
        {
            try
            {
                return (from coord in (IEnumerable<object>) tupleToConvert select ExtractDouble(coord.ToString(), description)).ToList();
            }
            catch (System.Exception)
            {
                throw new System.FormatException($"Could not convert {tupleToConvert} into an integer for {description}");
            }
        }

        private static bool ExtractBoolean(string valueToConvert, string description)
        {
            if (bool.TryParse(valueToConvert, out var result))
                return result;
            throw new System.FormatException($"Could not convert {valueToConvert} into a boolean for {description}");
        }

        private static int ExtractInteger(string valueToConvert, string description)
        {
            if (int.TryParse(valueToConvert, out var result))
                return result;
            throw new System.FormatException($"Could not convert {valueToConvert} into an integer for {description}");
        }

        private static double ExtractDouble(string valueToConvert, string description)
        {
            if (double.TryParse(valueToConvert, out var result))
                return result;
            throw new System.FormatException($"Could not convert {valueToConvert} into a double for {description}");
        }
    }
}
