using LitJson;
using System.Collections;

public class ProductDataJSON {
    private JsonData json;
    ushort ProductId = 1;

    /// <summary>
    /// 產生測試用的商品名稱、ID 資料
    /// </summary>
    public JsonData GeneratorProductNameData() {
        // 新建 JsonData
        json = new JsonData();
        // 寫入商品資訊
        json["product"] = new JsonData();
        json["product"].SetJsonType(JsonType.Array);

        // 取得商品價格 array
        ArrayList ProductPrice = ProductManager.Instance.priceRandom.GetArray_ProductPrice();

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

        ProductId = 1;
        return json;
    }
}
