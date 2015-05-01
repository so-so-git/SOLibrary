using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SO.Library.Components
{
    /// <summary>
    /// TextBox汎用拡張クラス
    /// </summary>
    public class ExTextBox : TextBox
    {
        #region インスタンス変数

        /// <summary>非読み取り専用時の背景色</summary>
        private Color _storeBackColor;

        /// <summary>非読み取り専用時の前景色</summary>
        private Color _storeForeColor;

        /// <summary>読み取り専用時の背景色</summary>
        private Color _readOnlyBackColor;

        /// <summary>読み取り専用時の前景色</summary>
        private Color _readOnlyForeColor;

        #endregion

        #region プロパティ

        /// <summary>
        /// コントロールを読み取り専用とするかどうかを取得または設定します。
        /// </summary>
        [Category("動作")]
        [Description("エディット コントロールの中の文字列を変更できるかどうかを設定します。")]
        [DefaultValue(false)]
        public new bool ReadOnly
        {
            get { return base.ReadOnly; }
            set
            {
                if (value)
                {
                    _storeBackColor = BackColor;
                    _storeForeColor = ForeColor;
                    base.ReadOnly = value;
                    BackColor = ReadOnlyBackColor;
                    ForeColor = ReadOnlyForeColor;
                }
                else
                {
                    base.ReadOnly = value;
                    BackColor = _storeBackColor;
                    ForeColor = _storeForeColor;
                }
            }
        }

        /// <summary>
        /// 読み取り専用時の背景色を取得または設定します。
        /// </summary>
        [Category("表示")]
        [Description("読み取り専用時のコンポーネントの背景色です。")]
        [DefaultValue(typeof(Color), "Control")]
        public Color ReadOnlyBackColor
        {
            get { return _readOnlyBackColor; }
            set
            {
                _readOnlyBackColor = value;
                if (ReadOnly)
                {
                    BackColor = _readOnlyBackColor;
                }
            }
        }

        /// <summary>
        /// 読み取り専用時の前景色を取得または設定します。
        /// </summary>
        [Category("表示")]
        [Description("読み取り専用時のテキストの表示に使用される、このコンポーネントの前景色です。")]
        [DefaultValue(typeof(Color), "WindowText")]
        public Color ReadOnlyForeColor
        {
            get { return _readOnlyForeColor; }
            set
            {
                _readOnlyForeColor = value;
                if (ReadOnly)
                {
                    ForeColor = _readOnlyForeColor;
                }
            }
        }

        #endregion

        #region コンストラクタ

        /// <summary>
        /// デフォルトのコンストラクタです。
        /// </summary>
        public ExTextBox()
        {
            _storeBackColor = BackColor;
            _storeForeColor = ForeColor;
        }

        #endregion
    }
}
