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
                        result = result.Replace(rule.SrcTag, rule.DestTag);
                        break;
                    case Rule.TermCategory.RemoveAttr:
                        // 何もなし
                        break;
                    case Rule.TermCategory.Class:
                        result = result.Replace(rule.SrcTag, rule.DestTag);
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
        /// <param name="newHtml">今回組み立てているHTML</param>
        /// <param name="rules"></param>
        private void MakeTree(IElement src, IElement html, IDocument newHtml, List<Rule> rules)
        {
            // element.LocalName: buttonとかdivとか
            // element.InnerHtml: 子要素のHTMLを取得 :<span class="navbar-toggler-icon">aaaa</span>
            // element.OuterHtml: 自分を含むHTMLを取得 :<button class="navbar-toggler" type="button" dddd="eeee" data-toggle="collapse" data-target="#navbarColor01" aria-controls="navbarColor01" aria-expanded="false" aria-label="Toggle navigation"><span class="navbar-toggler-icon">aaaa</span></button>
            // element.TextContent: 下にいくつ子要素があっても、一番下のテキストを表示する。複数子要素がある場合は全部繋げて表示してしまうので注意。子要素がない場合だけ取得するように。:aaaa

            var newElm = newHtml.CreateElement(src.LocalName);      // 要素を作成
                                                                    //newElm.SetAttribute("className", "ananan");       // 全部小文字にされてしまう。後で変換かける
                                                                    //newElm.SetAttribute("weapon", "sword");

            // classを新しい方に移す
            // ルールの適用をする
            foreach (var rule in rules) // TODO:分かんないんだけど、メタデータにしてから組み立ての方がよくない？1個ずつIElementに適用して大丈夫？
            {
                if (src.LocalName == rule.SrcTag)
                {
                    switch (rule.SrcTermCategory)
                    {
                        case Rule.TermCategory.SimpleReplacement:
                            // 何もなし
                            break;
                        case Rule.TermCategory.RemoveAttr:
                            // 要素を削除
                            src.RemoveAttribute(rule.SrcTermValue);
                            break;
                        case Rule.TermCategory.RemoveValue:
                            // 要素の値を削除（要素指定方法不明）
                            break;
                        case Rule.TermCategory.Class:
                            if (src.ClassList.Contains(rule.SrcTermValue))
                            {
                                src.ClassList.Remove(rule.SrcTermValue);
                                if (src.ClassList.Length == 0)
                                {
                                    // class=""は表示しない
                                    src.ClassName = null;
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            var clsList = new List<string>();
            foreach (var cls in src.ClassList)
            {
                clsList.Add(cls);
            }
            newElm.ClassList.Add(clsList.ToArray());

            // Attrを新しい方に移す
            foreach (var attr in src.Attributes)
            {
                newElm.SetAttribute(attr.Name, attr.Value);
            }

            // 最下層の場合、テキスト要素を移す
            if (src.Children.Length == 0)
            {
                newElm.TextContent = src.TextContent;
            }

            // 子要素を追加
            html.AppendChild(newElm);

            // 更に子要素を作成する
            foreach (var child in src.Children)
            {
                MakeTree(child, newElm, newHtml, rules);
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
                //new Rule { SrcTag = "div", SrcTermCategory = Rule.TermCategory.Class, SrcTermValue= "row", DestTag = "Row", DestAttr = "" },
                // とりあえず単純置換
                new Rule { SrcTag = "button", SrcTermCategory = Rule.TermCategory.SimpleReplacement, SrcTermValue= "",  DestTag = "Button" },
                new Rule { SrcTag = "\"", SrcTermCategory = Rule.TermCategory.SimpleReplacement, SrcTermValue= "",  DestTag = "'" },
                // 除去
                new Rule { SrcTag = "button", SrcTermCategory = Rule.TermCategory.RemoveAttr, SrcTermValue= "dddd" },
                new Rule { SrcTag = "button", SrcTermCategory = Rule.TermCategory.RemoveValue, SrcTermValue= "#navbarColor01" },

                // Class
                new Rule { SrcTag = "div", SrcTermCategory = Rule.TermCategory.Class, SrcTermValue= "row",  DestTag = "Row" }
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
        // classで残ったものはclassNameに変えたい。（単純変換で良いと思う）
        // 使用した条件は新しいものから削除：<div class='row aaaa'/> は <Row className='aaaa'/> になる。

        /// <summary>
        /// 対象のタグ:divなど
        /// </summary>
        public string SrcTag { get; set; }

        /// <summary>
        /// 条件・種類
        /// classなど
        /// "SimpleReplacement"の時は、SrcTagとSrcTermValueで単純置換する
        /// </summary>
        public TermCategory SrcTermCategory { get; set; }

        /// <summary>
        /// 条件・値
        /// mt-3 rowなど
        /// </summary>
        public string SrcTermValue { get; set; }

        /// <summary>
        /// 変換先タグ:Rowなど
        /// </summary>
        public string DestTag { get; set; }

        ///// <summary>
        ///// 付属変換attr
        ///// いくつか同じカテゴリごとに溜めてから合成する（複数のRule）
        ///// className aaaa と、className bbbbがあったら、className='aaaa bbbb'
        ///// </summary>
        //public string DestAttr { get; set; }

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
            /// 対象の値を除去する
            /// TODO:どうやって要素を指定するの？SrcTermCategoryかな？
            /// </summary>
            RemoveValue,

            /// <summary>
            /// class要素に対する条件
            /// </summary>
            Class
        }

        // TODO:どんな変換があるかよく整理して、決め直すこと！
        // <button type="button" class="btn btn-danger">Danger!</button>
        // <Button color='danger'>Danger!</Button>
        // ・単純置換：こもじになっちゃったのを大文字に。クォーテーションを変える。
        // ・除去：classの××があれば除去する。[class btn] [type button]
        // ・条件指定してAttrに変換：class="btn btn-danger" を color='danger'にする[class btn-danger attr color danger]

        // TODO:条件の指定方法
        // <h1>Heading <span class="badge badge-secondary">New</span></h1>
        // <h1>Heading <Badge color="secondary">New</Badge></h1>
        // ・タグ変換：条件指定方法は？
        //   spanで、classがbadgeだったら、Badgeタグにする。classのbadgeは除去。
        // ・条件指定してAttrに変換：class="badge-secondary"は、color="secondary"にする。[class badge-secondary attr color secondary]
        //   classとattrではパラメータの数が1個違う。どうやって吸収する？
        //   今のところattrからの変換はなし？ → Collapseとかだったらあるはず。

        // とりあえず、「単純置換」「除去」「classとattrの変換（除去機能付き、全小文字を大文字入りに変換する機能付き）」「classとタグの変換（左記機能付き）」の4種類？
        // 条件指定：タグ、class…だけかも？
    }

}
