using System;

namespace SavannahManagerStyleLib.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class SortingAttribute : Attribute
{
    public Type? AscComparer { get; set; }
    public Type? DescComparer { get; set; }
}