using Mintea.SnippetGenerator;
using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Practice
{

    class Program
    {
        static void Main()
        {
            // TODO:datファイルを読み込んで、テンプレートとして使用する
            // データは最終的にサーバのどこかに置くので、ここでは適当なデータフォルダ作ってアクセス

            // TODO:Modelデータを作成する

            //OKボタンがクリックされたとき、選択されたファイルを読み取り専用で開く
            Console.WriteLine("--------");
            using (var reader = new StreamReader("Template/Test.dat"))
            {
                string text = reader.ReadToEnd();
                Console.WriteLine(text);
            }
            Console.WriteLine("--------");

            var sg = new SnippetGenerator();
            sg.MakeXml();
        }
    }
}
