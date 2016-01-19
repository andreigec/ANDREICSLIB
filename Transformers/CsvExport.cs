using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using ANDREICSLIB.ClassExtras;

namespace ANDREICSLIB.Transformers
{

    public class CsvExport
    {
        /// <summary>
        /// To keep the ordered list of column names
        /// </summary>
        List<string> fields = new List<string>();

        /// <summary>
        /// The list of rows
        /// </summary>
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();

        /// <summary>
        /// The current row
        /// </summary>
        Dictionary<string, object> currentRow { get { return rows[rows.Count - 1]; } }

        /// <summary>
        /// Set a value on this column
        /// </summary>
        public object this[string field]
        {
            set
            {
                // Keep track of the field names, because the dictionary loses the ordering
                if (!fields.Contains(field)) fields.Add(field);
                currentRow[field] = value;
            }
        }

        /// <summary>
        /// Call this before setting any fields on a row
        /// </summary>
        public void AddRow()
        {
            rows.Add(new Dictionary<string, object>());
        }

        /// <summary>
        /// Converts a value to how it should output in a csv file
        /// If it has a comma, it needs surrounding with double quotes
        /// Eg Sydney, Australia -> "Sydney, Australia"
        /// Also if it contains any double quotes ("), then they need to be replaced with quad quotes[sic] ("")
        /// Eg "Dangerous Dan" McGrew -> """Dangerous Dan"" McGrew"
        /// </summary>
        string MakeValueCsvFriendly(object value)
        {
            if (value == null) return "";
            if (value is INullable && ((INullable)value).IsNull) return "";
            if (value is DateTime)
            {
                if (((DateTime)value).TimeOfDay.TotalSeconds == 0)
                    return ((DateTime)value).ToString("yyyy-MM-dd");
                return ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss");
            }
            string output = value.ToString();
            if (output.Contains(",") || output.Contains("\""))
                output = '"' + output.Replace("\"", "\"\"") + '"';
            return output;
        }

        /// <summary>
        /// Output all rows as a CSV returning a string
        /// </summary>
        public List<List<string>> Export(bool header)
        {
            var ret = new List<List<string>>();
            var r = new List<string>();

            if (header)
            {
                // The header
                r.AddRange(fields);
                ret.Add(r);
                r = new List<string>();
            }

            // The rows
            foreach (var row in rows)
            {
                //check unique if exists
                r.AddRange(fields.Select(field => MakeValueCsvFriendly(row[field])));
                ret.Add(r);
            }

            return ret;
        }

        /// <summary>
        /// Exports to a file
        /// </summary>
        public void ExportToFile(string path, bool header, int? uniqueColumn)
        {
            try
            {
                var e = Export(header);

                //check unique
                if (uniqueColumn != null)
                {
                    var f = CSVTransform.Load(path);
                    if (f != null)
                    {
                        var exist = f.Select(s => s[(int)uniqueColumn]).ToList();
                        var toadd = e.Select(s => s[(int)uniqueColumn]).ToList();
                        if (exist.Any(s => toadd.Any(s2 => s2 == s)))
                            return;
                    }
                }
                var c = string.Join("\r\n", e.Select(s1 => string.Join(",", s1)));
                FileExtras.SaveToFile(path, c, true);
            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }
}
