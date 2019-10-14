using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HtmlToDom
{
    public class Trans
    {
        private const char beginTag = '<';
        private const char endTag = '>';

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
            // ルート
            var root = new TreeNode<TagInfo>(new TagInfo(string.Empty));

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
                    var tagInfo = new TagInfo(tagName);
                    // TODO:残りの要素のリストを持たせる
                    foreach (var tag in split)
                    {
                        tagInfo.Parameters.Add(tag);
                    }
                    tagInfo.Parameters.RemoveAt(0);

                    // 現在のノードに子登録して深い階層へ
                    var tagTree = new TreeNode<TagInfo>(tagInfo);
                    currentNode.AddChild(tagTree);
                    currentNode = tagTree;
                }
            }
            if (currentNode.Value.Category == string.Empty)
            {
                Console.WriteLine("閉じタグが一致していない気がします。");
            }
            return root;
        }
    }
}
