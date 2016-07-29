using UnityEngine;
using System.Collections;

public class OnGroundProduct : MonoBehaviour {

    [Tooltip("商品 Tag")]
    public string ProductTag = "Product";

    [Tooltip("掉在地面上的商品會設定為此 Layer")]
    public string LayerName_OnGroundProduct = "OnGroundProduct";

    [Tooltip("OnGroundProductObj 子物件 (拿來放掉在地面上的所有商品)")]
    public Transform OnGroundProductObj;

    /// <summary>
    /// 找出 Layer
    /// </summary>
    private Find find;

    void Start() {
        // 找出 Layer
        find = gameObject.GetComponent<Find>();
    }

    /// <summary>
    /// 商品掉在地面上時，會將商品的 Layer 設為 "OnGroundProduct"，
    /// 並且將掉在地面上的所有商品放在 OnGroundProduct 子物件內
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
    /// 商品離開地面時，會將商品的 Layer 設為 預設
    /// </summary>
    void OnTriggerExit(Collider other) {
        if (other.tag == ProductTag) {
            //Debug.Log("Exit: " + other.gameObject.name);
            other.gameObject.layer = 0;
        }
    }
}
