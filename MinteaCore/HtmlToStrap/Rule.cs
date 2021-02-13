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

        ///// <summary>
        ///// パラメータ
        ///// 結局こういうの作っちゃうんだから
        ///// </summary>
        //public List<string> Parameters { get; set; } = new List<string>();
        //var parameters = new List<string>
        //    {
        //        destBegin,  // "{/*"
        //        destEnd     // "*/}"
        //    };

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
                case TermCategory.Class:
                    return source.Replace(TargetTag, DestValue);
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
        /// 値を持ったAttrを除去するルールを作成する
        /// あまり使わないかも
        /// </summary>
        /// <param name="targetTag">対象タグ</param>
        /// <param name="targetValue">条件となる値</param>
        /// <returns></returns>
        public static Rule GetRemoveAttrByValueRule(string targetTag, string targetValue)
        {
            return new Rule { SrcTermCategory = TermCategory.RemoveValue, TargetTag = targetTag, TargetValue = targetValue };
        }

        /// <summary>
        /// 
        /// クラス値は削除する
        /// </summary>
        /// <param name="targetTag">対象タグ</param>
        /// <param name="targetValue">条件となるClass値</param>
        /// <param name="destTag">どのタグに変換するか</param>
        /// <returns></returns>
        public static Rule GetReplaceTagByClass(string targetTag, string targetValue, string destTag)
        {
            return new Rule { SrcTermCategory = TermCategory.Class, TargetTag = targetTag, TargetValue = targetValue, DestValue = destTag };
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
            /// 全ての要素に対して
            /// 要素の値を条件に値を除去する
            /// 要素は指定しない：とにかくこの値は消すってイメージ（今のところ）
            /// </summary>
            RemoveValue,

            /// <summary>
            /// class要素に対する条件
            /// </summary>
            Class
        }

    }

    #endregion
}
