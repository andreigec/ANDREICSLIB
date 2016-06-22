using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace ANDREICSLIB.ClassExtras
{
    /// <summary>
    ///     example usage: https://github.com/andreigec/Folder-View
    /// </summary>
    public static class StringExtras
    {
        /// <summary>
        /// Parses the currency.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <param name="truncateToDecimalPlace">truncate to this number.</param>
        /// <param name="truncateEndingZeros">if set to <c>true</c> will remove all ending 0s.</param>
        /// <param name="minDecimalPlaces">if set, will pad ending with 0s</param>
        /// <returns></returns>
        public static decimal ParseCurrency(this string c, int? truncateToDecimalPlace = null,
            bool truncateEndingZeros = true, int? minDecimalPlaces = 1)
        {
            var ret = 0m;
            if (string.IsNullOrEmpty(c))
                return 0;

            var r = new Regex(@"\$?((([1-9][0-9]{0,2}(,[0-9]{3})*)|0)?(\.[0-9]{1,2})?)?");
            var res = r.Match(c);
            if (!res.Success || res.Groups.Count < 2)
                return ret;

            if (!decimal.TryParse(res.Groups[1].Value,
                NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands,
                CultureInfo.CurrentCulture, out ret))
                return ret;

            if (truncateToDecimalPlace.HasValue)
                ret = decimal.Round(ret, truncateToDecimalPlace.Value, MidpointRounding.AwayFromZero);

            if (truncateEndingZeros)
                ret = ret / 1.000000000000000000000000000000000m;

            if (minDecimalPlaces.HasValue)
            {
                int count = BitConverter.GetBytes(decimal.GetBits(ret)[3])[2];
                if (count != 0)
                    return ret;

                var retstr = ret.ToString(CultureInfo.InvariantCulture);
                if (!retstr.Contains("."))
                    retstr += ".";

                for (var a = 0; a < minDecimalPlaces.Value; a++)
                    retstr += "0";

                ret = decimal.Parse(retstr);
            }
            return ret;
        }

        /// <summary>
        /// convert a string to a stream
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        public static Stream ToStream(this string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        /// <summary>
        /// Gets the md5 of string.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        public static string GetMD5OfString(string s)
        {
            using (var md5Hash = MD5.Create())
            {
                var hash = GetMd5Hash(md5Hash, s);
                return hash;
            }
        }

        /// <summary>
        /// Gets the MD5 hash.
        /// </summary>
        /// <param name="md5Hash">The MD5 hash.</param>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static string GetMd5Hash(MD5 md5Hash, string input)
        {
            // Convert the input string to a byte array and compute the hash. 
            var data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes 
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data  
            // and format each one as a hexadecimal string. 
            for (var i = 0; i < data.Length; i++)
            {
                sBuilder.Append(i.ToString("x2"));
            }

            // Return the hexadecimal string. 
            return sBuilder.ToString();
        }

        /// <summary>
        /// Splits the string.
        /// </summary>
        /// <param name="instr">The instr.</param>
        /// <param name="split">The split.</param>
        /// <param name="removeempty">if set to <c>true</c> [removeempty].</param>
        /// <returns></returns>
        public static string[] SplitString(string instr, string split, bool removeempty = true)
        {
            var s = new string[1];
            s[0] = split;
            return instr.Split(s, removeempty ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
        }

        /// <summary>
        /// Splits the two.
        /// </summary>
        /// <param name="instr">The instr.</param>
        /// <param name="sep">The sep.</param>
        /// <returns></returns>
        public static Tuple<string, string> SplitTwo(string instr, char sep)
        {
            var sep2 = new[] { sep };
            var sep3 = instr.Split(sep2);
            if (sep3.Count() != 2)
                return null;

            return new Tuple<string, string>(sep3[0], sep3[1]);
        }

        /// <summary>
        /// Splits the two int.
        /// </summary>
        /// <param name="instr">The instr.</param>
        /// <param name="sep">The sep.</param>
        /// <returns></returns>
        public static Tuple<int, int> SplitTwoInt(string instr, char sep)
        {
            var x = SplitTwo(instr, sep);
            return new Tuple<int, int>(Int32.Parse(x.Item1), Int32.Parse(x.Item2));
        }

        /// <summary>
        /// count how many occurences of a substring occur in a string
        /// </summary>
        /// <param name="instr">The instr.</param>
        /// <param name="substring">The substring.</param>
        /// <returns></returns>
        public static int ContainsSubStringCount(string instr, string substring)
        {
            var count = 0;

            var c = instr.Length;
            var c1 = substring.Length;
            for (var a = 0; a < c; a++)
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
        /// <param name="instr">The instr.</param>
        /// <param name="max">The maximum.</param>
        /// <returns></returns>
        public static List<string> SplitStrings(string instr, int max)
        {
            var line = "";
            var ls = new List<string>();

            var count = 0;
            var splitchar = new[] { ' ' };
            var splitspace = instr.Split(splitchar);
            foreach (var s in splitspace)
            {
                if (String.IsNullOrEmpty(s))
                    continue;

                if ((count + s.Length) > max)
                {
                    ls.Add(PadString(line, max));
                    line = "";
                    count = 0;
                }

                var z = s + " ";
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
        /// <param name="instr">The instr.</param>
        /// <param name="maxlen">The maxlen.</param>
        /// <returns></returns>
        public static string PadString(string instr, int maxlen)
        {
            var ret = "";
            var len = instr.Length;
            for (var a = 0; a < maxlen; a++)
            {
                if (a < len)
                    ret += instr[a];
                else
                    ret += " ";
            }
            return ret;
        }

        /// <summary>
        /// Truncates the specified instr.
        /// </summary>
        /// <param name="instr">The instr.</param>
        /// <param name="maxlen">The maxlen.</param>
        /// <param name="trucatedend">The trucatedend.</param>
        /// <returns></returns>
        public static string Truncate(string instr, int maxlen, string trucatedend = "...")
        {
            var outstr = instr;
            if (outstr.Length > maxlen)
            {
                outstr = outstr.Substring(0, maxlen);
                outstr += trucatedend;
            }
            return outstr;
        }

        /// <summary>
        ///     Replace a char in a string with another
        /// </summary>
        /// <param name="str">The string to change a character in</param>
        /// <param name="newChar">The new character to be used</param>
        /// <param name="position">The position to use the new character</param>
        /// <returns>a string with the character replaced</returns>
        public static void ReplaceCharAtPosition(ref string str, char newChar, int position)
        {
            if (position < 0)
                return;

            if (String.IsNullOrEmpty(str))
                return;

            var bef = str.Substring(0, position);
            var af = str.Substring(position + 1);
            str = bef + newChar.ToString(CultureInfo.InvariantCulture) + af;
        }

        /// <summary>
        /// replace a length of string in a string with another string
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="newstr">The newstr.</param>
        /// <param name="startpos">The startpos.</param>
        /// <param name="endpos">The endpos.</param>
        /// <returns></returns>
        public static string ReplaceStringAtPosition(string str, string newstr, int startpos, int endpos)
        {
            if (startpos < 0 || endpos < 0)
                return str;

            if (String.IsNullOrEmpty(str))
                return str;

            var bef = str.Substring(0, startpos);
            var af = str.Substring(endpos);
            str = bef + newstr + af;
            return str;
        }

        /// <summary>
        /// Replace all instances of a char with another char
        /// </summary>
        /// <param name="origString">The original string.</param>
        /// <param name="replaceThis">The replace this.</param>
        /// <param name="withThis">The with this.</param>
        /// <returns></returns>
        public static string ReplaceAllChars(string origString, char replaceThis, char withThis)
        {
            if (String.IsNullOrEmpty(origString))
                return origString;

            return origString.Replace(replaceThis, withThis);
        }

        /// <summary>
        /// Replaces all chars.
        /// </summary>
        /// <param name="origString">The original string.</param>
        /// <param name="replaceThis">The replace this.</param>
        /// <param name="withThis">The with this.</param>
        /// <returns></returns>
        public static string ReplaceAllChars(string origString, string replaceThis, string withThis)
        {
            if (String.IsNullOrEmpty(origString))
                return origString;

            return origString.Replace(replaceThis, withThis);
        }

        /// <summary>
        /// Removes all non alphabet chars.
        /// </summary>
        /// <param name="origString">The original string.</param>
        /// <returns></returns>
        public static string RemoveAllNonAlphabetChars(string origString)
        {
            var outstr = "";
            for (var a = 0; a < origString.Length; a++)
            {
                var c = origString[a];

                if (Char.IsLetter(c) == false && Char.IsWhiteSpace(c) == false)
                    continue;

                outstr += c;
            }
            return outstr;
        }

        /// <summary>
        /// Replaces all chars.
        /// </summary>
        /// <param name="origString">The original string.</param>
        /// <param name="replaceTheseChars">The replace these chars.</param>
        /// <param name="withThis">The with this.</param>
        /// <returns></returns>
        public static string ReplaceAllChars(string origString, string replaceTheseChars, char withThis)
        {
            var outstr = "";
            for (var a = 0; a < origString.Length; a++)
            {
                var c = origString[a];
                if (replaceTheseChars.Contains(c))
                    c = withThis;

                outstr += c;
            }
            return outstr;
        }

        /// <summary>
        /// Trim a string of a certain number of chars, either from the start or the end
        /// </summary>
        /// <param name="origString">The original string.</param>
        /// <param name="isFront">if set to <c>true</c> [is front].</param>
        /// <param name="length">The length.</param>
        /// <param name="relativeStart">front=true, start=relativestart. front=end, start=end-length+relativestart</param>
        /// <returns></returns>
        public static string ApplyTrim(string origString, bool isFront, int length, int relativeStart = 0)
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

        /// <summary>
        /// merges all consecutive whitespace into one character, and trim
        /// </summary>
        /// <param name="origString">The original string.</param>
        /// <returns></returns>
        public static string MergeWhiteSpace(this string origString)
        {
            var s = Regex.Replace(origString, @"\s+", " ");
            var s2 = "";
            while (s2.Length != s.Length)
            {
                s2 = s;
                s = s.Replace("  ", " ");
            }
            return s.Trim();
        }

        /// <summary>
        /// append/prepend text to a string
        /// </summary>
        /// <param name="origString">The original string.</param>
        /// <param name="addText">The add text.</param>
        /// <param name="isFront">if set to <c>true</c> [is front].</param>
        /// <returns></returns>
        public static string AddText(string origString, string addText, bool isFront)
        {
            if (isFront)
                return addText + origString;
            return origString + addText;
        }

        /// <summary>
        /// Auto capitalise a string - first letter in each word is capitalised, rest are lower case
        /// </summary>
        /// <param name="origString">The string to change</param>
        /// <param name="capitaliseInitial">Should the first letter be capitalised?</param>
        /// <param name="capitaliseWordString">The capitalise word string.</param>
        /// <param name="spaceAfter">if set to <c>true</c> [space after].</param>
        /// <returns>
        /// the auto capitalised string
        /// </returns>
        public static string ToCamelCase(string origString, bool capitaliseInitial,
            List<string> capitaliseWordString = null, bool spaceAfter = true)
        {
            if (String.IsNullOrEmpty(origString))
                return null;

            try
            {
                var outstr = origString.ToLower();
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

                for (var a = 0; a < outstr.Length; a++)
                {
                    var x = outstr[a];

                    if (Char.IsLetter(x))
                    {
                        //capitalised words
                        foreach (var CWS in capitaliseWordString)
                        {
                            if ((a + CWS.Length) < outstr.Length && outstr.Substring(a, CWS.Length).Equals(CWS))
                            {
                                //only if the following char is blank or end of line, or a special char is after

                                if (((a + CWS.Length) > outstr.Length) || (Char.IsWhiteSpace(outstr[a + CWS.Length])) ||
                                    (capitalAfter.Contains(outstr[a + CWS.Length])))
                                {
                                    for (var a1 = a; a1 < a + CWS.Length; a1++)
                                    {
                                        var x1 = outstr[a1];
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

        /// <summary>
        /// Strings the is number.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Strings the starts with letter.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        public static bool StringStartsWithLetter(string s)
        {
            if (s.Length == 0)
                return false;

            return ((s[0] >= 65 && s[0] <= 90) || (s[0] >= 97 && s[0] <= 122));
        }

        /// <summary>
        /// remove comment lines etc
        /// </summary>
        /// <param name="multiline">The multiline.</param>
        /// <returns></returns>
        public static string RemoveComments(string multiline)
        {
            var blockComments = @"/\*(.*?)\*/";
            var lineComments = @"//(.*?)\r?\n";
            var hashComments = @"#(.*?)\r?\n";
            var strings = @"""((\\[^\n]|[^""\n])*)""";
            var verbatimStrings = @"@(""[^""]*"")+";

            var noComments = Regex.Replace(multiline,
                blockComments + "|" + lineComments + "|" + hashComments + "|" + strings + "|" + verbatimStrings,
                me =>
                {
                    if (me.Value.StartsWith("/*") || me.Value.StartsWith("//") || me.Value.StartsWith("#"))
                        return me.Value.StartsWith("//") ? Environment.NewLine : "";
                    // Keep the literal strings
                    return me.Value;
                },
                RegexOptions.Singleline);

            return noComments;
        }

        /// <summary>
        /// get first block of repetitions in string
        /// </summary>
        /// <param name="instr">The instr.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="minCount">The minimum count.</param>
        /// <returns>
        /// count,start position, end pos - must be more than mincount
        /// </returns>
        public static Tuple<int, int, int> GetFirstRepetition(string instr, string pattern, int minCount = 2)
        {
            var count = 0;

            var a = 0;
            var firstpos = -1;
            var lastpos = 0;
            while ((a = instr.IndexOf(pattern, a)) != -1)
            {
                var notConsecutive = lastpos != (a - pattern.Length);
                //if not consecutive, but have enough, then break
                if (notConsecutive && count >= minCount)
                    break;

                count++;
                if (firstpos == -1)
                {
                    lastpos = firstpos = a;
                }
                else
                {
                    //if not consecutive, restart
                    if (notConsecutive)
                    {
                        lastpos = firstpos = a;
                        count = 1;
                    }
                    else
                    {
                        lastpos = a;
                    }
                }

                a += pattern.Length;
            }

            lastpos += pattern.Length;
            if (count < minCount)
                return null;

            return new Tuple<int, int, int>(count, firstpos, lastpos);
        }

        /// <summary>
        /// Determines whether the specified word contains insensitive.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        /// <param name="word">The word.</param>
        /// <returns></returns>
        public static bool ContainsInsensitive(this string paragraph, string word)
        {
            var c = DateTimeExtras.MyCulture;
            return c.CompareInfo.IndexOf(paragraph, word, CompareOptions.IgnoreCase) >= 0;
        }

        /// <summary>
        /// Trims the specified refstr.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="refstr">The refstr.</param>
        /// <param name="trimchars">The trimchars.</param>
        /// <param name="sc">The sc.</param>
        /// <returns></returns>
        public static string Trim(this string s, string refstr, char[] trimchars = null,
            StringComparison sc = StringComparison.CurrentCultureIgnoreCase)
        {
            if (s.StartsWith(refstr, sc))
                s = s.Substring(refstr.Length);

            if (s.EndsWith(refstr, sc))
                s = s.Substring(0, s.Length - refstr.Length);

            s = s.Trim();
            if (trimchars != null)
                s = s.Trim(trimchars);
            s = s.Trim();

            return s;
        }

        /// <summary>
        /// Determines whether [is null or empty].
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string s)
        {
            return String.IsNullOrEmpty(s);
        }
    }
}