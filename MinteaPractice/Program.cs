using Mintea.SnippetGenerator;
using MinteaPractice.Template.T4;
using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MinteaPractice
{
    class Program
    {
        static void Main()
        {
            // RazorEngineを使ったシステムの作成
            // TODO:datファイルを読み込んで、テンプレートとして使用する
            // データは最終的にサーバのどこかに置くので、ここでは適当なデータフォルダ作ってアクセス

            // TODO:Modelデータを作成する

            //OKボタンがクリックされたとき、選択されたファイルを読み取り専用で開く
            Console.WriteLine("--------");
            using (var reader = new StreamReader("Template/Test.dat"))
            {
                string text = reader.ReadToEnd();
                Console.WriteLine(text);

                var model1 = new { Name = "World" };
                var result1 = Engine.Razor.RunCompile(text, "templateKey", null, model1);
                Console.WriteLine(result1);
            }

            Console.WriteLine("--------");

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
