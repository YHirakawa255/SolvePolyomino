using System;

//このファイルにいくつかのクラスをまとめた
public struct UInt128{//128ビット型を表す構造体
    //具体的なビット演算はUint128OperationCls経由で行う
    public ulong A, B;
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    public UInt128(ulong a=0, ulong b=0){
        A = a; B = b;
    }
}
//KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
public class Debug{//Console.WriteLine(string)を短く記述する
    public void db(string s){
        Console.WriteLine(s);
    }
}
//KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
//KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
//KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
public class UInt128OperationCls{//128ビット型のビット演算などを行うクラス
    UInt128 a = new UInt128();//バッファ
    UInt128 b = new UInt128();//バッファ
    UInt128 P = new UInt128();//ビットポインタ（マスクして使用する）
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    public string PrintBit(UInt128 aug, string s = ""){//ビット表示する
        string S = "0x";
        int count = -1;
        ulong P = 0x8000_0000_0000_0000;
        for (int i1 = 63; i1 >= 0; i1--){
            if(count >= 7){
                S += s;
                count = 0;
            }else{
                count++;
            }
            if ((aug.A & P) > 0){
                S += "1";
            }else{
                S += "0";
            }
            P >>= 1;
        }
        P = 0x8000_0000_0000_0000;
        for (int i1 = 63; i1 >= 0; i1--){
            if(count >= 7){
                S += s;
                count = 0;
            }else{
                count++;
            }
            if ((aug.B & P) > 0){
                S += "1";
            }else{
                S += "0";
            }
            P >>= 1;
        }
        Console.WriteLine(S);
        return S;
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    public string BitWrite(ulong aug, string s = ""){//ビット表示の文字列を取得する
        string S = "0x";
        int count = -1;
        ulong P = 0x8000_0000_0000_0000;
        for (int i1 = 63; i1 >= 0; i1--){
            if(count >= 7){
                S += s;
                count = 0;
            }else{
                count++;
            }
            if ((aug & P) > 0){
                S += "1";
            }else{
                S += "0";
            }
            P >>= 1;
        }
        Console.WriteLine(S);
        return S;
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    public void PrintShape(in UInt128 aug, int width, int height, bool Full = false){//形状のビット表現をコンソールに表示する
        P.A = 0;    P.B = 1;
        int counter = 0;
        string s = "\r\n";
        for (int ih = 0; ih < height; ih++){
            for (int iw = 0; iw < width; iw++){
                if( BitGet(aug, counter) ){ s += "$$";
                }else{  s += "[]";
                }
                counter++;
            }
            if (Full) {
                if( BitGet(aug, counter) ){   s += ".$$";
                }else{  s += ".[]";
                }
            }
            s += "\r\n";
            counter++;
        }
        if(Full){
            int tmp = counter;
            counter = 0;
            for (int i1 = tmp; i1 < 128; i1++){
                if(BitGet(aug, i1)){   s += "$$";
                }else{  s += "[]";
                }
                if(counter >= width){
                    s += "\r\n";
                    counter = 0;
                }else{
                    counter++;
                }
            }
        }
        Console.WriteLine(s);
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    public UInt128 ShiftL(in UInt128 aug, int n){//左方向のシフト
        if (n == 0) { return aug; }
        b.A = 0;    b.B = 0;
        // BitWrite(aug,"_");
        b.A = (aug.A << n) | (aug.B >> (64 - n) );
        // BitWrite(aug, "_");
        b.B = aug.B << n;
        // BitWrite(aug, "_");
        // BitWrite(b,"_");
        // Console.WriteLine("----------");
        // PrintShape(ref b, 11, 7, true);
        return b;
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    public UInt128 ShiftR(in UInt128 aug, int n){//右方向のシフト
        if (n == 0) { return aug; }
        b.A = aug.A >> n;
        b.B = (aug.A >> (64 - n)) | (aug.B >> n);
        return b;
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    public bool Eq(UInt128 aug1, UInt128 aug2){//２つの引数が一致するか
        if(aug1.A == aug2.A && aug1.B == aug2.B){
            return true;
        }else{
            return false;
        }
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    public bool Zero(UInt128 aug){//引数の全ビットが0か
        if (aug.A == 0 && aug.B == 0) {
            return true; 
        }else{
            return false;
        }
        
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    public bool AND(in UInt128 aug1, in UInt128 aug2){// & 後に立っているビットが一つでもあるか
        if( (aug1.A & aug2.A) > 0){ return true;    }
        if( (aug1.B & aug2.B) > 0){ return true;    }
        return false;
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    public UInt128 And(UInt128 aug1, UInt128 aug2){// &
        a.A = aug1.A & aug2.A;
        a.B = aug1.B & aug2.B;
        return a;
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    public UInt128 Or(UInt128 aug1, UInt128 aug2){// |
        a.A = aug1.A | aug2.A;
        a.B = aug1.B | aug2.B;
        return a;
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    public UInt128 Not(UInt128 aug){// ~
        a.A = ~aug.A;
        a.B = ~aug.B;
        return a;
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    public UInt128 Xor(UInt128 aug1, UInt128 aug2){// ^
        a.A = aug1.A ^ aug2.A;
        a.B = aug1.B ^ aug2.B;
        return a;
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    public UInt128 Pointing(UInt128 aug, int n){//n番目のビットだけを残し（他はすべて0）、返す
        a.A = 0;    a.B = 1;
        b = ShiftL(in a, n);
        a = And(aug, b);
        return a;
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    public bool BitGet(UInt128 aug, int n){//n番目のビットを取得する
        ulong P = 1;
        if(n>=64){
            P <<= (n - 64);
            if( (aug.A&P) > 0){
                return true;
            }else{
                return false;
            }
        }else{
            P <<= n;
            if( (aug.B&P) > 0){
                return true;
            }else{
                return false;
            }
        }
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    public UInt128 BitSet(ref UInt128 aug, int n){//n番目のビットをセットする
        ulong P = 1;
        if(n>=64){
            P <<= (n - 64);
            // BitWrite(P);
            aug.A |= P;
        }else{
            P <<= n;
            // BitWrite(P);
            aug.B |= P;
        }
        return aug;
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    public UInt128 BitReSet(ref UInt128 aug, int n){//n番目のビットをリセットする
        ulong P = 1;
        if(n>=64){
            P <<= (n - 64);
            aug.A &= ~P;
        }else{
            P <<= n;
            aug.B &= ~P;
        }
        return aug;
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    public int SeekUpTrue(UInt128 aug){//0ビット目から1を探し、位置を返す
        ulong p = 1;
        for (int i1 = 0; i1 < 64; i1++){
            if( (aug.B & p) > 0){
                return i1;
            }else{
                p <<= 1;
            }
        }
        p = 1;
        for (int i1 = 0; i1 < 64; i1++){
            if( (aug.A & p) > 0){
                return i1 + 64;
            }else{
                p <<= 1;
            }
        }
        return -1;
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    public int SeekUpFalse(UInt128 aug){//0ビット目から0を探し、位置を返す
        ulong p = 1;
        for (int i1 = 0; i1 < 64; i1++){
            if ((aug.B & p) == 0){
                return i1;
            }else{
                p <<= 1;
            }
        }
        p = 1;
        for (int i1 = 0; i1 < 64; i1++){
            if ((aug.A & p) == 0){
                return i1 + 64;
            }else{
                p <<= 1;
            }
        }
        return -1;
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    public int SeekDownFalse(UInt128 aug){//127ビット目から1を探し、位置を返す
        ulong p = 0x8000_0000_0000_0000;
        for (int i1 = 63; i1 >= 0; i1--){
            if( (aug.A & p) == 0){
                return i1 + 64;
            }else{
                p >>= 1;
            }
        }
        p = 0x8000_0000_0000_0000;
        for (int i1 = 63; i1 >= 0; i1--){
            if( (aug.B & p) == 0){
                return i1;
            }else{
                p >>= 1;
            }
        }
        return -1;
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    public int SeekDownTrue(UInt128 aug){//127ビット目から0を探し、位置を返す
        ulong p = 0x8000_0000_0000_0000;
        for (int i1 = 63; i1 >= 0; i1--){
            if( (aug.A & p) > 0){
                return i1 + 64;
            }else{
                p >>= 1;
            }
        }
        p = 0x8000_0000_0000_0000;
        for (int i1 = 63; i1 >= 0; i1--){
            if( (aug.B & p) > 0){
                return i1;
            }else{
                p >>= 1;
            }
        }
        return -1;
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    public UInt128 Reverse(UInt128 aug){//ビットの並び順を逆転する
        a.A = ReverseSub(aug.B);    a.B = ReverseSub(aug.A);
        return a;
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    ulong ReverseSub(ulong aug){//ビットの並び順を逆転する（分割）
        aug = (aug & 0xFFFF_FFFF_0000_0000) >> 32 | (aug & 0x0000_0000_FFFF_FFFF) << 32;
        aug = (aug & 0xFFFF_0000_FFFF_0000) >> 16 | (aug & 0x0000_FFFF_0000_FFFF) << 16;
        aug = (aug & 0xFF00_FF00_FF00_FF00) >> 8 | (aug & 0x00FF_00FF_00FF_00FF) << 8;
        aug = (aug & 0xF0F0_F0F0_F0F0_F0F0) >> 4 | (aug & 0x0F0F_0F0F_0F0F_0F0F) << 4;
        aug = (aug & 0xCCCC_CCCC_CCCC_CCCC) >> 2 | (aug & 0x3333_3333_3333_3333) << 2;
        aug = (aug & 0xAAAA_AAAA_AAAA_AAAA) >> 1 | (aug & 0x5555_5555_5555_5555) << 1;
        return aug;
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
    public UInt128 GetBitRegion(int w, int h, int BoadW, int BoadH, bool F = true){//サイズ情報から、占有領域のビット表現を取得する
        UInt128 r = new UInt128();
        UInt128 bar = new UInt128();
        // db($"{w}x{h}");
        for (int i1 = 0; i1 < w; i1++){
            BitSet(ref bar, i1);
        }
        for (int i1 = 0; i1 < h; i1++){
            r = Or(r, bar);
            bar = ShiftL(in bar, BoadW + 1);
        }
        // BitOpe.PrintShape(in r, BoadW, BoadH);
        if(F){
            return r;
        }else{
            return Not(r);
        }
    }
    //KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK
}