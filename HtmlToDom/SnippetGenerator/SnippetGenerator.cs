using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Mintea.SnippetGenerator
{
    // TODO:この後どうするか？
    // 

    /// <summary>
    /// スニペットファイルを生成する
    /// </summary>
    public class SnippetGenerator
    {
        public void MakeXml()
        {
            // 設定
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = ("  "),
                Encoding = Encoding.UTF8
            };

            using (var w = XmlWriter.Create("./file.xml", settings))
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
                w.WriteValue("Expansion");  // 改行せずに値を書く
                w.WriteEndElement();
                w.WriteEndElement();

                w.WriteStartElement("Title");   
                w.WriteValue("test_theory");
                w.WriteEndElement();

                w.WriteStartElement("Author");
                w.WriteEndElement();

                w.WriteStartElement("Description");
                w.WriteEndElement();

                w.WriteStartElement("HelpUrl");
                w.WriteEndElement();

                w.WriteStartElement("Shortcut");
                w.WriteValue("test_theory");
                w.WriteEndElement();

                w.WriteEndElement();

                // Snippet
                w.WriteStartElement("Snippet");

                w.WriteStartElement("Code");
                w.WriteStartAttribute("Language", "");
                w.WriteString("csharp");
                w.WriteStartAttribute("Delimiter", "");
                w.WriteString("$");
                w.WriteEndAttribute();
                w.WriteCData("");   // ここに本体を書く

                w.WriteEndElement();

                w.WriteEndElement();
                w.WriteEndElement();

                w.Flush();
            }
        }
    }
}
