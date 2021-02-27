using AngleSharp.Dom;
using AngleSharp.Html;

namespace MinteaCore.HtmlToStrap
{
    // "<option value='検索条件1' ></option>"   ではなく
    // "<option value='検索条件1' />"   にしたいからFormatterを改造する

    /// <summary>
    /// 閉じタグを生成するためのフォーマッタ
    /// https://anglesharp.github.io/docs/Questions.html#how-to-produce-self-closing-tags
    /// </summary>
    public class MyFormatter : PrettyMarkupFormatter
    {
        public override string OpenTag(IElement element, bool selfClosing)
        {
            var result = base.OpenTag(element, selfClosing);

            if (element.Children.Length == 0 && string.IsNullOrWhiteSpace(element.TextContent))
            {
                return result.Replace(">"," />");
            }

            return result;
        }

        public override string CloseTag(IElement element, bool selfClosing)
        {
            var result = base.CloseTag(element, selfClosing); // インデント処理があるので、元処理は行うこと
            if (element.Children.Length == 0 && string.IsNullOrWhiteSpace(element.TextContent))
            {
                return string.Empty;
            }
            return result;
        }
    }

    // 上の奴で"selfClosing"引数を使う場合はこの拡張を行う。まあいらないと思う。

    //public static class ElementExtensions
    //{
    //    public static void AsSelfClosing(this IElement element)
    //    {
    //        const int SelfClosing = 0x1;

    //        var type = typeof(IElement).Assembly.GetType("AngleSharp.Dom.Node");
    //        var field = type.GetField("_flags", BindingFlags.Instance | BindingFlags.NonPublic);

    //        var flags = (uint)field.GetValue(element);
    //        flags |= SelfClosing;
    //        field.SetValue(element, Enum.ToObject(field.FieldType, flags));
    //    }
    //}
}
