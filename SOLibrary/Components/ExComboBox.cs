using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SO.Library.Components
{
    /// <summary>
    /// ComboBox汎用拡張クラス
    /// </summary>
    public class ExComboBox : ComboBox
    {
        #region メンバ変数

        /// <summary>非読み取り専用時の背景色</summary>
        private Color _storeBackColor;

        /// <summary>非読み取り専用時の前景色</summary>
        private Color _storeForeColor;

        /// <summary>読み取り専用時の背景色</summary>
        private Color _readOnlyBackColor;

        /// <summary>読み取り専用時の前景色</summary>
        private Color _readOnlyForeColor;

        /// <summary>読み取り専用フラグ</summary>
        private bool _readOnly;

        #endregion

        #region ReadOnlyプロパティ - 読み取り専用設定
        /// <summary>
        /// コントロールを読み取り専用とするかどうかを取得または設定します。
        /// </summary>
        [Category("動作")]
        [Description("エディット コントロールの中の文字列を変更できるかどうかを設定します。")]
        [DefaultValue(false)]
        public bool ReadOnly
        {
            get { return _readOnly; }
            set
            {
                _readOnly = value;
                if (_readOnly)
                {
                    if (Enabled)
                    {
                        _storeBackColor = BackColor;
                        _storeForeColor = ForeColor;
                        BackColor = ReadOnlyBackColor;
                        ForeColor = ReadOnlyForeColor;
                    }
                    ContextMenu = new ContextMenu();
                    SetStyle(ControlStyles.Selectable, false);
                    SetStyle(ControlStyles.UserMouse, true);
                    UpdateStyles();
                    RecreateHandle();
                }
                else
                {
                    if (Enabled)
                    {
                        BackColor = _storeBackColor;
                        ForeColor = _storeForeColor;
                    }
                    ContextMenu = null;
                    SetStyle(ControlStyles.Selectable, true);
                    SetStyle(ControlStyles.UserMouse, false);
                    UpdateStyles();
                    RecreateHandle();
                }
            }
        }
        #endregion

        #region ReadOnlyBackColorプロパティ - 読み取り専用時背景色
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
                if (_readOnly && Enabled)
                    BackColor = _readOnlyBackColor;
            }
        }
        #endregion

        #region ReadOnlyForeColorプロパティ - 読み取り専用時の前景色
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
                if (_readOnly && Enabled)
                    ForeColor = _readOnlyForeColor;
            }
        }
        #endregion

        #region コンストラクタ
        /// <summary>
        /// デフォルトのコンストラクタです。
        /// </summary>
        public ExComboBox()
        {
            _storeBackColor = BackColor;
            _storeForeColor = ForeColor;
        }
        #endregion

        #region OnKeyDown - キーダウン時
        /// <summary>
        /// コントロール上でキーダウンされた際に実行される処理です。
        /// 読み取り専用時にキー入力を無効化します。
        /// </summary>
        /// <param name="e">イベントオブジェクト</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!_readOnly)
            {
                base.OnKeyDown(e);
                return;
            }

            switch (e.KeyCode)
            {
                case Keys.Delete:
                case Keys.Up:
                case Keys.Down:
                case Keys.PageUp:
                case Keys.PageDown:
                case Keys.F4:
                    e.Handled = true;
                    break;

                default:
                    base.OnKeyDown(e);
                    break;
            }
        }
        #endregion

        #region OnKeyPress - キープレス時
        /// <summary>
        /// コントロール上でキープレスされた際に実行される処理です。
        /// 読み取り専用時にキー入力を無効化します。
        /// </summary>
        /// <param name="e">イベントオブジェクト</param>
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (!_readOnly)
            {
                base.OnKeyPress(e);
                return;
            }

            if (!char.IsControl(e.KeyChar)) e.Handled = true;

            base.OnKeyPress(e);
        }
        #endregion
    }
}
