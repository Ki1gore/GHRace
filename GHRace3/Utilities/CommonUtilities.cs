using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces;

namespace Utilities
{
    public class CommonUtilities : ICommonUtilities
    {
        public float GetFloat(string p)
        {
            p = RemoveLetters(p);
            return float.Parse(p);
        }

        public int GetInt(string p)
        {
            p = RemoveLetters(p);
            return int.Parse(p);
        }

        public string RemoveLetters(string p)
        {
            char[] numbers = { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '.', '/' };
            List<char> numList = numbers.ToList();
            StringBuilder sb = new StringBuilder();

            foreach (char c in p)
            {
                if (numList.Contains(c))
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        public string RemoveNumbers(string p)
        {
            char[] numbers = { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '.', '/' };
            List<char> numList = numbers.ToList();
            StringBuilder sb = new StringBuilder();

            foreach (char c in p)
            {
                if (!numList.Contains(c))
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        public float GetFloatFromString(IEnumerable<string> tdl)
        {
            List<string> temp = tdl as List<string>;
            float returnValue = 0;
            if (IsFraction(temp))
            {
                if (temp.Count > 1)
                {
                    string[] s = temp[1].Split(new Char[] { '/' });
                    decimal r = int.Parse(temp[0]) + decimal.Divide(decimal.Parse(s[0]), decimal.Parse(s[1]));
                    returnValue = float.Parse(r.ToString());
                }
                else
                {
                    string[] s;
                    try
                    {
                        s = temp[1].Split(new Char[] { '/' });
                    }
                    catch (Exception)
                    {
                        s = temp[0].Split(new Char[] { '/' });
                    }

                    decimal p = decimal.Divide(int.Parse(s[0]), int.Parse(s[1]));
                    returnValue = float.Parse(p.ToString());
                }
            }
            else
            {
                returnValue = float.Parse(temp[0]);
            }
            return returnValue;
        }

        public bool IsFraction(IEnumerable<string> tdl)
        {
            bool result = true;
            foreach (string item in tdl)
            {
                if (item.IndexOf('/') < 0)
                {
                    result = false;
                }
                else
                {
                    result = true;
                }
            }
            return result;
        }


        public int? GetSP(string p, int section)
        {
            p = RemoveLetters(p);
            string[] split = p.Split(new Char[] { '/' });
            return GetInt(split[section]);
        }

        public string GetSP3(string p)
        {
            char[] numbers = { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '.', '-', '/' };
            List<char> numList = numbers.ToList();
            StringBuilder sb = new StringBuilder();

            foreach (char c in p)
            {
                if (!(numList.Contains(c)))
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        public double? GetTimeDistance1(string p, int section)
        {
            float returnValue = 0;

            if (section == 0)
            {
                try
                {
                    string[] split = p.Split(new Char[] { ' ' });
                    returnValue = GetFloat(split[section]);
                }
                catch (Exception)
                {
                    returnValue = 0;
                }

            }
            else
            {
                string[] split = p.Split(new Char[] { ' ' });
                List<string> timeDistanceList = split.ToList();
                timeDistanceList.RemoveAt(0);
                List<string> tdl = new List<string>();

                foreach (string item in timeDistanceList)
                {
                    string s;
                    string t;

                    s = (item.Replace("(", ""));
                    t = s.Replace(")", "");
                    tdl.Add(t);
                }

                try
                {
                    float f = float.Parse(tdl[0].Replace("/", "")); //if this throws an exception, we tried to parse letters (like SH which go in a different column)
                    returnValue = GetFloatFromString(tdl);
                }
                catch (Exception)
                {
                    returnValue = 0; //nothing to go into TimeDistance2
                }

            }


            return returnValue;
        }

        public string GetTimeDistance3(string p)
        {
            char[] numbers = { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '.', '-', '/', '(', ')' };
            List<char> numList = numbers.ToList();
            StringBuilder sb = new StringBuilder("");
            foreach (char item in p)
            {
                if (!(numList.Contains(item)))
                {
                    sb.Append(item);
                }

            }
            return sb.ToString().Trim();
        }

    }
}
