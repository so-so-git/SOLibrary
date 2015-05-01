using System;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace SO.Library.Extensions
{
    /// <summary>
    /// 色々なクラスの拡張メソッド提供クラス
    /// </summary>
    public static class MiscExtensions
    {
        #region ToValue - 列挙値を列挙体の基となる型のインスタンスに変換(Enum拡張)

        /// <summary>
        /// (System.Enumクラス拡張)
        /// 列挙値を、所属する列挙体の基となる型のインスタンスに変換します。
        /// </summary>
        /// <typeparam name="T">変換後の型(値型限定)</typeparam>
        /// <param name="source">変換対象の列挙値</param>
        /// <returns>変換後の値</returns>
        /// <exception cref="System.InvalidCastException">型Tが列挙体の基となる型と互換性が無い場合</exception>
        public static T ToValue<T>(this Enum source) where T : struct
        {
            return (T)Enum.Parse(source.GetType(), source.ToString());
        }

        #endregion

        #region StartWithHiding - シェル画面を表示せずプロセス開始(Process拡張)

        /// <summary>
        /// (System.Diagnostics.Process拡張)
        /// シェル画面を表示せずにプロセスを開始します。
        /// </summary>
        /// <param name="proc">開始するプロセス</param>
        /// <returns>true:成功 / false:失敗</returns>
        public static bool StartWithHiding(this Process proc)
        {
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.UseShellExecute = false;

            return proc.Start();
        }

        #endregion

        #region ConvertAddressBytes - IPAddressを等価の文字列に変換(IPAddress拡張)

        /// <summary>
        /// (System.Net.IPAddress拡張)
        /// IPAddress型で示されるIPアドレスを「xxx.xxx.xxx.xxx」形式の文字列に変換します。
        /// </summary>
        /// <param name="ip">変換元のIPAddress</param>
        /// <returns>IPアドレス文字列</returns>
        public static string ConvertAddressBytes(this IPAddress ip)
        {
            string address = string.Empty;
            foreach (var addrByte in ip.GetAddressBytes())
            {
                if (address != string.Empty)
                {
                    address += ".";
                }

                address += addrByte.ToString();
            }

            return address;
        }

        #endregion

        #region ToSafeString - Nullセーフな文字列変換(Object拡張)

        /// <summary>
        /// 対象がNullまたはSystem.DBNullの場合はString.Emptyを返します。
        /// それ以外の場合は対象のToString()の結果を返します。
        /// </summary>
        /// <param name="obj">変換対象のオブジェクト</param>
        /// <returns>変換結果の文字列</returns>
        public static string ToSafeString(this object obj)
        {
            if (obj == null || Convert.IsDBNull(obj))
            {
                return string.Empty;
            }

            return obj.ToString();
        }

        #endregion
    }
}
