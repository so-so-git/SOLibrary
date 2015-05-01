using System;
using System.Text;
using System.IO;

namespace SO.Library.IO
{
    /// <summary>
    /// テキストファイルへのロギング機能提供クラス
    /// </summary>
    public class Logger
    {
        #region クラス・インスタンス変数

        /// <summary>排他制御用オブジェクト</summary>
        protected static object _lockObj = new object();

        /// <summary>ログファイル</summary>
        protected FileInfo _logFile;

        /// <summary>ログファイルのエンコーディング指定</summary>
        protected Encoding _encoding;

        #endregion

        #region プロパティ

        /// <summary>
        /// ログファイルを取得または設定します。
        /// </summary>
        public FileInfo LogFile
        {
            get { return _logFile; }
            set { Initialize(value.FullName, _encoding); }
        }

        /// <summary>
        /// ログファイルの文字コードを取得または設定します。
        /// </summary>
        public Encoding FileEncoding
        {
            get { return _encoding; }
            set { Initialize(_logFile.FullName, value); }
        }

        #endregion

        #region コンストラクタ

        /// <summary>
        /// デフォルトのコンストラクタです。
        /// </summary>
        /// <param name="logPath">ログファイルパス</param>
        public Logger(string logPath)
        {
            // フィールド初期化
            Initialize(logPath, Encoding.Default);
        }

        /// <summary>
        /// 文字コード指定付きのコンストラクタです。
        /// </summary>
        /// <param name="logPath">ログファイルパス</param>
        /// <param name="encoding">ログファイルの文字コード</param>
        public Logger(string logPath, Encoding encoding)
        {
            // フィールド初期化
            Initialize(logPath, encoding);
        }

        #endregion

        #region Initialize - フィールド初期化

        /// <summary>
        /// メンバ変数の初期化を行ないます。
        /// </summary>
        /// <param name="logPath">ログファイルパス</param>
        /// <param name="encoding">ログファイルの文字コード</param>
        protected virtual void Initialize(string logPath, Encoding encoding)
        {
            // 各フィールド初期化
            _logFile = new FileInfo(logPath);
            _encoding = encoding;

            // 作成先ディレクトリ存在確認(無ければ作る)
            if (!Directory.Exists(Path.GetDirectoryName(logPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(logPath));
            }

            // ログファイルローテーション
            RotationLogFile();
        }

        #endregion

        #region RotationLogFile - ログファイルローテーション

        /// <summary>
        /// ログファイルのローテーションを行います。
        /// </summary>
        protected virtual void RotationLogFile()
        {
            // ログファイル存在確認
            if (File.Exists(_logFile.FullName))
            {
                if (File.GetLastAccessTime(_logFile.FullName).Date.CompareTo(DateTime.Today) != 0)
                {
                    // ファイルローテーション
                    File.Move(_logFile.FullName, _logFile.FullName + "-" + DateTime.Today.ToString("yyyy.MM.dd"));

                    // 新規ログファイル作成
                    CreateNewLog();
                }
            }
            else
            {
                // 新規ログファイル作成
                CreateNewLog();
            }
        }

        #endregion

        #region CreateNewLog - 新規ログファイル作成

        /// <summary>
        /// 空の新規ログファイルを作成します。
        /// </summary>
        protected virtual void CreateNewLog()
        {
            // Createメソッドでログファイル作成、Streamを即解放
            using (File.Create(_logFile.FullName)) { }
        }

        #endregion

        #region WriteLog - ログ書込

        /// <summary>
        /// 処理ログをログファイルに書き込みます。
        /// </summary>
        /// <param name="message"></param>
        public virtual void WriteLog(string message)
        {
            WriteLog(null, null, message);
        }

        /// <summary>
        /// クラス名、メソッド名を含めて処理ログをログファイルに書き込みます。
        /// </summary>
        /// <param name="className">ログ出力元クラス名</param>
        /// <param name="methodName">ログ出力元メソッド名</param>
        /// <param name="message">処理ログメッセージ</param>
        public virtual void WriteLog(string className, string methodName, string message)
        {
            lock (_lockObj)
            {
                using (var sw = new StreamWriter(_logFile.FullName, true, _encoding))
                {
                    // ログを出力
                    sw.WriteLine(GetLogHeader(className, methodName));
                    sw.WriteLine(message);
                    sw.Flush();
                }
            }
        }

        #endregion

        #region WriteErrorLog - エラーログ書込

        /// <summary>
        /// エラーログをログファイルに書き込みます。
        /// </summary>
        /// <param name="className">例外発生元クラス名</param>
        /// <param name="methodName">例外発生元メソッド名</param>
        /// <param name="ex">例外オブジェクト</param>
        public virtual void WriteErrorLog(string className, string methodName, Exception ex)
        {
            // 補足情報無しでログ書込
            WriteErrorLog(className, methodName, ex, null);
        }

        /// <summary>
        /// 例外補足情報付きのエラーログをログファイルに書き込みます。
        /// </summary>
        /// <param name="className">例外発生元クラス名</param>
        /// <param name="methodName">例外発生元メソッド名</param>
        /// <param name="ex">例外オブジェクト</param>
        /// <param name="optionMessage">例外補足情報</param>
        public virtual void WriteErrorLog(string className, string methodName, Exception ex, string optionMessage)
        {
            // 出力文字列構築
            var sb = new StringBuilder();
            sb.AppendLine(GetLogHeader(className, methodName));
            sb.AppendLine(ex.ToString());

            // 補足情報追加
            if (optionMessage != null)
            {
                sb.AppendLine("[補足情報]");
                sb.AppendLine(optionMessage);
            }

            lock (_lockObj)
            {
                using (var sw = new StreamWriter(_logFile.FullName, true, _encoding))
                {
                    // エラー内容出力
                    sw.WriteLine(sb.ToString());
                    sw.Flush();
                }
            }
        }

        #endregion

        #region GetLogHeader - ログのヘッダを取得

        /// <summary>
        /// ログのヘッダを取得を取得します。
        /// </summary>
        /// <param name="className">ログ出力元クラス名</param>
        /// <param name="methodName">ログ出力元メソッド名</param>
        /// <returns></returns>
        private string GetLogHeader(string className, string methodName)
        {
            var sb = new StringBuilder();
            sb.Append("<");
            sb.Append(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            sb.Append(">");
            if (!string.IsNullOrEmpty(className))
            {
                sb.Append(" : ").Append(className);
                if (string.IsNullOrEmpty(methodName))
                {
                    sb.Append("#").AppendLine(methodName);
                }
            }

            return sb.ToString();
        }

        #endregion
    }
}
