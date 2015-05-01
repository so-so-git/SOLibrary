using System;

namespace SO.Library.Net
{
    /// <summary>
    /// ネットワークアダプタ情報クラス
    /// </summary>
    public sealed class AdapterInfo
    {
        #region インスタンス変数

        /// <summary>IPアドレス</summary>
        private string _ipAddress;

        /// <summary>サブネットマスク</summary>
        private string _subnetMask;

        /// <summary>デフォルトゲートウェイ</summary>
        private string _defaultGateway;

        /// <summary>優先DNSサーバアドレス</summary>
        private string _primaryDns;

        /// <summary>代替DNSサーバアドレス</summary>
        private string _secondaryDns;

        #endregion

        #region プロパティ

        /// <summary>
        /// ネットワークアダプタの名称を取得します。
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// DHCPを利用したIPの自動取得を行うかどうかを取得または設定します。
        /// </summary>
        public bool UseIpDhcp { get; set; }

        /// <summary>
        /// IPアドレスを取得または設定します。
        /// 書式は「xxx.xxx.xxx.xxx」です。
        /// </summary>
        public string IpAddress
        {
            get { return _ipAddress; }
            set
            {
                if (!NetworkUtilities.IsValidAddress(value))
                    throw new ArgumentException("不正な書式のIPアドレスです。");

                _ipAddress = value;
            }
        }

        /// <summary>
        /// サブネットマスクを取得または設定します。
        /// 書式は「xxx.xxx.xxx.xxx」です。
        /// </summary>
        public string SubnetMask
        {
            get { return _subnetMask; }
            set
            {
                if (!NetworkUtilities.IsValidAddress(value))
                    throw new ArgumentException("不正な書式のサブネットマスクです。");

                _subnetMask = value;
            }
        }

        /// <summary>
        /// デフォルトゲートウェイを取得または設定します。
        /// 書式は「xxx.xxx.xxx.xxx」です。
        /// </summary>
        public string DefaultGateway
        {
            get { return _defaultGateway; }
            set
            {
                if (!NetworkUtilities.IsValidAddress(value))
                    throw new ArgumentException("不正な書式のデフォルトゲートウェイです。");

                _defaultGateway = value;
            }
        }

        /// <summary>
        /// DHCPを利用したDNSサーバのアドレス自動取得を行うかどうかを取得または設定します。
        /// </summary>
        public bool UseDnsDhcp { get; set; }

        /// <summary>
        /// 優先DNSサーバのアドレスを取得または設定します。
        /// 書式は「xxx.xxx.xxx.xxx」です。
        /// </summary>
        public string PrimaryDns
        {
            get { return _primaryDns; }
            set
            {
                if (!NetworkUtilities.IsValidAddress(value))
                    throw new ArgumentException("不正な書式の優先DNSサーバアドレスです。");

                _primaryDns = value;
            }
        }

        /// <summary>
        /// 代替DNSサーバのアドレスを取得または設定します。
        /// 書式は「xxx.xxx.xxx.xxx」です。
        /// </summary>
        public string SecondaryDns
        {
            get { return _secondaryDns; }
            set
            {
                if (!NetworkUtilities.IsValidAddress(value))
                    throw new ArgumentException("不正な書式の代替DNSサーバアドレスです。");

                _secondaryDns = value;
            }
        }

        #endregion

        #region コンストラクタ

        /// <summary>
        /// 唯一のコンストラクタです。
        /// publicコンストラクタを隠蔽し、アセンブリ内でのみインスタンスを生成可能にします。
        /// </summary>
        internal AdapterInfo() { }

        #endregion

        #region GetIPBlock - IPアドレスの指定バイトの文字列取得

        /// <summary>
        /// IPアドレスの各バイトの内、指定した箇所のものを文字列表現で取得します。
        /// </summary>
        /// <param name="block">取得するバイトの位置(0～3)</param>
        /// <returns>指定箇所のバイトの文字列表現</returns>
        public string GetIPBlock(int block)
        {
            return IpAddress.Split(new[] { '.' })[block];
        }

        #endregion

        #region GetSubnetBlock - サブネットマスクの指定バイトの文字列取得

        /// <summary>
        /// サブネットマスクの各バイトの内、指定した箇所のものを文字列表現で取得します。
        /// </summary>
        /// <param name="block">取得するバイトの位置(0～3)</param>
        /// <returns>指定箇所のバイトの文字列表現</returns>
        public string GetSubnetBlock(int block)
        {
            return SubnetMask.Split(new[] { '.' })[block];
        }

        #endregion

        #region GetGatewayBlock - デフォルトゲートウェイの指定バイトの文字列取得

        /// <summary>
        /// デフォルトゲートウェイの各バイトの内、指定した箇所のものを文字列表現で取得します。
        /// </summary>
        /// <param name="block">取得するバイトの位置(0～3)</param>
        /// <returns>指定箇所のバイトの文字列表現</returns>
        public string GetGatewayBlock(int block)
        {
            return DefaultGateway.Split(new[] { '.' })[block];
        }

        #endregion

        #region GetPrimaryDnsBlock - 優先DNSサーバの指定バイトの文字列取得

        /// <summary>
        /// 優先DNSサーバの各バイトの内、指定した箇所のものを文字列表現で取得します。
        /// </summary>
        /// <param name="block">取得するバイトの位置(0～3)</param>
        /// <returns>指定箇所のバイトの文字列表現</returns>
        public string GetPrimaryDnsBlock(int block)
        {
            return PrimaryDns.Split(new[] { '.' })[block];
        }

        #endregion

        #region GetSecondaryDnsBlock - 代替DNSサーバの指定バイトの文字列取得

        /// <summary>
        /// 代替DNSサーバの各バイトの内、指定した箇所のものを文字列表現で取得します。
        /// </summary>
        /// <param name="block">取得するバイトの位置(0～3)</param>
        /// <returns>指定箇所のバイトの文字列表現</returns>
        public string GetSecondaryDnsBlock(int block)
        {
            return SecondaryDns.Split(new[] { '.' })[block];
        }

        #endregion
    }
}
