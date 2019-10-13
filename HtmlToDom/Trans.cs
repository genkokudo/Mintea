using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HtmlToDom
{
    public class Trans
    {
        const string BeginTag = "<";
        const string EndTag = ">";

        public static List<string> SearchAll(string rawText)
        {
            var result = new List<string>();

            // 正規表現で変数をチェック
            //.:任意の文字
            //+:1回以上の繰り返し
            //*:0回以上の繰り返し
            //?: 最小判定（?がなかったら、最も大きな結果を取得する）

            // 何か1文字以上入っていることを条件に検索
            var terms = BeginTag + "(.+?)" + EndTag;
            // 条件に合った文字列を全部拾う
            var r = new Regex(terms, RegexOptions.Multiline);
            var mc = r.Matches(rawText);

            foreach (var item in mc)
            {
                result.Add(item.ToString());
            }

            return result;
        }
    }
}
