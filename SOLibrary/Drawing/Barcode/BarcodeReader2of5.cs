using System;
using System.Drawing;

namespace SO.Library.Drawing.Barcode
{
    /// <summary>
    /// 2of5系のバーコードリーダ抽象基底クラス
    /// </summary>
    public abstract class BarcodeReader2of5
    {
        #region インスタンス変数

        /// <summary>読込画像</summary>
        protected Bitmap _bmp;

        /// <summary>太い方の黒バーの幅</summary>
        protected int _blackWideWeight = int.MinValue;

        /// <summary>細い方の黒バーの幅</summary>
        protected int _blackNarrowWeight = int.MaxValue;

        /// <summary>表現桁数</summary>
        protected int _digit = 1;

        /// <summary>バーコード形式情報</summary>
        protected BarcodeFormatInfo _formatInfo;

        #endregion

        #region プロパティ

        /// <summary>
        /// 表現桁数を取得または設定します。
        /// </summary>
        /// <remarks>
        /// 指定された値が1未満の場合は1に補正されます。
        /// </remarks>
        public int Digit
        {
            get { return _digit; }
            set { _digit = value < 1 ? 1 : value; }
        }

        #endregion

        #region コンストラクタ

        /// <summary>
        /// 規定のコンストラクタです。
        /// </summary>
        public BarcodeReader2of5()
        {
            InitializeBarcodeFormatInfo(out _formatInfo);
        }

        #endregion

        #region ReadBarcode - バーコード解析

        /// <summary>
        /// 指定画像のバーコードを解析します。
        /// 不正なバーコードの場合、nullを返します。
        /// </summary>
        /// <param name="bmp">読込画像</param>
        /// <returns>解析結果のバーコード。不正なバーコードの場合はnull</returns>
        public string ReadBarcode(Bitmap bmp)
        {
            _bmp = bmp;

            // スタートコードの開始座標を検索
            int x, y;
            if (!ResetBarStartPoint(out x, out y))
            {
                return null;
            }

            // スタートコードの終了までを解析
            if (!ReadStartPart(ref x, y))
            {
                return null;
            }

            // 値部の終了までを解析
            string barcode = ReadValuePart(ref x, y);
            if (barcode == null)
            {
                return null;
            }

            // ストップコードを解析
            if (!ReadStopPart(ref x, y))
            {
                return null;
            }

            return barcode;
        }

        #endregion

        #region IsBlackPixel - 指定された座標色が黒かどうかを判定

        /// <summary>
        /// 指定された座標の色が黒かどうかを判定します。
        /// </summary>
        /// <param name="x">X座標</param>
        /// <param name="y">Y座標</param>
        /// <returns>true:黒である / false:黒でない</returns>
        public bool IsBlackPixel(int x, int y)
        {
            Color c = _bmp.GetPixel(x, y);
            int colorSum = c.R + c.G + c.B;

            return colorSum == 0;
        }

        #endregion

        #region IsWide - 指定幅が太い方の黒バーの幅であるかを判定

        /// <summary>
        /// 指定された幅が、太い方の黒バーの幅であるかを判定します。
        /// </summary>
        /// <remarks>
        /// 太い黒バーと細い黒バーの内、太い黒バーに近い幅であればtrueが返されます。
        /// </remarks>
        /// <param name="weight">判定する幅</param>
        /// <returns>true:太い黒バー / false:細い黒バー</returns>
        protected bool IsWide(int weight)
        {
            return Math.Abs(weight - _blackWideWeight)
                < Math.Abs(weight - _blackNarrowWeight);
        }

        #endregion

        #region IsNarrow - 指定幅が細い方の黒バーの幅であるかを判定

        /// <summary>
        /// 指定された幅が、細い方の黒バーの幅であるかを判定します。
        /// </summary>
        /// <remarks>
        /// 太い黒バーと細い黒バーの内、細い黒バーに近い幅であればtrueが返されます。
        /// </remarks>
        /// <param name="weight">判定する幅</param>
        /// <returns>true:細い黒バー / false:太い黒バー</returns>
        protected bool IsNarrow(int weight)
        {
            return Math.Abs(weight - _blackWideWeight)
                > Math.Abs(weight - _blackNarrowWeight);
        }

        #endregion

        #region 抽象メソッド

        /// <summary>
        /// バーコード形式情報クラスを初期化します。
        /// </summary>
        /// <param name="info">(出力引数)バーコード形式情報クラス</param>
        protected abstract void InitializeBarcodeFormatInfo(out BarcodeFormatInfo info);

        /// <summary>
        /// スタートコードの開始座標を設定します。
        /// </summary>
        /// <param name="x">(出力引数)スタートコードの開始X座標</param>
        /// <param name="y">(出力引数)スタートコードの開始Y座標</param>
        /// <returns>スタートコードの開始座標が見つかった場合はtrue</returns>
        protected abstract bool ResetBarStartPoint(out int x, out int y);

        /// <summary>
        /// スタートコードを解析します。
        /// </summary>
        /// <remarks>
        /// 本メソッドの終了時、x及びyはスタートコードの終了座標の1つ次の座標となります。
        /// </remarks>
        /// <param name="x">(参照引数)現在のX座標</param>
        /// <param name="y">(参照引数)現在のY座標</param>
        /// <returns>スタートコードの形式が正しい場合はtrue</returns>
        protected abstract bool ReadStartPart(ref int x, int y);

        /// <summary>
        /// 値部を解析し、解析結果の文字列を返します。
        /// </summary>
        /// <remarks>
        /// 本メソッドの終了時、x及びyは値部の終了座標の1つ次の座標となります。
        /// </remarks>
        /// <param name="x">(参照引数)現在のX座標</param>
        /// <param name="y">(参照引数)現在のY座標</param>
        /// <returns>解析結果の文字列。不正な形式の場合はnull</returns>
        protected abstract string ReadValuePart(ref int x, int y);

        /// <summary>
        /// ストップコードを解析します。
        /// </summary>
        /// <remarks>
        /// 本メソッドの終了時、x及びyはストップコードの終了座標の1つ次の座標となります。
        /// </remarks>
        /// <param name="x">(参照引数)現在のX座標</param>
        /// <param name="y">(参照引数)現在のY座標</param>
        /// <returns>ストップの形式が正しい場合はtrue</returns>
        protected abstract bool ReadStopPart(ref int x, int y);

        #endregion
    }
}
