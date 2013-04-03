using System;
namespace Interfaces
{
    public interface IGreyhound
    {
        int GreyhoundID { get; set; }
        string Name { get; set; }
        System.Collections.Generic.ICollection<IRace> Races { get; set; }
    }
}
