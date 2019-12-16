﻿


// -----------------------------------------------------
//			// 各マスタの選択項目
//			public Dictionary<string, SelectListItem[]> Lists { get; set; }

//				// 各マスタを選択項目にする
//				var status = _db.Statuses.OrderBy(x => x.StatusId).AsNoTracking().ToArray();
//				var statusSelectList = new List<SelectListItem> { new SelectListItem("--- 未選択 ---", "-1") };
//				foreach (var item in status)
//				{
//					statusSelectList.Add(new SelectListItem(item.Name, item.StatusId.ToString()));
//				}

//				var lists = new Dictionary<string, SelectListItem[]>();
//				lists.Add("Status", statusSelectList.ToArray());


//		// 選択肢作成
//        var _lists = @Html.Raw(JsonConvert.SerializeObject(Model.Data.Lists));

//            var select = _lists[parameterName];
//			select.forEach(function (value) {
//                tbody += '                <option value="' + value.Value + '">' + value.Text + '</option>';
//			});
// -----------------------------------------------------

//// TODO:サーバ側
//// 値読み込み時にEnumを選択肢として扱う方法を書く
//// 例になるViewModelを書く


//			/// <summary>
//			/// クライアント側で文字列を使用するため
//			/// Enumを辞書にする
//			/// </summary>
//			/// <typeparam name="T">Enum名</typeparam>
//			/// <returns>Enum辞書</returns>
//			private static Dictionary <int, string> EnumToDictionary<T>()
//            {
//                var values = Enum.GetValues(typeof (T)).Cast<T>();
//                var dict = values.ToDictionary(e => Convert.ToInt32(e), e => e.ToString());
//                return dict;
//            }

//// Enum
//var _filterModes = @Html.Raw(JsonConvert.SerializeObject(Model.Data.FilterModes));

//filter.FilterColors.forEach(function (value) {
//    AddColor(value.FilterColorId, value.Color, value.Value, value.IsActive, _filterModes[value.FilterMode]);
//});

// html_table //

//// 【注意事項】
//// ・削除方法
//// データを歯抜けにするとサーバ処理できないので行は消さずに非表示にする
//// 削除していることを示すため空文字や-1を設定する
//// ・name
//// サーバのモデルとの紐づけに使うので、好きな名前を付けられない
//// ・trigger
//// nameの代わりに使用すると良い、ここにjs処理を紐づける
//// ・long型データ
//// submitが飛ばなくなるので空文字列禁止

//// イベント発火がある要素にkeyを付与、値操作がある要素にid付与
//// nameは決まったものしか付与してはならない。それ以外の要素は自由（"key"とか"data-***"とか）


//<button id="ButtonAddData" type="button" class="btn btn-success btm-sm float-right">追加</button>
//    <table class="table table-sm table-hover" id="TableData">
//        <thead>
//            <tr>
//                <th>普通の入力</th>
//                <th>選択肢</th>
//                <th>削除</th>
//            </tr>
//        </thead>
//    </table>

//// 追加ボタン
//$('#ButtonAddData').on('click', function () {
//        var count = $('#TableData').children('tbody').length;

//        // 要素を追加
//        var tbodyData = '<tbody id="TbodyData' + count + '">';
//        tbodyData += '    <tr>';
//        tbodyData += '        <td><input id="InputDataId' + count + '" type="text" name="Input.Datas[' + count + '].Id" class="form-control" placeholder="サンプル入力" value="初期値" /></td>';
//        tbodyData += '        <td><select id="InputDataFunction' + count + '" name="Input.Datas[' + count + '].Function" class="form-control" trigger="InputDataFunction" >';
//        tbodyData += '                <option value="0">None</option>';
//        tbodyData += '                <option value="1">GenerateSwitchCases</option>';
//        tbodyData += '                <option value="2">ClassName</option>';
//        tbodyData += '                <option value="3">SimpleTypeName</option>';
//        tbodyData += '            </select>';
//        tbodyData += '        </td>';
//        tbodyData += '        <td>';
//        tbodyData += '            <button data-index="' + count + '" name="ButtonDeleteData" type="button" class="btn btn-danger">×</button>';
//        tbodyData += '        </td>';
//        tbodyData += '    </tr>';
//        tbodyData += '</tbody>';

//        $('#TableData')
//            .append(tbodyData);
//    });

//// 削除ボタン
//// データが歯抜けになるとサーバに送った時そこで配列が切れてしまう
//// 編集のことも考慮すると削除フラグ入れて非表示にするのが良い
//$(document).on('click', 'button[name=ButtonDeleteData]', function () {
//    var index = $(this).data('index');
//    $('#TbodyData' + index).addClass('d-none');
//    $('#InputDataId' + index).val('');  // 空文字列で削除したことにする
//});

// TODO:サーバ側
//// Input.Data.Idが""だったらスルー
//Input.Data?.RemoveAll(x => string.IsNullOrWhiteSpace(x.Id));

// チェックボックスにページモデルを連動させると、チェック入れたときにSubmitできない件を反映させること。

// -----------------------------------------------------

// js_ajax //
// TODO:以下の2つを作ること
// データ送信版：jQueryのやり方 js_ajax_data
// ファイルアップロード版：ソース js_ajax_upload

var formData = new FormData();
// 全てのアップロードファイルを取得
$("input[name=file]").each(function (index, element) {
    var file = element.files[0];
    if (file != undefined) {
        formData.append("uploadFiles", file, file.name);
        formData.append("Ids", element.getAttribute("id"));
    }
});
$.ajax({
    type: 'POST',
    url: '/Pdf/Upload', // TODO:コントローラ名とハンドラ名を設定
    contentType: false,
    processData: false,
    data: formData
}).then(
    function (data) {
        alert("更新が完了しました");
    },
    function (data) {
        alert("更新に失敗しました:" + data);
    }
);

// サーバ側
//        public async Task<IActionResult> UploadAsync(List<IFormFile> uploadFiles, List<long> Ids)
//        {
//        }

//// ※<input" type="file"/>を書かないとアップロードはできない。
//// でも、ファイル選択のボタンデザインを変えたい場合↓
////<button name="BtnUpload" type="button" class="btn btn-primary">×</button>
////<input type="file" name="file" id="file" value="" class="d-none" accept="application/pdf" />

//// 画面に表示しているアップロードボタンのイベント
//$(document).on("click", "button[name=BtnUpload]", function () {
//    $("#file").click();
//    return false;	// 無いと遷移してしまう
//});

//// 何かファイルを選択したときの処理
//$(document).on("change", "input[name=file]", function () {
//    var file = $(this)[0].files[0];
//    var fileName = file.name;
//    var fileSize = file.size;
//    var fileType = file.type;
//    alert('ファイル名 : ' + fileName + '\nファイルサイズ : ' + fileSize + ' bytes\nファイルタイプ : ' + fileType);
//});

// -----------------------------------------------------


・js_foreach
・js_jquery_each

$('#TableColorPoint').find('[id^=InputColor]').each(function (index) {
    alert($(this).val());
});

※thisで受け取れるので第２引数は使わなくてよい

スニペットはMinteaに書く。


// -----------------------------------------------------
// test_inital_setにNuget追加






