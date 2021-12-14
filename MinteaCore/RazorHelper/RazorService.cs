using System;
using System.Collections.Generic;
using System.Text;

namespace MinteaCore.RazorHelper
{
    public interface IRazorService
    {
    }

    public class RazorService : IRazorService
    {
        ///// <summary>
        ///// Razor読み込み用
        ///// Excelに記述しているパスのテキストを読み込む
        ///// </summary>
        ///// <param name="path">cs_asp/crud/ListBox</param>      // TODO:もっと自然な形で。
        ///// <returns></returns>
        //private string GetTemplate(string path)
        //{
        //    // メイン、サブ、ファイルの3つの名前でアクセス
        //    var splitedPath = path.Trim('/').Split("/");
        //    var razorData = _db.RazorFiles.First(x => x.Name == splitedPath[2] && x.Parent.Name == splitedPath[1] && x.Parent.Parent.Name == splitedPath[0]);
        //    var template = string.Empty;
        //    using (var stream = new MemoryStream(razorData.Razor))
        //    {
        //        template = Encoding.UTF8.GetString(stream.ToArray());
        //    }
        //    return template;
        //}
    }
}
