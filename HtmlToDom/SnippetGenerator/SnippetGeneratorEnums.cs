using System;
using System.Collections.Generic;
using System.Text;

namespace Mintea.SnippetGenerator
{
    //VB, CPP, XML, SQLは使用しない
    /// <summary></summary>
    public enum Language
    {
        /// <summary>C#</summary>
        CSharp,
        /// <summary>JavaScript</summary>
        JavaScript,
        /// <summary>TypeScript</summary>
        TypeScript,
        /// <summary>HTML</summary>
        HTML
    }

    /// <summary></summary>
    public enum Kind
    {
        // TODO:コメントをもっと砕いた表現にする

        /// <summary>コード スニペットがメソッドの本体であり、メソッド宣言の内部に挿入する必要があることを示します。</summary>
        [StringValue("method body")]
        MethodBody, //正常終了－

        /// <summary>コード スニペットがメソッドであり、クラスまたはモジュールの内部に挿入する必要があることを示します。</summary>
        [StringValue("method decl")]
        MethodDecl, //エラー 1

        /// <summary>コード スニペットが型であり、クラス、モジュール、または名前空間の内部に挿入する必要があることを示します。</summary>
        [StringValue("type decl")]
        TypeDecl, //エラー 1

        /// <summary>スニペットが完全なコード ファイルであることを示します。 これらのコード スニペットは、単体でコード ファイルに挿入することも、名前空間内に挿入することもできます。</summary>
        [StringValue("file")]
        File, //エラー 1

        /// <summary>スニペットをどこにでも挿入できることを示します。 このタグは、コメントなど、コンテキストに依存しないコード スニペットに使用します。</summary>
        [StringValue("any")]
        Any, //エラー 2
    }

    // TODO:コメントなどを直す
    // TODO:スニペット化（任意のクラスの拡張方法として1つ、上とセット（enumの拡張方法として）で1つ）
    #region Enumに文字列値を設定する
    /// <summary>
    /// Enumに文字列を付加するためのAttributeクラス
    /// </summary>
    public class StringValueAttribute : Attribute
    {
        /// <summary>
        /// Holds the stringvalue for a value in an enum.
        /// </summary>
        public string StringValue { get; protected set; }

        /// <summary>
        /// Constructor used to init a StringValue Attribute
        /// </summary>
        /// <param name="value"></param>
        public StringValueAttribute(string value)
        {
            this.StringValue = value;
        }
    }

    public static class CommonAttribute
    {

        /// <summary>
        /// Will get the string value for a given enums value, this will
        /// only work if you assign the StringValue attribute to
        /// the items in your enum.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetStringValue(this Enum value)
        {
            // Get the type
            Type type = value.GetType();

            // Get fieldinfo for this type
            System.Reflection.FieldInfo fieldInfo = type.GetField(value.ToString());

            //範囲外の値チェック
            if (fieldInfo == null) return null;

            StringValueAttribute[] attribs = fieldInfo.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];

            // Return the first if there was a match.
            return attribs.Length > 0 ? attribs[0].StringValue : null;

        }
    }
    #endregion
}
