using System;
using System.Globalization;

using SO.Library.Text;

namespace SO.Library.IO
{
    /// <summary>
    /// ファイル項目定義クラス
    /// </summary>
    public class FileItem
    {
        #region プロパティ

        /// <summary>項目名を取得または設定します。</summary>
        public string Name { get; set; }

        /// <summary>項目値を取得または設定します。</summary>
        public string Value { get; set; }

        /// <summary>
        /// 項目タイプを取得または設定します。
        /// 未設定の場合はUndefinedが返されます。
        /// </summary>
        public FileItemType ItemType { get; set; }

        /// <summary>
        /// Valueプロパティの値の長さ(バイト)を取得します。
        /// Valueプロパティがnullの場合は-1が返されます。
        /// </summary>
        public int Length
        {
            get
            {
                return Value == null ? -1 : StringUtilities.GetByteCount(Value);
            }
        }

        #endregion

        #region コンストラクタ

        /// <summary>
        /// 規定の初期値でインスタンスを生成します。
        /// </summary>
        public FileItem()
        {
            ItemType = FileItemType.Undefined;
        }

        /// <summary>
        /// 項目名、項目タイプ、項目長(バイト)を指定してインスタンスを生成します。
        /// </summary>
        /// <param name="name">項目名</param>
        /// <param name="type">項目タイプ</param>
        public FileItem(string name, FileItemType type)
        {
            Name = name;
            ItemType = type;
        }

        /// <summary>
        /// 項目名、項目タイプ、項目長(バイト)、項目値を指定してインスタンスを生成します。
        /// </summary>
        /// <param name="name">項目名</param>
        /// <param name="type">項目タイプ</param>
        /// <param name="value">項目値</param>
        public FileItem(string name, FileItemType type, string value)
        {
            Name = name;
            ItemType = type;
            Value = value;
        }

        #endregion

        #region ValidateType - 型チェック

        /// <summary>
        /// Valueプロパティの値が、Typeプロパティで指定された型として妥当かをチェックします。
        /// Valueプロパティの値がnullの場合、Typeプロパティの値がUndefinedの場合はfalseが返されます。
        /// </summary>
        /// <returns>true:妥当な値 / false:不正な値</returns>
        public bool ValidateType()
        {
            if (Value == null)
            {
                return false;
            }

            switch (ItemType)
            {
                case FileItemType.Text:
                    return true;

                case FileItemType.Integer:
                    long lng;
                    return long.TryParse(Value, out lng);

                case FileItemType.Decimal:
                    double dbl;
                    return double.TryParse(Value, out dbl);

                case FileItemType.Date:
                    DateTime dt;
                    return DateTime.TryParseExact(Value, "yyyy/MM/dd", null, DateTimeStyles.None, out dt);

                case FileItemType.Time:
                    DateTime tm;
                    return DateTime.TryParseExact(Value, "HH:mm:ss", null, DateTimeStyles.None, out tm);

                case FileItemType.DateTime:
                    DateTime dttm;
                    return DateTime.TryParseExact(Value, "yyyy/MM/dd HH:mm:ss", null, DateTimeStyles.None, out dttm);

                default:    // Undefined
                    return false;
            }
        }

        #endregion
    }
}
