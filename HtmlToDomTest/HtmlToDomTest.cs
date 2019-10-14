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

        [Fact(DisplayName = "1�̕�������΂炷")]
        [Trait("Category", "��芸�����m�F")]
        public void Test1()
        {
            string testData =
            "<tr class=\"collapse\" id=\"dummy@(item.Id)\"><td colspan=\"3\"><div class=\"d-flex justify-content-end\"><button type=\"button\" class=\"btn btn-primary\" style=\"margin-right:48px\" onclick=\"activate(@item.Id)\">�L����</button><button type=\"button\" class=\"btn btn-danger\" style=\"margin-right:48px\" onclick=\"delete(@item.Id)\">�폜</button></div></td></tr>";
            var result = Trans.SearchAll(testData, '<', '>');

            foreach (var item in result)
            {
                output.WriteLine(item);
            }

            result[0].Is("<tr class=\"collapse\" id=\"dummy@(item.Id)\">");
        }

        [Fact(DisplayName = "�K���Ȋm�F")]
        [Trait("Category", "��芸�����m�F")]
        public void Test2()
        {
            string testData =
            "<tr class=\"collapse\" id=\"dummy@(item.Id)\"><td colspan=\"3\"><div class=\"d-flex justify-content-end\"><button type=\"button\" class=\"btn btn-primary\" style=\"margin-right:48px\" onclick=\"activate(@item.Id)\">�L����</button><button type=\"button\" class=\"btn btn-danger\" style=\"margin-right:48px\" onclick=\"delete(@item.Id)\">�폜</button></div></td></tr>";
            var result = Trans.ParseTags(testData);

            result.Value.Is(string.Empty);
            output.WriteLine("Count:" + result.Children.Count);
            result.Children.Count.Is(1);
            output.WriteLine(result.Children[0].Value);
            result.Children[0].Value.Is("tr");

        }
    }
}
