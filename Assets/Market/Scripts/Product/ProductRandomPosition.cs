using System;
using System.Collections.Generic;
using UnityEngine;

public class ProductRandomPosition {
    /// <summary>
    /// 每排有幾列
    /// </summary>
    public ushort col = 4;
    /// <summary>
    /// 一個商品貨架有幾個商品
    /// </summary>
    public ushort productCount = 9;
    /// <summary>
    /// 第一列 Z 軸的位置
    /// </summary>
    public float first_Z = 5.23f;
    /// <summary>
    /// 第二列 Z 軸開始的位置
    /// </summary>
    public float second_Z;
    /// <summary>
    /// 最後一列 Z 軸的位置
    /// </summary>
    public float final_Z;
    /// <summary>
    /// 第一列與第二列之間的距離
    /// </summary>
    public float first_second_D = 1.456f;
    /// <summary>
    /// 兩排之間的距離
    /// </summary>
    public float twoCol_D = 2.104f;
    /// <summary>
    /// 相鄰兩列之間的距離
    /// </summary>
    public float twoRow_D = 0.876f;
    /// <summary>
    /// 不同區域(不相鄰的列)之間的距離
    /// </summary>
    public float twoArea_D = 6f;

    /// <summary>
    /// 載入 CabinetGroup.prefab
    /// </summary>
    private UnityEngine.Object cabinetPrefab;
    /// <summary>
    /// Instantiate 化商品貨架物件
    /// </summary>
    private GameObject cabinet;
    /// <summary>
    /// 商品貨架 ID
    /// </summary>
    private int cabinetID = 0;
    /// <summary>
    /// 商品 ID
    /// </summary>
    private int productID = 1;
    /// <summary>
    /// 商品貨架放置位置
    /// </summary>
    private Vector3 v_position;
    /// <summary>
    /// 商品貨架放置角度
    /// </summary>
    private Quaternion v_rotation;
    /// <summary>
    /// 暫存 array
    /// </summary>
    private System.Collections.ArrayList temp;
    /// <summary>
    /// 商品資訊 (JSON)
    /// </summary>
    private LitJson.JsonData json;
    /// <summary>
    /// 商品編號最大隨機參數
    /// </summary>
    private ushort proRandomMax = (ushort) (ProductManager.Instance.productNum + 1);
    /// <summary>
    /// 隨機商品編號
    /// </summary>
    private int proRandomID;

    /// <summary>
    /// 存放 預製商品 之 List
    /// </summary>
    private List<GameObject> ProPrefabList;
    /// <summary>
    /// 預製商品數量
    /// </summary>
    private int ProPrefabCount = 3;

    public void SetProductJSON_Path(LitJson.JsonData Json) {
        // 讀取 Json 檔
        json = Json;
    }

    /// <summary>
    /// 設定所有商品貨價的數量、位置、角度
    /// </summary>
    public void SetAllCabinetPosition() {
        temp = new System.Collections.ArrayList();

        // 載入 CabinetGroup.prefab
        cabinetPrefab = Resources.Load("Cabinet");

        ProPrefabList = new List<GameObject>();
        string ProPrefabName;
        GameObject ProPrefabTmp;
        // 載入所有 PrefabNamexxxx.prefab 放入 List
        for (int num = 1; num <= ProPrefabCount; num++) {
            ProPrefabName = "Product/ProductObj" + num.ToString().PadLeft(4, '0');
            ProPrefabTmp = Resources.Load(ProPrefabName, typeof(GameObject)) as GameObject;
            ProPrefabList.Add(ProPrefabTmp);
        }

        ProPrefabName = null;
        ProPrefabTmp = null;

        // 建立第 1 列商品貨價
        SetCabinetPosition(1, -1 * twoRow_D - twoArea_D, first_Z, 180);
        SetCabinetPosition(1, 0f, first_Z, 180);
        SetCabinetPosition(1, twoRow_D + twoArea_D, first_Z, 180);
        // 計算第二列 Z 軸開始的位置
        second_Z = first_Z + first_second_D;
        // 建立第 2~6 列商品貨價
        SetCabinetPosition(col, -2 * twoRow_D - twoArea_D, second_Z, -90);
        SetCabinetPosition(col, -1 * twoArea_D, second_Z, 90);
        SetCabinetPosition(col, -1 * twoRow_D, second_Z, -90);
        SetCabinetPosition(col, twoRow_D, second_Z, 90);
        SetCabinetPosition(col, twoArea_D, second_Z, -90);
        SetCabinetPosition(col, 2 * twoRow_D + twoArea_D, second_Z, 90);
        // 計算最後一列 Z 軸的位置
        final_Z = second_Z + twoCol_D * (col - 1) + first_second_D;
        // 建立第 7 列商品貨價
        SetCabinetPosition(1, -1 * twoRow_D - twoArea_D, final_Z, 0);
        SetCabinetPosition(1, 0f, final_Z, 0);
        SetCabinetPosition(1, twoRow_D + twoArea_D, final_Z, 0);

        // 清空 array (暫存)
        temp.Clear();

        cabinetPrefab = null;
        ProPrefabList.Clear();
        Resources.UnloadAsset(cabinetPrefab);
        Resources.UnloadUnusedAssets();
        cabinetID = 0;
        productID = 1;
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
            v_position = new Vector3(position_X, 0, position_Z);
            v_rotation = Quaternion.Euler(0, rotation_Y, 0);

            // Instantiate 化商品貨架物件
            InstantiateProduct();

            position_Z += twoCol_D;
        }
    }

    /// <summary>
    /// Instantiate 化商品貨架物件
    /// </summary>
    public void InstantiateProduct() {
        // Instantiate 化商品貨架物件
        cabinet = UnityEngine.Object.Instantiate(cabinetPrefab, v_position, v_rotation) as GameObject;
        // 將商品貨架放入商品貨架群組內內
        cabinet.transform.SetParent(ProductManager.Instance.cabinetGroup, false);
        // 每 Instantiate 化一個商品貨架物件之前，就將 Cabinet ID 就 + 1
        cabinetID++;
        // 設定 Cabinet name 為 Cabinetxxxx
        cabinet.name = "Cabinet" + cabinetID.ToString().PadLeft(4, '0');

        // CabinetGroup 內所有的子物件
        Array CabinetArray = cabinet.GetComponentsInChildren(typeof(Transform));

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
        ushort CabinetProID = 1;

        foreach (Transform child in Obj) {
            if (child.CompareTag("Pro_Price") || child.CompareTag("Pro_Name") ||
                child.CompareTag("Pro_Position")) {
                productID = CabinetProID + (productCount * (cabinetID - 1));

                if (child.CompareTag("Pro_Price")) {
                    // 隨機將 ProductDate.json 之商品價格放入
                    RandomProductPriceJson(child);
                    child.name = "Pro_Price" + proRandomID.ToString().PadLeft(4, '0');

                } else if (child.CompareTag("Pro_Name")) {
                    child.name = "Pro_Name" + temp[productID - 1].ToString().PadLeft(4, '0');
                    // 產生隨機商品名稱 (目前已商品編號亂數，不重複編號)
                    RandomProductName(child);

                } else if (child.CompareTag("Pro_Position")) {
                    child.name = "Pro_Position" + temp[productID - 1].ToString().PadLeft(4, '0');
                    // 隨機放入預製商品模型並套用隨機顏色
                    RandomProductModelAndColor(child);
                }

                CabinetProID++;

            } else
                continue;

            if (CabinetProID > productCount)
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
        proRandomID = ProductManager.Instance.randomCtrl.random.Next(1, proRandomMax);
        // 商品編號不重複使用
        while (temp.Contains(proRandomID))
            proRandomID = ProductManager.Instance.randomCtrl.random.Next(1, proRandomMax);
        // 將產生正確的商品編號放入暫存 array
        temp.Add(proRandomID);

        // JSON 編號
        int json_ID = proRandomID - 1;
        // 設定商品價格：$xxxx
        //string Price = proRandomID + "$" + json["product"][json_ID]["price"].ToString();
        string Price = "$" + json["product"][json_ID]["price"].ToString();
        childPriceText.text = Price;
    }

    /// <summary>
    /// 產生隨機商品名稱 (目前已商品編號亂數，不重複編號)
    /// </summary>
    private void RandomProductName(Transform child) {
        // 設定商品文字名稱：Productxxxx
        UnityEngine.UI.Text childNameText = child.GetComponent<UnityEngine.UI.Text>();
        childNameText.text = "Product" + temp[productID - 1].ToString().PadLeft(4, '0');
    }

    /// <summary>
    /// 隨機放入預製商品模型並套用隨機顏色
    /// </summary>
    private void RandomProductModelAndColor(Transform child) {
        // 預製商品總數會隨機產生其編號
        int num = ProductManager.Instance.randomCtrl.random.Next(0, ProPrefabCount);
        // 載入 PrefabNamexxxx.prefab
        GameObject ProPrefab = ProPrefabList[num];
        // Instantiate 化商品物件
        GameObject Product = UnityEngine.Object.Instantiate(ProPrefab) as GameObject;

        // 產生隨機顏色
        Color newColor = RandomColor();
        // 套用隨機顏色至商品物件
        Product.GetComponent<Renderer>().material.color = newColor;
        // 將商品放入商品貨架之指定位置內
        Product.transform.SetParent(child, false);
        // 設定商品物件名稱
        Product.name = "Pro_Obj" + temp[productID - 1].ToString().PadLeft(4, '0');
    }

    /// <summary>
    /// 產生隨機顏色
    /// </summary>
    private Color RandomColor() {
        return new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
    }
}
