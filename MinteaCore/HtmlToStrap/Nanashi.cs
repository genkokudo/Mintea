using AngleSharp;
using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
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
            // -------------------- 基本的な使い方 --------------------
            //// まずは、スクレイピングするデータを取得します
            //var sorce = "<button class=\"navbar-toggler\" type=\"button\" dddd=\"eeee\" data-toggle=\"collapse\" data-target=\"#navbarColor01\" aria-controls=\"navbarColor01\" aria-expanded=\"false\" aria-label=\"Toggle navigation\">" +
            //    "<span class=\"navbar-toggler-icon\">aaaa</span>" +
            //    "</button>";

            //// パースします
            //var parser = new HtmlParser();
            //var doc = await parser.ParseDocumentAsync(sorce);

            //Console.WriteLine("はじめ");

            //// 一部のタグを変換するならば、Bodyに対して行うと良い。FullのHTMLでも一部でも対応できるため。
            //var body = doc.Body;

            ////Console.WriteLine("くぎり");
            ////Console.WriteLine(doc.Source.Text);     // 上のsorceそのまま

            // -------------------- 試してみたい --------------------
            // https://anglesharp.github.io/docs/Examples.html#simple-document-manipulation
            // -----------------------------------------------------------

            // デフォ設定で新しいコンテキスト作成
            var config = Configuration.Default;

            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(req => req.Content("<button disabled class=\"navbar-toggler\" eeee bbbb=\"cccc\" type=\"button\" data-toggle=\"collapse\" data-target=\"#navbarColor01\" aria-controls=\"navbarColor01\" aria-expanded=\"false\" aria-label=\"Toggle navigation\">" +
                "<span class=\"navbar-toggler-icon\">aaaa</span>"));
            // <html><head></head><body><h1>サンプルソース</h1><p>これはparagraph要素</p></body></html>

            Console.WriteLine("シリアライズします：");
            Console.WriteLine(document.DocumentElement.OuterHtml);

            var p = document.CreateElement("p");
            p.TextContent = "こっちは新しいparagraph要素です。";

            Console.WriteLine("他の要素を追加します。");
            document.Body.AppendChild(p);

            Console.WriteLine("もう一度シリアライズします：");
            Console.WriteLine(document.DocumentElement.OuterHtml);
            // <html><head></head><body><h1>サンプルソース</h1><p>これはparagraph要素</p><p>こっちは新しいparagraph要素です。</p></body></html>

            var button = document.CreateElement("Aaaa");          // ダメ。大文字にしたいのに小文字にされてしまう。設定で直る？
            button.TextContent = "ReactStrapのボタン";

            Console.WriteLine("他の要素を追加します。");
            document.Body.AppendChild(button);

            Console.WriteLine("もう一度シリアライズします：");
            Console.WriteLine(document.DocumentElement.OuterHtml);

            // タグ内の隣の要素：NextElementSibling
            // 子要素：Children
            // bbbb="cccc" の取得方法。
            var btn = document.Body.Children[0];
            var attr = btn.Attributes;
            foreach (var item in attr)
            {
                Console.WriteLine($"{item.Name} {item.Value}");
            }
            // SetAttributeとかで追加削除できる。

            //class="navbar-toggler" はClassList
            //data-toggle="collapse" data-target="#navbarColor01" はDatasetに{ toggle, collapse } のような形で格納される。
            //disabledは、IsDisabledプロパティに入る。
            // "eeee"はAttributesにSpecified = falseとして入る、Valueは""

            // とりあえず、今回の用途ではタグ名を正しく出力出来ればOKかも。


            Console.WriteLine("おわり");
        }
    }
}
