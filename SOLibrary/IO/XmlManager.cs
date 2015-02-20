using System;
using System.IO;
using System.Xml.Serialization;

namespace SO.Library.IO
{
    /// <summary>
    /// XMLファイル管理機能提供クラス
    /// </summary>
    public static class XmlManager
    {
        #region Serialize - オブジェクトシリアライズ
        /// <summary>
        /// 指定されたオブジェクトをXMLファイルにシリアライズします。
        /// </summary>
        /// <typeparam name="T">引数無しコンストラクタを持つクラス</typeparam>
        /// <param name="path">XMLファイル出力先パス</param>
        /// <param name="obj">シリアライズするオブジェクト</param>
        public static void Serialize<T>(string path, T obj) where T : class, new()
        {
            using (var fs = new FileStream(path, FileMode.Create))
            {
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(fs, obj);
            }
        }
        #endregion

        #region Deserialize - オブジェクトデシリアライズ
        /// <summary>
        /// 指定されたXMLファイルをデシリアライズします。
        /// </summary>
        /// <typeparam name="T">引数無しコンストラクタを持つクラス</typeparam>
        /// <param name="path">デシリアライズするXMLファイルのパス</param>
        /// <returns>デシリアライズされたオブジェクト</returns>
        public static T Deserialize<T>(string path) where T : class, new()
        {
            return Deserialize<T>(path, false);
        }

        /// <summary>
        /// 指定されたXMLファイルをデシリアライズします。
        /// createにtrueが指定された場合、指定されたパスにXMLがファイルが無ければ、
        /// 新規インスタンスを作成して返します。
        /// また、空のオブジェクトをシリアライズしたXMLファイルを配置します。
        /// </summary>
        /// <typeparam name="T">引数無しコンストラクタを持つクラス</typeparam>
        /// <param name="path">デシリアライズするXMLファイルのパス</param>
        /// <param name="create">XMLファイルが無い場合に新規作成するかのフラグ</param>
        /// <returns>デシリアライズされたオブジェクト</returns>
        /// <exception cref="FileNotFoundException">createがfalseでかつXMLファイルが無かった場合にスローされます。</exception>
        public static T Deserialize<T>(string path, bool create) where T : class, new()
        {
            if (!File.Exists(path))
            {
                if (create)
                {
                    var obj = new T();
                    Serialize(path, obj);
                    return obj;
                }
                else
                {
                    throw new FileNotFoundException();
                }
            }

            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(fs);
            }
        }
        #endregion
    }
}
