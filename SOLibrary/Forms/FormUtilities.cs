using System;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using SO.Library.IO;

namespace SO.Library.Forms
{
    /// <summary>
    /// フォーム汎用機能提供クラス
    /// </summary>
    public static class FormUtilities
    {
        #region クラス定数

        /// <summary>メニューアイテム指定時の区切り文字</summary>
        public const string MENU_PATH_SEPARATOR = "/";

        #endregion

        #region ShowMessage - メッセージダイアログ表示

        /// <summary>
        /// 指定されたIDを持つメッセージをダイアログで表示します。
        /// </summary>
        /// <param name="messageId">メッセージID</param>
        /// <param name="args">置換パラメータ</param>
        /// <returns>応答結果</returns>
        public static DialogResult ShowMessage(string messageId, params string[] args)
        {
            MessageInfo info = MessageXml.GetMessageInfo(messageId, args);

            return MessageBox.Show(info.message, info.caption, info.buttons, info.icon);
        }

        #endregion

        #region ShowInformationMessage - 情報ダイアログ表示

        /// <summary>
        /// 指定されたメッセージを情報ダイアログで表示します。
        /// </summary>
        /// <param name="message">表示メッセージ</param>
        internal static void ShowInformationMessage(string message)
        {
            MessageBox.Show(message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// ボタン種別を指定し、指定されたメッセージを警告ダイアログで表示します。
        /// </summary>
        /// <param name="message">表示メッセージ</param>
        /// <param name="buttons">ボタン種別</param>
        /// <returns>応答結果</returns>
        internal static DialogResult ShowInformationMessage(string message, MessageBoxButtons buttons)
        {
            return MessageBox.Show(message, "Information", buttons, MessageBoxIcon.Information);
        }

        #endregion

        #region ShowWarningMessage - 警告ダイアログ表示

        /// <summary>
        /// OKボタンのみの表示で、指定されたメッセージを警告ダイアログで表示します。
        /// </summary>
        /// <param name="message">表示メッセージ</param>
        internal static void ShowWarningMessage(string message)
        {
            MessageBox.Show(message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// ボタン種別を指定し、指定されたメッセージを警告ダイアログで表示します。
        /// </summary>
        /// <param name="message">表示メッセージ</param>
        /// <param name="buttons">ボタン種別</param>
        /// <returns>応答結果</returns>
        internal static DialogResult ShowWarningMessage(string message, MessageBoxButtons buttons)
        {
            return MessageBox.Show(message, "Warning", buttons, MessageBoxIcon.Warning);
        }

        #endregion

        #region ShowErrorMessage - エラーダイアログ表示

        /// <summary>
        /// OKボタンのみの表示で、指定されたメッセージを警告ダイアログで表示します。
        /// </summary>
        /// <param name="message">表示メッセージ</param>
        internal static void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// ボタン種別を指定し、指定されたメッセージを警告ダイアログで表示します。
        /// </summary>
        /// <param name="message">表示メッセージ</param>
        /// <param name="buttons">ボタン種別</param>
        /// <returns>応答結果</returns>
        internal static DialogResult ShowErrorMessage(string message, MessageBoxButtons buttons)
        {
            return MessageBox.Show(message, "Error", buttons, MessageBoxIcon.Error);
        }

        #endregion

        #region ShowQuestionMessage - 確認ダイアログ表示

        /// <summary>
        /// はい・いいえボタンのみの表示で、指定されたメッセージを確認ダイアログで表示します。
        /// </summary>
        /// <param name="message">表示メッセージ</param>
        /// <returns>応答結果</returns>
        internal static DialogResult ShowQuestionMessage(string message)
        {
            return MessageBox.Show(message, "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        /// <summary>
        /// ボタン種別を指定し、指定されたメッセージを確認ダイアログで表示します。
        /// </summary>
        /// <param name="message">表示メッセージ</param>
        /// <param name="buttons">ボタン種別</param>
        /// <returns>応答結果</returns>
        internal static DialogResult ShowQuestionMessage(string message, MessageBoxButtons buttons)
        {
            return MessageBox.Show(message, "Question", buttons, MessageBoxIcon.Question);
        }

        #endregion

        #region ShowExceptionMessage - 例外内容通知ダイアログ表示

        /// <summary>
        /// 例外内容通知ダイアログを表示します。
        /// </summary>
        /// <param name="ex">例外オブジェクト</param>
        public static void ShowExceptionMessage(Exception ex)
        {
            ShowExceptionMessage(null, null, ex, null);
        }

        /// <summary>
        /// 捕捉情報付きで例外内容通知ダイアログを表示します。
        /// </summary>
        /// <param name="ex">例外オブジェクト</param>
        /// <param name="optionMessage">補足情報</param>
        public static void ShowExceptionMessage(Exception ex, string optionMessage)
        {
            ShowExceptionMessage(null, null, ex, optionMessage);
        }

        /// <summary>
        /// 発生元情報付きで例外内容通知ダイアログを表示します。
        /// </summary>
        /// <param name="className">エラー発生元クラス名</param>
        /// <param name="methodName">エラー発生元メソッド名</param>
        /// <param name="ex">例外オブジェクト</param>
        public static void ShowExceptionMessage(string className, string methodName, Exception ex)
        {
            ShowExceptionMessage(className, methodName, ex, null);
        }

        /// <summary>
        /// 発生元情報、捕捉情報付きで例外内容通知ダイアログを表示します。
        /// </summary>
        /// <param name="className">エラー発生元クラス名</param>
        /// <param name="methodName">エラー発生元メソッド名</param>
        /// <param name="ex">例外オブジェクト</param>
        /// <param name="optionMessage">補足情報</param>
        public static void ShowExceptionMessage(string className, string methodName, Exception ex, string optionMessage)
        {
            var errMsg = new StringBuilder();

            // エラー発生元情報
            if (!string.IsNullOrEmpty(className) && !string.IsNullOrEmpty(methodName))
            {
                errMsg.Append(className);
                errMsg.Append(".");
                errMsg.Append(methodName);
                errMsg.AppendLine("内で以下のエラーが発生しました。");
                errMsg.AppendLine();
            }
            errMsg.Append(ex.ToString());

            // 補足情報
            if (optionMessage != null)
            {
                errMsg.AppendLine(Environment.NewLine);
                errMsg.AppendLine("補足情報：");
                errMsg.Append(optionMessage);
            }

            // メッセージボックスでエラー内容通知
            MessageBox.Show(errMsg.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #endregion

        #region GetMenuItem - 指定した名称を持つメニューアイテムを取得

        /// <summary>
        /// 指定した名前を持つメニューアイテムを取得します。
        /// メニューアイテムの階層はMENU_PATH_SEPARATORで区切って指定します。
        /// </summary>
        /// <typeparam name="T">System.Windows.Forms.ToolStripItem及びその継承クラス</typeparam>
        /// <param name="items">検索起点となるToolStripItemCollection</param>
        /// <param name="pathName">階層構造で指定されたメニューアイテムの名前</param>
        /// <returns>pathNameで指定されたメニューアイテム</returns>
        public static T GetMenuItem<T>(ToolStripItemCollection items, string pathName)
            where T : ToolStripItem
        {
            string name;
            bool last;
            int pos = pathName.IndexOf(MENU_PATH_SEPARATOR);
            if (pos == -1)
            {
                name = pathName;
                last = true;
            }
            else
            {
                name = Regex.Split(pathName, MENU_PATH_SEPARATOR)[0];
                last = false;
            }

            if (last)
            {
                return items[name] as T;
            }
            else
            {
                var item = items[name] as ToolStripMenuItem;
                return item == null ? null :
                    GetMenuItem<T>(item.DropDownItems, pathName.Substring(pos + 1));
            }
        }

        #endregion
    }
}
