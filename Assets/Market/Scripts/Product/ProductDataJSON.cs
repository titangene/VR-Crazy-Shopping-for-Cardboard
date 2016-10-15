using UnityEngine;
using LitJson;
using System.Collections;

public class ProductDataJSON {
    /// <summary>
    /// 完整目錄
    /// </summary>
    public string FullPath { private set; get; }
    public JsonData json { private set; get; }

    ushort ProductId = 1;

    /// <summary>
    /// 設定並建立 JSON 路徑
    /// </summary>
    public void SetJSONPath() {
        // 目錄
        string path = Application.dataPath + "/Test/JSON/";
        // 設定 Output 的檔案位置
        // D:\YourProject\Assets\Test\JSON\ProductDate.json
        FullPath = path + "ProductDate.json";

        // 建立目錄
        ProductManager.Instance.jsonCtrl.CreateDirectory(path);
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

        // 取得商品價格 array
        ArrayList ProductPrice = ProductManager.Instance.productPriceRandom.GetArray_ProductPrice();
        // 產生新的亂數 value
        //RandomController.GeneratorRandom();

        for (ushort i = 0; i < ProductManager.Instance.ProductNum; i++) {
            json["product"].Add(new JsonData());
            json["product"][i]["id"] = ProductId;
            // PadLeft(補足的長度, '要補的內容')
            // EX：string str = "23"; PadLeft(4, '0');
            // 輸出結果： 0023
            json["product"][i]["name"] = "Product" + ProductId.ToString().PadLeft(4, '0');
            json["product"][i]["price"] = (ushort) ProductPrice[i];

            ProductId++;
        }
    }
}
