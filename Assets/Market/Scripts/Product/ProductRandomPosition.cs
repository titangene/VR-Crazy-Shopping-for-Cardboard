using System;
using UnityEngine;

public class ProductRandomPosition {
    /// <summary>
    /// 每排有幾列
    /// </summary>
    private int col = 6;
    /// <summary>
    /// 一個商品貨架有幾個商品
    /// </summary>
    private int ProductCount = 9;
    /// <summary>
    /// 第一列 Z 軸的位置
    /// </summary>
    private float First_Z = 5.23f;
    /// <summary>
    /// 第二列 Z 軸開始的位置
    /// </summary>
    private float Second_Z;
    /// <summary>
    /// 最後一列 Z 軸的位置
    /// </summary>
    private float Final_Z;
    /// <summary>
    /// 第一列與第二列之間的距離
    /// </summary>
    private float First_Second_D = 1.456f;
    /// <summary>
    /// 兩排之間的距離
    /// </summary>
    private float TwoCol_D = 2.104f;
    /// <summary>
    /// 相鄰兩列之間的距離
    /// </summary>
    private float TwoRow_D = 0.876f;
    /// <summary>
    /// 不同區域(不相鄰的列)之間的距離
    /// </summary>
    private float TwoArea_D = 6f;

    /// <summary>
    /// 載入 CabinetGroup.prefab
    /// </summary>
    private GameObject CabinetPrefab;
    /// <summary>
    /// Instantiate 化商品貨架物件
    /// </summary>
    private GameObject Cabinet;
    /// <summary>
    /// 商品貨架 ID
    /// </summary>
    private int CabinetID = 0;
    /// <summary>
    /// 商品 ID
    /// </summary>
    private int ProductID = 1;
    /// <summary>
    /// 商品貨架放置位置
    /// </summary>
    private Vector3 V_Position;
    /// <summary>
    /// 商品貨架放置角度
    /// </summary>
    private Quaternion V_Rotation;
    /// <summary>
    /// 暫存 array
    /// </summary>
    private System.Collections.ArrayList Temp = new System.Collections.ArrayList();
    /// <summary>
    /// Log 訊息 的完整目錄
    /// </summary>
    private string FullPath;
    /// <summary>
    /// 商品資訊 (JSON)
    /// </summary>
    private LitJson.JsonData json;
    /// <summary>
    /// 商品編號最大隨機參數
    /// </summary>
    private ushort ProRandomMax = (ushort) (ProductManager.Instance.ProductNum + 1);
    /// <summary>
    /// 隨機商品編號
    /// </summary>
    private ushort ProRandomID;

    public void SetProductJSON_Path() {
        // 目錄
        string Path = Application.dataPath + "/Test/JSON/";
        // 設定 DebugOutput 的檔案位置
        // D:\YourProject\Assets\DebugLog\ProductDate.json
        FullPath = Path + "ProductDate.json";

        // 讀取 Json 檔
        json = ProductManager.Instance.jsonCtrl.ReadJson_LoadJsonFile(FullPath);
    }

    /// <summary>
    /// 設定所有商品貨價的數量、位置、角度
    /// </summary>
    public void SetAllCabinetPosition() {
        // 建立第 1 列商品貨價
        SetCabinetPosition(1, -1 * TwoRow_D - TwoArea_D, First_Z, 180);
        SetCabinetPosition(1, 0f, First_Z, 180);
        SetCabinetPosition(1, TwoRow_D + TwoArea_D, First_Z, 180);
        // 計算第二列 Z 軸開始的位置
        Second_Z = First_Z + First_Second_D;
        // 建立第 2~6 列商品貨價
        SetCabinetPosition(col, -2 * TwoRow_D - TwoArea_D, Second_Z, -90);
        SetCabinetPosition(col, -1 * TwoArea_D, Second_Z, 90);
        SetCabinetPosition(col, -1 * TwoRow_D, Second_Z, -90);
        SetCabinetPosition(col, TwoRow_D, Second_Z, 90);
        SetCabinetPosition(col, TwoArea_D, Second_Z, -90);
        SetCabinetPosition(col, 2 * TwoRow_D + TwoArea_D, Second_Z, 90);
        // 計算最後一列 Z 軸的位置
        Final_Z = Second_Z + TwoCol_D * (col - 1) + First_Second_D;
        // 建立第 7 列商品貨價
        SetCabinetPosition(1, -1 * TwoRow_D - TwoArea_D, Final_Z, 0);
        SetCabinetPosition(1, 0f, Final_Z, 0);
        SetCabinetPosition(1, TwoRow_D + TwoArea_D, Final_Z, 0);

        // 清空 array (暫存)
        Temp.Clear();
    }

    /// <summary>
    /// 設定商品貨價的數量、位置、角度
    /// </summary>
    /// <param name="count">數量</param>
    /// <param name="position_X">X 軸位置</param>
    /// <param name="position_Z">Z 軸位置</param>
    /// <param name="rotation_Y">Y 軸角度</param>
    public void SetCabinetPosition(int count, float position_X, float position_Z, float rotation_Y) {
        for (int i = 0; i < count; i++) {
            // 設定商品貨架的位置、角度 (X 軸位置、Z 軸位置、Y 軸角度)
            V_Position = new Vector3(position_X, 0, position_Z);
            V_Rotation = Quaternion.Euler(0, rotation_Y, 0);

            // Instantiate 化商品貨架物件
            InstantiateProduct();

            position_Z += TwoCol_D;
        }
    }

    /// <summary>
    /// Instantiate 化商品貨架物件
    /// </summary>
    public void InstantiateProduct() {
        // 載入 CabinetGroup.prefab
        CabinetPrefab = Resources.Load("Cabinet", typeof(GameObject)) as GameObject;
        // Instantiate 化商品貨架物件
        Cabinet = UnityEngine.Object.Instantiate(CabinetPrefab, V_Position, V_Rotation) as GameObject;
        // 將商品貨架放入商品貨架群組內內
        Cabinet.transform.SetParent(ProductManager.Instance.CabinetGroup, false);
        // 每 Instantiate 化一個商品貨架物件之前，就將 Cabinet ID 就 + 1
        CabinetID++;
        // 設定 Cabinet name 為 Cabinetxxxx
        Cabinet.name = "Cabinet" + CabinetID.ToString().PadLeft(4, '0');

        // CabinetGroup 內所有的子物件
        Array CabinetArray = Cabinet.GetComponentsInChildren(typeof(Transform));

        // 在 CabinetGroup 內找出物件名稱內為以下時，將其改為：
        // Pro_Pricexxxx, Pro_Namexxxx, Pro_Positionxxxx, Pro_Objxxxx
        ChildObjectRename(CabinetArray);
    }

    /// <summary>
    /// 找出物件名稱為 ObjName，將物件名稱改為 ObjNamexxxx，
    /// EX：ProObj0001
    /// </summary>
    /// <param name="Obj">想要尋找某子物件的物件</param>
    /// <param name="ObjChildName">想要尋找的子物件名稱</param>
    private void ChildObjectRename(Array Obj) {
        // 商品貨架內第幾個商品
        int CabinetProID = 1;

        foreach (Transform child in Obj) {
            if (child.CompareTag("Pro_Price") || child.CompareTag("Pro_Name") ||
                child.CompareTag("Pro_Position")) {
                ProductID = CabinetProID + (ProductCount * (CabinetID - 1));
            }

            if (child.CompareTag("Pro_Price")) {
                // 隨機將 ProductDate.json 之商品價格放入
                RandomProductPriceJson(child);
                child.name = "Pro_Price" + ProRandomID.ToString().PadLeft(4, '0');
            }

            if (child.CompareTag("Pro_Name")) {
                child.name = "Pro_Name" + Temp[ProductID - 1].ToString().PadLeft(4, '0');
                // 產生隨機商品名稱 (目前已商品編號亂數，不重複編號)
                RandomProductName(child);
            }

            if (child.name.Contains("Pro_Position")) {
                child.name = "Pro_Position" + Temp[ProductID - 1].ToString().PadLeft(4, '0');
                // 隨機放入預製商品模型並套用隨機顏色
                RandomProductModelAndColor(child);
            }

            if (child.CompareTag("Pro_Price") || child.CompareTag("Pro_Name") ||
                child.CompareTag("Pro_Position")) {
                CabinetProID++;
            }

            if (CabinetProID > ProductCount)
                CabinetProID = 1;
        }
    }

    /// <summary>
    /// 隨機將 ProductDate.json 之商品價格放入
    /// </summary>
    private void RandomProductPriceJson(Transform child) {
        // 商品價格文字
        TextMesh childPriceText = child.GetComponent<TextMesh>();

        // 隨機產生商品編號
        ProRandomID = (ushort) ProductManager.Instance.randomCtrl.random.Next(1, ProRandomMax);
        // 商品編號不重複使用
        while (Temp.Contains(ProRandomID))
            ProRandomID = (ushort) ProductManager.Instance.randomCtrl.random.Next(1, ProRandomMax);
        // 將產生正確的商品編號放入暫存 array
        Temp.Add(ProRandomID);

        // JSON 編號
        int json_ID = ProRandomID - 1;
        // 設定商品價格：$xxxx
        string Price = ProRandomID + "$" + json["product"][json_ID]["price"].ToString();
        childPriceText.text = Price;
    }

    /// <summary>
    /// 產生隨機商品名稱 (目前已商品編號亂數，不重複編號)
    /// </summary>
    private void RandomProductName(Transform child) {
        // 設定商品文字名稱：Productxxxx
        UnityEngine.UI.Text childNameText = child.GetComponent<UnityEngine.UI.Text>();
        childNameText.text = "Product" + Temp[ProductID - 1].ToString().PadLeft(4, '0');
    }

    /// <summary>
    /// 隨機放入預製商品模型並套用隨機顏色
    /// </summary>
    private void RandomProductModelAndColor(Transform child) {
        // 預製商品總數會隨機產生其編號
        ushort num = (ushort) ProductManager.Instance.randomCtrl.random.Next(1, 4);
        // 利用隨機編號找出對應的預製商品名稱
        string PrefabName = "Product/ProductObj" + num.ToString().PadLeft(4, '0');

        // 載入 PrefabNamexxxx.prefab
        GameObject ProPrefab = Resources.Load(PrefabName, typeof(GameObject)) as GameObject;
        // Instantiate 化商品物件
        GameObject Product = UnityEngine.Object.Instantiate(ProPrefab) as GameObject;
        // 產生隨機顏色
        Color newColor = RandomColor();
        // 套用隨機顏色至商品物件
        Product.GetComponent<Renderer>().material.color = newColor;
        // 將商品放入商品貨架之指定位置內
        Product.transform.SetParent(child, false);
        // 設定商品物件名稱
        Product.name = "Pro_Obj" + Temp[ProductID - 1].ToString().PadLeft(4, '0');
    }

    /// <summary>
    /// 產生隨機顏色
    /// </summary>
    private Color RandomColor() {
        return new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
    }
}
