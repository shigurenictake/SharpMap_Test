using GeoAPI.Geometries;
using SharpMap.Forms;
using SharpMap.Layers;
using System;
using System.Collections.ObjectModel;

namespace SharpMap_Test
{
    //SharpMap補助クラス
    internal class SharpMapHelper
    {
        /// <summary>
        /// VectorLayer型でレイヤ取得
        /// メリット：DataSourceを参照できる
        /// </summary>
        /// <param name="mapBox"></param>
        /// <param name="layername"></param>
        /// <returns></returns>
        public VectorLayer GetVectorLayerByName(MapBox mapBox, string layername)
        {
            VectorLayer retlayer = null;
            LayerCollection layers = mapBox.Map.Layers;
            foreach (VectorLayer layer in layers)
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
        /// レイヤ内の全ジオメトリ（地図上に配置した LineString や Point など）を取得
        /// 範囲:地図全体(経度-180～180, 緯度-90～90で囲まれる四角形)
        /// </summary>
        /// <param name="layer"></param>
        public Collection<IGeometry> GetIGeometrysAll(VectorLayer layer)
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
        public void RemoveLayer(MapBox mapBox, string layername)
        {
            //Layersのindexを初めから検索し最初に該当したレイヤを取得
            ILayer ilayer = mapBox.Map.Layers.GetLayerByName(layername);
            //symbolレイヤを削除
            mapBox.Map.Layers.Remove(ilayer);
            //mapBoxを再描画
            mapBox.Refresh();
        }

        /// <summary>
        /// レイヤ全体を表示する
        /// </summary>
        /// <param name="mapBox"></param>
        public void ViewWholeMap(MapBox mapBox)
        {
            //レイヤ全体を表示する(全レイヤの      範囲にズームする)
            mapBox.Map.ZoomToExtents();
            //mapBoxを再描画
            mapBox.Refresh();
        }

        /// <summary>
        /// いずれかのPointと衝突しているか判定
        /// </summary>
        /// <param name="rIndex"></param>
        /// <param name="rHitIgeome"></param>
        /// <param name="mapbox"></param>
        /// <param name="layername"></param>
        /// <param name="nowWorldPos"></param>
        /// <returns></returns>
        public bool CheckHitAnyPoints(
            ref int rIndex,
            ref IGeometry rHitIgeome, 
            MapBox mapbox, 
            string layername, 
            Coordinate nowWorldPos
        )
        {
            //レイヤからジオメトリリストを取得
            //レイヤ取得
            VectorLayer layer = this.GetVectorLayerByName(mapbox, layername);
            //ジオメトリ取得
            Collection<IGeometry> igeoms = this.GetIGeometrysAll(layer);

            //座標を取得ピクセル座標に変換
            System.Drawing.PointF nowImagePos = mapbox.Map.WorldToImage(nowWorldPos);

            //レイヤ内の全ジオメトリの中から衝突するPointを探す
            int index = 0;
            IGeometry hitIgeome = null;
            bool ret = false;
            foreach (IGeometry igeom in igeoms)
            {
                if (igeom.GeometryType == "Point")
                {
                    //座標を取得ピクセル座標に変換
                    System.Drawing.PointF pointImagePos = mapbox.Map.WorldToImage(igeom.Coordinate);

                    //衝突するかチェック
                    if (Distance(nowImagePos, pointImagePos) <= 6.0)
                    {
                        //衝突したジオメトリを取得して、ループを抜ける
                        hitIgeome = igeom;
                        ret = true;
                        break;
                    }
                }
                index++;
            }

            if(ret == false)
            {
                index = -1;
            }

            rIndex = index;
            rHitIgeome = hitIgeome;
            return ret;
        }

        /// <summary>
        /// 2点間の距離
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public double Distance(System.Drawing.PointF a, System.Drawing.PointF b)
        {
            return ( Math.Sqrt( (Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2)) ) );
        }

    }
}
