using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ANDREICSLIB
{
	public static class StringUpdates
	{
        public static string[] SplitString(String instr,String split,bool removeempty=true)
        {
            var s = new string[1];
            s[0] = split;
            return instr.Split(s, removeempty?StringSplitOptions.RemoveEmptyEntries:StringSplitOptions.None);
        }
		public static Tuple<string, string> splitTwo(String instr,char sep)
		{
			var sep2 = new [] {sep};
			var sep3 = instr.Split(sep2);
			if (sep3.Count() != 2)
				return null;

            return new Tuple<string, string>(sep3[0], sep3[1]);
		}

        public static Tuple<int, int> splitTwoInt(String instr, char sep)
		{
			var x = splitTwo(instr, sep);
            return new Tuple<int, int>(Int32.Parse(x.Item1), Int32.Parse(x.Item2));
		}

        /// <summary>
        /// count how many occurences of a substring occur in a string
        /// </summary>
        /// <param name="instr"></param>
        /// <param name="substring"></param>
        /// <returns></returns>
		public static int ContainsSubStringCount(String instr,String substring)
		{
			var count = 0;

			var c = instr.Length;
			var c1 = substring.Length;
			for (var a = 0; a < c;a++ )
			{
				if (instr.Substring(a,c1).Equals(substring))
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
		public static List<string> splitstrings(String instr, int max)
		{
			var line = "";
			var ls = new List<string>();

			var count = 0;
			var splitchar = new char[] { ' ' };
			var splitspace = instr.Split(splitchar);
			foreach (var s in splitspace)
			{
				if (String.IsNullOrEmpty(s))
					continue;

				if ((count + s.Length) > max)
				{
					ls.Add(padString(line, max));
					line = "";
					count = 0;
				}

				var z = s + " ";
				count += z.Length;
				line += z;
			}
			if (count != 0)
			{
				ls.Add(padString(line, max));
			}
			return ls;
		}

		/// <summary>
		/// Truncate or pad a string with spaces to a certain length
		/// </summary>
		/// <param name="instr"></param>
		/// <param name="maxlen"></param>
		/// <returns></returns>
		public static String padString(String instr, int maxlen)
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
		/// Replace a char in a string with another
		/// </summary>
		/// <param name="origString">The string to change a character in</param>
		/// <param name="newChar">The new character to be used</param>
		/// <param name="position">The position to use the new character</param>
		/// <returns>a string with the character replaced</returns>
		public static String replaceCharAtPosition(String origString, char newChar, int position)
		{
			if (position < 0)
				return origString;

			else if (String.IsNullOrEmpty(origString))
				return null;

			var bef = origString.Substring(0, position);
			var af = origString.Substring(position + 1);
			return bef + newChar.ToString() + af;
		}

		/// <summary>
		/// Replace all instances of a char with another char
		/// </summary>
		/// <param name="origString"></param>
		/// <param name="replaceThis"></param>
		/// <param name="withThis"></param>
		/// <returns></returns>
		public static String replaceAllChars(String origString, char replaceThis, char withThis)
		{
			if (String.IsNullOrEmpty(origString))
				return origString;

			return origString.Replace(replaceThis, withThis);
		}

		public static String replaceAllChars(String origString, String replaceThis, String withThis)
		{
			if (String.IsNullOrEmpty(origString))
				return origString;

			return origString.Replace(replaceThis, withThis);
		}


        /// <summary>
        /// Trim a string of a certain number of chars, either from the start or the end
        /// </summary>
        /// <param name="origString"></param>
        /// <param name="isFront"></param>
        /// <param name="length"></param>
        /// <param name="relativeStart">front=true, start=relativestart. front=end, start=end-relativestart</param>
        /// <returns></returns>
        public static String ApplyTrim(String origString, bool isFront, int length,int relativeStart=0)
		{
            //relative start is bad or length is more than the entire length, then cancel
            if (relativeStart<0||
                (isFront&&(relativeStart+length) > origString.Length)|| 
                (isFront==false && (origString.Length-relativeStart + length) > origString.Length))
				return origString;

			if (isFront==false)
			    relativeStart = origString.Length - relativeStart;
         
            return origString.Remove(relativeStart, length);
		}



		/// <summary>
		/// removes \n \r and \0 from the start and end of a string
		/// </summary>
		/// <param name="origString"></param>
		/// <returns>the 'cleaned' string</returns>
		public static String cleanString(String origString)
		{
			if (String.IsNullOrEmpty(origString))
				return origString;

			char[] bad = { '\n', '\r', '\0',' '};

		retry:
			if (origString.Length == 0)
				return "";
			var start = origString[0];
			var end = origString[origString.Length - 1];

			foreach (var b in bad)
			{
				if (start == b)
				{
					origString = origString.Remove(0, 1);
					goto retry;
				}
				else if (end == b)
				{
					origString = origString.Remove(origString.Length - 1, 1);
					goto retry;
				}
			}

			//remove duplicate whitespace
			var change = true;
			int len;
			while (change)
			{
				len = origString.Length;
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
		public static String addText(String origString, String addText, bool isFront)
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
		/// <returns>the auto capitalised string</returns>
		public static String ToCamelCase(String origString, Boolean capitaliseInitial, List<string> capitaliseWordString=null,bool spaceAfter = true)
		{
			if (origString == null || origString.Length == 0)
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

								if (((a + CWS.Length) > outstr.Length) || (Char.IsWhiteSpace(outstr[a + CWS.Length])) || (capitalAfter.Contains(outstr[a + CWS.Length])))
								{
									for (var a1 = a; a1 < a + CWS.Length; a1++)
									{
										var x1 = outstr[a1];
										x1 = Char.ToUpper(x1);
										outstr = replaceCharAtPosition(outstr, x1, a1);
									}
								}
							}
						}

						//cap if first, or after space (camel case), or bracket
						if ((a == 0 && capitaliseInitial) || Char.IsWhiteSpace(outstr[a - 1]) || capitalAfter.Contains(outstr[a - 1]))
							outstr = replaceCharAtPosition(outstr, Char.ToUpper(x), a);
					}

					//space after comma and close bracket
					if (spaceAfter)
					{
						if (((a + 1) < outstr.Length) &&
						    ((outstr[a] == ',' || closebracket.Contains(outstr[a])) && Char.IsWhiteSpace(outstr[a + 1]) == false))
						{
							outstr = outstr.Insert(a + 1, " ");
						}

						//space before openbracket
						if (((a - 1) > 0) && (openbracket.Contains(outstr[a])) && (Char.IsWhiteSpace(outstr[a - 1]) == false))
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
