using UnityEngine;
using System.Collections;
using System;

public class ProductPriceRandom : MonoBehaviour {
    [Tooltip("商品數量")]
    public int ProductNum = 200;

    [Tooltip("限制高價值商品數量")]
    public int HighScore = 8000;

    [Serializable]
    public struct ProductPriceRange {
        public int minPrice;
        public int maxPrice;
        public int minRange;
        public int maxRange;
    }

    [SerializeField]
    public ProductPriceRange[] productPriceRange = new ProductPriceRange[0];

    // 商品價格 array
    private ArrayList ProductPrice;
    // 暫存 array
    private ArrayList Temp;
    // 亂數 value
    private System.Random random;

    string str;

    void Awake() {
        // 建立 array (商品價格、暫存)
        CreateArray();
        // 建立所有商品價格資料
        SetAllData();
        // 隨機產生商品價格
        GeneratorProductPrice();
        // 將隨機產生的商品價格，從 Temp array 放入 ProductPrice array
        PutRandomIntoArray();
    }

    void Start() {
        foreach (int i in ProductPrice) {
            str = i + ", " + str;
        }
        Debug.Log(ProductPrice.Count);
        Debug.Log(str);

        // 清空 array(商品價格、暫存)
        ClearArray();
    }

    /// <summary>
    /// 建立 array (商品價格、暫存)
    /// </summary>
    public void CreateArray() {
        ProductPrice = new ArrayList();
        Temp = new ArrayList();
    }

    /// <summary>
    /// 清空 array (商品價格、暫存)
    /// </summary>
    public void ClearArray() {
        ProductPrice.Clear();
        Temp.Clear();
    }

    /// <summary>
    /// 設定商品價格區間 minPrice ~ maxPrice 與 minCount ~ maxCount 個商品數
    /// </summary>
    /// <param name="id"></param>
    /// <param name="minPrice">該價格區間之最低價格</param>
    /// <param name="maxPrice">該價格區間之最高價格</param>
    /// <param name="minRange">該價格區間隨機產生的最少數量</param>
    /// <param name="maxRange">該價格區間隨機產生的最大數量</param>
    public void SetData(int id, int minPrice, int maxPrice, int minRange, int maxRange) {
        productPriceRange[id].minPrice = minPrice;
        productPriceRange[id].maxPrice = maxPrice;
        productPriceRange[id].minRange = minRange;
        productPriceRange[id].maxRange = maxRange;
    }

    /// <summary>
    /// 建立所有商品價格資料
    /// </summary>
    public void SetAllData() {
        SetData(0, 50, 300, 5, 8);
        SetData(1, 301, 800, 6, 10);
        SetData(2, 801, 1600, 8, 12);
        SetData(3, 1601, 2400, 10, 14);
        SetData(4, 2401, 3200, 11, 16);
        SetData(5, 3201, 4000, 10, 14);
        SetData(6, 4001, 4800, 9, 12);
        SetData(7, 4801, 5600, 8, 11);
        SetData(8, 5601, 6400, 7, 10);
        SetData(9, 6401, 7200, 6, 8);
        SetData(10, 7201, 8000, 5, 7);
        SetData(11, 8001, 9600, 4, 6);
        SetData(12, 9601, 11200, 3, 5);
        SetData(13, 11201, 12800, 3, 4);
        SetData(14, 12801, 14400, 2, 3);
        SetData(15, 14401, 16000, 1, 2);
        SetData(16, 16001, 18000, 0, 1);
        SetData(17, 18001, 20000, 0, 1);
    }

    /// <summary>
    /// 隨機產生商品價格
    /// </summary>
    public void GeneratorProductPrice() {
        for (int i = 0; i < productPriceRange.Length; i++) {
            RandomRange(productPriceRange[i].minPrice, productPriceRange[i].maxPrice,
                        productPriceRange[i].minRange, productPriceRange[i].maxRange);
        }
    }

    /// <summary>
    /// 產生新的亂數 value
    /// </summary>
    public void GeneratorRandom() {
        // 使用 DateTime.Now.Ticks 可產生不重複的隨機亂數
        // DateTime.Now.Ticks 是指從 DateTime.MinValue 之後過了多少時間，10000000 為一秒
        random = new System.Random((int) DateTime.Now.Ticks);
    }

    /// <summary>
    /// 將隨機產生的商品價格，從 Temp array 放入 ProductPrice array
    /// </summary>
    public void PutRandomIntoArray() {
        GeneratorRandom();
        for (int i = 0; i < Temp.Count; i++) {
            // Temp array 中第幾個
            int num = random.Next(0, Temp.Count);

            // 如商品價格已放至 ProductPrice array，就重新找出還沒放入之其他商品價格
            while (ProductPrice.Contains(Temp[num])) {
                num = random.Next(0, Temp.Count);
            }

            // 將商品價格放入 ProductPrice array
            ProductPrice.Add(Temp[num]);
        }
    }

    /// <summary>
    /// 1. 隨機產生多個某範圍區間的商品價格，
    /// 2. 將隨機產生的商品價格放入 Temp array。
    /// EX：價格區間 min ~ max 有 count 個隨機產生的商品價格
    /// </summary>
    /// <param name="min">該價格區間之最低價格</param>
    /// <param name="max">該價格區間之最高價格</param>
    /// <param name="count">該價格區間隨機產生的數量</param>
    public void RandomCount(int min, int max, int count) {
        GeneratorRandom();
        for (int i = 0; i < count; i++) {
            int price = random.Next(min, max + 1);

            if (!Temp.Contains(price)) {
                Temp.Add(price);
            }
        }
    }

    /// <summary>
    /// 每個價格範圍區間產生一定數量的商品 (數量是隨機產生，且可自定隨機數量範圍)，
    /// EX：價格區間 min ~ max 有 minCount ~ maxCount 個商品數以隨機的方式產生的商品價格及數量
    /// </summary>
    /// <param name="min">該價格區間之最低價格</param>
    /// <param name="max">該價格區間之最高價格</param>
    /// <param name="minCount">該價格區間隨機產生的最少數量</param>
    /// <param name="maxCount">該價格區間隨機產生的最大數量</param>
    public void RandomRange(int min, int max, int minCount, int maxCount) {
        GeneratorRandom();
        //int minRangePercent = Convert.ToInt32((float) minRange * ProductNum / 100);
        //int maxRangePercent = Convert.ToInt32((float) (maxRange + 1) * ProductNum / 100 * 1.1);
        int minRangePercent = Convert.ToInt32(Math.Floor((double) minCount * ProductNum / 100 * 1.3));
        int maxRangePercent = Convert.ToInt32(Math.Floor((double) (maxCount + 1) * ProductNum / 100));

        if (min > HighScore) {
            minRangePercent = minCount;
            maxRangePercent = maxCount;
        }

        int range = random.Next(minRangePercent, maxRangePercent);

        // 將隨機產生的某價格區間商品價格 放入 Temp array
        RandomCount(min, max, range);

        if (max > HighScore) {
            //Debug.Log(min + " ~ " + max + ":" + range);
        }
        //Debug.Log(min + " ~ " + max + ":" + range);
    }

    public ArrayList GetArray_ProductPrice() {
        return this.ProductPrice;
    }

    public ArrayList GetArray_Temp() {
        return this.Temp;
    }
}
