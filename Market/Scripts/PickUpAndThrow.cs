using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class PickUpAndThrow : MonoBehaviour {
    [Tooltip("商品被拿取時的位置")]
    public Transform PickupPosition;

    [Tooltip("拿取商品時的力")]
    public float PickPower = 10.0f;

    [Tooltip("丟出商品時的力")]
    public float ThrowPower = 3.0f;

    [Tooltip("拿取時，商品與人物角色的距離")]
    public float RayDistance = 2.0f;

    [Tooltip("是否拿取商品")]
    public bool PickingUp = false;

    [Tooltip("商品與人物角色的距離是否超過設定範圍")]
    public bool PickUpOverRange = false;

    [Tooltip("是否按第二次 Gvr 按鈕")]
    public bool SecondClick = false;

    private static CardboardControl cardboard;
    /// <summary>
    /// 準心對準的物件
    /// </summary>
    private static CardboardControlGaze gaze;
    /// <summary>
    /// 準心對準的物件名稱
    /// </summary>
    private string GazeObjectName;
    /// <summary>
    /// 準心對準的物件名稱是否為商品(產品編號：ProObjxxxx)
    /// </summary>
    private bool GazeObjIsProduct;
    /// <summary>
    /// 商品物件的 EventTrigger
    /// </summary>
    private EventTrigger ProductGaze;

    private Transform MainCamera;
    /// <summary>
    /// 商品物件
    /// </summary>
    private Transform Product;
    /// <summary>
    /// 射線碰撞參數
    /// </summary>
    private RaycastHit hit;
    /// <summary>
    /// 射線對準的物件名稱
    /// </summary>
    private string RayHitObjectName;

    void Start() {
        // 找到 MainCamera
        MainCamera = Camera.main.transform;
        // 找到 CardboardControlManager 中的 CardboardControl.cs Script
        cardboard = GameObject.Find("CardboardControlManager").GetComponent<CardboardControl>();

        // 在設定時間內按下並且快速放開 Gvr 按鈕間，會判定為點擊事件觸發
        cardboard.trigger.OnClick += CardboardClick;
        // 準心改變對準的物件時
        cardboard.gaze.OnChange += CardboardGazeChange;
        // 準心持續對準某個物件時
        cardboard.gaze.OnStare += CardboardStare;
    }

    /// <summary>
    /// 檢查準心是否對準商品
    /// </summary>
    private void GazeCheck(object sender) {
        // 準心對準的物件
        gaze = sender as CardboardControlGaze;
        // Debug 如果準心沒有對準任何東西，會設定對準目標名稱 = nothing
        GazeObjectName = gaze.IsHeld() ? gaze.Object().name : "nothing";
        // 準心對準的物件名稱是否為商品(產品編號：ProObjxxxx)
        GazeObjIsProduct = GazeObjectName.Contains("ProObj");
    }

    /// <summary>
    /// 射線是否對準商品物件。
    /// hit.rigidbod：射線是否對準物件，
    /// IsHitProducts：射線對準的物件名稱是否為商品(產品編號：ProObjxxxx，
    /// xxxx 為邊號，EX：ProObj0001)
    /// </summary>
    private bool RayHitObjIsProduct() {
        return hit.rigidbody && hit.transform.name.Contains("ProObj");
    }

    /// <summary>
    /// 檢查商品與人物角色的距離是否在設定範圍內，
    /// 如果有可拿取商品(準心會放大)，沒有則無(準心不會放大)
    /// </summary>
    private void PickUpCheck() {
        // 準心是否對準物件 && 射線是否對準物件 && 準心對準的物件名稱是否為商品
        if (gaze.IsHeld() && GazeObjIsProduct) {
            RaycastHit Producthit;
            // 物理射線 (射線原點, 射線軸向, 射線碰撞參數, 射線距離)
            Physics.Raycast(MainCamera.position, MainCamera.forward, out Producthit, RayDistance);
            // 找到商品物件
            Product = GameObject.Find(GazeObjectName).transform;
            // 找到商品物件的 EventTrigger
            ProductGaze = Product.GetComponent<EventTrigger>();

            // 更新商品與人物角色的距離
            Vector3 Product_Player = Product.position - MainCamera.position;
            // 紀錄商品與人物角色的距離
            float Product_Player_Distance = Mathf.Sqrt(Mathf.Pow(Product_Player.x, 2) +
                                            Mathf.Pow(Product_Player.z, 2));
            // 商品與人物角色的距離是否超過設定範圍
            bool Product_Player_IsOverRange = Product_Player_Distance > RayDistance;
            //Debug.Log("有無超過設定範圍：" + Product_Player_IsOverRange);

            // 射線是否對準商品物件
            if (Producthit.rigidbody) {
                if (Producthit.transform.name.Contains("ProObj")) {
                    Debug.Log("CanPickup");
                    // 將 商品與人物角色的距離是否超過設定範圍 狀態改成 false
                    PickUpOverRange = false;
                    // 準心會放大
                    ProductGaze.enabled = true;
                } else {
                    Debug.Log("CantPickup");
                    // 將 商品與人物角色的距離是否超過設定範圍 狀態改成 true
                    PickUpOverRange = true;
                    // 準心不會放大
                    ProductGaze.enabled = false;
                }
            } else {
                Debug.Log("CantPickup");
                // 將 商品與人物角色的距離是否超過設定範圍 狀態改成 true
                PickUpOverRange = true;
                // 準心不會放大
                ProductGaze.enabled = false;
            }

            //// 如果商品與人物角色的距離在設定範圍內
            //// 準心對準商品時，準心會放大，而且可拿取商品
            //if (Product_Player_IsOverRange && !RayHitObjIsProduct()) {
            //    Debug.Log("CantPickup");
            //    // 將 商品與人物角色的距離是否超過設定範圍 狀態改成 true
            //    PickUpOverRange = true;
            //    // 準心不會放大
            //    ProductGaze.enabled = false;
                
            //// 如果商品與人物角色的距離超過設定範圍
            //// 準心對準商品時，準心不會放大，而且不可拿取商品
            //} else if (!Product_Player_IsOverRange && RayHitObjIsProduct()) {
            //    Debug.Log("CanPickup");
            //    // 將 商品與人物角色的距離是否超過設定範圍 狀態改成 false
            //    PickUpOverRange = false;
            //    // 準心會放大
            //    ProductGaze.enabled = true;
            //}

        // 準心沒有對準商品
        } else {
            // 將 商品與人物角色的距離是否超過設定範圍 狀態改成 false
            PickUpOverRange = false;
        }
    }

    /// <summary>
    /// 準心改變對準的物件時
    /// </summary>
    private void CardboardGazeChange(object sender) {
        // 檢查準心是否對準商品
        GazeCheck(sender);
        // 檢查商品與人物角色的距離是否在設定範圍內
        PickUpCheck();
    }

    /// <summary>
    /// 準心持續對準某個物件時
    /// </summary>
    private void CardboardStare(object sender) {
        // 檢查準心是否對準商品
        GazeCheck(sender);
        // 檢查商品與人物角色的距離是否在設定範圍內
        PickUpCheck();
    }

    /// <summary>
    /// 在設定時間內按下並且快速放開 Gvr 按鈕間，會判定為點擊事件觸發
    /// </summary> 
    private void CardboardClick(object sender) {
        // 物理射線 (射線原點, 射線軸向, 射線碰撞參數, 射線距離)
        Physics.Raycast(MainCamera.position, MainCamera.forward, out hit, RayDistance);

        // 按第一次 Gvr 按鈕，商品會跟著玩家頭部方向移動
        // 準心是否對準物件 && 射線是否對準物件 && 射線對準的物件名稱是否為 ProObjxxxx
        if (RayHitObjIsProduct() && !SecondClick) {
            Debug.Log("PickUp：" + hit.transform.name);
            // 將 是否拿取商品 狀態改成 true
            PickingUp = true;
            // 將 是否按第二次 Gvr 按鈕 狀態改成 true
            SecondClick = true;

        // 按第二次 Gvr 按鈕，丟出商品
        } else if (SecondClick) {
            Debug.Log("Throw");
            // 將 是否拿取商品 狀態改成 false
            PickingUp = false;
            // 將 是否按第二次 Gvr 按鈕 狀態改成 false
            SecondClick = false;
            // 丟出商品
            ThrowProduct();

        // 商品與人物角色的距離超過設定範圍
        } else if (PickUpOverRange) {
            Debug.Log("商品超過可拿取範圍");
        }
    }

    void Update() {
        // 畫出射線，可以知道射線的位置與方向
        // Debug.DrawRay(射線原點, 射線軸向 * 射線距離);
        Debug.DrawRay(MainCamera.position, MainCamera.forward * RayDistance);

        // 是否按一下 Gvr 按鈕拿取商品
        if (PickingUp) {
            // 商品跟著玩家頭部方向移動
            PickUpProduct();
        }
    }

    /// <summary>
    /// 商品跟著玩家頭部方向移動
    /// </summary>
    private void PickUpProduct() {
        // 準心是否對準物件 && 射線是否對準物件 && 射線對準的物件名稱是否為 ProObjxxxx
        if (hit.rigidbody) {
            // 商品會跟著玩家頭部方向移動
            hit.rigidbody.velocity = (PickupPosition.position -
                    (hit.transform.position + hit.rigidbody.centerOfMass)) * Time.deltaTime * PickPower;
        }
    }

    /// <summary>
    /// 丟出商品
    /// </summary>
    private void ThrowProduct() {
        // 準心是否對準物件 && 射線是否對準物件
        if (gaze.IsHeld() && hit.rigidbody) {
            // 丟出商品
            hit.rigidbody.velocity = MainCamera.forward * ThrowPower;
        }
    }
}