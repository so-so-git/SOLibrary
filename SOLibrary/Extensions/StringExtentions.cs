using System.Linq;
using System.IO;

namespace SO.Library.Extensions
{
    /// <summary>
    /// 文字列拡張メソッド提供クラス
    /// </summary>
    public static class StringExtentions
    {
        #region HasInvalidPathChar - ファイルパス用禁則文字チェック(string拡張)

        /// <summary>
        /// (System.Stringクラス拡張)
        /// 文字列内にファイルパスとして使用できない文字が含まれているか確認します。
        /// </summary>
        /// <param name="target">チェック対象文字列</param>
        /// <param name="excludes">例外文字(含まれていてもOKとする文字)</param>
        /// <returns>禁止文字が含まれる場合:true、含まれない場合:false</returns>
        public static bool HasInvalidPathChar(this string target, params char[] excludes)
        {
            if (string.IsNullOrEmpty(target))
            {
                return false;
            }

            return Path.GetInvalidFileNameChars().Any(x =>
                {
                    if (excludes.Contains(x))
                    {
                        return false;
                    }

                    return target.IndexOf(x) != -1;
                });
        }

        #endregion

        #region BlankToNull - ブランク→Null変換(string拡張)

        /// <summary>
        /// (System.Stringクラス拡張)
        /// 文字列がString.Emptyの場合null(VisualBasic.NETの場合はNothing)に変換します。
        /// 文字列がString.Empty以外の場合はそのままの文字列を返します。
        /// </summary>
        /// <param name="source">変換対象文字列</param>
        /// <returns>null(Nothing)、もしくは変換対象文字列</returns>
        public static object BlankToNull(this string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return null;
            }

            return source;
        }

        #endregion

        #region NullToBlank - Null→ブランク変換(string拡張)

        /// <summary>
        /// (System.Stringクラス拡張)
        /// 文字列がnull(VisualBasic.NETの場合はNothing)の場合String.Emptyに変換します。
        /// 文字列がnull以外の場合はそのままの文字列を返します。
        /// </summary>
        /// <param name="source">変換対象文字列</param>
        /// <returns>String.Empty、もしくは変換対象文字列</returns>
        public static object NullToBlank(this string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return string.Empty;
            }

            return source;
        }

        #endregion
    }
}
