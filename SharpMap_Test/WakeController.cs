using GeoAPI.Geometries;
using Newtonsoft.Json;
using SharpMap.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SharpMap_Test
{
    internal class WakeController
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
