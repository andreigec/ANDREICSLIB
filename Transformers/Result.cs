using System.Collections.Generic;

namespace ANDREICSLIB.Transformers
{
    /// <summary>
    /// 
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Gets or sets the error status.
        /// </summary>
        /// <value>
        /// The error status.
        /// </value>
        public string ErrorStatus { get; set; }
        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public List<string> Items { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Result"/> is status.
        /// </summary>
        /// <value>
        ///   <c>true</c> if status; otherwise, <c>false</c>.
        /// </value>
        public bool Status { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Result"/> class.
        /// </summary>
        public Result()
        {
            Items = new List<string>();
            Status = false;
        }
    }
}