using GeoAPI.Geometries;
using Newtonsoft.Json;
using SharpMap.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

/** メモ
●group1 : 福岡県の西
[ 0 ] : POINT (120.30740526824003 35.846238257443026)
[ 1 ] : POINT (124.49650372695156 33.370861895477127)
[ 2 ] : LINESTRING (120.30740526824003 35.846238257443026, 124.49650372695156 33.370861895477127)
[ 3 ] : POINT (121.25947309976539 31.974495742573282)
[ 4 ] : LINESTRING (124.49650372695156 33.370861895477127, 121.25947309976539 31.974495742573282)
[ 5 ] : POINT (123.92526302803636 30.197302457059305)
[ 6 ] : LINESTRING (121.25947309976539 31.974495742573282, 123.92526302803636 30.197302457059305)

●group2 : 石川県の北
[ 0 ] : POINT (134.08066205432851 41.495174555158819)
[ 1 ] : POINT (136.23868247245261 40.479635534865118)
[ 2 ] : LINESTRING (134.08066205432851 41.495174555158819, 136.23868247245261 40.479635534865118)
[ 3 ] : POINT (133.57289254418166 39.7814524584132)
[ 4 ] : LINESTRING (136.23868247245261 40.479635534865118, 133.57289254418166 39.7814524584132)
[ 5 ] : POINT (136.23868247245261 38.892855815656205)
[ 6 ] : LINESTRING (133.57289254418166 39.7814524584132, 136.23868247245261 38.892855815656205)

●group3 : 千葉県の南東
[ 0 ] : POINT (143.85522512465539 34.703757356944685)
[ 1 ] : POINT (145.50547603263266 33.30739120404084)
[ 2 ] : LINESTRING (143.85522512465539 34.703757356944685, 145.50547603263266 33.30739120404084)
[ 3 ] : POINT (143.03009967066674 32.545736938820568)
[ 4 ] : LINESTRING (145.50547603263266 33.30739120404084, 143.03009967066674 32.545736938820568)
[ 5 ] : POINT (145.37853365509594 31.276313163453437)
[ 6 ] : LINESTRING (143.03009967066674 32.545736938820568, 145.37853365509594 31.276313163453437)
**/

namespace SharpMap_Test
{
    internal class WakeListGenerator
    {
        //Form1参照用(初期化は参照側で行う)
        public Form1 refForm1 = null;

        //生成する
        public void Generate()
        {
            //JSON文字列
            string strWakeList = @"
                {   
                    wake1:{
                        pos1:{ x: 120.307 , y: 35.846 },
                        pos2:{ x: 124.496 , y: 33.370 },
                        pos3:{ x: 121.259 , y: 31.974 },
                        pos4:{ x: 123.925 , y: 30.197 }
                    },
                    wake2:{
                        pos1:{ x: 134.080 , y: 41.495 },
                        pos2:{ x: 136.238 , y: 40.479 },
                        pos3:{ x: 133.572 , y: 39.781 },
                        pos4:{ x: 136.238 , y: 38.892 }
                    },
                    wake3:{
                        pos1:{ x: 143.855 , y: 34.703 },
                        pos2:{ x: 145.505 , y: 33.307 },
                        pos3:{ x: 143.030 , y: 32.545 },
                        pos4:{ x: 145.378 , y: 31.276 }
                    }
                }";
            // 文字列の空白文字を削除する
            //strWakeList = System.Text.RegularExpressions.Regex.Replace(strWakeList, @"[\s]+", "");

            //JSON文字列をデシリアライズして、 Dictionary<string, Dictionary<string, Dictionary<string, double>>> 型のオブジェクトに格納
            var wakeList = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, double>>>>(strWakeList);

            //描画する
            Draw(wakeList);
        }

        //描画する
        private void Draw(Dictionary<string, Dictionary<string, Dictionary<string, double>>> wakeList)
        {
            /*
            foreach (var wake in wakeList)
            {
                Console.WriteLine("■" + wake.Key);
                foreach (var pos in wake.Value)
                {
                    Console.Write($"　{pos.Key} : ");
                    Console.Write($"( {pos.Value["x"]} , ");
                    Console.Write($"{pos.Value["y"]} )\n");
                }
                Console.WriteLine();
            }
            */

            //wakeを取得
            foreach (var wake in wakeList)
            {
                //レイヤを作成 レイヤ名 = wake.Key (wake1,wake2,wake3,…)
                string layername = wake.Key;
                refForm1.GenerateLayer(layername);

                //座標を取得
                foreach (var pos in wake.Value)
                {
                    Coordinate wpos = new Coordinate(pos.Value["x"], pos.Value["y"]);
                    //点と線を描画
                    refForm1.AddPointToLayer(layername, wpos);
                    refForm1.AddLineConnectLast2PointsToLayer(layername);
                    refForm1.SetLineDash(layername);
                    refForm1.SetLineArrow(layername);
                    refForm1.SetPointOutLine(layername);

                    if(pos.Key== "pos1")
                    {
                        //ラベル名はwake.Keyの4文字目以降を切り出し
                        GenerateLabel(wpos, wake.Key.Substring(4));
                    }
                }
                Console.WriteLine();
            }
        }

        //==============================================
        //ラベル操作
        public struct WakeLabel
        {
            public Label label;
            public Coordinate wpos;
        }
        List<WakeLabel> wakeLabelList = new List<WakeLabel>();

        //ラベル生成
        private void GenerateLabel(Coordinate worldPos, string text)
        {
            //新しいラベルを生成
            Label newLabel = new Label();
            newLabel.Text = text;
            newLabel.AutoSize = true;
            //配置
            newLabel.Location = refForm1.TransPosWorldToImage(worldPos);
            //newLabel.Parent = refForm1.mapBox1; //親を設定
            //newLabel.BackColor = System.Drawing.Color.Transparent;//背景を透過
            //newLabel.Name = labelName; //識別名
            //コントロールに追加
            refForm1.mapBox1.Controls.Add(newLabel);

            //リストに追加
            WakeLabel wakeLabel = new WakeLabel();
            wakeLabel.label = newLabel;
            wakeLabel.wpos = worldPos;
            wakeLabelList.Add(wakeLabel);
        }

        // ラベルをmapboxに合わせて再配置
        public void RelocateLabel()
        {
            //Console.WriteLine("RelocateLabel");
            foreach (WakeLabel wakeLabel in wakeLabelList)
            {
                wakeLabel.label.Location = refForm1.TransPosWorldToImage(wakeLabel.wpos);
            }
        }
        //==============================================
    }
}
