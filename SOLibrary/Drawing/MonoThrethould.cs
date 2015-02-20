namespace SO.Library.Drawing
{
    /// <summary>
    /// モノクローム変換時の白黒の閾値クラス
    /// </summary>
    public sealed class MonoThrethould
    {
        /// <summary>白黒閾値</summary>
        private int _value;

        /// <summary>極めて低めの閾値を取得します。</summary>
        public static MonoThrethould VeryLow { get { return new MonoThrethould(42); } }
        /// <summary>低めの閾値を取得します。</summary>
        public static MonoThrethould Low { get { return new MonoThrethould(84); } }
        /// <summary>中間の閾値を取得します。</summary>
        public static MonoThrethould Medium { get { return new MonoThrethould(126); } }
        /// <summary>高めの閾値を取得します。</summary>
        public static MonoThrethould High { get { return new MonoThrethould(170); } }
        /// <summary>極めて高めの閾値を取得します。</summary>
        public static MonoThrethould VeryHigh { get { return new MonoThrethould(212); } }

        /// <summary>
        /// 白黒の閾値を取得します。
        /// </summary>
        public int Value { get { return _value; } }

        /// <summary>
        /// 唯一のコンストラクタです。
        /// </summary>
        /// <param name="value"></param>
        public MonoThrethould(int value)
        {
            _value = value;
        }
    }
}
