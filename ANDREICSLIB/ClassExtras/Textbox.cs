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
        /// <param name="keyValue">e.Item1Value/KeyChar</param>
        /// <param name="tb">ref to the textbox</param>
        /// <returns>e.Handled</returns>
        public static bool HandleInput(InputType IT, char keyValue, ref TextBox tb)
        {
         //ctrl+a
            if (tb!=null&&keyValue==1)
            {
                tb.SelectionStart = 0;
                tb.SelectionLength = tb.Text.Length;
                return false;
            }

            if (keyValue <= 31)//control chars
                return false;

            bool hit = false;
            
            if ((keyValue >= 65 && keyValue <= 90) || (keyValue >= 97 && keyValue <= 122))
            {
                if (IT.AllowChars == false)
                    return true;
                hit = true;
            }

            if (keyValue >= 48 && keyValue <= 57)
            {
                if (IT.AllowNumbers == false)
                    return true;
                hit = true;
            }

            if (keyValue == '.')
            {
                if (IT.AllowDot == false)
                    return true;
                hit = true;
            }

            if (keyValue == ' ' || keyValue == '\t' || keyValue == '\n')
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
