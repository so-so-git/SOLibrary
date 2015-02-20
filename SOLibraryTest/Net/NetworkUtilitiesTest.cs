using NUnit.Framework;

using SO.Library.Net;

namespace SO.LibraryTest.Net
{
    [TestFixture]   
    public class NetworkUtilitiesTest
    {
        [Test]
        public void IsValidAddress()
        {
            Assert.AreEqual(true, NetworkUtilities.IsValidAddress("0.0.0.0"));
            Assert.AreEqual(true, NetworkUtilities.IsValidAddress("9.9.9.9"));
            Assert.AreEqual(true, NetworkUtilities.IsValidAddress("10.10.10.10"));
            Assert.AreEqual(true, NetworkUtilities.IsValidAddress("99.99.99.99"));
            Assert.AreEqual(true, NetworkUtilities.IsValidAddress("100.100.100.100"));
            Assert.AreEqual(true, NetworkUtilities.IsValidAddress("199.199.199.199"));
            Assert.AreEqual(true, NetworkUtilities.IsValidAddress("200.200.200.200"));
            Assert.AreEqual(true, NetworkUtilities.IsValidAddress("255.255.255.255"));

            Assert.AreEqual(false, NetworkUtilities.IsValidAddress("00.0.0.0"));
            Assert.AreEqual(false, NetworkUtilities.IsValidAddress("01.0.0.0"));
            Assert.AreEqual(false, NetworkUtilities.IsValidAddress("000.0.0.0"));
            Assert.AreEqual(false, NetworkUtilities.IsValidAddress("001.0.0.0"));
            Assert.AreEqual(false, NetworkUtilities.IsValidAddress("256.0.0.0"));

            Assert.AreEqual(false, NetworkUtilities.IsValidAddress("a.0.0.0"));
            Assert.AreEqual(false, NetworkUtilities.IsValidAddress("a1.0.0.0"));

            Assert.AreEqual(false, NetworkUtilities.IsValidAddress("0.0.0"));
            Assert.AreEqual(false, NetworkUtilities.IsValidAddress("0.0.0.0.0"));

            Assert.AreEqual(false, NetworkUtilities.IsValidAddress("0.a.0.0"));
            Assert.AreEqual(false, NetworkUtilities.IsValidAddress("0.0.a.0"));
            Assert.AreEqual(false, NetworkUtilities.IsValidAddress("0.0.0.a"));
        }
    }
}
