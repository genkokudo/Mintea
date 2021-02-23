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
        const string CulumnTag = "columntag";

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

            // ルールの適用をする:適用対象のルールを集めてから、適用する

            // 対象のルールを集める
            var delClassList = new List<string>();  // 削除するクラス
            var targetRules = new List<Rule>();     // タグに適用するルール
            string replaceTagTo = src.LocalName;    // 新しいタグが元のタグ名と異なる場合に設定する
            foreach (var rule in rules)
            {
                if (src.LocalName == rule.TargetTag)    // まず対象のタグか判定
                {
                    var classList = string.Join(',', src.ClassList).Split(',');
                    switch (rule.SrcTermCategory)
                    {
                        case Rule.TermCategory.IncludeStrClassToTagName:
                            foreach (var singleClass in classList)
                            {
                                var words = singleClass.Split('-');
                                if (words.Contains(rule.TargetValue))
                                {
                                    // 該当ルールが複数あっても1つしか適用できないようにしている
                                    replaceTagTo = rule.TargetValue;
                                    targetRules.Add(rule);
                                    delClassList.Add(singleClass);
                                }
                            }
                            break;
                        case Rule.TermCategory.RemoveClass:
                            if (classList.Contains(rule.TargetValue))
                            {
                                targetRules.Add(rule);
                                delClassList.Add(rule.TargetValue);
                            }
                            break;
                        case Rule.TermCategory.IncludeStrClassToAttr:
                            foreach (var singleClass in classList)
                            {
                                var words = singleClass.Split('-');
                                if (words.Contains(rule.TargetValue))
                                {
                                    targetRules.Add(rule);
                                    delClassList.Add(singleClass);
                                }
                            }
                            break;
                        default:
                            targetRules.Add(rule);
                            break;
                    }
                }
            }

            // 要素を作成
            var dest = CopyTag(src, newHtml, replaceTagTo);

            // ルールを適用する
            foreach (var rule in targetRules)
            {
                switch (rule.SrcTermCategory)
                {
                    case Rule.TermCategory.RemoveAttr:
                        dest.RemoveAttribute(rule.TargetValue);
                        break;
                    case Rule.TermCategory.IncludeStrClassToAttr:
                        // 含むクラスを全て取得してAttrに変換する
                        var targetClasses = dest.ClassList.Where(x => x.Contains(rule.TargetValue)).ToArray();
                        var values = rule.GetValues();
                        foreach (var className in targetClasses)
                        {
                            dest.SetAttribute(values[0], values[1]);
                        }
                        break;
                    case Rule.TermCategory.RemoveClass:
                    case Rule.TermCategory.IncludeStrClassToTagName:
                    case Rule.TermCategory.SimpleReplacement:
                    default:
                        // 何もなし
                        break;
                }
            }

            // 削除対象クラスを削除
            foreach (var del in delClassList)
            {
                dest.ClassList.Remove(del);
            }
            if (dest.ClassList.Length == 0)
            {
                // class=""は表示しない
                dest.RemoveAttribute("class");
            }

            // 子要素を追加
            html.AppendChild(dest);

            // 更に子要素を作成する
            foreach (var child in src.Children)
            {
                MakeTree(child, dest, newHtml, rules);
            }
        }

        /// <summary>
        /// 新しいタグに古いタグの内容を移す
        /// </summary>
        /// <param name="src">古いタグ</param>
        /// <param name="newHtml"></param>
        /// <param name="newTagName">新しいタグ名</param>
        /// <returns></returns>
        private static IElement CopyTag(IElement src, IDocument newHtml, string newTagName = null)
        {
            if (newTagName == "col")
            {
                newTagName = CulumnTag;   // "col"タグを作ろうとするとhtmlの<col>タグと勘違いされて閉じタグが無くなるので別のタグに退避
            }
            var dest = newHtml.CreateElement(newTagName ?? src.LocalName);

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

            return dest;
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
                Rule.GetReplaceTagRule(CulumnTag, "Col"),  // colタグを作ろうとすると<col>と勘違いされるので回避


                // コメント退避
                Rule.GetCommentReplacementRule("{/*", "*/}"),

                // とりあえず単純置換
                Rule.GetSimpleReplacementRule("\"", "'"),
                Rule.GetSimpleReplacementRule("class", "className"),
                Rule.GetSimpleReplacementRule("=''", string.Empty),

                // 除去
                Rule.GetRemoveAttrRule("button", "type"),
                Rule.GetRemoveAttrRule("button", "data-toggle"),
                Rule.GetRemoveAttrRule("button", "data-target"),
                Rule.GetRemoveAttrRule("button", "type"),
                
                // "-"で区切って特定文字列が入っているClassをタグに変換、そのClassは削除
                Rule.GetReplaceTagByClassRule("div", "row", "Row"),
                Rule.GetReplaceTagByClassRule("div", "col", "Col"),
                Rule.GetReplaceTagByClassRule("div", "collapse", "Collapse"),
                Rule.GetReplaceTagByClassRule("div", "container", "Container"),

                // タグを単純に変換する、内部的な仕組みはGetReplaceTagByClassRuleと同じ
                Rule.GetReplaceTagRule("button", "Button"),
                Rule.GetReplaceTagRule("label", "Label"),
                Rule.GetReplaceTagRule("input", "Input"),

                // Class削除
                Rule.GetRemoveClassRule("button", "btn"),
                
                //・Classを"-"で区切って特定文字列が含まれている場合Attrを作成、そのClassは削除
                Rule.GetIncludeClassToAttrRule("button", "outline", "outline", "true"),
                Rule.GetIncludeClassToAttrRule("button", "primary", "color", "primary"),
                Rule.GetIncludeClassToAttrRule("button", "secondary", "color", "secondary"),
                Rule.GetIncludeClassToAttrRule("div", "fluid", "fluid", ""),
                
                // 今の仕様だと"col-auto"ではヒットしない。でも"auto"だけだと他と被るのでは？
                Rule.GetIncludeClassToAttrRule("div", "auto", "xs", "auto"),
                //<div class="col-auto は <Col xs='auto'

                // TODO:"col-sm-8"とか"col-4"みたいなのは？
                // →普通に考えて"col"が含んでいるとき、文字列があればそれをAttrにして、数字が来たらAttrの値にする…なんだけど。
                // "-"で区切って長さが3ならAttrにするとか？
                // クラス名に"col"が入ってたら…っていうのは、汎用性が無くなるから諦めるべき
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
