using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SO.Library.IO
{
    /// <summary>
    /// ファイル操作抽象基底クラス
    /// </summary>
    /// <typeparam name="T">項目情報クラス</typeparam>
    /// <seealso cref="System.IDisposable"/>
    public abstract class FileHelper<T> : IDisposable where T : FileItem
    {
        #region インスタンス変数

        /// <summary>現在の行位置</summary>
        protected int _currentRow = -1;

        /// <summary>項目定義リスト</summary>
        protected List<T> _items;

        #endregion

        #region プロパティ

        /// <summary>
        /// 読込ファイルのパスを取得または設定します。
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 改行コードを取得または設定します。
        /// 規定値はOS標準の改行コードです。
        /// </summary>
        public string LineCode { get; set; }

        /// <summary>
        /// 文字コードを取得または設定します。
        /// 規定値はShift_JISです。
        /// </summary>
        public Encoding FileEncoding { get; set; }

        /// <summary>
        /// レコードフェッチ状態を取得します。
        /// </summary>
        public FileFetchStatus FetchStatus { get; protected set; }

        /// <summary>
        /// 読込ファイルが存在するかどうかを取得します。
        /// </summary>
        public bool Exists
        {
            get { return !string.IsNullOrEmpty(FilePath) && File.Exists(FilePath); }
        }

        /// <summary>
        /// 現在の行位置を取得します。
        /// 初期位置は-1です。
        /// 読込ファイルが存在しない場合は-1が返されます。
        /// </summary>
        public int CurrentRow
        {
            get
            {
                if (!Exists)
                {
                    return -1;
                }

                return _currentRow;
            }
            protected set
            {
                _currentRow = value;
            }
        }

        /// <summary>
        /// 読込ファイルのサイズを取得します。
        /// 読込ファイルが存在しない場合は-1が返されます。
        /// </summary>
        public long FileSize
        {
            get
            {
                if (!Exists)
                {
                    return -1;
                }

                return new FileInfo(FilePath).Length;
            }
        }

        /// <summary>
        /// 項目リストを取得または設定します。
        /// </summary>
        public virtual List<T> Items
        {
            get { return _items; }
            set { _items = value; }
        }

        #endregion
        
        #region コンストラクタ

        /// <summary>
        /// インスタンスを作成します。
        /// </summary>
        /// <param name="filePath">読み込むファイルのパス</param>
        public FileHelper(string filePath)
        {
            FilePath = filePath;
            LineCode = Environment.NewLine;
            FileEncoding = Encoding.GetEncoding(932);
            FetchStatus = FileFetchStatus.BOF;
        }

        /// <summary>
        /// 項目定義を指定してインスタンスを作成します。
        /// </summary>
        /// <param name="filePath">読み込むファイルのパス</param>
        /// <param name="items">項目定義リスト</param>
        public FileHelper(string filePath, List<T> items)
        {
            FilePath = filePath;
            Items = items;
            LineCode = Environment.NewLine;
            FileEncoding = Encoding.GetEncoding(932);
            FetchStatus = FileFetchStatus.BOF;
        }

        #endregion

        #region Dispose - リソース破棄

        /// <summary>
        /// Close()を呼び出し、使用しているリソースを全て破棄します。
        /// </summary>
        public virtual void Dispose()
        {
            Close();
        }

        #endregion

        #region 抽象メソッド

        /// <summary>
        /// 対象ファイルの読込ストリームを開きます。
        /// </summary>
        public abstract void Open();

        /// <summary>
        /// 対象ファイルの読込ストリームを閉じます。
        /// </summary>
        public abstract void Close();

        /// <summary>
        /// 現在の行位置の次の行の内容を読み込み、その値をItemsにセットします。
        /// </summary>
        /// <returns>レコードフェッチ状態</returns>
        public abstract FileFetchStatus Next();

        #endregion
    }

    #region enum FileItemType - ファイル項目データ型列挙体

    /// <summary>
    /// ファイル項目データ型列挙体
    /// </summary>
    public enum FileItemType
    {
        /// <summary>未定義</summary>
        Undefined,
        /// <summary>文字</summary>
        Text,
        /// <summary>整数</summary>
        Integer,
        /// <summary>小数</summary>
        Decimal,
        /// <summary>日付</summary>
        Date,
        /// <summary>時刻</summary>
        Time,
        /// <summary>日時</summary>
        DateTime,
    }

    #endregion

    #region enum FileFetchStatus - 固定長ファイルレコードフェッチ状態列挙体

    /// <summary>
    /// 固定長ファイルレコードフェッチ状態列挙体
    /// </summary>
    public enum FileFetchStatus
    {
        /// <summary>最初のレコードの前に位置している状態</summary>
        BOF,
        /// <summary>全ての項目を不足無く読み込んだ状態</summary>
        Normal,
        /// <summary>読み込みを行ったが一部の項目で指定桁に満たない状態</summary>
        NotFilled,
        /// <summary>最後のレコードの後に位置している状態</summary>
        EOF,
    }

    #endregion
}
