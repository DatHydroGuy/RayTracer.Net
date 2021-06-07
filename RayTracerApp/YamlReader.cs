using System.Linq;
using System.Collections.Generic;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using RayTracer;

namespace RayTracerApp
{
    public class YamlReader
    {
        // private List<string> materialList = new List<string>{"color", "ambient", "diffuse", "specular", "shininess", "reflective", "transparency", "refractive-index", "casts-shadow", "pattern"};
        // private List<string> transformList = new List<string>{"translate", "scale", "rotate-x", "rotate-y", "rotate-z"};
        private Dictionary<string, Dictionary<object, object>> _materialDefinitions = new Dictionary<string, Dictionary<object, object>>();
        private Dictionary<string, List<object>> _transformDefinitions = new Dictionary<string, List<object>>();
        private Dictionary<string, int> _renderingDetails = new Dictionary<string, int> {{"reflectiondepth", 5}, {"antialiaslevel", 1}, {"showprogress", 0}};

        public YamlReader(string yamlFile)
        {
            if (!System.IO.File.Exists(yamlFile))
                System.Console.WriteLine($"YAML file {yamlFile} not found.");

            var rawLines = System.IO.File.ReadAllText(yamlFile);
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(HyphenatedNamingConvention.Instance)
                .Build();
            var rawObjects = deserializer.Deserialize<List<Dictionary<string, object>>>(rawLines.ToLower());
            ConvertToObjects(rawObjects);
        }

        public void ConvertToObjects(List<Dictionary<string, object>> objects)
        {
            var world = new World();
            var cameraInfo = objects.Where(x => x.Keys.First() == "add" && x.Values.First().ToString() == "camera").First().Skip(1).ToList();
            var camera = ProcessCameraObject(cameraInfo);
            string outFile = null;

            foreach (var item in objects)
            {
                if (item.Keys.First() == "add")
                {
                    if (item.Values.First().ToString() != "camera")
                    {
                        ProcessWorldObject(world, item);
                    }
                }
                else if (item.Keys.First() == "define")
                {
                    ProcessDefinitions(item);
                }
                else if (item.Keys.First() == "file")
                {
                    outFile = item.Values.First().ToString();
                    if (System.Diagnostics.Debugger.IsAttached)
                        outFile = "..\\" + outFile;
                    outFile = System.IO.Path.GetFullPath(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), outFile));
                    System.Console.WriteLine($"Writing to file: {outFile}");
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
            string name = null;

            // Key is either define, extend, or value
            foreach (var item in definitionData)
            {
                if (item.Key == "define")
                {
                    name = item.Value.ToString();
                }
                else if (item.Key == "extend")
                {
                    // Pre-existing value, so need to perform a lookup
                    var itemString = item.Value.ToString();
                    if (_materialDefinitions.ContainsKey(itemString))
                        materialValuesList = materialValuesList.Concat(_materialDefinitions[itemString]).ToDictionary(x => x.Key, x => x.Value);
                    else if (_transformDefinitions.ContainsKey(itemString))
                        transformValuesList.AddRange(_transformDefinitions[name]);
                    else
                        throw new System.Exception($"Unknown items in definition data: ({item.Key} : {item.Value})");
                }
                else if (item.Key == "value")
                {
                    var tempMaterial = item.Value as Dictionary<object, object>;
                    var tempTransform = item.Value as List<object>;

                    if (tempMaterial != null)
                    {
                        foreach (var kvp in tempMaterial)
                        {
                            if (!materialValuesList.TryAdd(kvp.Key, kvp.Value))
                            {
                                materialValuesList[kvp.Key] = kvp.Value;    // overwrite any pre-existing values
                            }
                        }
                        
                        _materialDefinitions.Add(name, materialValuesList);
                    }
                    else if (tempTransform != null)
                    {
                        foreach (var item2 in tempTransform)
                        {
                            if (_transformDefinitions.ContainsKey(item2.ToString()))
                            {
                                transformValuesList.AddRange(_transformDefinitions[item2.ToString()]);
                            }
                            else
                            {
                                transformValuesList.Add(item2);
                            }
                        }                        
                        _transformDefinitions.Add(name, transformValuesList);
                    }
                    else
                    {
                        throw new System.Exception($"Unknown items in definition data: ({definitionData.First().Value} : {item.Key})");
                    }
                }
                else
                {
                    throw new System.Exception($"Unknown items in definition data: ({item.Key} : {item.Value})");
                }
            }
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
                default:
                    break;
            }
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

        private Shape ProcessShape(List<KeyValuePair<string, object>> shapeData)
        {
            var shapeObject = DetermineShape(shapeData[0].Value.ToString());
            for (int i = 1; i < shapeData.Count; i++)
            {
                var attribute = shapeData[i];
                if (attribute.Key == "material")
                {
                    // Need to de-cipher material attributes
                    var attributeDictionary = attribute.Value as Dictionary<object, object>;
                    if (attributeDictionary == null)
                        shapeObject.Material = ProcessMaterial(_materialDefinitions[attribute.Value.ToString()]);   // pre-defined material definition
                    else
                        shapeObject.Material = ProcessMaterial(attributeDictionary);        // new material definition
                }
                else if (attribute.Key == "transform")
                {
                    // Need to de-cipher transform attributes
                    var attributeList = attribute.Value as List<object>;
                    shapeObject.Transform = ProcessTransform(attributeList);
                }
                else
                {
                    throw new System.Exception($"Unknown attribute in shape data: ({attribute.Key}: {attribute.Value})");
                }
            }
            return shapeObject;
        }

        private Material ProcessMaterial(Dictionary<object, object> materialData)
        {
            var material = new Material();

            foreach (var materialInstruction in materialData)
            {
                var instruction = materialInstruction.Key.ToString();
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
            var patternInstructions = patternData as Dictionary<object, object>;
            if (patternInstructions == null)
            {
                throw new System.Exception("Unknown type(s) in pattern data");
            }

            Pattern pattern = null;
            foreach (var key in patternInstructions.Keys)
            {
                var instruction = key.ToString();
                if (instruction.Contains("type"))
                {
                    pattern = GetEmptyPattern(patternInstructions[instruction].ToString());
                }
                else if (instruction.Contains("parent-pattern"))
                {
                    ((NestedPattern)pattern).ParentPattern = ProcessPattern(patternInstructions[instruction]);
                }
                else if (instruction.Contains("patterns"))
                {
                    if (pattern is NestedPattern)
                    {
                        var subPatterns = patternInstructions[instruction] as Dictionary<object, object>;
                        if (subPatterns == null)
                        {
                            throw new System.Exception("Invalid sub-pattern data format");
                        }
                        ((NestedPattern)pattern).PatternA = ProcessPattern(subPatterns["pattern-a"]);
                        ((NestedPattern)pattern).PatternB = ProcessPattern(subPatterns["pattern-b"]);
                    }
                    else if (pattern is BlendedPattern)
                    {
                        var subPatterns = patternInstructions[instruction] as Dictionary<object, object>;
                        if (subPatterns == null)
                        {
                            throw new System.Exception("Invalid sub-pattern data format");
                        }
                        ((BlendedPattern)pattern).PatternA = ProcessPattern(subPatterns["pattern-a"]);
                        ((BlendedPattern)pattern).PatternB = ProcessPattern(subPatterns["pattern-b"]);
                    }
                    else
                    {
                        throw new System.Exception("Invalid compound pattern type");
                    }
                }
                else if (instruction.Contains("transform"))
                {
                    var transform = patternInstructions[instruction] as List<object>;
                    if (transform == null)
                    {
                        throw new System.Exception("Invalid transform format in pattern data");
                    }
                    pattern.Transform = ProcessTransform(transform);
                }
                else if (instruction.Contains("colours") || instruction.Contains("colors"))
                {
                    var colours = patternInstructions[instruction] as List<object>;
                    if (colours == null)
                    {
                        throw new System.Exception("Invalid colour data format");
                    }
                    
                    var colourLevels = ExtractTuple(colours[0], "light intensity");
                    pattern.ColourA = new Colour(colourLevels[0], colourLevels[1], colourLevels[2]);
                    colourLevels = ExtractTuple(colours[1], "light intensity");
                    pattern.ColourB = new Colour(colourLevels[0], colourLevels[1], colourLevels[2]);
                }
            }

            return pattern;
        }

        private Pattern GetEmptyPattern(string patternType)
        {
            switch (patternType)
            {
                case "blended":
                    return new BlendedPattern(null, null);
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
                case "nested":
                    return new NestedPattern(null, null, null);
                case "perturbed":
                    return new PerturbedPattern(null);
                case "ring":
                    return new RingPattern(Colour.BLACK, Colour.WHITE);
                case "stripe":
                    return new StripePattern(Colour.BLACK, Colour.WHITE);
                default:
                    throw new System.Exception($"Unknown Pattern type {patternType}");
            }
        }

        private Matrix ProcessTransform(List<object> transformData)
        {
            var rotation = Matrix.Identity(4);
            var scaling = Matrix.Identity(4);
            var translation = Matrix.Identity(4);
            var definedTransform = Matrix.Identity(4);

            foreach (var transformInstruction in transformData)
            {
                if (_transformDefinitions.ContainsKey(transformInstruction.ToString()))
                {
                    definedTransform = ProcessTransform(_transformDefinitions[transformInstruction.ToString()]);
                }
                else
                {
                    var transformItem = transformInstruction as List<object>;
                    var instruction = transformItem[0].ToString();

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
                        throw new System.Exception("Unknown transform type in transform data");
                    }
                }
            }

            return translation * scaling * rotation * definedTransform;
        }

        private Light ProcessLight(List<KeyValuePair<string, object>> lightData)
        {
            Point lightPosition = null;
            Colour lightIntensity = null;
            for (int i = 0; i < lightData.Count; i++)
            {
                var attribute = lightData[i];
                if (attribute.Key.StartsWith("at"))
                {
                    var positionCoords = ExtractTuple(attribute.Value, "light position");
                    lightPosition = new Point(positionCoords[0], positionCoords[1], positionCoords[2]);
                }
                else if (attribute.Key.StartsWith("intensity"))
                {
                    var colourLevels = ExtractTuple(attribute.Value, "light intensity");
                    lightIntensity = new Colour(colourLevels[0], colourLevels[1], colourLevels[2]);
                }
                else
                {
                    throw new System.Exception("Unknown light attribute in YAML file");
                }
            }
            return new Light(lightPosition, lightIntensity);
        }

        private Camera ProcessCameraObject(List<KeyValuePair<string, object>> objData)
        {
            int width = 200;
            int height = 200;
            double fieldOfView = 0.785;
            var from = new Point();
            var to = new Point();
            var up = new Vector();
            System.Console.WriteLine("Adding: Camera");

            for (int i = 0; i < objData.Count; i++)
            {
                var attribute = objData[i];
                var attributeValue = attribute.Value.ToString();
                if (attribute.Key.StartsWith("width"))
                {
                    width = ExtractInteger(attributeValue, "camera width");
                }
                else if (attribute.Key.StartsWith("height"))
                {
                    height = ExtractInteger(attributeValue, "Camera height");
                }
                else if (attribute.Key.StartsWith("field-of-view"))
                {
                    fieldOfView = ExtractDouble(attributeValue, "Camera FoV");
                }
                else if (attribute.Key.StartsWith("from"))
                {
                    var tuple = ExtractTuple(attribute.Value, "Camera from");
                    from = new Point(tuple[0], tuple[1], tuple[2]);
                }
                else if (attribute.Key.StartsWith("to"))
                {
                    var tuple = ExtractTuple(attribute.Value, "Camera to");
                    to = new Point(tuple[0], tuple[1], tuple[2]);
                }
                else if (attribute.Key.StartsWith("up"))
                {
                    var tuple = ExtractTuple(attribute.Value, "Camera up");
                    up = new Vector(tuple[0], tuple[1], tuple[2]);
                }
                else if (attribute.Key.StartsWith("reflection-depth"))
                {
                    _renderingDetails["reflectiondepth"] = ExtractInteger(attributeValue, "Camera reflection depth");
                }
                else if (attribute.Key.StartsWith("antialias-level"))
                {
                    _renderingDetails["antialiaslevel"] = ExtractInteger(attributeValue, "Camera anti-alias level");
                }
                else if (attribute.Key.StartsWith("show-progress"))
                {
                    _renderingDetails["showprogress"] = ExtractBoolean(attributeValue, "Camera show progress") ? 1 : 0;
                }
                else
                {
                    throw new System.Exception($"Unknown camera attribute: {attribute.Key}");
                }
            }

            var camera = new Camera(width, height, fieldOfView);
            camera.Transform = Transformations.ViewTransform(from, to, up);
            return camera;
        }

        private List<double> ExtractTuple(object tupleToConvert, string description)
        {
            try
            {
                var xyz = new List<double>();
                foreach (var coord in tupleToConvert as IEnumerable<object>)
                {
                    xyz.Add(ExtractDouble(coord.ToString(), description));
                }
                return xyz;
            }
            catch (System.Exception)
            {
                throw new System.FormatException($"Could not convert {tupleToConvert} into an integer for {description}");
            }
        }

        private bool ExtractBoolean(string valueToConvert, string description)
        {
            bool result;
            if (bool.TryParse(valueToConvert, out result))
                return result;
            else
                throw new System.FormatException($"Could not convert {valueToConvert} into a boolean for {description}");
        }

        private int ExtractInteger(string valueToConvert, string description)
        {
            int result;
            if (int.TryParse(valueToConvert, out result))
                return result;
            else
                throw new System.FormatException($"Could not convert {valueToConvert} into an integer for {description}");
        }

        private double ExtractDouble(string valueToConvert, string description)
        {
            double result;
            if (double.TryParse(valueToConvert, out result))
                return result;
            else
                throw new System.FormatException($"Could not convert {valueToConvert} into a double for {description}");
        }
    }
}
