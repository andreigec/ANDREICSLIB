using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ANDREICSLIB.Helpers;

namespace ANDREICSLIB.NewControls
{
    /// <summary>
    ///     example usage: https://github.com/andreigec/Word-Find-Solver
    /// </summary>
    public partial class MassVariableEdit : Form
    {
        public List<Tuple<string, string>> ReturnValues;
        public List<TextBox> textboxes;

        /// <summary>
        /// Initializes a new instance of the <see cref="MassVariableEdit"/> class.
        /// </summary>
        public MassVariableEdit()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Shows the dialog static.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="formText">The form text.</param>
        /// <param name="classInstance">The class instance.</param>
        /// <param name="overload">The overload.</param>
        /// <returns></returns>
        public static T ShowDialogStatic<T>(string formText, T classInstance,
            List<TextBoxItems> overload = null) where T : class
        {
            var mve = new MassVariableEdit();
            return mve.ShowDialog(formText, classInstance, overload);
        }

        /// <summary>
        /// populate with the variable names from a class. will also copy the instances values as preset text by default
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="formText">The form text.</param>
        /// <param name="classInstance">The class instance.</param>
        /// <param name="overload">The overload.</param>
        /// <returns></returns>
        public T ShowDialog<T>(string formText, T classInstance,
            List<TextBoxItems> overload = null) where T : class
        {
            var ty = classInstance.GetType();
            var fields = Reflection.GetFieldNames(ty);

            var k = new List<TextBoxItems>();
            foreach (var f in fields)
            {
                var ob = Reflection.GetFieldValue(classInstance, f);
                var preval = ob == null ? "" : ob.ToString();
                k.Add(new TextBoxItems(f, preval));
            }

            if (overload != null)
            {
                var ind = 0;
                foreach (var o in overload)
                {
                    var o1 = o;
                    var k1 = k.Where(s => s.TextBoxName.Equals(o1.TextBoxName));
                    if (k1.Count() == 1)
                    {
                        k.Remove(k1.First());
                        k.Insert(ind, o);
                    }
                    else
                    {
                        k.Add(o);
                    }
                    ind++;
                }
            }

            var res = ShowDialog(formText, k);
            if (res == null)
                return null;
            //construct using field names
            var retob = (T) Reflection.DeserialiseObject(ty, res);
            return retob;
        }

        /// <summary>
        /// show a dialog from a list of variable names,variable values,and validation/handlers
        /// </summary>
        /// <param name="formText">The form text.</param>
        /// <param name="items">list of items to add. param name,preset value,keypress validation</param>
        /// <returns></returns>
        public List<Tuple<string, string>> ShowDialog(string formText, List<TextBoxItems> items)
        {
            Text = formText;
            textboxes = new List<TextBox>();
            foreach (var k in items)
            {
                var l = new Label();
                l.Width = Width - 50;
                l.Text = k.LabelText;
                itemspanel.AddControl(l, false);

                var tb = new TextBox();
                tb.Name = k.TextBoxName;
                tb.Text = k.TextBoxPreSetText;
                tb.Tag = k;
                tb.KeyPress += tb_KeyPress;
                textboxes.Add(tb);

                itemspanel.AddControl(tb, false);
            }

            ShowDialog();

            return ReturnValues;
        }

        /// <summary>
        /// Handles the KeyPress event of the tb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyPressEventArgs"/> instance containing the event data.</param>
        private static void tb_KeyPress(object sender, KeyPressEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb == null || tb.Tag == null)
                return;
            var h = (TextBoxItems) tb.Tag;
            if (h.handleKeyPress == null)
                return;

            e.Handled = h.handleKeyPress(e.KeyChar, tb);
        }

        /// <summary>
        /// Handles the Click event of the okbutton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void okbutton_Click(object sender, EventArgs e)
        {
            foreach (var tb in textboxes)
            {
                var h = (TextBoxItems) tb.Tag;
                if (h == null)
                    continue;

                if (h.acceptFinalText != null && h.acceptFinalText(tb.Text) == false)
                {
                    MessageBox.Show("Error: " + h.FinalTextError);
                    return;
                }
            }

            ReturnValues = textboxes.Select(tb => new Tuple<string, string>(tb.Name, tb.Text)).ToList();
            Close();
        }

        /// <summary>
        /// Handles the Click event of the cancelbutton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void cancelbutton_Click(object sender, EventArgs e)
        {
            Close();
        }

        #region Nested type: TextBoxItems

        /// <summary>
        /// 
        /// </summary>
        public class TextBoxItems
        {
            public AcceptFinalTextBoxText acceptFinalText;
            public string FinalTextError;
            public HandleKeyPress handleKeyPress;
            public string LabelText;
            public string TextBoxName;
            public string TextBoxPreSetText;

            /// <summary>
            /// Initializes a new instance of the <see cref="TextBoxItems"/> class.
            /// </summary>
            /// <param name="fieldname">The fieldname.</param>
            /// <param name="presetvalue">The presetvalue.</param>
            /// <param name="handleKeyPressH">The handle key press h.</param>
            /// <param name="acceptFinalTextBoxTextH">The accept final text box text h.</param>
            /// <param name="errortext">The errortext.</param>
            public TextBoxItems(string fieldname, string presetvalue = "",
                HandleKeyPress handleKeyPressH = null,
                AcceptFinalTextBoxText acceptFinalTextBoxTextH = null,
                string errortext = "")
            {
                acceptFinalText = acceptFinalTextBoxTextH;
                FinalTextError = errortext;
                handleKeyPress = handleKeyPressH;
                LabelText = fieldname;
                TextBoxName = fieldname;
                TextBoxPreSetText = presetvalue;
            }
        }

        #endregion

        #region Delegates

        /// <summary>
        ///     will get called to do final validations on textbox text before accepting
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public delegate bool AcceptFinalTextBoxText(string s);

        /// <summary>
        ///     will get called on textbox.keypress if set
        /// </summary>
        /// <param name="keyChar"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public delegate bool HandleKeyPress(char keyChar, Control c);

        #endregion
    }
}