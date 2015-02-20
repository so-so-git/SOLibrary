using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;

namespace SO.Library.Drawing
{
    #region class ImageUtilities - 汎用画像処理機能提供クラス
    /// <summary>
    /// 汎用画像処理機能提供クラス
    /// </summary>
    public static class ImageUtilities
    {
        #region クラス定数

        /// <summary>NTSCの赤の加重率</summary>
        private const int NTSC_R_RATIO = (int)(0.298912 * 1024);
        /// <summary>NTSCの緑の加重率</summary>
        private const int NTSC_G_RATIO = (int)(0.586611 * 1024);
        /// <summary>NTSCの青の加重率</summary>
        private const int NTSC_B_RATIO = (int)(0.114478 * 1024);

        #endregion

        #region Trim - 画像トリミング
        /// <summary>
        /// 画像をトリミングします。
        /// (ピクセル形式はFormat32bppArgb固定です)
        /// </summary>
        /// <param name="src">トリミング元画像</param>
        /// <param name="rect">トリミング範囲</param>
        /// <returns>トリミングした画像</returns>
        public static Bitmap Trim(this Bitmap src, Rectangle rect)
        {
            return Trim(src, rect, PixelFormat.Format32bppArgb);
        }

        /// <summary>
        /// 画像をトリミングします。
        /// </summary>
        /// <param name="src">トリミング元画像</param>
        /// <param name="rect">トリミング範囲</param>
        /// <param name="pxFormat">ピクセル形式</param>
        /// <returns>トリミングした画像</returns>
        public static Bitmap Trim(this Bitmap src, Rectangle rect, PixelFormat pxFormat)
        {
            return src.Clone(rect, pxFormat);
        }
        #endregion

        #region ToMonoScale - モノクローム変換
        /// <summary>
        /// 指定された手法と閾値を用いて画像をモノクロームに変換します。
        /// </summary>
        /// <param name="src">元画像</param>
        /// <param name="method">グレースケール変換手法</param>
        /// <param name="threshold">白黒の閾値</param>
        /// <returns>モノクロームに変換した画像</returns>
        public static Bitmap ToMonoScale(Bitmap src, GrayScaleMethod method, MonoThrethould threshold)
        {
            return ToMonoScaleCommon(src, method, threshold);
        }
        #endregion

        #region ToGrayScale - グレースケール変換
        /// <summary>
        /// 指定された手法で画像をグレースケールに変換します。
        /// </summary>
        /// <param name="src">元画像</param>
        /// <param name="method">グレースケール変換手法</param>
        /// <returns>グレースケールに変換した画像</returns>
        public static Bitmap ToGrayScale(Bitmap src, GrayScaleMethod method)
        {
            return ToMonoScaleCommon(src, method, null);
        }
        #endregion

        #region ToMonoScaleCommon - モノクローム・グレースケール変換
        /// <summary>
        /// モノクローム変換とグレースケール変換の共通処理です。
        /// thresholdがnullの場合はグレースケールに、
        /// null以外の場合はモノクロームに変換します。
        /// </summary>
        /// <param name="src">元画像</param>
        /// <param name="method">グレースケール変換手法</param>
        /// <param name="threshold">白黒の閾値</param>
        /// <returns>モノクロームもしくはグレースケールに変換した画像</returns>
        private static Bitmap ToMonoScaleCommon(Bitmap src, GrayScaleMethod method, MonoThrethould threshold)
        {
            Bitmap dest = null;
            BitmapData srcData = null;
            BitmapData destData = null;
            try
            {
                // 画像入出力用のポインタを取得
                dest = new Bitmap(src.Width, src.Height);
                srcData = src.LockBits(new Rectangle(0, 0, src.Width, src.Height),
                        ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                destData = dest.LockBits(new Rectangle(0, 0, dest.Width, dest.Height),
                        ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

                // 元画像のピクセルデータを取得
                var pixels = new byte[src.Width * src.Height * 4];
                Marshal.Copy(srcData.Scan0, pixels, 0, pixels.Length);

                Func<byte[], int, byte> funcGetGrayByte;
                switch (method)
                {
                    case GrayScaleMethod.Basic: // 単純平均法
                        funcGetGrayByte = GetGrayByteByBasic;
                        break;

                    case GrayScaleMethod.MiddleValue:   // 中間値法
                        funcGetGrayByte = GetGrayByteByMiddleValue;
                        break;

                    case GrayScaleMethod.NTSC:  // NTSC加重平均法
                        funcGetGrayByte = GetGrayByteByNTSC;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException("method", "規定外のグレースケール手法");
                }

                for (int i = 0; i < pixels.Length; i += 4)
                {
                    // グレースケール値を算出
                    byte grayValue = funcGetGrayByte(pixels, i);

                    if (threshold == null)
                    {
                        // グレースケール変換：平均値を各色に上書き
                        pixels[i] = pixels[i + 1] = pixels[i + 2] = grayValue;
                    }
                    else
                    {
                        // モノスケール変換：閾値以上は白、閾値未満は黒として各色に上書き
                        if (grayValue >= threshold.Value)
                            pixels[i] = pixels[i + 1] = pixels[i + 2] = 0xFF;
                        else
                            pixels[i] = pixels[i + 1] = pixels[i + 2] = 0x00;
                    }

                    // alphaは最大値固定
                    pixels[i + 3] = 0xFF;
                }

                // 平均値を適用したピクセルデータを返却する画像にセット
                Marshal.Copy(pixels, 0, destData.Scan0, pixels.Length);
            }
            finally
            {
                // 画像データのポインタを破棄
                if (srcData != null) src.UnlockBits(srcData);
                if (destData != null) dest.UnlockBits(destData);
            }

            return dest;
        }
        #endregion

        #region GetGrayByteByBasic - 単純平均法を用いてグレースケール値を取得
        /// <summary>
        /// 単純平均法を用いてグレースケール値を取得します。
        /// </summary>
        /// <param name="pixels">画像全体のピクセルデータ</param>
        /// <param name="index">グレースケール値を算出するピクセルのインデックス</param>
        /// <returns>グレースケール値</returns>
        private static byte GetGrayByteByBasic(byte[] pixels, int index)
        {
            int grayValue = 0;

            // RGBの平均を算出
            grayValue += pixels[index];
            grayValue += pixels[index + 1];
            grayValue += pixels[index + 2];

            return (byte)(grayValue / 3);
        }
        #endregion

        #region GetGrayByteByMiddleValue - 中間値法を用いてグレースケール値を取得
        /// <summary>
        /// 中間値法を用いてグレースケール値を取得します。
        /// </summary>
        /// <param name="pixels">画像全体のピクセルデータ</param>
        /// <param name="index">グレースケール値を算出するピクセルのインデックス</param>
        /// <returns>グレースケール値</returns>
        private static byte GetGrayByteByMiddleValue(byte[] pixels, int index)
        {
            // RGBの最小値と最大値の平均を算出
            List<byte> sort = new List<byte>(3);
            for (int i = index; i < index + 3; ++i)
            {
                for (int j = 0; j < sort.Count; ++j)
                {
                    if (pixels[i] < sort[j])
                    {
                        sort.Insert(j, pixels[i]);
                        goto next;
                    }
                }
                sort.Add(pixels[i]);

            next: ;
            }

            byte minValue = sort[0];
            byte maxValue = sort[2];

            return (byte)((maxValue + minValue) / 2);
        }
        #endregion

        #region GetGrayByteByNTSC - NTSC係数を使用した加重平均法を用いてグレースケール値を取得
        /// <summary>
        /// NTSC係数を使用した加重平均法を用いてグレースケール値を取得します。
        /// </summary>
        /// <param name="pixels">画像全体のピクセルデータ</param>
        /// <param name="index">グレースケール値を算出するピクセルのインデックス</param>
        /// <returns>グレースケール値</returns>
        private static byte GetGrayByteByNTSC(byte[] pixels, int index)
        {
            int grayValue = 0;

            // NTSC係数を適用し平均値を算出(下位バイトからBGRAとなる)
            grayValue += pixels[index] * NTSC_B_RATIO;      // B
            grayValue += pixels[index + 1] * NTSC_G_RATIO;  // G
            grayValue += pixels[index + 2] * NTSC_R_RATIO;  // R

            // NTSC係数をintにする為に履かせた下駄の分下位にビットシフト
            grayValue >>= 10;

            return (byte)grayValue;
        }
        #endregion
    }
    #endregion

    #region GrayScaleMethod - グレースケール化手法列挙体
    /// <summary>
    /// グレースケール変換手法列挙体
    /// </summary>
    public enum GrayScaleMethod
    {
        /// <summary>単純平均法</summary>
        Basic,
        /// <summary>中間値法</summary>
        MiddleValue,
        /// <summary>NTSC係数を使用した加重平均法</summary>
        NTSC,
    }
    #endregion
}
