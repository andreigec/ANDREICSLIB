using System;
using System.Windows.Forms;

namespace ANDREICSLIB.ClassExtras
{
    /// <summary>
    ///     example usage: https://github.com/andreigec/Squarification
    /// </summary>
    public static class TextboxExtras
    {
        /// <summary>
        /// Scrolls to end.
        /// </summary>
        /// <param name="tb">The tb.</param>
        public static void ScrollToEnd(TextBox tb)
        {
            tb.SelectionStart = tb.TextLength;
            tb.ScrollToCaret();
        }

        /// <summary>
        /// Gets the selection.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns></returns>
        private static Tuple<int, int> GetSelection(Control c)
        {
            var start = 0;
            var length = 0;
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

        /// <summary>
        /// Gets the future text box after key press.
        /// </summary>
        /// <param name="keyChar">The key character.</param>
        /// <param name="c">The c.</param>
        /// <returns></returns>
        public static string GetFutureTextBoxAfterKeyPress(char keyChar, Control c)
        {
            var t = c.Text;

            var l = GetSelection(c);

            if (l.Item2 > 0)
                t = StringExtras.ApplyTrim(t, true, l.Item2, l.Item1);

            t = t.Insert(l.Item1, keyChar.ToString());
            return t;
        }

        /// <summary>
        /// quick handle for floats - Connect to keyboard-keypress event. Pass in KeyChar, and make the return value =
        /// e.Handled
        /// </summary>
        /// <param name="keyChar">The key character.</param>
        /// <param name="c">e.KeyChar</param>
        /// <returns>
        /// e.Handled
        /// </returns>
        public static bool HandleInputAsFloat(char keyChar, Control c)
        {
            if (IsSpecial(keyChar, c))
                return false;

            var t = GetFutureTextBoxAfterKeyPress(keyChar, c);
            float r;

            //since we are eventually passing back to event.ishandle, we need the inverse
            return !float.TryParse(t, out r);
        }

        /// <summary>
        /// Determines whether the specified key character is special.
        /// </summary>
        /// <param name="keyChar">The key character.</param>
        /// <param name="c">The c.</param>
        /// <returns></returns>
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
        /// Quick keyboard handling of fields - Connect to keyboard-keypress event. Pass in KeyChar, and make the return value
        /// = e.Handled
        /// </summary>
        /// <param name="IT">the input type</param>
        /// <param name="keyChar">e.KeyChar</param>
        /// <param name="c">ref to the textbox/combobox for ctrl+a keys, not required</param>
        /// <returns>
        /// e.Handled
        /// </returns>
        public static bool HandleInput(InputType IT, char keyChar, Control c = null)
        {
            if (IsSpecial(keyChar, c))
                return false;

            var hit = false;

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

        /// <summary>
        /// 
        /// </summary>
        public class InputType
        {
            internal bool AllowChars;
            internal bool AllowDot;
            internal bool AllowNumbers;
            internal bool AllowWhiteSpace;

            /// <summary>
            /// Creates the specified allow chars.
            /// </summary>
            /// <param name="AllowChars">if set to <c>true</c> [allow chars].</param>
            /// <param name="AllowNumbers">if set to <c>true</c> [allow numbers].</param>
            /// <param name="AllowDot">if set to <c>true</c> [allow dot].</param>
            /// <param name="AllowWhiteSpace">if set to <c>true</c> [allow white space].</param>
            /// <returns></returns>
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
            /// <returns></returns>
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