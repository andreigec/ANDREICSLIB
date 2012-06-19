using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ANDREICSLIB
{
    public static class TextboxUpdates
    {
        public class InputType
        {
            public bool AllowChars = false;
            public bool AllowNumbers = false;
            public bool AllowDot = false;
            public bool AllowWhiteSpace = false;

            public static InputType Create(bool AllowChars = false, bool AllowNumbers = false, bool AllowDot = false, bool AllowWhiteSpace = false)
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

        /// <summary>
        /// Quick keyboard handling of fields - Connect to keyboard-keypress event. Pass in KeyChar, and make the return value = e.Handled
        /// </summary>
        /// <param name="IT">the input type</param>
        /// <param name="keyChar">KeyChar</param>
        /// <param name="c">ref to the textbox/combobox for ctrl+a keys, not required</param>
        /// <returns>e.Handled</returns>
        public static bool HandleInput(InputType IT, char keyChar, Control c=null)
        {
         //ctrl+a
            if (c!=null&&keyChar==1)
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
               
                return false;
            }

            if (keyChar <= 31)//control chars
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

    }
}
