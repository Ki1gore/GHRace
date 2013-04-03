using System;
namespace Interfaces
{
    public interface ICommonUtilities
    {
        float GetFloat(string p);
        float GetFloatFromString(System.Collections.Generic.IEnumerable<string> tdl);
        int GetInt(string p);
        int? GetSP(string p, int section);
        string GetSP3(string p);
        double? GetTimeDistance1(string p, int section);
        string GetTimeDistance3(string p);
        bool IsFraction(System.Collections.Generic.IEnumerable<string> tdl);
        string RemoveLetters(string p);
        string RemoveNumbers(string p);
    }
}
