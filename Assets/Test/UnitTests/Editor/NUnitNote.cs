using UnityEngine;
using System;
using System.Collections;
using NUnit.Framework;

namespace UnityTest {
    // 表示這個類別包含測試，此類別必須是 public 的，否則 NUnit 會無法識別
    [TestFixture]
    public class NUnitNote {
        // 每個 Test Case 的前置動作，在 Test Case 執行前都會先執行這 function
        [SetUp]
        [Ignore]
        public void setUp() {
            Debug.Log("Setup");
        }

        // 每個 Test Case 結束後會執行這個 function，來恢復為原本的設定值
        [TearDown]
        [Ignore]
        public void tearDown() {
            Debug.Log("tearDown");
        }

        // 之前有介紹過 [SetUp]/[TearDown]，分別為每個 Test Case 的前置動作，
        // 與每個 Test Case 的結束動作，TestFixtureSetUp/TestFixtureTearDown 的功能也是相似的，
        // 只是不是以每個 Test Case　為單位，而是以每個 TestFixture 為單位。
        // EX：如果要測試資料庫的相關操作，寫在 setUp 的話，每個 Test Case 都必須做一次連線的動作，
        // 這樣下來會耗費相當多的資源，也可能會增加測試時間，因此，如果寫在 TestFixtureSetUp 裡，
        // 只要連線一次，之後的 Test Case 就可以直接使用，不必重覆再作連線資料庫的動作

        // TestFixtureSetUp/TestFixtureTearDown 只會作用在整個 TestFixture 起始跟結尾，
        // 而 setUp/tearDown 則是作用在每個 Test Case 的起始跟結尾
        [TestFixtureSetUp]
        public void testFixtureSetUp() {
            Debug.Log("testFixtureSetUp");
            Assert.AreEqual(1, 1);
        }

        [TestFixtureTearDown]
        public void testFixtureTearDown() {
            Debug.Log("testFixtureTearDown");
            Assert.AreEqual(1, 1);
        }

        // 表示這個 function 為一個 Test Case，作為 Test Case 這個 function 不能有參數與回傳值
        [Test]
        [Ignore]
        public void ExceptionTest() {
            throw new Exception("Exception throwing test");
        }

        // 有時候可能此 testcase 尚未完成或在重構，就可以使用這個屬性讓此測試先不執行
        [Ignore]
        [Test]
        public void IgnoreTest() {
            Debug.Log("IgnoreTest");
            Assert.AreEqual(1, 1);
        }

        // 在 Test Case 或 TestFixture 使用這個屬性，在執行全部 Test Case 的時候會去忽略掉
        // 這個 Test Case 或 TestFixture，但如果單獨去選擇這個 Test Case 或 TestFixture，則會執行。
        // 那跟上篇介紹的[Ignore] 屬性差別在哪呢?
        // 如果使用[Ignore] 屬性，這個 Test Case 會整個被忽略掉，
        // 即使去指定要執行這個 Test Case 也是不會被執行的，只有將[Ignore] 屬性拿掉才能執行此 Test Case
        [Explicit]
        [Test]
        [Ignore]
        public void ExplicitTest() {
            Debug.Log("ExplicitTest");
            Assert.AreEqual(1, 1);
        }

        // 預期這個 Test Case 會丟出 Exception，如果沒有得到預期的 Exception 這個 Test Case 就會 Fail。
        // EX：像是如果用任何數除以零，都會得到 DivideByZeroException，
        // 所以我們就可以在 Test Case 前面設定 ExpectedException
        [Test]
        [ExpectedException("System.DivideByZeroException")]
        [Ignore]
        public void testException() {
            int zero = 0;
            int number = 2 / zero;
        }
    }
}
