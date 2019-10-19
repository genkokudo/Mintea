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
//
// できれば@foreach, @ifに対応する（そんなことできるの？）
// 

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

            // 出てきたタグについて親子関係を作成
            foreach (var item in tags)
            {
                var current = item;
                // "<", ">"を取り除いて、スペースで区切る
                current = current.Trim(beginTag);
                current = current.Trim(endTag);
                var split = current.Split(' ');

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

        /// <summary>
        /// 1つのタグ情報をjQueryにする
        /// </summary>
        /// <param name="tagInfo">1つのタグ情報</param>
        public static string ToJQueryDom(TagInfo tagInfo)
        {
            // <tr class="a b"></tr>
            // を
            // const tr1 = $("<tr>")
            //     .addClass("a")
            //     .addClass("b");
            // にする。

            // <a id="a" name="b"></a>
            // を
            // const a1 = $("<a>")
            //     .attr("id", "a") 
            //     .attr("name", "b");
            // にする。

            switch (tagInfo.Category)
            {
                case "":
                    break;
                default:
                    break;
            }

            return "";
        }

        // 個々のタグができたら、親子関係に従ってappendする

        // innerText検出する
        // <aa>ここを検出</aa>

        #region 複数スペースを1つのスペースにする
        /// <summary>
        /// 複数スペースを1つのスペースにする
        /// </summary>
        /// <param name="text">対象テキスト</param>
        /// <returns>複数スペースを1つのスペースにしたテキスト</returns>
        private static string ReplaceSpaces(string text)
        {
            var pattern = @"\s\s+";
            var regex = new Regex(pattern);
            return regex.Replace(text, " ");
        }
        #endregion

    }
}
