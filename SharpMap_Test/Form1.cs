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

namespace SharpMap_Test
{
    public partial class Form1 : Form
    {
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
            mapBox1.Map.BackColor = Color.LightBlue;

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
            VectorLayer orgLayer = new VectorLayer("子午線");
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
    }
}
