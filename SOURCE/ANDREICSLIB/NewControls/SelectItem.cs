using System;

namespace ANDREICSLIB.NewControls
{
    public class SelectItem
    {
        public bool preselected = false;
        public string text = "";

        public SelectItem(String text, bool isSelected)
        {
            this.text = text;
            preselected = isSelected;
        }
    }
}