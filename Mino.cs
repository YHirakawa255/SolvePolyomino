using System;

public class MinoCls    :Debug{//ミノを表すクラス
    //ミノの形状などの情報が格納されている
    //解の探索中に、このクラスのインスタンスが変更されることはない
    //探索前にSolverに必要な情報を渡すためのクラスである
    public bool EnableInvert = true;//ミノの裏表反転を許可するか
    bool[,] RawShape = new bool[0,0];//ミノの基本形状
    int Width, Height;//ミノの占有領域のサイズ
    int BoadW, BoadH;//盤面のサイズ
    int NDir;//向きのポインタ
    int NMino = 1;//ミノの個数
    UInt128[] Shape = new UInt128[8];//形状のビット表現を各向き毎に格納
    UInt128[] Region = new UInt128[8];//Shpaeに対応する占有領域を格納
    UInt128OperationCls BitOpe = new UInt128OperationCls();//128ビット演算を可能にする
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    public MinoCls(ref BoadCls Boad){
        (BoadW, BoadH) = Boad.GetSize();
        RawShape = new bool[BoadW, BoadH];//配列のリサイズ
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    public UInt128 GetShape(int n){//向きnのときの形状を取得するアクセサ
        return Shape[n];
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    public UInt128 GetRegion(int n){//向きnのときの占有領域を取得するアクセサ
        return Region[n];
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    public int GetNMino(){//ミノの個数を取得するアクセサ
        return NMino;
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    public void SetNMino(int n){//ミノの個数を変更するアクセサ
        NMino = n;
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    public void ShapeSet(int w, int h, bool tf){//ミノの基本形状を変更する
        try{    RawShape[w, h] = tf;  }
        catch { }
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    public void BeforeSolve(bool Mirror = false){//解探索前に必要なミノの形状などの情報を準備するメソッド
        BoadW = RawShape.GetLength(0);
        BoadH = RawShape.GetLength(1);
        //4回回転し、形状を記録
        NDir = 0;
        Cut();
        for (int id = 0; id < 4; id++){
            BeforeSolveSub(NDir);
            NDir++;
            if(!RotateRight()){
                Rotate180();
                id++;
            }
            // db("after rotate");
        }
        //許可しているなら、鏡に反転
        if(EnableInvert){
            RotateMirror();//裏表反転
            for (int id = 0; id < 4; id++){
                BeforeSolveSub(NDir);
                NDir++;
                if(!RotateRight()){
                    Rotate180();
                    id++;
                }
            }
            RotateMirror();//裏表反転
        }
        Cut();
        CheckSame();
        //
        // PriOffset();
        //表示デバッグ
        // for (int i1 = 0; i1 < NDir; i1++){
        //     // BitOpe.PrintBit(Shape[i1],"_");
        //     // BitOpe.PrintShape(in Shape[i1], BoadW, BoadH, true);
        // }
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    void BeforeSolveSub(int id){//BeforeSolveより分割、主に形状のビット表現を準備する
        int n, w, h;
        (w, h) = GetRegionSize();
        Shape[id] = GetBitShape();
        Region[id] = BitOpe.GetBitRegion(w, h, BoadW, BoadH);
        n = BitOpe.SeekUpTrue(Shape[id]);
        Shape[id] = BitOpe.ShiftR(Shape[id], n);
        Region[id] = BitOpe.ShiftR(Region[id], n);
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    bool CheckColumnEmpty(int n = 0){//n列目が空白か確認する（形状を左に詰めるのに利用する）
        try{
            for (int i1 = 0; i1 < BoadH; i1++){
                if(RawShape[n, i1]){
                    return false;
                }
            }
        }
        catch { return true; }
        return true;
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    bool CheckRowEmpty(int n = 0){//n行目が空白か確認する（形状を上に詰めるのに利用する）
        try{
            for (int i1 = 0; i1 < BoadW; i1++){
                if(RawShape[i1, n]){
                    return false;
                }
            }
        }
        catch { return true; }
        return true;
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    void RotateMirror(){//RawShape左右反転する
        bool[,] tmp = RawShape.Clone() as bool[,];
        int wMax = RawShape.GetLength(0);
        for (int w = 0; w < wMax; w++){
            for (int h = 0; h < RawShape.GetLength(1); h++){
                RawShape[w, h] = tmp[wMax - w - 1, h];
            }
        }
        Cut();
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    void Rotate180(){//RawShapeを180度回転する
        bool[,] tmp = RawShape.Clone() as bool[,];
        for (int h = 0; h < RawShape.GetLength(1); h++){
            for (int w = 0; w < RawShape.GetLength(0); w++){
                RawShape[w, h] = tmp[RawShape.GetLength(0) - w, RawShape.GetLength(1) - h];
            }
        }
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    bool RotateRight(){//RawShapeを右に回転する
        //右に回転できるとき、tureを返す
        //右に回転できないとき（盤面サイズを超えるとき）,falseを返す
        // db("RotateRight Start");
        int NMaxWidth, NMaxHeight;//形状が収まるサイズが処理により入る
        Cut();//RawShapeを詰める
        // db("CutFin");
        for (NMaxWidth = RawShape.GetLength(0); NMaxWidth >= 0; NMaxWidth--){//横幅について
            // db($"nw={NMaxWidth}");
            if (!CheckColumnEmpty(NMaxWidth)) { break; }
        }
        for (NMaxHeight = RawShape.GetLength(1); NMaxHeight >= 0; NMaxHeight--){//縦幅について
            if (!CheckRowEmpty(NMaxHeight)) { break; }
        }
        // db($"{NMaxWidth}-{NMaxHeight}");
        if(NMaxWidth<=BoadH && NMaxHeight<= BoadW){//幅、高さが盤面を超えないので回転する
            // db("pri right sub");
            RotateRightSub();
            Cut();
            // db("Rotate Fin");
            return true;//回転できるのでtrueを返す
        }else{//回転後の形状が縦横を超え、回転できないのでfalseを返す
            return false;
        }
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    void RotateRightSub(){//代入担当部分
        bool[,] tmp = RawShape.Clone() as bool[,];
        // db($"tmp:{ tmp.GetLength(0)}-{ tmp.GetLength(1)}");

        // db($"{RawShape.GetLength(0)}-{RawShape.GetLength(1)}");
        // RawShape = RawShape.Clone() as bool[,];
        // RawShape = new bool[2, 2];
        RawShape = new bool[tmp.GetLength(1), tmp.GetLength(0)];//サイズを回転
        // db($"{RawShape.GetLength(0)}-{RawShape.GetLength(1)}");
        // Array.Copy(RawShape, tmp, RawShape.Length);
        int max = Math.Min(BoadW, BoadH);
        // int max = 0;
        for (int h = 0; h < max; h++){
            for (int w = 0; w < max; w++){
                // db($"{w}x{h}");
                    RawShape[w, h] = tmp[h, max - w - 1];
            }
        }
        // db($"sub fin");
        // width = RawShape.GetLength(0);
        // height = RawShape.GetLength(1);
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    public void Cut(){//RawShapを端に詰める
        (int w, int h) = GetRegionSize();
        int n;
        //何列目まで空か調べる
        for (n = 0; n < RawShape.GetLength(0) - 1; n++) {
            if( !CheckColumnEmpty(n) ){
                break;
            }
            // Console.WriteLine($"w={n}");
        }
        CutWidth(n);//空の分を詰める
        //何行目まで空か調べる
        for (n = 0; n < RawShape.GetLength(1) - 1; n++) {
            if( !CheckRowEmpty(n) ){
                break;
            }
            // Console.WriteLine($"h={n}");
        }
        CutHeight(n);//空の分を詰める
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    (int w, int h) GetRegionSize(){//ミノの占有領域を調べる
        int n, w = 0, h =0;
        //何列目まで空か調べる
        for (n = RawShape.GetLength(0) - 1; n >= 0; n--) {
            if( !CheckColumnEmpty(n) ){
                w = n + 1;
                break;
            }
            // Console.WriteLine($"w={n}");
        }
        //何行目まで空か調べる
        for (n = RawShape.GetLength(1) - 1; n >= 0 ; n--) {
            if( !CheckRowEmpty(n) ){
                h = n + 1;
                break;
            }
            // Console.WriteLine($"h={n}");
        }
        // db($"w={w}, h={h}");
        return (w, h);
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    void CheckSame(){//重複を排除
        bool F = false;
        for (int id = NDir - 1; id > 0; id--){
            for (int ip = 0; ip < id; ip++){
                F = Shape[id].Equals(Shape[ip]);
                if(F){
                    NDir = CheckSameSub(id, NDir);
                    break;
                }
            }
        }
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    int CheckSameSub(int id, int n){//回転、反転などをした時に、同じ形状になる場合はまとめてしまう
        n--;
        for (int ip = id; ip < n; ip++){
            Shape[ip] = Shape[ip + 1];
            Region[ip] = Region[ip + 1];
        }
        return n;
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    void CutWidth(int n=1){//RawShapをnマス幅方向を詰める
        if (n == 0) { return; }
        for (int w = 0; w<RawShape.GetLength(0) - n; w++) {
            for (int h = 0; h < RawShape.GetLength(1); h++){
                RawShape[w, h] = RawShape[w + n, h];
                RawShape[w + n, h] = false;
            }
        }
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    void CutHeight(int n=1){//RawShapをnマス高さ方向を詰める
        if (n == 0) { return; }
        for (int h = 0; h < RawShape.GetLength(1) - n; h++){
            for (int w = 0; w<RawShape.GetLength(0); w++) {
                RawShape[w, h] = RawShape[w, h + n];
                RawShape[w, h + n] = false;
            }
        }
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    UInt128 GetBitShape(){//RawShapeからビット表現の形状を取得する
        // db("GetBitShape");
        // Cut();
        // BoadPrint();
        UInt128 r = new UInt128(0, 0);
        // BitOpe.BitWrite(r, "_");
        int count = 0;
        for (int h = 0; h < RawShape.GetLength(1); h++){
            for (int w = 0; w < RawShape.GetLength(0); w++){
                count = (BoadW + 1) * h + w;
                if(RawShape[w, h]){
                    BitOpe.BitSet(ref r, count);
                }else{
                    BitOpe.BitReSet(ref r, count);
                }
            }
        }
        // BitOpe.BitWrite(r,"_");
        // BitOpe.PrintShape(ref r, BoadW, BoadH, true);
        return r;
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    public void BoadPrint(string s = "", bool Full = false){//形状確認のために表示をする
        UInt128 P = new UInt128();
        P.A = 0;    P.B = 1;
        int counter = 0;
        s += "\r\n";
        for (int ih = 0; ih < RawShape.GetLength(1); ih++){
            for (int iw = 0; iw < RawShape.GetLength(0); iw++){
                if( RawShape[iw, ih] ){
                    s += "$$";
                }else{
                    s += "[]";
                }
                counter++;
            }
            if (Full) {
                if(RawShape[RawShape.GetLength(0) - 1, ih]) {
                    s += ".$$";  
                }else{
                    s += ".[]";
                }
            }
            s += "\r\n";
            counter++;
        }
        if(Full){
            int tmp = counter;
            counter = 0;
            for (int i1 = tmp; i1 < 128; i1++){
                s += "[]";
                if(counter >= BoadW){
                    s += "\r\n";
                    counter = 0;
                }else{
                    counter++;
                }
            }
        }
        db(s);
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    public int GetNDir(){//向き違いの形状がいくつあるのか取得するアクセサ
        return NDir;
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK

}

