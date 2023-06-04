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

        WakeListGenerator wakeListGenerator = new WakeListGenerator();

        //コンストラクタ
        public Form1()
        {
            InitializeComponent();

            //SharpMap初期化
            this.InitializeMap();

            //Form1参照用
            wakeListGenerator.refForm1 = this;
            //WakeList生成
            wakeListGenerator.Generate();
        }

        //マップ初期化
        private void InitializeMap()
        {
            //baseLayerレイヤ初期化
            this.InitializeBaseLayer();

            //pointLineLayerレイヤ生成
            this.GenerateLayer("pointLineLayer");

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
            baseLayer.Style.Outline = Pens.Black;
            baseLayer.Style.EnableOutline = true;

            //マップにレイヤーを追加
            mapBox1.Map.Layers.Add(baseLayer);
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
            mapBox1.Map.Layers.Add(layer);
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
                LayerCollection layers = mapBox1.Map.Layers;
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
                }//選択したものがないならば"選択なし"をラベルに表示
                else
                {
                    this.label4.Text = $"選択なし";
                }

                //前回選択ジオメトリを更新
                g_selectedGeomPrev = g_selectedGeom;
            }
        }

        //イベント - ラジオボタン「パン」変更時
        private void radioButtonClickModePan_CheckedChanged(object sender, EventArgs e)
        {
            //「クリックモード == パン」ならばActiveToolをPanにする
            if (this.radioButtonClickModePan.Checked == true)
            {
                mapBox1.ActiveTool = MapBox.Tools.Pan;
            }
            else
            {
                mapBox1.ActiveTool = MapBox.Tools.None;
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
            if (mapBox1.ActiveTool == MapBox.Tools.Pan)
            {
                //「押した瞬間のイメージ座標」から「離れた瞬間のイメージ座標」がほぼ移動していなければ、地図は動かさない
                if ((new SharpMapHelper()).Distance(g_mouseDownImagePos, imagePos.Location) <=1.0)
                {
                    //ActiveToolをNoneとすることでパンさせない
                    mapBox1.ActiveTool = MapBox.Tools.None;

                    //指定時間（ミリ秒）後、Panに戻す
                    DelayActivePan(500);
                }
            }
        }

        //非同期処理 - 指定時間後、ActiveToolをPanにする
        private async void DelayActivePan(int msec)
        {
            await Task.Delay(msec);
            mapBox1.ActiveTool = MapBox.Tools.Pan;
        }

        //イベント - button1クリック
        private void button1_Click(object sender, EventArgs e)
        {
            (new SharpMapHelper()).ViewWholeMap(mapBox1);//全体表示
        }

        //イベント - button2クリック
        private void button2_Click(object sender, EventArgs e)
        {
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

            //地理座標→イメージ座標に変換
            this.label1.Text = g_worldPos.ToString() + "\n" +
                mapBox1.Map.WorldToImage(g_worldPos);
        }

        //画面上のイメージ座標の更新
        private void UpdateImagePos(MouseEventArgs imagePos)
        {
            g_imagePos = imagePos.Location;

            //イメージ座標→地理座標に変換
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

        //指定レイヤにPoint追加
        public void AddPointToLayer(string layername, Coordinate worldPos)
        {
            //SharpMap補助クラス
            SharpMapHelper smh = new SharpMapHelper();

            //レイヤ取得
            VectorLayer layer = smh.GetVectorLayerByName(mapBox1, layername);
            //ジオメトリ取得
            Collection<IGeometry> igeoms = smh.GetIGeometrysAll(layer);
            //点をジオメトリに追加
            GeometryFactory gf = new GeometryFactory();
            igeoms.Add(gf.CreatePoint(worldPos));
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
        public void AddLineConnectLast2PointsToLayer(string layername)
        {
            //pointLineLayerから最後の2点を取得
            Coordinate[] linePos = GetLast2Points(layername);

            if ((linePos[0] != null) && (linePos[1] != null))
            {
                //SharpMap補助クラス
                SharpMapHelper smh = new SharpMapHelper();

                //レイヤ取得
                VectorLayer layer = smh.GetVectorLayerByName(mapBox1, layername);
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

        //レイヤの線を破線にする
        public void SetLineDash(string layername)
        {
            //SharpMap補助クラス
            SharpMapHelper smh = new SharpMapHelper();
            //レイヤ取得
            VectorLayer layer = smh.GetVectorLayerByName(mapBox1, layername);

            //破線にする { 破線の長さ, 間隔 }
            layer.Style.Line.DashPattern = new float[] { 5.0F, 5.0F };
        }

        //レイヤの線を矢印にする
        public void SetLineArrow(string layername)
        {
            //SharpMap補助クラス
            SharpMapHelper smh = new SharpMapHelper();
            //レイヤ取得
            VectorLayer layer = smh.GetVectorLayerByName(mapBox1, layername);

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
            VectorLayer layer = smh.GetVectorLayerByName(mapBox1, layername);

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
            int index = mapBox1.Map.Layers.IndexOf(layer);
            //レイヤを更新
            mapBox1.Map.Layers[index] = layer;




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
            GeometryProvider Lgpro = new GeometryProvider(Ligeoms);

            labelLayer.DataSource = (Ligoms);


            //mapBoxを再描画
            mapBox1.Refresh();
            
            //====================

            //④点を非表示or透明化or別の色に塗りつぶし
            //▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲


            //▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼
            //【mapBox上にPictureBoxを置く これでは地図の拡大縮小についていけない】
            picbox.Parent = mapBox1;
            //PictureBox picbox = new PictureBox();
            //picbox.ClientSize = new Size(300, 300);
            //描画先とするImageオブジェクトを作成する
            Bitmap canvas = new Bitmap(picbox.Width, picbox.Height);
            //ImageオブジェクトのGraphicsオブジェクトを作成する
            Graphics g = Graphics.FromImage(canvas);
            //四角に内接する楕円を黒で描く
            g.DrawEllipse(Pens.Black, 10, 20, 100, 80);
            //リソースを解放する
            g.Dispose();
            // 透過にする
            //canvas.MakeTransparent();
            //PictureBox1に表示する
            picbox.Image = canvas;

            //mapBox1.Image = canvas;
            //▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲




        }

        /// <summary>
        /// 指定レイヤから最近の2つのポイントを取得する
        /// </summary>
        private Coordinate[] GetLast2Points(string layername)
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

        //指定レイヤ内で、マウスカーソルに当たるPointがあれば、そのPointを選択ジオメトリにセットする
        private bool SelectPoint(string layername)
        {
            //いずれかのPointと衝突しているか判定
            IGeometry hitIgeome = null;
            int index = new int();
            bool isSelect = (new SharpMapHelper()).CheckHitAnyPoints(ref index, ref hitIgeome, mapBox1, layername, g_worldPos);
            if (isSelect == true)
            {
                //ヒットしたPointを選択ジオメトリにセットする
                g_selectedGeom.Set(mapBox1, layername, index, hitIgeome);
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
                VectorLayer layer = smh.GetVectorLayerByName(mapBox1, g_selectedGeomPrev.layername);
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

            //今選択しているものがあるならば、そのレイヤは選択中の色を設定
            if (g_selectedGeom.igeom != null)
            {
                //SharpMap補助クラス
                SharpMapHelper smh = new SharpMapHelper();
                //レイヤ取得
                VectorLayer layer = smh.GetVectorLayerByName(mapBox1, g_selectedGeom.layername);
                //Pointの色を変更
                layer.Style.PointColor = Brushes.BlueViolet;
                layer.Style.Line = new Pen(Color.Blue, 1.5f);
                //レイヤのインデックスを取得
                int index = mapBox1.Map.Layers.IndexOf(layer);
                //レイヤを更新
                mapBox1.Map.Layers[index] = layer;
                //mapBoxを再描画
                mapBox1.Refresh();
            }
        }

        //リッチテキストボックス「レイヤリスト」更新
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
            //pointLineLayerレイヤ生成
            this.GenerateLayer("pointLineLayer");

            this.g_selectedGeom = new GeomInfo();

            //mapBoxを再描画
            mapBox.Refresh();
        }

    }
}
