using UnityEngine;

public class InCartProduct : MonoBehaviour {

    [Tooltip("商品 Tag")]
    public string ProductTag = "Product";

    [Tooltip("放入購物車內的商品會設定為此 Layer")]
    public string LayerName_InCartProduct = "InCartProduct";

    /// <summary>
    /// 商品放入購物車時，會將商品的 Layer 設為 "InCartProduct"
    /// </summary>
    void OnTriggerEnter(Collider other) {
        if (other.tag == ProductTag) {
            //Debug.Log("Enter: " + other.gameObject.name);
            other.gameObject.layer = LayerMask.NameToLayer(LayerName_InCartProduct);
        }
    }

    /// <summary>
    /// 商品離開購物車時，會將商品的 Layer 設為 預設
    /// </summary>
    void OnTriggerExit(Collider other) {
        if (other.tag == ProductTag) {
            //Debug.Log("Exit: " + other.gameObject.name);
            other.gameObject.layer = 0;
        }   
    }
}
