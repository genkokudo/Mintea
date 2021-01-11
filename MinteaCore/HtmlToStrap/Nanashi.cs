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

        public async Task Test()
        {
            var formatter = new PrettyMarkupFormatter();    // 整形してHTML出力する
            // -------------------- 基本的な使い方 --------------------
            // まずは、スクレイピングするデータを取得します
            // とりあえずボタンが2つ並んでいて、それぞれ子要素を持っている感じ。
            var sorce = 
                "<div class=\"row\"/>" +
                "<button class=\"navbar-toggler\" type=\"button\" dddd=\"eeee\" data-toggle=\"collapse\" data-target=\"#navbarColor01\" aria-controls=\"navbarColor01\" aria-expanded=\"false\" aria-label=\"Toggle navigation\">" +
                "<span class=\"navbar-toggler-icon\">aaaa</span>" +
                "<span class=\"navbar-toggler-icon\">bbbb</span>" +
                "</button>" + 
                "</div>" +
                "<button class=\"navbar-toggler\" type=\"button\" dddd=\"eeee\" data-toggle=\"collapse\" data-target=\"#navbarColor01\" aria-controls=\"navbarColor01\" aria-expanded=\"false\" aria-label=\"Toggle navigation\">" +
                "<span class=\"navbar-toggler-icon\">1111</span>" +
                "</button>";

            // デフォ設定で新しいコンテキスト作成
            var config = Configuration.Default;
            var context = BrowsingContext.New(config);

            // HTML5をパースする
            var document = await context.OpenAsync(req => req.Content(sorce));

            // 出力してみる
            Console.WriteLine("対象部品をシリアライズ（html,head,bodyタグが付いた状態で出力）します：");
            Console.WriteLine(document.DocumentElement.ToHtml(formatter));

            // タグ内の隣の要素：NextElementSibling
            // 子要素：Children
            // dddd="eeee" の取得方法。
            var btn = document.Body.Children[0];
            var attr = btn.Attributes;
            foreach (var item in attr)
            {
                Console.WriteLine($"{item.Name} {item.Value}"); // これが"dddd"と"eeee"
            }
            // SetAttributeとかで追加削除できる。

            //class="navbar-toggler" はClassList
            //data-toggle="collapse" data-target="#navbarColor01" はDatasetに{ toggle, collapse } のような形で格納される。
            //disabledは、IsDisabledプロパティに入る。
            // "eeee"はAttributesにSpecified = falseとして入る、Valueは""

            // 後は、入力のHTML5に従って親子関係が同じ構造でタグ出力出来たら作りたいものが作れる
            // →元データをrootから辿って片っ端から変換かましていけば良さそうな感じ。


            Console.WriteLine("おわり");

            // じゃあ次は、ソースの階層構造を保ったまま各タグと要素を変換する仕組みを考えます。
            Console.WriteLine("変換処理をやってみよう。");

            // ルールの取得
            var rules = GetRules();

            // ツリー構造に従って、新しいHTMLを作成
            var newHtml = await context.OpenAsync(req => req.Content(""));            
            MakeTree(document.Body, newHtml);

            Console.WriteLine("★生成結果★");
            Console.WriteLine(newHtml.ToHtml(formatter));
        }

        #region 元のHTMLからルールを適用したHTMLを生成する
        private void MakeTree(IHtmlElement element, IDocument doc)
        {
            foreach (var child in element.Children)
            {
                MakeTree(child, doc.Body, doc);
            }
        }
        private void MakeTree(IElement src, IElement html, IDocument doc)
        {
            // element.LocalName: buttonとかdivとか
            // element.InnerHtml: 子要素のHTMLを取得 :<span class="navbar-toggler-icon">aaaa</span>
            // element.OuterHtml: 自分を含むHTMLを取得 :<button class="navbar-toggler" type="button" dddd="eeee" data-toggle="collapse" data-target="#navbarColor01" aria-controls="navbarColor01" aria-expanded="false" aria-label="Toggle navigation"><span class="navbar-toggler-icon">aaaa</span></button>
            // element.TextContent: 下にいくつ子要素があっても、一番下のテキストを表示する。複数子要素がある場合は全部繋げて表示してしまうので注意。子要素がない場合だけ取得するように。:aaaa

            var newElm = doc.CreateElement(src.LocalName); // 要素を作成、強制的に小文字にされる。
            //newElm.TextContent = "ReactStrapのボタン";
            //newElm.SetAttribute("className", "ananan");     // 全部小文字にされてしまう。後で変換かける
            //newElm.SetAttribute("weapon", "sword");
            
            // classを新しい方に移す
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
                MakeTree(child, newElm, doc);
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
                new Rule { SrcTag = "div", SrcTermCategory = "class", SrcTermValue= "row", DestTag = "Row", DestAttr = "" },
                new Rule { SrcTag = "button", SrcTermCategory = "", SrcTermValue= "",  DestTag = "Button", DestAttr = "" }
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
        /// </summary>
        public string SrcTermCategory { get; set; }
        /// <summary>
        /// 条件・値
        /// mt-3 rowなど
        /// </summary>
        public string SrcTermValue { get; set; }

        /// <summary>
        /// 変換先タグ:Rowなど
        /// </summary>
        public string DestTag { get; set; }

        /// <summary>
        /// 付属変換attr
        /// いくつか同じカテゴリごとに溜めてから合成する（複数のRule）
        /// className aaaa と、className bbbbがあったら、className='aaaa bbbb'
        /// </summary>
        public string DestAttr { get; set; }

    }

}
