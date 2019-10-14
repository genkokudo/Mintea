using System;
using System.Collections.Generic;
using System.Text;

namespace HtmlToDom
{
    /// <summary>
    /// ツリー構造のインターフェース
    /// </summary>
    public interface ITreeNode<T>
    {
        T Parent { get; set; }
        IList<T> Children { get; set; }

        T AddChild(T child);
        T RemoveChild(T child);
        bool TryRemoveChild(T child);
        T ClearChildren();
        bool TryRemoveOwn();
        T RemoveOwn();
    }

}
