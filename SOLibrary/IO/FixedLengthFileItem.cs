using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using SO.Library.Text;

namespace SO.Library.IO
{
    /// <summary>
    /// 固定長レコードファイル項目定義クラス
    /// </summary>
    public sealed class FixedLengthFileItem : FileItem
    {
        #region インスタンス変数

        /// <summary>項目長(バイト)</summary>
        private int _definedLength = 0;

        #endregion

        #region プロパティ

        /// <summary>
        /// 定義上の項目長(バイト)を取得・設定します。
        /// 1未満の値は指定出来ません。
        /// 未設定の場合は0が返されます。
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">1未満の値が指定された場合</exception>
        public int DefinedLength
        {
            get { return _definedLength; }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException(
                        "Lengthプロパティに1未満の値は指定出来ません。");

                _definedLength = value;
            }
        }

        #endregion

        #region コンストラクタ
        /// <summary>
        /// 規定の初期値でインスタンスを生成します。
        /// </summary>
        public FixedLengthFileItem() : base() { }

        /// <summary>
        /// 項目名、項目タイプ、項目長(バイト)を指定してインスタンスを生成します。
        /// </summary>
        /// <param name="name">項目名</param>
        /// <param name="type">項目タイプ</param>
        /// <param name="length">項目長(バイト)</param>
        public FixedLengthFileItem(string name, FileItemType type, int length)
            : base(name, type)
        {
            DefinedLength = length;
        }

        /// <summary>
        /// 項目名、項目タイプ、項目長(バイト)、項目値を指定してインスタンスを生成します。
        /// </summary>
        /// <param name="name">項目名</param>
        /// <param name="type">項目タイプ</param>
        /// <param name="length">項目長(バイト)</param>
        /// <param name="value">項目値</param>
        public FixedLengthFileItem(string name, FileItemType type, int length, string value)
            : base(name, type, value)
        {
            DefinedLength = length;
        }
        #endregion

        #region ValidateLength - 値長チェック
        /// <summary>
        /// Valueプロパティの長さ(バイト)が、Lengthプロパティで指定された長さと等しいかをチェックします。
        /// Valueプロパティの値がnullの場合、Lengthプロパティが未設定の場合はfalseが返されます。
        /// </summary>
        /// <returns>true:等しい / false:等しくない</returns>
        public bool ValidateLength()
        {
            return (Value == null) ? false : (Length == DefinedLength);
        }
        #endregion
    }
}
