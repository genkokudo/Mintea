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
        /// 無い場合（Rootの場合）はnull
        /// </summary>
        public string Category
        {
            get;
            private set;
        }

        /// <summary>
        /// タグのパラメータ
        /// 「class="aa bb"」って感じの文字列
        /// </summary>
        public List<string> Parameters { get; set; } = new List<string>();

        /// <summary>
        /// ルートかどうか
        /// </summary>
        public bool IsRoot
        {
            get { return Category == null; }
        }

        /// <summary>
        /// タグ1つ分の情報
        /// 引数なしはRoot
        /// </summary>
        public TagInfo()
        {
            // なにもしない
        }

        /// <summary>
        /// タグ1つ分の情報
        /// </summary>
        /// <param name="parameters">タグ内のパラメータ：タグの括弧内をスペースで区切った配列</param>
        public TagInfo(string[] parameters)
        {
            // 残りの要素のリストを持たせる
            foreach (var tag in parameters)
            {
                Parameters.Add(tag);
            }
            Category = Parameters[0];
            Parameters.RemoveAt(0);
        }
    }
}
