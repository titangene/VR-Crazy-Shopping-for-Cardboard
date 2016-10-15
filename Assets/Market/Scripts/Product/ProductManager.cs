using System;
using UnityEngine;

public class ProductManager : MonoBehaviour {
    /// <summary>
    /// 商品數量
    /// </summary>
    public ushort ProductNum = 378;
    /// <summary>
    /// 限制高價值商品數量
    /// </summary>
    public ushort HighScore = 8000;
    /// <summary>
    /// 商品貨架群組物件
    /// </summary>
    public Transform CabinetGroup;

    public RandomController randomCtrl;
    public JSONController jsonCtrl;
    public ProductPriceRandom productPriceRandom;
    private ProductDataJSON productDataJSON;
    private ProductRandomPosition productRandomPosition;

    private static ProductManager instance = null;

    public static ProductManager Instance {
        get {
#if UNITY_EDITOR
            if (instance == null && !Application.isPlaying)
                instance = UnityEngine.Object.FindObjectOfType<ProductManager>();
#endif
            if (instance == null) {
                Debug.LogError("No ProductManager instance found.  Ensure one exists in the scene, or call "
                    + "ProductManager.Create() at startup to generate one.\n"
                    + "If one does exist but hasn't called Awake() yet, "
                    + "then this error is due to order-of-initialization.\n"
                    + "In that case, consider moving "
                    + "your first reference to ProductManager.Instance to a later point in time.\n"
                    + "If exiting the scene, this indicates that the ProductManager object has already "
                    + "been destroyed.");
            }
            return instance;
        }
    }

    public static void Create() {
        if (instance == null && UnityEngine.Object.FindObjectOfType<ProductManager>() == null) {
            Debug.Log("Creating ProductManager object");
            var go = new GameObject("ProductManager", typeof(ProductManager));
            go.transform.localPosition = Vector3.zero;
            // sdk will be set by Awake().
        }
    }

    void Awake() {
        if (instance == null)
            instance = this;
        if (instance != this) {
            Debug.LogError("There must be only one ProductManager object in a scene.");
            UnityEngine.Object.DestroyImmediate(this);
            return;
        }
        CabinetGroup = GameObject.FindWithTag("CabinetGroup").transform;

        randomCtrl = new RandomController();
        jsonCtrl = new JSONController();
        productPriceRandom = new ProductPriceRandom();
        productDataJSON = new ProductDataJSON();
        productRandomPosition = new ProductRandomPosition();

        /* ProductPriceRandom */
        // 建立 array (商品價格、暫存)
        productPriceRandom.CreateArray();
        // 建立所有商品價格資料
        productPriceRandom.SetAllData();
        // 隨機產生商品價格
        productPriceRandom.GeneratorProductPrice();
        // 將隨機產生的商品價格，從 Temp array 放入 ProductPrice array
        productPriceRandom.PutRandomIntoArray();
    }

    void Start () {
        string str = "";
        foreach (ushort i in productPriceRandom.GetArray_ProductPrice()) {
            str = str + ", " + i;
        }
        Debug.Log(productPriceRandom.GetArray_ProductPrice().Count);
        Debug.Log(str);

        /* ProductDataJSON */
        // 設定並建立 JSON 路徑
        productDataJSON.SetJSONPath();
        // 刪除 Json 檔
        jsonCtrl.DeleteFile(productDataJSON.FullPath);
        // 產生測試用的商品名稱、ID 資料
        productDataJSON.GeneratorProductNameData();
        // 將資料寫入 Json 檔
        jsonCtrl.OutputJsonFile(productDataJSON.json, productDataJSON.FullPath);

        /* ProductPriceRandom */
        // 清空 array(商品價格、暫存)
        productPriceRandom.ClearArray();

        /* ProductRandomPosition */
        productRandomPosition.SetProductJSON_Path();
        // 設定所有商品貨價的數量、位置、角度
        productRandomPosition.SetAllCabinetPosition();

        /* Clear */
        randomCtrl = null;
        jsonCtrl = null;
        productPriceRandom = null;
        productDataJSON = null;
        productRandomPosition = null;
        GC.Collect();
        GC.WaitForPendingFinalizers();
    }

    void OnDestroy() {
        if (instance == this)
            instance = null;
    }
}
