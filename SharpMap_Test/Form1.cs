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
using System.Threading.Tasks;
using SharpMap.Rendering.Symbolizer;
using System.Drawing.Drawing2D;
using SharpMap.Styles;
using System.Linq;
using SharpMap.Rendering.Thematics;
using NetTopologySuite.IO;

namespace SharpMap_Test
{
    public partial class Form1 : Form
    {
        //クラス変数
        Coordinate g_worldPos = new Coordinate();                       //地理座標
        System.Drawing.Point g_imagePos = new System.Drawing.Point();   //イメージ座標

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
        GeomInfo g_selectedGeom = new GeomInfo(); //選択ジオメトリ
        GeomInfo g_selectedGeomPrev = new GeomInfo(); //前回選択ジオメトリ

        private System.Drawing.Point g_mouseDownImagePos = new System.Drawing.Point();   //マウスを押した瞬間のイメージ座標

        WakeController refWakeController = new WakeController();

        //コンストラクタ
        public Form1()
        {
            InitializeComponent();

            //SharpMap初期化
            this.InitializeMap();

            //Form1参照用
            refWakeController.refForm1 = this;
            //WakeList生成
            refWakeController.Generate();
        }

        //マップ初期化
        private void InitializeMap()
        {
            //baseLayerレイヤ初期化
            this.InitializeBaseLayer();

            //pointLineLayerレイヤ生成
            this.GenerateLayer("pointLineLayer");

            TestLayerPoint();                  //テスト Point
            TestLayerMultiPointFromCoords();   //テスト MultiPointFromCoords
            TestLayerLineString();             //テスト LineString
            TestLayerMultiLineString();        //テスト MultiLineString
            TestLayerPolygon();                //テスト Polygon
            TestLayerMultiPolygon();           //テスト MultiPolygon
            TestLayerLinearRing();             //テスト LinearRing

            TestLayerUserdata();               //テスト Userdata

            //Zoom制限
            mapBox.Map.MinimumZoom = 0.1;
            mapBox.Map.MaximumZoom = 360.0;

            //レイヤ全体を表示する(全レイヤの範囲にズームする)
            mapBox.Map.ZoomToExtents();
            
            //mapBoxを再描画
            mapBox.Refresh();
        }

        //テスト Point
        private void TestLayerPoint()
        {
            //レイヤ生成
            VectorLayer layer = new VectorLayer("TestLayerPoint");
            //ジオメトリ生成
            List<IGeometry> igeoms = new List<IGeometry>();
            //図形生成クラス
            GeometryFactory gf = new GeometryFactory();

            //点1の生成
            Coordinate pos1 = new Coordinate(140, 30);
            IPoint ipont1 = gf.CreatePoint(pos1);
            igeoms.Add(ipont1);

            //点2の生成
            Coordinate pos2 = new Coordinate(141, 31);
            IPoint ipont2 = gf.CreatePoint(pos2);
            igeoms.Add(ipont2);

            //ジオメトリをレイヤに反映
            GeometryProvider gpro = new GeometryProvider(igeoms);
            layer.DataSource = gpro;

            // レイヤーのスタイル設定
            layer.Style.PointColor = Brushes.Blue;
            layer.Style.PointSize = 7;

            //レイヤをmapBoxに追加
            mapBox.Map.Layers.Add(layer);
        }

        //テスト MultiPointFromCoords
        private void TestLayerMultiPointFromCoords()
        {
            //レイヤ生成
            VectorLayer layer = new VectorLayer("TestLayerMultiPointFromCoords");
            //ジオメトリ生成
            List<IGeometry> igeoms = new List<IGeometry>();
            //図形生成クラス
            GeometryFactory gf = new GeometryFactory();

            //点の生成
            Coordinate basecoordinate = new Coordinate(140, 30);

            Coordinate[] coordinates = new Coordinate[4];
            coordinates[0] = basecoordinate;
            coordinates[1] = new Coordinate(basecoordinate.X - 2, basecoordinate.Y + 1);
            coordinates[2] = new Coordinate(basecoordinate.X - 3, basecoordinate.Y + 2);
            coordinates[3] = new Coordinate(basecoordinate.X - 5, basecoordinate.Y + 3);
            IMultiPoint imultipoint = gf.CreateMultiPointFromCoords(coordinates);
            igeoms.Add(imultipoint);

            //ジオメトリをレイヤに反映
            GeometryProvider gpro = new GeometryProvider(igeoms);
            layer.DataSource = gpro;

            // レイヤーのスタイル設定
            layer.Style.PointColor = Brushes.Blue;
            layer.Style.PointSize = 7;

            //レイヤをmapBoxに追加
            mapBox.Map.Layers.Add(layer);

            //ジオメトリ一覧をコンソールに出力
            string text = string.Empty;
            for (int i = 0; i < igeoms.Count; i++) { text = text + $"[ {i} ] : {igeoms[i]}" + "\n"; }
            Console.WriteLine("■TestLayerMultiPointFromCoords");
            Console.WriteLine(text);
            //↓出力
            //■TestLayerMultiPointFromCoords
            //[ 0 ] : MULTIPOINT ((140 30), (138 31), (137 32), (135 33))
        }

        //テスト LineString
        private void TestLayerLineString()
        {
            //レイヤ生成
            VectorLayer layer = new VectorLayer("TestLayerLineString");
            //ジオメトリ生成
            List<IGeometry> igeoms = new List<IGeometry>();
            //図形生成クラス
            GeometryFactory gf = new GeometryFactory();

            //線1の生成
            Coordinate[] linePos1 = new Coordinate[2];
            linePos1[0] = new Coordinate(110, 45);
            linePos1[1] = new Coordinate(115, 40);
            ILineString ilinestring1 = gf.CreateLineString(linePos1);
            igeoms.Add(ilinestring1);
            //Console.WriteLine($"  ◆ilinestring1[0] = {ilinestring1.Coordinates[0].X}, {ilinestring1.Coordinates[0].Y}");

            //線2の生成
            Coordinate[] linePos2 = new Coordinate[4];
            linePos2[0] = new Coordinate(140, 35);
            linePos2[1] = new Coordinate(143, 36);
            linePos2[2] = new Coordinate(144, 38);
            linePos2[3] = new Coordinate(147, 39);
            ILineString ilinestring2 = gf.CreateLineString(linePos2);
            igeoms.Add(ilinestring2);

            //ジオメトリをレイヤに反映
            GeometryProvider gpro = new GeometryProvider(igeoms);
            layer.DataSource = gpro;

            // レイヤーのスタイル設定
            //layer.Style.PointColor = Brushes.Blue;
            layer.Style.Line = new Pen(Color.Red, 2); //ラインの色、幅
            layer.Style.Line.DashPattern = new float[] { 4.0F, 2.0F }; //破線にする { 破線の長さ, 間隔 }
            layer.Style.EnableOutline = true; //アウトラインをレンダリングするかどうか
            layer.Style.Outline = new Pen(Color.YellowGreen, 1.0f); //アウトラインの色、幅
            //layer.Style.Enabled= false; //スタイルをレンダリングするかどうか

            //レイヤをmapBoxに追加
            mapBox.Map.Layers.Add(layer);
        }

        //テスト MultiLineString
        private void TestLayerMultiLineString()
        {
            //レイヤ生成
            VectorLayer layer = new VectorLayer("TestLayerMultiLineString");
            //ジオメトリ生成
            List<IGeometry> igeoms = new List<IGeometry>();
            //図形生成クラス
            GeometryFactory gf = new GeometryFactory();

            //線1の生成
            Coordinate[] linePos1 = new Coordinate[2];
            linePos1[0] = new Coordinate(110, 45);
            linePos1[1] = new Coordinate(115, 40);
            ILineString ilinestring1 = gf.CreateLineString(linePos1);

            //線2の生成
            Coordinate[] linePos2 = new Coordinate[4];
            linePos2[0] = new Coordinate(140, 35);
            linePos2[1] = new Coordinate(143, 36);
            linePos2[2] = new Coordinate(144, 38);
            linePos2[3] = new Coordinate(147, 39);
            ILineString ilinestring2 = gf.CreateLineString(linePos2);

            ILineString[] ilinestrings = new ILineString[] { ilinestring1, ilinestring2 };
            IMultiLineString imultilinestring = gf.CreateMultiLineString( ilinestrings );
            igeoms.Add(imultilinestring);

            //ジオメトリをレイヤに反映
            GeometryProvider gpro = new GeometryProvider(igeoms);
            layer.DataSource = gpro;

            // レイヤーのスタイル設定
            //layer.Style.PointColor = Brushes.Blue;
            layer.Style.Line = new Pen(Color.Green, 2); //ラインの色、幅
            layer.Style.Line.DashPattern = new float[] { 4.0F, 2.0F }; //破線にする { 破線の長さ, 間隔 }
            layer.Style.EnableOutline = true; //アウトラインをレンダリングするかどうか
            layer.Style.Outline = new Pen(Color.YellowGreen, 1.0f); //アウトラインの色、幅
            //layer.Style.Enabled= false; //スタイルをレンダリングするかどうか

            //レイヤをmapBoxに追加
            mapBox.Map.Layers.Add(layer);

            //ジオメトリ一覧をコンソールに出力
            string text = string.Empty;
            for (int i = 0; i < igeoms.Count; i++) { text = text + $"[ {i} ] : {igeoms[i]}" + "\n"; }
            Console.WriteLine("■TestLayerMultiLineString");
            Console.WriteLine(text);
            //↓出力
            //■TestLayerMultiLineString
            //[ 0 ] : MULTILINESTRING ((110 45, 115 40), (140 35, 143 36, 144 38, 147 39))
        }

        //テスト Polygon
        private void TestLayerPolygon()
        {
            //レイヤ生成
            VectorLayer layer = new VectorLayer("TestLayerPolygon");
            //ジオメトリ生成
            List<IGeometry> igeoms = new List<IGeometry>();
            //図形生成クラス
            GeometryFactory gf = new GeometryFactory();

            // 多角形 塗りつぶし
            //   開始点と終点を同じにしなければ例外エラーとなる
            Coordinate basepos = new Coordinate(135, 38);
            Coordinate[] coordinates = new Coordinate[6]{
                new Coordinate(basepos),
                new Coordinate(basepos.X + 3, basepos.Y - 6),
                new Coordinate(basepos.X - 4, basepos.Y - 2),
                new Coordinate(basepos.X + 4, basepos.Y - 2),
                new Coordinate(basepos.X - 3, basepos.Y - 6),
                new Coordinate(basepos)
            };
            var ipoly = gf.CreatePolygon(coordinates);
            igeoms.Add(ipoly);
            //Console.WriteLine($"【出力】poly.AsText() = {ipoly.AsText()}");
            //                  →【出力】poly.AsText() = POLYGON ((110 45, 115 40, 145 30, 140 35, 155 25, 110 45))

            //ジオメトリをレイヤに反映
            GeometryProvider gpro = new GeometryProvider(igeoms);
            layer.DataSource = gpro;

            // レイヤーのスタイル設定
            //layer.Style.Fill = Brushes.Red; //塗りつぶし色
            //layer.Style.Fill = new SolidBrush(Color.Empty); //塗りつぶし色 なし
            layer.Style.Fill = new SolidBrush(Color.FromArgb(128, 0, 0, 255)); //塗りつぶし色 透過あり
            layer.Style.EnableOutline = true; //アウトラインをレンダリングするかどうか
            layer.Style.Outline = new Pen(Color.Yellow, 1.0f); //アウトライン

            //レイヤをmapBoxに追加
            mapBox.Map.Layers.Add(layer);

            //ジオメトリ一覧をコンソールに出力
            string text = string.Empty;
            for (int i = 0; i < igeoms.Count; i++) { text = text + $"[ {i} ] : {igeoms[i]}" + "\n"; }
            Console.WriteLine("■TestLayerPolygon");
            Console.WriteLine(text);
            //↓出力
            //■TestLayerPolygon
            //[ 0 ] : POLYGON ((135 38, 138 32, 131 36, 139 36, 132 32, 135 38))
        }

        //テスト MultiPolygon
        private void TestLayerMultiPolygon()
        {
            //レイヤ生成
            VectorLayer layer = new VectorLayer("TestLayerMultiPolygon");
            //ジオメトリ生成
            List<IGeometry> igeoms = new List<IGeometry>();
            //図形生成クラス
            GeometryFactory gf = new GeometryFactory();

            // 多角形 塗りつぶし
            //   開始点と終点を同じにしなければ例外エラーとなる
            Coordinate basepos1 = new Coordinate(135, 38);
            Coordinate[] coordinates1 = new Coordinate[6]{
                new Coordinate(basepos1),
                new Coordinate(basepos1.X + 3, basepos1.Y - 6),
                new Coordinate(basepos1.X - 4, basepos1.Y - 2),
                new Coordinate(basepos1.X + 4, basepos1.Y - 2),
                new Coordinate(basepos1.X - 3, basepos1.Y - 6),
                new Coordinate(basepos1)
            };
            IPolygon ipoly1 = gf.CreatePolygon(coordinates1);

            Coordinate basepos2 = new Coordinate(140, 40);
            Coordinate[] coordinates2 = new Coordinate[6]{
                new Coordinate(basepos2),
                new Coordinate(basepos2.X + 3, basepos2.Y - 6),
                new Coordinate(basepos2.X - 4, basepos2.Y - 2),
                new Coordinate(basepos2.X + 4, basepos2.Y - 2),
                new Coordinate(basepos2.X - 3, basepos2.Y - 6),
                new Coordinate(basepos2)
            };
            IPolygon ipoly2 = gf.CreatePolygon(coordinates2);

            IPolygon[] ipolys = new IPolygon[] { ipoly1, ipoly2 };
            IMultiPolygon imultipoly = gf.CreateMultiPolygon(ipolys);
            igeoms.Add(imultipoly);

            //ジオメトリをレイヤに反映
            GeometryProvider gpro = new GeometryProvider(igeoms);
            layer.DataSource = gpro;

            // レイヤーのスタイル設定
            //layer.Style.Fill = Brushes.Red; //塗りつぶし色
            //layer.Style.Fill = new SolidBrush(Color.Empty); //塗りつぶし色 なし
            layer.Style.Fill = new SolidBrush(Color.FromArgb(128, 0, 0, 255)); //塗りつぶし色 透過あり
            layer.Style.EnableOutline = true; //アウトラインをレンダリングするかどうか
            layer.Style.Outline = new Pen(Color.Yellow, 1.0f); //アウトライン

            //レイヤをmapBoxに追加
            mapBox.Map.Layers.Add(layer);

            //ジオメトリ一覧をコンソールに出力
            string text = string.Empty;
            for (int i = 0; i < igeoms.Count; i++) { text = text + $"[ {i} ] : {igeoms[i]}" + "\n"; }
            Console.WriteLine("■TestLayerMultiPolygon");
            Console.WriteLine(text);
            //↓出力
            //■TestLayerMultiPolygon
            //[ 0 ] : MULTIPOLYGON (((135 38, 138 32, 131 36, 139 36, 132 32, 135 38)), ((140 40, 143 34, 136 38, 144 38, 137 34, 140 40)))
        }

        //テスト LinearRing
        private void TestLayerLinearRing()
        {
            //レイヤ生成
            VectorLayer layer = new VectorLayer("TestLayerLinearRing");
            //ジオメトリ生成
            List<IGeometry> igeoms = new List<IGeometry>();
            //図形生成クラス
            GeometryFactory gf = new GeometryFactory();

            // 多角形 塗りつぶし
            //   開始点と終点を同じにしなければ例外エラーとなる
            Coordinate basepos = new Coordinate(145, 35);
            Coordinate[] coordinates = new Coordinate[6]{
                new Coordinate(basepos),
                new Coordinate(basepos.X + 3, basepos.Y - 6),
                new Coordinate(basepos.X - 4, basepos.Y - 2),
                new Coordinate(basepos.X + 4, basepos.Y - 2),
                new Coordinate(basepos.X - 3, basepos.Y - 6),
                new Coordinate(basepos)
            };
            var ipoly = gf.CreateLinearRing(coordinates);
            igeoms.Add(ipoly);

            //ジオメトリをレイヤに反映
            GeometryProvider gpro = new GeometryProvider(igeoms);
            layer.DataSource = gpro;

            // レイヤーのスタイル設定
            layer.Style.Line = new Pen(Color.Orange, 2); //ラインの色、幅
            layer.Style.Line.DashPattern = new float[] { 4.0F, 2.0F }; //破線にする { 破線の長さ, 間隔 }
            layer.Style.EnableOutline = true; //アウトラインをレンダリングするかどうか
            layer.Style.Outline = new Pen(Color.Yellow, 5.0f); //アウトライン

            //レイヤをmapBoxに追加
            mapBox.Map.Layers.Add(layer);

            //ジオメトリ一覧をコンソールに出力
            string text = string.Empty;
            for (int i = 0; i < igeoms.Count; i++) { text = text + $"[ {i} ] : {igeoms[i]}" + "\n"; }
            Console.WriteLine("■TestLayerLinearRing");
            Console.WriteLine(text);
            //↓出力
            //■TestLayerLinearRing
            //[ 0 ] : LINEARRING (145 35, 148 29, 141 33, 149 33, 142 29, 145 35)
        }

        //テスト LineString
        private void TestLayerUserdata()
        {
            //レイヤ生成
            VectorLayer layer = new VectorLayer("TestLayerUserdata");
            //ジオメトリ生成
            List<IGeometry> igeoms = new List<IGeometry>();
            //図形生成クラス
            GeometryFactory gf = new GeometryFactory();

            //線1の生成
            Coordinate[] linePos1 = new Coordinate[2];
            linePos1[0] = new Coordinate(150, 45);
            linePos1[1] = new Coordinate(155, 40);
            ILineString ilinestring1 = gf.CreateLineString(linePos1);
            ilinestring1.UserData = "ラインストリング１";

            igeoms.Add(ilinestring1);

            //線2の生成
            Coordinate[] linePos2 = new Coordinate[4];
            linePos2[0] = new Coordinate(160, 35);
            linePos2[1] = new Coordinate(163, 36);
            linePos2[2] = new Coordinate(164, 38);
            linePos2[3] = new Coordinate(167, 39);
            ILineString ilinestring2 = gf.CreateLineString(linePos2);
            ilinestring2.UserData = "ラインストリング２";
            igeoms.Add(ilinestring2);

            //ジオメトリをレイヤに反映
            GeometryProvider gpro = new GeometryProvider(igeoms);
            layer.DataSource = gpro;

            // レイヤーのスタイル設定
            layer.Style.Line = new Pen(Color.Purple, 2); //ラインの色、幅
            layer.Style.Line.DashPattern = new float[] { 4.0F, 2.0F }; //破線にする { 破線の長さ, 間隔 }
            layer.Style.EnableOutline = true; //アウトラインをレンダリングするかどうか
            layer.Style.Outline = new Pen(Color.Olive, 1.0f); //アウトラインの色、幅

            //レイヤをmapBoxに追加
            mapBox.Map.Layers.Add(layer);

            //ジオメトリ一覧をコンソールに出力
            string text = string.Empty;
            for (int i = 0; i < igeoms.Count; i++) { text = text + $"[ {i} ] : {igeoms[i]} : {igeoms[i].UserData}" + "\n"; }
            Console.WriteLine("■TestLayerMultiLineString");
            Console.WriteLine(text);
            //↓出力
            //■TestLayerUserdata
            //[ 0 ] : LINESTRING (150 45, 155 40) : ラインストリング１
            //[ 1 ] : LINESTRING (160 35, 163 36, 164 38, 167 39) : ラインストリング２
        }

        //基底レイヤ初期化
        private void InitializeBaseLayer()
        {
            //Map生成
            mapBox.Map = new Map(new Size(mapBox.Width, mapBox.Height));
            mapBox.Map.BackColor = System.Drawing.Color.LightBlue;

            //レイヤーの作成
            VectorLayer baseLayer = new VectorLayer("baseLayer");
            baseLayer.DataSource = new ShapeFile(@"..\..\ShapeFiles\polbnda_jpn\polbnda_jpn.shp");
            //baseLayer.DataSource = new ShapeFile(@"..\..\ShapeFiles\ne_10m_coastline\ne_10m_coastline.shp");

            baseLayer.Style.Fill = Brushes.LimeGreen;
            baseLayer.Style.Outline = Pens.Black;
            baseLayer.Style.EnableOutline = true;

            //マップにレイヤーを追加
            mapBox.Map.Layers.Add(baseLayer);
        }

        //PointLineLayerレイヤ初期化
        public void GenerateLayer(string layername)
        {
            //レイヤ生成
            VectorLayer layer = new VectorLayer(layername);
            //ジオメトリ生成
            List<IGeometry> igeoms = new List<IGeometry>();
            //ジオメトリをレイヤに反映
            GeometryProvider gpro = new GeometryProvider(igeoms);
            layer.DataSource = gpro;
            layer.Style.PointColor = Brushes.Red;
            layer.Style.Line = new Pen(Color.DarkRed, 1.0f);
            //レイヤをmapBoxに追加
            mapBox.Map.Layers.Add(layer);
        }

        //イベント - 地図上でマウス移動
        private void mapBox1_MouseMove(Coordinate worldPos, MouseEventArgs imagePos)
        {
            //labelに座標表示
            UpdateWorldPos(worldPos);//地理座標系上の座標の更新
            UpdateImagePos(imagePos);//画面上のイメージ座標の更新

            //点とマウスの衝突時のアクション
            CollisionsWithPoints();//点との衝突
        }

        //イベント - 地図上でクリック(ボタンを離した瞬間)
        private void mapBox1_Click(object sender, EventArgs e)
        {
            //クリックモード == 点を描く
            if (this.radioButtonClickModeDraw.Checked == true)
            {
                AddPointToLayer("pointLineLayer", g_worldPos);//pointLineLayerレイヤに点を追加
                AddLineConnectLast2PointsToLayer("pointLineLayer");//pointLineLayerレイヤに線を追加
            }
            
            //クリックモード == 点を選択する
            if (this.radioButtonClickModeSelect.Checked == true)
            {
                //全レイヤの中からクリックした点を探索し、選択する
                LayerCollection layers = mapBox.Map.Layers;
                foreach(Layer layer in layers)
                {
                    //レイヤの点が当たっていたらを選択する
                    bool isSelect = SelectPoint(layer.LayerName);
                    if (isSelect == true)
                    {
                        break;
                    }
                }
                UpdateSelectLayer();//選択ジオメトリのレイヤを更新

                //選択したものがあるならばラベルに表示
                if (g_selectedGeom.igeom != null)
                {
                    this.label4.Text = $"{g_selectedGeom.layername} : [ {g_selectedGeom.index} ] : {g_selectedGeom.igeom}";
                    this.richTextBox1.AppendText(this.label4.Text + "\n");
                }//選択したものがないならば"選択なし"をラベルに表示
                else
                {
                    this.label4.Text = $"選択なし";
                    this.richTextBox1.Clear();
                }

                //前回選択ジオメトリを更新
                g_selectedGeomPrev = g_selectedGeom;
            }

            //クリックモード == Envelope当たり判定
            if (this.radioButtonEnvelopeHit.Checked == true)
            {
                Console.WriteLine("EnvelopeHit Check");
                //SharpMap補助クラス
                SharpMapHelper smh = new SharpMapHelper();
                //レイヤ取得
                //VectorLayer layer = smh.GetVectorLayerByName(mapBox, "pointLineLayer");
                //VectorLayer layer = smh.GetVectorLayerByName(mapBox, "testlayer");
                VectorLayer layer = smh.GetVectorLayerByName(mapBox, "wake1");
                //クリック位置の周辺矩形の当たり判定を行う
                Collection<IGeometry> igeoms = HitCheckEnvelope(layer, g_worldPos);
                if (igeoms != null)
                {
                    foreach (IGeometry igeom in igeoms) { Console.WriteLine(igeom); }
                }
                
            }
        }

        //イベント - ラジオボタン「パン」変更時
        private void radioButtonClickModePan_CheckedChanged(object sender, EventArgs e)
        {
            //「クリックモード == パン」ならばActiveToolをPanにする
            if (this.radioButtonClickModePan.Checked == true)
            {
                mapBox.ActiveTool = MapBox.Tools.Pan;
            }
            else
            {
                mapBox.ActiveTool = MapBox.Tools.None;
            }
        }

        //イベント - マウスボタンが押された瞬間
        private void mapBox1_MouseDown(Coordinate worldPos, MouseEventArgs imagePos)
        {
            //マウスボタンが押された瞬間のイメージ座標
            g_mouseDownImagePos = imagePos.Location;
        }

        //イベント - マウスボタンが離れた瞬間
        private void mapBox1_MouseUp(Coordinate worldPos, MouseEventArgs imagePos)
        {
            //ActiveToolがPanならばパン処理を行う
            if (mapBox.ActiveTool == MapBox.Tools.Pan)
            {
                //「押した瞬間のイメージ座標」から「離れた瞬間のイメージ座標」がほぼ移動していなければ、地図は動かさない
                if ((new SharpMapHelper()).Distance(g_mouseDownImagePos, imagePos.Location) <=1.0)
                {
                    //ActiveToolをNoneとすることでパンさせない
                    mapBox.ActiveTool = MapBox.Tools.None;
        
                    //指定時間（ミリ秒）後、Panに戻す
                    DelayActivePan(500);
                }
            }
        }
        //非同期処理 - 指定時間後、ActiveToolをPanにする
        private async void DelayActivePan(int msec)
        {
            await Task.Delay(msec);
            mapBox.ActiveTool = MapBox.Tools.Pan;
        }

        //イベント - button1クリック
        private void button1_Click(object sender, EventArgs e)
        {
            (new SharpMapHelper()).ViewWholeMap(mapBox);//全体表示
        }

        //イベント - button2クリック
        private void button2_Click(object sender, EventArgs e)
        {
        }

        //イベント - button3クリック
        private void button3_Click(object sender, EventArgs e)
        {
            InitLayerOtherThanBase(mapBox); //ベース以外のレイヤ初期化
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

            //地理座標→イメージ座標に変換
            this.label1.Text = g_worldPos.ToString() + "\n" +
                mapBox.Map.WorldToImage(g_worldPos);
        }

        //画面上のイメージ座標の更新
        private void UpdateImagePos(MouseEventArgs imagePos)
        {
            g_imagePos = imagePos.Location;

            //イメージ座標→地理座標に変換
            this.label2.Text = g_imagePos + "\n" +
                mapBox.Map.ImageToWorld(g_imagePos);
        }

        //点との衝突
        private void CollisionsWithPoints()
        {
            //いずれかのPointと衝突しているか判定
            IGeometry hitIgeome = null;
            int index = new int();
            bool ishit = (new SharpMapHelper()).CheckHitAnyPoints(ref index, ref hitIgeome, mapBox, "pointLineLayer", g_worldPos);
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

        //指定レイヤにPoint追加
        public void AddPointToLayer(string layername, Coordinate worldPos)
        {
            //SharpMap補助クラス
            SharpMapHelper smh = new SharpMapHelper();

            //レイヤ取得
            VectorLayer layer = smh.GetVectorLayerByName(mapBox, layername);
            //ジオメトリ取得
            Collection<IGeometry> igeoms = smh.GetIGeometrysAll(layer);
            //点をジオメトリに追加
            GeometryFactory gf = new GeometryFactory();
            igeoms.Add(gf.CreatePoint(worldPos));
            //ジオメトリをレイヤに反映
            GeometryProvider gpro = new GeometryProvider(igeoms);
            layer.DataSource = gpro;
            //レイヤのインデックスを取得
            int index = mapBox.Map.Layers.IndexOf(layer);
            //レイヤを更新
            mapBox.Map.Layers[index] = layer;
            //mapBoxを再描画
            mapBox.Refresh();
        }


        //pointLineLayerレイヤの更新(lineString追加)
        public void AddLineConnectLast2PointsToLayer(string layername)
        {
            //pointLineLayerから最後の2点を取得
            Coordinate[] linePos = GetLast2Points(layername);

            if ((linePos[0] != null) && (linePos[1] != null))
            {
                //SharpMap補助クラス
                SharpMapHelper smh = new SharpMapHelper();

                //レイヤ取得
                VectorLayer layer = smh.GetVectorLayerByName(mapBox, layername);
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
                int index = mapBox.Map.Layers.IndexOf(layer);
                //レイヤを更新
                mapBox.Map.Layers[index] = layer;
                //mapBoxを再描画
                mapBox.Refresh();
            }
        }

        //レイヤの線を破線にする
        public void SetLineDash(string layername)
        {
            //SharpMap補助クラス
            SharpMapHelper smh = new SharpMapHelper();
            //レイヤ取得
            VectorLayer layer = smh.GetVectorLayerByName(mapBox, layername);

            //破線にする { 破線の長さ, 間隔 }
            layer.Style.Line.DashPattern = new float[] { 5.0F, 5.0F };
        }

        //レイヤの線を矢印にする
        public void SetLineArrow(string layername)
        {
            //SharpMap補助クラス
            SharpMapHelper smh = new SharpMapHelper();
            //レイヤ取得
            VectorLayer layer = smh.GetVectorLayerByName(mapBox, layername);

            //矢印にする (width, height, isFilled)
            layer.Style.Line.CustomEndCap = new System.Drawing.Drawing2D.AdjustableArrowCap(8f, 8f, false);
        }

        //悩み中・・・・
        //レイヤの点に枠線を設定する
        public void SetPointOutLine(string layername)
        {

            //▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼
            //SharpMap補助クラス
            SharpMapHelper smh = new SharpMapHelper();
            //レイヤ取得
            VectorLayer layer = smh.GetVectorLayerByName(mapBox, layername);

            //点の枠線を設定する
            layer.Style.EnableOutline = true;
            layer.Style.Outline = new Pen(Color.Blue, 1.0f);
            layer.Style.Outline.Color = Color.Blue;
            layer.Style.Outline.Width = 2.0f;
            //layer.Style.Fill = Brushes.BlueViolet;
            //layer.Style.PointColor = ;
            //layer.Style.PointColor = Brushes.BlueViolet;
            //layer.Style.PointSymbolizer = ;
            //layer.Style.Fill = new SolidBrush(Color.FromArgb(24, Color.Red));

            // 点のスタイルを作成

            //半透明
            layer.Style.PointColor = new SolidBrush(Color.FromArgb(1, Color.Red));
            layer.Style.SymbolScale= 10.5f;

            layer.Style.PointSize = 30.0f;

            // レイヤーのスタイル設定
            var style = new VectorStyle();
            //style.Point = new Symbol(PointSymbolType.Circle, Color.Blue, 6);
            style.Outline = new Pen(Color.Black, 1); // アウトラインの設定
            layer.Style = style;

            //layer.Theme = new SharpMap.Rendering.Thematics.CustomTheme( new SharpMap.Rendering.Thematics.CustomTheme.GetStyleMethod());
            //layer.Theme = new SharpMap.Rendering.Thematics.UniqueValuesTheme<string>("SOV_A3", styles, m_noneStyleDefault);

            //Graphics.DrawEllipse(Pens.Red, 10, 20, 200, 150);
            //▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲

            //▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼
            //①点オブジェクトを取得

            //点オブジェクト数ループ②～④

            //②座標を取得

            //③座標の位置に円を描画

            //===============================

            //ジオメトリ取得
            Collection <IGeometry> igeoms = smh.GetIGeometrysAll(layer);
            //図形生成クラス
            GeometryFactory gf = new GeometryFactory();

            foreach (IGeometry igeom in igeoms)
            {
                if(igeom.GeometryType == "Point")
                {
                    //LinearRingを使う？
                    //Coordinate c1 = new Coordinate(130, 30);
                    //igeom.Coordinates.Append(c1);
                    //Coordinate c2 = new Coordinate(135, 35);
                    //igeom.Coordinates.Append(c2);
                    //Coordinate c3 = igeom.Coordinate;
                    //igeom.Coordinates.Append(c3);
                    //gf.CreateLinearRing(igeom.Coordinates);

                    //それとも
                    //一回り小さいPointを重ねて疑似的にアウトラインがあるように見せる？
                    //　・・・レイヤ内で色と大きさは個別設定できないので別レイヤを作る必要あり
                    //gf.CreatePoint();
                    //igeom.Length = 50;

                }
            }

            //ジオメトリをレイヤに反映
            GeometryProvider gpro = new GeometryProvider(igeoms);
            layer.DataSource = gpro;
            //レイヤのインデックスを取得
            int index = mapBox.Map.Layers.IndexOf(layer);
            //レイヤを更新
            mapBox.Map.Layers[index] = layer;

            //それとも
            //文字の描画？
            // レイヤーを作成してテキストのスタイルを設定
            LabelLayer labelLayer = new LabelLayer("Texts");
            //layer.Style = textStyle;
            GeometryFactory Lgf = new GeometryFactory();
            List<IGeometry> Ligoms = new List<IGeometry>();

            // テキストのジオメトリと属性を作成してレイヤーに追加
            var point = new Coordinate(135, 35); // テキストの座標
            //ジオメトリをレイヤに反映
            //GeometryProvider Lgpro = new GeometryProvider(Ligeoms);

            //labelLayer.DataSource = (Ligoms);

            //mapBoxを再描画
            mapBox.Refresh();
            //====================

            //④点を非表示or透明化or別の色に塗りつぶし
            //▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲

            ////▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼
            ////【mapBox上にPictureBoxを置く】
            ////   ×　mapBoxをクリックできなくなってしまう
            ////   ×　地図の拡大縮小についていけない
            //picbox.Parent = mapBox;
            ////PictureBox picbox = new PictureBox();
            ////picbox.ClientSize = new Size(300, 300);
            ////描画先とするImageオブジェクトを作成する
            //Bitmap canvas = new Bitmap(picbox.Width, picbox.Height);
            ////ImageオブジェクトのGraphicsオブジェクトを作成する
            //Graphics g = Graphics.FromImage(canvas);
            ////四角に内接する楕円を黒で描く
            //g.DrawEllipse(Pens.Black, 10, 20, 100, 80);
            ////リソースを解放する
            //g.Dispose();
            //// 透過にする
            ////canvas.MakeTransparent();
            ////PictureBox1に表示する
            //picbox.Image = canvas;
            //
            ////mapBox.Image = canvas;
            ////▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲
        }

        /// <summary>
        /// 指定レイヤから最近の2つのポイントを取得する
        /// </summary>
        private Coordinate[] GetLast2Points(string layername)
        {
            //SharpMap補助クラス
            SharpMapHelper smh = new SharpMapHelper();

            //レイヤ取得
            VectorLayer layer = smh.GetVectorLayerByName(mapBox, layername);
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

        //指定レイヤ内で、マウスカーソルに当たるPointがあれば、そのPointを選択ジオメトリにセットする
        private bool SelectPoint(string layername)
        {
            //いずれかのPointと衝突しているか判定
            IGeometry hitIgeome = null;
            int index = new int();
            bool isSelect = (new SharpMapHelper()).CheckHitAnyPoints(ref index, ref hitIgeome, mapBox, layername, g_worldPos);
            if (isSelect == true)
            {
                //ヒットしたPointを選択ジオメトリにセットする
                g_selectedGeom.Set(mapBox, layername, index, hitIgeome);
            }//マウスカーソルに衝突するPointがないならば、選択ジオメトリを初期化
            else
            {
                //初期化
                g_selectedGeom = new GeomInfo();
            }
            return isSelect;
        }

        //選択ジオメトリのレイヤを更新
        private void UpdateSelectLayer()
        {
            //前回選択しているものがあったならば、そのレイヤは未選択の色を設定
            if (g_selectedGeomPrev.igeom != null)
            {
                //SharpMap補助クラス
                SharpMapHelper smh = new SharpMapHelper();
                //レイヤ取得
                VectorLayer layer = smh.GetVectorLayerByName(mapBox, g_selectedGeomPrev.layername);
                //Pointの色を変更
                layer.Style.PointColor = Brushes.Red;
                layer.Style.Line = new Pen(Color.DarkRed, 1.0f);
                //レイヤのインデックスを取得
                int index = mapBox.Map.Layers.IndexOf(layer);
                //レイヤを更新
                mapBox.Map.Layers[index] = layer;
                //mapBoxを再描画
                mapBox.Refresh();
            }

            //今選択しているものがあるならば、そのレイヤは選択中の色を設定
            if (g_selectedGeom.igeom != null)
            {
                //SharpMap補助クラス
                SharpMapHelper smh = new SharpMapHelper();
                //レイヤ取得
                VectorLayer layer = smh.GetVectorLayerByName(mapBox, g_selectedGeom.layername);
                //Pointの色を変更
                layer.Style.PointColor = Brushes.BlueViolet;
                layer.Style.Line = new Pen(Color.Blue, 1.5f);
                //レイヤのインデックスを取得
                int index = mapBox.Map.Layers.IndexOf(layer);
                //レイヤを更新
                mapBox.Map.Layers[index] = layer;
                //mapBoxを再描画
                mapBox.Refresh();
            }
        }

        //リッチテキストボックス「レイヤリスト」更新
        private void UpdeteLayerList()
        {
            //レイヤリストの取得
            string text = null;
            LayerCollection layers = mapBox.Map.Layers;
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
            Collection<IGeometry> igeoms = smh.GetIGeometrysAll(smh.GetVectorLayerByName(mapBox, "pointLineLayer"));
            //ジオメトリ一覧をラベルに表示 (レイヤ一覧表示 )
            string text = string.Empty;
            for (int i = 0; i < igeoms.Count; i++)
            {
                text = text + $"[ {i} ] : {igeoms[i]}" + "\n";
            }
            this.richTextBoxPointLayerList.Text = text;
        }

        //レイヤのジオメトリ一覧をコンソールに出力
        private void ConsoleGeometryList(string layername)
        {
            //SharpMap補助クラス
            SharpMapHelper smh = new SharpMapHelper();

            //レイヤ内の全ジオメトリを取得
            Collection<IGeometry> igeoms = smh.GetIGeometrysAll(smh.GetVectorLayerByName(mapBox, layername));
            //ジオメトリ一覧をラベルに表示 (レイヤ一覧表示 )
            string text = string.Empty;
            for (int i = 0; i < igeoms.Count; i++)
            {
                text = text + $"[ {i} ] : {igeoms[i]}" + "\n";
            }
            Console.WriteLine(text);
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
            //pointLineLayerレイヤ生成
            this.GenerateLayer("pointLineLayer");

            this.g_selectedGeom = new GeomInfo();

            //mapBoxを再描画
            mapBox.Refresh();
        }

        //イベント - マップの中心が変更(ZoomやPanによる変更も対象)
        private void mapBox1_MapCenterChanged(Coordinate center)
        {
            //ラベルの再配置
            refWakeController.RelocateLabel();
        }

        //地図座標→イメージ座標に変換
        public System.Drawing.Point TransPosWorldToImage(Coordinate worldPos)
        {
            return System.Drawing.Point.Round(this.mapBox.Map.WorldToImage(worldPos));
        }

        //周辺矩形の当たり判定を行う
        public Collection<IGeometry> HitCheckEnvelope(VectorLayer layer, Coordinate worldPos)
        {
            if (layer == null) { return null; }

            double x1 = worldPos.X;

            //指定した領域の特徴を返す Envelope( x1 , x2 , y1, y2)
            Collection<IGeometry> igeoms =
                layer.DataSource.GetGeometriesInView(
                    new GeoAPI.Geometries.Envelope(
                        worldPos.X - 1, worldPos.X + 1,
                        worldPos.Y - 1, worldPos.Y + 1)
                );
            return igeoms;

            /*
            //使用例
            foreach (IGeometry igeom in igeoms) { Console.WriteLine(geom); }
            */
        }
    }
}
