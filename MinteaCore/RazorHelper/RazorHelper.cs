//using ClosedXML.Excel;
//using Microsoft.Extensions.FileProviders;
//using MinteaCore.Extensions;
//using MithrilCube.Data;
//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.IO;
//using System.Linq;

//namespace MinteaCore.RazorHelper
//{
//    /// <summary>
//    /// Razorによるコード生成で使用した関数
//    /// 主にExcelから読み取るためのメソッド
//    /// </summary>
//    public class RazorHelper
//    {
//        /// <summary>
//        /// "Doc"が最初についているシートは読まない
//        /// </summary>
//        const string DocumentSheetPrefix = "Doc";

//        /// <summary>
//        /// "Is"が最初についている列は0,1,型文字列をbool型とする
//        /// </summary>
//        const string BoolCulumnPrefix = "Is";

//        /// <summary>
//        /// 必須シート名
//        /// </summary>
//        const string SettingsSheet = "Settings";

//        /// <summary>
//        /// 必須シート名
//        /// </summary>
//        const string RootListSheet = "RootList";

//        /// <summary>
//        /// 必須シート内の必須カラム名
//        /// </summary>
//        const string IndexValue = "Index";

//        /// <summary>
//        /// "Out"が最後についているシートは外部入力
//        /// </summary>
//        const string OutSheet = "Out";
//        /// <summary>
//        /// "OutList"が最後についているシートは外部入力
//        /// </summary>
//        const string OutListSheetSuffix = "OutList";

//        /// <summary>
//        /// "List"が最後についているシートはリスト
//        /// </summary>
//        const string ListSheetSuffix = "List";

//        /// <summary>
//        /// キー情報を格納する列
//        /// </summary>
//        const string KeyCulumn = "Key";

//        /// <summary>
//        /// 親情報を格納する列
//        /// </summary>
//        const string ParentCulumn = "Parent";

//        /// <summary>
//        /// Camel, Pascal, Pluralを用意する列
//        /// CamelPlural
//        /// PascalPlural
//        /// </summary>
//        const string InflectCulumn = "Name";
//        const string Camel = "Camel";
//        const string Pascal = "Pascal";
//        const string Plural = "Plural";
//        const string Hyphen = "Hyphen";
//        const string Snake = "Snake";
//        const string CamelPlural = "CamelPlural";
//        const string PascalPlural = "PascalPlural";

//    }
//}
