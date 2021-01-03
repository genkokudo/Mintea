using AngleSharp;
using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
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
            // -------------------- 基本的な使い方 --------------------
            // まずは、スクレイピングするデータを取得します
            var sorce = "<button class=\"navbar-toggler\" type=\"button\" dddd=\"eeee\" data-toggle=\"collapse\" data-target=\"#navbarColor01\" aria-controls=\"navbarColor01\" aria-expanded=\"false\" aria-label=\"Toggle navigation\">" +
                "<span class=\"navbar-toggler-icon\">aaaa</span>" +
                "</button>";

            //// パースします
            //var parser = new HtmlParser();
            //var doc = await parser.ParseDocumentAsync(sorce);

            //Console.WriteLine("はじめ");

            //// 一部のタグを変換するならば、Bodyに対して行うと良い。FullのHTMLでも一部でも対応できるため。
            //var body = doc.Body;

            ////Console.WriteLine("くぎり");
            ////Console.WriteLine(doc.Source.Text);     // 上のsorceそのまま

            // -------------------- 試してみたい --------------------
            // ドキュメントを作成し、テキストを含む別の段落要素を挿入してツリー構造を変更します。
            // https://anglesharp.github.io/docs/Examples.html#simple-document-manipulation
            // -----------------------------------------------------------

            // デフォ設定で新しいコンテキスト作成
            var config = Configuration.Default;
            var context = BrowsingContext.New(config);

            // HTML5をパースする
            var document = await context.OpenAsync(req => req.Content(sorce));

            // 出力してみる
            Console.WriteLine("対象部品をシリアライズ（html,head,bodyタグが付いた状態で出力）します：");
            Console.WriteLine(document.DocumentElement.OuterHtml);

            // タグを追加して出力してみる
            // "<p>こっちは新しいparagraph要素です。</p>"を、sorceの内容の次（buttonタグの次）に追加する
            Console.WriteLine("他の要素を追加して、もう一度シリアライズします：");
            var p = document.CreateElement("p");
            p.TextContent = "こっちは新しいparagraph要素です。";
            document.Body.AppendChild(p);
            Console.WriteLine(document.DocumentElement.OuterHtml);

            // HTML5に無いタグを追加して出力してみる
            Console.WriteLine("Aaaaというタグを追加してもう一度シリアライズします：");
            var button = document.CreateElement("Aaaa");          // ダメ。大文字にしたいのに小文字にされてしまう。
            button.TextContent = "ReactStrapのボタン";
            button.SetAttribute("className", "ananan");     // ダメ。全部小文字にされてしまう…。後で変換かけるしかないのー？？
            button.SetAttribute("weapon", "sword");
            document.Body.AppendChild(button);
            Console.WriteLine(document.DocumentElement.OuterHtml);

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
        }
    }
}
