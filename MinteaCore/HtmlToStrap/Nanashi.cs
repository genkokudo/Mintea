using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MinteaCore.HtmlToStrap
{
    /// <summary>
    /// AngleSharpを思いっきり使い倒す
    /// </summary>
    public class Nanashi
    {
        private readonly string BeginCommentTag = "<comment value=\"";
        private readonly string EndCommentTag = "\"></comment>";

        #region いらん
        public async Task GetDownloadFile()
        {
            // まずは、スクレイピングするデータを取得します
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(@"https://honokak.osaka/bootstrap-ja.html");
            var sorce = await response.Content.ReadAsStringAsync();

            // パースします
            var parser = new HtmlParser();
            var doc = await parser.ParseDocumentAsync(sorce);

            Console.WriteLine(doc.Title);
        }
        #endregion

        public async Task Test(string html)
        {
            var formatter = new PrettyMarkupFormatter();    // 整形してHTML出力する

            // ルールの取得
            var rules = GetRules();

            // コメントはパースされないので退避する
            var commentRule = rules.FirstOrDefault(x => x.SrcTermCategory == Rule.TermCategory.CommentReplacement);
            html = EvacuateComment(commentRule, html);

            // HTML5をパースする
            var document = await ParseHtml(html);

            #region もういらない
            // タグ内の隣の要素：NextElementSibling
            // 子要素：Children
            // dddd="eeee" の取得方法。
            //var btn = document.Body.Children[0];
            //var attr = btn.Attributes;
            //foreach (var item in attr)
            //{
            //    Console.WriteLine($"{item.Name} {item.Value}"); // これが"dddd"と"eeee"
            //}
            // SetAttributeとかで追加削除できる。

            //class="navbar-toggler" はClassList
            //data-toggle="collapse" data-target="#navbarColor01" はDatasetに{ toggle, collapse } のような形で格納される。
            //disabledは、IsDisabledプロパティに入る。
            #endregion

#if DEBUG
            // とりあえず比較のための元HTMLを表示
            var sourceHtml = await MakeTreeAsync(document, new List<Rule>());
            var sourceResult = sourceHtml.ToHtml(formatter);
            Console.WriteLine("------------------------------ 変換前 ------------------------------");
            Console.WriteLine(sourceResult);
#endif

            // ツリー構造に従って、新しいHTMLを作成
            // 変換
            var newHtml = await MakeTreeAsync(document, rules);
            var result = newHtml.ToHtml(formatter);

            // 最後に、ToHtmlしたnewHtmlに対してrulesの単純置換を行う
            // タグごとの分岐とかしない
            foreach (var rule in rules)
            {
                result = rule.Replace(result);
            }

            // 退避したコメントを復帰、その言語のコメントにする
            result = RecoverComment(rules, commentRule, result);
            Console.WriteLine("------------------------------ 変換後 ------------------------------");
            Console.WriteLine(result);


        }

        /// <summary>
        /// EvacuateCommentで退避したコメントを戻し、コメントルールを適用する
        /// </summary>
        /// <param name="rules"></param>
        /// <param name="commentRule"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private string RecoverComment(List<Rule> rules, Rule commentRule, string result)
        {
            if (commentRule != null)
            {
                // 単純置換のせいで拾えなくなってる
                var altBegin = BeginCommentTag;
                var altEnd = EndCommentTag;
                var replaceRules = rules.Where(x => x.SrcTermCategory == Rule.TermCategory.SimpleReplacement);
                foreach (var replaceRule in replaceRules)
                {
                    altBegin = replaceRule.Replace(altBegin);
                    altEnd = replaceRule.Replace(altEnd);
                }

                result = result.Replace(altBegin, commentRule.TargetValue);
                result = result.Replace(altEnd, commentRule.DestValue);
            }

            return result;
        }

        /// <summary>
        /// コメントはパースされないので退避する
        /// </summary>
        /// <param name="commentRule"></param>
        /// <param name="html"></param>
        /// <returns></returns>
        private string EvacuateComment(Rule commentRule, string html)
        {
            var BeginCommentHtml = "<!--";
            var EndCommentHtml = "-->";

            // コメントなどパースされないものを退避する
            // commentタグは存在するが、IE用なので無視して使っちゃってOK
            if (commentRule != null)
            {
                Console.WriteLine("コメントを退避します。");
                html = html.Replace(BeginCommentHtml, BeginCommentTag);
                html = html.Replace(EndCommentHtml, EndCommentTag);
            }
            else
            {
                Console.WriteLine("コメントルールが設定されてないので、コメントは削除します。");
            }

            return html;
        }

        #region 元のHTMLからルールを適用したHTMLを生成する
        /// <summary>
        /// 元のHTMLから、新しいHTMLを作成して
        /// 変換ルールを適用する
        /// </summary>
        /// <param name="srcDocument">元のHTML</param>
        /// <param name="rules">ルール</param>
        private async Task<IDocument> MakeTreeAsync(IDocument srcDocument, List<Rule> rules)
        {
            // 空のHTMLを作成
            var newHtml = await ParseHtml(string.Empty);
            foreach (var child in srcDocument.Body.Children)
            {
                MakeTree(child, newHtml.Body, newHtml, rules);
            }
            return newHtml;
        }

        // 1つのタグに対して、ルールに基づいてオブジェクトの値を書き換える
        /// <summary>
        /// 
        /// </summary>
        /// <param name="src">編集元のタグ</param>
        /// <param name="html">このメソッドで編集する対象のタグ</param>
        /// <param name="newHtml">今回組み立てているHTML、書き換える対象</param>
        /// <param name="rules"></param>
        private void MakeTree(IElement src, IElement html, IDocument newHtml, List<Rule> rules)
        {
            // element.LocalName: buttonとかdivとか
            // element.InnerHtml: 子要素のHTMLを取得 :<span class="navbar-toggler-icon">aaaa</span>
            // element.OuterHtml: 自分を含むHTMLを取得 :<button class="navbar-toggler" type="button" dddd="eeee" data-toggle="collapse" data-target="#navbarColor01" aria-controls="navbarColor01" aria-expanded="false" aria-label="Toggle navigation"><span class="navbar-toggler-icon">aaaa</span></button>
            // element.TextContent: 下にいくつ子要素があっても、一番下のテキストを表示する。複数子要素がある場合は全部繋げて表示してしまうので注意。子要素がない場合だけ取得するように。:aaaa
            //newElm.SetAttribute("className", "ananan");       // 全部小文字にされてしまう。後で変換かける
            //newElm.SetAttribute("weapon", "sword");

            // 要素を作成
            var dest = newHtml.CreateElement(src.LocalName);

            // 新しいタグに古いタグの内容を移す
            // classを新しい方に移す
            var clsList = new List<string>();
            foreach (var cls in src.ClassList)
            {
                clsList.Add(cls);
            }
            dest.ClassList.Add(clsList.ToArray());

            // Attrを新しい方に移す
            foreach (var attr in src.Attributes)
            {
                dest.SetAttribute(attr.Name, attr.Value);
            }

            // 最下層の場合、テキスト要素を移す
            if (src.Children.Length == 0)
            {
                dest.TextContent = src.TextContent;
            }

            // ルールの適用をする:適用対象のルールを集めてから、適用する

            // 対象のルールを集める
            var targetRules = new List<Rule>();
            foreach (var rule in rules)
            {
                if (src.LocalName == rule.TargetTag)       // TODO:条件ってこれだけじゃないはず。後で追加。
                {
                    switch (rule.SrcTermCategory)
                    {
                        case Rule.TermCategory.Class:
                            if (!src.ClassList.Contains(rule.TargetValue))
                            {
                                continue;
                            }
                            break;
                        default:
                            break;
                    }
                    targetRules.Add(rule);
                }
            }

            // ルールを適用する
            foreach (var rule in targetRules)
            {
                switch (rule.SrcTermCategory)
                {
                    case Rule.TermCategory.SimpleReplacement:
                        // 何もなし
                        break;
                    case Rule.TermCategory.RemoveAttr:
                        // 要素を削除
                        dest.RemoveAttribute(rule.TargetValue);
                        break;
                    case Rule.TermCategory.RemoveValue:
                        // 要素の値を条件に削除（Attrの値は必ず1つなので要素を削除すればよい）
                        var targets = dest.Attributes.Where(x => x.Value == rule.TargetValue).ToArray();
                        foreach (var target in targets)
                        {
                            dest.RemoveAttribute(target.Name);
                        }
                        break;
                    case Rule.TermCategory.Class:
                        dest.ClassList.Remove(rule.TargetValue);
                        if (dest.ClassList.Length == 0)
                        {
                            // class=""は表示しない
                            dest.RemoveAttribute("class");
                        }
                        break;
                    default:
                        break;
                }
            }


            // 子要素を追加
            html.AppendChild(dest);

            // 更に子要素を作成する
            foreach (var child in src.Children)
            {
                MakeTree(child, dest, newHtml, rules);
            }
        }
        #endregion

        #region 変換ルールを取得する
        /// <summary>
        /// 変換ルールを得る
        /// </summary>
        private List<Rule> GetRules()
        {
            var result = new List<Rule>
            {
                // コメント退避
                Rule.GetCommentReplacementRule("{/*", "*/}"),

                // とりあえず単純置換
                Rule.GetSimpleReplacementRule("button", "Button"),
                Rule.GetSimpleReplacementRule("label", "Label"),
                Rule.GetSimpleReplacementRule("\"", "'"),
                Rule.GetSimpleReplacementRule("class", "className"),

                // 除去
                Rule.GetRemoveAttrRule("button", "dddd"),
                Rule.GetRemoveAttrByValueRule("button", "#navbarColor01"),
                Rule.GetRemoveAttrByValueRule("button", "navbarColor01"),
                
                // Class
                Rule.GetReplaceTagByClass("div", "row", "Row")
            };

            return result;
        }
        #endregion

        /// <summary>
        /// HTMLをパースしてIDocument形式にする
        /// TODO:Dispose問題が未解決
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private async Task<IDocument> ParseHtml(string html)
        {
            // デフォ設定で新しいコンテキスト作成
            var context = BrowsingContext.New(Configuration.Default);   // 全部終わるまでDisposeしないこと。やったら読み込んだデータが消えます。

            // HTML5をパースする
            return await context.OpenAsync(req => req.Content(html));
        }
    }


}
