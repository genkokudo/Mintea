using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

// TODO:このあとやること

// ツリーを再帰的にたどって、DOMの順番を出力
// ↑普通にたどったらOK
// 順番通りにDOMを出力する（順番によって連番付ける）
// 基本は"名前=パラメータ"だが、classのようにスペースで区切って複数ある前提で組む
// switch文作っておくとフレキシブルに組みやすい 
//
// できれば、タグに囲まれたinnerTextに対応する（どうやって？）
// こういうパターンがある
// ・<aa><bb></bb></aa>
// ・<aa/>
// ・<aa>aaaa</aa>
// 検出した順番に、元の文字列から取り除いていって見つけるしかなさそう。
// タグの次に一般文字が来てたらそれを得る
// 
// 
// 
// 
// 
// できれば@foreach, @ifに対応する（そんなことできるの？）
// 
// 完成したjQueryを"."で改行、インデントを付ける。

namespace HtmlToDom
{
    public class Trans
    {
        private const char beginTag = '<';
        private const char endTag = '>';

        // TODO:
        /// <summary>
        /// styleタグを作成する
        /// 
        /// style="" は使わないので
        /// 見つけたら自動的に<style></style>作ってあげる
        /// </summary>
        /// <param name="tagInfo"></param>
        /// <returns></returns>
        public static string MakeStyle(TagInfo tagInfo) // count
        {
            // style="margin-right:48px"

            // <style>
            // button1 {
            //     margin-right:48px;
            // }
            // </style>
            return "";
        }

        // TODO:
        /// <summary>
        /// on関係の関数を作成する
        /// 
        /// onなんとか= "" は使わないので
        /// 見つけたら自動的に関数作ってあげる
        /// </summary>
        /// <param name="tagInfo"></param>
        /// <returns></returns>
        public static string MakeFunction(TagInfo tagInfo)
        {
            // onclick="activate(@item.Id)"

            // $(document).on('click', 'a[name=voteIcon]', function () { 
            //     activate(@item.Id)
            // }

            // イベントがあるのにnameがないとかぶっちゃけありえないので、無かったら強制的に付ける
            return "";
        }

        #region SearchAll:設定した開始終了文字列に囲まれた文字列を抽出する
        /// <summary>
        /// 設定した開始終了文字列に囲まれた文字列を抽出する
        /// </summary>
        /// <param name="rawText">入力テキスト</param>
        /// <param name="begin">開始文字列</param>
        /// <param name="end">終了文字列</param>
        /// <returns>抽出した値のリスト（設定した開始終了文字列含む）</returns>
        public static List<string> SearchAll(string rawText, char begin, char end)
        {
            var result = new List<string>();

            // 正規表現で変数をチェック
            //.:任意の文字
            //+:1回以上の繰り返し
            //*:0回以上の繰り返し
            //?: 最小判定（?がなかったら、最も大きな結果を取得する）

            // "<",">"に何か1文字以上入っていることを条件に検索
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

        #endregion

        #region SearchInnerTextAll:全てのinnerText（各タグに囲まれた文字）を抽出する
        /// <summary>
        /// 全てのinnerText（各タグに囲まれた文字）を抽出する
        /// </summary>
        /// <param name="rawText">入力テキスト</param>
        /// <param name="begin">開始文字列</param>
        /// <param name="end">終了文字列</param>
        /// <returns>抽出したinnerTextのリスト</returns>
        public static List<string> SearchInnerTextAll(string rawText, char begin, char end)
        {
            // 改行を消す
            var text = rawText.Replace("\r\n", "\n");
            text = text.Replace("\n", "");

            // innerTextのリスト
            var result = new List<string>();

            // "<",">"に何か1文字以上入っていることを条件に検索
            var terms = begin + "(.+?)" + end;
            // 条件に合った文字列を全部拾う
            var r = new Regex(terms, RegexOptions.Multiline);
            var mc = r.Matches(rawText);

            foreach (var item in mc)
            {
                // innerTextがあっても無くても登録していく
                // 先頭から現在のタグを削除
                var current = item.ToString();
                text = text.Substring(current.Length);

                // "<"があるか
                var index = text.IndexOf(begin);
                if (index >= 0)
                {
                    if (current.Contains("/"))
                    {
                        result.Add(string.Empty);
                    }
                    else
                    {
                        var innerText = text.Substring(0, index);
                        result.Add(innerText.Trim());
                        text = text.Substring(innerText.Length);
                    }
                }
                else
                {
                    result.Add(string.Empty);
                }
            }

            return result;
        }

        #endregion

        #region ParseTags:タグの階層構造を作成する
        /// <summary>
        /// タグの階層構造を作成する
        /// リストのリストを作成して階層構造を示す
        /// </summary>
        /// <param name="rawText"></param>
        public static TreeNode<TagInfo> ParseTags(string rawText)
        {
            // 邪魔なので最初に複数スペースは削除する
            rawText = ReplaceSpaces(rawText);

            // ルート
            var root = new TreeNode<TagInfo>(new TagInfo());

            // 現在編集中のノード
            var currentNode = root;

            // タグごとにバラす
            var tags = SearchAll(rawText, beginTag, endTag);
            // 各タグのinnerText抽出
            var innerTexts = SearchInnerTextAll(rawText, beginTag, endTag);

            // 出てきたタグについて親子関係を作成
            for (int i = 0; i < tags.Count; i++)
            {
                var currentTag = tags[i];
                var currentInnerText = innerTexts[i];

                // "<", ">"を取り除いて、スペースで区切る
                currentTag = currentTag.Trim(beginTag);
                currentTag = currentTag.Trim(endTag);
                var split = currentTag.Split(' ');

                // 0番目がタグ名
                var tagName = split[0];
                if (tagName.StartsWith("/"))
                {
                    //閉じタグ
                    currentNode = currentNode.Parent;
                }
                else
                {
                    // タグ情報作成
                    var tagInfo = new TagInfo(split);

                    // 現在のノードに子登録して深い階層へ
                    var tagTree = new TreeNode<TagInfo>(tagInfo);
                    // innerTextがあれば追加
                    if (!string.IsNullOrWhiteSpace(currentInnerText))
                    {
                        tagInfo.Parameters.Add(new TagParameter(TagParameter.InnerText, currentInnerText));
                    }

                    currentNode.AddChild(tagTree);
                    currentNode = tagTree;

                    // "/"で終わってたら閉じタグ処理
                    if (tagName.EndsWith("/"))
                    {
                        currentNode = currentNode.Parent;
                    }
                }
            }
            if (currentNode.Value.Category == string.Empty)
            {
                Console.WriteLine("閉じタグが一致していない気がします。");
            }
            return root;
        }
        #endregion

        /// <summary>
        /// 1つのタグ情報をjQueryにする
        /// </summary>
        /// <param name="tagInfo">1つのタグ情報</param>
        public static string ToJQueryDom(TagInfo tagInfo)
        {
            var result = string.Empty;
            var count = 0;

            // <tr class="a b"></tr>
            // を
            // const tr1 = $("<tr>")
            //     .addClass("a")
            //     .addClass("b");
            // にする。

            // TODO:順序を確立する

            // TODO:for文を書く
            // constなんとかを書く
            count++;
            if (!string.IsNullOrWhiteSpace(result))
            {
                result = $"{result}\n\n";
            }
            result = $"{result}const {tagInfo.Category}{count} = $(\"<{tagInfo.Category}>\")";

            // パラメータを追加していく
            foreach (var tagParameter in tagInfo.Parameters)
            {
                foreach (var item in tagParameter.Parameters)
                {
                    if (tagParameter.Category.StartsWith("on"))
                    {
                        // onイベントならば、即席で関数を作成して、それを設定する
                        // TODO:関数作成
                        result = $"{result}.addClass(\"{tagParameter.Category}func{count}\")";
                    }
                    else
                    {
                        switch (tagParameter.Category)
                        {
                            case "class":
                                // .addClass("fa-thumbs-up")
                                result = $"{result}.addClass(\"{item}\")";
                                break;
                            case "style":
                                // 即席でcssクラスを作成して、それを設定する
                                // TODO:class作成
                                // .addClass("class1")
                                result = $"{result}.addClass(\"class{count}\")";
                                break;
                            case TagParameter.InnerText:
                                result = $"{result}.text(\"{item}\")";
                                break;
                            default:
                                // .attr("id", "votes-count")
                                result = $"{result}.attr(\"{tagParameter.Category}\", \"{item}\")";
                                break;
                        }
                    }
                }
            }

            // 個々のタグができたら、親子関係に従ってappendする
            // TODO: この後子をappendして、";"を付ける

            // 改行とインデントを付ける
            result = Format(result);
            return result;
        }

        #region ReplaceSpaces:複数スペースを1つのスペースにする
        /// <summary>
        /// 複数スペースを1つのスペースにする
        /// </summary>
        /// <param name="text">対象テキスト</param>
        /// <returns>複数スペースを1つのスペースにしたテキスト</returns>
        public static string ReplaceSpaces(string text)
        {
            var pattern = @"\s\s+";
            var regex = new Regex(pattern);
            return regex.Replace(text, " ");
        }
        #endregion

        #region Format:完成したjQueryを"."で改行
        /// <summary>
        /// 完成したjQueryを"."で改行
        /// インデントを付ける
        /// </summary>
        /// <param name="raw">完成したjQuery</param>
        /// <returns>フォーマット後のjQuery</returns>
        public static string Format(string raw)
        {
            return raw.Replace(".", "\n    .");
        }
        #endregion
    }
}
