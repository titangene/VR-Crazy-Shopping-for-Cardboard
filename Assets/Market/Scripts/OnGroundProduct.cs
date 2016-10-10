using UnityEngine;

public class OnGroundProduct : MonoBehaviour {
    /// <summary>
    /// 商品 Tag
    /// </summary>
    public string ProductTag = "Product";
    /// <summary>
    /// 掉在地面上的商品會設定為此 Layer
    /// </summary>
    public string LayerName_OnGroundProduct = "OnGroundProduct";
    /// <summary>
    /// OnGroundProductObj 子物件 (拿來放掉在地面上的所有商品)
    /// </summary>
    public Transform OnGroundProductObj;

    /// <summary>
    /// 找出 Layer / Tag
    /// </summary>
    private Find find;

    void Start() {
        // 找出 Layer / Tag
        find = GameObject.FindWithTag("Player").GetComponent<Find>();
    }

    /// <summary>
    /// 商品掉在地面上時
    /// 1. 將商品的 Layer 設為 "OnGroundProduct"，
    /// 2. 將掉在地面上的所有商品放在 OnGroundProduct 子物件內，
    /// 3. 將該商品的 Collider 關閉，
    /// 4. 將該商品的 Rigidbody useGravity 關閉
    /// </summary>
    void OnTriggerEnter(Collider other) {
        if (other.tag == ProductTag) {
            //Debug.Log("Enter: " + other.gameObject.name);
            other.gameObject.layer = LayerMask.NameToLayer(LayerName_OnGroundProduct);
            // 將所有是 "OnGroundProduct" Layer 的商品物件放入 OnGroundProduct 子物件內
            find.PlacedObjectParent(LayerName_OnGroundProduct, OnGroundProductObj);
        }
    }

    /// <summary>
    /// 商品離開地面時，
    /// 1. 將商品的 Layer 設為 預設，
    /// 2. 將該商品的 Collider 開啟，
    /// 3. 將該商品的 Rigidbody useGravity 開啟
    /// </summary>
    void OnTriggerExit(Collider other) {
        if (other.tag == ProductTag) {
            //Debug.Log("Exit: " + other.gameObject.name);
            other.gameObject.layer = 0;
        }
    }
}
