using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SO.Library.Drawing.Barcode
{
    /// <summary>
    /// インタリーブ2of5形式のバーコードリーダクラス
    /// </summary>
    internal sealed class InterleavedBarcodeReader : BarcodeReader2of5
    {
        #region インスタンス変数

        /// <summary>太い方の白バーの幅</summary>
        private int _whiteWideWeight;

        /// <summary>細い方の白バーの幅</summary>
        private int _whiteNarrowWeight;

        #endregion

        #region InitializeBarcodeFormatInfo - バーコード形式情報初期化

        /// <summary>
        /// バーコード形式情報クラスを初期化します。
        /// </summary>
        /// <param name="info">(出力引数)バーコード形式情報クラス</param>
        protected override void InitializeBarcodeFormatInfo(out BarcodeFormatInfo info)
        {
            info = new BarcodeFormatInfo(3, 10, 3, new[] { 1, 1, 2, 2, 4, 4, 7, 7, 0, 0 });
        }

        #endregion

        #region ResetBarStartPoint - スタートコード開始座標設定

        /// <summary>
        /// スタートコードの開始座標を設定します。
        /// </summary>
        /// <param name="x">(出力引数)スタートコードの開始X座標</param>
        /// <param name="y">(出力引数)スタートコードの開始Y座標</param>
        /// <returns>スタートコードの開始座標が見つかった場合はtrue</returns>
        protected override bool ResetBarStartPoint(out int x, out int y)
        {
            for (y = 0, x = 0; y < _bmp.Height; y++)
            {
                for (x = 0; x < _bmp.Width; x++)
                {
                    if (IsBlackPixel(x, y))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        #endregion

        #region ReadStartPart - スタートコード解析

        /// <summary>
        /// スタートコードを解析します。
        /// </summary>
        /// <remarks>
        /// 本メソッドの終了時、x及びyはスタートコードの終了座標の1つ次の座標となります。
        /// </remarks>
        /// <param name="x">(参照引数)現在のX座標</param>
        /// <param name="y">(参照引数)現在のY座標</param>
        /// <returns>スタートコードの形式が正しい場合はtrue</returns>
        protected override bool ReadStartPart(ref int x, int y)
        {
            if (!PreScanWeights(x, y))
            {
                return false;
            }

            int weight = 0;
            int barCnt = 0;
            var weights = new int[_formatInfo.StartBarCount];

            for (; x < _bmp.Width; x++)
            {
                if (IsBlackPixel(x, y))
                {
                    weight++;
                }
                else
                {
                    if (weight > 0)
                    {
                        weights[barCnt] = weight;
                        weight = 0;

                        barCnt++;
                    }
                }

                if (barCnt == _formatInfo.StartBarCount)
                {
                    break;
                }
            }

            if (barCnt < _formatInfo.StartBarCount)
            {
                return false;
            }

            return IsNarrow(weights[0]) && IsNarrow(weights[1]);
        }

        #endregion

        #region PreScanWeights - バー幅事前解析

        /// <summary>
        /// 本解析の前にバーコード全体を事前解析し、各バーの幅を取得します。
        /// </summary>
        /// <param name="x">解析開始地点のX座標</param>
        /// <param name="y">解析開始地点のY座標</param>
        /// <returns>不正な形式のバーコードの場合はfalse</returns>
        private bool PreScanWeights(int x, int y)
        {
            int maxBarNum = _formatInfo.StartBarCount + _formatInfo.StopBarCount
                + _formatInfo.ValueBarCount * Digit;

            var blackWeightList = new List<int>(maxBarNum);
            var whiteWeightList = new List<int>(maxBarNum);
            bool isCurrentColorBlack = true;
            int weight = 0;
            int barCnt = 0;

            for (; blackWeightList.Count < maxBarNum && x < _bmp.Width; x++)
            {
                if (IsBlackPixel(x, y))
                {
                    if (isCurrentColorBlack)
                    {
                        weight++;
                    }
                    else
                    {
                        blackWeightList.Add(weight);

                        barCnt++;
                        weight = 0;
                        isCurrentColorBlack = false;
                    }
                }
                else
                {
                    if (!isCurrentColorBlack)
                    {
                        weight++;
                    }
                    else
                    {
                        if (barCnt >= _formatInfo.StartBarCount + 1
                            && barCnt < maxBarNum - _formatInfo.StopBarCount)
                        {
                            whiteWeightList.Add(weight);
                            barCnt++;
                        }

                        weight = 0;
                        isCurrentColorBlack = true;
                    }
                }
            }

            if (barCnt < maxBarNum)
            {
                return false;
            }

            _blackWideWeight = blackWeightList.Max();
            _blackNarrowWeight = blackWeightList.Min();

            _whiteWideWeight = whiteWeightList.Max();
            _whiteNarrowWeight = whiteWeightList.Min();

            return true;
        }

        #endregion

        #region ReadValuePart - 値部解析

        /// <summary>
        /// 値部を解析し、解析結果の文字列を返します。
        /// </summary>
        /// <remarks>
        /// 本メソッドの終了時、x及びyは値部の終了座標の1つ次の座標となります。
        /// </remarks>
        /// <param name="x">(参照引数)現在のX座標</param>
        /// <param name="y">(参照引数)現在のY座標</param>
        /// <returns>解析結果の文字列。不正な形式の場合はnull</returns>
        protected override string ReadValuePart(ref int x, int y)
        {
            for (; !IsBlackPixel(x, y) && x < _bmp.Width; x++) ;

            if (x >= _bmp.Width)
            {
                return null;
            }

            string barcode = string.Empty;
            var barWeights = new bool[Digit, _formatInfo.ValueBarCount];
            int weight = 0;
            int barCnt = 0;
            int setCnt = 0;
            bool isCurrentColorBlack = true;

            for (; setCnt < Digit && x < _bmp.Width; x++)
            {
                if (IsBlackPixel(x, y))
                {
                    if (isCurrentColorBlack)
                    {
                        weight++;
                    }
                    else
                    {
                        barWeights[setCnt, barCnt] = IsWide(weight);
                        weight = 0;

                        barCnt++;
                    }
                }
                else
                {
                    if (!isCurrentColorBlack)
                    {
                        weight++;
                    }
                    else
                    {
                        barWeights[setCnt, barCnt] = IsWideWhite(weight);
                        weight = 0;

                        barCnt++;
                    }
                }

                if (barCnt == _formatInfo.ValueBarCount)
                {
                    setCnt++;
                    barCnt = 0;
                }
            }

            if (setCnt < Digit)
            {
                return null;
            }

            for (int i = 0; i < barWeights.GetLength(0); i++)
            {
                for (int offset = 0; offset <= 1; offset++)
                {
                    int val = 0;
                    int boldCnt = 0;

                    for (int j = offset; j < barWeights.GetLength(1); j += 2)
                    {
                        if (barWeights[i, j])
                        {
                            val += _formatInfo.BarValues[j];
                            boldCnt++;
                        }
                    }

                    if (boldCnt > 2)
                    {
                        return null;
                    }

                    if (val > 9)
                    {
                        val = 0;
                    }

                    barcode += val.ToString();
                }
            }

            return barcode;
        }

        #endregion

        #region ReadStopPart - ストップコード解析

        /// <summary>
        /// ストップコードを解析します。
        /// </summary>
        /// <remarks>
        /// 本メソッドの終了時、x及びyはストップコードの終了座標の1つ次の座標となります。
        /// </remarks>
        /// <param name="x">(参照引数)現在のX座標</param>
        /// <param name="y">(参照引数)現在のY座標</param>
        /// <returns>ストップの形式が正しい場合はtrue</returns>
        protected override bool ReadStopPart(ref int x, int y)
        {
            int weight = 0;
            int barCnt = 0;
            var weights = new int[_formatInfo.StopBarCount];

            for (; x < _bmp.Width; x++)
            {
                if (IsBlackPixel(x, y))
                {
                    weight++;
                }
                else
                {
                    if (weight > 0)
                    {
                        weights[barCnt] = weight;
                        weight = 0;

                        barCnt++;
                    }
                }

                if (barCnt == _formatInfo.StopBarCount)
                {
                    break;
                }
            }

            if (barCnt < _formatInfo.StopBarCount)
            {
                return false;
            }

            return IsWide(weights[0]) && IsNarrow(weights[1]);
        }

        #endregion

        #region IsWideWhite - 指定幅が太い方の白バーの幅であるかを判定

        /// <summary>
        /// 指定された幅が、太い方の白バーの幅であるかを判定します。
        /// </summary>
        /// <remarks>
        /// 太い白バーと細い白バーの内、太い白バーに近い幅であればtrueが返されます。
        /// </remarks>
        /// <param name="weight">判定する幅</param>
        /// <returns>true:太い白バー / false:細い白バー</returns>
        private bool IsWideWhite(int weight)
        {
            return Math.Abs(weight - _whiteWideWeight)
                < Math.Abs(weight - _whiteNarrowWeight);
        }

        #endregion

        #region IsNarrowWhite - 指定幅が細い方の白バーの幅であるかを判定

        /// <summary>
        /// 指定された幅が、細い方の白バーの幅であるかを判定します。
        /// </summary>
        /// <remarks>
        /// 太い白バーと細い白バーの内、細い白バーに近い幅であればtrueが返されます。
        /// </remarks>
        /// <param name="weight">判定する幅</param>
        /// <returns>true:細い白バー / false:太い白バー</returns>
        private bool IsNarrowWhite(int weight)
        {
            return Math.Abs(weight - _whiteWideWeight)
                > Math.Abs(weight - _whiteNarrowWeight);
        }

        #endregion
    }
}
