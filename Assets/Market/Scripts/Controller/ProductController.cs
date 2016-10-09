using UnityEngine;
using System.Collections;

public class ProductController : MonoBehaviour {

    public void PickupAndThrow_Product() {
        PickUpAndThrowController.Instance.CheckClick();
    }
}
