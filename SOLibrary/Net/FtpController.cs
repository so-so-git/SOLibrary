using System;
using System.IO;
using System.Net;
using System.Text;

namespace SO.Library.Net
{
    /// <summary>
    /// FTP制御クラス
    /// </summary>
    public static class FtpController
    {
        #region クラス定数

        /// <summary>アップロードファイル読み込み時のブロックのバイト数</summary>
        private const int READ_BLOCK_BYTES = 1024;

        /// <summary>ファイルサイズ取得要求でエラーとなったことを表す数値</summary>
        private const long FILESIZE_GET_ERROR = -1;

        #endregion

        #region プロパティ

        /// <summary>
        /// 処理完了後に接続を閉じるかを取得・設定します。
        /// 規定値はfalseです。
        /// </summary>
        public static bool KeepAlive { get; set; }

        /// <summary>
        /// パッシブモードで転送するかを取得・設定します。
        /// falseが設定されている場合はアクティブモードで転送を行ないます。
        /// 規定値はfalseです。
        /// </summary>
        public static bool UsePassive { get; set; }

        /// <summary>
        /// FTPサーバの文字コードを取得・設定します。
        /// 規定値はUTF8です。
        /// </summary>
        public static Encoding ServerEncoding { get; set; }

        /// <summary>
        /// 最後に実行された処理のステータスコードを取得します。
        /// </summary>
        public static FtpStatusCode LastStatusCode { get; private set; }

        /// <summary>
        /// 最後に実行された処理のステータス説明を取得します。
        /// </summary>
        public static string LastStatusDescription { get; private set; }

        #endregion

        #region 静的コンストラクタ
        /// <summary>
        /// 静的コンストラクタです。
        /// </summary>
        static FtpController()
        {
            KeepAlive = false;
            UsePassive = false;
            ServerEncoding = Encoding.UTF8;
        }
        #endregion

        #region UploadFile - ファイルアップロード
        /// <summary>
        /// FTP転送でファイルをアップロードします。
        /// </summary>
        /// <remarks>
        /// FTP処理の失敗によるSystem.Net.WebExceptionは本メソッド内で捕捉し、戻り値falseを返却します。
        /// </remarks>
        /// <param name="useBinary">true:バイナリ転送 / false:アスキー転送</param>
        /// <param name="filePath">アップロードするファイルのパス</param>
        /// <param name="uploadUri">アップロード先のURI(フルパス)</param>
        /// <param name="ftpUserId">FTPサーバのユーザID</param>
        /// <param name="ftpPassword">FTPサーバのパスワード</param>
        /// <returns>true:正常終了 / false:異常終了</returns>
        public static bool UploadFile(bool useBinary, string filePath, string uploadUri,
                                      string ftpUserId, string ftpPassword)
        {
            try
            {
                // FtpWebRequestの作成
                FtpWebRequest request = CreateRequest(
                        WebRequestMethods.Ftp.UploadFile, uploadUri, ftpUserId, ftpPassword);

                // モード設定
                request.UseBinary = useBinary;

                using (Stream upStream = request.GetRequestStream())
                using (var fStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    var bfr = new byte[READ_BLOCK_BYTES];
                    int readLen;
                    while ((readLen = fStream.Read(bfr, 0, bfr.Length)) > 0)
                        // 対象ファイルから読み込んだバイトをUpload用Streamに書き込み
                        upStream.Write(bfr, 0, readLen);
                }

                // 転送結果取得
                using (FtpWebResponse response = request.GetResponse() as FtpWebResponse)
                {
                    LastStatusCode = response.StatusCode;
                    LastStatusDescription = response.StatusDescription;
                }
            }
            catch (WebException)
            {
                // FTP処理の失敗時は中でハンドル
                return false;
            }

            return (LastStatusCode == FtpStatusCode.ClosingData ||
                    LastStatusCode == FtpStatusCode.FileActionOK);
        }
        #endregion

        #region UploadFiles - 複数のファイルをアップロード
        /// <summary>
        /// FTP転送で複数のファイルをアップロードします。
        /// </summary>
        /// <remarks>
        /// FTP処理の失敗によるSystem.Net.WebExceptionは本メソッド内で捕捉し、戻り値falseを返却します。
        /// </remarks>
        /// <param name="useBinary">true:バイナリ転送 / false:アスキー転送</param>
        /// <param name="filePathes">アップロードするファイルのパスの配列</param>
        /// <param name="uploadUri">アップロード先のURI(フルパス)</param>
        /// <param name="ftpUserId">FTPサーバのユーザID</param>
        /// <param name="ftpPassword">FTPサーバのパスワード</param>
        /// <returns>true:正常終了 / false:異常終了</returns>
        public static bool UploadFiles(bool useBinary, string[] filePathes, string uploadUri,
                                       string ftpUserId, string ftpPassword)
        {
            bool ret = true;
            foreach (var filePath in filePathes)
                ret &= UploadFile(useBinary, filePath, uploadUri, ftpUserId, ftpPassword);

            return ret;
        }
        #endregion

        #region DeleteFile - FTPサーバ上のファイル削除
        /// <summary>
        /// FTPサーバ上の指定ファイルを削除します。
        /// </summary>
        /// <remarks>
        /// FTP処理の失敗によるSystem.Net.WebExceptionは本メソッド内で捕捉し、戻り値falseを返却します。
        /// </remarks>
        /// <param name="deleteUri">削除対象のファイルのURI(フルパス)</param>
        /// <param name="ftpUserId">FTPサーバのユーザID</param>
        /// <param name="ftpPassword">FTPサーバのパスワード</param>
        /// <returns>true:正常終了 / false:異常終了</returns>
        public static bool DeleteFile(string deleteUri, string ftpUserId, string ftpPassword)
        {
            try
            {
                // FtpWebRequestの作成
                FtpWebRequest request = CreateRequest(
                        WebRequestMethods.Ftp.DeleteFile, deleteUri, ftpUserId, ftpPassword);

                // 削除実行・結果取得
                using (FtpWebResponse response = request.GetResponse() as FtpWebResponse)
                {
                    LastStatusCode = response.StatusCode;
                    LastStatusDescription = response.StatusDescription;
                }
            }
            catch (WebException)
            {
                // FTP処理の失敗時は中でハンドル
                return false;
            }

            return (LastStatusCode == FtpStatusCode.ClosingData ||
                    LastStatusCode == FtpStatusCode.FileActionOK);
        }
        #endregion

        #region DeleteFilesInDirectory - FTPサーバ上の指定ディレクトリ内の全ファイル削除
        /// <summary>
        /// FTPサーバ上の指定ディレクトリ内の全てのファイルを削除します。
        /// </summary>
        /// <remarks>
        /// FTP処理の失敗によるSystem.Net.WebExceptionは本メソッド内で捕捉し、戻り値falseを返却します。
        /// </remarks>
        /// <param name="deleteDirUri">削除対象のディレクトリのURI(フルパス)</param>
        /// <param name="ftpUserId">FTPサーバのユーザID</param>
        /// <param name="ftpPassword">FTPサーバのパスワード</param>
        /// <returns>true:正常終了 / false:異常終了</returns>
        public static bool DeleteFilesInDirectory(string deleteDirUri, string ftpUserId, string ftpPassword)
        {
            try
            {
                // ls用FtpWebRequestの作成
                FtpWebRequest lsRequest = CreateRequest(
                        WebRequestMethods.Ftp.ListDirectoryDetails, deleteDirUri, ftpUserId, ftpPassword);

                // ls実行・結果取得
                using (FtpWebResponse lsResponse = lsRequest.GetResponse() as FtpWebResponse)
                {
                    LastStatusCode = lsResponse.StatusCode;
                    LastStatusDescription = lsResponse.StatusDescription;

                    if (LastStatusCode != FtpStatusCode.OpeningData &&
                            LastStatusCode != FtpStatusCode.DataAlreadyOpen)
                        return false;

                    // 存在する全てのファイルに対して削除処理実行
                    // (エラーが発生しても、他のファイルに対して処理継続する)
                    bool ret = true;
                    using (var lsReader = new StreamReader(lsResponse.GetResponseStream(), ServerEncoding))
                    {
                        string bfr;
                        while ((bfr = lsReader.ReadLine()) != null)
                        {
                            // ディレクトリの場合はスキップ
                            if (bfr.StartsWith("d")) continue;

                            // 削除対象のURIを組み立て
                            if (!deleteDirUri.EndsWith("/")) deleteDirUri += "/";

                            string deleteFile = bfr.Substring(bfr.LastIndexOf(" ")).Trim();
                            string deleteUri = deleteDirUri + deleteFile;

                            if (!DeleteFile(deleteUri, ftpUserId, ftpPassword))
                                ret = false;
                        }
                    }

                    return ret;
                }
            }
            catch (WebException)
            {
                return false;
            }
        }
        #endregion

        #region GetFileSize - FTPサーバ上のファイルサイズ取得
        /// <summary>
        /// FTPサーバ上の指定ファイルのサイズを取得します。
        /// </summary>
        /// <remarks>
        /// FTP処理の失敗によるSystem.Net.WebExceptionは本メソッド内で捕捉し、戻り値falseを返却します。
        /// </remarks>
        /// <param name="targetUri">サイズ取得対象ファイルのURI(フルパス)</param>
        /// <param name="ftpUserId">FTPサーバのユーザID</param>
        /// <param name="ftpPassword">FTPサーバのパスワード</param>
        /// <returns>ファイルサイズ(異常終了時はFILESIZE_GET_ERRORの値)</returns>
        public static long GetFileSize(string targetUri, string ftpUserId, string ftpPassword)
        {
            try
            {
                // FtpWebRequestの作成
                FtpWebRequest request = CreateRequest(
                        WebRequestMethods.Ftp.GetFileSize, targetUri, ftpUserId, ftpPassword);

                // サイズ取得実行・結果取得
                using (FtpWebResponse response = request.GetResponse() as FtpWebResponse)
                {
                    LastStatusCode = response.StatusCode;
                    LastStatusDescription = response.StatusDescription;

                    // 正常に取得出来た場合はファイルサイズを、異常時はエラーコードを返却
                    return LastStatusCode == FtpStatusCode.FileStatus ?
                            response.ContentLength :
                            FILESIZE_GET_ERROR;
                }
            }
            catch (WebException)
            {
                return FILESIZE_GET_ERROR;
            }
        }
        #endregion

        #region CreateRequest - FtpWebRequestオブジェクト生成
        /// <summary>
        /// FtpWebRequestオブジェクトを生成します。
        /// </summary>
        /// <param name="method">FTPのメソッド名(WebRequestMethods.Ftp から指定)</param>
        /// <param name="targetUri">処理対象のURI(フルパス)</param>
        /// <param name="ftpUserId">FTPサーバのユーザID</param>
        /// <param name="ftpPassword">FTPサーバのパスワード</param>
        /// <returns>生成したFtpWebRequestオブジェクト</returns>
        private static FtpWebRequest CreateRequest(string method, string targetUri,
                                                   string ftpUserId, string ftpPassword)
        {
            FtpWebRequest request = WebRequest.Create(targetUri) as FtpWebRequest;
            request.Credentials = new NetworkCredential(ftpUserId, ftpPassword);
            request.Proxy = null;
            request.Method = method;

            if (method == WebRequestMethods.Ftp.UploadFile)
            {
                // 転送時は転送モード等も設定
                request.KeepAlive = FtpController.KeepAlive;
                request.UsePassive = FtpController.UsePassive;
            }

            return request;
        }
        #endregion
    }
}
