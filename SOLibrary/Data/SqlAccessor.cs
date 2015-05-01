using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace SO.Library.Data
{
    /// <summary>
    /// SQL Serverアクセス用ラッパクラス
    /// </summary>
    public class SqlAccessor : IDisposable
    {
        #region クラス定数

        /// <summary>既定のタイムアウト秒数</summary>
        protected const int DEFAULT_COMMAND_TIMEOUT = 180;

        #endregion

        #region インスタンス変数

        /// <summary>唯一のインスタンス</summary>
        protected static SqlAccessor _instance;

        /// <summary>SQLコネクション</summary>
        protected SqlConnection _conn;

        /// <summary>SQLコマンド</summary>
        protected SqlCommand _cmd;

        /// <summary>SQLトランザクション</summary>
        protected SqlTransaction _tran;

        /// <summary>リソース破棄済フラグ</summary>
        protected bool _isDisposed;

        #endregion

        #region プロパティ

        /// <summary>
        /// 唯一のインスタンスを取得します。
        /// </summary>
        public static SqlAccessor Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SqlAccessor();
                }
                return _instance;
            }
        }

        /// <summary>
        /// コネクションの状態を取得します。
        /// </summary>
        public ConnectionState? State
        {
            get
            {
                if (_conn == null)
                {
                    return null;
                }
                return _conn.State;
            }
        }

        /// <summary>
        /// SQLコマンドのタイムアウト秒数を取得します。
        /// </summary>
        public int CommandTimeout { get; private set; }

        #endregion

        #region コンストラクタ・デストラクタ

        /// <summary>
        /// プライベートコンストラクタです。
        /// </summary>
        protected SqlAccessor()
        {
            CommandTimeout = DEFAULT_COMMAND_TIMEOUT;
        }

        /// <summary>
        /// デストラクタです。
        /// </summary>
        ~SqlAccessor()
        {
            Dispose();
        }

        #endregion

        #region メソッド

        /// <summary>
        /// アプリケーション設定ファイルに記述されている、
        /// 既定の接続文字列を使用して接続を開きます。
        /// </summary>
        public void Open()
        {
            Open(ConfigurationManager.ConnectionStrings[0].ConnectionString);
        }

        /// <summary>
        /// 指定された接続文字列を使用して接続を開きます。
        /// </summary>
        /// <param name="connectionString">接続文字列</param>
        public void Open(string connectionString)
        {
            if (_conn != null)
            {
                _conn.Dispose();
            }

            _conn = new SqlConnection(connectionString);
            _conn.Open();

            if (_isDisposed)
            {
                GC.ReRegisterForFinalize(this);
                _isDisposed = false;
            }
        }

        /// <summary>
        /// トランザクションを開始します。
        /// </summary>
        public void BeginTransaction()
        {
            _tran = _conn.BeginTransaction();
        }

        /// <summary>
        /// トランザクションをコミットします。
        /// </summary>
        public void Commit()
        {
            _tran.Commit();

            _tran.Dispose();
            _tran = null;
        }

        /// <summary>
        /// トランザクションをロールバックします。
        /// </summary>
        public void Rollback()
        {
            _tran.Rollback();

            _tran.Dispose();
            _tran = null;
        }

        /// <summary>
        /// 指定されたSQLを実行し、結果をSqlDataReaderで取得します。
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="param">SQLパラメータ</param>
        /// <returns>実行結果</returns>
        public SqlDataReader ExecuteReader(string sql, params SqlParameter[] param)
        {
            CreateSqlCommand(sql, param);

            SqlDataReader ret = _cmd.ExecuteReader();

            _cmd.Dispose();
            _cmd = null;

            return ret;
        }

        /// <summary>
        /// 指定されたSQLを実行し、結果をDataTableで取得します。
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="param">SQLパラメータ</param>
        /// <returns>実行結果</returns>
        public DataTable ExecuteTable(string sql, params SqlParameter[] param)
        {
            CreateSqlCommand(sql, param);

            var ret = new DataTable();
            using (var adapter = new SqlDataAdapter(_cmd))
            {
                adapter.Fill(ret);
            }

            _cmd.Dispose();
            _cmd = null;

            return ret;
        }

        /// <summary>
        /// 指定されたSQLを実行し、変更を与えた行数を取得します。
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="param">SQLパラメータ</param>
        /// <returns>変更を与えた行数</returns>
        public int ExecuteNonQuery(string sql, params SqlParameter[] param)
        {
            CreateSqlCommand(sql, param);

            int ret = _cmd.ExecuteNonQuery();

            _cmd.Dispose();
            _cmd = null;

            return ret;
        }

        /// <summary>
        /// SQLコマンドを生成します。
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="param">SQLパラメータ</param>
        protected void CreateSqlCommand(string sql, params SqlParameter[] param)
        {
            _cmd = _conn.CreateCommand();

            _cmd.CommandType = CommandType.Text;
            _cmd.CommandTimeout = CommandTimeout;
            _cmd.CommandText = sql;
            _cmd.Parameters.AddRange(param);

            if (_tran != null)
            {
                _cmd.Transaction = _tran;
            }
        }

        /// <summary>
        /// 接続を閉じます。
        /// </summary>
        public void Close()
        {
            if (_cmd != null)
            {
                _cmd.Dispose();
                _cmd = null;
            }

            if (_tran != null)
            {
                _tran.Rollback();
                _tran.Dispose();
                _tran = null;
            }

            if (_conn != null && _conn.State != ConnectionState.Closed)
            {
                _conn.Close();
            }
        }

        /// <summary>
        /// 全てのリソースを破棄します。
        /// トランザクションがコミットされていない場合は、ロールバックされます。
        /// </summary>
        public void Dispose()
        {
            Close();

            _conn.Dispose();
            _conn = null;

            GC.SuppressFinalize(this);
            _isDisposed = true;
        }

        #endregion
    }
}
