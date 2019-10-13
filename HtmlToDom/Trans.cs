using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HtmlToDom
{
    public class Trans
    {
        /// <summary>
        /// 設定した開始終了文字列に囲まれた文字列を抽出する
        /// </summary>
        /// <param name="rawText">入力テキスト</param>
        /// <param name="begin">開始文字列</param>
        /// <param name="end">終了文字列</param>
        /// <returns>抽出した値のリスト（設定した開始終了文字列含む）</returns>
        public static List<string> SearchAll(string rawText, string begin, string end)
        {
            var result = new List<string>();

            // 正規表現で変数をチェック
            //.:任意の文字
            //+:1回以上の繰り返し
            //*:0回以上の繰り返し
            //?: 最小判定（?がなかったら、最も大きな結果を取得する）

            // 何か1文字以上入っていることを条件に検索
            var terms = begin + "(.+?)" + end;
            // 条件に合った文字列を全部拾う
            var r = new Regex(terms, RegexOptions.Multiline);
            var mc = r.Matches(rawText);

            foreach (var item in mc)
            {
                result.Add(item.ToString());
            }

            return result;
        }

        /// <summary>
        /// タグの階層構造を作成する
        /// </summary>
        /// <param name="rawText"></param>
        public static void ParceTags(string rawText)
        {
            var tags = SearchAll(rawText, "<", ">");
        }
    }
}
