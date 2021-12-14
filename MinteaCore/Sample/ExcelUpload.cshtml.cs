//using DigitalMegaFlare.Data;
//using DigitalMegaFlare.Models;
//using MediatR;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.RazorPages;
//using MinteaCore.RazorHelper;
//using RazorLight;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.IO.Compression;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//// TODO:OutはRootList、OutListはListとしてmodelに格納しないと互換性がなくなってしまう

//namespace DigitalMegaFlare.Pages.ExcelWorldOnline
//{
//    public class ExcelUploadModel
//    {
//        #region 外部生成
//        /// <summary>
//        /// ソースを生成する
//        /// 外部用
//        /// </summary>
//        /// <param name="excel">Excel</param>
//        /// <param name="engine">エンジン</param>
//        /// <returns>[Name][生成結果]：Outが無かったらNull</returns>
//        private async Task<Dictionary<string, string>> GenerateOut(Dictionary<string, List<List<string>>> excel, RazorLightEngine engine)
//        {
//            if (excel.ContainsKey("Out"))
//            {
//                // Outの入力を作成する
//                dynamic model;

//                model = RazorHelper.CreateOutModel(excel);

//                var generatedSource = new Dictionary<string, string>();
//                for (int i = 0; i < model.RootList.Count; i++)
//                {
//                    // 変数入れる
//                    model.General.Index = i.ToString();

//                    // テンプレート読み込み
//                    var template = GetTemplate((string)model.RootList[i].RazorTemplate);

//                    // ソース生成
//                    // 同じキーを指定すると登録したスクリプトを使いまわすことが出来るが、何故か2回目以降Unicodeにされるので毎回違うキーを使う。
//                    generatedSource.Add((string)model.RootList[i].Name, await engine.CompileRenderStringAsync($"{model.RootList[i].Name}Out", template, model));
//                }

//                // Outが無くても空ディクショナリを返す
//                return generatedSource;
//            }
//            return null;
//        }

//        #region GetTemplate
//        /// <summary>
//        /// DBからテンプレート取得
//        /// </summary>
//        /// <param name="path">cs_asp/crud/ListBox</param>
//        /// <returns></returns>
//        private string GetTemplate(string path)
//        {
//            // メイン、サブ、ファイルの3つの名前でアクセス
//            var splitedPath = path.Trim('/').Split("/");
//            var razorData = _db.RazorFiles.First(x => x.Name == splitedPath[2] && x.Parent.Name == splitedPath[1] && x.Parent.Parent.Name == splitedPath[0]);
//            var template = string.Empty;
//            using (var stream = new MemoryStream(razorData.Razor))
//            {
//                template = Encoding.UTF8.GetString(stream.ToArray());
//            }
//            return template;
//        }

//        #endregion

//        #endregion

//        /// <summary>
//        /// 生成ボタン
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        public async Task<IActionResult> OnPostGenerateAsync(long id)       // Excelのパス
//        {
//            try
//            {
//                // エンジンを生成
//                var engine = new RazorLightEngineBuilder()
//                          .UseEmbeddedResourcesProject(typeof(RazorService))
//                          .UseMemoryCachingProvider()
//                          .DisableEncoding()        // 日本語がUnicodeにされるのを防止
//                          .Build();

//                // 対象のExcelを読み込む
//                // TODO:Excelのパスをメモリストリームにする
//                var data = _db.ExcelFiles.First(x => x.Id == id);

//                Dictionary<string, List<List<string>>> excel = null;
//                using (var stream = new MemoryStream(data.Xlsx))
//                {
//                    excel = RazorHelper.ReadExcel(stream, true);
//                }

//                // 外部生成
//                var outInput = await GenerateOut(excel, engine);

//                // Modelの作成
//                dynamic model;
//                model = RazorHelper.CreateModel(excel, outInput);

//                // 一時出力先を作成
//                string outPath = Path.Combine(_hostEnvironment.WebRootPath, "temp");
//                RazorHelper.DeleteDirectory(outPath, true); // linuxだとパーミッションが消えてダウンロードできなくなるので最初のフォルダは残す
//                RazorHelper.SafeCreateDirectory(outPath);
//                // 一時ファイル消す
//                RazorHelper.DeleteFiles(outPath);

//                // リストを読んでソース生成する
//                // ↓この"RootList"は動的に変えられないので、ファイル生成の一覧となるListシートの名前は"RootList"固定にする。
//                var outFileList = new List<string>();
//                var razorFileDirectry = Path.Combine(_hostEnvironment.WebRootPath, SystemConstants.FileDirectory, "razors");

//                // 各ソース生成
//                List<dynamic> generatedSource = await GenerateSource(model, engine);

//                for (int i = 0; i < model.RootList.Count; i++)
//                {
//                    // ファイル名生成（絶対かぶらないように）
//                    var resultFilename = await engine.CompileRenderStringAsync(
//                        $"{model.RootList[i].Name}Name{i}",
//                        model.RootList[i].OutputFileName,
//                        new
//                        {
//                            model.RootList[i].Name,
//                            model.RootList[i].Camel,
//                            model.RootList[i].Pascal,
//                            model.RootList[i].Plural,
//                            model.RootList[i].Hyphen,
//                            model.RootList[i].Snake,
//                            model.RootList[i].CamelPlural,
//                            model.RootList[i].PascalPlural
//                        });

//                    // 生成したファイルを一時保存
//                    // VisualStudioが勘違いを起こすのでファイル末尾に"_"をつける
//                    var outFileName = $"{resultFilename}_";
//                    outFileList.Add(outFileName);

//                    // ディレクトリ分けしたZipを作成する
//                    RazorHelper.SafeCreateDirectory(Path.Combine(outPath, Path.GetDirectoryName(outFileName)));
//                    System.IO.File.WriteAllText(Path.Combine(outPath, outFileName), generatedSource[i], Encoding.UTF8);
//                }

//                // 圧縮ファイルの準備
//                string dateFormat = "yyyyMMddHHmmss";
//                string outFilePath = Path.Combine(outPath, $"{DateTime.UtcNow.ToString(dateFormat)}.zip");
//                // 一時保存したファイルをZipにする
//                using (ZipArchive archive = ZipFile.Open(outFilePath, ZipArchiveMode.Create))
//                {
//                    foreach (var item in outFileList)
//                    {
//                        archive.CreateEntryFromFile(
//                            Path.Combine(outPath, $"{item}"),
//                            $"{item.TrimEnd('_')}", // ここでスラッシュを入れると、ディレクトリ分けしたZipが作成できる
//                            CompressionLevel.NoCompression
//                        );
//                    }
//                }

//                return File(new FileStream(outFilePath, FileMode.Open), "application/zip", $"{data.RawFileName}.zip");
//            }
//            catch (Exception e)
//            {
//                ViewData["Error"] = e.Message;
//                return await OnGetAsync();
//            }
//        }

//        /// <summary>
//        /// ソースを生成する
//        /// </summary>
//        /// <param name="model">モデル</param>
//        /// <param name="engine">エンジン</param>
//        /// <returns></returns>
//        private async Task<List<dynamic>> GenerateSource(dynamic model, RazorLightEngine engine)
//        {
//            var generatedSource = new List<dynamic>();
//            for (int i = 0; i < model.RootList.Count; i++)
//            {
//                // 変数入れる
//                model.General.Index = i.ToString();

//                // テンプレート読み込み
//                var template = GetTemplate((string)model.RootList[i].RazorTemplate);

//                // ソース生成
//                // 同じキーを指定すると登録したスクリプトを使いまわすことが出来るが、何故か2回目以降Unicodeにされるので毎回違うキーを使う。
//                var name = $"{model.RootList[i].Name + i}";
//                try
//                {
//                    generatedSource.Add(await engine.CompileRenderStringAsync(name, template, model));
//                }
//                catch (Exception e)
//                {
//                    // 生成で何かエラーがあったらここ。
//                    throw e;
//                }
//            }

//            return generatedSource;
//        }

//    }


//    ///// <summary>
//    ///// Excelを読み込む
//    ///// </summary>
//    ///// <param name="stream"></param>
//    ///// <returns>Excelシートの内容：シート、行、列</returns>
//    //private ExcelUploadResult ReadExcel(Stream stream)
//    //{
//    //    // ファイルの読み込み
//    //    List<string> sheetNames = new List<string>();
//    //    List<List<List<string>>> xlsx = new List<List<List<string>>>();

//    //    var excel = RazorHelper.ReadExcel(stream, false);

//    //    foreach (var sheet in excel.Values)
//    //    {
//    //        xlsx.Add(sheet);
//    //    }
//    //    foreach (var sheetName in excel.Keys)
//    //    {
//    //        sheetNames.Add(sheetName);
//    //    }

//    //    return new ExcelUploadResult
//    //    {
//    //        RawExcel = xlsx,
//    //        SheetNames = sheetNames
//    //    };
//    //}
//    #endregion

//}