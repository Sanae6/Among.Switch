﻿using System.Diagnostics;
using Among.Switch;
using Among.Switch.Byml;

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

        break;
    }
}

