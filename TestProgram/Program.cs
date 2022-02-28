using System.Diagnostics;
using Among.Switch;
using Among.Switch.Bflyt;
using Among.Switch.Byml;
using Newtonsoft.Json;

switch (args[0]) {
    case "extract": {
        SarcFile sarc = SarcFile.Load(File.ReadAllBytes(args[1]));
        string dirName = Path.GetFileName(args[1]) + " Files";
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
        Console.WriteLine(JsonConvert.SerializeObject(byml, Formatting.Indented));
        // Console.WriteLine(byml.Dump(new DumpOptions {
        //     DumpStyle = DumpStyle.CSharp,
        //     MaxLevel = int.MaxValue
        // }));
        break;
    }
    case "bflyt": {
        BflytFile bflyt = BflytFile.Load(File.ReadAllBytes(args[1]));
        Console.WriteLine(ObjectDumper.Dump(bflyt));
        break;
    }
}

