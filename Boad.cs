using System;

public class BoadCls    :Debug{//ボード形状を表すクラス
    //Solverにボードの情報を与えるためのクラス
    //解探索中はこのクラスのインスタンスが変更されることはない
    //今後、長方形以外の形状にも対応できるように変更を加える
    //ミノ情報を準備する際も利用される
    int width, height;
    UInt128 BoadEmpty = new UInt128();
    bool[,] RowBoad = new bool[0,0];
    UInt128 Boad = new UInt128();
    UInt128 Region = new UInt128();
    UInt128 P = new UInt128();
    UInt128OperationCls BitOpe = new UInt128OperationCls();
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    public BoadCls(int w, int h){//盤面クラス
        width = w;  height = h;
        BoadInit();
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    public BoadCls Clone(){//インスタンスの複製
        return (BoadCls)MemberwiseClone();
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    public void BeforeSolve(){//解探索前の処理
        BoadInit();
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    public UInt128 GetBoad(){//ビット表現のボードを取得する
        return Boad;
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    public UInt128 GetRegion(){//ビット表現のボード領域を取得する
        return Region;
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    public UInt128 GetBoadEmpty(){//何も配置していない、空のボードを取得する
        return BoadEmpty;
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    public (int width, int height) GetSize(){//ボードのサイズを取得する
        return (width, height);
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    void BoadInit(){//ボードの初期化
        P = new UInt128(0, 1);//ビットのポインタ
        BoadEmpty = BitOpe.GetBitRegion(width, height, width, height, false);//空のボードを取得する
        Boad = BoadEmpty;
        Region = BitOpe.GetBitRegion(width, height, width, height, false);
        BitOpe.PrintShape(BoadEmpty, width, height, true);
        BitOpe.PrintShape(Region, width, height, true);
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    public void Resize(int w, int h){
        width = w;  height = h;
        BoadInit();
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
}