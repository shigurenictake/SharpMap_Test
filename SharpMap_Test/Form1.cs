using GeoAPI.Geometries;//SharpMap
using NetTopologySuite.Geometries;//SharpMap
using SharpMap.Data.Providers;//SharpMap
using SharpMap.Forms;
using SharpMap.Layers;//SharpMap
using SharpMap;//SharpMap
using System;
using System.Collections.Generic;//SharpMap
using System.ComponentModel;
using System.Data;
using System.Drawing;//SharpMap
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpMap.Drawing;
using System.Collections.ObjectModel;

namespace SharpMap_Test
{
    public partial class Form1 : Form
    {
        //構造体
        struct StPoint
        {
            public int num;
            public double x;
            public double y;
        };

        //クラス変数
        StPoint g_worldPos = new StPoint();

        //コンストラクタ
        public Form1()
        {
            InitializeComponent();

            //SharpMap初期化
            InitSharpMap();
        }

        private void InitSharpMap()
        {
            //======================
            //SharpMap
            //参考
            //http://blog.livedoor.jp/kuro_program/archives/7235669.html

            mapBox1.Map = new Map(new Size(mapBox1.Width, mapBox1.Height));
            mapBox1.Map.BackColor = System.Drawing.Color.LightBlue;

            //レイヤーの作成
            VectorLayer baseLayer = new VectorLayer("Countries");
            baseLayer.DataSource = new ShapeFile(@"..\..\ShapeFiles\polbnda_jpn\polbnda_jpn.shp");
            //baseLayer.DataSource = new ShapeFile(@"..\..\ShapeFiles\ne_10m_coastline\ne_10m_coastline.shp");

            baseLayer.Style.Fill = Brushes.LimeGreen;
            //baseLayer.Style.Fill = new SolidBrush(Color.LimeGreen);
            baseLayer.Style.Outline = Pens.Black;
            baseLayer.Style.EnableOutline = true;

            //マップにレイヤーを追加
            mapBox1.Map.Layers.Add(baseLayer);


            //線と点を書く start -------
            VectorLayer orgLayer = new VectorLayer("symbol");
            GeometryFactory gf = new GeometryFactory();
            List<IGeometry> eomColl = new List<IGeometry>();

            //線を書く
            Coordinate[] linePos = { new Coordinate(135, 30), new Coordinate(135, 37) };
            eomColl.Add(gf.CreateLineString(linePos));

            //点を書く
            eomColl.Add(gf.CreatePoint(new Coordinate(135, 35)));

            GeometryProvider vpro = new GeometryProvider(eomColl);
            orgLayer.DataSource = vpro;

            mapBox1.Map.Layers.Add(orgLayer);
            //線と点を書く end -------


            //Zoom制限
            mapBox1.Map.MinimumZoom = 0.1;
            mapBox1.Map.MaximumZoom = 360.0;

            mapBox1.Map.ZoomToExtents();
            mapBox1.Refresh();
            //======================
        }

        //イベント - 地図上のマウス移動
        private void mapBox1_MouseMove(Coordinate worldPos, MouseEventArgs imagePos)
        {
            UpdateWorldPos(worldPos);
        }


        //イベント - 地図上のクリック
        private void mapBox1_Click(object sender, EventArgs e)
        {
            UpdateSymbolLayer();
        }

        //世界座標の更新
        private void UpdateWorldPos(Coordinate worldPos)
        {
            g_worldPos.x = worldPos.X;
            g_worldPos.y = worldPos.Y;

            this.label1.Text = worldPos.ToString();
        }

        //symbolレイヤーの更新（考え中）
        private void UpdateSymbolLayer()
        {
            //"symbol" レイヤーを取得
            var symbolLayer = mapBox1.Map.GetLayerByName("symbol") as SharpMap.Layers.VectorLayer;

            //"symbol" レイヤーに描画されている赤色の点を取得

            //点を黄色に変更
        }

        //削除？
        private void DeleteLayer()
        {
            string layerName = "myLayer"; // 削除するレイヤーの名前

            // レイヤーを取得
            var layer =  mapBox1.Map.GetLayerByName(layerName) as SharpMap.Layers.VectorLayer;

            // マップコントロールからレイヤーを削除
            mapBox1.Map.Layers.Remove(layer);
        }

        //symbolレイヤーの更新（ボツ）
        private void UpdateSymbolLayer_Botu1()
        {
            //symbolレイヤーを取得する
            VectorLayer symbolLayer = null;
            string layerName = "symbol";

            foreach (var layer in mapBox1.Map.Layers)
            {
                if (layer is VectorLayer vectorLayer && vectorLayer.LayerName == layerName)
                {
                    symbolLayer = vectorLayer;
                    break;
                }
            }

            if (symbolLayer != null)
            {
                // 交差するジオメトリを指定します（ここでは矩形を指定しています）
                var geometryFactory = new NetTopologySuite.Geometries.GeometryFactory();
                var envelope = new GeoAPI.Geometries.Envelope(100, 200, 300, 400);
                var geometry = geometryFactory.ToGeometry(envelope);

                Console.WriteLine(geometry);

                //List<IGeometry> eomColl = new List<IGeometry>();
                //IGeometry geometry = eomColl[0]; // 交差するジオメトリを指定する

                SharpMap.Data.FeatureDataSet featureSet = new SharpMap.Data.FeatureDataSet();

                Console.WriteLine(featureSet);

                //symbolLayer.DataSource.GetFeatureTableName(countriesLayer.DataSource.GetGeomType());
                var featureSetSL = symbolLayer.DataSource as SharpMap.Data.Providers.IProvider;

                Console.WriteLine(featureSetSL);

                featureSet.Tables.Add((SharpMap.Data.FeatureDataTable)featureSetSL);
                //symbolLayer.DataSource.ExecuteIntersectionQuery(geometry.Envelope, featureSet);
                //DataTable featureDataTable = featureSet.Tables[0];


                /*              
                 *              SharpMap.Data.FeatureDataSet featureSet = new SharpMap.Data.FeatureDataSet();
                                var featureSetb = symbolLayer.DataSource as SharpMap.Data.Providers.IProvider;
                                featureSet.Tables.Add(symbolLayer.DataSource.GetFeatureTableName(symbolLayer.DataSource.GetGeomType()));
                                countriesLayer.DataSource.ExecuteIntersectionQuery(geometry.Envelope, featureSet);
                                DataTable featureDataTable = featureSet.Tables[0];
                */



                //var featureSet = symbolLayer.DataSource as SharpMap.Data.Providers.IProvider;
                //var featureDataTable = featureSet.ExecuteIntersectionQuery(geometry.Envelope, (SharpMap.Data.FeatureDataSet)geometry);
                //var featureDataTable = featureSet.ExecuteIntersectionQuery(geometry.Envelope, (SharpMap.Data.FeatureDataSet)geometry);
                //foreach (SharpMap.Data.FeatureDataRow row in featureDataTable.Rows)



                /*                    var features = symbolLayer.DataSource.GetFeaturesIntersection(geometries);
                                foreach (IFeature feature in features)


                                    // symbolレイヤーに描画された赤い点を黄色に変更する
                                    foreach (var feature in symbolLayer.DataSource.GetGeometriesInView(new GeoAPI.Geometries.Envelope(-90, 90, -180, 180)) )
                                {
                                    Geometry geo = new Geometry();
                                    if (feature.GeometryType is new Geometry().InteriorPoint point )
                                    {
                                        point.Symbol = new Symbol(Color.Yellow, 10, new Pen(Color.Yellow));
                                    }
                                }
                */


                // MapBoxを再描画する
                //mapBox1.Refresh();
            }
            else
            {
                // Countriesレイヤーが見つからなかった場合の処理
            }
        }

        //symbolレイヤーの更新（ボツ）
        private void UpdateSymbolLayer_Botu2()
        {
            // 1. "symbol" レイヤーを取得
            var symbolLayer = mapBox1.Map.GetLayerByName("symbol") as SharpMap.Layers.VectorLayer;

            // 2. "symbol" レイヤーに描画されている赤色の点を取得
            //var symbolFeatures = symbolLayer.DataSource.GetFeatures().Where(f => f.Geometry is Point && f["color"].ToString() == "red");
            var symbolFeatures = symbolLayer.DataSource as SharpMap.Data.Providers.IProvider;

            /*            if (!symbolFeatures.Any())
                        {
                            // 赤色の点が存在しない場合の処理
                            return;
                        }*/

            // 3. 点を黄色に変更
            foreach (var symbolFeature in symbolFeatures)
            {
                symbolFeature.Style.Fill = new SolidBrush(Color.Yellow);
            }
            symbolLayer.RenderRequired = true;
            mapBox1.Refresh();
        }
    }
}
