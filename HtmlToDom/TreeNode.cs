using System;
using System.Collections.Generic;
using System.Text;

namespace HtmlToDom
{
    /// <summary>
    /// 簡易ツリー構造のジェネリッククラス
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public /*abstract*/ class TreeNode<T>
    {
        /// <summary>
        /// 親への参照フィールド
        /// </summary>
        protected TreeNode<T> parent = null;

        /// <summary>
        /// 親への参照プロパティ
        /// </summary>
        public virtual TreeNode<T> Parent
        {
            get
            {
                return parent;
            }
            set
            {
                parent = value;
            }
        }

        /// <summary>
        /// 子ノードのリストフィールド
        /// </summary>
        protected IList<TreeNode<T>> children = null;

        /// <summary>
        /// 子ノードのリストプロパティ
        /// </summary>
        public virtual IList<TreeNode<T>> Children
        {
            get
            {
                if (children == null)
                    children = new List<TreeNode<T>>();
                return children;
            }
            set
            {
                children = value;
            }
        }

        /// <summary>
        /// データ実体
        /// </summary>
        public T Value = default(T);

        public TreeNode(T data)
        {
            Value = data;
        }


        /// <summary>
        /// 子ノードを追加する。
        /// </summary>
        /// <param name="child">追加したいノード</param>
        /// <returns>追加後のオブジェクト</returns>
        public virtual TreeNode<T> AddChild(TreeNode<T> child)
        {
            if (child == null)
                throw new ArgumentNullException("Adding tree child is null.");

            this.Children.Add(child);
            child.Parent = this;

            return this;
        }

        /// <summary>
        /// 子ノードを削除する。
        /// </summary>
        /// <param name="child">削除したいノード</param>
        /// <returns>削除後のオブジェクト</returns>
        public virtual TreeNode<T> RemoveChild(TreeNode<T> child)
        {
            this.Children.Remove(child);
            return this;
        }

        /// <summary>
        /// 子ノードを削除する。
        /// </summary>
        /// <param name="child">削除したいノード</param>
        /// <returns>削除の可否</returns>
        public virtual bool TryRemoveChild(TreeNode<T> child)
        {
            return this.Children.Remove(child);
        }

        /// <summary>
        /// 子ノードを全て削除する。
        /// </summary>
        /// <returns>子ノードを全削除後のオブジェクト</returns>
        public virtual TreeNode<T> ClearChildren()
        {
            this.Children.Clear();
            return this;
        }

        /// <summary>
        /// 自身のノードを親ツリーから削除する。
        /// </summary>
        /// <returns>親のオブジェクト</returns>
        public virtual TreeNode<T> RemoveOwn()
        {
            TreeNode<T> parent = this.Parent;
            parent.RemoveChild(this);
            return parent;
        }

        /// <summary>
        /// 自身のノードを親ツリーから削除する。
        /// </summary>
        /// <returns>削除の可否</returns>
        public virtual bool TryRemoveOwn()
        {
            TreeNode<T> parent = this.Parent;
            return parent.TryRemoveChild(this);
        }

    }

}
