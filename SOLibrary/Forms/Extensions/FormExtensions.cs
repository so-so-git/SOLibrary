using System.Windows.Forms;

namespace SO.Library.Forms.Extensions
{
    /// <summary>
    /// フォーム拡張メソッド提供クラス
    /// </summary>
    public static class FormExtensions
    {
        #region BackToOwner - 指定フォームを破棄、オーナーフォーム表示(Form拡張)

        /// <summary>
        /// (System.Windows.Forms.Form拡張)
        /// 指定フォームを破棄し、そのオーナーフォームがある場合はオーナーフォームを表示します。
        /// </summary>
        /// <param name="form">破棄するフォーム</param>
        public static void BackToOwner(this Form form)
        {
            if (form.Owner != null)
            {
                if (!form.Owner.Visible)
                {
                    form.Owner.Visible = true;
                }

                form.Owner.Activate();
                form.Owner.Invalidate(true);
            }

            form.Dispose();
        }

        #endregion
    }
}
