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
    private static CardboardControlGaze gaze;
    // 準心對準的物體名稱
    private string GazeObjectName;
    // 找到商品物件的 EventTrigger
    private EventTrigger ProductGaze;

    private Transform MainCamera;
    // 射線碰撞參數
    private RaycastHit hit;
    // 某物體的剛體
    private Rigidbody Product_rbody;
    // 找到商品物件
    private Transform Product;

    void Start() {
        // 找到 MainCamera
        MainCamera = Camera.main.transform;
        // 找到 CardboardControlManager 中的 CardboardControl.cs Script
        cardboard = GameObject.Find("CardboardControlManager").GetComponent<CardboardControl>();

        // 在設定時間內按下並且快速放開 Gvr 按鈕間，會判定為點擊事件觸發
        cardboard.trigger.OnClick += CardboardClick;
        // 準心改變對準的物體時
        cardboard.gaze.OnChange += CardboardGazeChange;
        // 準心持續對準某個物體時
        cardboard.gaze.OnStare += CardboardStare;
    }

    /// <summary>
    /// hit.rigidbod：射線是否對準物體，
    /// hit.transform.name.Contains("ProObj")：射線對準的物體名稱是否為 ProObjxxxx。
    /// 商品物件名稱為 ProObjxxxx (xxxx 為邊號，EX：ProObj0001)
    /// </summary>
    private bool GazeCheck() {
        return hit.rigidbody && hit.transform.name.Contains("ProObj");
    }

    /// <summary>
    /// 檢查商品與人物角色的距離是否在設定範圍內，
    /// 如果有可拿取商品(準心會放大)，沒有則無(準心不會放大)
    /// </summary>
    private void PickUpCheck(object sender) {
        // 準心對準的物體
        gaze = sender as CardboardControlGaze;
        // Debug 如果準心沒有對準任何東西，會設定對準目標名稱 = nothing
        GazeObjectName = gaze.IsHeld() ? gaze.Object().name : "nothing";

        // 準心是否對準物體 && 射線是否對準物體 && 射線對準的物體名稱是否為 ProObjxxxx
        if (gaze.IsHeld() && GazeObjectName.Contains("ProObj")) {
            // 找到商品物件
            Product = GameObject.Find(GazeObjectName).transform;
            // 找到商品物件的 EventTrigger
            ProductGaze = Product.GetComponent<EventTrigger>();
        }

        // 準心是否對準物體 && 射線是否對準物體 && 射線對準的物體名稱是否為 ProObjxxxx
        if (gaze.IsHeld() && GazeObjectName.Contains("ProObj")) {
            // 更新商品與人物角色的距離
            Vector3 Product_Player_Distance = Product.position - transform.position;
            // 紀錄商品與人物角色的距離
            float Product_Player = Mathf.Sqrt(Mathf.Pow(Product_Player_Distance.x, 2) +
                                   Mathf.Pow(Product_Player_Distance.z, 2));
            //Debug.Log(Product_Player);

            // 如果商品與人物角色的距離在設定範圍內
            // 準心對準商品時，準心會放大，而且可拿取商品
            if (Product_Player > RayDistance) {
            // 將 商品與人物角色的距離是否超過設定範圍 狀態改成 true
            PickUpOverRange = true;
            // 準心不會放大
            ProductGaze.enabled = false;

            // 如果商品與人物角色的距離超過設定範圍
            // 準心對準商品時，準心不會放大，而且不可拿取商品
            } else {
                // 將 商品與人物角色的距離是否超過設定範圍 狀態改成 false
                PickUpOverRange = false;
                // 準心會放大
                ProductGaze.enabled = true;
            }

        // 準心沒有對準商品
        } else {
            // 將 商品與人物角色的距離是否超過設定範圍 狀態改成 false
            PickUpOverRange = false;
        }
    }

    /// <summary>
    /// 準心改變對準的物體時
    /// </summary>
    private void CardboardGazeChange(object sender) {
        // 檢查商品與人物角色的距離是否在設定範圍內
        PickUpCheck(sender);
    }

    /// <summary>
    /// 準心持續對準某個物體時
    /// </summary>
    private void CardboardStare(object sender) {
        // 檢查商品與人物角色的距離是否在設定範圍內
        PickUpCheck(sender);
    }

    /// <summary>
    /// 在設定時間內按下並且快速放開 Gvr 按鈕間，會判定為點擊事件觸發
    /// </summary> 
    private void CardboardClick(object sender) {
        // 物理射線 (射線原點, 射線軸向, 射線碰撞參數, 射線距離)
        Physics.Raycast(MainCamera.position, MainCamera.forward, out hit, RayDistance);

        // 按第一次 Gvr 按鈕，商品會跟著玩家頭部方向移動
        // 準心是否對準物體 && 射線是否對準物體 && 射線對準的物體名稱是否為 ProObjxxxx
        if (GazeCheck() && !SecondClick) {
            Debug.Log("PickingUp");
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
        // 準心是否對準物體 && 射線是否對準物體 && 射線對準的物體名稱是否為 ProObjxxxx
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
        // 準心是否對準物體 && 射線是否對準物體
        if (gaze.IsHeld() && hit.rigidbody) {
            // 丟出商品
            hit.rigidbody.velocity = MainCamera.forward * ThrowPower;
        }
    }
}