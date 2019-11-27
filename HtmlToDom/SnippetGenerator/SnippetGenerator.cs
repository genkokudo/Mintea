using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Mintea.SnippetGenerator
{
    // TODO:この後ここを作り込んでいく
    // Web画面に出す場合、ModelはSnippetDataを継承して作成し、BindDataすればよい

    /// <summary>
    /// スニペットファイルを生成する
    /// </summary>
    public class SnippetGenerator
    {
        #region 設定

        readonly XmlWriterSettings Settings = new XmlWriterSettings
        {
            Indent = true,
            IndentChars = ("  "),
            Encoding = Encoding.UTF8
        };

        /// <summary>
        /// 何故かXMLヘッダがUTF-16になるので、UTF8に矯正する
        /// </summary>
        private class StringWriterUTF8 : StringWriter
        {
            public override Encoding Encoding
            {
                get { return Encoding.UTF8; }
            }
        }

        #endregion

        /// <summary>
        /// スニペットXMLを出力する
        /// </summary>
        /// <param name="Data">スニペットデータ</param>
        /// <returns>StringBuilderを返す</returns>
        public StringWriter MakeSnippetXml(SnippetData Data)
        {
            if(Data == null) return null;

            var sw = new StringWriterUTF8();
            using (var w = XmlWriter.Create(sw, Settings))
            {
                // 基本：WriteStartElementとWriteEndElementがセット、タグで囲んだ値はWriteValueを呼んで設定。
                // WriteStartAttributeとWriteEndAttributeもセット、ここの値設定はWriteStringを呼ぶ。

                // <?xml version="1.0" encoding="utf-8"?>
                w.WriteStartDocument();

                // <CodeSnippets xmlns="http://schemas.microsoft.com/VisualStudio/2005/CodeSnippet">
                w.WriteStartElement("CodeSnippets", "http://schemas.microsoft.com/VisualStudio/2005/CodeSnippet");

                // <CodeSnippet Format="1.0.0">
                w.WriteStartElement("CodeSnippet");
                w.WriteStartAttribute("Format", "");
                w.WriteString("1.0.0");
                w.WriteEndAttribute();

                // Header句
                w.WriteStartElement("Header");

                // 可変長の要素になる・・・かな？
                w.WriteStartElement("SnippetTypes");
                w.WriteStartElement("SnippetType");
                w.WriteValue(Data.SnippetType.ToString());  // 改行せずに値を書く
                w.WriteEndElement();
                w.WriteEndElement();

                w.WriteStartElement("Title");
                w.WriteValue(Data.Title);
                w.WriteEndElement();

                w.WriteStartElement("Author");
                w.WriteValue(Data.Author);
                w.WriteEndElement();

                w.WriteStartElement("Description");
                w.WriteValue(Data.Description);
                w.WriteEndElement();

                w.WriteStartElement("HelpUrl");
                w.WriteValue(Data.HelpUrl);
                w.WriteEndElement();

                w.WriteStartElement("Shortcut");
                w.WriteValue(Data.Shortcut);
                w.WriteEndElement();

                w.WriteEndElement();

                // Snippet句
                w.WriteStartElement("Snippet");

                w.WriteStartElement("Code");
                w.WriteStartAttribute("Language", "");
                w.WriteString(Data.Language.ToString());
                w.WriteStartAttribute("Kind", "");
                w.WriteString(Data.Kind.GetStringValue());
                w.WriteStartAttribute("Delimiter", "");
                w.WriteString(Data.Delimiter);
                w.WriteEndAttribute();
                w.WriteCData(Data.Code);

                w.WriteEndElement();
                // TODO: こっから作業すること！
                // Declarations
                //<Declarations>
                //    <Literal>
                //        <ID>type</ID>
                //        <ToolTip>プロパティの型</ToolTip>
                //        <Default>int</Default>
                //    </Literal>
                //    <Literal>
                //        <ID>property</ID>
                //        <ToolTip>プロパティ名</ToolTip>
                //        <Default>MyProperty</Default>
                //        <Function>MyProperty</Function>
                //    </Literal>
                //</Declarations>

                // Imports
                //<Imports>
                //    <Import>
                //        <Namespace>System.Data</Namespace>
                //    </Import>
                //    ...
                //</Imports>

                w.WriteEndElement();
                w.WriteEndElement();

                // 完成
                w.Flush();
            }
            return sw;


            //using (var w = XmlWriter.Create($"./{Filename}.xml", Settings))
            //{
            //    // 基本：WriteStartElementとWriteEndElementがセット、タグで囲んだ値はWriteValueを呼んで設定。
            //    // WriteStartAttributeとWriteEndAttributeもセット、ここの値設定はWriteStringを呼ぶ。

            //    // <?xml version="1.0" encoding="utf-8"?>
            //    w.WriteStartDocument();

            //    // <CodeSnippets xmlns="http://schemas.microsoft.com/VisualStudio/2005/CodeSnippet">
            //    w.WriteStartElement("CodeSnippets", "http://schemas.microsoft.com/VisualStudio/2005/CodeSnippet");

            //    // <CodeSnippet Format="1.0.0">
            //    w.WriteStartElement("CodeSnippet");
            //    w.WriteStartAttribute("Format", "");
            //    w.WriteString("1.0.0");
            //    w.WriteEndAttribute();

            //    // Header句
            //    w.WriteStartElement("Header");

            //    // 可変長の要素になる・・・かな？
            //    w.WriteStartElement("SnippetTypes");
            //    w.WriteStartElement("SnippetType");
            //    w.WriteValue("Expansion");  // 改行せずに値を書く
            //    w.WriteEndElement();
            //    w.WriteEndElement();

            //    w.WriteStartElement("Title");
            //    w.WriteValue("test_theory");
            //    w.WriteEndElement();

            //    w.WriteStartElement("Author");
            //    w.WriteEndElement();

            //    w.WriteStartElement("Description");
            //    w.WriteEndElement();

            //    w.WriteStartElement("HelpUrl");
            //    w.WriteEndElement();

            //    w.WriteStartElement("Shortcut");
            //    w.WriteValue("test_theory");
            //    w.WriteEndElement();

            //    w.WriteEndElement();

            //    // Snippet
            //    w.WriteStartElement("Snippet");

            //    w.WriteStartElement("Code");
            //    w.WriteStartAttribute("Language", "");
            //    w.WriteString("csharp");
            //    w.WriteStartAttribute("Delimiter", "");
            //    w.WriteString("$");
            //    w.WriteEndAttribute();
            //    w.WriteCData("");   // ここに本体を書く

            //    w.WriteEndElement();

            //    w.WriteEndElement();
            //    w.WriteEndElement();

            //    // 完成
            //    w.Flush();
            //}
        }
    }
}
