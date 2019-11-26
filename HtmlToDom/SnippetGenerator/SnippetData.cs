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
        #region Header要素
        // Keywords 要素:誰も使ってないみたい。いらない

        /// <summary>スニペットのタイプ</summary>
        public SnippetType SnippetType { get; set; } = SnippetType.Expansion;

        /// <summary>タイトル、ファイル名にもなる</summary>
        public string Title { get; set; } = "Untitled";

        /// <summary>作者</summary>
        public string Author { get; set; } = "ginpay";

        /// <summary>説明</summary>
        public string Description { get; set; } = "このスニペットの説明です。";

        /// <summary>VisualStudioでは使いません</summary>
        public string HelpUrl { get; set; } = "www.microsoft.com";

        /// <summary>ショートカットになるフレーズ</summary>
        public string Shortcut { get; set; } = string.Empty;

        #endregion

        #region Snippet要素

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

        #endregion

        // References要素:スニペットで参照する必要のあるアセンブリ（dllのこと）、VB用なのでいらない。

        #region Code要素
        // <Code Language = "Language"　Kind="method body/method decl/type decl/page/file/any"　Delimiter="Delimiter">
        /// <summary>テンプレートにするコード</summary>
        public string Code { get; set; } = "Console.WriteLine(\"Hello Work!\")";

        /// <summary>言語</summary>
        public Language Language { get; set; } = Language.CSharp;

        /// <summary>特殊文字</summary>
        public string Delimiter { get; set; } = "$";

        /// <summary>スニペットの種類</summary>
        public Kind Kind { get; set; } = Kind.Any;

        public SnippetData()
        {
        }

        #endregion

        /// <summary>
        /// 可変長要素以外で必須のものを設定する
        /// </summary>
        /// <param name="title">ファイル名</param>
        /// <param name="author">作者</param>
        /// <param name="description">説明</param>
        /// <param name="shortcut">ショートカットになるフレーズ</param>
        /// <param name="code">コード</param>
        /// <param name="language">言語</param>
        /// <param name="delimiter">特殊文字</param>
        /// <param name="kind">スニペットの種類</param>
        public SnippetData(string title, string author, string description, string shortcut, string code, Language language, string delimiter, Kind kind)
        {
            Title = title;
            Author = author;
            Description = description;
            Shortcut = shortcut;
            Code = code;
            Language = language;
            Delimiter = delimiter;
            Kind = kind;
        }

        /// <summary>
        /// スニペット変数を追加する
        /// </summary>
        /// <param name="literal">変数</param>
        public void AddDeclaration(Literal literal)
        {
            if (Declarations is null)
            {
                Declarations = new List<Literal>();
            }
            Declarations.Add(literal);
        }

        /// <summary>
        /// スニペット変数を追加する
        /// </summary>
        /// <param name="id">変数名</param>
        /// <param name="toolTip">説明</param>
        /// <param name="_default">デフォルト値</param>
        /// <param name="function">適用する関数</param>
        /// <param name="functionValue">関数に使用する引数</param>
        public void AddDeclaration(string id, string toolTip, string _default, Function? function, string functionValue)
        {
            if (Declarations is null)
            {
                Declarations = new List<Literal>();
            }
            Declarations.Add(new Literal(id, toolTip, _default, function, functionValue));
        }

        /// <summary>
        /// スニペット変数を追加する
        /// </summary>
        /// <param name="id">変数名</param>
        /// <param name="toolTip">説明</param>
        /// <param name="_default">デフォルト値</param>
        /// <param name="function">適用する関数</param>
        public void AddDeclaration(string id, string toolTip, string _default, Function? function)
        {
            if (Declarations is null)
            {
                Declarations = new List<Literal>();
            }
            Declarations.Add(new Literal(id, toolTip, _default, function));
        }

        /// <summary>
        /// スニペット変数を追加する
        /// </summary>
        /// <param name="id">変数名</param>
        /// <param name="toolTip">説明</param>
        /// <param name="_default">デフォルト値</param>
        public void AddDeclaration(string id, string toolTip, string _default)
        {
            if (Declarations is null)
            {
                Declarations = new List<Literal>();
            }
            Declarations.Add(new Literal(id, toolTip, _default));
        }

        //<Imports>
        //    <Import>
        //        <Namespace>System.Data</Namespace>
        //    </Import>
        //    ...
        //</Imports>

        /// <summary>
        /// インポートする必要のある名前空間を追加する
        /// </summary>
        /// <param name="import">System.Data など</param>
        public void AddImport(string import)
        {
            // C# 8.0以降
            //(Imports ??= new List<string>()).Add(import);
            if(Imports is null)
            {
                Imports = new List<string>();
            }
            Imports.Add(import);
        }

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
        public string Id { get; set; } = "Unnamed";

        /// <summary>説明</summary>
        public string ToolTip { get; set; } = "この変数の説明";

        /// <summary>デフォルト値</summary>
        public string Default { get; set; } = "DefaultValue";

        /// <summary>リテラルに適用する関数</summary>
        public Function? Function { get; set; } = null;

        /// <summary>関数に使用する引数</summary>
        public string FunctionValue { get; set; } = null;

        public Literal(string id, string toolTip, string _default, Function? function, string functionValue)
        {
            Initialize(id, toolTip, _default);
            Function = function;
            FunctionValue = functionValue;
        }

        public Literal(string id, string toolTip, string _default, Function? function)
        {
            Initialize(id, toolTip, _default);
            Function = function;
        }

        public Literal(string id, string toolTip, string _default)
        {
            Initialize(id, toolTip, _default);
        }

        private void Initialize(string id, string toolTip, string _default)
        {
            Id = id;
            ToolTip = toolTip;
            Default = _default;
        }

    }

    #region オブジェクト使わないので削除
    // オブジェクト使わないので削除
    ///// <summary>
    ///// デミリタ文字で定義される部分の値
    ///// </summary>
    //public class Declaration
    //{
    //    /// <summary>名前</summary>
    //    public Literal Literal { get; set; }

    //    public Declaration(string id, string toolTip, string _default, Function? function)
    //    {
    //        Literal = new Literal(id, toolTip, _default, function);
    //    }

    //    public Declaration(string id, string toolTip, string _default)
    //    {
    //        Literal = new Literal(id, toolTip, _default);
    //    }

    //    public Declaration(Literal literal)
    //    {
    //        Literal = literal;
    //    }
    //}

    #endregion
}
