using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Mintea.SnippetGenerator
{
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

                var isImport = Data.Imports != null && Data.Imports.Count > 0;
                if (isImport)
                {
                    //<Imports>
                    //    <Import>
                    //        <Namespace>System.Data</Namespace>
                    //    </Import>
                    //    ...
                    //</Imports>
                    w.WriteStartElement("Imports");
                    foreach (var item in Data.Imports)
                    {
                        w.WriteStartElement("Import");
                        w.WriteStartElement("Namespace");
                        w.WriteValue(item);
                        w.WriteEndElement();
                        w.WriteEndElement();
                    }
                    w.WriteEndElement();
                }

                var isDeclarations = Data.Declarations != null && Data.Declarations.Count > 0;
                if (isDeclarations)
                {
                    //<Declarations>
                    //    <Literal>
                    //        <ID>type</ID>
                    //        <ToolTip>プロパティの型</ToolTip>
                    //        <Default>int</Default>
                    //    </Literal>
                    //    <Literal Editable="false">
                    //        <ID>property</ID>
                    //        <ToolTip>プロパティ名</ToolTip>
                    //        <Default>MyProperty</Default>
                    //        <Function>MyProperty</Function>
                    //    </Literal>
                    //</Declarations>
                    w.WriteStartElement("Declarations");
                    foreach (var item in Data.Declarations)
                    {
                        w.WriteStartElement("Literal");
                        w.WriteStartElement("ID");
                        w.WriteValue(item.Id);
                        w.WriteEndElement();
                        if (item.ToolTip != null)
                        {
                            w.WriteStartElement("ToolTip");
                            w.WriteValue(item.ToolTip);
                            w.WriteEndElement();
                        }
                        if (item.Default != null)
                        {
                            w.WriteStartElement("Default");
                            w.WriteValue(item.Default);
                            w.WriteEndElement();
                        }
                        if (item.Function != null && item.Function != Function.None)
                        {
                            w.WriteStartElement("Function");
                            if (item.Editable != null)
                            {
                                w.WriteStartAttribute("Editable", "");
                                w.WriteString(item.Editable);
                                w.WriteEndAttribute();
                            }
                            if (!string.IsNullOrWhiteSpace(item.FunctionValue))
                            {
                                w.WriteValue(item.Function.ToString() + "(" + item.FunctionValue + ")");
                            }
                            w.WriteEndElement();
                        }
                        w.WriteEndElement();
                    }
                    w.WriteEndElement();
                }

                w.WriteStartElement("Code");
                w.WriteStartAttribute("Language", "");
                w.WriteString(Data.Language.ToString());
                w.WriteEndAttribute();
                w.WriteStartAttribute("Kind", "");
                w.WriteString(Data.Kind.GetStringValue());
                w.WriteEndAttribute();
                w.WriteStartAttribute("Delimiter", "");
                w.WriteString(Data.Delimiter);
                w.WriteEndAttribute();
                w.WriteCData(Data.Code);

                // Snippet句ここまで
                w.WriteEndElement();

                w.WriteEndElement();
                w.WriteEndElement();

                // 完成
                w.Flush();
            }
            return sw;

        }
    }
}
