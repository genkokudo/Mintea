using System;
using System.Collections.Generic;
using System.Text;

namespace Mintea.SnippetGenerator
{
    /// <summary>
    /// スニペットデータ
    /// </summary>
    public class SnippetData
    {
        // Header 要素
        // Keywords 要素:誰も使ってないみたい。いらない

        /// <summary>スニペットのタイプ</summary>
        public SnippetType SnippetType { get; set; } = SnippetType.Expansion;

        /// <summary>タイトル</summary>
        public string Title { get; set; } = "Untitled";

        /// <summary>作者</summary>
        public string Author { get; set; } = "ginpay";

        /// <summary>説明</summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>VisualStudioでは使いません</summary>
        public string HelpUrl { get; set; } = "www.microsoft.com";

        /// <summary>ショートカットになるフレーズ</summary>
        public string Shortcut { get; set; } = string.Empty;

        #region Snippet要素

        #endregion
        // Snippet 要素
        /// <summary>
        /// LiteralとObjectのリストだが
        /// Objectは使わないのでLiteralのリストにする
        /// </summary>
        public List<Literal> Declarations { get; set; }

        /// <summary>
        /// インポートする必要のある名前空間が格納されます。
        /// Imports > Import(複数) > Namespace
        /// </summary>
        public List<string> Imports { get; set; }

        // References要素:スニペットで参照する必要のあるアセンブリ（dllのこと）、VB用なのでいらない。

        #region Code要素
        // <Code Language = "Language"　Kind="method body/method decl/type decl/page/file/any"　Delimiter="Delimiter">
        /// <summary>テンプレートにするコード</summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>言語</summary>
        public Language Language { get; set; } = Language.CSharp;

        /// <summary>特殊文字</summary>
        public string Delimiter { get; set; } = "$";

        /// <summary>スニペットの種類</summary>
        public Kind Kind { get; set; } = Kind.Any;
        #endregion

    }

    /// <summary>
    /// デミリタ文字で定義される部分の値
    /// Literal以外にObjectがあるが、こっちは使わない
    /// </summary>
    public class Literal    // abstruct class Declarationsを作っても良い
    {
        // Declarations:編集できるコード スニペットの部分を構成するリテラルとオブジェクトを指定します。
        //<Declarations>
        //    <Literal>
        //        <ID>type</ID>
        //        <ToolTip>プロパティの型</ToolTip>
        //        <Default>int</Default>
        //    </Literal>
        //    <Literal>     // default="true" Editable="false" みたいな要素があるけど不要なので無視
        //        <ID>property</ID>
        //        <ToolTip>プロパティ名</ToolTip>
        //        <Default>MyProperty</Default>
        //        <Function>MyProperty</Function>
        //    </Literal>
        //</Declarations>

        /// <summary>デミリタで囲まれた文字列</summary>
        public string Id { get; set; }

        /// <summary>説明</summary>
        public string ToolTip { get; set; }

        /// <summary>デフォルト値</summary>
        public string Default { get; set; }

        /// <summary>リテラルに適用する関数</summary>
        public Function Function { get; set; }
    }

    /// <summary>
    /// デミリタ文字で定義される部分の値
    /// </summary>
    public class Declaration
    {
        /// <summary>名前</summary>
        public Literal Literal { get; set; }
    }
}
