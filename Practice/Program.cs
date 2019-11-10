using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Practice
{
    /// <summary>
    /// 日報を生成する
    /// </summary>
    class DeilyReportGenerator
    {
        /// <summary>
        /// 現在処理中のデータ
        /// </summary>
        public DeilyReportData CurrentData { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public void AddData()
        {

        }
    }

    /// <summary>
    /// 日報データ
    /// </summary>
    class DeilyReportData
    {
        /// <summary>
        /// 昨日の日報
        /// </summary>
        public List<DeilyReportSubData> YesterdaySubjects { get; set; }
        /// <summary>
        /// 今日の日報
        /// </summary>
        public List<DeilyReportSubData> TodaySubjects { get; set; }
        
        /// <summary>
        /// 現在のデータを入力する
        /// これを昨日のデータにして編集を再開する
        /// </summary>
        public void ContinueData()
        {

        }
    }
    /// <summary>
    /// 1つの案件データ
    /// </summary>
    class DeilyReportSubData
    {
        /// <summary>
        /// 案件名
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> Tasks { get; set; }
    }

    class Program
    {
        static void Main()
        {
            //// @に代入する
            //string template = "Hello @Model.Name, welcome to RazorEngine!";
            //var model1 = new { Name = "World" };
            //var result1 = Engine.Razor.RunCompile(template, "templateKey", null, model1);
            //Console.WriteLine(result1);

            //// キーで登録したテンプレートを呼び出して適用する
            //var model2 = new { Name = "Max" };
            //var result2 = Engine.Razor.Run("templateKey", null, model2);
            //Console.WriteLine(result2);

            //// 型を指定して代入する
            //var result3 = Engine.Razor.RunCompile("templateKey", typeof(Person), new Person { Name = "Max" });
            //Console.WriteLine(result3);

            // じゃあ、何かやってみよう。
            // 日報を作る

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
            using (var reader = new StreamReader("Template/DailyBase.dat"))
            {
                string text = reader.ReadToEnd();
                Console.WriteLine(text);
            }
            Console.WriteLine("--------");


        }
    }
}
