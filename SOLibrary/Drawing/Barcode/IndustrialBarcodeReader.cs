namespace SO.Library.Drawing.Barcode
{
    /// <summary>
    /// インダストリアル2of5形式のバーコードリーダクラス
    /// </summary>
    internal sealed class IndustrialBarcodeReader : BarcodeReader2of5
    {
        #region InitializeBarcodeFormatInfo - バーコード形式情報初期化

        /// <summary>
        /// バーコード形式情報クラスを初期化します。
        /// </summary>
        /// <param name="info">(出力引数)バーコード形式情報クラス</param>
        protected override void InitializeBarcodeFormatInfo(out BarcodeFormatInfo info)
        {
            info = new BarcodeFormatInfo(3, 5, 3, new[] { 1, 2, 4, 7, 0 });
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
                        if (_blackNarrowWeight > weight)
                        {
                            _blackNarrowWeight = weight;
                        }
                        if (_blackWideWeight < weight)
                        {
                            _blackWideWeight = weight;
                        }

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

            return IsWide(weights[0]) && IsWide(weights[1]) && IsNarrow(weights[2]);
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
            string barcode = string.Empty;
            var barWeights = new bool[Digit, _formatInfo.ValueBarCount];
            int weight = 0;
            int barCnt = 0;
            int setCnt = 0;

            for (; setCnt < Digit && x < _bmp.Width; x++)
            {
                if (IsBlackPixel(x, y))
                {
                    weight++;
                }
                else
                {
                    if (weight > 0)
                    {
                        barWeights[setCnt, barCnt] = IsWide(weight);
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
                int val = 0;
                int boldCnt = 0;

                for (int j = 0; j < barWeights.GetLength(1); j++)
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

            return IsWide(weights[0]) && IsNarrow(weights[1]) && IsWide(weights[2]);
        }

        #endregion
    }
}
