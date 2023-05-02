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
        //クラス変数
        Coordinate g_worldPos = new Coordinate();
        System.Drawing.Point g_imagePos = new System.Drawing.Point();

        //コンストラクタ
        public Form1()
        {
            InitializeComponent();

            //SharpMap初期化
            this.InitializeMap();
        }

        //マップ初期化
        private void InitializeMap()
        {
            this.InitializeBaseLayer();
            this.InitializePointLayer();
            this.InitializeLineStringLayer();
            this.InitializeTestLayer();
            this.InitializeTest2Layer();

            //Zoom制限
            mapBox1.Map.MinimumZoom = 0.1;
            mapBox1.Map.MaximumZoom = 360.0;

            //レイヤ全体を表示する(全レイヤの範囲にズームする)
            mapBox1.Map.ZoomToExtents();

            //mapBoxを再描画
            mapBox1.Refresh();
        }

        //基底レイヤ初期化
        private void InitializeBaseLayer()
        {
            //Map生成
            mapBox1.Map = new Map(new Size(mapBox1.Width, mapBox1.Height));
            mapBox1.Map.BackColor = System.Drawing.Color.LightBlue;

            //レイヤーの作成
            VectorLayer baseLayer = new VectorLayer("baseLayer");
            baseLayer.DataSource = new ShapeFile(@"..\..\ShapeFiles\polbnda_jpn\polbnda_jpn.shp");
            //baseLayer.DataSource = new ShapeFile(@"..\..\ShapeFiles\ne_10m_coastline\ne_10m_coastline.shp");

            baseLayer.Style.Fill = Brushes.LimeGreen;
            //baseLayer.Style.Fill = new SolidBrush(Color.LimeGreen);
            baseLayer.Style.Outline = Pens.Black;
            baseLayer.Style.EnableOutline = true;

            //マップにレイヤーを追加
            mapBox1.Map.Layers.Add(baseLayer);
        }

        //pointレイヤ初期化
        private void InitializePointLayer()
        {
            //レイヤ生成
            VectorLayer layer = new VectorLayer("pointLayer");
            //ジオメトリ生成
            List<IGeometry> igeoms = new List<IGeometry>();
            /*
            //点をジオメトリに追加
            GeometryFactory gf = new GeometryFactory();
            igeoms.Add(gf.CreatePoint(new Coordinate(135, 35)));
            */
            //ジオメトリをレイヤに反映
            GeometryProvider gpro = new GeometryProvider(igeoms);
            layer.DataSource = gpro;
            //レイヤをmapBoxに追加
            mapBox1.Map.Layers.Add(layer);
        }

        //LineStringレイヤ初期化
        private void InitializeLineStringLayer()
        {
            //レイヤ生成
            VectorLayer layer = new VectorLayer("lineStringLayer");
            //ジオメトリ生成
            List<IGeometry> igeoms = new List<IGeometry>();
            /*
            //線をジオメトリに追加
            GeometryFactory gf = new GeometryFactory();
            Coordinate[] linePos = { new Coordinate(135, 30), new Coordinate(135, 37) };
            eomColl.Add(gf.CreateLineString(linePos));
            */
            //ジオメトリをレイヤに反映
            GeometryProvider gpro = new GeometryProvider(igeoms);
            layer.DataSource = gpro;
            //レイヤをmapBoxに追加
            mapBox1.Map.Layers.Add(layer);
        }

        //Testレイヤ初期化
        private void InitializeTestLayer()
        {
            //線と点を書く start -------
            //レイヤの作成
            VectorLayer testLayer = new VectorLayer("testLayer");

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
            testLayer.DataSource = vpro;

            testLayer.Style.PointColor = Brushes.Magenta;

            //レイヤをmapBoxに追加
            mapBox1.Map.Layers.Add(testLayer);
            //線と点を書く end -------
        }

        //Testレイヤ初期化
        private void InitializeTest2Layer()
        {
            //黄色い点を書く start -------
            //レイヤの作成
            VectorLayer test2Layer = new VectorLayer("test2Layer");

            //ジオメトリ準備
            GeometryFactory ypgf = new GeometryFactory();
            List<IGeometry> ypeomColl = new List<IGeometry>();
            //点を書く
            ypeomColl.Add(ypgf.CreatePoint(new Coordinate(140, 30)));

            //レイヤに反映
            GeometryProvider ypvpro = new GeometryProvider(ypeomColl);
            test2Layer.DataSource = ypvpro;
            //点の色を指定
            test2Layer.Style.PointColor = Brushes.Yellow;

            //レイヤをmapBoxに追加
            mapBox1.Map.Layers.Add(test2Layer);
            //線と点を書く end -------
        }

        //イベント - 地図上でマウス移動
        private void mapBox1_MouseMove(Coordinate worldPos, MouseEventArgs imagePos)
        {
            UpdateWorldPos(worldPos);//地理座標系上の座標の更新
            UpdateImagePos(imagePos);//画面上のピクセル座標の更新
        }

        //イベント - 地図上でクリック
        private void mapBox1_Click(object sender, EventArgs e)
        {
            UpdatePointLayer();//pointLayerレイヤの更新
        }

        //イベント - button1クリック
        private void button1_Click(object sender, EventArgs e)
        {
            ViewWholeMap();//全体表示
        }

        //イベント - button2クリック
        private void button2_Click(object sender, EventArgs e)
        {
            UpdateTestLayer(); //testレイヤーの更新
        }

        //イベント - button3クリック
        private void button3_Click(object sender, EventArgs e)
        {
            RemoveLayerOtherThanBase(mapBox1); //ベース以外のレイヤ削除
        }

        //地理座標系上の座標の更新
        private void UpdateWorldPos(Coordinate worldPos)
        {
            g_worldPos = worldPos;

            //変換テスト
            this.label1.Text = g_worldPos.ToString() + "\n" +
                mapBox1.Map.WorldToImage(g_worldPos);
        }

        //画面上のピクセル座標の更新
        private void UpdateImagePos(MouseEventArgs imagePos)
        {
            g_imagePos.X = imagePos.X;
            g_imagePos.Y = imagePos.Y;


            //変換テスト
            this.label2.Text = g_imagePos + "\n" +
                mapBox1.Map.ImageToWorld(g_imagePos);
        }

        //pointLayerレイヤの更新
        private void UpdatePointLayer()
        {
            //レイヤ取得
            VectorLayer layer = this.GetVectorLayerByName(mapBox1, "pointLayer");
            //ジオメトリ取得
            Collection<IGeometry> igeoms = this.GetIGeometrysAll(layer);
            //点をジオメトリに追加
            GeometryFactory gf = new GeometryFactory();
            igeoms.Add(gf.CreatePoint(g_worldPos));
            //ジオメトリをレイヤに反映
            GeometryProvider gpro = new GeometryProvider(igeoms);
            layer.DataSource = gpro;
            //レイヤをmapBoxに追加
            mapBox1.Map.Layers.Add(layer);
            //mapBoxを再描画
            mapBox1.Refresh();

            //ラベルにジオメトリを一覧で表示 - pointLayerレイヤ一覧表示
            Collection<IGeometry> igeomsT = this.GetIGeometrysAll( GetVectorLayerByName(mapBox1, "pointLayer") );
            string textT = string.Empty;
            foreach (IGeometry igeom in igeomsT) { textT = textT + igeomsT.IndexOf(igeom) + " : " + igeom + "\n"; }
            this.label3.Text = textT;
        }

        //symbolレイヤーの更新
        private void UpdateTestLayer()
        {
            //レイヤ取得
            VectorLayer layer = this.GetVectorLayerByName(mapBox1, "testLayer");
            //レイヤが存在しない場合は何もしない
            if (layer == null ) { return; }
            //ジオメトリ（地図上に配置した LineString や Point など）を取得
            Collection<IGeometry> igeoms = this.GetIGeometrysAll(layer);
            //foreach (IGeometry geom in geoms) { Console.WriteLine(geom); }

            //点を削除(複数該当する場合はindex上で前にいるもの)
            foreach (IGeometry igeom in igeoms) 
            {
                //Console.WriteLine(geom.GeometryType);
                if(igeom.GeometryType == "Point")
                {
                    igeoms.Remove(igeom);
                    break;
                }
            }
            //レイヤに反映
            GeometryProvider vpro = new GeometryProvider(igeoms);
            layer.DataSource = vpro;
            //レイヤをmapBoxに追加
            mapBox1.Map.Layers.Add(layer);
            //mapBoxを再描画
            mapBox1.Refresh();
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
            VectorLayer retlayer = null;
            LayerCollection layers = mapBox.Map.Layers;
            foreach(VectorLayer layer in layers)
            {
                if (layer.LayerName == layername)
                {
                    retlayer = layer;
                    break;
                }
            }
            return retlayer;
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

        /// <summary>
        /// ジオメトリ（地図上に配置した LineString や Point など）を取得
        /// 範囲:地図全体(経度-180～180, 緯度-90～90で囲まれる四角形)
        /// </summary>
        /// <param name="layer"></param>
        private Collection<IGeometry> GetIGeometrysAll(VectorLayer layer)
        {
            if (layer == null) { return null; }

            //指定した領域の特徴を返す Envelope( x1 , x2 , y1, y2)
            //地図全体(経度-180～180, 緯度-90～90で囲まれる四角形)
            Collection<IGeometry> igeoms =
                layer.DataSource.GetGeometriesInView(
                    new GeoAPI.Geometries.Envelope(-180, 180, -90, 90)
                );
            return igeoms;

            /*
            //使用例
            foreach (IGeometry igeom in igeoms) { Console.WriteLine(geom); }
            */
        }

        /// <summary>
        /// 指定レイヤ削除
        /// </summary>
        /// <param name="mapBox"></param>
        /// <param name="layername"></param>
        private void RemoveLayer(MapBox mapBox, string layername)
        {
            //Layersのindexを初めから検索し最初に該当したレイヤを取得
            ILayer ilayer = mapBox.Map.Layers.GetLayerByName(layername);
            //symbolレイヤを削除
            mapBox1.Map.Layers.Remove(ilayer);
            //mapBoxを再描画
            mapBox1.Refresh();
        }

        /// <summary>
        /// ベース以外の全レイヤ削除
        /// <param name="mapBox"></param>
        /// </summary>
        private void RemoveLayerOtherThanBase(MapBox mapBox)
        {
            //ベース(0番目)以外のレイヤ削除
            while(mapBox.Map.Layers.Count > 1) {
                mapBox.Map.Layers.RemoveAt( (mapBox.Map.Layers.Count-1) );
            }
            //ベース以外のレイヤ初期化
            this.InitializePointLayer();
            this.InitializeLineStringLayer();
            //mapBoxを再描画
            mapBox1.Refresh();
        }

        /// <summary>
        /// レイヤ全体を表示する
        /// </summary>
        private void ViewWholeMap()
        {
            //レイヤ全体を表示する(全レイヤの範囲にズームする)
            mapBox1.Map.ZoomToExtents();
            //mapBoxを再描画
            mapBox1.Refresh();
        }
    }
}
