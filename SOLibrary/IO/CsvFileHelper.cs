using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace SO.Library.IO
{
    /// <summary>
    /// CSVファイル操作クラス
    /// </summary>
    public sealed class CsvFileHelper : FileHelper<FileItem>
    {
        #region クラス定数

        /// <summary>CSVファイルの項目セパレータ</summary>
        private const string CSV_SEPALATOR = ",";

        /// <summary>TSVファイルの項目セパレータ</summary>
        private const string TSV_SEPALATOR = "\t";

        #endregion

        #region インスタンス変数

        /// <summary>項目セパレータ</summary>
        private string _sepalator;

        /// <summary>CSV/TSVファイル種別</summary>
        private CsvTsvFileKind _fileKind;

        /// <summary>項目値をダブルクォーテーションで囲むか</summary>
        private bool _useQuote = false;

        /// <summary>ファイル内容読み込みストリーム</summary>
        private StreamReader _reader;

        #endregion

        #region プロパティ

        /// <summary>
        /// 項目セパレータを取得します。
        /// </summary>
        public string Sepalator
        {
            get { return _sepalator; }
            private set { _sepalator = value; }
        }

        /// <summary>
        /// CSV/TSVファイル種別を取得します。
        /// </summary>
        public CsvTsvFileKind FileKind
        {
            get { return _fileKind; }
            private set
            {
                _fileKind = value;

                if (value == CsvTsvFileKind.CSV)
                {
                    Sepalator = CSV_SEPALATOR;
                }
                else    // TSV
                {
                    Sepalator = TSV_SEPALATOR;
                    UseDoubleQuote = false;
                }
            }
        }

        /// <summary>
        /// 項目値をダブルクォーテーションで囲むかを取得または設定します。
        /// FileKind が TSV の場合は、常にfalseになります。
        /// 既定値はfalseです。
        /// </summary>
        public bool UseDoubleQuote
        {
            get { return _useQuote; }
            set { _useQuote = FileKind == CsvTsvFileKind.TSV ? false : value; }
        }

        #endregion

        #region コンストラクタ

        /// <summary>
        /// インスタンスを作成します。
        /// </summary>
        /// <param name="filePath">読み込むファイルのパス</param>
        /// <param name="kind">CSV/TSVファイル種別</param>
        public CsvFileHelper(string filePath, CsvTsvFileKind kind)
            : base(filePath)
        {
            FileKind = kind;
        }

        /// <summary>
        /// インスタンスを作成します。
        /// </summary>
        /// <param name="filePath">読み込むファイルのパス</param>
        /// <param name="items">項目定義リスト</param>
        /// <param name="kind">CSV/TSVファイル種別</param>
        public CsvFileHelper(string filePath, List<FileItem> items, CsvTsvFileKind kind)
            : base(filePath, items)
        {
            FileKind = kind;
        }

        #endregion

        #region Open - 読込ストリームオープン

        /// <summary>
        /// 対象ファイルの読込ストリームを開きます。
        /// </summary>
        public override void Open()
        {
            if (_reader != null)
            {
                return;
            }

            if (!Exists)
            {
                throw new FileNotFoundException("読み込みファイルが見つかりません。");
            }

            _reader = new StreamReader(FilePath, FileEncoding);
        }

        #endregion

        #region Close - 読込ストリームクローズ

        /// <summary>
        /// 対象ファイルの読込ストリームを閉じます。
        /// </summary>
        public override void Close()
        {
            if (_reader != null)
            {
                _reader.Dispose();
                _reader = null;
            }
        }

        #endregion

        #region Next - 次の行を読込

        /// <summary>
        /// 現在の行位置の次の行の内容を読み込み、その値をItemsにセットします。
        /// </summary>
        /// <returns>レコードフェッチ状態</returns>
        public override FileFetchStatus Next()
        {
            if (FetchStatus == FileFetchStatus.EOF) return FileFetchStatus.EOF;

            string bfr = _reader.ReadLine();
            Items.Clear();
            ++CurrentRow;

            if (bfr == null)
            {
                return FetchStatus = FileFetchStatus.EOF;
            }

            if (UseDoubleQuote)
            {
                SplitItemsWithQuote(bfr);
            }
            else
            {
                SplitItems(bfr);
            }

            return FetchStatus = FileFetchStatus.Normal;
        }

        #endregion

        private void SplitItems(string rec)
        {
            foreach (var val in Regex.Split(rec, Sepalator))
            {
                var item = new FileItem();
                item.Value = val;

                Items.Add(item);
            }
        }

        private void SplitItemsWithQuote(string rec)
        {
            Regex.Match(rec, "\".*\"");
        }
    }

    #region enum CSV/TSVファイル種別列挙体

    /// <summary>
    /// CSV/TSVファイル種別列挙体
    /// </summary>
    public enum CsvTsvFileKind
    {
        /// <summary>CSVファイル</summary>
        CSV,
        /// <summary>TSVファイル</summary>
        TSV,
    }

    #endregion
}
