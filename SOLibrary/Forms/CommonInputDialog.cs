using System;
using System.Windows.Forms;

namespace SO.Library.Forms
{
    /// <summary>
    /// 共通ユーザ入力ダイアログクラス
    /// </summary>
    /// <seealso cref="System.Windows.Forms.Form"/>
    public partial class CommonInputDialog : Form
    {
        #region インスタンス変数

        /// <summary>入力チェック実施フラグ</summary>
        private bool _inputChkFlg;

        #endregion

        #region プロパティ

        /// <summary>
        /// ユーザが入力した文字列を取得します。
        /// </summary>
        public string InputString
        {
            get { return txtInput.Text; }
        }

        /// <summary>
        /// 入力チェックフラグを取得・設定します。
        /// </summary>
        public bool IsInputCheck
        {
            get { return _inputChkFlg; }
            set { _inputChkFlg = value; }
        }

        #endregion

        #region コンストラクタ
        /// <summary>
        /// ダイアログタイトル、説明文言付きのコンストラクタです。
        /// </summary>
        /// <param name="title">ダイアログタイトル</param>
        /// <param name="labelText">説明文言</param>
        public CommonInputDialog(string title, string labelText)
        {
            // コンポーネント初期化
            InitializeComponent();

            // フォームタイトル、ラベル表示初期化
            this.Text = title;
            lblInput.Text = labelText;
        }

        /// <summary>
        /// ダイアログタイトル、説明文言、入力チェックフラグ付きのコンストラクタです。
        /// </summary>
        /// <param name="title">ダイアログタイトル</param>
        /// <param name="labelText">説明文言</param>
        /// <param name="inputCheckFlag">入力チェックフラグ</param>
        public CommonInputDialog(string title, string labelText, bool inputCheckFlag)
        {
            // コンポーネント初期化
            InitializeComponent();

            // フォームタイトル、ラベル表示初期化
            this.Text = title;
            lblInput.Text = labelText;

            // 入力チェックフラグ初期化
            _inputChkFlg = inputCheckFlag;
        }

        /// <summary>
        /// ダイアログタイトル、説明文言、初期値付きのコンストラクタです。
        /// </summary>
        /// <param name="title">ダイアログタイトル</param>
        /// <param name="labelText">説明文言</param>
        /// <param name="initialValue">初期値</param>
        public CommonInputDialog(string title, string labelText, string initialValue)
        {
            // コンポーネント初期化
            InitializeComponent();

            // フォームタイトル、ラベル表示初期化
            this.Text = title;
            lblInput.Text = labelText;

            // 初期値設定
            txtInput.Text = initialValue;
            txtInput.SelectAll();
        }

        /// <summary>
        /// ダイアログタイトル、説明文言、入力チェックフラグ、初期値付きのコンストラクタです。
        /// </summary>
        /// <param name="title">ダイアログタイトル</param>
        /// <param name="labelText">説明文言</param>
        /// <param name="inputCheckFlag">入力チェックフラグ</param>
        /// <param name="initialValue">初期値</param>
        public CommonInputDialog(string title, string labelText, bool inputCheckFlag, string initialValue)
        {
            // コンポーネント初期化
            InitializeComponent();

            // フォームタイトル、ラベル表示初期化
            this.Text = title;
            lblInput.Text = labelText;

            // 入力チェックフラグ初期化
            _inputChkFlg = inputCheckFlag;

            // 初期値設定
            txtInput.Text = initialValue;
            txtInput.SelectAll();
        }
        #endregion

        #region IsValidInput - 入力チェック
        /// <summary>
        /// 入力チェックを実施します。
        /// ユーザからの何らかの入力が有る場合のみチェックOKとなります。
        /// </summary>
        /// <returns>チェックOK時:true、チェックNG時:false</returns>
        protected virtual bool IsValidInput()
        {
            // 入力チェック実施フラグがOFFの場合は常にOK
            if (!_inputChkFlg) return true;

            // 入力チェック実施フラグがONでかつ未入力の場合はNG
            if (string.IsNullOrEmpty(txtInput.Text))
            {
                txtInput.Focus();
                FormUtilities.ShowErrorMessage("未入力です。");
                return false;
            }
            return true;
        }
        #endregion

        #region Show - モーダルダイアログとして表示
        /// <summary>
        /// (Form.Show()を隠蔽します)
        /// ダイアログをモーダル状態で表示します。
        /// </summary>
        /// <returns>ダイアログ処理結果</returns>
        public new virtual DialogResult Show()
        {
            // 必ずモーダルダイアログとして表示
            return ShowDialog();
        }

        /// <summary>
        /// (Form.Show(IWin32Window)を隠蔽します)
        /// オーナーウィンドウを指定し、ダイアログをモーダル状態で表示します。
        /// </summary>
        /// <param name="owner">オーナーウィンドウ</param>
        /// <returns>ダイアログ処理結果</returns>
        public new virtual DialogResult Show(IWin32Window owner)
        {
            // 必ずモーダルダイアログとして表示
            return ShowDialog(owner);
        }
        #endregion

        #region btnOk_Click - OKボタン押下時
        /// <summary>
        /// OKボタン押下時の処理です。
        /// 入力チェックを実施し、OKなら呼出元へ制御を返します。
        /// </summary>
        /// <param name="sender">イベント発生元オブジェクト</param>
        /// <param name="e">イベント引数</param>
        protected virtual void btnOk_Click(object sender, EventArgs e)
        {
            // 入力チェック、相関チェックOKなら呼出元へOKを返す
            if (IsValidInput()) this.DialogResult = DialogResult.OK;
        }
        #endregion
    }
}
