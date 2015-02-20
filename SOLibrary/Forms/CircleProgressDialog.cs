using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

using SO.Library.Collections;
using SO.Library.Extensions;

namespace SO.Library.Forms
{
    /// <summary>
    /// サークルプログレス表示ダイアログ
    /// </summary>
    public partial class CircleProgressDialog : Form
    {
        #region クラス定数

        /// <summary>規定のアニメーション間隔</summary>
        private const int DEFAULT_CIRCLE_RADIUS = 50;
        /// <summary>規定の表示更新間隔</summary>
        private const int DEFAULT_INTERVAL = 100;

        /// <summary>プログレスサークルを構成する部品の数</summary>
        private const int PARTS_COUNT = 8;
        /// <summary>プログレスサークルを構成する部品の規定の半径</summary>
        private const int DEFAULT_PARTS_RADIUS = 10;

        #endregion

        #region インスタンス変数

        /// <summary>プログレスサークルの中心位置</summary>
        private Point _centerPoint;

        /// <summary>プログレスサークルを構成する部品の位置の循環リスト</summary>
        private CirculateList<PointF> _partsPoints = new CirculateList<PointF>(PARTS_COUNT);
        /// <summary>プログレスサークルを構成する部品の半径</summary>
        private int _partsRadius = DEFAULT_PARTS_RADIUS;

        /// <summary>プログレスサークルを構成する部品の描画数</summary>
        private int _drawNumber = 0;
        /// <summary>プログレスサークルを構成する部品の透過率の段階</summary>
        private int _opacityStep = 6;

        /// <summary>アニメーションスレッド</summary>
        private Thread _animationThread;

        #endregion

        #region プロパティ

        /// <summary>
        /// プログレスサークルの半径を取得・設定します。
        /// </summary>
        public int CircleRadius { get; set; }

        /// <summary>
        /// プログレスサークルを構成する部品の表示更新の間隔(ミリ秒)を取得・設定します。
        /// </summary>
        public int Interval { get; set; }

        #endregion

        #region コンストラクタ

        /// <summary>
        /// 唯一のコンストラクタです。
        /// </summary>
        /// <param name="owner">親フォーム</param>
        public CircleProgressDialog(Form owner)
        {
            InitializeComponent();

            // ダイアログ表示設定
            Owner = owner;
            Location = Owner.Location;
            Size = Owner.Size;
            SetStyle(ControlStyles.Selectable, false);

            picAnime.BackColor = Color.FromArgb(127, Color.White);

            // デフォルト値設定
            CircleRadius = DEFAULT_CIRCLE_RADIUS;
            Interval = DEFAULT_INTERVAL;

            // パラメータ初期化
            ResetParameters();
        }

        #endregion

        #region ResetParameters - パラメータ再設定

        /// <summary>
        /// 各パラメータを再設定します。
        /// </summary>
        private void ResetParameters()
        {
            // 表示時のサイズを基に各パラメータを初期化
            _centerPoint = new Point(Size.Width / 2, Size.Height / 2);

            // 各構成部品の位置を算出
            double offsetRad = -90 * Math.PI / 180; // 12時方向を角度0とする為のオフセット
            double partsDeg = 360.0 / PARTS_COUNT;
            _partsPoints.Clear();
            for (int i = 0; i < PARTS_COUNT; ++i)
            {
                double rad = partsDeg * i * Math.PI / 180 + offsetRad;

                _partsPoints.Add(new PointF(
                    _centerPoint.X + (float)(CircleRadius * Math.Cos(rad)) - _partsRadius,
                    _centerPoint.Y + (float)(CircleRadius * Math.Sin(rad)) - _partsRadius));
            }
        }

        #endregion

        #region StartProgress - プログレス表示開始

        /// <summary>
        /// プログレス表示を開始します。
        /// </summary>
        public void StartProgress()
        {
            Show();
            Update();

            _animationThread = new Thread(new ThreadStart(UpdateAnimation));
            _animationThread.Start();
        }

        #endregion

        #region UpdateAnimation - アニメーション表示更新

        /// <summary>
        /// プログレスアニメーションの表示を更新します。
        /// </summary>
        private void UpdateAnimation()
        {
            using (var g = picAnime.CreateGraphics())
            {
                while (true)
                {
                    g.Clear(SystemColors.Window);

                    int perAlpha = 255 / _opacityStep;
                    float partsSize = _partsRadius * 2;
                    for (int i = 0; i <= _drawNumber; ++i)
                    {
                        int alpha = Math.Max(0, 255 - i * perAlpha);

                        var brush = new SolidBrush(Color.FromArgb(alpha, Color.Lime));
                        PointF point = _partsPoints[_partsPoints.CurrentPosition - i];
                        g.FillEllipse(brush, point.X, point.Y, partsSize, partsSize);
                    }

                    if (_drawNumber < _opacityStep) ++_drawNumber;

                    ++_partsPoints.CurrentPosition;

                    Thread.Sleep(Interval);
                }
            }
        }

        #endregion

        #region CircleProgressDialog_Resize - フォームリサイズ時

        /// <summary>
        /// フォームリサイズ時の処理です。
        /// 変更後のサイズに応じて、各パラメータを再設定します。
        /// </summary>
        /// <param orderName="sender">イベント発生元オブジェクト</param>
        /// <param orderName="e">イベント引数</param>
        private void CircleProgressDialog_Resize(object sender, EventArgs e)
        {
            try
            {
                //ResetParameters();
            }
            catch (Exception ex)
            {
                ex.DoDefault(GetType().FullName, MethodBase.GetCurrentMethod());
            }
        }

        #endregion
    }
}
