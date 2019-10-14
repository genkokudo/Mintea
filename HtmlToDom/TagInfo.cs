using System;
using System.Collections.Generic;
using System.Text;

namespace HtmlToDom
{
    /// <summary>
    /// タグ1つ分の情報
    /// </summary>
    public class TagInfo
    {
        /// <summary>
        /// タグの種類
        /// "tr"とかのHTMLタグ内の0番目の要素
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// タグのパラメータ
        /// </summary>
        public List<string> Parameters { get; set; }

        /// <summary>
        /// タグ1つ分の情報
        /// </summary>
        public TagInfo(string name)
        {
            Category = name;
            Parameters = new List<string>();
        }
    }
}
