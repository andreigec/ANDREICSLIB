using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace ANDREICSLIB.ClassExtras
{
    /// <summary>
    /// example usage: https://github.com/andreigec/Folder-View
    /// </summary>
    public static class StringExtras
	{
        public static string GetMD5OfString(string s)
        {
            using (var md5Hash = MD5.Create())
            {
                var hash = GetMd5Hash(md5Hash, s);
                return hash;
            }
        }

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

		public static string[] SplitString(string instr, string split, bool removeempty = true)
		{
			var s = new string[1];
			s[0] = split;
			return instr.Split(s, removeempty ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
		}

		public static Tuple<string, string> SplitTwo(string instr, char sep)
		{
			var sep2 = new[] { sep };
			string[] sep3 = instr.Split(sep2);
			if (sep3.Count() != 2)
				return null;

			return new Tuple<string, string>(sep3[0], sep3[1]);
		}

		public static Tuple<int, int> SplitTwoInt(string instr, char sep)
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
		public static int ContainsSubStringCount(string instr, string substring)
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
		public static List<string> SplitStrings(string instr, int max)
		{
			string line = "";
			var ls = new List<string>();

			int count = 0;
			var splitchar = new[] { ' ' };
			string[] splitspace = instr.Split(splitchar);
			foreach (string s in splitspace)
			{
				if (string.IsNullOrEmpty(s))
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
		public static string PadString(string instr, int maxlen)
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

		public static string Truncate(string instr, int maxlen, string trucatedend = "...")
		{
			string outstr = instr;
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
		public static void ReplaceCharAtPosition(ref string str, char newChar, int position)
		{
			if (position < 0)
				return;

			if (string.IsNullOrEmpty(str))
				return;

			string bef = str.Substring(0, position);
			string af = str.Substring(position + 1);
			str = bef + newChar.ToString(CultureInfo.InvariantCulture) + af;
		}

		/// <summary>
		/// replace a length of string in a string with another string
		/// </summary>
		/// <param name="str"></param>
		/// <param name="newstr"></param>
		/// <param name="startpos"></param>
		/// <param name="endpos"></param>
		public static string ReplaceStringAtPosition(string str, string newstr, int startpos, int endpos)
		{
			if (startpos < 0 || endpos < 0)
				return str;

			if (string.IsNullOrEmpty(str))
				return str;

			string bef = str.Substring(0, startpos);
			string af = str.Substring(endpos);
			str = bef + newstr + af;
			return str;
		}

		/// <summary>
		/// Replace all instances of a char with another char
		/// </summary>
		/// <param name="origString"></param>
		/// <param name="replaceThis"></param>
		/// <param name="withThis"></param>
		/// <returns></returns>
		public static string ReplaceAllChars(string origString, char replaceThis, char withThis)
		{
			if (string.IsNullOrEmpty(origString))
				return origString;

			return origString.Replace(replaceThis, withThis);
		}

		public static string ReplaceAllChars(string origString, string replaceThis, string withThis)
		{
			if (string.IsNullOrEmpty(origString))
				return origString;

			return origString.Replace(replaceThis, withThis);
		}

		public static string RemoveAllNonAlphabetChars(string origString)
		{
			string outstr = "";
			for (int a = 0; a < origString.Length; a++)
			{
				char c = origString[a];

				if (char.IsLetter(c) == false && char.IsWhiteSpace(c) == false)
					continue;

				outstr += c;
			}
			return outstr;
		}

		public static string ReplaceAllChars(string origString, string replaceTheseChars, char withThis)
		{
			string outstr = "";
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
		/// merges all consecutive whitespace into one character
		/// </summary>
		/// <param name="origString"></param>
		/// <returns></returns>
		public static string MergeWhiteSpace(string origString, char mergeTo = ' ')
		{
			var mstr = mergeTo.ToString();
			var str = origString;
			str = str.Replace("\r\n", mstr);
			str = str.Replace('\n', mergeTo);
			str = str.Replace('\r', mergeTo);
			str = str.Replace('\t', mergeTo);

			str = str.Replace("  ", " ");
			str = str.Replace("  ", " ");
			str = str.Replace("  ", " ");
			str = str.Replace("  ", " ");

			str = str.Replace(' ', mergeTo);
			return str;
		}

		/// <summary>
		/// removes \n \r and \0 from the start and end of a string
		/// </summary>
		/// <param name="origString"></param>
		/// <returns>the 'cleaned' string</returns>
		public static string CleanString(string origString)
		{
			if (string.IsNullOrEmpty(origString))
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
		public static string AddText(string origString, string addText, bool isFront)
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
		public static string ToCamelCase(string origString, Boolean capitaliseInitial,
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

		public static bool StringStartsWithLetter(string s)
		{
			if (s.Length == 0)
				return false;

			return ((s[0] >= 65 && s[0] <= 90) || (s[0] >= 97 && s[0] <= 122));
        }


        /// <summary>
        /// remove comment lines etc
        /// </summary>
        /// <param name="multiline"></param>
        /// <returns></returns>
        public static string RemoveComments(string multiline)
        {
            var blockComments = @"/\*(.*?)\*/";
            var lineComments = @"//(.*?)\r?\n";
            var hashComments = @"#(.*?)\r?\n";
            var strings = @"""((\\[^\n]|[^""\n])*)""";
            var verbatimStrings = @"@(""[^""]*"")+";

            string noComments = Regex.Replace(multiline,
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
		/// <param name="instr"></param>
		/// <param name="pattern"></param>
		/// <param name="minCount"></param>
		/// <returns>count,start position, end pos - must be more than mincount</returns>
		public static Tuple<int, int, int> GetFirstRepetition(string instr, string pattern, int minCount = 2)
		{
			int count = 0;

			int a = 0;
			int firstpos = -1;
			int lastpos = 0;
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
		
        public static string Trim(this string s, string refstr, char[] trimchars = null, StringComparison sc = StringComparison.CurrentCultureIgnoreCase)
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

        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }
	}
}