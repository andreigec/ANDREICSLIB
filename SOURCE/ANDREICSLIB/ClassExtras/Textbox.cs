using System;
using System.Windows.Forms;

namespace ANDREICSLIB.ClassExtras
{
    public static class TextboxExtras
    {

	    public static void ScrollToEnd(TextBox tb)
	    {
			tb.SelectionStart = tb.TextLength;
			tb.ScrollToCaret();
	    }
        private static Tuple<int, int> GetSelection(Control c)
        {
            int start = 0;
            int length = 0;
            if (c is TextBox)
            {
                var tb = c as TextBox;
                start = tb.SelectionStart;
                length = tb.SelectionLength;
            }
            else if (c is ComboBox)
            {
                var cb = c as ComboBox;
                start = cb.SelectionStart;
                length = cb.SelectionLength;
            }

            return new Tuple<int, int>(start, length);
        }

        public static string GetFutureTextBoxAfterKeyPress(char keyChar, Control c)
        {
            string t = c.Text;

            Tuple<int, int> l = GetSelection(c);

            if (l.Item2 > 0)
                t = StringExtras.ApplyTrim(t, true, l.Item2, l.Item1);

            t = t.Insert(l.Item1, keyChar.ToString());
            return t;
        }

        /// <summary>
        /// quick handle for floats - Connect to keyboard-keypress event. Pass in KeyChar, and make the return value = e.Handled
        /// </summary>
        /// <param name="c">e.KeyChar</param>
        /// <returns>e.Handled</returns>
        public static bool HandleInputAsFloat(char keyChar, Control c)
        {
            if (IsSpecial(keyChar, c))
                return false;

            string t = GetFutureTextBoxAfterKeyPress(keyChar, c);
            float r;

            //since we are eventually passing back to event.ishandle, we need the inverse
            return !float.TryParse(t, out r);
        }

        public static bool IsSpecial(char keyChar, Control c = null)
        {
            //ctrl+a
            if (c != null && keyChar == 1)
            {
                if (c is TextBox)
                {
                    var tb = c as TextBox;
                    tb.SelectionStart = 0;
                    tb.SelectionLength = tb.Text.Length;
                }

                else if (c is ComboBox)
                {
                    var cb = c as ComboBox;
                    cb.SelectionStart = 0;
                    cb.SelectionLength = cb.Text.Length;
                }

                return true;
            }

            if (keyChar <= 31) //control chars
                return true;

            return false;
        }

        /// <summary>
        /// Quick keyboard handling of fields - Connect to keyboard-keypress event. Pass in KeyChar, and make the return value = e.Handled
        /// </summary>
        /// <param name="IT">the input type</param>
        /// <param name="keyChar">e.KeyChar</param>
        /// <param name="c">ref to the textbox/combobox for ctrl+a keys, not required</param>
        /// <returns>e.Handled</returns>
        public static bool HandleInput(InputType IT, char keyChar, Control c = null)
        {
            if (IsSpecial(keyChar, c))
                return false;

            bool hit = false;

            if ((keyChar >= 65 && keyChar <= 90) || (keyChar >= 97 && keyChar <= 122))
            {
                if (IT.AllowChars == false)
                    return true;
                hit = true;
            }

            if (keyChar >= 48 && keyChar <= 57)
            {
                if (IT.AllowNumbers == false)
                    return true;
                hit = true;
            }

            if (keyChar == '.')
            {
                if (IT.AllowDot == false)
                    return true;
                hit = true;
            }

            if (keyChar == ' ' || keyChar == '\t' || keyChar == '\n')
            {
                if (IT.AllowWhiteSpace == false)
                    return true;
                hit = true;
            }

            //wasnt caught by any, so handle
            return (hit == false);
        }

        #region Nested type: InputType

        public class InputType
        {
            public bool AllowChars = false;
            public bool AllowDot = false;
            public bool AllowNumbers = false;
            public bool AllowWhiteSpace = false;

            public static InputType Create(bool AllowChars = false, bool AllowNumbers = false, bool AllowDot = false,
                                           bool AllowWhiteSpace = false)
            {
                var it = new InputType();
                it.AllowChars = AllowChars;
                it.AllowNumbers = AllowNumbers;
                it.AllowDot = AllowDot;
                it.AllowWhiteSpace = AllowWhiteSpace;
                return it;
            }

            /// <summary>
            /// set all types to input param
            /// </summary>
            /// <param name="allType"></param>
            public static InputType CreateAllTrue()
            {
                var it = new InputType();
                it.AllowChars = it.AllowNumbers = it.AllowDot = it.AllowWhiteSpace = true;
                return it;
            }
        }

        #endregion
    }
}