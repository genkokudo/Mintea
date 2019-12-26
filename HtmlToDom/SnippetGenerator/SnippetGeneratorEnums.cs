using System;
using System.Collections.Generic;
using System.Text;

namespace Mintea.SnippetGenerator
{
    /// <summary>
    /// テキストタイプ
    /// ぶっちゃけよくわからないのでExpansionだけ使う
    /// </summary>
    public enum SnippetType
    {
        /// <summary>選択したコードの周りにコード スニペットを配置</summary>
        SurroundsWith,
        /// <summary>カーソル位置にコード スニペットを挿入</summary>
        Expansion,
        /// <summary>
        /// C# のリファクタリング中にコード スニペットを使用
        /// カスタムのコード スニペットには使用できない
        /// </summary>
        Refactoring
    }

    // https://docs.microsoft.com/ja-jp/visualstudio/ide/code-snippet-functions?view=vs-2019
    /// <summary>リテラルに適用する関数</summary>
    public enum Function
    {
        /// <summary>
        /// なし
        /// </summary>
        None,
        /// <summary>
        /// Switch文のCaseを生成する
        /// 要引数：他のリテラルのIDを指定（リテラルの型は列挙体）
        /// </summary>
        GenerateSwitchCases,
        /// <summary>クラス名を設定する</summary>
        ClassName,
        /// <summary>
        /// 引数の型名を最も単純な形にする
        /// 例："global::System.Console"にするとSystemがインポートされていればConsoleに置き換えられる
        /// 要引数：型名を指定
        /// </summary>
        SimpleTypeName
    }

    //VB, CPPは使用しない
    /// <summary>言語</summary>
    public enum Language
    {
        /// <summary>C#</summary>
        CSharp,
        /// <summary>JavaScript</summary>
        JavaScript,
        /// <summary>TypeScript</summary>
        TypeScript,
        /// <summary>HTML</summary>
        HTML,
        /// <summary>SQL</summary>
        SQL,
        /// <summary>XML</summary>
        XML,
        /// <summary>XAML</summary>
        XAML
    }

    /// <summary>スニペットの種類</summary>
    public enum Kind
    {
        /// <summary>
        /// 処理のみ
        /// メソッドの内部で使用する
        /// </summary>
        [StringValue("method body")]
        MethodBody, //正常終了－

        /// <summary>
        /// メソッド宣言含む
        /// クラスの中、メソッドの外で使用する
        /// </summary>
        [StringValue("method decl")]
        MethodDecl, //エラー 1

        /// <summary>
        /// 型
        /// クラスの中、メソッドの外で使用する
        /// </summary>
        [StringValue("type decl")]
        TypeDecl, //エラー 1

        /// <summary>
        /// 完全なコードファイル 
        /// 単体でコードファイル、名前空間内に使用する
        /// </summary>
        [StringValue("file")]
        File, //エラー 1

        /// <summary>
        /// コンテキストに依存しない
        /// どこにでも使用できる
        /// </summary>
        [StringValue("any")]
        Any, //エラー 2
    }

    #region Enumに文字列値を設定する
    /// <summary>
    /// Enumに文字列を付加するための属性クラス
    /// </summary>
    public class StringValueAttribute : Attribute
    {
        /// <summary>
        /// Enum内にstring型の値を持たせる
        /// </summary>
        public string StringValue { get; protected set; }

        /// <summary>
        /// stringの値を初期化
        /// </summary>
        /// <param name="value"></param>
        public StringValueAttribute(string value)
        {
            this.StringValue = value;
        }
    }

    /// <summary>
    /// Enumに対して拡張メソッドを追加する
    /// </summary>
    public static class EnumExtension
    {
        /// <summary>
        /// Enum値が渡されたらstringの値を取得する
        /// これはEnum値にStringの値を設定したときのみ動作する
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetStringValue(this Enum value)
        {
            Type type = value.GetType();

            // この型のフィールド情報を取得
            System.Reflection.FieldInfo fieldInfo = type.GetField(value.ToString());

            // 範囲外の値チェック
            if (fieldInfo == null) return null;

            // フィールド情報から、目的の属性を探す
            StringValueAttribute[] attribs = fieldInfo.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];

            // 属性があればその設定値を返却する
            // 複数あっても最初のものだけ返却する
            return attribs.Length > 0 ? attribs[0].StringValue : null;

        }
    }
    #endregion

}
