using GeoAPI.Geometries;//SharpMap
using NetTopologySuite.Geometries;//SharpMap
using SharpMap.Data.Providers;//SharpMap
using SharpMap.Forms;
using SharpMap.Layers;//SharpMap
using SharpMap;//SharpMap
using System;
using System.Collections.Generic;//SharpMap
using System.Drawing;//SharpMap
using System.Windows.Forms;
using System.Collections.ObjectModel;

namespace SharpMap_Test
{
    public partial class Form1 : Form
    {
        //クラス変数
        Coordinate g_worldPos = new Coordinate();                       //地理座標
        System.Drawing.Point g_imagePos = new System.Drawing.Point();   //ピクセル座標

        //ジオメトリ情報格納用
        struct GeomInfo
        {
            public MapBox mapbox;     //MapBoxオブジェクト
            public string layername;  //レイヤ名
            public int index;      //インデックス
            public IGeometry igeom;      //ジオメトリ
            //セット関数
            public void Set(MapBox mb, string ln, int id, IGeometry ig)
            {
                this.mapbox = mb;
                this.layername = ln;
                this.index = id;
                this.igeom = ig;
            }
        };
        GeomInfo selectedGeom = new GeomInfo(); //選択ジオメトリ
        GeomInfo selectedGeomPrev = new GeomInfo(); //変更前の選択ジオメトリ

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
            //baseLayerレイヤ初期化
            this.InitializeBaseLayer();

            //レイヤ初期化
            this.InitializePointLineLayer();         //pointLineLayerレイヤ初期化

            //テスト用
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

        //PointLineLayerレイヤ初期化
        private void InitializePointLineLayer()
        {
            //レイヤ生成
            VectorLayer layer = new VectorLayer("pointLineLayer");
            //ジオメトリ生成
            List<IGeometry> igeoms = new List<IGeometry>();
            //ジオメトリをレイヤに反映
            GeometryProvider gpro = new GeometryProvider(igeoms);
            layer.DataSource = gpro;
            layer.Style.PointColor = Brushes.Red;
            layer.Style.Line = new Pen(Color.DarkRed, 1.0f);
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
            List<IGeometry> eomColl = new List<IGeometry>();

            //図形生成クラス
            GeometryFactory gf = new GeometryFactory();

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
            test2Layer.Style.PointColor = Brushes.YellowGreen;

            //レイヤをmapBoxに追加
            mapBox1.Map.Layers.Add(test2Layer);
            //線と点を書く end -------
        }

        //イベント - 地図上でマウス移動
        private void mapBox1_MouseMove(Coordinate worldPos, MouseEventArgs imagePos)
        {
            //labelに座標表示
            UpdateWorldPos(worldPos);//地理座標系上の座標の更新
            UpdateImagePos(imagePos);//画面上のピクセル座標の更新

            //点との衝突時のアクション
            CollisionsWithPoints();//点との衝突
        }

        //イベント - 地図上でクリック
        private void mapBox1_Click(object sender, EventArgs e)
        {
            //クリックモード == 点を描く
            if (this.radioButtonClickModeDraw.Checked == true)
            {
                UpdatePointLayer();//pointLineLayerレイヤの更新
                UpdateLineStringLayer();//pointLineLayerレイヤの更新
            }//クリックモード == 点を選択する
            else if (this.radioButtonClickModeSelect.Checked == true)
            {
                SelectPoint();//点を選択
                UpdateSelectLayer();//selectLayerレイヤの更新
                UpdateSelectPointLayer();//selectPointLayerレイヤの更新

                //選択したものがある
                if (selectedGeom.igeom != null)
                {
                    this.label4.Text = $"[ {this.selectedGeom.index} ] : {this.selectedGeom.igeom.ToString()}";
                }//選択したものがない
                else
                {
                    this.label4.Text = $"選択なし";
                }

                selectedGeomPrev = selectedGeom;
            }
        }

        //イベント - button1クリック
        private void button1_Click(object sender, EventArgs e)
        {
            (new SharpMapHelper()).ViewWholeMap(mapBox1);//全体表示
        }

        //イベント - button2クリック
        private void button2_Click(object sender, EventArgs e)
        {
            UpdateTestLayer(); //testレイヤーの更新
        }

        //イベント - button3クリック
        private void button3_Click(object sender, EventArgs e)
        {
            InitLayerOtherThanBase(mapBox1); //ベース以外のレイヤ初期化
        }

        //イベント - button4クリック
        private void button4_Click(object sender, EventArgs e)
        {
            UpdeteLayerList(); //レイヤリストの更新
        }

        //イベント - button5クリック
        private void button5_Click(object sender, EventArgs e)
        {
            UpdeteGeometryListOfPointLineLayer(); //ジオメトリリストの更新
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

        //点との衝突
        private void CollisionsWithPoints()
        {
            //いずれかのPointと衝突しているか判定
            IGeometry hitIgeome = null;
            int index = new int();
            bool ishit = (new SharpMapHelper()).CheckHitAnyPoints(ref index, ref hitIgeome, mapBox1, "pointLineLayer", g_worldPos);
            if (ishit == true)
            {
                string txt = $"ヒットしました : [ {index} ] : " + hitIgeome.ToString();
                this.label3.Text = txt;
            }
            else
            {
                this.label3.Text = "ヒットなし";
            }
        }

        //pointLineLayerレイヤの更新(Point追加)
        private void UpdatePointLayer()
        {
            //SharpMap補助クラス
            SharpMapHelper smh = new SharpMapHelper();

            //レイヤ取得
            VectorLayer layer = smh.GetVectorLayerByName(mapBox1, "pointLineLayer");
            //ジオメトリ取得
            Collection<IGeometry> igeoms = smh.GetIGeometrysAll(layer);
            //点をジオメトリに追加
            GeometryFactory gf = new GeometryFactory();
            igeoms.Add(gf.CreatePoint(g_worldPos));
            //ジオメトリをレイヤに反映
            GeometryProvider gpro = new GeometryProvider(igeoms);
            layer.DataSource = gpro;
            //レイヤのインデックスを取得
            int index = mapBox1.Map.Layers.IndexOf(layer);
            //レイヤを更新
            mapBox1.Map.Layers[index] = layer;
            //mapBoxを再描画
            mapBox1.Refresh();
        }


        //pointLineLayerレイヤの更新(lineString追加)
        private void UpdateLineStringLayer()
        {
            //pointLineLayerから最後の2点を取得
            Coordinate[] linePos = GetRecently2Points("pointLineLayer");

            if ((linePos[0] != null) && (linePos[1] != null))
            {
                //SharpMap補助クラス
                SharpMapHelper smh = new SharpMapHelper();

                //レイヤ取得
                VectorLayer layer = smh.GetVectorLayerByName(mapBox1, "pointLineLayer");
                //ジオメトリ取得
                Collection<IGeometry> igeoms = smh.GetIGeometrysAll(layer);

                //図形生成クラス
                GeometryFactory gf = new GeometryFactory();

                //線をジオメトリに追加
                igeoms.Add(gf.CreateLineString(linePos));

                //ジオメトリをレイヤに反映
                GeometryProvider gpro = new GeometryProvider(igeoms);
                layer.DataSource = gpro;

                //レイヤのインデックスを取得
                int index = mapBox1.Map.Layers.IndexOf(layer);
                //レイヤを更新
                mapBox1.Map.Layers[index] = layer;
                //mapBoxを再描画
                mapBox1.Refresh();
            }
        }
        

        /// <summary>
        /// 指定レイヤから最近の2つのポイントを取得する
        /// </summary>
        private Coordinate[] GetRecently2Points(string layername)
        {
            //SharpMap補助クラス
            SharpMapHelper smh = new SharpMapHelper();

            //レイヤ取得
            VectorLayer layer = smh.GetVectorLayerByName(mapBox1, layername);
            //レイヤ内の全ジオメトリを取得
            Collection<IGeometry> igeoms = smh.GetIGeometrysAll(layer);

            //レイヤ内の全ジオメトリの中からPointのみを抽出
            Collection<IGeometry> pointIgeoms = new Collection<IGeometry>();
            foreach (IGeometry igeom in igeoms)
            {
                if (igeom.GeometryType == "Point")
                {
                    pointIgeoms.Add(igeom);
                }
            }
            //Pointが2つ以上ならばコレクション上、最後の2点を取得する
            Coordinate[] linePos = new Coordinate[2];
            int cnt = pointIgeoms.Count;
            if (cnt >= 2) {
                linePos[0] = pointIgeoms[cnt - 2].Coordinate;
                linePos[1] = pointIgeoms[cnt - 1].Coordinate;
            }

            return linePos;
        }

        //点を選択する
        private void SelectPoint()
        {
            //いずれかのPointと衝突しているか判定
            IGeometry hitIgeome = null;
            int index = new int();
            bool ishit = (new SharpMapHelper()).CheckHitAnyPoints(ref index, ref hitIgeome, mapBox1, "pointLineLayer", g_worldPos);
            if (ishit == true)
            {
                //ヒットした点を選択する
                selectedGeom.Set(mapBox1, "pointLineLayer", index, hitIgeome);
            }//選択していものがない
            else
            {
                //初期化
                selectedGeom = new GeomInfo();
            }
        }

        //selectPointLayerレイヤの更新
        private void UpdateSelectPointLayer()
        {
            if (selectedGeom.igeom != null)
            {
                //レイヤを削除
                (new SharpMapHelper()).RemoveLayer(mapBox1, "selectPointLayer");

                //レイヤ生成
                VectorLayer layer = new VectorLayer("selectPointLayer");
                //ジオメトリ生成
                Collection<IGeometry> igeoms = new Collection<IGeometry>();
                //点をジオメトリに追加
                GeometryFactory gf = new GeometryFactory();
                igeoms.Add(gf.CreatePoint(selectedGeom.igeom.Coordinate));
                //ジオメトリをレイヤに反映
                GeometryProvider gpro = new GeometryProvider(igeoms);
                layer.DataSource = gpro;
                layer.Style.PointSize = 12f;
                layer.Style.PointColor = Brushes.Yellow;
                //レイヤをmapBoxに追加
                mapBox1.Map.Layers.Add(layer);
                //mapBoxを再描画
                mapBox1.Refresh();
            }//選択していものがない
            else
            {
                //レイヤを削除
                (new SharpMapHelper()).RemoveLayer(mapBox1, "selectPointLayer");
            }
        }

        //selectPointLayerレイヤの更新
        private void UpdateSelectLayer()
        {
            //選択しているものがある
            if (selectedGeom.igeom != null)
            {
                //SharpMap補助クラス
                SharpMapHelper smh = new SharpMapHelper();
                //レイヤ取得
                VectorLayer layer = smh.GetVectorLayerByName(mapBox1, selectedGeom.layername);
                //Pointの色を変更
                layer.Style.PointColor = Brushes.BlueViolet;
                layer.Style.Line = new Pen(Color.Blue, 1.5f);
                //レイヤのインデックスを取得
                int index = mapBox1.Map.Layers.IndexOf(layer);
                //レイヤを更新
                mapBox1.Map.Layers[index] = layer;
                //mapBoxを再描画
                mapBox1.Refresh();
            }//選択していものがない
            else
            {
                //前回選択したものがある
                if (selectedGeomPrev.igeom != null)
                {
                    //SharpMap補助クラス
                    SharpMapHelper smh = new SharpMapHelper();
                    //レイヤ取得
                    VectorLayer layer = smh.GetVectorLayerByName(mapBox1, selectedGeomPrev.layername);
                    //Pointの色を変更
                    layer.Style.PointColor = Brushes.Red;
                    layer.Style.Line = new Pen(Color.DarkRed, 1.0f);
                    //レイヤのインデックスを取得
                    int index = mapBox1.Map.Layers.IndexOf(layer);
                    //レイヤを更新
                    mapBox1.Map.Layers[index] = layer;
                    //mapBoxを再描画
                    mapBox1.Refresh();
                }
            }
        }

        //testLayerレイヤーの更新
        private void UpdateTestLayer()
        {
            //SharpMap補助クラス
            SharpMapHelper smh = new SharpMapHelper();

            //レイヤ取得
            VectorLayer layer = smh.GetVectorLayerByName(mapBox1, "testLayer");
            //レイヤが存在しない場合は何もしない
            if (layer == null ) { return; }
            //ジオメトリ（地図上に配置した LineString や Point など）を取得
            Collection<IGeometry> igeoms = smh.GetIGeometrysAll(layer);
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

        //レイヤリストの更新
        private void UpdeteLayerList()
        {
            //レイヤリストの取得
            string text = null;
            LayerCollection layers = mapBox1.Map.Layers;
            for (int i = 0; i < layers.Count; i++) {
                text = text + $"[ {i} ] : {layers[i].LayerName }" + "\n";

            }
            richTextBoxLayerList.Text = text;
        }

        //pointLineLayerレイヤのジオメトリ一覧更新
        private void UpdeteGeometryListOfPointLineLayer()
        {
            //SharpMap補助クラス
            SharpMapHelper smh = new SharpMapHelper();

            //レイヤ内の全ジオメトリを取得
            Collection<IGeometry> igeoms = smh.GetIGeometrysAll(smh.GetVectorLayerByName(mapBox1, "pointLineLayer"));
            //ジオメトリ一覧をラベルに表示 ( pointLineLayerレイヤ一覧表示 )
            string text = string.Empty;
            for (int i = 0; i < igeoms.Count; i++)
            {
                text = text + $"[ {i} ] : {igeoms[i]}" + "\n";
            }
            this.richTextBoxPointLayerList.Text = text;
        }

        /// <summary>
        /// ベース以外の全レイヤ削除
        /// <param name="mapBox"></param>
        /// </summary>
        private void InitLayerOtherThanBase(MapBox mapBox)
        {
            //ベース(0番目)以外のレイヤ削除
            while (mapBox.Map.Layers.Count > 1)
            {
                mapBox.Map.Layers.RemoveAt((mapBox.Map.Layers.Count - 1));
            }
            //ベース以外のレイヤ初期化
            this.InitializePointLineLayer();

            this.selectedGeom = new GeomInfo();

            //mapBoxを再描画
            mapBox.Refresh();
        }
    }
}
