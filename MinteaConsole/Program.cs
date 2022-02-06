using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using MinteaCore.HtmlToStrap;

// HTMLをReactとかのコードに変換するつもりだったが、やる必要なくなった。
// また変換装置的なものがあったらいじるけど、基本的に要らなくなるはず。
// それよりもソースジェネレータで色々対応する習慣を付けていくことにシフトするべき。

// でもBlazorだったらRazorだしreactよりやりやすくなるので、その時に作ったらいいかも。
// コメントの所がごちゃごちゃしているが、React用の処理なので"{/*","*/}"に置き換えるところは捨てても良い

// やっぱ要らないかなあ…、どのテーマ使うかとか決まってないし。
// もうちょっとAngleSharpとか無しにして、regexによる変換器にすることで汎用的に作った方が良いかも。

namespace MinteaConsole
{
    class Program
    {
        static async Task Main()
        {
            var nanashi = new Nanashi();
            var data = File.ReadAllText(@"../../../abcdefg/sample3.txt");
            await nanashi.Test(data);

            //var a = TreeNode<string>.GetDirectoryFileList(@"C:\Users\ginpay\source\repos\DigitalMegaFlare\DigitalMegaFlare\wwwroot\files\razors");

            //foreach (var item in a.Children)
            //{
            //    Console.WriteLine(item.Value);
            //}


            // RazorEngineを使ったシステムの作成
            // TODO:datファイルを読み込んで、テンプレートとして使用する
            // データは最終的にサーバのどこかに置くので、ここでは適当なデータフォルダ作ってアクセス

            // TODO:Modelデータを作成する

            //OKボタンがクリックされたとき、選択されたファイルを読み取り専用で開く
            //Console.WriteLine("--------");
            //using (var reader = new StreamReader("Template/Test.dat"))
            //{
            //    string text = reader.ReadToEnd();
            //    Console.WriteLine(text);

            //    var model1 = new { Name = "World" };
            //    var result1 = Engine.Razor.RunCompile(text, "templateKey", null, model1);
            //    Console.WriteLine(result1);
            //}

            //Console.WriteLine("--------");

            // --------------------------------------------------
            //// スニペットを生成する
            //var sg = new SnippetGenerator();

            //var imports = new List<string>
            //{
            //    "System",
            //    "System.Collections.Generic"
            //};
            //var declarations = new List<Literal>
            //{
            //    new Literal("Id", "通し番号", "0"),
            //    new Literal("Expression", "ここに列挙体を指定する", "switchOn"),
            //    new Literal("Cases", Function.GenerateSwitchCases, "$expression$"),
            //    new Literal("Name", "名前クラス", "ginpay", Function.ClassName),
            //    new Literal("SystemConsole", Function.SimpleTypeName, "global::System.Console")
            //};
            //var data = new SnippetData
            //{
            //    Imports = imports,
            //    Declarations = declarations
            //};
            //var sw = sg.MakeSnippetXml(data);
            //Console.WriteLine(sw.ToString());
            // --------------------------------------------------

            //// パラメータを作って
            //var context = new GenerationContext
            //{
            //    NamespaceName = "Ananan",
            //    TypeSuffix = "TottemoDaisuki",
            //    RepeatCount = 2
            //};

            //// テキストを生成して
            //var text = new MyCodeGenerator(context).TransformText();

            //Console.WriteLine(text);

            //// UTF8(BOMなし)で出力
            //File.WriteAllText(outputPath, text, new UTF8Encoding(false));

            //Console.WriteLine("Success generate:" + outputPath);
        }
    }
}
