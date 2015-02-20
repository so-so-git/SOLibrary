using System;
using System.Text;
using System.Linq;

namespace SO.Library.Text
{
    #region class StringUtilities - 文字列汎用機能提供クラス
    /// <summary>
    /// 文字列汎用機能提供クラス
    /// </summary>
    public static class StringUtilities
    {
        #region メンバ変数

        /// <summary>SQL文エスケープ文字定義</summary>
        private static readonly string[,] _sqlEscapeTexts =
        {
            { "'", "''" },
        };

        /// <summary>HTMLテキストエスケープ文字定義</summary>
        private static readonly string[,] _thmlEscapeTexts =
        {
            { "&" , "&amp;" },
            { "\"", "&quot;" },
            { "'" , "&#039;" },
            { "<" , "&lt;" },
            { ">" , "&gt;" },
        };

        /// <summary>CSVテキストエスケープ文字定義</summary>
        private static readonly string[,] _csvEscapeTexts =
        {
            { "\"", "\"\"" },
        };

        #endregion

        #region === 文字列エスケープ系処理 ===

        #region enum EscapeType - エスケープタイプ列挙体
        /// <summary>
        /// エスケープタイプ列挙体
        /// </summary>
        private enum EscapeType
        {
            /// <summary>SQLテキスト</summary>
            SQL,
            /// <summary>HTMLテキスト</summary>
            HTML,
            /// <summary>CSVテキスト</summary>
            CSV,
        }
        #endregion

        #region enum EscapeDirection - エスケープ処理方向列挙体
        /// <summary>
        /// エスケープ処理方向列挙体
        /// </summary>
        private enum EscapeDirection
        {
            /// <summary>エスケープ</summary>
            Escape,
            /// <summary>アンエスケープ</summary>
            Unescape,
        }
        #endregion

        #region EscapeSqlText - SQLテキストエスケープ
        /// <summary>
        /// SQLテキストのエスケープ処理を行います。
        /// </summary>
        /// <param name="sql">SQLテキスト</param>
        /// <returns>エスケープ後のSQLテキスト</returns>
        public static string EscapeSqlText(string sql)
        {
            return EscapeCommon(sql, EscapeType.SQL, EscapeDirection.Escape);
        }
        #endregion

        #region UnescapeSqlText - SQLテキストアンエスケープ
        /// <summary>
        /// SQLテキストのアンエスケープ処理を行います。
        /// </summary>
        /// <param name="sql">SQLテキスト</param>
        /// <returns>アンエスケープ後のSQLテキスト</returns>
        public static string UnescapeSqlText(string sql)
        {
            return EscapeCommon(sql, EscapeType.SQL, EscapeDirection.Unescape);
        }
        #endregion

        #region EscapeHtmlText - HTMLテキストエスケープ
        /// <summary>
        /// HTMLテキストのエスケープ処理を行います。
        /// </summary>
        /// <param name="html">HTMLテキスト</param>
        /// <returns>エスケープ後のHTMLテキスト</returns>
        public static string EscapeHtmlText(string html)
        {
            return EscapeCommon(html, EscapeType.HTML, EscapeDirection.Escape);
        }
        #endregion

        #region UnescapeHtmlText - HTMLテキストアンエスケープ
        /// <summary>
        /// HTMLテキストのアンエスケープ処理を行います。
        /// </summary>
        /// <param name="html">HTMLテキスト</param>
        /// <returns>アンエスケープ後のHTMLテキスト</returns>
        public static string UnescapeHtmlText(string html)
        {
            return EscapeCommon(html, EscapeType.HTML, EscapeDirection.Unescape);
        }
        #endregion

        #region EscapeCsvText - CSVテキストエスケープ
        /// <summary>
        /// CSVテキストのエスケープ処理を行います。
        /// </summary>
        /// <param name="csv">CSVテキスト</param>
        /// <returns>エスケープ後のCSVテキスト</returns>
        public static string EscapeCsvText(string csv)
        {
            return EscapeCommon(csv, EscapeType.CSV, EscapeDirection.Escape);
        }
        #endregion

        #region UnescapeCsvText - CSVテキストアンエスケープ
        /// <summary>
        /// CSVテキストのアンエスケープ処理を行います。
        /// </summary>
        /// <param name="csv">CSVテキスト</param>
        /// <returns>アンエスケープ後のCSVテキスト</returns>
        public static string UnescapeCsvText(string csv)
        {
            return EscapeCommon(csv, EscapeType.CSV, EscapeDirection.Unescape);
        }
        #endregion

        #region EscapeCommon - エスケープ系共通処理
        /// <summary>
        /// エスケープ系機能の共通処理です。
        /// </summary>
        /// <param name="val">処理対象の文字列</param>
        /// <param name="type">エスケープタイプ</param>
        /// <param name="direction">エスケープ処理の方向</param>
        /// <returns>処理後の文字列</returns>
        private static string EscapeCommon(string val, EscapeType type, EscapeDirection direction)
        {
            string[,] escapeTexts = null;
            switch (type)
            {
                case EscapeType.SQL:
                    escapeTexts = _sqlEscapeTexts;
                    break;

                case EscapeType.HTML:
                    escapeTexts = _thmlEscapeTexts;
                    break;

                case EscapeType.CSV:
                    escapeTexts = _csvEscapeTexts;
                    break;

                default:
                    throw new ArgumentException(
                        "引数 type に規定外のエスケープタイプが指定されました。");
            }

            int before, after;
            if (direction == EscapeDirection.Escape)
            {
                before = 0;
                after = 1;
            }
            else
            {
                before = 1;
                after = 0;
            }

            string ret = val;
            for (int i = 0; i < escapeTexts.GetLength(0); ++i)
                ret = ret.Replace(escapeTexts[i, before], escapeTexts[i, after]);

            return ret;
        }
        #endregion

        #endregion

        #region PaddingByZero - 指定桁0埋め
        /// <summary>
        /// 整数の先頭を指定された桁数まで0で埋めた文字列を返します。
        /// </summary>
        /// <param name="source">対象整数</param>
        /// <param name="length">変換後桁数</param>
        /// <returns>0埋めされた文字列</returns>
        public static string PaddingByZero(int source, int length)
        {
            return PaddingByZero(source.ToString(), length, PaddingOption.Before);
        }

        /// <summary>
        /// 文字列の先頭を指定された桁数まで0で埋めた文字列を返します。
        /// </summary>
        /// <param name="source">対象文字列</param>
        /// <param name="length">変換後桁数</param>
        /// <returns>0埋めされた文字列</returns>
        public static string PaddingByZero(string source, int length)
        {
            return PaddingByZero(source, length, PaddingOption.Before);
        }

        /// <summary>
        /// 整数の先頭もしくは末尾を指定された桁数まで0で埋めた文字列を返します。
        /// </summary>
        /// <param name="source">対象整数</param>
        /// <param name="length">変換後桁数</param>
        /// <param name="option">0埋め方向指定</param>
        /// <returns>0埋めされた文字列</returns>
        public static string PaddingByZero(int source, int length, PaddingOption option)
        {
            return PaddingByZero(source.ToString(), length, option);
        }

        /// <summary>
        /// 文字列の先頭もしくは末尾を指定された桁数まで0で埋めた文字列を返します。
        /// </summary>
        /// <param name="source">対象文字列</param>
        /// <param name="length">変換後桁数</param>
        /// <param name="option">0埋め方向指定</param>
        /// <returns>0埋めされた文字列</returns>
        public static string PaddingByZero(string source, int length, PaddingOption option)
        {
            var ret = new StringBuilder(source);
            while (ret.Length < length)
                if (option == PaddingOption.Before) ret.Insert(0, "0");
                else ret.Append("0");

            return ret.ToString();
        }
        #endregion

        #region PaddingBySpace - 指定桁スペース埋め
        /// <summary>
        /// 整数の末尾を指定された桁数まで半角スペースで埋めた文字列を返します。
        /// </summary>
        /// <param name="source">対象整数</param>
        /// <param name="length">変換後桁数</param>
        /// <returns>半角スペース埋めされた文字列</returns>
        public static string PaddingBySpace(int source, int length)
        {
            return PaddingBySpace(source.ToString(), length, PaddingOption.After);
        }

        /// <summary>
        /// 文字列の末尾を指定された桁数まで半角スペースで埋めた文字列を返します。
        /// </summary>
        /// <param name="source">対象文字列</param>
        /// <param name="length">変換後桁数</param>
        /// <returns>半角スペース埋めされた文字列</returns>
        public static string PaddingBySpace(string source, int length)
        {
            return PaddingBySpace(source, length, PaddingOption.After);
        }

        /// <summary>
        /// 整数の末尾を指定された桁数まで半角スペースで埋めた文字列を返します。
        /// </summary>
        /// <param name="source">対象整数</param>
        /// <param name="length">変換後桁数</param>
        /// <param name="option">半角スペース埋め方向指定</param>
        /// <returns>半角スペース埋めされた文字列</returns>
        public static string PaddingBySpace(int source, int length, PaddingOption option)
        {
            return PaddingBySpace(source.ToString(), length, option);
        }

        /// <summary>
        /// 文字列の末尾を指定された桁数まで半角スペースで埋めた文字列を返します。
        /// </summary>
        /// <param name="source">対象文字列</param>
        /// <param name="length">変換後桁数</param>
        /// <param name="option">半角スペース埋め方向指定</param>
        /// <returns>半角スペース埋めされた文字列</returns>
        public static string PaddingBySpace(string source, int length, PaddingOption option)
        {
            var ret = new StringBuilder(source);
            while (ret.Length < length)
                if (option == PaddingOption.Before) ret.Insert(0, " ");
                else ret.Append(" ");

            return ret.ToString();
        }
        #endregion

        #region GetByteCount - バイト数取得
        /// <summary>
        /// 指定された文字列のバイト数を取得します。
        /// </summary>
        /// <param name="val">対象の文字列</param>
        /// <returns>文字列のバイト数</returns>
        public static int GetByteCount(string val)
        {
            return Encoding.GetEncoding(932).GetByteCount(val);
        }
        #endregion

        #region IsNarrow - 半角チェック
        /// <summary>
        /// 指定された文字列が全て半角かどうかのチェックを行います。
        /// </summary>
        /// <param name="val">チェック対象の文字列</param>
        /// <returns>true:全て半角 / false:全角が混入している</returns>
        public static bool IsNarrow(string val)
        {
            return GetByteCount(val) == val.Length;
        }
        #endregion

        #region IsWide - 全角チェック
        /// <summary>
        /// 指定された文字列が全て全角かどうかのチェックを行います。
        /// </summary>
        /// <param name="val">チェック対象の文字列</param>
        /// <returns>true:全て全角 / false:半角が混入している</returns>
        public static bool IsWide(string val)
        {
            return GetByteCount(val) == val.Length * 2;
        }
        #endregion
    }
    #endregion

    #region enum PaddingOption - 指定桁数埋め方向指定列挙体
    /// <summary>
    /// 指定桁数埋め方向指定列挙体
    /// </summary>
    public enum PaddingOption
    {
        /// <summary>先頭を埋める</summary>
        Before,
        /// <summary>末尾を埋める</summary>
        After,
    }
    #endregion
}
