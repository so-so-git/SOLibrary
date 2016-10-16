using System.Collections.Generic;
using System.IO;

namespace SO.Library.IO
{
    /// <summary>
    /// 固定長レコードファイル操作クラス
    /// </summary>
    public sealed class FixedLengthFileHelper : FileHelper<FixedLengthFileItem>
    {
        #region インスタンス変数

        /// <summary>ファイル内容読み込みストリーム</summary>
        private FileStream _reader;

        #endregion

        #region プロパティ

        /// <summary>
        /// レコード長(バイト)を取得または設定します。
        /// </summary>
        public int RecordLength { get; set; }

        /// <summary>
        /// 現在の位置以降の残りサイズ(バイト)を取得します。
        /// 読込ファイルが存在しない場合は-1が返されます。
        /// </summary>
        public long RemainSize
        {
            get
            {
                if (!Exists)
                {
                    return -1;
                }

                switch (FetchStatus)
                {
                    case FileFetchStatus.BOF:
                        return FileSize;

                    case FileFetchStatus.Normal:
                        return FileSize - RecordLength * (CurrentRow + 1);
                    
                    default:    // NotFilled or EOF
                        return 0;
                }
            }
        }

        /// <summary>
        /// ファイルが正しいサイズか(RecordLengthの倍数であるか)を取得します。
        /// 読込ファイルが存在しない場合はfalseが返されます。
        /// </summary>
        public bool IsValidSize
        {
            get
            {
                if (!Exists)
                {
                    return false;
                }

                return FileSize % RecordLength == 0;
            }
        }

        /// <summary>
        /// 項目リストを取得または設定します。
        /// 同時に、全項目の項目長定義からレコード長を自動算出します。
        /// </summary>
        public override List<FixedLengthFileItem> Items
        {
            get { return _items; }
            set
            {
                _items = value;

                int len = 0;
                foreach (var item in value)
                {
                    len += item.DefinedLength;
                }

                RecordLength = len;
            }
        }

        /// <summary>
        /// 指定されたインデックスの項目を取得します。
        /// </summary>
        /// <param name="index">取得する項目のインデックス</param>
        /// <returns>指定されたインデックスの項目</returns>
        public FixedLengthFileItem this[int index]
        {
            get { return _items[index]; }
        }

        #endregion

        #region コンストラクタ

        /// <summary>
        /// レコード長を指定してインスタンスを作成します。
        /// </summary>
        /// <param name="filePath">読み込むファイルのパス</param>
        /// <param name="recLen">レコード長</param>
        public FixedLengthFileHelper(string filePath, int recLen)
            : base(filePath)
        {
            RecordLength = recLen;
        }

        /// <summary>
        /// 項目定義を指定してインスタンスを作成します。
        /// </summary>
        /// <param name="filePath">読み込むファイルのパス</param>
        /// <param name="items">項目定義リスト</param>
        public FixedLengthFileHelper(string filePath, List<FixedLengthFileItem> items)
            : base(filePath, items) { }

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

            _reader = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
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
            if (FetchStatus == FileFetchStatus.EOF)
            {
                return FileFetchStatus.EOF;
            }

            var bfr = new byte[RecordLength];
            int remain = _reader.Read(bfr, 0, RecordLength);
            ++CurrentRow;

            if (remain == 0)
            {
                foreach (var item in Items)
                {
                    item.Value = null;
                }

                return FetchStatus = FileFetchStatus.EOF;
            }

            int offset = 0;
            for (int i = 0; i < Items.Count; ++i)
            {
                if (Items[i].DefinedLength > remain)
                {
                    Items[i].Value = FileEncoding.GetString(bfr, offset, remain);

                    for (int j = i + 1; j < Items.Count; ++j)
                    {
                        Items[j].Value = null;
                    }

                    return FetchStatus = FileFetchStatus.NotFilled;
                }

                Items[i].Value = FileEncoding.GetString(bfr, offset, Items[i].DefinedLength);
                offset += Items[i].DefinedLength;
                remain -= Items[i].DefinedLength;
            }

            return FetchStatus = FileFetchStatus.Normal;
        }

        #endregion

        #region SplitByLineCode - レコード長単位で改行コードで分割

        /// <summary>
        /// レコード長毎に改行コードを挿入し、新しいファイルを作成します。
        /// </summary>
        /// <param name="savePath">保存先ファイルパス</param>
        public void SplitByLineCode(string savePath)
        {
            using (var fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
            using (var sw = new StreamWriter(savePath, false, FileEncoding))
            {
                var bfr = new byte[RecordLength];
                int read;
                while ((read = fs.Read(bfr, 0, RecordLength)) > 0)
                {
                    sw.Write(FileEncoding.GetString(bfr));
                    sw.Write(LineCode);
                }
            }
        }

        #endregion

        #region RemoveLineCode - 改行コードを除去

        /// <summary>
        /// 改行コードを除去し、新しいファイルを作成します。
        /// </summary>
        /// <param name="savePath">保存先ファイルパス</param>
        public void RemoveLineCode(string savePath)
        {
            using (var sr = new StreamReader(FilePath, FileEncoding))
            using (var sw = new StreamWriter(savePath, false, FileEncoding))
            {
                string bfr;
                while ((bfr = sr.ReadLine()) != null)
                {
                    sw.WriteLine(bfr);
                }
            }
        }

        #endregion
    }
}
