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

// 欲しい機能
// ・インデント付きで出力

namespace MinteaCore.HtmlToStrap
{
    /// <summary>
    /// AngleSharpを思いっきり使い倒す
    /// </summary>
    public class Nanashi
    {
        private static readonly HttpClient httpClient;

        static Nanashi()
        {
            httpClient = new HttpClient();
        }

        public async Task GetDownloadFile()
        {
            // まずは、スクレイピングするデータを取得します
            var response = await httpClient.GetAsync(@"https://honokak.osaka/bootstrap-ja.html");
            var sorce = await response.Content.ReadAsStringAsync();

            // パースします
            var parser = new HtmlParser();
            var doc = await parser.ParseDocumentAsync(sorce);

            Console.WriteLine(doc.Title);
        }

        /// <summary>
        /// HTMLをパースしてIDocument形式にする
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

        public async Task Test()
        {
            var formatter = new PrettyMarkupFormatter();    // 整形してHTML出力する
            // -------------------- 基本的な使い方 --------------------
            // まずは、スクレイピングするデータを取得します
            // とりあえずボタンが2つ並んでいて、それぞれ子要素を持っている感じ。
            var source = 
                "<div class=\"row\"/>" +
                "<button class=\"navbar-toggler\" type=\"button\" dddd=\"eeee\" data-toggle=\"collapse\" data-target=\"#navbarColor01\" aria-controls=\"navbarColor01\" aria-expanded=\"false\" aria-label=\"Toggle navigation\">" +
                "<span class=\"navbar-toggler-icon\">aaaa</span>" +
                "<span class=\"navbar-toggler-icon\">bbbb</span>" +
                "</button>" + 
                "</div>" +
                "<button class=\"navbar-toggler\" type=\"button\" dddd=\"eeee\" data-toggle=\"collapse\" data-target=\"#navbarColor01\" aria-controls=\"navbarColor01\" aria-expanded=\"false\" aria-label=\"Toggle navigation\">" +
                "<span class=\"navbar-toggler-icon\">1111</span>" +
                "</button>" +
                "<input readonly/>" +
                "<label>" +
                "</label>";

            // HTML5をパースする
            var document = await ParseHtml(source);

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
            // ルールの取得
            var rules = GetRules(); // TODO: 今からここを追加していく

            // 変換
            var newHtml = await MakeTreeAsync(document, rules);
            var result = newHtml.ToHtml(formatter);

            // 最後に、ToHtmlしたnewHtmlに対してrulesの単純置換を行う
            // タグごとの分岐とかしない
            foreach (var rule in rules)
            {
                // TODO: 今からここを追加していく
                switch (rule.SrcTermCategory)
                {
                    case Rule.TermCategory.SimpleReplacement:
                        // 置換処理
                        result = result.Replace(rule.TargetTag, rule.DestValue);
                        break;
                    case Rule.TermCategory.RemoveAttr:
                        // 何もなし
                        break;
                    case Rule.TermCategory.RemoveValue:
                        // 何もなし
                        break;
                    case Rule.TermCategory.Class:
                        result = result.Replace(rule.TargetTag, rule.DestValue);
                        break;
                    default:
                        break;
                }
            }

            Console.WriteLine("------------------------------ 変換後 ------------------------------");
            Console.WriteLine(result);

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
    }

    /// <summary>
    /// 変換ルール
    /// </summary>
    public class Rule
    {
        /// <summary>
        /// 条件・種類
        /// classなど
        /// "SimpleReplacement"の時は、SrcTagとSrcTermValueで単純置換する
        /// </summary>
        public TermCategory SrcTermCategory { get; set; } = TermCategory.SimpleReplacement;

        /// <summary>
        /// 対象のタグ:divなど
        /// </summary>
        public string TargetTag { get; set; } = string.Empty;

        /// <summary>
        /// 条件・値
        /// mt-3 rowなど
        /// 単純置換の時は不使用
        /// </summary>
        public string TargetValue { get; set; } = string.Empty;

        /// <summary>
        /// 変換先の値
        /// 条件に合った部分をこの値に変換する
        /// </summary>
        public string DestValue { get; set; } = string.Empty;

        /// <summary>
        /// 条件の種類
        /// </summary>
        public enum TermCategory
        {
            /// <summary>
            /// 単純置換
            /// この時はSrcTermValueに何も設定しない
            /// SrcTagはタグ名ではなく、置換前の文字列を入れる
            /// </summary>
            SimpleReplacement,

            /// <summary>
            /// 対象の要素を除去する
            /// </summary>
            RemoveAttr,

            /// <summary>
            /// 全ての要素に対して
            /// 要素の値を条件に値を除去する
            /// 要素は指定しない：とにかくこの値は消すってイメージ（今のところ）
            /// </summary>
            RemoveValue,

            /// <summary>
            /// class要素に対する条件
            /// </summary>
            Class,

            // 特定のClassをAttributeに変換する
        }

        #region 条件作成を補助するクラスメソッド
        /// <summary>
        /// 全ての変換後、単純に文字を置換する
        /// </summary>
        /// <param name="src">置換前文字列</param>
        /// <param name="dest">置換後文字列</param>
        /// <returns></returns>
        public static Rule GetSimpleReplacementRule(string src, string dest)
        {
            return new Rule { TargetTag = src, SrcTermCategory = TermCategory.SimpleReplacement, DestValue = dest };
        }

        /// <summary>
        /// 特定の名称のAttrを除去するルールを作成する
        /// </summary>
        /// <param name="targetTag">対象タグ</param>
        /// <param name="targetValue">条件となるAttr名</param>
        /// <returns></returns>
        public static Rule GetRemoveAttrRule(string targetTag, string targetValue)
        {
            return new Rule { TargetTag = targetTag, SrcTermCategory = TermCategory.RemoveAttr, TargetValue = targetValue };
        }

        /// <summary>
        /// 値を持ったAttrを除去するルールを作成する
        /// あまり使わないかも
        /// </summary>
        /// <param name="targetTag">対象タグ</param>
        /// <param name="targetValue">条件となる値</param>
        /// <returns></returns>
        public static Rule GetRemoveAttrByValueRule(string targetTag, string targetValue)
        {
            return new Rule { TargetTag = targetTag, SrcTermCategory = TermCategory.RemoveValue, TargetValue = targetValue };
        }

        /// <summary>
        /// 
        /// クラス値は削除する
        /// </summary>
        /// <param name="targetTag">対象タグ</param>
        /// <param name="targetValue">条件となるClass値</param>
        /// <param name="destTag">どのタグに変換するか</param>
        /// <returns></returns>
        public static Rule GetReplaceTagByClass(string targetTag, string targetValue, string destTag)
        {
            return new Rule { TargetTag = targetTag, SrcTermCategory = TermCategory.Class, TargetValue = targetValue, DestValue = destTag };
        }
        #endregion

        // TODO:どんな変換があるかよく整理して、決め直すこと！
        // ・条件指定してAttrに変換：class="btn btn-danger" を color='danger'にする[class btn-danger attr color danger]

    }

}
