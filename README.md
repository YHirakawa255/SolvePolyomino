ポリオミノパズルの解を求めるプログラムを作成しました。
ポリオミノパスルとは、正方形をいくつか組み合わせた形状のミノ（ピース）を
ケースに収めるパズルです。
こちらは商品の例です（アマゾン）
https://www.amazon.co.jp/%E3%83%8F%E3%83%8A%E3%83%A4%E3%83%9E-HANAYAMA-%E6%98%8E%E6%B2%BB%E3%83%96%E3%83%A9%E3%83%83%E3%82%AF%E3%83%81%E3%83%A7%E3%82%B3%E3%83%AC%E3%83%BC%E3%83%88%E3%83%91%E3%82%BA%E3%83%AB-%E3%83%93%E3%82%BF%E3%83%BC/dp/B01NAB2LG2/ref=sr_1_6?__mk_ja_JP=%E3%82%AB%E3%82%BF%E3%82%AB%E3%83%8A&dchild=1&keywords=%E6%98%8E%E6%B2%BB+%E3%83%91%E3%82%BA%E3%83%AB+%E3%83%93%E3%82%BF%E3%83%BC&qid=1620801099&sr=8-6
解は唯一つしかないそうで、私のプログラムでもそれを確認しました。

プログラミングの学習として作成しました。
ミノ形状を関数で読み込み、解をコンソールで出力します。
関数を変更することで解く問題を変更できます。
配置パターンの総当りを行って解を探索します。
ビット演算を多用して高速化を図りました。
今後はテキストファイルから問題の読み込み、出力ができるようにしたいと考えております。
その次のステップではウィンドウを表示し、グラフィカルに問題の設定、表示ができるようにします。


