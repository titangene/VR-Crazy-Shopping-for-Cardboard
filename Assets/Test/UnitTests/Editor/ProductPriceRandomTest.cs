using UnityEngine;
using System;
using System.Collections;
using NUnit.Framework;

namespace UnityTest {
    [TestFixture]
    public class ProductPriceRandomTest : MonoBehaviour {
        /* --------------------------
        ProductPriceRandom productPriceRandom;
        bool IsPass = false;

        // 每個 Test Case 的前置動作，在 Test Case 執行前都會先執行這 function
        [SetUp]
        public void setUp() {
            productPriceRandom = new ProductPriceRandom();
            productPriceRandom.CreateArray();
        }

        // 每個 Test Case 結束後會執行這個 function，來恢復為原本的設定值
        [TearDown]
        public void tearDown() {
            productPriceRandom.ClearArray();
            IsPass = false;
        }

        /// <summary>
        /// 隨機產生範圍區間的商品價格
        /// </summary>
        public void 隨機產生商品價格(ushort min, ushort max, ushort count) {
            productPriceRandom.RandomCount(min, max, count);
            ArrayList randomArray = productPriceRandom.GetArray_Temp();

            string str = "";

            for (int i = 0; i < randomArray.Count; i++) {
                for (int j = i + 1; j < randomArray.Count; j++) {
                    if (randomArray[i] == randomArray[j]) {
                        IsPass = false;
                    } else {
                        IsPass = true;
                    }
                }
                str = randomArray[i] + ", " + str;
            }

            productPriceRandom.ClearArray();
            //Debug.Log(str);
        }

        public void GeneratorProductPrice() {
            productPriceRandom.RandomRange(50, 300, 5, 8);
            productPriceRandom.RandomRange(301, 800, 6, 10);
            productPriceRandom.RandomRange(801, 1600, 8, 12);
            productPriceRandom.RandomRange(1601, 2400, 10, 14);
            productPriceRandom.RandomRange(2401, 3200, 11, 16);
            productPriceRandom.RandomRange(3201, 4000, 10, 14);
            productPriceRandom.RandomRange(4001, 4800, 9, 12);
            productPriceRandom.RandomRange(4801, 5600, 8, 11);
            productPriceRandom.RandomRange(5601, 6400, 7, 10);
            productPriceRandom.RandomRange(6401, 7200, 6, 8);
            productPriceRandom.RandomRange(7201, 8000, 5, 7);
            productPriceRandom.RandomRange(8001, 9600, 4, 6);
            productPriceRandom.RandomRange(9601, 11200, 3, 5);
            productPriceRandom.RandomRange(11201, 12800, 3, 4);
            productPriceRandom.RandomRange(12801, 14400, 2, 3);
            productPriceRandom.RandomRange(14401, 16000, 1, 2);
            productPriceRandom.RandomRange(16001, 18000, 0, 1);
            productPriceRandom.RandomRange(18001, 20000, 0, 1);
        }

        [Test]
        public void Test1_一組範圍的隨機產生價格都不同() {
            // 隨機產生商品價格
            隨機產生商品價格(50, 300, 5);
            隨機產生商品價格(301, 800, 6);
            隨機產生商品價格(801, 1600, 8);
            隨機產生商品價格(1601, 2400, 10);
            隨機產生商品價格(2401, 3200, 11);
            隨機產生商品價格(3201, 4000, 10);
            隨機產生商品價格(4001, 4800, 9);
            隨機產生商品價格(4801, 5600, 8);
            隨機產生商品價格(5601, 6400, 7);
            隨機產生商品價格(6401, 7200, 6);
            隨機產生商品價格(7201, 8000, 5);
            隨機產生商品價格(8001, 9600, 4);
            隨機產生商品價格(9601, 11200, 3);
            隨機產生商品價格(11201, 12800, 3);
            隨機產生商品價格(12801, 14400, 2);
            隨機產生商品價格(14401, 16000, 2);
            隨機產生商品價格(16001, 18000, 2);
            隨機產生商品價格(18001, 20000, 2);

            Assert.IsTrue(IsPass);
        }
        */

        /*
        [Test]
        public void 三組相同範圍的隨機產生價格都不同() {
            string str1 = "", str2 = "", str3 = "";

            ProductPriceRandom.RandomCount(1, 10, 5);
            ArrayList randomArray1 = ProductPriceRandom.GetArray_Temp();

            foreach (int price in randomArray1) {
                str1 = price + ", " + str1;
            }

            ProductPriceRandom.ClearArray();

            // ---------------------------------------------------------

            ProductPriceRandom.RandomCount(1, 10, 5);
            ArrayList randomArray2 = ProductPriceRandom.GetArray_Temp();

            foreach (int price in randomArray2) {
                str2 = price + ", " + str2;
            }

            ProductPriceRandom.ClearArray();

            // ---------------------------------------------------------

            ProductPriceRandom.RandomCount(1, 10, 5);
            ArrayList randomArray3 = ProductPriceRandom.GetArray_Temp();

            foreach (int price in randomArray3) {
                str3 = price + ", " + str3;
            }

            ProductPriceRandom.ClearArray();

            Debug.Log(str1 + " != " + str2 + " != " + str3);
            Assert.AreNotEqual(str1, str2);
            Assert.AreNotEqual(str2, str3);
            Assert.AreNotEqual(str1, str3);
        }
        */

        /*
        [Test]
        public void 多組相同範圍的隨機產生不同數量的商品() {
            ArrayList randomArray1 = new ProductPriceRandom().RandomRange(50, 3000, 4, 6);
            ArrayList randomArray2 = new ProductPriceRandom().RandomRange(50, 3000, 4, 6);
            ArrayList randomArray3 = new ProductPriceRandom().RandomRange(50, 3000, 4, 6);

            Debug.Log(randomArray1.Count + " != " + randomArray2.Count + " != " + randomArray3.Count);

            if ((randomArray1.Count == randomArray2.Count) &&
                (randomArray2.Count == randomArray3.Count) &&
                (randomArray1.Count == randomArray3.Count)) {
                Assert.Fail();
            }
        }
        */

        /*
        [Test]
        public void 多組相同範圍的隨機產生一定數量的商品() {
            string str1 = "", str2 = "", str3 = "";

            ProductPriceRandom.RandomRange(50, 3000, 4, 6);
            ArrayList randomArray1 = ProductPriceRandom.GetArray_Temp();

            foreach (int price in randomArray1) {
                str1 = price + ", " + str1;
            }

            ProductPriceRandom.ClearArray();

            // ---------------------------------------------------------

            ProductPriceRandom.RandomRange(50, 3000, 4, 6);
            ArrayList randomArray2 = ProductPriceRandom.GetArray_Temp();

            foreach (int price in randomArray2) {
                str2 = price + ", " + str2;
            }

            ProductPriceRandom.ClearArray();

            // ---------------------------------------------------------

            ProductPriceRandom.RandomRange(50, 3000, 4, 6);
            ArrayList randomArray3 = ProductPriceRandom.GetArray_Temp();

            foreach (int price in randomArray3) {
                str3 = price + ", " + str3;
            }

            ProductPriceRandom.ClearArray();
            
            Debug.Log(str1);
            Debug.Log(str2);
            Debug.Log(str3);
            
            Assert.AreNotEqual(str1, str2);
            Assert.AreNotEqual(str2, str3);
            Assert.AreNotEqual(str1, str3);
        }
        */


        /* ------------------------------------
        [Test]
        public void Test2_將隨機產生的商品價格從TempArray放入ProductPriceArray且數量相同() {
            GeneratorProductPrice();
            productPriceRandom.PutRandomIntoArray();
            ArrayList randomArrayTemp = productPriceRandom.GetArray_Temp();
            ArrayList randomArrayProduct = productPriceRandom.GetArray_ProductPrice();

            if (randomArrayTemp.Count == 0 || randomArrayProduct.Count == 0) {
                Assert.Fail();
            }
            //Debug.Log(randomArrayTemp.Count + ", " + randomArrayProduct.Count);
            Assert.AreEqual(randomArrayTemp.Count, randomArrayProduct.Count);
        }

        [Test]
        public void Test3_將隨機產生的商品價格從TempArray放入ProductPriceArray且內容相同() {
            GeneratorProductPrice();
            productPriceRandom.PutRandomIntoArray();
            ArrayList randomArrayTemp = productPriceRandom.GetArray_Temp();
            ArrayList randomArrayProduct = productPriceRandom.GetArray_ProductPrice();

            Assert.AreEqual(randomArrayTemp.Count, randomArrayProduct.Count);

            for (int i = 0; i < randomArrayTemp.Count; i++) {
                for (int j = i + 1; j < randomArrayProduct.Count; j++) {
                    if (randomArrayTemp[i] == randomArrayTemp[j]) {
                        IsPass = false;
                    } else {
                        IsPass = true;
                    }
                }
            }
            //Debug.Log(randomArrayTemp.Count + ", " + randomArrayProduct.Count);
            Assert.IsTrue(IsPass);
        }

        [Test]
        public void Test4_利用百分比計算各價格區間的商品數量() {
            productPriceRandom.ProductNum = 200;

            GeneratorProductPrice();
            productPriceRandom.PutRandomIntoArray();
            ArrayList ProductPrice = productPriceRandom.GetArray_ProductPrice();
            string str = "";

            foreach (int price in ProductPrice) {
                str = price + ", " + str;
            }
            //Debug.Log(ProductPrice.Count);
            //Debug.Log(str);
            Assert.IsTrue(ProductPrice.Count >= productPriceRandom.ProductNum);
        }

        [Test]
        public void Test5_高於8000價值商品不會因用百分比計算而提升倍率依然只會固定在預設百分比數量內() {
            productPriceRandom.ProductNum = 200;
            productPriceRandom.HighScore = 8000;

            GeneratorProductPrice();
            productPriceRandom.PutRandomIntoArray();
            ArrayList ProductPrice = productPriceRandom.GetArray_ProductPrice();

        }
        */
    }
}
