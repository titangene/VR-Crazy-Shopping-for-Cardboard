using System;
using UnityEngine;

public class ProductManager : MonoBehaviour {
    /// <summary>
    /// 商品數量
    /// </summary>
    public ushort productNum = 270;
    /// <summary>
    /// 限制高價值商品數量
    /// </summary>
    public ushort highScore = 8000;
    /// <summary>
    /// 商品貨架群組物件
    /// </summary>
    public Transform cabinetGroup;

    public RandomController randomCtrl;
    public JSONController jsonCtrl;
    public ProductPriceRandom priceRandom;
    private ProductDataJSON dataJSON;
    public ProductRandomPosition randomPosition;
    public RangePosition rangePosition;

    /// <summary>
    /// JSON 目錄
    /// </summary>
    private string path;
    /// <summary>
    /// JSON 完整目錄
    /// </summary>
    private string fullPath;
    public LitJson.JsonData json;

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
        // 商品貨架群組物件
        cabinetGroup = GameObject.FindWithTag("CabinetGroup").transform;

        randomCtrl = new RandomController();
        jsonCtrl = new JSONController();
        priceRandom = new ProductPriceRandom();
        dataJSON = new ProductDataJSON();
        randomPosition = new ProductRandomPosition();
        rangePosition = new RangePosition();

        // JSON 目錄
        path = jsonCtrl.SetPath();
        // JSON 完整目錄
        fullPath = jsonCtrl.SetAllPath(path, "/ProductDate.json");
    }

    void Start() {
        /*
        string str = "";
        foreach (ushort i in productPriceRandom.GetArray_ProductPrice()) {
            str = str + ", " + i;
        }
        Debug.Log(productPriceRandom.GetArray_ProductPrice().Count);
        Debug.Log(str);
        */
    }

    public void CreateAllScript() {
        randomCtrl = new RandomController();
        jsonCtrl = new JSONController();
        priceRandom = new ProductPriceRandom();
        dataJSON = new ProductDataJSON();
        randomPosition = new ProductRandomPosition();
        rangePosition = new RangePosition();

        Debug.Log("ProductManager：CreateAllScript");
    }

    public void Create_Cabinet_Product() {
        // 商品貨架群組物件
        cabinetGroup = GameObject.FindWithTag("CabinetGroup").transform;

        /* ProductPriceRandom */
        // 建立 array (商品價格、暫存)
        priceRandom.CreateArray();
        // 建立所有商品價格資料
        priceRandom.SetAllData();
        // 隨機產生商品價格
        priceRandom.GeneratorProductPrice();
        // 將隨機產生的商品價格，從 Temp array 放入 ProductPrice array
        priceRandom.PutRandomIntoArray();

        /* ProductDataJSON */
        // 設定並建立 JSON 路徑
        //jsonCtrl.CreateDirectory(path);
        // 刪除 Json 檔
        jsonCtrl.DeleteFile(fullPath);
        // 產生測試用的商品名稱、ID 資料
        json = dataJSON.GeneratorProductNameData();
        // 將資料寫入 Json 檔
        jsonCtrl.OutputJsonFile(json, fullPath);

        /* ProductPriceRandom */
        // 清空 array(商品價格、暫存)
        priceRandom.ClearArray();

        /* ProductRandomPosition */
        randomPosition.SetProductJSON_Path(json);
        // 設定所有商品貨價的數量、位置、角度
        randomPosition.SetAllCabinetPosition();

        /* RangePosition */
        // 自動產生 Cabinet Range
        rangePosition.SetCreate();

        Debug.Log("ProductManager：Create_Cabinet_Product");
    }

    public void Deleted() {
        Destroy(cabinetGroup.gameObject);
        GameObject obj = new GameObject();
        obj.name = "CabinetGroup";
        obj.tag = "CabinetGroup";

        /* RangePosition */
        // 摧毀所有 Range 物件
        rangePosition.Destroy();

        Debug.Log("ProductManager：Deleted All Cabinet and Range");
    }

    public void DestoryAll() {
        /* Clear */
        randomCtrl = null;
        jsonCtrl = null;
        priceRandom = null;
        dataJSON = null;
        randomPosition = null;
        GC.Collect();
        GC.WaitForPendingFinalizers();

        Debug.Log("ProductManager：DestoryAll");
    }

    void OnDestroy() {
        /* Clear */
        randomCtrl = null;
        jsonCtrl = null;
        priceRandom = null;
        dataJSON = null;
        randomPosition = null;
        GC.Collect();
        GC.WaitForPendingFinalizers();

        if (instance == this)
            instance = null;
    }
}
