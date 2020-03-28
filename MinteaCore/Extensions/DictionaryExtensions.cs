using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace MinteaCore.Extensions
{
    /// <summary>
    /// Dictionary 型の拡張メソッドを管理するクラス
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// 指定したキーと値をディクショナリに追加します
        /// 指定したキーが既に格納されている場合は何もしません
        /// </summary>
        public static void AddIfNotExists<TKey, TValue>(this Dictionary<TKey, TValue> self, TKey key, TValue value)
        {
            if (!self.TryGetValue(key, out _))
            {
                self.Add(key, value);
            }
        }

        /// <summary>
        /// 指定したキーが辞書になければ、新しい辞書を追加します
        /// </summary>
        public static void NewDictionaryIfNotExists<TPKey, TKey, TValue>(this Dictionary<TPKey, Dictionary<TKey, TValue>> self, TPKey key)
        {
            if (!self.ContainsKey(key))
            {
                self.Add(key, new Dictionary<TKey, TValue>());
            }
        }

        /// <summary>
        /// 指定したキーが辞書になければ、新しい辞書を追加します
        /// </summary>
        public static void NewDictionaryIfNotExists<TPKey>(this Dictionary<TPKey, dynamic> self, TPKey key)
        {
            if (!self.ContainsKey(key))
            {
                self.Add(key, new Dictionary<TPKey, dynamic>());
            }
        }

        /// <summary>
        /// 値を取得、keyがなければデフォルト値を設定し、デフォルト値を取得
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <param name="key">キー</param>
        /// <param name="defaultValue">値が存在しなかった場合の戻り値を指定、指定が無ければそのクラスのデフォルト値</param>
        /// <returns></returns>
        public static TValue GetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key, TValue defaultValue = default)
        {
            return dic.TryGetValue(key, out TValue result) ? result : defaultValue;
        }

        /// <summary>
        /// 動的にdynamic型を生成する
        /// </summary>
        /// <typeparam name="TKey">string</typeparam>
        /// <typeparam name="TValue">何でもよい</typeparam>
        /// <param name="fields">フィールド名とそのオブジェクトの組み合わせ</param>
        /// <returns>辞書のキーをフィールドとしたdynamic型データ</returns>
        public static dynamic ToDynamic<TKey, TValue>(this Dictionary<TKey, TValue> fields)
        {
            dynamic result = new ExpandoObject();
            IDictionary<TKey, TValue> work = result;
            foreach (var item in fields) { work.Add(item.Key, item.Value); }

            return result;
        }

        /// <summary>
        /// Listを格納した辞書のみ使用可能
        /// 辞書登録する前にキーの存在をチェックし、なければ追加する
        /// </summary>
        /// <typeparam name="TPKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <param name="key"></param>
        public static void NewListIfNotExists<TPKey, TValue>(this Dictionary<TPKey, List<TValue>> dic, TPKey key)
        {
            if (!dic.ContainsKey(key))
            {
                dic.Add(key, new List<TValue>());
            }
        }
        /// <summary>
        /// Listを格納した辞書のみ使用可能
        /// 辞書登録する前にキーの存在をチェックし、なければ追加する
        /// </summary>
        /// <typeparam name="TPKey"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <param name="key"></param>
        public static void NewListIfNotExists<TPKey, TKey, TValue>(this Dictionary<TPKey, Dictionary<TKey, TValue>> dic, TPKey key)
        {
            if (!dic.ContainsKey(key))
            {
                dic.Add(key, new Dictionary<TKey, TValue>());
            }
        }

        /// <summary>
        /// 指定キーがあればそこに要素を追加
        /// 指定キーがなければ辞書にリストを作成して要素を追加
        /// </summary>
        /// <typeparam name="TPKey">辞書のキーの型</typeparam>
        /// <typeparam name="TValue">格納しているリストの型</typeparam>
        /// <param name="dic"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddList<TPKey, TValue>(this Dictionary<TPKey, List<TValue>> dic, TPKey key, TValue value)
        {
            dic.NewListIfNotExists(key);
            dic[key].Add(value);
        }

        public static void AddDictionary<TPKey, TKey, TValue>(this Dictionary<TPKey, Dictionary<TKey, TValue>> dic, TPKey key, TKey childKey, TValue value)
        {
            if (!dic.ContainsKey(key))
            {
                dic.Add(key, new Dictionary<TKey, TValue>());
            }
            dic[key].Add(childKey, value);
        }

        #region InputDynamic:動的にdynamic型を生成する
        /// <summary>
        /// 動的にdynamic型を生成する
        /// </summary>
        /// <param name="Fields">フィールド名とそのオブジェクト(このメソッドで生成したdynamicでも良い)の組み合わせ</param>
        /// <returns></returns>
        public static dynamic ToDynamic(this Dictionary<string, object> Fields)
        {
            dynamic result = new ExpandoObject();
            IDictionary<string, object> work = result;
            foreach (var item in Fields) { work.Add(item.Key, item.Value); }

            return result;
        }
        #endregion
    }
}
