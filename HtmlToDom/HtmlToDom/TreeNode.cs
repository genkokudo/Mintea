using Mintea.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mintea.HtmlToDom
{
    #region ListboxFile
    /// <summary>
    /// ディレクトリとファイルの情報
    /// </summary>
    public class ListboxFile
    {
        /// <summary>
        /// フルパス
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// 名前
        /// リストボックス選択肢のnameになる
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// ディレクトリならtrue
        /// ファイルならfalse
        /// </summary>
        public bool IsDirectory { get; set; }

        /// <summary>
        /// リストボックスで選択したとき
        /// 下位の要素リストを取得するためのキー
        /// リストボックス選択肢のvalueになる
        /// 
        /// ファイルならFullPathと同じ
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 所属する辞書のキー
        /// </summary>
        public string DictionaryKey { get; set; }

        public override string ToString()
        {
            return $"FullPath:{FullPath}\nDictionaryKey:{DictionaryKey}\nIsDirectory:{IsDirectory}\nName:{Name}\nValue:{Value}";
        }
    }
    #endregion

    /// <summary>
    /// 簡易ツリー構造のジェネリッククラス
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeNode<T>
    {

        #region privateGetDirectoryFileTree
        /// <summary>
        /// 引数以下のディレクトリの階層構造を取得します
        /// 
        /// ファイルとフォルダの区別は？
        /// とりあえずフォルダなら最後にスラッシュつける
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        private static TreeNode<ListboxFile> GetDirectoryFileTree(ListboxFile parent, List<ListboxFile> list)
        {
            var currentDir = new TreeNode<ListboxFile>(parent);

            IEnumerable<string> subFiles = Directory.GetFiles(parent.FullPath, "*", SearchOption.TopDirectoryOnly);
            IEnumerable<string> subFolders = Directory.GetDirectories(parent.FullPath, "*", SearchOption.TopDirectoryOnly);

            // ファイルの登録
            foreach (var file in subFiles)
            {
                var subFile = new ListboxFile
                {
                    FullPath = file,
                    Name = Path.GetFileName(file),
                    IsDirectory = false,
                    Value = file,
                    DictionaryKey = parent.Value
                };
                list.Add(subFile);
                currentDir.AddChild(new TreeNode<ListboxFile>(subFile));
            }

            // ディレクトリの登録
            foreach (var folder in subFolders)
            {
                var subFolder = new ListboxFile
                {
                    FullPath = folder,
                    Name = Path.GetFileName(folder),
                    IsDirectory = true,
                    Value = $"{parent.Value}#{Path.GetFileName(folder)}".Trim('#'),
                    DictionaryKey = parent.Value
                };
                list.Add(subFolder);
                var child = new TreeNode<ListboxFile>(subFolder);

                // 更に下の階層のディレクトリ
                GetDirectoryFileTree(subFolder, list);

                // このディレクトリに追加
                currentDir.AddChild(child);
            }

            return currentDir;
        }
        #endregion

        public static Dictionary<string, Dictionary<string, string>> GetDirectoryFileList(string path)
        {
            var list = new List<ListboxFile>();
            GetDirectoryFileTree(new ListboxFile
            {
                FullPath = path,
                Name = "root",
                IsDirectory = true,
                Value = "",
                DictionaryKey = ""
            }, list);

            var result = new Dictionary<string, Dictionary<string, string>>();

            foreach (var item in list)
            {
                result.NewDictionaryIfNotExists(item.DictionaryKey);
                result[item.DictionaryKey].Add(item.Name, item.Value);
            }

            return result;
        }

        /// <summary>
        /// それぞれのデータの親が分かっている場合
        /// 木構造データを作成します
        /// 子が持つ親は1つまで
        /// もう木構造データ作るの大変だからこういう補助メソッド作り込もうね
        /// </summary>
        /// <param name="root">根</param>
        /// <param name="dictionary">データリスト</param>
        /// <param name="parentList">Key名に対する親Key名を格納したデータ</param>
        /// <returns></returns>
        public static TreeNode<T> MakeTree(TreeNode<T> root, Dictionary<string, T> dictionary, Dictionary<string, string> parentList)
        {
            // ノードを一通り作る
            var nodeList = new Dictionary<string, TreeNode<T>>();
            foreach (var item in dictionary)
            {
                // 登録
                var node = new TreeNode<T>(item.Value);
                nodeList.Add(item.Key, node);
            }

            foreach (var node in nodeList)
            {
                if (parentList.ContainsKey(node.Key))
                {
                    // Parentがあればその親に登録
                    nodeList[parentList[node.Key]].AddChild(node.Value);
                }
                else
                {
                    // Parentがなければroot直下に追加
                    root.AddChild(node.Value);
                }
            }

            return root;
        }

        /// <summary>
        /// 左方優先で順に辿ってリストにする
        /// </summary>
        /// <param name="parent"></param>
        public void ToList(List<T> parent)
        {
            parent.Add(Value);

            if (children != null)
            {
                foreach (var item in children)
                {
                    item.ToList(parent);
                }
            }
        }

        /// <summary>
        /// 底優先探索の順を返す
        /// 多分深さ優先とは違うと思う
        /// 辿っていって根っこだったら追加
        /// </summary>
        /// <returns></returns>
        public List<TreeNode<T>> DepthList()
        {
            var result = new List<TreeNode<T>>();

            // 底
            if(children != null)
            {
                foreach (var item in children)
                {
                    var list = item.DepthList();
                    result.AddRange(list);
                }
            }

            result.Add(this);

            return result;
        }

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
        public T Value = default;

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
