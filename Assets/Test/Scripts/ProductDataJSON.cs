using UnityEngine;
using LitJson;
using System;
using System.IO;
using System.Collections;

public class ProductDataJSON : MonoBehaviour {

    private ProductPriceRandom productPriceRandom;
    public int ProductNum = 200;

    /// <summary>
    /// 完整目錄
    /// </summary>
    private string FullPath;

    private JsonData json;

    int ProductId = 1;
    // 亂數 value
    private System.Random random;

    void Start () {
        // 目錄
        string path = Application.dataPath + "/Test/JSON/";

        // 設定 Output 的檔案位置
        // D:\YourProject\Assets\Test\JSON\ProductNameDate.json
        FullPath = path + "ProductDate.json";

        // 建立目錄
        CreateDirectory(path);
        // 刪除 Json 檔
        DeleteFile();
        // 產生測試用的商品名稱、ID 資料
        GeneratorProductNameData();
        // 將資料寫入 Json 檔
        OutputJsonFile();
        //Debug.Log(WriteJsonAndPrettyPrint());
    }

    /// <summary>
    /// 建立目錄
    /// </summary>
    /// <param name="path">目錄</param>
    private void CreateDirectory(string path) {
        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }
    }

    /// <summary>
    /// 刪除 Json 檔
    /// </summary>
    private void DeleteFile() {
        if (File.Exists(FullPath)) {
            File.Delete(FullPath);
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
    /// 產生測試用的商品名稱、ID 資料
    /// </summary>
    public void GeneratorProductNameData() {
        // 新建 JsonData
        json = new JsonData();
        // 寫入商品資訊
        json["product"] = new JsonData();
        json["product"].SetJsonType(JsonType.Array);

        productPriceRandom = gameObject.GetComponent<ProductPriceRandom>();

        // 取得商品價格 array
        ArrayList ProductPrice = productPriceRandom.GetArray_ProductPrice();
        // 產生新的亂數 value
        GeneratorRandom();

        for (int i = 0; i < ProductNum; i++) {
            json["product"].Add(new JsonData());
            json["product"][i]["id"] = ProductId;
            // PadLeft(補足的長度, '要補的內容')
            // EX：string str = "23"; PadLeft(4, '0');
            // 輸出結果： 0023
            json["product"][i]["name"] = "Product" + ProductId.ToString().PadLeft(4, '0');
            json["product"][i]["price"] = (int)ProductPrice[i];

            ProductId++;
        }
    }

    /// <summary>
    /// 寫入 Json 資料 && 美化 Json
    /// </summary>
    public string WriteJsonAndPrettyPrint() {
        JsonWriter jsonwriter = new JsonWriter();
        // 美化 Json
        jsonwriter.PrettyPrint = true;
        // 縮排大小
        jsonwriter.IndentValue = 2;
        JsonMapper.ToJson(json, jsonwriter);

        string str = jsonwriter.ToString();
        return str;
    }

    /// <summary>
    /// 將資料寫入 Json 檔
    /// </summary>
    private void OutputJsonFile() {
        using (StreamWriter sw = new StreamWriter(FullPath, true)) {
            sw.WriteLine(WriteJsonAndPrettyPrint());
        }
    }
}
