using System.Threading;

namespace SO.Library.Forms
{
    partial class CircleProgressDialog
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (_animationThread != null && _animationThread.IsAlive)
            {
                try { _animationThread.Abort(); }
                catch (ThreadInterruptedException) { }
                catch (ThreadAbortException) { }

                _animationThread = null;
            }

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.picAnime = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picAnime)).BeginInit();
            this.SuspendLayout();
            // 
            // picAnime
            // 
            this.picAnime.BackColor = System.Drawing.SystemColors.Window;
            this.picAnime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picAnime.Location = new System.Drawing.Point(0, 0);
            this.picAnime.Margin = new System.Windows.Forms.Padding(0);
            this.picAnime.Name = "picAnime";
            this.picAnime.Size = new System.Drawing.Size(100, 100);
            this.picAnime.TabIndex = 0;
            this.picAnime.TabStop = false;
            // 
            // CircleProgressDialog
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(100, 100);
            this.ControlBox = false;
            this.Controls.Add(this.picAnime);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Meiryo UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "CircleProgressDialog";
            this.Opacity = 0.5D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Resize += new System.EventHandler(this.CircleProgressDialog_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.picAnime)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox picAnime;
    }
}

