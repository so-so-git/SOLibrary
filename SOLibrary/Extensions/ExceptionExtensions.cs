using System;
using System.Reflection;

using SO.Library.Forms;
using SO.Library.IO;

using Config = System.Configuration.ConfigurationManager;

namespace SO.Library.Extensions
{
    /// <summary>
    /// 例外拡張メソッド提供クラス
    /// </summary>
    public static class ExceptionExtensions
    {
        #region DoDefault - デフォルトエラー処理(Exception拡張)

        /// <summary>
        /// (System.Exceptionクラス拡張)
        /// エラーログ出力、エラーダイアログ表示を行ないます。
        /// </summary>
        /// <param name="ex">例外オブジェクト</param>
        /// <param name="className">例外発生元クラス名</param>
        /// <param name="method">例外発生元メソッド情報</param>
        public static void DoDefault(this Exception ex, string className, MethodBase method)
        {
            DoDefault(ex, className, method, null);
        }

        /// <summary>
        /// (System.Exceptionクラス拡張)
        /// 補足情報付きでエラーログ出力、エラーダイアログ表示を行ないます。
        /// </summary>
        /// <param name="ex">例外オブジェクト</param>
        /// <param name="className">例外発生元クラス名</param>
        /// <param name="method">例外発生元メソッド情報</param>
        /// <param name="optionMessage">補足情報</param>
        public static void DoDefault(this Exception ex, string className,
                                     MethodBase method, string optionMessage)
        {
            // エラーログ出力
            var logger = new Logger(Config.AppSettings["ErrorLogPath"]);
            logger.WriteErrorLog(className, method.Name, ex, optionMessage);

            // エラーダイアログ表示
            FormUtilities.ShowExceptionMessage(className, method.Name, ex, optionMessage);
        }

        #endregion
    }
}
