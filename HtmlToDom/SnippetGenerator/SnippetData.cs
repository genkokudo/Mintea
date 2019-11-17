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


        // SurroundsWith: 選択したコードの周りにコード スニペットを配置します。
        // Expansion : カーソル位置にコード スニペットを挿入します。
        // Refactoring: C# のリファクタリング中にコード スニペットを使用するよう指定します。 Refactoring は、カスタムのコード スニペットには使用できません。
        /// <summary>スニペットのタイプ</summary>
        public string SnippetType { get; set; } = "Expansion";

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

        // Snippet 要素
        // Declarations:編集できるコード スニペットの部分を構成するリテラルとオブジェクトを指定します。
        //    <Snippet>
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
        //    </Literal>
        //    <Literal>
        //        <ID>field</ID>
        //        <ToolTip>このプロパティのバッキング変数</ToolTip>
        //        <Default>myVar</Default>
        //    </Literal>
        //</Declarations>
        // Literalのfunctionsとは
        // https://docs.microsoft.com/ja-jp/visualstudio/ide/code-snippet-functions?view=vs-2019

        // Imports:コード スニペットでインポートする必要のある名前空間が格納されます。
        // References:コード スニペットで参照する必要のあるアセンブリ（dllのこと）についての情報が格納されます。VB用なのでいらない。

        // Code要素
        //    <Code Language = "Language"
        //        Kind="method body/method decl/type decl/page/file/any"
        //        Delimiter="Delimiter">

        /// <summary>テンプレートにするコード</summary>
        public string Code { get; set; } = string.Empty;

        // TODO:Enumにする
        /// <summary>言語</summary>
        public string Language { get; set; } = "csharp";

        /// <summary>特殊文字</summary>
        public string Delimiter { get; set; } = "$";

        // TODO:Kind属性
        //method body コード スニペットがメソッドの本体であり、メソッド宣言の内部に挿入する必要があることを示します。
        //method decl コード スニペットがメソッドであり、クラスまたはモジュールの内部に挿入する必要があることを示します。
        //type decl   コード スニペットが型であり、クラス、モジュール、または名前空間の内部に挿入する必要があることを示します。
        //file スニペットが完全なコード ファイルであることを示します。 これらのコード スニペットは、単体でコード ファイルに挿入することも、名前空間内に挿入することもできます。
        //any スニペットをどこにでも挿入できることを示します。 このタグは、コメントなど、コンテキストに依存しないコード スニペットに使用します。

    }
}
