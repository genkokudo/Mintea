using System;
using System.Collections.Generic;
using System.Text;

namespace MinteaPractice.Template.T4
{
    // 名前空間/クラス名(ファイル名)を合わせる(親クラスの継承部分は書かなくてOK)
    public partial class MyCodeGenerator
    {
        public GenerationContext Context { get; }

        // とりあえずコンストラクタで受け取る
        public MyCodeGenerator(GenerationContext context)
        {
            this.Context = context;
        }
    }

    // 生成に使うパラメーターはクラス一個にまとめておいたほうが取り回しは良い
    public class GenerationContext
    {
        public string NamespaceName { get; set; }
        public string TypeSuffix { get; set; }
        public int RepeatCount { get; set; }
    }
}
