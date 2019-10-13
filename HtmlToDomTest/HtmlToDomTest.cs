using System;
using Xunit;
using HtmlToDom;
using ChainingAssertion;
using Xunit.Abstractions;

namespace HtmlToDomTest
{
    public class HtmlToDomTest
    {
        private readonly ITestOutputHelper output;
        public HtmlToDomTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void Test1()
        {
            string testData =
            "<tr class=\"collapse\" id=\"dummy@(item.Id)\"><td colspan=\"3\"><div class=\"d-flex justify-content-end\"><button type=\"button\" class=\"btn btn-primary\" style=\"margin-right:48px\" onclick=\"activate(@item.Id)\">有効化</button><button type=\"button\" class=\"btn btn-danger\" style=\"margin-right:48px\" onclick=\"delete(@item.Id)\">削除</button></div></td></tr>";
            var result = Trans.SearchAll(testData);

            output.WriteLine("テスト開始");
            foreach (var item in result)
            {
                output.WriteLine(item);
            }

            result[0].Is("<tr class=\"collapse\" id=\"dummy@(item.Id)\">");
        }
    }
}
