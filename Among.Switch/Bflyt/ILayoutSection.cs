﻿using Among.Switch.Buffers;

namespace Among.Switch.Bflyt; 

public interface ILayoutSection : IReadableStructure {
    public string SectionName { get; set; }
}