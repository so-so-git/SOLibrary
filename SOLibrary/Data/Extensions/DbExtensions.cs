using System;

namespace SO.Library.Data.Extensions
{
    /// <summary>
    /// データベースアクセス処理に関する拡張メソッド定義クラス
    /// </summary>
    public static class DbExtensions
    {
        /// <summary>
        /// 渡されたオブジェクトを文字列に変換します。
        /// オブジェクトがDBNullの場合、nullに変換されます。
        /// </summary>
        /// <param name="source">変換前のオブジェクト</param>
        /// <returns>変換後の文字列</returns>
        public static string ToStringWithDBNullToNull(this object source)
        {
            if (source == DBNull.Value)
            {
                return null;
            }

            return source.ToString();
        }

        /// <summary>
        /// 渡されたオブジェクトを文字列に変換します。
        /// オブジェクトがDBNullの場合、String.Emptyに変換されます。
        /// </summary>
        /// <param name="source">変換前のオブジェクト</param>
        /// <returns>変換後の文字列</returns>
        public static string ToStringWithDBNullToEmpty(this object source)
        {
            if (source == DBNull.Value)
            {
                return string.Empty;
            }

            return source.ToString();
        }

        /// <summary>
        /// 渡されたオブジェクトを指定された値型に変換します。
        /// オブジェクトがDBNullの場合、nullに変換されます。
        /// </summary>
        /// <typeparam name="T">変換後の値の型</typeparam>
        /// <param name="source">変換前のオブジェクト</param>
        /// <returns>変換後の値</returns>
        public static T? ToValueWithDBNullToNull<T>(this object source) where T : struct
        {
            if (source == DBNull.Value)
            {
                return null;
            }

            return (T)source;
        }

        /// <summary>
        /// 渡されたオブジェクトを指定された値型に変換します。
        /// オブジェクトがDBNullの場合、指定された値型の既定値に変換されます。
        /// </summary>
        /// <typeparam name="T">変換後の値の型</typeparam>
        /// <param name="source">変換前のオブジェクト</param>
        /// <returns>変換後の値</returns>
        public static T ToValueWithDBNullToDefault<T>(this object source) where T : struct
        {
            if (source == DBNull.Value)
            {
                return default(T);
            }

            return (T)source;
        }
    }
}
