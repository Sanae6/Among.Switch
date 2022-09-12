using Among.Switch;
using Among.Switch.Byml;
using Among.Switch.Byml.Nodes;
using Newtonsoft.Json;

namespace TestProgram; 

public class MapDataExtractor {
    public void Load(string dir) {
        foreach (string file in Directory.EnumerateFiles(dir)) {
            string mapByml = Path.ChangeExtension(Path.GetFileName(file), ".byml");
            Console.WriteLine($"Loading {file} ~ {mapByml}");
            SarcFile sarc = SarcFile.Load(File.ReadAllBytes(file));
            if (sarc.Files.TryGetValue(mapByml, out byte[]? data)) {
                ReadByml(BymlFile.Load(data));
            } else {
                Console.WriteLine($"File {file} has no map byml {mapByml}");
            }
        }
    }

    private void ReadByml(BymlFile byml) {
        if (byml.Root == null) {
            Console.WriteLine("Byml had no root, continuing onward!");
            return;
        }
        foreach (DictionaryNode node in ((ArrayNode) byml.Root).Cast<DictionaryNode>()) {
            foreach ((string key, ArrayNode value) in node.Children.Select(x => new KeyValuePair<string, ArrayNode>(x.Key, (ArrayNode) x.Value))) {
                Console.WriteLine($"List: {key}");
                // Console.WriteLine(JsonConvert.SerializeObject(value));
                // Environment.Exit(1);
            }
        }
    }
}