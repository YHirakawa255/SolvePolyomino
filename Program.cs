using System;
using System.Collections.Generic;

namespace SolvePolyomino{
    class Program{
        //概要
        //ポリオミノパズルの解を総当りにより探索する
        //ポリオミノとは、正方形を組み合わせた形状のことである
        //ポリオミノの有名な例は、テトリスのミノである
        //私が所持しているパズルを完成させることができなくなり、言語の勉強も兼ねて制作した
        //ビット演算を多用することで高速化を狙った
        //解の表示は最低限のコンソール表示とした
        //今後
        static void Main(string[] args){
            BoadCls Boad = new BoadCls(0, 0);//ミノが収まるボード
            List<MinoCls> Mino = new List<MinoCls>();//ミノ
            DataBaseCls DataBase = new DataBaseCls(1);//解く問題の設定のようなもの
            SolverCls Solver = new SolverCls();//問題を解くクラス
            UInt128OperationCls BitOpe = new UInt128OperationCls();//128ビットのビット演算を行うクラス
            System.Diagnostics.Stopwatch Timer = new System.Diagnostics.Stopwatch();//所要時間の計測

            DataBase.LoadMinoData(ref Mino, ref Boad);//盤面やミノの形状をロードする
            Boad.BeforeSolve();//解探索の前の下準備（ボード）
            foreach(MinoCls a in Mino){//解探索前の下準備（ミノ）
                a.BeforeSolve();
            }
            Timer.Start();//時間計測開始
            Solver.Solve(in Mino, in Boad);//解探索開始
            Console.WriteLine($"{Timer.ElapsedMilliseconds} mSec elapsed");//経過時間表示
        }
    }
    //作成所要時間の履歴(min)
    //約20時間で完成させた
    //
    //min 60
    //1030-1130:60
    //1200-1230:30:90
    //1530-1600:30:120
    //2300-2600:180:sum=300(5h)
    //1630-1730(0429):60:360
    //1800-1900:60:420
    //2000-2130:90:510
    //1000-1030:30:540
    //1200-1230:30:570(9.5h)
    //1630-1730:60:630
    //1830-1930:60:690
    //2015-2100:45:735(12.25h)
    //2200-2300:60:795
    //2345-2400:15:810
    //1145-1200:15:825
    //1345-1400:15:840
    //1415-1515:60:900
    //1645-1745:60:960
    //2130-2400:150:1110(18.5h)//Solved//解探索部分の作成終了
    //+60//表示
    //約1.5h：コメントを残す
    //1030-1200:90
    //1400-1630:150//22.5h//解の重複排除完成
}
