using UnityEngine;
using System.Collections;

public class OnGroundProduct : MonoBehaviour {

    [Tooltip("商品 Tag")]
    public string ProductTag = "Product";

    [Tooltip("掉在地面上的商品會設定為此 Layer")]
    public string OnGround_Product = "OnGroundProduct";

    /// <summary>
    /// 商品掉在地面上時，會將商品的 Layer 設為 "OnGroundProduct"
    /// </summary>
    void OnTriggerEnter(Collider other) {
        if (other.tag == ProductTag) {
            //Debug.Log("Enter: " + other.gameObject.name);
            other.gameObject.layer = LayerMask.NameToLayer(OnGround_Product);
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
