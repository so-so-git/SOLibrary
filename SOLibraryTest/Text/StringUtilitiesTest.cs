using NUnit.Framework;

using SO.Library.Text;

namespace SO.LibraryTest.Text
{
    [TestFixture]
    public class StringUtilitiesTest
    {
        #region インスタンス変数

        private const string SQL_UNESCAPED = "' OR '1' = '1";
        private const string SQL_ESCAPED = "'' OR ''1'' = ''1";

        private const string HTML_UNESCAPED = "A & B <script>alert('test' + \" proc.\")</script>";
        private const string HTML_ESCAPED = "A &amp; B &lt;script&gt;alert(&#039;test&#039; + &quot; proc.&quot;)&lt;/script&gt;";

        private const string CSV_UNESCAPED = "\"Test\",\"Proc\"";
        private const string CSV_ESCAPED = "\"\"Test\"\",\"\"Proc\"\"";

        #endregion

        #region 文字列エスケープ系処理

        [Test]
        public void EscapeSqlText()
        {
            Assert.AreEqual(SQL_ESCAPED,
                StringUtilities.EscapeSqlText(SQL_UNESCAPED));
        }

        [Test]
        public void UnescapeSqlText()
        {
            Assert.AreEqual(SQL_UNESCAPED,
                StringUtilities.UnescapeSqlText(SQL_ESCAPED));
        }

        [Test]
        public void EscapeHtmlText()
        {
            Assert.AreEqual(HTML_ESCAPED,
                StringUtilities.EscapeHtmlText(HTML_UNESCAPED));
        }

        [Test]
        public void UnescapeHtmlText()
        {
            Assert.AreEqual(HTML_UNESCAPED,
                StringUtilities.UnescapeHtmlText(HTML_ESCAPED));
        }

        [Test]
        public void EscapeCsvText()
        {
            Assert.AreEqual(CSV_ESCAPED,
                StringUtilities.EscapeCsvText(CSV_UNESCAPED));
        }

        [Test]
        public void UnescapeCsvText()
        {
            Assert.AreEqual(CSV_UNESCAPED,
                StringUtilities.UnescapeCsvText(CSV_ESCAPED));
        }

        #endregion

        [Test]
        public void PaddingByZero()
        {
            Assert.AreEqual("00123", StringUtilities.PaddingByZero(123, 5), "int, int");
            Assert.AreEqual("00123", StringUtilities.PaddingByZero("123", 5), "string, int");
            Assert.AreEqual("00123", StringUtilities.PaddingByZero(123, 5, PaddingOption.Before), "int, int, PaddingOption.Before");
            Assert.AreEqual("12300", StringUtilities.PaddingByZero(123, 5, PaddingOption.After), "int, int, PaddingOption.After");
            Assert.AreEqual("00123", StringUtilities.PaddingByZero("123", 5, PaddingOption.Before), "string, int, PaddingOption.Before");
            Assert.AreEqual("12300", StringUtilities.PaddingByZero("123", 5, PaddingOption.After), "string, int, PaddingOption.After");
        }

        [Test]
        public void PaddingBySpace()
        {
            Assert.AreEqual("123  ", StringUtilities.PaddingBySpace(123, 5), "int, int");
            Assert.AreEqual("123  ", StringUtilities.PaddingBySpace("123", 5), "string, int");
            Assert.AreEqual("  123", StringUtilities.PaddingBySpace(123, 5, PaddingOption.Before), "int, int, PaddingOption.Before");
            Assert.AreEqual("123  ", StringUtilities.PaddingBySpace(123, 5, PaddingOption.After), "int, int, PaddingOption.After");
            Assert.AreEqual("  123", StringUtilities.PaddingBySpace("123", 5, PaddingOption.Before), "string, int, PaddingOption.Before");
            Assert.AreEqual("123  ", StringUtilities.PaddingBySpace("123", 5, PaddingOption.After), "string, int, PaddingOption.After");
        }

        [Test]
        public void GetByteCount()
        {
            Assert.AreEqual(15, StringUtilities.GetByteCount("あいうえおｱｲｳｴｵ"));
        }

        [Test]
        public void IsNarrow()
        {
            Assert.AreEqual(false, StringUtilities.IsNarrow("アイウエオａｂｃＡＢＣ！？"), "All Wide");
            Assert.AreEqual(true, StringUtilities.IsNarrow("ｱｲｳｴｵabcABC!?"), "All Narrow");
            Assert.AreEqual(false, StringUtilities.IsNarrow("アｲウｴオaｂcＡBＣ！?"), "Wide and Narrow");
        }

        [Test]
        public void IsWide()
        {
            Assert.AreEqual(true, StringUtilities.IsWide("アイウエオａｂｃＡＢＣ！？"), "All Wide");
            Assert.AreEqual(false, StringUtilities.IsWide("ｱｲｳｴｵabcABC!?"), "All Narrow");
            Assert.AreEqual(false, StringUtilities.IsWide("アｲウｴオaｂcＡBＣ！?"), "Wide and Narrow");
        }
    }
}
