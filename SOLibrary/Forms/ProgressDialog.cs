using System;
using System.Drawing;
using System.Windows.Forms;

namespace SO.Library.Forms
{
    /// <summary>
    /// プログレス表示フォームクラス
    /// </summary>
    /// <seealso cref="System.Windows.Forms.Form"/>
    public partial class ProgressDialog : Form
    {
        #region プロパティ

        #region Title - フォームタイトル取得または設定
        /// <summary>
        /// フォームのタイトルを取得または設定します。
        /// </summary>
        public string Title
        {
            get { return Text; }
            set { Text = value; }
        }
        #endregion

        #region Message - 処理メッセージ取得または設定
        /// <summary>
        /// 処理メッセージを取得または設定します。
        /// </summary>
        public string Message
        {
            get { return lblMessage.Text; }
            set
            {
                lblMessage.Text = value;
                Update();
            }
        }
        #endregion

        #region ProgressMinimun - プログレスバー最小値取得または設定
        /// <summary>
        /// プログレスバーの最小値を取得または設定します。
        /// </summary>
        public int ProgressMinimun
        {
            get { return barProgress.Minimum; }
            set { barProgress.Minimum = value; }
        }
        #endregion

        #region ProgressMaximum - プログレスバー最大値取得または設定
        /// <summary>
        /// プログレスバーの最大値を取得または設定します。
        /// </summary>
        public int ProgressMaximum
        {
            get { return barProgress.Maximum; }
            set { barProgress.Maximum = value; }
        }
        #endregion

        #region ProgressStep - プログレスバー増加幅取得または設定
        /// <summary>
        /// プログレスバーの増分を取得または設定します。
        /// </summary>
        public int ProgressStep
        {
            get { return barProgress.Step; }
            set { barProgress.Step = value; }
        }
        #endregion

        #region CurrentValue - プログレスバー現在値取得または設定
        /// <summary>
        /// プログレスバーの現在値を取得または設定します。
        /// </summary>
        public int CurrentValue
        {
            get { return barProgress.Value; }
            set { barProgress.Value = value; }
        }
        #endregion

        #region ProgressVisible - プログレスバー可視状態取得または設定
        /// <summary>
        /// プログレスバーの可視状態を取得または設定します。
        /// </summary>
        public bool ProgressVisible
        {
            get { return barProgress.Visible; }
            set { barProgress.Visible = value; }
        }
        #endregion

        #endregion

        #region コンストラクタ
        /// <summary>
        /// 唯一のコンストラクタです。
        /// </summary>
        /// <param name="owner">親フォーム</param>
        public ProgressDialog(Form owner)
        {
            // コンポーネント初期化
            InitializeComponent();

            // ダイアログ表示位置設定
            Owner = owner;
            Location = new Point(Owner.Location.X + (Owner.Width - this.Width) / 2,
                    Owner.Location.Y + (Owner.Height - this.Height) / 2);
        }
        #endregion

        #region InitializeProgressbar - プログレスバー初期化
        /// <summary>
        /// 最小値、最大値を指定してプログレスバーの状態を初期化します。
        /// 増分は1に設定されます。
        /// </summary>
        /// <param name="minimum">プログレスバーの最小値</param>
        /// <param name="maximum">プログレスバーの最大値</param>
        public void InitializeProgressbar(int minimum, int maximum)
        {
            InitializeProgressbar(minimum, maximum, 1);
        }

        /// <summary>
        /// 最小値、最大値、増分を指定してプログレスバーの状態を初期化します。
        /// </summary>
        /// <param name="minimum">プログレスバーの最小値</param>
        /// <param name="maximum">プログレスバーの最大値</param>
        /// <param name="step">プログレスバーの増分</param>
        public void InitializeProgressbar(int minimum, int maximum, int step)
        {
            barProgress.Minimum = minimum;
            barProgress.Maximum = maximum;
            barProgress.Step = step;
            barProgress.Value = minimum;
        }
        #endregion

        #region StartProgress - プログレス表示開始
        /// <summary>
        /// タイトル、処理メッセージ、最小値、最大値を指定してプログレス表示を開始します。
        /// 増分は1に設定されます。
        /// </summary>
        /// <param name="title">フォームタイトル</param>
        /// <param name="message">処理メッセージ</param>
        /// <param name="minimum">プログレスバーの最小値</param>
        /// <param name="maximum">プログレスバーの最大値</param>
        public void StartProgress(string title, string message, int minimum, int maximum)
        {
            StartProgress(title, message, minimum, maximum, 1);
        }

        /// <summary>
        /// タイトル、処理メッセージ、最小値、最大値、増分を指定してプログレス表示を開始します。
        /// </summary>
        /// <param name="title">フォームタイトル</param>
        /// <param name="message">処理メッセージ</param>
        /// <param name="minimum">プログレスバーの最小値</param>
        /// <param name="maximum">プログレスバーの最大値</param>
        /// <param name="step">プログレスバーの増分</param>
        public void StartProgress(string title, string message, int minimum, int maximum, int step)
        {
            Text = title;
            lblMessage.Text = message;
            barProgress.Minimum = minimum;
            barProgress.Maximum = maximum;
            barProgress.Step = step;
            barProgress.Value = minimum;
            barProgress.Style = ProgressBarStyle.Blocks;

            Show();
            Update();
        }
        #endregion

        #region StartProgressWithMarquee
        /// <summary>
        /// タイトル、処理メッセージを指定してマーキースタイルでプログレス表示を開始します。
        /// </summary>
        /// <param name="title">フォームタイトル</param>
        /// <param name="message">処理メッセージ</param>
        public void StartProgressWithMarquee(string title, string message)
        {
            Text = title;
            lblMessage.Text = message;
            barProgress.Style = ProgressBarStyle.Marquee;

            Show();
            Update();
        }
        #endregion

        #region PerformStep - プログレスバー値増加
        /// <summary>
        /// プログレスバーの現在値を増分の分だけ増加させます。
        /// </summary>
        public void PerformStep()
        {
            barProgress.PerformStep();
        }

        /// <summary>
        /// プログレスバーの現在値を増分の分だけ増加させ、処理メッセージを更新します。
        /// </summary>
        /// <param name="message">処理メッセージ</param>
        public void PerformStep(string message)
        {
            lblMessage.Text = message;
            Update();

            PerformStep();
        }
        #endregion
    }
}
