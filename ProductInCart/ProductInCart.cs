using UnityEngine;
using System.Collections;

/* 1.  在購物車要推之前(準心對準購物車手把時)，紀錄每個商品在購物車內的位置
 * 2.  以 Mesh 作為觸發區域，偵測在觸發區域內的商品 = 放入購物車內的商品
 * 3.  以陣列或其他方式記錄每個商品和購物車的距離(x, y, z)
 * 4.  購物車移動時，會造成購物車位置改變和旋轉角度改變，必須要修正每個商品的位置看起來在購物車沒有隨便穿透或改變位置
 */
public class ProductInCart : MonoBehaviour {

    // 找到 class CardboardControl
    private static CardboardControl cardboard;
    // 準心對準的物體
    private static CardboardControlGaze gaze;
    // 準心對準的物體名稱
    private string GazeObjectName;

    // 紀錄是否放入購物車內的所有商品物件
    public bool PrintStr = true;

    // 紀錄放入購物車內的所有商品物件
    private ArrayList GetProduct;

    private ArrayList Product;

    void Start () {
        // 找到 CardboardControlManager 中的 CardboardControl.cs Script
        cardboard = GameObject.Find("CardboardControlManager").GetComponent<CardboardControl>();

        // 準心改變對準的物體時
        cardboard.gaze.OnChange += CardboardGazeChange;
        // 準心持續對準某個物體時
        cardboard.gaze.OnStare += CardboardStare;

        //CartTrigger.SetProductPosition(transform.position);
    }

    // 準心改變對準的物體時
    private void CardboardGazeChange(object sender) {
        // 準心對準的物體
        gaze = sender as CardboardControlGaze;
        // Debug 如果準心沒有對準任何東西，會設定對準目標名稱 = nothing
        GazeObjectName = gaze.IsHeld() ? gaze.Object().name : "nothing";
        //Debug.Log("Change：" + GazeObjectName);

        CheckGaze();
    }

    // 準心持續對準某個物體時
    private void CardboardStare(object sender) {
        gaze = sender as CardboardControlGaze;
        // Debug 如果準心沒有對準任何東西，會設定對準目標名稱 = nothing
        GazeObjectName = gaze.IsHeld() ? gaze.Object().name : "nothing";
        //Debug.Log("Stare：" + GazeObjectName);

        
    }

    void Update () {
        
    }

    // 檢查準心是否對準購物車手把
    private void CheckGaze() {
        // 準心對準購物車手把時 gaze.IsHeld() = true，準心沒有對準時 = false
        // 購物車手把物件名稱為 Handle
        if (gaze.IsHeld() && GazeObjectName == "Handle") {
            SaveProduct();
        }
    }

    private void SaveProduct() {
        // 取得所有放入購物車內的所有商品物件 return ArrayList
        //GetProduct = CartTrigger.GetProduct();

        foreach (GameObject GameObj in GetProduct) {
            Product.Add(GameObj);
            //Debug.Log(ProductName);
        }
    }
}
