using System.Diagnostics;
using Among.Switch;
using Among.Switch.Bflyt;
using Among.Switch.Byml;
using Newtonsoft.Json;
using TestProgram;

switch (args[0]) {
    case "sarc": {
        SarcFile sarc = SarcFile.Load(File.ReadAllBytes(args[1]));
        string dirName = Path.GetFileName(args[1]) + "_Files";
        Directory.CreateDirectory(dirName);
        foreach ((string? key, byte[]? data) in sarc.Files) {
            Debug.WriteLine($"Writing {key}");
            File.WriteAllBytes(Path.Combine(dirName, key), data);
            if (key.EndsWith("byml")) {
                BymlFile byml = BymlFile.Load(data);
            }
        }

        break;
    }
    case "byml": {
        BymlFile byml = BymlFile.Load(File.ReadAllBytes(args[1]));
        // Console.WriteLine(JsonConvert.SerializeObject(byml, Formatting.Indented));
        File.WriteAllText("lastByml.json", JsonConvert.SerializeObject(byml, Formatting.Indented));
        break;
    }
    case "bflyt": {
        BflytFile bflyt = BflytFile.Load(File.ReadAllBytes(args[1]));
        // Console.WriteLine(JsonConvert.SerializeObject(bflyt, Formatting.Indented));
        File.WriteAllText($"{Path.GetFileNameWithoutExtension(args[1])}.json", JsonConvert.SerializeObject(bflyt, Formatting.Indented));
        File.WriteAllBytes(Path.GetFileName(args[1]) + ".p", bflyt.Save().Buffer.ToArray());
        break;
    }
    case "odymap": {
        MapDataExtractor mapDataExtractor = new MapDataExtractor();
        mapDataExtractor.Load(args[1]);
        break;
    }
}

