using System;
using System.Collections.Generic;
using System.Text;

namespace MinteaCore.HtmlToStrap
{
    #region Class:変換ルール

    /// <summary>
    /// 変換ルール
    /// </summary>
    public class Rule
    {
        const string Separate = "&";

        /// <summary>
        /// 条件・種類
        /// classなど
        /// "SimpleReplacement"の時は、SrcTagとSrcTermValueで単純置換する
        /// </summary>
        public TermCategory SrcTermCategory { get; set; } = TermCategory.SimpleReplacement;

        /// <summary>
        /// 対象のタグ:divなど
        /// </summary>
        public string TargetTag { get; set; } = string.Empty;

        /// <summary>
        /// 条件・値
        /// mt-3 rowなど
        /// 単純置換の時は不使用
        /// </summary>
        public string TargetValue { get; set; } = string.Empty;

        /// <summary>
        /// 変換先の値
        /// 条件に合った部分をこの値に変換する
        /// </summary>
        public string DestValue { get; set; } = string.Empty;

        /// <summary>
        /// 特定のルールについて、単純置換を行う
        /// 置換を行わないルールの場合、そのまま返す
        /// </summary>
        /// <param name="source">対象文字列</param>
        /// <returns>置換後の文字列</returns>
        public string Replace(string source)
        {
            switch (SrcTermCategory)
            {
                case TermCategory.SimpleReplacement:
                    return source.Replace(TargetTag, DestValue);
                case TermCategory.IncludeStrClassToTagName:
                    // <row aaaa
                    // </row>
                    return source.Replace($"<{TargetValue} ", $"<{DestValue} ").Replace($"<{TargetValue}>", $"<{DestValue}>").Replace($"</{TargetValue}>", $"</{DestValue}>");
                default:
                    return source;
            }
        }

        #region 条件作成を補助するクラスメソッド
        /// <summary>
        /// コメントはパースしたときに保持されないので退避する
        /// </summary>
        /// <param name="destBegin">置換後開きタグ</param>
        /// <param name="destEnd">置換後閉じタグ</param>
        /// <returns></returns>
        public static Rule GetCommentReplacementRule(string destBegin, string destEnd)
        {
            // htmlは"<!-- -->"だけなのでそれしか対応しない
            return new Rule { SrcTermCategory = TermCategory.CommentReplacement, TargetValue = destBegin, DestValue = destEnd };
        }

        /// <summary>
        /// 全ての変換後、単純に文字を置換する
        /// </summary>
        /// <param name="src">置換前文字列</param>
        /// <param name="dest">置換後文字列</param>
        /// <returns></returns>
        public static Rule GetSimpleReplacementRule(string src, string dest)
        {
            return new Rule { SrcTermCategory = TermCategory.SimpleReplacement, TargetTag = src, DestValue = dest };
        }

        /// <summary>
        /// 特定の名称のAttrを除去するルールを作成する
        /// </summary>
        /// <param name="targetTag">対象タグ</param>
        /// <param name="targetValue">条件となるAttr名</param>
        /// <returns></returns>
        public static Rule GetRemoveAttrRule(string targetTag, string targetValue)
        {
            return new Rule { SrcTermCategory = TermCategory.RemoveAttr, TargetTag = targetTag, TargetValue = targetValue };
        }

        /// <summary>
        /// タグを他のタグに変換する
        /// </summary>
        /// <param name="targetTag">対象タグ</param>
        /// <param name="destTag">どのタグに変換するか</param>
        /// <returns></returns>
        public static Rule GetReplaceTagRule(string targetTag, string destTag)
        {
            return new Rule { SrcTermCategory = TermCategory.IncludeStrClassToTagName, TargetTag = targetTag, TargetValue = targetTag, DestValue = destTag };
        }

        /// <summary>
        /// クラスを他のタグに変換する
        /// 既存のクラス値は削除する
        /// </summary>
        /// <param name="targetTag">対象タグ</param>
        /// <param name="targetValue">条件となるClass値（ハイフンで区切った時のキーワード）</param>
        /// <param name="destTag">どのタグに変換するか</param>
        /// <returns></returns>
        public static Rule GetReplaceTagByClassRule(string targetTag, string targetValue, string destTag)
        {
            return new Rule { SrcTermCategory = TermCategory.IncludeStrClassToTagName, TargetTag = targetTag, TargetValue = targetValue, DestValue = destTag };
        }

        /// <summary>
        /// クラス値を削除する
        /// </summary>
        /// <param name="targetTag">対象タグ</param>
        /// <param name="targetValue">条件となるClass値</param>
        /// <returns></returns>
        public static Rule GetRemoveClassRule(string targetTag, string targetValue)
        {
            return new Rule { SrcTermCategory = TermCategory.RemoveClass, TargetTag = targetTag, TargetValue = targetValue };
        }

        /// <summary>
        /// クラスに特定文字が含まれていたらAttrに変換する
        /// </summary>
        /// <param name="targetTag">対象タグ</param>
        /// <param name="targetClassString">条件となるClass文字列</param>
        /// <param name="targetAttr">変換するAttr名</param>
        /// <param name="targetAttrValue">変換するAttr値</param>
        /// <returns></returns>
        public static Rule GetIncludeClassToAttrRule(string targetTag, string targetClassString, string targetAttr, string targetAttrValue)
        {
            return new Rule { SrcTermCategory = TermCategory.IncludeStrClassToAttr, TargetTag = targetTag, TargetValue = targetClassString, DestValue = $"{targetAttr}{Separate}{targetAttrValue}" };
        }

        public string[] GetValues()
        {
            return DestValue.Split(Separate);
        }
        #endregion

        /// <summary>
        /// 条件の種類
        /// </summary>
        public enum TermCategory
        {
            /// <summary>
            /// コメントなど、パースしたときに保持されない情報を退避するための仕組み
            /// 一時的に別のタグに置き換えてから変換する
            /// </summary>
            CommentReplacement,

            /// <summary>
            /// 単純置換
            /// この時はSrcTermValueに何も設定しない
            /// SrcTagはタグ名ではなく、置換前の文字列を入れる
            /// </summary>
            SimpleReplacement,

            /// <summary>
            /// 対象の要素を除去する
            /// </summary>
            RemoveAttr,

            /// <summary>
            /// class要素を他のタグに変換する
            /// </summary>
            IncludeStrClassToTagName,

            /// <summary>
            /// class値を削除する
            /// </summary>
            RemoveClass,

            /// <summary>
            /// 特定の文字列が含まれているClassを
            /// 特定のAttr値に変換する
            /// </summary>
            IncludeStrClassToAttr
        }

    }

    #endregion
}
