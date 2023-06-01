using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    internal class TrackListGenerator
    {

        //TrackListの生成
        public void Generate()
        {
            //JSON文字列
            string str = @"
                {   
                    'group1':{
                        'pos1':{ 'x': 120.307 , 'y': 35.846 },
                        'pos2':{ 'x': 124.496 , 'y': 33.370 },
                        'pos3':{ 'x': 121.259 , 'y': 31.974 },
                        'pos4':{ 'x': 123.925 , 'y': 30.197 }
                    },
                    'group2':{
                        'pos1':{ 'x': 134.080 , 'y': 41.495 },
                        'pos2':{ 'x': 136.238 , 'y': 40.479 },
                        'pos3':{ 'x': 133.572 , 'y': 39.781 },
                        'pos4':{ 'x': 136.238 , 'y': 38.892 }
                    },
                    'group3':{
                        'pos1':{ 'x': 143.855 , 'y': 34.703 },
                        'pos2':{ 'x': 145.505 , 'y': 33.307 },
                        'pos3':{ 'x': 143.030 , 'y': 32.545 },
                        'pos4':{ 'x': 145.378 , 'y': 31.276 }
                    }
                }";

            // 文字列の空白文字を削除する
            //str = System.Text.RegularExpressions.Regex.Replace(str, @"[\s]+", "");

            //JSON文字列をデシリアライズして、 Dictionary<string, Dictionary<string, Dictionary<string, double>>> 型のオブジェクトに格納
            var trackList = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, double>>>>(str);

            foreach (var group in trackList)
            {
                Console.WriteLine("Group: " + group.Key);
                foreach (var position in group.Value)
                {
                    Console.WriteLine("Position: " + position.Key);
                    Console.WriteLine("x: " + position.Value["x"]);
                    Console.WriteLine("y: " + position.Value["y"]);
                }
                Console.WriteLine();
            }

        }

    }
}
