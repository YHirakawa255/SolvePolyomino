using System;
using System.Collections.Generic;

public class DataBaseCls    :Debug{//解く問題のボードやミノの形状を読み込むためのクラス
    //現状、Set_()メソッドを変更することで、解く問題（ボード形状やミノ形状）を定義できる
    //このクラスの情報を元にボードやミノのインスタンスが生成される
    List<List<string>> priSetMino = new List<List<string>>(){
        new List<string>(){}
    };
    BoadCls priSetBoad = new BoadCls(0, 0);
    List<int> preSetN = new List<int>();
    List<bool> priSetEnableInvert = new List<bool>();
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    public DataBaseCls(int n){
        switch (n){
            case 0:
                Set0();
                break;
            case 1:
                Set1();
                break;
            default:
                break;
        }
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    public void LoadMinoData(ref List<MinoCls> aug, ref BoadCls Boad){
        //形状を読み込むためのサブルーチン
        Func<string, int, bool> StrToShape = (s, n) => {
            if( s.Substring(n, 1) == "1" ){ return true;    
            }else { return false;   
            }
        };
        //盤面を呼び込む
        Boad = priSetBoad.Clone();
        for (int i1 = aug.Count; i1 < priSetMino.Count; i1++){
            aug.Add(new MinoCls(ref Boad));
        }
        //ミノを読み込む
        for (int i1 = 0; i1 < priSetMino.Count; i1++){//ミノの個数について
            for (int i2 = 0; i2 < priSetMino[i1].Count; i2++){//ミノ形状設定文字列について
                // Console.WriteLine($"{i1}.{i2}");
                for (int i3 = 0; i3 < priSetMino[i1][i2].Length; i3++){//文字列の文字数について
                    aug[i1].ShapeSet(i3, i2, StrToShape(priSetMino[i1][i2], i3));//形状を表す文字列を変換
                }
            }
            aug[i1].EnableInvert = priSetEnableInvert[i1];//裏表の有無
            aug[i1].SetNMino(preSetN[i1]);//ミノの数
            aug[i1].Cut();//形状を端に詰める
            aug[i1].BoadPrint($"Load Mino {i1} =>", true);
        }
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    void Set0(){//問題のプリセットその１
        db("Set0");
        //盤面の情報
        priSetBoad.Resize(11, 6);
        //ミノの形状情報
        preSetN.Add(1);//個数
        priSetEnableInvert.Add(false);//裏表の有無
        priSetMino[0].Add("01");//0 :X  //形状
        priSetMino[0].Add("1111");
        priSetMino[0].Add("01");

        preSetN.Add(1);
        priSetEnableInvert.Add(false);
        priSetMino.Add(new List<string>());//  :J
        priSetMino[1].Add("001");
        priSetMino[1].Add("101");
        priSetMino[1].Add("111");

        preSetN.Add(1);
        priSetEnableInvert.Add(false);
        priSetMino.Add(new List<string>());//  :Y
        priSetMino[2].Add("101");
        priSetMino[2].Add("111");
        priSetMino[2].Add("01");

        preSetN.Add(1);
        priSetEnableInvert.Add(false);
        priSetMino.Add(new List<string>());//  :W
        priSetMino[3].Add("111");
        priSetMino[3].Add("11");
        priSetMino[3].Add("1");

        preSetN.Add(1);
        priSetEnableInvert.Add(false);
        priSetMino.Add(new List<string>());//  :L
        priSetMino[4].Add("01");
        priSetMino[4].Add("11");
        priSetMino[4].Add("0111");

        preSetN.Add(1);
        priSetEnableInvert.Add(false);
        priSetMino.Add(new List<string>());//  :5-0
        priSetMino[5].Add("1");
        priSetMino[5].Add("11111");

        preSetN.Add(1);
        priSetEnableInvert.Add(false);
        priSetMino.Add(new List<string>());//3  :5-1
        priSetMino[6].Add("01");
        priSetMino[6].Add("11111");

        preSetN.Add(1);
        priSetEnableInvert.Add(false);
        priSetMino.Add(new List<string>());//  :5-2
        priSetMino[7].Add("11");
        priSetMino[7].Add("01111");

        preSetN.Add(1);
        priSetEnableInvert.Add(false);
        priSetMino.Add(new List<string>());//  :VA
        priSetMino[8].Add("111");
        priSetMino[8].Add("0111");

        preSetN.Add(1);
        priSetEnableInvert.Add(false);
        priSetMino.Add(new List<string>());//  :t
        priSetMino[9].Add("1111");
        priSetMino[9].Add("101");

        preSetN.Add(1);
        priSetEnableInvert.Add(false);
        priSetMino.Add(new List<string>());//  :d
        priSetMino[10].Add("111");
        priSetMino[10].Add("1011");
        priSetEnableInvert.Add(false);
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    void Set1(){//問題のプリセットその１
        db("Set1");
        //盤面の情報
        priSetBoad.Resize(8, 5);
        //ミノの情報
        priSetMino[0].Add("11");//0 :O  //形状
        priSetMino[0].Add("11");
        preSetN.Add(2);//個数
        priSetEnableInvert.Add(true);//裏表の有無

        priSetMino.Add(new List<string>());//  :I
        priSetMino[1].Add("1111");
        preSetN.Add(2);
        priSetEnableInvert.Add(true);

        priSetMino.Add(new List<string>());//  :T
        priSetMino[2].Add("111");
        priSetMino[2].Add("01");
        preSetN.Add(2);
        priSetEnableInvert.Add(true);

        priSetMino.Add(new List<string>());//  :L
        priSetMino[3].Add("111");
        priSetMino[3].Add("1");
        preSetN.Add(2);
        priSetEnableInvert.Add(true);

        priSetMino.Add(new List<string>());//  :S
        priSetMino[4].Add("11");
        priSetMino[4].Add("011");
        preSetN.Add(2);
        priSetEnableInvert.Add(true);
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    
}