using UnityEngine;

public class ProductController : MonoBehaviour {
    public void PickupAndThrow_Product() {
        PickUpAndThrowController.Instance.CheckClick();
    }
}
