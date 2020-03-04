using System;
using System.Collections.Generic;
using System.Text;

namespace Mintea.Extensions
{
    /// <summary>
    /// List 型の拡張メソッドを管理するクラス
    /// </summary>
    public static class ListExtentions
    {
        /// <summary>
        /// リストに要素を追加しますが
        /// 同じものが既に格納されている場合は何もしません
        /// </summary>
        public static void AddIfNotExists<TType>(this List<TType> self, TType value)
        {
            if (!self.Contains(value))
            {
                self.Add(value);
            }
        }
    }
}
