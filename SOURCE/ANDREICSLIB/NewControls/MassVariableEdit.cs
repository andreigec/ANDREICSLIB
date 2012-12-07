using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ANDREICSLIB.NewControls
{
    public partial class MassVariableEdit : Form
    {
        #region Delegates

        /// <summary>
        /// will get called to do final validations on textbox text before accepting 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public delegate bool AcceptFinalTextBoxText(String s);

        /// <summary>
        /// will get called on textbox.keypress if set
        /// </summary>
        /// <param name="keyChar"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public delegate bool HandleKeyPress(char keyChar, Control c);

        #endregion

        public List<Tuple<String, String>> ReturnValues;
        public List<TextBox> textboxes;

        public MassVariableEdit()
        {
            InitializeComponent();
        }

        public static List<Tuple<String, String>> ShowDialogStatic(String FormText, object classInstance,
                                                                   List<TextBoxItems> overload = null)
        {
            var mve = new MassVariableEdit();
            return mve.ShowDialog(FormText, classInstance, overload);
        }

        /// <summary>
        /// populate with the variable names from a class. will also copy the instances values as preset text by default
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public List<Tuple<String, String>> ShowDialog(String FormText, object classInstance,
                                                      List<TextBoxItems> overload = null)
        {
            Type ty = classInstance.GetType();
            List<string> fields = Reflection.GetFieldNames(ty);

            var k = new List<TextBoxItems>();
            foreach (string f in fields)
            {
                object ob = Reflection.GetFieldValue(classInstance, f);
                string preval = ob == null ? "" : ob.ToString();
                k.Add(TextBoxItems.Create(f, preval));
            }

            if (overload != null)
            {
                int ind = 0;
                foreach (TextBoxItems o in overload)
                {
                    TextBoxItems o1 = o;
                    IEnumerable<TextBoxItems> k1 = k.Where(s => s.TextBoxName.Equals(o1.TextBoxName));
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

            return ShowDialog(FormText, k);
        }

        /// <summary>
        /// show a dialog from a list of variable names,variable values,and validation/handlers
        /// </summary>
        /// <param name="items">list of items to add. param name,preset value,keypress validation</param>
        /// <returns></returns>
        public static List<Tuple<String, String>> ShowDialog(String FormText, List<TextBoxItems> items)
        {
            var mve = new MassVariableEdit();
            mve.Text = FormText;
            mve.textboxes = new List<TextBox>();
            foreach (TextBoxItems k in items)
            {
                var l = new Label();
                l.Text = k.LabelText;
                mve.itemspanel.addControl(l, true);

                var tb = new TextBox();
                tb.Name = k.TextBoxName;
                tb.Text = k.TextBoxPreSetText;
                tb.Tag = k;
                tb.KeyPress += tb_KeyPress;
                mve.textboxes.Add(tb);

                mve.itemspanel.addControl(tb, false);
            }

            mve.ShowDialog();

            return mve.ReturnValues;
        }

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

        private void okbutton_Click(object sender, EventArgs e)
        {
            foreach (TextBox tb in textboxes)
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

        private void cancelbutton_Click(object sender, EventArgs e)
        {
            Close();
        }

        #region Nested type: TextBoxItems

        public class TextBoxItems
        {
            /// <summary>
            /// will get shown in a dialog if the acceptfinaltext delegate fails
            /// </summary>
            public string FinalTextError;

            /// <summary>
            /// label text before text box
            /// </summary>
            public string LabelText;

            /// <summary>
            /// must be unique
            /// </summary>
            public string TextBoxName;

            /// <summary>
            /// pre set text box test, empty by default
            /// </summary>
            public string TextBoxPreSetText;

            public AcceptFinalTextBoxText acceptFinalText;
            public HandleKeyPress handleKeyPress;

            public static TextBoxItems Create(String fieldname, string presetvalue = "",
                                              HandleKeyPress handleKeyPressH = null,
                                              AcceptFinalTextBoxText acceptFinalTextBoxTextH = null,
                                              string errortext = "")
            {
                return new TextBoxItems
                           {
                               acceptFinalText = acceptFinalTextBoxTextH,
                               FinalTextError = errortext,
                               handleKeyPress = handleKeyPressH,
                               LabelText = fieldname,
                               TextBoxName = fieldname,
                               TextBoxPreSetText = presetvalue
                           };
            }
        }

        #endregion
    }
}