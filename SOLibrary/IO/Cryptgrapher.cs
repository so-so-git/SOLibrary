using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace SO.Library.IO
{
    /// <summary>
    /// 暗号化・復号機能提供クラス
    /// </summary>
    public static class Cryptgrapher
    {
        #region Encrypt - 平文文字列の暗号化

        /// <summary>
        /// 平文文字列の暗号化を行ないます。
        /// </summary>
        /// <param name="source">暗号化を行なう文字列</param>
        /// <param name="key">暗号化に用いる共有キー</param>
        /// <returns>暗号化された文字列</returns>
        public static string Encrypt(string source, string key)
        {
            // 元文字列、キーをバイト配列に変換
            byte[] bytesIn = Encoding.Unicode.GetBytes(source);
            byte[] bytesKey = Encoding.Unicode.GetBytes(key);

            var tdes = new TripleDESCryptoServiceProvider();

            // 共有キー、初期化ベクタ設定
            tdes.Key = AdjustByteLength(bytesKey, tdes.Key.Length);
            tdes.IV = AdjustByteLength(bytesKey, tdes.IV.Length);

            using (var ms = new MemoryStream())
            using (var cs = new CryptoStream(ms, tdes.CreateEncryptor(), CryptoStreamMode.Write))
            {
                // 暗号化書き込み
                cs.Write(bytesIn, 0, bytesIn.Length);
                cs.FlushFinalBlock();

                // メモリ内のバイトデータを文字列変換し返却
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        #endregion

        #region Decrypt - 暗号化文字列の復号

        /// <summary>
        /// 暗号化された文字列を復号します。
        /// </summary>
        /// <param name="source">暗号化された文字列</param>
        /// <param name="key">復号に用いる共有キー</param>
        /// <returns>復号された文字列</returns>
        public static string Decrypt(string source, string key)
        {
            var des = new TripleDESCryptoServiceProvider();

            // キーをバイト配列に変換
            byte[] bytesKey = Encoding.Unicode.GetBytes(key);

            // 共有キー、初期化ベクタ設定
            des.Key = AdjustByteLength(bytesKey, des.Key.Length);
            des.IV = AdjustByteLength(bytesKey, des.IV.Length);

            using (var ms = new MemoryStream(Convert.FromBase64String(source)))
            using (var cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Read))
            using (var sr = new StreamReader(cs, Encoding.Unicode))
            {
                // 復号されたデータを返却
                return sr.ReadToEnd();
            }
        }

        #endregion

        #region AdjustByteLength - バイト配列内容を規定の長さに併せて調整

        /// <summary>
        /// バイト配列の配列長を、指定された長さに調整します。
        /// </summary>
        /// <param name="source">調整対象のバイト配列</param>
        /// <param name="length">調整後の長さ</param>
        /// <returns>長さを調整されたバイト配列</returns>
        private static byte[] AdjustByteLength(byte[] source, int length)
        {
            var ret = new byte[length];
            if (source.Length <= length)    // 長さが足りない場合
            {
                // 元データの先頭に繰り返し数を追加し、元データでパディングする
                byte turn = byte.MinValue;
                for (int i = 0; i < ret.Length; ++i)
                {
                    for (int j = 0; j < source.Length && i < ret.Length; ++j, ++i)
                    {
                        ret[i] = source[j];
                    }

                    if (i < ret.Length)
                    {
                        if (turn > byte.MaxValue)
                        {
                            turn = byte.MinValue;
                        }
                        ret[i++] = turn;
                    }

                }
            }
            else    // 長さが超過している場合
            {
                // 超過している分のバイトで、先頭のバイトに対しビットXORを掛ける
                for (int i = 0; i < source.Length; ++i)
                {
                    for (int j = 0; j < ret.Length && i < source.Length; ++i, ++j)
                    {
                        ret[j] ^= source[i];
                    }
                }
            }

            return ret;
        }

        #endregion

        #region GetFileMD5 - ファイルのMD5ハッシュを取得

        /// <summary>
        /// 指定されたパスのファイルのMD5ハッシュを取得します。
        /// </summary>
        /// <param name="filePath">対象となるファイルのパス</param>
        /// <returns>ファイルのMD5ハッシュ</returns>
        public static string GetFileMD5(string filePath)
        {
            MD5 md5 = MD5.Create();
            var sb = new StringBuilder();
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                Array.ForEach(md5.ComputeHash(fs), b => sb.Append(b.ToString("x2")));
            }

            return sb.ToString();
        }

        /// <summary>
        /// 指定されたファイルのMD5ハッシュを取得します。
        /// </summary>
        /// <param name="file">対象となるファイル</param>
        /// <returns>ファイルのMD5ハッシュ</returns>
        public static string GetFileMD5(FileInfo file)
        {
            return GetFileMD5(file.FullName);
        }

        #endregion

        #region GetBytesMD5 - バイトデータのMD5ハッシュを取得

        /// <summary>
        /// 指定されたバイトデータのMD5ハッシュを取得します。
        /// </summary>
        /// <param name="data">対象のバイトデータ</param>
        /// <returns>データのMD5ハッシュ</returns>
        public static string GetBytesMD5(byte[] data)
        {
            MD5 md5 = MD5.Create();
            var sb = new StringBuilder();

            Array.ForEach(md5.ComputeHash(data), b => sb.Append(b.ToString("x2")));

            return sb.ToString();
        }

        #endregion
    }
}
