using System;
using System.Linq;
using System.Collections.Generic;

//KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
public class SolverCls  :Debug{//盤面形状、ミノ形状を取得し、解を探索するクラス
    int[] NMinoLeave = new int[0];//ミノの残り個数
    List<OderCls> CurrentSolution = new List<OderCls>();//現在探索中の解
    int[] CurrentVoidPointer = new int[0];//そのステップにおける空白マスの位置
    public List<List<OderCls>> Solution = new List<List<OderCls>>();//見つかった解を保存するためのクラス
    int iN, NOder;//解探索のポインタ（注目中のミノ、注目中の方向、配置済みの個数、ミノの種類）
    List<OderCls> Shape = new List<OderCls>();//全ミノの全方向の置き方を格納する
    bool[,] FCanPut = new bool[0, 0];//Shapeが配置可能（すでにおいてあるミノと重ならない）かどうかを格納する
        //各ステップ毎に保存する    //すべてのフラグが降りたら、総当たり（解探索）終了
    UInt128 SolverBoad = new UInt128();//探索現在のボードの状態
    UInt128 SolverRegion = new UInt128();//縦横の境界を表す
    int Width, Height;//ボードサイズ
    int[,] ForPrint = new int[0, 0];//盤面表示用の配列
    UInt128OperationCls BitOpe = new UInt128OperationCls();//128ビット演算を可能にする
    int ProgresFin = 0;//探索ツリーの終端の数を記録する
    int ProgresCounter = 0;//探索ツリーの終端が見つかる度にインクリメントされ、一定値を超えるとリセットされる
    int ProgresPrintTiming = 100000;//ProgresCounterのリセットタイミング
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    public void Solve(in List<MinoCls> minos, in BoadCls Boad){//解を探索するメソッド

        //解探索の概要
        //ミノを配置する総当りを行う。樹形図のすべての分岐を調べ、盤面が埋まる解を見つけ出す。
        //以下に記述する規則に従い樹形図を探索するので、解の取りこぼしはない（はずである）
        //盤面のマスを端から探索し、空白マスを見つける。（左上から右方向に）
        //その空白マスにそれぞれのミノの左上のマスを合わせ、配置可能ならば分岐をつくる
        //分岐を作るとき、現在の状態を「分岐元」、分岐元から一つミノを置いた状態を「派生分岐」と呼ぶことにする
        //派生分岐を一つ選び新しい分岐元とし、同様に空白マスを探索し、派生分岐を作る
        //この操作を繰り返してゆく
        //盤面を埋まりきらない状態で、配置できるミノがなければ、その派生分岐は解に到達できない分岐だとわかる
        //別の派生分岐を選ぶ操作を繰り返していくと、すべての派生分岐が解に到達しない場合が現れる
        //このとき、その分岐元も解に到達できないとわかるから、ミノを一つ戻し別の派生分岐を選ぶ
        //盤面が埋まりきれば、それが求める解である。解は保存して、ステップを戻し、また別の分岐を探索してゆく
        //この操作を繰り返してゆくことで解を取りこぼしなく見つけることができる

        //以下、解探索のフロー
        //①それぞれの配置種類について、配置可能か調べる★全検定
        //②最も若い配置候補で配置し、分岐を調べる（＝＞①）★分岐元決定・分岐なし★配置と次のステップ
        //③分岐がなければ、今の配置候補のフラグを降ろす★配置戻しと前のステップ
        //④すべての配置候補のフラグが降りれば、配置を戻す処理をし、探索再開（ー＞②）☆分岐元決定・分岐なし
        //
        //⑤解が見つかれば（ボードのビットがすべて立てば）、新しい解を保存する★解保存メソッド
        //⑥すべての配置候補フラグが降りれば、解探索終了★終了条件
        //（⑦解の重複を調べる）★重解チェッカー

        //必要なメソッドの書き出し
        //☆List変数の準備                          void PripareList(minos, boad)
        // ★全検定                                         void CanPutAll()
        // ★配置可能検定（単体）           bool CanPut()
        // ★分岐元決定・分岐なし           int SerchBranchSource() :分岐元のポインタを返す
        // ★配置と次のステップ                 int Put(int P)   :ステップのポインタを返す
        // ★配置戻しと前のステップ         int PutReverse()    :ステップのポインタを返す
        // ★解保存メソッド                         void AddSolution()
        // ★重解チェッカー                         void CheckSameSolution()
        // ★進捗カウンタ                               void ProgressInc()


        //メイン処理
        PripareVariable(in minos, in Boad);//変数の準備
        (Width, Height) = Boad.GetSize();//盤面サイズの取得（表示用）
        SolverBoad = Boad.GetBoad();//盤面の取得
        SolverRegion = Boad.GetRegion();//盤面領域の取得
        BitOpe.PrintBit(SolverBoad, "_");//盤面の初期状態を表示
        BitOpe.PrintShape(SolverBoad, Width, Height, false);//盤面の初期状態を表示

        int P;
        bool FFin = false;//探索終了フラグ
        iN = 0;//探索ステップ（ミノを置いた数）
        CurrentVoidPointer[iN] = 0;//現在ステップの空白マスの場所を保存しておく
            //１ステップ戻したときに、同じ処理を行わないためである
        CanPutAll(iN, CurrentVoidPointer[iN]);//引数の空白位置でおけるパターンを調べる

        //総当たり
        while(!FFin){
            P = SeekBranchSource(iN);//派生分岐のインデックスを取得する
            if(P>=0){//P<=-1は未探索の分岐が存在しないことを表す
                CurrentSolution[iN].Index = P;//分岐の種類
                CurrentSolution[iN].Kind = Shape[P].Kind;//ミノの種類
                (iN, CurrentSolution[iN].Shape) = Put(P, CurrentVoidPointer[iN]);//ミノの配置
                if(BitOpe.Zero( BitOpe.Not(SolverBoad) ) ){//盤面が埋まっているかどうか
                    db("Find A Solution!");//解発見
                    AddSolution(iN - 1);//解を保存、配置したミノの数が引数に必要
                    iN = PutReverse(P, CurrentVoidPointer[iN - 1]);//配置を戻す
                    FCanPut[iN, P] = false;//選択肢から除外
                    ProgresInc();
                }else{//盤面に空白が残っているとき
                    // BitOpe.PrintShape(SolverBoad, Width, Height, false);//現在の形状を表示する
                    CurrentVoidPointer[iN] = BitOpe.SeekUpFalse(SolverBoad);//空白マスを探索する
                    CanPutAll(iN, CurrentVoidPointer[iN]);
                }
            }else{//派生分岐が残っていないとき>>ステップを戻し、分岐候補から除外する
                if(iN<=0){//配置済みのミノが０個、かつ未探索の分岐なし＞＞探索終了
                    FFin = true;
                    break;
                }
                P = CurrentSolution[iN - 1].Index;//最後の配置パターンのインデックスを取得
                iN = PutReverse(P, CurrentVoidPointer[iN - 1]);//配置を戻す
                FCanPut[iN, P] = false;//選択肢から除外
                ProgresInc();
            }
        }
        // BitOpe.PrintShape(SolverBoad, Width, Height, true);
        // PrintFCanPut();

        foreach(List<OderCls> a in Solution){
            PrintSolution(a);
        }

        int nSolutionBefore = Solution.Count;
        db($"{Solution.Count} Solutions are Found");
        List<int[,]> SolutionMaps = CheckSameSolutoinAndGetSolutionMaps(Solution);

        //解の表示
        SolutionMaps.ForEach(a => db(GetMapShapeString(a)));
        db($"Solutions {nSolutionBefore} => {SolutionMaps.Count} Only");

    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    void PripareVariable(in List<MinoCls> minos, in BoadCls Boad){//List変数を準備するメソッド
        //置き方の総数を数える
        NOder = 0;
        Shape = new List<OderCls>(0);//置き方Listの初期化
        for (int im = 0; im < minos.Count;im++){//ミノの種類
            for (int id = 0; id < minos[im].GetNDir(); id++){//ミノ内の置き方の数
                Shape.Add(new OderCls());//要素追加
                Shape[NOder].Index = NOder;
                Shape[NOder].Kind = im;//どのミノなのか
                Shape[NOder].Shape = minos[im].GetShape(id);//どんな置き方（形状）なのか
                Shape[NOder].Region = minos[im].GetRegion(id);//置き方の占有領域
                NOder++;//置き方の数
            }
        }
        //変数をリサイズ、初期化する
        NMinoLeave = new int[minos.Count];  //ミノの残数
        for (int i1 = 0; i1 < minos.Count; i1++){
            NMinoLeave[i1] = minos[i1].GetNMino();
            db($"NMinoLeave[{i1}] = {NMinoLeave[i1]}");
        }
        int NMino = NMinoLeave.Sum();//ミノの総数
        db($"NMino = {NMino}");
        while(CurrentSolution.Count < NMino){
            CurrentSolution.Add(new OderCls());
        }
        // CurrentSolution.addrange;//現在の解
        CurrentVoidPointer = new int[NMino];
        FCanPut = new bool[NMino, NOder];//配置可能判定
        ForPrint = new int[Width, Height];
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    void CanPutAll(int P, int PVoid){//すべての形状について、それぞれが配置可能かの検定を行う
        for (int ip = 0; ip<FCanPut.GetLength(1); ip++){
            FCanPut[P, ip] = CanPut(ip, PVoid);
        }
        // PrintFCanPut();
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    bool CanPut(int P, int offset){//引数の配置種類が配置可能か確認する
        UInt128 tmp = BitOpe.ShiftL(in Shape[P].Shape, offset);
        // db($"CanPut({ip}, {offset})");
        // BitOpe.PrintBit(SolverBoad, "_");
        // BitOpe.PrintBit(tmp, "_");
        if(NMinoLeave[ Shape[P].Kind ] <= 0){
            return false;
        }else if( !BitOpe.AND(in SolverBoad, in tmp) ){
            tmp = BitOpe.ShiftL(in Shape[P].Region, offset);
            // BitOpe.PrintBit(SolverRegion, "_");
            // BitOpe.PrintBit(tmp, "_");
            return !BitOpe.AND(in SolverRegion, in tmp);
        }else{
            return false;
        }
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    int SeekBranchSource(int P){//分岐を選ぶ：FCanPutのtrueを探索してインデックスを返す
        for (int i2 = 0; i2 < FCanPut.GetLength(1); i2++){
            if( FCanPut[P, i2] ){
                return i2;
            }
        }
        return -1;
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    (int i1, UInt128 PutShape) Put(int P, int offset){//ミノを配置する
        UInt128 PutShape = BitOpe.ShiftL(Shape[P].Shape, offset);
        NMinoLeave[Shape[P].Kind]--;
        if (BitOpe.AND(SolverBoad, PutShape)) { db($"Worrning Can Not Put");  }
        SolverBoad = BitOpe.Or(SolverBoad, PutShape);
        return (++iN, PutShape);
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    int PutReverse(int P, int offset){//ミノの配置を戻す
        UInt128 TmpShape = BitOpe.ShiftL(Shape[P].Shape, offset);
        NMinoLeave[Shape[P].Kind]++;
        if (!BitOpe.Eq(TmpShape ,BitOpe.And(SolverBoad, TmpShape) ) ){   db($"Worrning Can Not Put"); }
        SolverBoad = BitOpe.Xor(SolverBoad, TmpShape);
        return --iN;
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    void AddSolution(int N){//解が見つかったとき、解リストに新しい解を追加する
        Solution.Add(CurrentSolution.Take(N).Select( a=>new OderCls(a) ).ToList() );
        PrintSolution(CurrentSolution);
        PrintSolution(Solution[Solution.Count - 1]);
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK

    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    List<int[,]> CheckSameSolutoinAndGetSolutionMaps(List<List<OderCls>> Solution){//同一解を探し、片方を削除する（未実装）
        db("Checking all duplicated solution");
        int[,] Map;
        List<int[,]> TransSolutoinMaps = new List<int[,]>();//変形したMpaの入れ物
        List<int[,]> SolutionMaps = Solution.Select(a => GetSolutionMap(a) ).ToList();//解の２次元配列表現を取得し、Listで保存
        for (int i1 = 0; i1 < SolutionMaps.Count; i1++){
            TransSolutoinMaps = GetTransMapList(SolutionMaps[i1]);//変形した解を取得
            int[] Indexes = SolutionMaps.Select((a, b) => JudgmentSameSolution(a, TransSolutoinMaps) ? b : -1).Where(n=>n>i1).ToArray();
            // db($"Check {Indexes.Length}");
            for (int i2 = Indexes.Length - 1; i2 >= 0; i2--) {
                // db($"{i2} : {Indexes[i2]}");
                SolutionMaps.RemoveAt(Indexes[i2]);
            }
        }
        return SolutionMaps;
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    List<int[,]> GetTransMapList(int[,] aug){
        List<int[,]> R = new List<int[,]>();
        int[,] Map = new int[Width, Height];
        //そのまま
        R.Add(new int[Width, Height]);
        Array.Copy(aug, R[0], aug.Length);
        //180度回転
        R.Add(new int[Width, Height]);
        for (int ih = 0; ih < aug.GetLength(1); ih++){
            for (int iw = 0; iw < aug.GetLength(0); iw++){
                R[1][Width - iw - 1, Height - ih - 1] = aug[iw, ih];
            }
        }
        //鏡像（左右）
        R.Add(new int[Width, Height]);
        for (int ih = 0; ih < aug.GetLength(1);ih++){
            for (int iw = 0; iw < aug.GetLength(0);iw++){
                R[2][Width - iw - 1, ih] = aug[iw, ih];
            }
        }
        //鏡像（上下）
        R.Add(new int[Width, Height]);
        for (int ih = 0; ih < aug.GetLength(1);ih++){
            for (int iw = 0; iw < aug.GetLength(0);iw++){
                R[3][iw, Height - ih - 1] = aug[iw, ih];
            }
        }
        return R;
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    bool JudgmentSameSolution(int[,] Map, List<int[,]> Base){//解が同一か判別する
        bool FSame;
        int Max = Map.Cast<int>().Max();//ミノの数を取得
        int[] Index = new int[Max + 1];
        for (int i1 = 0; i1 < Base.Count; i1++){
            Index = Index.Select(n => n = -1).ToArray();//-1で配列の初期化
            // db("--------------------------------------------------");
            // PrintMap(Map);
            // PrintMap(Base[i1]);
            FSame = true;
            for (int ih = 0; ih < Height; ih++){
                for (int iw = 0; iw < Width; iw++){
                    // db($"({i1}, {iw}, {ih}): Map={Map[iw, ih]}, Base={Base[i1][iw, ih]}");
                    if(Index[ Map[iw,ih] ] == -1){
                        Index[Map[iw, ih]] = Base[i1][iw, ih];
                    }else if(Index[Map[iw, ih]] != Base[i1][iw, ih]){
                        // db($"FALSE : ({i1}, {iw}, {ih}): Map={Map[iw, ih]}, Base={Base[i1][iw, ih]}");
                        FSame = false;
                        break;
                    }
                }
                if (!FSame) { break; }
            }
            if (FSame) { 
                // db($"TRUE Dir : {i1}");
                return true;
            }
        }
        return false;
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    void PrintMap(int[,] Map){//盤面の要素の種類を表示する
        string s = "";
        for (int ih = 0; ih < Height; ih++){
            for (int iw = 0; iw < Width; iw++){
                if(Map[iw,ih] < 10){
                    s += $"_{Map[iw, ih]},";
                }else{
                    s += $"{Map[iw, ih]},";
                }
            }
            s += "\r\n";
        }
        db(s);
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    int[,] GetSolutionMap(List<OderCls> Solution){//解の２次元配列表現を取得する
        int[,] Map = new int[Width, Height];
        for(int n = 0; n < Solution.Count; n++){
            UInt128 Shape = Solution[n].Shape;
            int P, w, h;
            while (!BitOpe.Zero(Shape)){
                P = BitOpe.SeekUpTrue(Shape);
                h = P / (Width + 1);
                w = P - (Width + 1) * h;
                Map[w, h] = n + 1;
                BitOpe.BitReSet(ref Shape, P);
            }
        }
        // db("GetSolutionMap");
        // PrintMap(Map);
        return Map;
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    void ProgresInc(){//処理の進捗に合わせてカウントアップし、一定数毎に表示
        ProgresFin++;
        if(++ProgresCounter >= ProgresPrintTiming){
            // PrintFCanPut();//デバック
            // BitOpe.PrintShape(SolverBoad, Width, Height, true);//デバック
            db($"Progress : {ProgresFin}");
            ProgresCounter = 0;
            // Console.ReadLine();
        }
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    void PrintFCanPut(){//FCanPutのデバック用表示
        int counter = 0;
        string s = "FCanPut = \r\n";
        for (int i2 = 0; i2 < FCanPut.GetLength(1); i2++){
            s += "|";
            for (int i1 = 0; i1 < FCanPut.GetLength(0); i1++){
                if(FCanPut[i1, i2]){    s += " 1, ";
                }else{  s += " 0, ";
                }
            }
            s += "|\r\n";
            if(counter >= 3){
                s += "-----------------------------------------------\r\n";
                counter = 0;
            }else{
                counter++;
            }
        }
        db(s);
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    void PrintSolution(List<OderCls> Sol){//解をコンソール表示する
        int[,] Map = new int[Width, Height];
        int[,] EqW = new int[Width + 1, Height + 2];
        int[,] EqH = new int[Width + 2, Height + 1];
        int w, h, tmp, iw, ih;
        // char Kind = 'A';
        //初期化
        for (ih = 0; ih < Height; ih++){
            for (iw = 0; iw < Width; iw++){
                Map[iw, ih] = -1;
            }
        }
        //形状を読み込む
        var _ = Sol.Select((a, n) => PrintSolutionSub(a, n, Map)).ToArray();
        //境界線を作る
        string s = GetMapShapeString(Map);
        //表示
        db(s);
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    int PrintSolutionSub(OderCls Oder, int n, int[,] Map){//ミノ形状に合わせて配列（Map）の値を変更する
        UInt128 Shape = Oder.Shape;
        int P, w, h;
        while( !BitOpe.Zero(Shape) ){
            P = BitOpe.SeekUpTrue(Shape);
            h = P / (Width + 1);
            w = P - (Width + 1) * h;
            // db($"w = {w}, h = {h}, Kind : {Oder.Kind}");
            Map[w, h] = n;
            BitOpe.BitReSet(ref Shape, P);
        }
        return n;
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    string GetMapShapeString(int[,] Map){//境界線を作る
        int ih, iw;
        string s = "X" + string.Concat( Enumerable.Repeat("XXXX",Width).ToArray() ) + "\r\nX";//上部の枠
        for (ih = 0; ih < Height - 1; ih++){
            for (iw = 0; iw < Width - 1; iw++){
                if (Map[iw, ih] == Map[iw + 1, ih]) { s += "    "; }//左右のミノのが異なれば壁ができる、同じならば壁はできない
                else { s += "   X"; }
            }
            s += "   X\r\nX";
            for (iw = 0; iw < Width; iw++){
                if (Map[iw, ih] == Map[iw, ih + 1]) { s += "   X"; }//上下のミノのが異なれば壁ができる、同じならば壁はできない
                else { s += "XXXX"; }
            }
            s += "\r\nX";
        }
        ih = Height - 1;
        for (iw = 0; iw < Width - 1; iw++){
            if (Map[iw, ih] == Map[iw + 1, ih]) { s += "    "; }
            else { s += "   X"; }
        }
        s += "   X\r\nX" + string.Concat(Enumerable.Repeat("XXXX", Width).ToArray()) + "\r\n";//下部の枠
        return s;
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
}
//KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
//KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
//KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
public class OderCls
{//配置したミノの種類、向きなどを保存するクラス
    //配列にして利用する
    public int Index;//Shpaeのインデックスを記録する
    public int Kind;//ミノの種類を記録する
    public UInt128 Shape = new UInt128();//配置ミノ形状のビット表現
    public UInt128 Region = new UInt128();//ミノの占有する領域のビット表現
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    public OderCls(OderCls R){
        this.Index = R.Index;
        this.Kind = R.Kind;
        this.Shape = R.Shape;
        this.Region = R.Region;
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    public OderCls(){
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    public OderCls Clone(OderCls R){
        return (OderCls)MemberwiseClone();
    }
}
//KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
//KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
//KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
// public struct IndexItem2<T>{
//     public T Element;
//     public int x, y;
//     public IndexItem2(T aug, int X, int Y){
//         this.Element = aug;
//         this.x = X;
//         this.y = Y;
//     }
// }
//KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK



