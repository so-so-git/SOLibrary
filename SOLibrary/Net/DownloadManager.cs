using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;

namespace SO.Library.Net
{
    /// <summary>
    /// ダウンロードマネージャクラス
    /// </summary>
    public static class DownloadManager
    {
        #region クラス定数

        /// <summary>HTTPリクエストヘッダのUser-Agentにセットする値</summary>
        private const string USER_AGENT = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)";

        #endregion

        #region インスタンス変数

        /// <summary>非同期ウェブクライアントのリスト</summary>
        private static List<WebClient> _asyncWcList = new List<WebClient>();

        #endregion

        #region プロパティ

        /// <summary>
        /// 進行中の非同期ダウンロードタスクが存在するかを取得します。
        /// </summary>
        public static bool IsDownloadingAsync
        {
            get { return _asyncWcList.Any(); }
        }

        #endregion

        #region 同期処理

        /// <summary>
        /// 同期処理で指定されたアドレスからファイルをダウンロードします。
        /// </summary>
        /// <param name="address">ダウンロード元アドレス</param>
        /// <param name="filePath">ファイル保存先パス</param>
        public static void DownloadFile(string address, string filePath)
        {
            using (var wc = new WebClient())
            {
                wc.Headers.Add("user-agent", USER_AGENT);
                wc.DownloadFile(address, filePath);
            }
        }

        /// <summary>
        /// 同期処理で指定されたアドレスからデータをダウンロードします。
        /// </summary>
        /// <param name="address">ダウンロード元アドレス</param>
        /// <returns>ダウンロードしたデータ</returns>
        public static byte[] DownloadData(string address)
        {
            using (var wc = new WebClient())
            {
                wc.Headers.Add("user-agent", USER_AGENT);
                return wc.DownloadData(address);
            }
        }

        #endregion

        #region 非同期処理

        /// <summary>
        /// 非同期処理で指定されたアドレスからファイルをダウンロードします。
        /// </summary>
        /// <param name="address">ダウンロード元アドレス</param>
        /// <param name="filePath">ファイル保存先パス</param>
        /// <param name="progressChanged">進捗変更時イベントハンドラ</param>
        /// <param name="completed">ダウンロード完了時イベントハンドラ</param>
        /// <returns>ダウンロードタスク判別用に使用する為のトークン</returns>
        public static int DownloadFileAsync(string address, string filePath,
            DownloadProgressChangedEventHandler progressChanged,
            AsyncCompletedEventHandler completed)
        {
            var uri = new Uri(address);
            var wc = new WebClient();
            wc.Headers.Add("user-agent", USER_AGENT);

            if (progressChanged != null)
                wc.DownloadProgressChanged += progressChanged;

            if (completed != null)
                wc.DownloadFileCompleted += completed;

            wc.DownloadFileCompleted += DownloadAsyncCompleted;

            int token = wc.GetHashCode();

            _asyncWcList.Add(wc);
            wc.DownloadFileAsync(uri, filePath, token);

            return wc.GetHashCode();
        }

        /// <summary>
        /// 非同期処理で指定されたアドレスからデータをダウンロードします。
        /// </summary>
        /// <param name="address">ダウンロード元アドレス</param>
        /// <param name="progressChanged">進捗変更時イベントハンドラ</param>
        /// <param name="completed">ダウンロード完了時イベントハンドラ</param>
        /// <returns>ダウンロードタスク判別用に使用する為のトークン</returns>
        public static int DownloadDataAsync(string address,
            DownloadProgressChangedEventHandler progressChanged,
            DownloadDataCompletedEventHandler completed)
        {
            var uri = new Uri(address);
            var wc = new WebClient();
            wc.Headers.Add("user-agent", USER_AGENT);

            if (progressChanged != null)
                wc.DownloadProgressChanged += progressChanged;

            if (completed != null)
                wc.DownloadDataCompleted += completed;

            wc.DownloadDataCompleted += DownloadAsyncCompleted;

            int token = wc.GetHashCode();

            _asyncWcList.Add(wc);
            wc.DownloadDataAsync(uri, token);

            return wc.GetHashCode();
        }

        /// <summary>
        /// 非同期ダウンロード処理をキャンセルします。
        /// </summary>
        /// <param name="token">キャンセル対象のタスクのトークン</param>
        public static void CancelDownloadAsync(int token)
        {
            var wc = _asyncWcList.Where(w => w.GetHashCode() == token).SingleOrDefault();

            if (wc != null)
            {
                wc.CancelAsync();

                _asyncWcList.Remove(wc);
                wc.Dispose();
            }
        }

        /// <summary>
        /// 全ての非同期ファイルダウンロード処理をキャンセルします。
        /// </summary>
        public static void CancelAllDownloadAsync()
        {
            foreach (var wc in _asyncWcList)
            {
                wc.CancelAsync();
                wc.Dispose();
            }

            _asyncWcList.Clear();
        }

        /// <summary>
        ///  非同期ファイルダウンロード処理完了時の必須イベント処理です。
        ///  非同期ウェブクライアントのリストから処理が完了したものを除去します。
        /// </summary>
        /// <param name="sender">イベント送信元オブジェクト</param>
        /// <param name="e">イベント引数</param>
        private static void DownloadAsyncCompleted(object sender, AsyncCompletedEventArgs e)
        {
            var wc = sender as WebClient;

            _asyncWcList.Remove(wc);
            wc.Dispose();
        }

        #endregion
    }
}
