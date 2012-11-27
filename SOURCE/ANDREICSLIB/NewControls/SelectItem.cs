using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ANDREICSLIB.NewControls
{
    public class SelectItem
    {
        public string text = "";
        public bool preselected = false;

        public SelectItem(String text, bool isSelected)
        {
            this.text = text;
            preselected = isSelected;
        }
    }
}
