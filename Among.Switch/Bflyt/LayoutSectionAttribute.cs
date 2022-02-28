using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Among.Switch.Bflyt; 

public class LayoutSectionAttribute : Attribute {
    public string AsciiName { get; }
    public LayoutSectionAttribute(string asciiName) {
        AsciiName = asciiName;
    }
}