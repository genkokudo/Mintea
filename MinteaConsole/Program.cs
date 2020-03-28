using Inflector;
using System;
using System.Globalization;

namespace MinteaConsole
{
    class Program
    {
        static void Main()
        {
            var inf = new Inflector.Inflector(new CultureInfo("en-US"));
            Console.WriteLine(inf.Pluralize("cost"));
            Console.WriteLine(inf.Pluralize("sword"));
            Console.WriteLine(inf.Pluralize("water"));
            Console.WriteLine(inf.Pluralize("data"));
            Console.WriteLine(inf.Pluralize("child"));
            Console.WriteLine(inf.Pluralize("apple"));
            Console.WriteLine(inf.Pluralize("Apple"));
            Console.WriteLine(inf.Pluralize("History"));
            Console.WriteLine(inf.Pluralize("JapanHistory"));
            Console.WriteLine(inf.Pluralize("FlowerChild"));
            Console.WriteLine("---------------------------------");
            Console.WriteLine(inf.Singularize("oranges"));
            Console.WriteLine(inf.Singularize("Lemons"));
            Console.WriteLine(inf.Singularize("templates"));
            Console.WriteLine(inf.Singularize("children"));
            Console.WriteLine(inf.Singularize("boxes"));
            Console.WriteLine("---------------------------------");
            Console.WriteLine(inf.Pascalize("tryCatchPrecure"));    // できる
            Console.WriteLine(inf.Pascalize("try-catch-precure"));  // できない
            Console.WriteLine(inf.Pascalize("try catch precure"));  // できない
            Console.WriteLine(inf.Camelize("TryCatchPrecure"));     // できる
            Console.WriteLine(inf.Camelize("try_catch_precure"));   // できる
            Console.WriteLine(inf.Dasherize("TryCatchPrecure"));    // TryCatchPrecure  // 未実装っぽい
            Console.WriteLine(inf.Humanize("TryCatchPrecure"));     // Trycatchprecure  // よく分からん変換
            Console.WriteLine(inf.Humanize("try_catch_precure"));   // Try catch precure
            Console.WriteLine(inf.Ordinalize(3));                   // 1st 2nd...
            Console.WriteLine(inf.Ordinalize(54321));               // 54321st
            Console.WriteLine(inf.Titleize("try-catch-precure"));   // Try Catch Precure
            Console.WriteLine(inf.Titleize("try_catch_precure"));   // Try Catch Precure
            Console.WriteLine(inf.Titleize("TryCatchPrecure"));     // Try Catch Precure // 大文字とスペース区切りにする
            Console.WriteLine(inf.Uncapitalize("TryCatchPrecure")); // tryCatchPrecure // わからん
            Console.WriteLine(inf.Underscore("TryCatchPrecure"));   // try_catch_precure // snake caseにする


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
