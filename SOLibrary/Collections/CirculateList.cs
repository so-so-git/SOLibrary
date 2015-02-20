using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SO.Library.Collections
{
    /// <summary>
    /// 循環リストクラス
    /// </summary>
    /// <typeparam name="T">格納する要素の型</typeparam>
    public class CirculateList<T> : IList<T>
    {
        #region インスタンスフィールド

        /// <summary>内部リスト</summary>
        private List<T> _internalList;

        #endregion

        #region プロパティ

        /// <summary>現在位置のインデックスを取得・または設定します。</summary>
        public int CurrentPosition { get; set; }

        #endregion

        #region コンストラクタ

        /// <summary>
        /// 空の内部リストを持ったインスタンスを作成するコンストラクタです。
        /// </summary>
        public CirculateList()
        {
            _internalList = new List<T>();
            CurrentPosition = 0;
        }

        /// <summary>
        /// 指定された初期容量の内部リストを持ったインスタンスを作成するコンストラクタです。
        /// </summary>
        /// <param name="capacity">内部リストの初期容量</param>
        public CirculateList(int capacity)
        {
            _internalList = new List<T>(capacity);
            CurrentPosition = 0;
        }

        /// <summary>
        /// 渡されたコレクションを内部リストにコピーしてインスタンスを作成するコンストラクタです。
        /// </summary>
        /// <param name="collection">コピー元のコレクション</param>
        public CirculateList(IEnumerable<T> collection)
        {
            _internalList = new List<T>(collection);
            CurrentPosition = 0;
        }

        #endregion

        #region TakeSome - 指定した数の要素を取得
        /// <summary>
        /// 現在の位置のインデックスから指定した数の要素を取得します。
        /// </summary>
        /// <param name="length">取得する要素の数</param>
        /// <returns>取得した要素</returns>
        public IEnumerable<T> TakeSome(int length)
        {
            for (int i = CurrentPosition, j = 0; j < length; ++i, ++j)
            {
                yield return _internalList[ToCirculateIndex(i)];
            }
        }
        #endregion

        #region TakeOneAround - 一周分の要素を取得
        /// <summary>
        /// 現在の位置のインデックスから、一周分の要素を取得します。
        /// 要素の順序は、現在の位置のインデックスのものが最初になります。
        /// </summary>
        /// <returns>取得した要素</returns>
        public IEnumerable<T> TakeOneAround()
        {
            return TakeSome(_internalList.Count);
        }
        #endregion

        #region ToCirculateIndex - 循環を考慮したインデックスに変換
        /// <summary>
        /// 渡されたインデックスを、循環を考慮したインデックスに変換します。
        /// </summary>
        /// <param name="index">元のインデックス</param>
        /// <returns>循環を考慮したインデックス</returns>
        public int ToCirculateIndex(int index)
        {
            if (_internalList.Count == 0)
                return 0;

            if (index >= _internalList.Count)
                return index % _internalList.Count;

            if (index < 0)
                return _internalList.Count - index % _internalList.Count;

            return index;
        }
        #endregion

        #region IList<T>実装
#pragma warning disable

        public int IndexOf(T item)
        {
            return _internalList.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _internalList.Insert(ToCirculateIndex(index), item);
        }

        public void RemoveAt(int index)
        {
            _internalList.RemoveAt(ToCirculateIndex(index));
        }

        public T this[int index]
        {
            get { return _internalList[ToCirculateIndex(index)]; }
            set { _internalList[ToCirculateIndex(index)] = value; }
        }

        public void Add(T item)
        {
            _internalList.Add(item);
        }

        public void Clear()
        {
            _internalList.Clear();
        }

        public bool Contains(T item)
        {
            return _internalList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _internalList.CopyTo(array, ToCirculateIndex(arrayIndex));
        }

        public int Count
        {
            get { return _internalList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            return _internalList.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _internalList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _internalList.GetEnumerator();
        }

#pragma warning restore
        #endregion
    }
}
