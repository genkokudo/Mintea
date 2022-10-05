# HtmlToDom（完全に没）
HTMLタグをJavaScriptのDOMに変換するだけ。変数名はネストの深さで番号をつけるだけ。

しばらくこれ開発しないと思うので忘れないうちに書いとく。  
publicメソッドいっぱいあるけど、そいつら全部privateにしなきゃいけない。
ToJQueryしか使わない。

普通にAngleSharp使えば良かったことと、しばらくjQueryさわらないので凍結。
HTMLからReactStrapやBlazorStrap変換できるようにしたい。

# その他
HTMLからBlazorStrapとかに変換しようとしてやめた感じ。  
最初は簡単なのを作って、動的に変換ルール追加できるようにすれば良いかなーって思ってた。  
ビルドするにはMithrilCubeもクローンすること。  
