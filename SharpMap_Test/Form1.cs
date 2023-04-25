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
            //レイヤの作成
            VectorLayer orgLayer = new VectorLayer("symbolLayer");

            //ジオメトリ準備
            GeometryFactory gf = new GeometryFactory();
            List<IGeometry> eomColl = new List<IGeometry>();
            //線を書く
            Coordinate[] linePos = { new Coordinate(135, 30), new Coordinate(135, 37) };
            eomColl.Add(gf.CreateLineString(linePos));
            //点を書く
            eomColl.Add(gf.CreatePoint(new Coordinate(135, 35)));
            //線を書く
            Coordinate[] linePosb = { new Coordinate(130, 35), new Coordinate(140, 35) };
            eomColl.Add(gf.CreateLineString(linePosb));
            //点を書く
            eomColl.Add(gf.CreatePoint(new Coordinate(145, 45)));

            //レイヤに反映
            GeometryProvider vpro = new GeometryProvider(eomColl);
            orgLayer.DataSource = vpro;

            //レイヤをmapBoxに追加
            mapBox1.Map.Layers.Add(orgLayer);
            //線と点を書く end -------

            //黄色い点を書く start -------
            //レイヤの作成
            VectorLayer ypLayer = new VectorLayer("ypLayer");

            //ジオメトリ準備
            GeometryFactory ypgf = new GeometryFactory();
            List<IGeometry> ypeomColl = new List<IGeometry>();
            //点を書く
            ypeomColl.Add(ypgf.CreatePoint(new Coordinate(140, 30)));

            //レイヤに反映
            GeometryProvider ypvpro = new GeometryProvider(ypeomColl);
            ypLayer.DataSource = ypvpro;
            //点の色を指定
            ypLayer.Style.PointColor = Brushes.Yellow;

            //レイヤをmapBoxに追加
            mapBox1.Map.Layers.Add(ypLayer);
            //線と点を書く end -------

            //Zoom制限
            mapBox1.Map.MinimumZoom = 0.1;
            mapBox1.Map.MaximumZoom = 360.0;

            //全レイヤの範囲にズームする(初期化？)
            mapBox1.Map.ZoomToExtents();

            //mapBoxを再描画
            mapBox1.Refresh();
            //======================
        }

        //イベント - 地図上でマウス移動
        private void mapBox1_MouseMove(Coordinate worldPos, MouseEventArgs imagePos)
        {
            UpdateWorldPos(worldPos);
        }

        //イベント - 地図上でクリック
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

        //symbolレイヤーの更新
        private void UpdateSymbolLayer()
        {
            //レイヤ取得
            VectorLayer rlayer = GetVectorLayerByName(mapBox1, "symbolLayer");

            //ジオメトリ（地図上に配置した LineString や Point など）を取得
            Collection<IGeometry> geoms = GetIGeometrys(mapBox1, rlayer);
            //foreach (IGeometry geom in geoms) { Console.WriteLine(geom); }

            //点を削除(複数該当する場合はindex上で前にいるもの)
            foreach (IGeometry geom in geoms) 
            {
                //Console.WriteLine(geom.GeometryType);
                if(geom.GeometryType == "Point")
                {
                    geoms.Remove(geom);
                    break;
                }
            }

            //レイヤに反映
            GeometryProvider vpro = new GeometryProvider(geoms);
            rlayer.DataSource = vpro;

            //レイヤをmapBoxに追加
            mapBox1.Map.Layers.Add(rlayer);

            //全レイヤの範囲にズームする
            mapBox1.Map.ZoomToExtents();

            //mapBoxを再描画
            mapBox1.Refresh();
        }

        /// <summary>
        /// ジオメトリ（地図上に配置した LINESTRING や POINT など）を取得
        /// </summary>
        /// <param name="mapBox"></param>
        /// <param name="layer"></param>
        private Collection<IGeometry> GetIGeometrys(MapBox mapBox, VectorLayer layer)
        {
            //指定した領域の特徴を返す Envelope( x1 , x2 , y1, y2)
            Collection<IGeometry> geoms =
                layer.DataSource.GetGeometriesInView(
                    new GeoAPI.Geometries.Envelope(-180, 180, -90, 90) //地図全体(経度-180～180, 緯度-90～90で囲まれる四角形)
                );
            return geoms;

            /*
            //使用例
            foreach (IGeometry geom in geoms) { Console.WriteLine(geom); }
            */
        }

        /// <summary>
        /// レイヤ削除
        /// </summary>
        /// <param name="mapBox"></param>
        /// <param name="layername"></param>
        private void RemoveLayer(MapBox mapBox, string layername)
        {
            //Layersのindexを初めから検索し最初に該当したレイヤを取得
            ILayer rlayer = mapBox.Map.Layers.GetLayerByName(layername);
            //symbolレイヤを削除
            mapBox1.Map.Layers.Remove(rlayer);
        }

        /// <summary>
        /// VectorLayer型でレイヤ取得
        /// メリット：DataSourceを参照できる
        /// </summary>
        /// <param name="mapBox"></param>
        /// <param name="layername"></param>
        /// <returns></returns>
        private VectorLayer GetVectorLayerByName(MapBox mapBox, string layername)
        {
            VectorLayer rlayer = new VectorLayer("");

            LayerCollection rlayers = mapBox.Map.Layers;
            foreach(VectorLayer layer in rlayers)
            {
                if (layer.LayerName == layername)
                {
                    rlayer = layer;
                    break;
                }
            }

            return rlayer;

            /*
            //使用例
            //指定した領域()の特徴を返す Envelope( x1 , x2 , y1, y2)
            Collection<IGeometry> geoms =
                rlayer.DataSource.GetGeometriesInView(
                    new GeoAPI.Geometries.Envelope(130, 140, 30, 40) //経度130～140, 緯度30～40で囲まれる四角形
                );
            foreach (IGeometry geom in geoms) { Console.WriteLine(geom); }
            */
        }
    }
}
