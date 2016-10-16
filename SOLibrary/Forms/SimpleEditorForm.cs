using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace SO.Library.Forms
{
    /// <summary>
    /// 簡易エディタフォームクラス
    /// </summary>
    public partial class SimpleEditorForm : Form
    {
        #region クラス定数

        /// <summary>タイトルバーに表示される編集中マーク</summary>
        private const string EDITED_MARK = " *";

        #endregion

        #region プロパティ

        /// <summary>
        /// 編集対象ファイルパスを取得します。
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// 編集対象ファイル名を取得します。
        /// </summary>
        private string FileName
        {
            get { return Path.GetFileName(FilePath); }
        }

        /// <summary>
        /// 編集対象ファイルの文字コードを取得または設定します。
        /// </summary>
        public Encoding FileEncoding { get; set; }

        /// <summary>
        /// 上書き保存時に確認を行うかどうかを取得または設定します。
        /// 規定値はfalseです。
        /// </summary>
        public bool SaveConfirm { get; set; }

        /// <summary>
        /// ファイルが編集中かどうかを取得します。
        /// </summary>
        public bool Updated { get; private set; }

        #endregion

        #region コンストラクタ

        /// <summary>
        /// 規定のコンストラクタです。
        /// </summary>
        /// <param name="path">編集対象ファイルパス</param>
        public SimpleEditorForm(string path)
        {
            InitializeComponent();

            FilePath = path;
            FileEncoding = Encoding.GetEncoding(932);
            SaveConfirm = false;
            Updated = false;
        }

        #endregion

        #region イベントハンドラ

        #region SimpleEditorForm_Shown - フォーム表示時処理

        /// <summary>
        /// フォーム表示時の処理です。
        /// 編集対象ファイルの内容をテキストエリアに表示します。
        /// </summary>
        /// <param name="sender">イベント発生元オブジェクト</param>
        /// <param name="e">イベント引数</param>
        private void SimpleEditorForm_Shown(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(FilePath) || !File.Exists(FilePath))
                throw new FileNotFoundException("開くファイルのパスが不正です。");

            using (var reader = new StreamReader(FilePath, FileEncoding))
            {
                txtEditor.Text = reader.ReadToEnd();
            }

            txtEditor.Select(txtEditor.Text.Length, 0);
            Text = FileName;
        }

        #endregion

        #region SimpleEditorForm_KeyDown - フォーム上でのキー押下時処理

        /// <summary>
        /// フォーム上でのキー押下時の処理です。
        /// 押下されたショートカットキーに応じた処理を行います。
        /// </summary>
        /// <remarks>
        /// Ctrl+Sキー押下時、テキストエリアの内容を編集対象ファイルに上書き保存します。
        /// Ctrl+Aキー押下時、テキストエリアの内容を全選択します。
        /// </remarks>
        /// <param name="sender">イベント発生元オブジェクト</param>
        /// <param name="e">イベント引数</param>
        private void SimpleEditorForm_KeyDown(object sender, KeyEventArgs e)
        {
            bool pressAlt = (e.Modifiers & Keys.Alt) == Keys.Alt;
            bool pressShift = (e.Modifiers & Keys.Shift) == Keys.Shift;
            bool pressCtrl = (e.Modifiers & Keys.Control) == Keys.Control;
            if (!pressAlt && !pressShift && pressCtrl)
            {
                // 修飾キーの内、Ctrlキーのみが押下されている
                e.Handled = true;
                switch (e.KeyCode)
                {
                    case Keys.S:    // 上書き保存
                        btnSave_Click(sender, e);
                        break;

                    case Keys.A:    // テキスト全選択
                        txtEditor.SelectAll();
                        break;

                    default:
                        // 上記以外は未ハンドルに戻す
                        e.Handled = false;
                        break;
                }
            }
        }

        #endregion

        #region SimpleEditorForm_FormClosing - フォームクローズ前処理

        /// <summary>
        /// フォームクローズ前の処理です。
        /// ファイルが編集中の場合、フォームを閉じて良いか確認します。
        /// </summary>
        /// <param name="sender">イベント発生元オブジェクト</param>
        /// <param name="e">イベント引数</param>
        private void SimpleEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Updated
                && FormUtilities.ShowQuestionMessage("保存せずに閉じてよろしいですか？") == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        #endregion

        #region btnSave_Click - 上書き保存ボタン押下時処理

        /// <summary>
        /// 上書き保存ボタン押下時の処理です。
        /// テキストエリアの内容を編集対象ファイルに上書き保存します。
        /// </summary>
        /// <param name="sender">イベント発生元オブジェクト</param>
        /// <param name="e">イベント引数</param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (SaveConfirm
                && FormUtilities.ShowQuestionMessage("上書き保存してよろしいですか？") == DialogResult.No)
            {
                return;
            }

            using (var writer = new StreamWriter(FilePath, false, FileEncoding))
            {
                writer.Write(txtEditor.Text);
            }

            Text = FileName;
            Updated = false;

            FormUtilities.ShowInformationMessage("保存しました。");
        }

        #endregion

        #region btnClose_Click - 閉じるボタン押下時処理

        /// <summary>
        /// 閉じるボタン押下時の処理です。
        /// フォームを閉じます。
        /// </summary>
        /// <param name="sender">イベント発生元オブジェクト</param>
        /// <param name="e">イベント引数</param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion

        #region txtEditor_TextChanged - テキストエリア内容変更時処理

        /// <summary>
        /// テキストエリアの内容変更時の処理です。
        /// タイトルバーに編集中マークを表示し、編集中フラグをONに設定します。
        /// </summary>
        /// <param name="sender">イベント発生元オブジェクト</param>
        /// <param name="e">イベント引数</param>
        private void txtEditor_TextChanged(object sender, EventArgs e)
        {
            Text = FileName + EDITED_MARK;
            Updated = true;
        }

        #endregion

        #endregion
    }
}
