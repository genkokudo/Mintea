using System;
using System.Collections.Generic;
using System.Text;

namespace MinteaCore.HtmlToDom
{
    #region TagInfo：タグ1つ分の情報
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
        public string Name
        {
            get
            {
                return $"{Category}{Id}";
            }
        }

        /// <summary>
        /// タグのパラメータ
        /// 「class="aa bb"」って感じの文字列
        /// </summary>
        public List<TagParameter> Parameters { get; set; } = new List<TagParameter>();

        /// <summary>
        /// ルートかどうか
        /// </summary>
        public bool IsRoot
        {
            get { return Category == null; }
        }

        /// <summary>
        /// 通し番号
        /// </summary>
        public int Id { get; set; }

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
        /// <param name="category">タグの種類, buttonとかaとか</param>
        /// <param name="parameters">タグ内のパラメータ：「class="aa bb"」って感じの文字列</param>
        public TagInfo(string category, List<string> parameters)
        {
            Category = category;

            foreach (var tag in parameters)
            {
                Parameters.Add(new TagParameter(tag));
            }
        }
    }
    #endregion

    #region TagParameter：タグ情報のパラメータ要素
    /// <summary>
    /// タグ情報のパラメータ要素
    /// 「class="aa bb"」って感じの文字列
    /// </summary>
    public class TagParameter
    {
        /// <summary>
        /// タグの種類
        /// innerText
        /// </summary>
        public const string InnerText = "innerText";

        /// <summary>
        /// パラメータの種類
        /// classとか
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// パラメータの内容
        /// 無い（長さ0）場合もある
        /// </summary>
        public List<string> Parameters { get; private set; } = new List<string>();

        /// <summary>
        /// タグ情報のパラメータ要素
        /// 任意のパラメータ（1つのみ）
        /// </summary>
        /// <param name="category">パラメータの種類,classとか</param>
        /// <param name="parameter">パラメータの内容</param>
        public TagParameter(string category, string parameter)
        {
            Category = category;
            Parameters.Add(parameter);
        }

        /// <summary>
        /// タグ情報のパラメータ要素
        /// 「class="aa bb"」って感じの文字列
        /// </summary>
        /// <param name="parameterStr">「class="aa bb"」って感じの文字列</param>
        public TagParameter(string parameterStr)
        {

            if (parameterStr.Contains("data-"))
            {
                var split = parameterStr.Split('-');
                Category = split[0];
                Parameters.Add(split[1]);
            }
            else
            {
                if (parameterStr.Contains("="))
                {
                    var split = parameterStr.Split('=');
                    Category = split[0];
                    if (split.Length > 1)
                    {
                        // セミコロンの付け方が人によって違うと思うのでスペース入れながら分解
                        var tempStr = split[1].Trim('\'');
                        tempStr = tempStr.Replace(";", "; ");
                        tempStr = Trans.ReplaceSpaces(tempStr);
                        var parameters = tempStr.Split(' ');
                        foreach (var item in parameters)
                        {
                            if (!string.IsNullOrWhiteSpace(item))
                            {
                                Parameters.Add(item);
                            }
                        }
                    }
                }
                else
                {
                    Category = parameterStr;
                    Parameters.Add(parameterStr);
                }
            }
        }
    }
    #endregion
}
