using System;
using UnityEngine;

[Flags]
public enum DropdownRuntimeFlags
{
    None = 0,
    DontAddNoneInContentFirst = 1 << 0,
}

public class DropdownRuntimeAttribute : PropertyAttribute
{
    public string Label;
    public string ContentGetter;
    public DropdownRuntimeFlags Flags;

    public DropdownRuntimeAttribute(string contentGetter, DropdownRuntimeFlags flags = DropdownRuntimeFlags.None)
    {
        ContentGetter = contentGetter;
        Label = string.Empty;
        Flags = flags;
    }
    public DropdownRuntimeAttribute(string contentGetter, string label, DropdownRuntimeFlags flags = DropdownRuntimeFlags.None)
        : this(contentGetter, flags)
    {
        Label = label;
    }
}
