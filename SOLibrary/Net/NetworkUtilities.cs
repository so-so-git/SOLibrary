using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;

using SO.Library.Extensions;

namespace SO.Library.Net
{
    /// <summary>
    /// ネットワーク関連のユーティリティクラス
    /// </summary>
    public static class NetworkUtilities
    {
        #region クラス定数

        /// <summary>netshコマンド</summary>
        private const string NETSH_CMD = "netsh";

        /// <summary>netshコマンドの規定のタイムアウト時間(ミリ秒)</summary>
        private const int DEFAULT_CMD_TIMEOUT = 30000;

        #endregion

        #region プロパティ

        /// <summary>
        /// netshコマンドのタイムアウト時間(ミリ秒)を取得または設定します。
        /// 規定値は30000ミリ秒です。
        /// </summary>
        public static int ProcessTimeout { get; set; }

        #endregion

        #region 静的コンストラクタ

        /// <summary>
        /// 静的コンストラクタです。
        /// </summary>
        static NetworkUtilities()
        {
            ProcessTimeout = DEFAULT_CMD_TIMEOUT;
        }

        #endregion

        #region IsValidAddress - アドレス書式チェック

        /// <summary>
        /// 指定されたアドレスが、ネットワークアドレスとして
        /// 正しい書式(xxx.xxx.xxx.xxx)かをチェックします。
        /// </summary>
        /// <param name="address">チェック対象のアドレス文字列</param>
        /// <returns>true:正しい書式 / false:不正な書式</returns>
        public static bool IsValidAddress(string address)
        {
            //  0～255の正規表現を定義
            // (但し、2桁以上の場合に先頭1文字ないし2文字が0のものはNG)
            const string BLOCK_PATTERN = "([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])";

            return Regex.IsMatch(address, string.Format("^{0}\\.{0}\\.{0}\\.{0}$", BLOCK_PATTERN));
        }

        #endregion

        #region GetAdapterInfo - ネットワークアダプタ情報取得

        /// <summary>
        /// 指定された名前のネットワークアダプタ情報を取得します。
        /// 一致する名前のネットワークアダプタが存在しない場合はnullが返されます。
        /// </summary>
        /// <param name="name">取得するネットワークアダプタ名</param>
        /// <returns>ネットワークアダプタ情報</returns>
        public static AdapterInfo GetAdapterInfo(string name)
        {
            foreach (var netInfo in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (netInfo.Name == name)
                {
                    return CreateAdapterInfo(netInfo);
                }
            }

            return null;
        }

        #endregion

        #region GetAdapterInfos - 複数のネットワークアダプタ情報取得

        /// <summary>
        /// イーサネットタイプの全てのネットワークアダプタ情報を取得します。
        /// </summary>
        /// <returns>ネットワークアダプタ情報</returns>
        public static IEnumerable<AdapterInfo> GetAdapterInfos()
        {
            // イーサネットタイプのみ
            return GetAdapterInfos(NetworkInterfaceType.Ethernet);
        }

        /// <summary>
        /// 指定したネットワークインタフェースタイプの全てのネットワークアダプタ情報を取得します。
        /// </summary>
        /// <param name="type">ネットワークインタフェースタイプ</param>
        /// <returns>ネットワークアダプタ情報</returns>
        public static IEnumerable<AdapterInfo> GetAdapterInfos(NetworkInterfaceType type)
        {
            foreach (var netInfo in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (netInfo.NetworkInterfaceType != type) continue;

                yield return CreateAdapterInfo(netInfo);
            }
        }

        #endregion

        #region CreateAdapterInfo - ネットワークアダプタ情報生成

        /// <summary>
        /// 指定されたNetworkInterfaceを基にネットワークアダプタ情報を生成します。
        /// </summary>
        /// <param name="netInfo">ネットワークインタフェース情報</param>
        /// <returns>ネットワークアダプタ情報</returns>
        private static AdapterInfo CreateAdapterInfo(NetworkInterface netInfo)
        {
            var adapterInfo = new AdapterInfo();
            adapterInfo.Name = netInfo.Name; // アダプタ名

            IPInterfaceProperties ipProps = netInfo.GetIPProperties();
            if (ipProps == null)
            {
                // IP自動取得DHCP
                adapterInfo.UseIpDhcp = ipProps.DhcpServerAddresses.Count > 0;

                // IP、サブネットマスク
                if (ipProps.UnicastAddresses.Count > 0)
                {
                    UnicastIPAddressInformation ipInfo = ipProps.UnicastAddresses[0];
                    adapterInfo.IpAddress = ipInfo.Address.ConvertAddressBytes();
                    adapterInfo.SubnetMask = ipInfo.IPv4Mask.ConvertAddressBytes();
                }
                else
                {
                    adapterInfo.IpAddress = string.Empty;
                    adapterInfo.SubnetMask = string.Empty;
                }

                // デフォルトゲートウェイ
                adapterInfo.DefaultGateway = ipProps.GatewayAddresses.Count > 0
                    ? ipProps.GatewayAddresses[0].Address.ConvertAddressBytes()
                    : string.Empty;

                // DNSサーバ
                if (ipProps.DnsAddresses.Count > 0)
                {
                    adapterInfo.PrimaryDns = ipProps.DnsAddresses[0].ConvertAddressBytes();

                    adapterInfo.SecondaryDns = ipProps.DnsAddresses.Count > 1
                        ? ipProps.DnsAddresses[1].ConvertAddressBytes()
                        : string.Empty;
                }
                else
                {
                    adapterInfo.PrimaryDns = string.Empty;
                    adapterInfo.SecondaryDns = string.Empty;
                }
            }

            return adapterInfo;
        }

        #endregion

        #region === netshコマンド関連 ===

        #region DoNetSh - netshコマンド実行

        /// <summary>
        /// netshコマンドを別プロセスで実行します。
        /// 設定内容に関してはargumentsで指定します。
        /// </summary>
        /// <param name="arguments">netshコマンドの引数</param>
        /// <returns>netshコマンド実行結果(SO.Library.Net.NetshExitCodesの何れかの値)</returns>
        private static int DoNetSh(string arguments)
        {
            var proc = new Process();
            proc.StartInfo.FileName = NETSH_CMD;
            proc.StartInfo.Arguments = arguments;

            if (!proc.StartWithHiding())
            {
                // プロセス開始エラー
                return NetshExitCodes.PROCESS_START_ERROR;
            }

            if (!proc.WaitForExit(ProcessTimeout))
            {
                // プロセスタイムアウト
                return NetshExitCodes.PROCESS_TIMEOUT;
            }

            return proc.ExitCode == NetshExitCodes.SUCCEEDED ?
                NetshExitCodes.SUCCEEDED : NetshExitCodes.COMMAND_ERROR;
        }

        #endregion

        #region SetAddresses - IPアドレス関連情報設定

        /// <summary>
        /// ネットワークアダプタ情報に基づき、
        /// DHCP使用有無、IPアドレス、サブネットマスク、デフォルトゲートウェイを設定します。
        /// </summary>
        /// <param name="adapter">ネットワークアダプタ情報</param>
        /// <returns>netshコマンド実行結果(SO.Library.Net.NetshExitCodesの何れかの値)</returns>
        public static int SetAddresses(AdapterInfo adapter)
        {
            var arg = new StringBuilder("interface ip set address");
            arg.AppendFormat(" name=\"{0}\"", adapter.Name);
            if (adapter.UseIpDhcp)
            {
                // DHCPを使用する
                arg.Append(" source=dhcp");
            }
            else
            {
                // DHCPを使用しない
                arg.Append(" source=static");
                arg.AppendFormat(" addr={0}", adapter.IpAddress);
                arg.AppendFormat(" mask={0}", adapter.SubnetMask);
                arg.AppendFormat(" gateway={0}", string.IsNullOrEmpty(adapter.DefaultGateway)
                    ? "none" : adapter.DefaultGateway);
            }

            return DoNetSh(arg.ToString());
        }

        #endregion

        #region SetPrimaryDns - 優先DNSサーバ情報設定

        /// <summary>
        /// ネットワークアダプタ情報に基づき、
        /// DHCP使用有無、優先DNSサーバ情報を設定します。
        /// 既に優先DNSサーバが設定済みの場合は上書きされます。
        /// </summary>
        /// <param name="adapter">ネットワークアダプタ情報</param>
        /// <returns>netshコマンド実行結果(SO.Library.Net.NetshExitCodesの何れかの値)</returns>
        public static int SetPrimaryDns(AdapterInfo adapter)
        {
            var arg = new StringBuilder("interface ip set dns");
            arg.AppendFormat(" name=\"{0}\"", adapter.Name);
            if (adapter.UseIpDhcp)
            {
                // DHCPを使用する
                arg.Append(" source=dhcp");
            }
            else
            {
                // DHCPを使用しない
                arg.Append(" source=static");
                arg.AppendFormat(" addr={0}", string.IsNullOrEmpty(adapter.PrimaryDns)
                    ? "none" : adapter.PrimaryDns);
            }

            return DoNetSh(arg.ToString());
        }

        #endregion

        #region SetSecondaryDns - 代替DNSサーバ情報設定

        /// <summary>
        /// ネットワークアダプタ情報に基づき、代替DNSサーバ情報を設定します。
        /// 既に代替DNSサーバが設定済みの場合は上書きされます。
        /// </summary>
        /// <param name="adapter">ネットワークアダプタ情報</param>
        /// <returns>netshコマンド実行結果(SO.Library.Net.NetshExitCodesの何れかの値)</returns>
        public static int SetSecondaryDns(AdapterInfo adapter)
        {
            if (adapter.UseDnsDhcp || string.IsNullOrEmpty(adapter.SecondaryDns))
            {
                return NetshExitCodes.NOT_EXECUTE;
            }

            var arg = new StringBuilder("interface ip add dns");
            arg.AppendFormat(" name=\"{0}\"", adapter.Name);
            arg.AppendFormat(" addr={0}", adapter.SecondaryDns);
            arg.Append(" index=2");

            return DoNetSh(arg.ToString());
        }

        #endregion

        #region DeleteDns - DNSサーバ情報削除

        /// <summary>
        /// ネットワークアダプタ内の指定されたDNSサーバ設定を削除します。
        /// </summary>
        /// <param name="adapter">ネットワークアダプタ情報</param>
        /// <param name="address">削除するDNSサーバのIPアドレス(書式は「xxx.xxx.xxx.xxx」です)</param>
        /// <returns>netshコマンド実行結果(SO.Library.Net.NetshExitCodesの何れかの値)</returns>
        public static int DeleteDns(AdapterInfo adapter, string address)
        {
            if (!NetworkUtilities.IsValidAddress(address))
            {
                throw new ArgumentException("不正な書式のDNSサーバアドレスです。");
            }

            var arg = new StringBuilder("interface ip delete dns");
            arg.AppendFormat(" name=\"{0}\"", adapter.Name);
            arg.AppendFormat(" addr={0}", address);

            return DoNetSh(arg.ToString());
        }

        #endregion

        #region DeleteAllDns - 全DNSサーバ情報削除

        /// <summary>
        /// ネットワークアダプタの全てのDNSサーバ設定を削除します。
        /// </summary>
        /// <param name="adapter">ネットワークアダプタ情報</param>
        /// <returns>netshコマンド実行結果(SO.Library.Net.NetshExitCodesの何れかの値)</returns>
        public static int DeleteAllDns(AdapterInfo adapter)
        {
            var arg = new StringBuilder("interface ip delete dns");
            arg.AppendFormat(" name=\"{0}\"", adapter.Name);
            arg.Append(" all");

            return DoNetSh(arg.ToString());
        }

        #endregion

        #endregion
    }

    #region class NetshExitCodes - netshコマンド戻り値定義クラス

    /// <summary>
    /// netshコマンド戻り値定義クラス
    /// </summary>
    public struct NetshExitCodes
    {
        /// <summary>コマンド正常終了</summary>
        public const int SUCCEEDED = 0;

        /// <summary>コマンド異常終了</summary>
        public const int COMMAND_ERROR = 1;

        /// <summary>コマンドを実行せず終了</summary>
        public const int NOT_EXECUTE = -100;

        /// <summary>プロセス開始失敗</summary>
        public const int PROCESS_START_ERROR = -200;

        /// <summary>プロセスタイムアウト</summary>
        public const int PROCESS_TIMEOUT = -300;
    }

    #endregion
}
