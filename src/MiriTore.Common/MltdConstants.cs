using System;
using System.Collections.Generic;
using System.Text;
using OpenMLTD.MiriTore.Mltd.Entities;

namespace OpenMLTD.MiriTore {
    public static class MltdConstants {

        public static readonly Encoding Utf8WithoutBom = new UTF8Encoding(false);

        public static IReadOnlyList<IdolInfo> Idols => IdolsArray;

        private static readonly IdolInfo[] IdolsArray = {
            new IdolInfo("天海 春香", 1, "har", ColorType.Princess),
            new IdolInfo("如月 千早", 2, "chi", ColorType.Fairy),
            new IdolInfo("星井 美希", 3, "mik", ColorType.Angel),
            new IdolInfo("萩原 雪歩", 4, "yuk", ColorType.Princess),
            new IdolInfo("高槻 やよい", 5, "yay", ColorType.Angel),
            new IdolInfo("菊地 真", 6, "mak", ColorType.Princess),
            new IdolInfo("水瀬 伊織", 7, "ior", ColorType.Fairy),
            new IdolInfo("四条 贵音", 8, "tak", ColorType.Fairy),
            new IdolInfo("秋月 律子", 9, "rit", ColorType.Fairy),
            new IdolInfo("三浦 あずさ", 10, "azu", ColorType.Angel),
            new IdolInfo("双海 亜美", 11, "ami", ColorType.Angel),
            new IdolInfo("双海 真美", 12, "mam", ColorType.Angel),
            new IdolInfo("我那霸 響", 13, "hib", ColorType.Princess),
            new IdolInfo("春日 未来", 14, "mir", ColorType.Princess),
            new IdolInfo("最上 静香", 15, "siz", ColorType.Fairy),
            new IdolInfo("伊吹 翼", 16, "tsu", ColorType.Angel),
            new IdolInfo("田中 琴葉", 17, "kth", ColorType.Princess),
            new IdolInfo("島原 エレナ", 18, "ele", ColorType.Angel),
            new IdolInfo("佐竹 美奈子", 19, "min", ColorType.Angel),
            new IdolInfo("所 惠美", 20, "meg", ColorType.Fairy),
            new IdolInfo("徳川 まつり", 21, "mat", ColorType.Princess),
            new IdolInfo("箱崎 星梨花", 22, "ser", ColorType.Angel),
            new IdolInfo("野々原 茜", 23, "aka", ColorType.Angel),
            new IdolInfo("望月 杏奈", 24, "ann", ColorType.Angel),
            new IdolInfo("ロコ", 25, "roc", ColorType.Fairy),
            new IdolInfo("七尾 百合子", 26, "yur", ColorType.Princess),
            new IdolInfo("高山 紗代子", 27, "say", ColorType.Princess),
            new IdolInfo("松田 亜利沙", 28, "ari", ColorType.Princess),
            new IdolInfo("高坂 海美", 29, "umi", ColorType.Princess),
            new IdolInfo("中谷 育", 30, "iku", ColorType.Princess),
            new IdolInfo("天空橋 朋花", 31, "tom", ColorType.Fairy),
            new IdolInfo("エミリー・スチュアート", 32, "emi", ColorType.Princess),
            new IdolInfo("北沢 志保", 33, "sih", ColorType.Fairy),
            new IdolInfo("舞浜 歩", 34, "ayu", ColorType.Fairy),
            new IdolInfo("木下 ひなた", 35, "hin", ColorType.Angel),
            new IdolInfo("矢吹 可奈", 36, "kan", ColorType.Princess),
            new IdolInfo("横山 奈緒", 37, "nao", ColorType.Princess),
            new IdolInfo("二階堂 千鶴", 38, "chz", ColorType.Fairy),
            new IdolInfo("馬場 このみ", 39, "kon", ColorType.Angel),
            new IdolInfo("大神 環", 40, "tam", ColorType.Angel),
            new IdolInfo("豊川 風花", 41, "fuk", ColorType.Angel),
            new IdolInfo("宮尾 美也", 42, "miy", ColorType.Angel),
            new IdolInfo("福田 のり子", 43, "nor", ColorType.Princess),
            new IdolInfo("真壁 瑞希", 44, "miz", ColorType.Fairy),
            new IdolInfo("篠宮 可憐", 45, "kar", ColorType.Angel),
            new IdolInfo("百瀬 莉緒", 46, "rio", ColorType.Fairy),
            new IdolInfo("永吉 昴", 47, "sub", ColorType.Fairy),
            new IdolInfo("北上 麗花", 48, "rei", ColorType.Angel),
            new IdolInfo("周防 桃子", 49, "mom", ColorType.Fairy),
            new IdolInfo("ジュリア", 50, "jul", ColorType.Fairy),
            new IdolInfo("白石 紬", 51, "tmg", ColorType.Fairy),
            new IdolInfo("桜守 歌織", 52, "kao", ColorType.Angel),
            new IdolInfo("音無 小鳥", 101,"kot", ColorType.Extra),
            new IdolInfo("青叶 美咲", 102, "mis", ColorType.Extra),
            // Presumed to be 201/sik after update
            new IdolInfo("詩花", 201,  "xxx", ColorType.Extra),
            // Presumed to be 202/rei after update
            new IdolInfo("玲音", 202,  "xxx", ColorType.Extra)
        };

    }
}
