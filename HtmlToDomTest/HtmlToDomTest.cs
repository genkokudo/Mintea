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

        [Fact(DisplayName = "���K���Ȗ؍\���̍쐬�m�F")]
        [Trait("Category", "��芸�����m�F")]
        public void Test2()
        {
            string testData =
            "<tr class=\"collapse\" id=\"dummy@(item.Id)\"><td colspan=\"3\"><div class=\"d-flex justify-content-end\"><button type=\"button\" class=\"btn btn-primary\" style=\"margin-right:48px\" onclick=\"activate(@item.Id)\">�L����</button><button type=\"button\" class=\"btn btn-danger\" style=\"margin-right:48px\" onclick=\"delete(@item.Id)\">�폜</button></div></td></tr>";
            var result = Trans.ParseTags(testData);

            result.Children.Count.Is(1);
            result.Children[0].Value.Category.Is("tr");

        }

        [Fact(DisplayName = "1�̃^�O��jQuery�ɂ���")]
        [Trait("Category", "��芸�����m�F")]
        public void Test3()
        {
            string testData =
            "<tr class=\"collapse\" id=\"dummy@(item.Id)\"><td colspan=\"3\"><div class=\"d-flex justify-content-end\"><button type=\"button\" class=\"btn btn-primary\" style=\"margin-right:48px\" onclick=\"activate(@item.Id)\">�L����</button><button type=\"button\" class=\"btn btn-danger\" style=\"margin-right:48px\" onclick=\"delete(@item.Id)\">�폜</button></div></td></tr>";
            var root = Trans.ParseTags(testData);

            var result1 = Trans.ToJQueryDom(root.Children[0].Value);
            result1.Is("const td1 = $(#\"<td>\").attr(\"colspan\",\"3\");");

            var result2 = Trans.ToJQueryDom(root.Children[0].Value);
            result2.Is("const div2 = $(#\"<div>\").addClass(\"d-flex\").addClass(\"justify-content-end\")");

            var result3 = Trans.ToJQueryDom(root.Children[0].Value);
            result3.Is("const button3 = $(#\"<button>\").attr(\"type\", \"button\").addClass(\"btn\").addClass(\"btn-primary\")");

        }
    }
}
