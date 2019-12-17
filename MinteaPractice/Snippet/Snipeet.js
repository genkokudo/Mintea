


// -----------------------------------------------------
// js_listbox_from_master
// jsでマスタデータによるリストボックスを動的に作成します。
// using Microsoft.AspNetCore.Mvc.Rendering;
// $DbPascal$   DB名単数形
// $DbCamel$    DB名単数形camel

// クライアント側 //
// 選択肢作成
// @using Newtonsoft.Json;
// var _lists = @Html.Raw(JsonConvert.SerializeObject(Model.Data.Lists));

var select = _lists["$DbPascal$"];
select.forEach(function (value) {
    tbody += ' <option value="' + value.Value + '">' + value.Text + '</option>';
});

// サーバ側 //
//			// TODO:Resultクラスに追加し、クライアントから参照できるようにします。
//			// 各マスタの選択項目
//			public Dictionary<string, SelectListItem[]> Lists { get; set; }

//              // TODO:検索ハンドラクラスに追加します。
//				// 各マスタを選択項目にする
//				var lists = new Dictionary<string, SelectListItem[]>();

//				var $DbCamel$ = _db.$DbPascal$s.OrderBy(x => x.Id).AsNoTracking().ToArray();
//				var $DbCamel$SelectList = new List<SelectListItem> { new SelectListItem("--- 未選択 ---", "-1") };
//				foreach (var item in $DbCamel$)
//				{
//					$DbCamel$SelectList.Add(new SelectListItem(item.Name, item.Id.ToString()));
//				}
//				lists.Add("$DbPascal$", $DbCamel$SelectList.ToArray());

// -----------------------------------------------------
// js_listbox_from_enum
// jsでenumによるリストボックスを動的に作成します。

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
// js_ajax_upload

$('#ButtonSave').on('click', function () {
    var formData = new FormData();
    // 全てのアップロードファイルを取得
    $('input[name=file]').each(function (index, element) {
        var file = element.files[0];
        if (file != undefined) {
            formData.append('uploadFiles', file, file.name);
            formData.append('ids', element.getAttribute('id')); // $(element).attr('id');
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
            alert('更新が完了しました');
        },
        function (data) {
            alert('更新に失敗しました:' + data);
        }
    );
}

// 画面に表示しているアップロードボタンのイベント
$(document).on('click', 'button[name=BtnUpload]', function () {
    $('#file').click();
    return false;	// 無いと遷移してしまう
});

// 何かファイルを選択したときの処理（実装しなくても良い）
$(document).on('change', 'input[name=file]', function () {
    var file = $(this)[0].files[0];
    var fileName = file.name;
    var fileSize = file.size;
    var fileType = file.type;
    alert('ファイル名 : ' + fileName + '\nファイルサイズ : ' + fileSize + ' bytes\nファイルタイプ : ' + fileType);
});

// サーバ側:PdfController.cs
//        public async Task<IActionResult> UploadAsync(List<IFormFile> uploadFiles, List<long> ids)
//        {
//        }

//// ※<input" type="file"/>を書かないとアップロードはできない。
//// でも、ファイル選択のボタンデザインを変えたい場合↓
////<button name="BtnUpload" type="button" class="btn btn-primary">×</button>
// TODO:id, acceptを適切に変更すること
////<input type="file" id="file" name="file" value="" class="d-none" accept="application/pdf" />

//<form method="post" id="FormDetail">
//    <div class="col"><button id="ButtonSave" type="button" class="btn btn-warning float-right">入力内容を反映</button></div>
//</form>

// -----------------------------------------------------


// ・js_foreach
arr.forEach((value) => {
    console.log(value);
});

// js_filter
// データ配列から条件に合うものを抽出します。
var pomeranians = dogs.filter(dog => dog.type === 'pomeranian');


// js_find
// データ配列から条件に合うものを1件見つけます。
var myDog = dogs.find(dog => dog.name === 'ポメラニアス3世');

// js_select
// map関数を使用してデータ配列から要素の配列を作成します。
var dogNames = dogs.map(dog => dog.name);

// js_reduce
// データ配列の要素を文字列結合したり合計したりします。
var total = dogs.reduce((acc, dog) => acc + dog.price, 0);

// ・js_each
// セレクタで指定したデータ全てに処理を行います。

$('#TableData input[id^=AaaaId]').each(function (index, elem) {
    elem.checked = true;
});

// ・js_each_find
// セレクタで指定したデータ全てに処理を行います。
$('#TableData').find('input[id^=AaaaId]').each(function (index, elem) {
    // ※thisで受け取れるので第２引数は使わなくてよい
    alert($(this).val());
});


// -----------------------------------------------------






