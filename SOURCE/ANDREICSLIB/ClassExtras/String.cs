﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ANDREICSLIB
{
    public static class StringExtras
    {
        public static string[] SplitString(String instr, String split, bool removeempty = true)
        {
            var s = new string[1];
            s[0] = split;
            return instr.Split(s, removeempty ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
        }

        public static Tuple<string, string> SplitTwo(String instr, char sep)
        {
            var sep2 = new[] {sep};
            string[] sep3 = instr.Split(sep2);
            if (sep3.Count() != 2)
                return null;

            return new Tuple<string, string>(sep3[0], sep3[1]);
        }

        public static Tuple<int, int> SplitTwoInt(String instr, char sep)
        {
            Tuple<string, string> x = SplitTwo(instr, sep);
            return new Tuple<int, int>(Int32.Parse(x.Item1), Int32.Parse(x.Item2));
        }

        /// <summary>
        /// count how many occurences of a substring occur in a string
        /// </summary>
        /// <param name="instr"></param>
        /// <param name="substring"></param>
        /// <returns></returns>
        public static int ContainsSubStringCount(String instr, String substring)
        {
            int count = 0;

            int c = instr.Length;
            int c1 = substring.Length;
            for (int a = 0; a < c; a++)
            {
                if (instr.Substring(a, c1).Equals(substring))
                {
                    count++;
                    a += c1 - 1;
                }
            }

            return count;
        }

        /// <summary>
        /// Split a long string into separate lines based on a max length
        /// </summary>
        /// <param name="instr"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static List<string> SplitStrings(String instr, int max)
        {
            string line = "";
            var ls = new List<string>();

            int count = 0;
            var splitchar = new[] {' '};
            string[] splitspace = instr.Split(splitchar);
            foreach (string s in splitspace)
            {
                if (String.IsNullOrEmpty(s))
                    continue;

                if ((count + s.Length) > max)
                {
                    ls.Add(PadString(line, max));
                    line = "";
                    count = 0;
                }

                string z = s + " ";
                count += z.Length;
                line += z;
            }
            if (count != 0)
            {
                ls.Add(PadString(line, max));
            }
            return ls;
        }

        /// <summary>
        /// Truncate or pad a string with spaces to a certain length
        /// </summary>
        /// <param name="instr"></param>
        /// <param name="maxlen"></param>
        /// <returns></returns>
        public static String PadString(String instr, int maxlen)
        {
            string ret = "";
            int len = instr.Length;
            for (int a = 0; a < maxlen; a++)
            {
                if (a < len)
                    ret += instr[a];
                else
                    ret += " ";
            }
            return ret;
        }

        public static string Truncate(String instr, int maxlen, string trucatedend = "...")
        {
            String outstr = instr;
            if (outstr.Length > maxlen)
            {
                outstr = outstr.Substring(0, maxlen);
                outstr += trucatedend;
            }
            return outstr;
        }

        /// <summary>
        /// Replace a char in a string with another
        /// </summary>
        /// <param name="str">The string to change a character in</param>
        /// <param name="newChar">The new character to be used</param>
        /// <param name="position">The position to use the new character</param>
        /// <returns>a string with the character replaced</returns>
        public static void ReplaceCharAtPosition(ref String str, char newChar, int position)
        {
            if (position < 0)
                return;

            if (String.IsNullOrEmpty(str))
                return;

            string bef = str.Substring(0, position);
            string af = str.Substring(position + 1);
            str= bef + newChar.ToString(CultureInfo.InvariantCulture) + af;
        }

        /// <summary>
        /// Replace all instances of a char with another char
        /// </summary>
        /// <param name="origString"></param>
        /// <param name="replaceThis"></param>
        /// <param name="withThis"></param>
        /// <returns></returns>
        public static String ReplaceAllChars(String origString, char replaceThis, char withThis)
        {
            if (String.IsNullOrEmpty(origString))
                return origString;

            return origString.Replace(replaceThis, withThis);
        }

        public static String ReplaceAllChars(String origString, String replaceThis, String withThis)
        {
            if (String.IsNullOrEmpty(origString))
                return origString;

            return origString.Replace(replaceThis, withThis);
        }

        public static String RemoveAllNonAlphabetChars(String origString)
        {
            String outstr = "";
            for (int a = 0; a < origString.Length; a++)
            {
                char c = origString[a];

                if (char.IsLetter(c) == false && char.IsWhiteSpace(c) == false)
                    continue;

                outstr += c;
            }
            return outstr;
        }

        public static String ReplaceAllChars(String origString, String replaceTheseChars, char withThis)
        {
            String outstr = "";
            for (int a = 0; a < origString.Length; a++)
            {
                char c = origString[a];
                if (replaceTheseChars.Contains(c))
                    c = withThis;

                outstr += c;
            }
            return outstr;
        }


        /// <summary>
        /// Trim a string of a certain number of chars, either from the start or the end
        /// </summary>
        /// <param name="origString"></param>
        /// <param name="isFront"></param>
        /// <param name="length"></param>
        /// <param name="relativeStart">front=true, start=relativestart. front=end, start=end-length+relativestart</param>
        /// <returns></returns>
        public static String ApplyTrim(String origString, bool isFront, int length, int relativeStart = 0)
        {
            //relative start is bad or length is more than the entire length, then cancel
            if (relativeStart < 0 ||
                (isFront && (relativeStart + length) > origString.Length) ||
                (isFront == false && (origString.Length + relativeStart - length) > origString.Length))
                return origString;

            if (isFront == false)
                relativeStart = origString.Length - length + relativeStart;

            return origString.Remove(relativeStart, length);
        }
        /*
          public static string[] SplitAtArgument(string s1)
        {

            var ret = new List<string>();
            string str = s1;
            int colonloc = 0;
            //find next colon
            while ((colonloc=str.IndexOf(":"))!=-1)
            {
                //go back to next space
                int spaceloc = colonloc;
                while (spaceloc > 0 && str[spaceloc] != ' ')
                {
                    spaceloc--;
                }

                //get next colon or end
                int nextcolonloc = str.IndexOf(":", colonloc+1);
                //get previous space
                int nextspaceloc = nextcolonloc;
                while (nextspaceloc > 0 && str[nextspaceloc] != ' ')
                {
                    nextspaceloc--;
                }

                //get arg name/val
                string argname = str.Substring(spaceloc, colonloc - spaceloc);
                string argval = "";
                if (nextspaceloc!=-1)
                argval= str.Substring(colonloc+1, nextspaceloc - colonloc);
                //remove up to next space loc
                str=str.Remove(spaceloc, nextspaceloc - spaceloc);
                argname = CleanString(argname);
                argval=CleanString(argval);
                ret.Add(argname);
                ret.Add(argval);
            }

            return ret.ToArray();
        }
         * */

        /// <summary>
        /// removes \n \r and \0 from the start and end of a string
        /// </summary>
        /// <param name="origString"></param>
        /// <returns>the 'cleaned' string</returns>
        private static String CleanString(String origString)
        {
            if (String.IsNullOrEmpty(origString))
                return origString;

            char[] bad = { '\n', '\r', '\0', ' ' };

            //keep going while changes have been made
            bool change = true;
            while (change)
            {
                change = false;
                if (origString.Length == 0)
                    return "";
                char start = origString[0];
                char end = origString[origString.Length - 1];


                foreach (char b in bad)
                {
                    if (start == b)
                    {
                        origString = origString.Remove(0, 1);
                        change = true;
                    }
                    if (end == b)
                    {
                        origString = origString.Remove(origString.Length - 1, 1);
                        change = true;
                    }

                    if (change)
                        break;
                }
            }

            //remove duplicate whitespace
            change = true;
            while (change)
            {
                int len = origString.Length;
                origString = origString.Replace("  ", " ");
                if (origString.Length == len)
                    change = false;
            }

            return origString;
        }

        /// <summary>
        /// append/prepend text to a string
        /// </summary>
        /// <param name="origString"></param>
        /// <param name="addText"></param>
        /// <param name="isFront"></param>
        /// <returns></returns>
        public static String AddText(String origString, String addText, bool isFront)
        {
            if (isFront)
                return addText + origString;
            else
                return origString + addText;
        }


        /// <summary>
        /// Auto capitalise a string - first letter in each word is capitalised, rest are lower case
        /// </summary>
        /// <param name="origString">The string to change</param>
        /// <param name="capitaliseInitial">Should the first letter be capitalised?</param>
        /// <param name="capitaliseWordString"> </param>
        /// <returns>the auto capitalised string</returns>
        public static String ToCamelCase(String origString, Boolean capitaliseInitial,
                                         List<string> capitaliseWordString = null, bool spaceAfter = true)
        {
            if (string.IsNullOrEmpty(origString))
                return null;

            try
            {
                string outstr = origString.ToLower();
                //dj/mc

                var openbracket = new List<char>();
                var closebracket = new List<char>();

                openbracket.Add('(');
                closebracket.Add(')');
                openbracket.Add('[');
                closebracket.Add(']');
                openbracket.Add('{');
                closebracket.Add('}');
                openbracket.Add('<');
                closebracket.Add('>');

                var capitalAfter = new List<char>();
                capitalAfter.AddRange(openbracket);
                capitalAfter.AddRange(closebracket);
                capitalAfter.Add('/');
                capitalAfter.Add('\\');
                capitalAfter.Add(',');
                capitalAfter.Add('.');

                for (int a = 0; a < outstr.Length; a++)
                {
                    char x = outstr[a];

                    if (Char.IsLetter(x))
                    {
                        //capitalised words
                        foreach (string CWS in capitaliseWordString)
                        {
                            if ((a + CWS.Length) < outstr.Length && outstr.Substring(a, CWS.Length).Equals(CWS))
                            {
                                //only if the following char is blank or end of line, or a special char is after

                                if (((a + CWS.Length) > outstr.Length) || (Char.IsWhiteSpace(outstr[a + CWS.Length])) ||
                                    (capitalAfter.Contains(outstr[a + CWS.Length])))
                                {
                                    for (int a1 = a; a1 < a + CWS.Length; a1++)
                                    {
                                        char x1 = outstr[a1];
                                        x1 = Char.ToUpper(x1);
                                        ReplaceCharAtPosition(ref outstr, x1, a1);
                                    }
                                }
                            }
                        }

                        //cap if first, or after space (camel case), or bracket
                        if ((a == 0 && capitaliseInitial) || Char.IsWhiteSpace(outstr[a - 1]) ||
                            capitalAfter.Contains(outstr[a - 1]))
                            ReplaceCharAtPosition(ref outstr, Char.ToUpper(x), a);
                    }

                    //space after comma and close bracket
                    if (spaceAfter)
                    {
                        if (((a + 1) < outstr.Length) &&
                            ((outstr[a] == ',' || closebracket.Contains(outstr[a])) &&
                             Char.IsWhiteSpace(outstr[a + 1]) == false))
                        {
                            outstr = outstr.Insert(a + 1, " ");
                        }

                        //space before openbracket
                        if (((a - 1) > 0) && (openbracket.Contains(outstr[a])) &&
                            (Char.IsWhiteSpace(outstr[a - 1]) == false))
                        {
                            outstr = outstr.Insert(a, " ");
                        }
                    }
                }

                return outstr;
            }
            catch
            {
                return origString;
            }
        }

        public static bool StringIsNumber(string s)
        {
            try
            {
                Double.Parse(s);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool StringStartsWithLetter(String s)
        {
            if (s.Length == 0)
                return false;

            return ((s[0] >= 65 && s[0] <= 90) || (s[0] >= 97 && s[0] <= 122));
        }
    }
}