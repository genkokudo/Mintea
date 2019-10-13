using System;
using Xunit;
using HtmlToDom;

namespace HtmlToDomTest
{
    public class HtmlToDomTest
    {
        [Fact]
        public void Test1()
        {
            string testData =
            "<tr class=\"collapse\" id=\"dummy@(item.Id)\"><td colspan=\"3\"><div class=\"d-flex justify-content-end\"><button type=\"button\" class=\"btn btn-primary\" style=\"margin-right:48px\" onclick=\"activate(@item.Id)\">—LŒø‰»</button><button type=\"button\" class=\"btn btn-danger\" style=\"margin-right:48px\" onclick=\"delete(@item.Id)\">íœ</button></div></td></tr>";

        }
    }
}
