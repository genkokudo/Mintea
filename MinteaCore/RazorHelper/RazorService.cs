using MithrilCube.Data;
using MithrilCube.Services;
using RazorLight;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Globalization;
using MithrilCube.Extensions;
using Microsoft.Extensions.Options;

// ■注意■
// このサービスを使用する場合、利用するプロジェクト（モジュールではなくその大元のプロジェクト）の.csprojに以下を追加すること。
//<PropertyGroup>
//   <PreserveCompilationContext>true</PreserveCompilationContext>
//</PropertyGroup>

namespace MinteaCore.RazorHelper
{
    public interface IRazorService
    {
        /// <summary>
        /// 旧プログラムで生成ボタンを押したときの動作
        /// </summary>
        /// <param name="path">Excelのパス</param>
        /// <param name="baseDirectory">exe実行ディレクトリ</param>
        /// <param name="outDirectory">出力先フォルダ</param>
        /// <returns></returns>
        public Task GenerateAsync(string path, string baseDirectory, string outDirectory);
    }

    public class RazorServiceOptions
    {
        /// <summary>
        /// 文書用シート
        /// "Doc"が最初についているシートはRazor生成時は読まない
        /// </summary>
        public string DocumentSheetPrefix { get; set; } = "Doc";

        /// <summary>
        /// 必須シート名
        /// この名前のシートは必ず含んでいる必要があり、グローバル的な変数・設定値などを列挙する
        /// </summary>
        public string SettingsSheet { get; set; } = "Settings";

        /// <summary>
        /// 必須シート名
        /// </summary>
        public string RootListSheet { get; set; } = "RootList";

        /// <summary>
        /// 共通変数の添え字項目名
        /// OutGeneral.Index=0
        /// General.Index=0
        /// </summary>
        public string IndexValue { get; set; } = "Index";

        /// <summary>
        /// "Is"が最初についている列は0,1,型文字列をbool型とする
        /// </summary>
        public string BoolCulumnPrefix { get; set; } = "Is";

        /// <summary>
        /// "Out"は外部入力
        /// </summary>
        public string OutSheet { get; set; } = "Out";
        /// <summary>
        /// "OutList"が最後についているシートは外部入力
        /// </summary>
        public string OutListSheetSuffix { get; set; } = "OutList";

        /// <summary>
        /// "List"が最後についているシートはリスト
        /// </summary>
        public string ListSheetSuffix { get; set; } = "List";

        /// <summary>
        /// キー情報を格納する列
        /// </summary>
        public string KeyCulumn { get; set; } = "Key";

        /// <summary>
        /// 親シートを格納する列
        /// </summary>
        public string ParentCulumn { get; set; } = "Parent";

        /// <summary>
        /// Camel, Pascal, Pluralを用意する列
        /// CamelPlural
        /// PascalPlural
        /// </summary>
        public string InflectCulumn { get; set; } = "Name";
        public string Camel { get; set; } = "Camel";
        public string Pascal { get; set; } = "Pascal";
        public string Plural { get; set; } = "Plural";  // 複数形
        public string Hyphen { get; set; } = "Hyphen";
        public string Snake { get; set; } = "Snake";
        public string CamelPlural { get; set; } = "CamelPlural";
        public string PascalPlural { get; set; } = "PascalPlural";
    }

    public class RazorService : IRazorService
    {
        /// <summary>
        /// 共通変数名：変更不可
        /// </summary>
        const string General = "General";

        private readonly ExcelService _excelService;
        private readonly DirectoryService _directoryService;
        private readonly RazorServiceOptions _options;
        public RazorService(ExcelService excelService, DirectoryService directoryService, IOptions<RazorServiceOptions> options)
        {
            _excelService = excelService;
            _directoryService = directoryService;
            _options = options.Value;
        }

        /// <summary>
        /// "Out"という名前のシートについてソースを生成する
        /// 出力結果はRazor内に@Model.Out[Name]という形で代入できる
        /// 
        /// これによって他のRazorで出力した結果をRazorに適用できる。
        /// </summary>
        /// <param name="excel">Excel</param>
        /// <param name="engine">エンジン</param>
        /// <returns>[Name][生成結果]：Outが無かったらNull</returns>
        private async Task<Dictionary<string, string>> GenerateOut(Dictionary<string, List<List<string>>> excel, RazorLightEngine engine)
        {
            if (excel.ContainsKey("Out"))
            {
                // "Out"シートの入力を作成する
                dynamic model = CreateOutModel(excel);

                var generatedSource = new Dictionary<string, string>();
                for (int i = 0; i < model.RootList.Count; i++)
                {
                    // 変数入れる
                    model.General.Index = i.ToString();

                    // テンプレート読み込み
                    var template = GetTemplate((string)model.RootList[i].RazorTemplate);

                    // ソース生成
                    // 同じキーを指定すると登録したスクリプトを使いまわすことが出来るが、何故か2回目以降Unicodeにされるので毎回違うキーを使う。
                    generatedSource.Add((string)model.RootList[i].Name, await engine.CompileRenderStringAsync($"{model.RootList[i].Name}Out", template, model));
                }

                // Outが無くても空ディクショナリを返す
                return generatedSource;
            }
            return null;
        }

        /// <summary>
        /// Razor読み込み用
        /// Excelに記述しているパスのテキストを読み込む
        /// "./Razor"以下のパスで書いて、拡張子抜きという特殊形式にしているので注意
        /// </summary>
        /// <param name="pathStr">CSharp\WPF\SampleWpf</param>
        /// <returns>ファイルの内容</returns>
        private string GetTemplate(string pathStr)
        {
            var path = Path.Combine("./Razor", $"{pathStr}.razor");
            if (!File.Exists(path))
            {
                throw new Exception($"必要なファイルが見つかりませんでした。{path}");
            }
            return File.ReadAllText(path);
        }

        // TODO:改善の余地あり
        /// <summary>
        /// Excelの各シートのParent列で指定されている親リストを作成する
        /// ある子要素は、親無しまたは1種類の親を持つ前提のデータ構造である
        /// 親要素はシート内のParent列で判断する
        /// </summary>
        /// <param name="excel"></param>
        /// <param name="errors"></param>
        /// <returns>各要素の親がどの要素かを挙げたリスト</returns>
        private Dictionary<string, string> MakeParentList(Dictionary<string, List<List<string>>> excel, List<string> errors)
        {
            // TODO:Parentに書かれている親は全て同じでなければならないので、シート名をRootList.FieldListという風に指定する方が良い気がする。
            var parentList = new Dictionary<string, string>();

            foreach (var sheetName in excel.Keys)
            {
                // リスト
                var sheet = excel[sheetName];

                // Parentの列番号を取得
                var parentIndex = _excelService.GetIndex(sheet, _options.ParentCulumn);

                // Parentがある場合
                if (parentIndex >= 0)
                {
                    for (int i = 2; i < sheet.Count; i++)
                    {
                        if (!sheet[i][parentIndex].Contains("."))
                        {
                            errors.Add($"{_options.ParentCulumn}に'.'が入ってない。sheet:{sheetName} row:{i} column:{parentIndex} value:{sheet[i][parentIndex]}");
                        }
                        else
                        {
                            // 子情報を登録する
                            var splited = sheet[i][parentIndex].Split('.');

                            if (parentList.Keys.Contains(sheetName))
                            {
                                if (parentList[sheetName] != splited[0])
                                {
                                    errors.Add($"同じ{_options.ParentCulumn}列に違う親が書かれている。sheet:{sheetName} row:{i} column:{parentIndex} value:{sheet[i][parentIndex]}");
                                }
                            }
                            else
                            {
                                parentList.Add(sheetName, splited[0]);
                            }
                        }
                    }
                }
            }
            return parentList;
        }

        public async Task GenerateAsync(string path, string baseDirectory, string outDirectory)
        {
            // エンジンを生成
            var engine = new RazorLightEngineBuilder()
                      .UseEmbeddedResourcesProject(typeof(RazorService))
                      .UseMemoryCachingProvider()
                      .DisableEncoding()        // 日本語がUnicodeにされるのを防止
                      .Build();

            // 対象のExcelを読み込む
            var excel = _excelService.ReadExcel(path, true);

            // 外部生成：Razorに他のRazorを代入するために、先に生成しておく
            var outInput = await GenerateOut(excel, engine);

            // Modelの作成
            dynamic model = CreateModel(excel, outInput);

            // 出力先を作成
            var outPath = Path.Combine(baseDirectory, "./temp");
            if (Directory.Exists(outPath))
            {
                Directory.Delete(outPath, true);
            }
            _directoryService.SafeCreateDirectory(outPath);

            // リストを読んでソース生成する
            // ↓この"RootList"は動的に変えられないので、ファイル生成の一覧となるListシートの名前は"RootList"固定にする。
            var outFileList = new List<string>();

            // 各ソース生成
            var generatedSource = await GenerateSource(model, engine);

            for (int i = 0; i < model.RootList.Count; i++)
            {
                // ファイル名生成（絶対かぶらないように）
                var resultFilename = await engine.CompileRenderStringAsync(
                    $"{model.RootList[i].Name}Name{i}",
                    model.RootList[i].OutputFileName,
                    new
                    {
                        model.RootList[i].Name,
                        model.RootList[i].Camel,
                        model.RootList[i].Pascal,
                        model.RootList[i].Plural,
                        model.RootList[i].Hyphen,
                        model.RootList[i].Snake,
                        model.RootList[i].CamelPlural,
                        model.RootList[i].PascalPlural
                    });

                // 生成したファイルを一時保存
                // VisualStudioが勘違いを起こすのでファイル末尾に"_"をつける
                var outFileName = $"{resultFilename}_";
                outFileList.Add(outFileName);

                // ディレクトリ分けしたZipを作成する
                _directoryService.SafeCreateDirectory(Path.Combine(outPath, Path.GetDirectoryName(outFileName)));
                File.WriteAllText(Path.Combine(outPath, outFileName), generatedSource[i], Encoding.UTF8);
            }

            // 圧縮ファイルの準備
            var dateFormat = "yyyyMMddHHmmss";
            var fileName = $"{DateTime.UtcNow.ToString(dateFormat)}.zip";
            var outFilePath = Path.Combine(outPath, fileName);
            // 一時保存したファイルをZipにする
            using (ZipArchive archive = ZipFile.Open(outFilePath, ZipArchiveMode.Create))
            {
                foreach (var item in outFileList)
                {
                    archive.CreateEntryFromFile(
                        Path.Combine(outPath, $"{item}"),
                        $"{item.TrimEnd('_')}", // ここでスラッシュを入れると、ディレクトリ分けしたZipが作成できる
                        CompressionLevel.NoCompression
                    );
                }
            }

            // 出力と一時フォルダの削除
            File.Move(outFilePath, Path.Combine(outDirectory, fileName));
            Directory.Delete(outPath, true);
        }

        /// <summary>
        /// Razorとモデルデータからソースを生成する
        /// </summary>
        /// <param name="model">CreateModelでExcelから作成したモデル</param>
        /// <param name="engine">エンジン</param>
        /// <returns></returns>
        private async Task<List<dynamic>> GenerateSource(dynamic model, RazorLightEngine engine)
        {
            var generatedSource = new List<dynamic>();
            for (int i = 0; i < model.RootList.Count; i++)
            {
                // 変数入れる
                model.General.Index = i.ToString();

                // テンプレート読み込み
                var template = GetTemplate((string)model.RootList[i].RazorTemplate);

                // ソース生成
                // 同じキーを指定すると登録したスクリプトを使いまわすことが出来るが、何故か2回目以降Unicodeにされるので毎回違うキーを使う。
                var name = $"{model.RootList[i].Name + i}";
                generatedSource.Add(await engine.CompileRenderStringAsync(name, template, model));
            }
            return generatedSource;
        }

        #region private

        /// <summary>
        /// 生成する時は親シートより先に子シートを処理しなければならないので、
        /// 子シート優先になるようにシートの順番を作成する
        /// </summary>
        /// <param name="excel">Excelデータ</param>
        /// <param name="errors">エラーを格納するところ</param>
        /// <returns>シートの順番が書かれたList</returns>
        private List<string> MakeSequence(Dictionary<string, List<List<string>>> excel, List<string> errors)
        {
            // 親リスト作成
            var parentList = MakeParentList(excel, errors);

            // 名前リスト作成の為の意味のないリスト
            // （本来は木構造に持たせるデータを入れることができるが、今回は順番が欲しいだけなので名前だけ持たせる）
            var nameList = new Dictionary<string, string>();
            foreach (var item in excel)
            {
                nameList.Add(item.Key, item.Key);
            }

            // 木構造作成
            var tree = TreeNode<string>.MakeTree(new TreeNode<string>(string.Empty), nameList, parentList);

            // 深さ優先探索
            var result = new List<string>();
            var resultTreeList = tree.DepthList();

            // 名前を取り出す
            foreach (var item in resultTreeList)
            {
                if (item.Value != string.Empty)
                {
                    result.Add(item.Value);
                }
            }

            return result;
        }

        /// <summary>
        /// 0や空白、Falseをfalseに
        /// 1やTrueをtrueに変換する
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private bool ToBool(string val)
        {
            if (string.IsNullOrWhiteSpace(val) || val.ToLower() == "false" || int.TryParse(val, out int intval) && intval <= 0)
            {
                return false;
            }
            else if (val.ToLower() == "true" || int.TryParse(val, out intval) && intval > 0)
            {
                return true;
            }
            else
            {
                throw new Exception($"これはboolにできない:{val}");
            }
        }

        /// <summary>
        /// Excelから取得した全ての子情報の中に、対象のシートに関連するものがあれば
        /// そのシートの行データに子データを追加する
        /// </summary>
        /// <param name="childList">子情報</param>
        /// <param name="children">子シートデータ</param>
        /// <param name="sheetName">親シート名</param>
        /// <param name="key">親キー</param>
        /// <param name="rowData">追加対象行データ</param>
        private void AddChildDynamic(Dictionary<string, List<string>> childList, Dictionary<string, Dictionary<string, Dictionary<string, dynamic>>> children, string sheetName, string key, Dictionary<string, object> rowData)
        {
            // 子を追加
            // Key列があるListシートの場合、子dynamicデータを探して保持する。
            if (childList.ContainsKey(sheetName))
            {
                var childNames = childList[sheetName];
                foreach (var childName in childNames)
                {
                    if (children.ContainsKey(sheetName) && children[sheetName].ContainsKey(key) && children[sheetName][key].ContainsKey(childName))
                    {
                        rowData.Add(childName, children[sheetName][key][childName]);
                    }
                    else
                    {
                        // 子情報があるのにキーに対する子データはなかった場合は、空データを作っておくべき。
                        rowData.Add(childName, new List<string>());
                    }
                }
            }
        }

        /// <summary>
        /// 各シートについて、各シートがどのシートを子としているかの情報を作成します。
        /// </summary>
        /// <param name="excel">Excelデータ</param>
        /// <param name="errors">エラーリスト</param>
        /// <returns>0:親Sheet名、1:子Sheet名重複なし</returns>
        private Dictionary<string, List<string>> MakeChildData(Dictionary<string, List<List<string>>> excel, List<string> errors)
        {
            var childList = new Dictionary<string, List<string>>();

            foreach (var sheetName in excel.Keys)
            {
                if (sheetName.EndsWith(_options.ListSheetSuffix))    // OutList含む
                {
                    // リスト
                    var sheet = excel[sheetName];
                    var parentIndex = _excelService.GetIndex(sheet, _options.ParentCulumn);
                    if (sheet.Count > 2)
                    {
                        // Parentがある場合
                        if (parentIndex > 0)
                        {
                            for (int i = 2; i < sheet.Count; i++)
                            {
                                if (!sheet[i][parentIndex].Contains("."))
                                {
                                    errors.Add($"{_options.ParentCulumn}に'.'が入ってない。sheet:{sheetName} row:{i} column:{parentIndex} value:{sheet[i][parentIndex]}");
                                }
                                else
                                {
                                    // 子情報を登録する
                                    var splited = sheet[i][parentIndex].Split('.'); // 親名、親キー

                                    // シートが未登録ならば追加
                                    childList.NewListIfNotExists(splited[0]);

                                    // 今回はキーまで載せない。
                                    // 重複して同じ名前が入らないようにする。（キーも格納する場合は別だが。）
                                    childList[splited[0]].AddIfNotExists(sheetName);
                                }
                            }
                        }
                    }
                }
            }
            return childList;
        }

        #region CreateModel:Razorに入力するModelを作成する
        /// <summary>
        /// Razorに入力するModelを作成する
        /// 外部ソース用なので、OutListだけ
        /// </summary>
        /// <returns></returns>
        private dynamic CreateOutModel(Dictionary<string, List<List<string>>> excel)
        {
            var errors = new List<string>();

            // データ作成順を決める
            var sequence = MakeSequence(excel, errors);

            // 親を持たない各シートのデータ：キーはシート名
            var topDataList = new Dictionary<string, dynamic>();

            // TODO:多分MakeListModelに移動させればよい。
            // 各シートについて、各シートがどのシートを子としているかの情報を作成します。
            var childList = MakeChildData(excel, errors);

            // 子シートデータ
            var childDynamic = new Dictionary<string, Dictionary<string, Dictionary<string, dynamic>>>();

            foreach (var sheetName in sequence)
            {
                if (sheetName.StartsWith(_options.DocumentSheetPrefix))
                {
                    // なにもなし
                    continue;
                }

                // 1つのシート
                var sheet = excel[sheetName];

                // Razorの互換性を持たせるために名前を変更して扱う
                if (sheetName == _options.OutSheet)
                {
                    // OutはRootListとして登録
                    MakeListModel(errors, topDataList, childList, childDynamic, _options.RootListSheet, sheet, string.Empty);
                }
                else if (sheetName.EndsWith(_options.OutListSheetSuffix))
                {
                    // OutListはListとして登録
                    MakeListModel(errors, topDataList, childList, childDynamic, sheetName.Replace(_options.OutListSheetSuffix, _options.ListSheetSuffix), sheet, string.Empty);
                }
            }
            if (errors.Count > 0)
            {
                throw new Exception($"Excelの内容がおかしい:\n{string.Join(",\n", errors)}");
            }

            // 共通変数としてOutGeneral.Index=0を入れる
            var generalData = new Dictionary<string, object>
            {
                { "Index", "0" }
            };
            topDataList.Add("General", generalData.ToDynamic());

            var result = topDataList.ToDynamic();

            return result;
        }

        /// <summary>
        /// Razorに入力するModelを作成する
        /// </summary>
        /// <param name="excel">Excelデータ</param>
        /// <param name="outScript"></param>
        /// <returns></returns>
        private dynamic CreateModel(Dictionary<string, List<List<string>>> excel, Dictionary<string, string> outScript = null)
        {
            var errors = new List<string>();

            // データ作成順を決める
            var sequence = MakeSequence(excel, errors);

            // 親を持たない各シートのデータ：キーはシート名
            var topDataList = new Dictionary<string, dynamic>();

            // 外部スクリプトの生成結果を追加
            if (outScript != null)
            {
                var outData = new Dictionary<string, object>();
                foreach (var script in outScript)
                {
                    outData.Add(script.Key, script.Value);
                }
                topDataList.Add(_options.OutSheet, outData.ToDynamic());
            }

            // 各シートについて、各シートがどのシートを子としているかの情報を作成します。
            var childList = MakeChildData(excel, errors);

            // 子シートデータ
            var childDynamic = new Dictionary<string, Dictionary<string, Dictionary<string, dynamic>>>();

            // 子シートから順番にデータ作成
            var isRequiredSheetExists = false;
            var isRootListExists = false;
            foreach (var sheetName in sequence)
            {
                // 1つのシート
                var sheet = excel[sheetName];


                // 親シート名
                var parentName = string.Empty;

                // キー取得
                var keyIndex = _excelService.GetIndex(sheet, _options.KeyCulumn);

                if (sheetName.StartsWith(_options.DocumentSheetPrefix))
                {
                    // なにもなし
                    continue;
                }
                else if (sheetName.EndsWith(_options.OutListSheetSuffix) || sheetName == _options.OutSheet)
                {
                    // OutList:外部入力リスト
                    // なにもなし
                    continue;
                }
                else if (sheetName.EndsWith(_options.ListSheetSuffix))
                {
                    // List:通常リスト
                    // 必須シート存在チェック
                    if (sheetName == _options.RootListSheet)
                    {
                        isRootListExists = true;
                    }
                    parentName = MakeListModel(errors, topDataList, childList, childDynamic, sheetName, sheet, parentName);
                }
                else
                {
                    // 必須シート存在チェック
                    if (sheetName == _options.SettingsSheet)
                    {
                        isRequiredSheetExists = true;
                    }

                    // 今回、通常シートは親子関係を持たない。持たせたい場合はListシートと同様に実装すればできるはず。
                    // …というか、リストの下位互換かも。通常シート不要説。
                    // 通常シート：1列目が名前、2列目が値
                    if (sheet.Count > 2)
                    {
                        var data = new Dictionary<string, object>();
                        // 2行目まで読まない
                        for (int row = 2; row < sheet.Count; row++)
                        {
                            var name = sheet[row][0];
                            // 必須項目チェック

                            var value = sheet[row][1];
                            if (name.StartsWith(_options.BoolCulumnPrefix))
                            {
                                // bool型判定
                                var val = sheet[row][1];
                                try
                                {
                                    data.Add(name, ToBool(sheet[row][1]));
                                }
                                catch (Exception)
                                {
                                    errors.Add($"{_options.BoolCulumnPrefix}で始まってる項目なのにboolにできない。sheet:{sheetName} row:{row} value:{val}");
                                }
                            }
                            else
                            {
                                data.Add(name, value);
                            }
                        }
                        // 親がないのでトップデータリストに追加
                        topDataList.Add(sheetName, data.ToDynamic());
                    }
                }
            }
            if (!isRootListExists)
            {
                errors.Add($"{_options.RootListSheet}という名前のシートがない。");
            }
            if (!isRequiredSheetExists)
            {
                errors.Add($"{_options.SettingsSheet}という名前のシートがない。");
            }
            if (errors.Count > 0)
            {
                throw new Exception($"Excelの内容がおかしい:\n{string.Join(",\n", errors)}");
            }

            // 共通変数としてGeneral.Index=0を入れる
            var generalData = new Dictionary<string, object>
                {
                    { "Index", "0" }
                };
            topDataList.Add("General", generalData.ToDynamic());

            var result = topDataList.ToDynamic();

            return result;
        }
        #endregion


        /// <summary>
        /// 子シートのデータを親のキー別にdynamic化したものを格納して保持、親dynamic生成時にここから取得する。
        /// Listではない。既にdynamicでまとまったデータを格納している。1つの親キーに1件のみ。
        /// </summary>
        /// <param name="children">子データを格納する配列</param>
        /// <param name="parentName">親の名前</param>
        /// <param name="parentKey">親のキー</param>
        /// <param name="childSheetName">子の名前( = 親のフィールド名)</param>
        private void AddChildrenData(Dictionary<string, Dictionary<string, Dictionary<string, dynamic>>> children, string parentName, string parentKey, string childSheetName, dynamic data)
        {
            children.NewListIfNotExists(parentName);
            children[parentName].NewListIfNotExists(parentKey);
            children[parentName][parentKey].NewDictionaryIfNotExists(childSheetName);
            children[parentName][parentKey][childSheetName] = data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="errors">エラーを溜めておくリスト</param>
        /// <param name="topDataList">親を持たない各シートのデータ：キーはシート名</param>
        /// <param name="childList">シートに対して、どのシートが子シートかの情報</param>
        /// <param name="childDynamic">子要素をdynamic形式にしたもの</param>
        /// <param name="sheetName">処理対象のシート名</param>
        /// <param name="sheet">処理対象のシートデータ</param>
        /// <param name="parentName">親の名前（親がある場合のみ）</param>
        /// <returns></returns>
        private string MakeListModel(List<string> errors, Dictionary<string, dynamic> topDataList, Dictionary<string, List<string>> childList, Dictionary<string, Dictionary<string, Dictionary<string, dynamic>>> childDynamic, string sheetName, List<List<string>> sheet, string parentName)
        {
            // PascalCaseなどの形式変換するオブジェクト
            var inf = new Inflector.Inflector(new CultureInfo("en-US"));

            // キー列取得
            var keyIndex = _excelService.GetIndex(sheet, _options.KeyCulumn);

            // 親があるか
            var parentIndex = _excelService.GetIndex(sheet, _options.ParentCulumn);

            // リスト
            if (sheet.Count > 2)
            {
                // 親があってもなくてもトップからデータアクセス
                var topData = new List<dynamic>();

                // 親Key別データ、親が無い場合はシート全体のデータ（親キー、1行データ）
                var dataByParent = new Dictionary<string, List<dynamic>>();

                // 2行目まで読まない
                for (int row = 2; row < sheet.Count; row++)
                {
                    // 1行読む
                    var parentKeys = new List<string>();   // 対象の親のKey
                    if (parentIndex == -1)
                    {
                        // 親がない場合は-1に格納する
                        parentKeys.Add("-1");
                    }

                    var rowData = new Dictionary<string, object>();
                    for (int col = 0; col < sheet[row].Count; col++)
                    {
                        // 列を読む
                        if (col == parentIndex)
                        {
                            // 親参照はdynamicデータに登録しないが、子dynamicデータリストに保持させるための情報を取得する
                            var split = sheet[row][col].Split('.');
                            parentName = split[0];
                            for (int i = 1; i < split.Length; i++)
                            {
                                parentKeys.Add(split[i]);
                            }
                        }
                        else if (col == keyIndex)
                        {
                            // キー列の場合、子を追加
                            var key = sheet[row][col];  // 書かれているKeyを取得
                            AddChildDynamic(childList, childDynamic, sheetName, key, rowData);
                        }
                        else if (sheet[0][col].StartsWith(_options.BoolCulumnPrefix))
                        {
                            // Isならば、bool型判定
                            var val = sheet[row][col];
                            try
                            {
                                rowData.Add(sheet[0][col], ToBool(sheet[row][col]));
                            }
                            catch (Exception)
                            {
                                errors.Add($"{_options.BoolCulumnPrefix}で始まってる項目なのにboolにできない。sheet:{sheetName} row:{row} column:{col} value:{val}");
                            }
                        }
                        else if (sheet[0][col].EndsWith(_options.InflectCulumn))
                        {
                            // 語尾がNameならば、フィールドを余分に作る
                            var baseName = sheet[0][col].Remove(sheet[0][col].LastIndexOf(_options.InflectCulumn), _options.InflectCulumn.Length);
                            rowData.Add(sheet[0][col], sheet[row][col]);
                            rowData.Add(baseName + _options.Camel, inf.Camelize(sheet[row][col]));
                            rowData.Add(baseName + _options.Pascal, inf.Pascalize(sheet[row][col]));
                            rowData.Add(baseName + _options.Plural, inf.Pluralize(sheet[row][col]));
                            rowData.Add(baseName + _options.CamelPlural, inf.Camelize(inf.Pluralize(sheet[row][col])));
                            rowData.Add(baseName + _options.PascalPlural, inf.Pascalize(inf.Pluralize(sheet[row][col])));
                            rowData.Add(baseName + _options.Snake, inf.Underscore(sheet[row][col]));
                            rowData.Add(baseName + _options.Hyphen, inf.Underscore(sheet[row][col]).Replace('_', '-'));
                        }
                        else
                        {
                            // ParentでもKeyでもない通常の列
                            rowData.Add(sheet[0][col], sheet[row][col]);
                        }
                    }

                    // 行データをdynamic化し、親Key別のリストに追加する。
                    // 親がないシートは必ず1件の同じリストに入る（parentKeyは"-1"）
                    foreach (var parentKey in parentKeys)
                    {
                        if (!dataByParent.ContainsKey(parentKey))
                        {
                            dataByParent.Add(parentKey, new List<dynamic>());
                        }
                        dataByParent[parentKey].Add(rowData.ToDynamic());
                    }
                    topData.Add(rowData.ToDynamic());
                }

                // 親Key別のリストをどこかに登録する。
                foreach (var dataByParentKey in dataByParent.Keys)
                {
                    if (parentIndex >= 0)
                    {
                        // 親がある場合は、データを溜めておく
                        AddChildrenData(childDynamic, parentName, dataByParentKey, sheetName, dataByParent[dataByParentKey]);

                        // 親子でなくてもModelからListにアクセスできるようにするため
                        // 親が無いシートと同様にトップにもデータを入れる
                        if (!topDataList.ContainsKey(sheetName))
                        {
                            topDataList.Add(sheetName, topData);
                        }
                    }
                    else
                    {
                        // 親が無いシートはトップにデータを入れる
                        topDataList.Add(sheetName, dataByParent[dataByParentKey]);
                    }
                }
            }

            return parentName;
        }


        ///// <summary>
        ///// シート内の指定した列の番号を取得する
        ///// </summary>
        ///// <param name="sheet"></param>
        ///// <returns>なかったら-1</returns>
        //private int GetIndex(List<List<string>> sheet, string name)
        //{
        //    var result = -1;

        //    if (sheet.Count > 2)
        //    {
        //        for (int i = 0; i < sheet[0].Count; i++)
        //        {
        //            if (sheet[0][i] == name)
        //            {
        //                return i;
        //            }
        //        }
        //    }
        //    return result;
        //}
        #endregion
    }
}
