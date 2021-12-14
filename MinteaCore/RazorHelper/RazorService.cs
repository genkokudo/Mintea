using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MinteaCore.RazorHelper
{
    public interface IRazorService
    {
        /// <summary>
        /// Excelファイルを読み込み、シート名をキーとした辞書にする
        /// xlsxのみ対応
        /// </summary>
        /// <param name="path">ファイルパス</param>
        /// <param name="isRequiredTitle">1行目に何もない列を無視する</param>
        /// <returns>シート名をキーとした辞書、行と列の2次元string</returns>
        public Dictionary<string, List<List<string>>> ReadExcel(string path, bool isRequiredTitle = false);
    }

    public class RazorService : IRazorService
    {
        public Dictionary<string, List<List<string>>> ReadExcel(string path, bool isRequiredTitle = false)
        {
            // ファイルの読み込み
            var xlsx = new Dictionary<string, List<List<string>>>();
            using (var wb = new XLWorkbook(path))
            {
                foreach (var ws in wb.Worksheets)
                {
                    // ワークシート
                    List<List<string>> sheet = new List<List<string>>();
                    // TODO:何も書いてないシートがあると落ちる？
                    for (int i = 1; i <= ws.LastCellUsed().Address.RowNumber; i++)
                    {
                        List<string> raw = new List<string>();
                        for (int j = 1; j <= ws.LastCellUsed().Address.ColumnNumber; j++)
                        {
                            // 1行目に何もない列を無視する
                            if (!isRequiredTitle || !string.IsNullOrWhiteSpace(ws.Cell(1, j).Value.ToString()))
                            {
                                raw.Add(ws.Cell(i, j).Value.ToString());
                            }
                        }
                        sheet.Add(raw);
                    }

                    // シート名と一緒に登録
                    xlsx.Add(ws.Name, sheet);
                }
            }

            return xlsx;
        }
    }
}
