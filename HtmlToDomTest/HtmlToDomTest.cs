using System;
using Xunit;
using ChainingAssertion;
using Xunit.Abstractions;
using System.Collections.Generic;
using Mintea.HtmlToDom;

namespace MinteaTest
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

        [Fact(DisplayName = "innerText�擾�m�F")]
        [Trait("Category", "��芸�����m�F")]
        public void Test3()
        {
            string testData =
            "<tr class=\"collapse\" id=\"dummy@(item.Id)\"><td colspan=\"3\"><div class=\"d-flex justify-content-end\"><button type=\"button\" class=\"btn btn-primary\" style=\"margin-right:48px\" onclick=\"activate(@item.Id)\">�L����</button><button type=\"button\" class=\"btn btn-danger\" style=\"margin-right:48px\" onclick=\"delete(@item.Id)\">�폜</button></div></td></tr>";
            var result = Trans.SearchInnerTextAll(testData, '<', '>');

            result[0].Is("");
            result[1].Is("");
            result[2].Is("");
            result[3].Is("�L����");
            result[4].Is("");
            result[5].Is("�폜");
            result[6].Is("");
            result[7].Is("");
            result[8].Is("");
            result[9].Is("");
        }

        [Fact(DisplayName = "jQuery�̏����Eappend���쐬")]
        [Trait("Category", "��芸�����m�F")]
        public void Test5()
        {
            string testData =
            "<tr class=\"collapse\" id=\"dummy@(item.Id)\"><td colspan=\"3\"><div class=\"d-flex justify-content-end\"><button type=\"button\" class=\"btn btn-primary\" style=\"margin-right:48px\" onclick=\"activate(@item.Id)\">�L����</button><button type=\"button\" class=\"btn btn-danger\" style=\"margin-right:48px\" onclick=\"delete(@item.Id)\">�폜</button></div></td></tr>";
            var root = Trans.ParseTags(testData);
            var sequence = new List<TagInfo>();
            var result = Trans.BuildJQuery(root, string.Empty, sequence);

            // ������t���Ȃ���΂Ȃ�Ȃ�
            result.Is("div3.appendChild(button4);\ndiv3.appendChild(button5);\ntd2.appendChild(div3);\ntr1.appendChild(td2);\n");

            sequence.Count.Is(5);
            sequence[0].Category.Is("button");
            sequence[1].Category.Is("button");
            sequence[2].Category.Is("div");
            sequence[3].Category.Is("td");
            sequence[4].Category.Is("tr");
        }

        [Fact(DisplayName = "�S��jQuery�ɂ���")]
        [Trait("Category", "��芸�����m�F")]
        public void Test6()
        {
            string testData =
               "<tr class=\"collapse\" id=\"dummy@(item.Id)\"><td colspan=\"3\"><div class=\"d-flex justify-content-end\"><button type=\"button\" class=\"btn btn-primary\" style=\"margin-right:48px\" onclick=\"activate(@item.Id)\">�L����</button><button type=\"button\" class=\"btn btn-danger\" style=\"margin-right:48px\" onclick=\"delete(@item.Id)\">�폜</button></div></td></tr>";

            var result = Trans.ToJQuery(testData);

            output.WriteLine(result);
            result.Is("const button4 = $('<button>')\n    .attr('type', 'button')\n    .addClass('btn')\n    .addClass('button4Class')\n    .addClass('button4Funconclick')\n    .text('�L����');\n\n\nconst button5 = $('<button>')\n    .attr('type', 'button')\n    .addClass('btn')\n    .addClass('button5Class')\n    .addClass('button5Funconclick')\n    .text('�폜');\n\n\nconst div3 = $('<div>')\n    .addClass('d-flex');\n\n\nconst td2 = $('<td>')\n    .attr('colspan', '3');\n\n\nconst tr1 = $('<tr>')\n    .addClass('collapse')\n    .attr('id', 'dummy@(item\n    .Id)');\n\ndiv3.appendChild(button4);\ndiv3.appendChild(button5);\ntd2.appendChild(div3);\ntr1.appendChild(td2);\n");
        }
    }
}
