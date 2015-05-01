using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SO.Library.Drawing.Barcode
{
    /// <summary>
    /// バーコード形式情報クラス
    /// </summary>
    public class BarcodeFormatInfo
    {
        #region インスタンス変数

        /// <summary>スタートコードのバー本数</summary>
        public int StartBarCount { get; private set; }

        /// <summary>値部のバー本数</summary>
        public int ValueBarCount { get; private set; }

        /// <summary>ストップコードのバー本数</summary>
        public int StopBarCount { get; private set; }

        /// <summary>太いバーの値の定義</summary>
        public int[] BarValues { get; private set; }

        #endregion

        #region コンストラクタ

        /// <summary>
        /// 規定のコンストラクタです。
        /// </summary>
        /// <param name="startBarCnt">スタートコードのバー本数</param>
        /// <param name="valueBarCnt">値部のバー本数</param>
        /// <param name="stopBarCnt">ストップコードのバー本数</param>
        /// <param name="barValues">太いバーの値の定義</param>
        public BarcodeFormatInfo(int startBarCnt, int valueBarCnt, int stopBarCnt, int[] barValues)
        {
            StartBarCount = startBarCnt;
            ValueBarCount = valueBarCnt;
            StopBarCount = stopBarCnt;
            BarValues = barValues;
        }

        #endregion
    }
}
