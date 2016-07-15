using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct Product {
    public GameObject ProductObject;
    public Vector3 ProductPosition;

    public Product(GameObject name, Vector3 vector) {
        this.ProductObject = name;
        this.ProductPosition = vector;
    }
};

/* 1.  在購物車要推之前(準心對準購物車手把時)，紀錄每個商品在購物車內的位置
 * 2.  以 Mesh 作為觸發區域，偵測在觸發區域內的商品 = 放入購物車內的商品
 * 3.  以陣列或其他方式記錄每個商品和購物車的距離(x, y, z)
 * 4.  購物車移動時，會造成購物車位置改變和旋轉角度改變，必須要修正每個商品的位置看起來在購物車沒有隨便穿透或改變位置
 */
public class CartTrigger : MonoBehaviour {

    // 找到 class CardboardControl
    private static CardboardControl cardboard;
    // 準心對準的物體
    private static CardboardControlGaze gaze;
    // 準心對準的物體名稱
    private string GazeObjectName;


    bool PrintStr = true;

    // 
    static List<Product> Products;

    // 紀錄商品物體
    //public static ArrayList Product = new ArrayList();

    void Start () {
        // 找到 CardboardControlManager 中的 CardboardControl.cs Script
        cardboard = GameObject.Find("CardboardControlManager").GetComponent<CardboardControl>();

        // 準心改變對準的物體時
        cardboard.gaze.OnChange += CardboardGazeChange;
        // 準心持續對準某個物體時
        cardboard.gaze.OnStare += CardboardStare;

        Products = new List<Product>();
    }

    // 準心改變對準的物體時
    private void CardboardGazeChange(object sender) {
        // 準心對準的物體
        gaze = sender as CardboardControlGaze;
        // Debug 如果準心沒有對準任何東西，會設定對準目標名稱 = nothing
        GazeObjectName = gaze.IsHeld() ? gaze.Object().name : "nothing";
        Debug.Log("Change：" + GazeObjectName);

        //CheckGaze();
    }

    // 檢查準心是否對準購物車手把
    private void CheckGaze() {
        // 準心對準購物車手把時 gaze.IsHeld() = true，準心沒有對準時 = false
        // 購物車手把物件名稱為 Handle
        if (gaze.IsHeld() && GazeObjectName == "Handle") {
            SetProductPosition();
        }
    }

    // 紀錄購物車內所有商品的位置 Vector3 (x, y, z)
    public static void SetProductPosition() {
        // 購物車內有商品時，才紀錄所有商品的位置 Vector3 (x, y, z)
        if (Products.Count != 0) {
            foreach (Product Item in Products) {
                // 紀錄商品的位置 Vector3 (x, y, z)
                Products.Add(new Product() { ProductPosition = Item.ProductObject.transform.position });  
            }
        }
    }

    // 準心持續對準某個物體時
    private void CardboardStare(object sender) {
        gaze = sender as CardboardControlGaze;
        // Debug 如果準心沒有對準任何東西，會設定對準目標名稱 = nothing
        GazeObjectName = gaze.IsHeld() ? gaze.Object().name : "nothing";
        Debug.Log("Stare：" + GazeObjectName);
    }

    void Update () {
        if (PrintStr) {
            foreach (Product Item in Products) {
                //Debug.Log(Item.ProductObject);
            }
            PrintStr = false;
        }
    }

    // 商品只要進入觸發區域內，就會將商品物體紀錄至 List
    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Product")) {
            GameObject GameObj = other.gameObject;
            if (!Products.Contains(new Product { ProductObject = GameObj })) {
                Products.Add(new Product() { ProductObject = GameObj });
            }
        }
    }

    // 商品只要離開觸發區域，就會將 List 內的該商品物體刪除紀錄
    void OnTriggerExit(Collider other) {
        if (other.CompareTag("Product")) {
            GameObject GameObj = other.gameObject;
            Products.Remove(new Product() { ProductObject = GameObj });
        }
    }

    //void OnTriggerStay(Collider InCartProduct) {
    //    //Product.Add(InCartProduct.gameObject);
    //    foreach (GameObject GameObj in ListObjects) {
    //        if (GameObj.transform == InCartProduct.transform) {
    //            Product.Add(InCartProduct.gameObject);
    //        }
    //    }
    //}

    //public static ArrayList GetProduct() {
    //    return Product;
    //}
}
