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
        /// 指定したキーと値をディクショナリに追加します
        /// 指定したキーが既に格納されている場合は何もしません
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
