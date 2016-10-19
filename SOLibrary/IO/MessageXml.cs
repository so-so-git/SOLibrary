using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml.Linq;

namespace SO.Library.IO
{
    /// <summary>
    /// メッセージ定義XML管理クラス
    /// </summary>
    public static class MessageXml
    {
        #region プロパティ

        /// <summary>
        /// メッセージXMLファイルパスを取得または設定します。
        /// </summary>
        public static string MessageFilePath { get; set; }

        /// <summary>
        /// メッセージタグ名を取得または設定します。
        /// </summary>
        public static string TagName { get; set; }

        /// <summary>
        /// ID属性名を取得または設定します。
        /// </summary>
        public static string IdAttribute { get; set; }

        /// <summary>
        /// ダイアログキャプション属性名を取得または設定します。
        /// </summary>
        public static string CaptionAttribute { get; set; }

        /// <summary>
        /// ボタン指定属性名を取得または設定します。
        /// </summary>
        public static string ButtonsAttribute { get; set; }

        /// <summary>
        /// アイコン指定属性名を取得または設定します。
        /// </summary>
        public static string IconAttribute { get; set; }

        #endregion

        #region 静的コンストラクタ

        /// <summary>
        /// 静的コンストラクタです。
        /// </summary>
        static MessageXml()
        {
            TagName = "msg";
            IdAttribute = "id";
            CaptionAttribute = "caption";
            ButtonsAttribute = "buttons";
            IconAttribute = "icon";
        }

        #endregion

        #region GetMessageInfo - アプリケーションメッセージ情報取得

        /// <summary>
        /// 指定されたIDのアプリケーションメッセージ情報を取得します。
        /// </summary>
        /// <param name="messageId">メッセージID</param>
        /// <param name="args">置換文字列</param>
        /// <returns>アプリケーションメッセージ情報</returns>
        public static MessageInfo GetMessageInfo(string messageId, params object[] args)
        {
            if (string.IsNullOrEmpty(MessageFilePath))
            {
                throw new InvalidOperationException(
                    "メッセージXMLファイルのパスが指定されていません。");
            }

            MessageInfo msgInfo;
            msgInfo.id = messageId;

            // 指定idのmsgノード取得クエリ
            var elm = (from e in XDocument.Load(MessageFilePath).Descendants(TagName)
                       where e.Attribute(IdAttribute).Value == messageId
                       select new
                       {
                           Message = e.Value,
                           Caption = e.Attribute(CaptionAttribute).Value,
                           Buttons = e.Attribute(ButtonsAttribute).Value,
                           Icon = e.Attribute(IconAttribute).Value
                       }).Single();

            // エスケープシーケンスが含まれるか検査し、あるならばエスケープ文字をアンエスケープする
            string msg;
            if (elm.Message.IndexOf("\\") > -1)
            {
                msg = Regex.Unescape(elm.Message.Trim());
            }
            else
            {
                msg = elm.Message.Trim();
            }

            // 第二引数以降の引数の数とメッセージ中のプレースホルダの数を比較
            MatchCollection phs = Regex.Matches(msg, "[{][0-9]*[}]");

            var distinct = new List<string>();
            foreach (Match ph in phs)
            {
                if (distinct.Count == 0 || !distinct.Contains(ph.Value))
                {
                    distinct.Add(ph.Value);
                }
            }

            if (distinct.Count != args.Length)
            {
                throw new ArgumentException("メッセージ中の置換対象と引数の数が一致しません。");
            }

            msgInfo.message = string.Format(msg, args);

            msgInfo.caption = elm.Caption;
            msgInfo.buttons = (MessageBoxButtons)
                typeof(MessageBoxButtons).GetField(elm.Buttons).GetValue(null);
            msgInfo.icon = (MessageBoxIcon)
                typeof(MessageBoxIcon).GetField(elm.Icon).GetValue(null);

            return msgInfo;
        }

        #endregion

        #region ShowMessageById - メッセージIDを指定しアプリケーションメッセージ表示

        /// <summary>
        /// メッセージIDを指定し、アプリケーションメッセージダイアログを表示します。
        /// </summary>
        /// <param name="messageId">メッセージID</param>
        /// <param name="messageParams">置換文字列</param>
        /// <returns>ユーザ応答</returns>
        public static DialogResult ShowMessageById(string messageId, params string[] messageParams)
        {
            MessageInfo msgInfo = MessageXml.GetMessageInfo(messageId, messageParams);

            return MessageBox.Show(msgInfo.message, msgInfo.caption, msgInfo.buttons, msgInfo.icon);
        }

        /// <summary>
        /// メッセージID、ボタン構成を指定してアプリケーションメッセージダイアログを表示します。
        /// </summary>
        /// <param name="messageId">メッセージID</param>
        /// <param name="buttons">ボタン構成</param>
        /// <param name="messageParams">置換文字列</param>
        /// <returns>ユーザ応答</returns>
        public static DialogResult ShowMessageById(string messageId, MessageBoxButtons buttons,
                                               params string[] messageParams)
        {
            MessageInfo msgInfo = MessageXml.GetMessageInfo(messageId, messageParams);

            return MessageBox.Show(msgInfo.message, msgInfo.caption, buttons, msgInfo.icon);
        }

        /// <summary>
        /// メッセージID、ボタン構成、アイコン種別を指定してアプリケーションメッセージダイアログを表示します。
        /// </summary>
        /// <param name="messageId">メッセージID</param>
        /// <param name="buttons">ボタン構成</param>
        /// <param name="icon">アイコン種別</param>
        /// <param name="messageParams">置換文字列</param>
        /// <returns>ユーザ応答</returns>
        public static DialogResult ShowMessageById(string messageId, MessageBoxButtons buttons,
                                               MessageBoxIcon icon, params string[] messageParams)
        {
            MessageInfo msgInfo = MessageXml.GetMessageInfo(messageId, messageParams);

            return MessageBox.Show(msgInfo.message, msgInfo.caption, buttons, icon);
        }

        #endregion
    }

    #region struct MessageInfo - アプリケーションメッセージ情報構造体

    /// <summary>
    /// アプリケーションメッセージ情報構造体
    /// </summary>
    public struct MessageInfo
    {
        /// <summary>メッセージID</summary>
        public string id;

        /// <summary>メッセージ本文</summary>
        public string message;

        /// <summary>メッセージダイアログタイトル</summary>
        public string caption;

        /// <summary>ダイアログボタン構成</summary>
        public MessageBoxButtons buttons;

        /// <summary>ダイアログアイコン種別</summary>
        public MessageBoxIcon icon;
    }

    #endregion
}
