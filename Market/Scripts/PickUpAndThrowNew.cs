using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Linq;

public class PickUpAndThrowNew : MonoBehaviour {
    public GameObject Cart_Collider;
    public Collider Basket_Collider;

    [Tooltip("商品被拿取時的位置")]
    public Transform PickupPosition;

    public string ProductTagName = "Product";

    [Tooltip("拿取商品時的力")]
    public float PickPower = 10.0f;

    [Tooltip("丟出商品時的力")]
    public float ThrowPower = 3.0f;

    [Tooltip("拿取時，商品與人物角色的距離")]
    public float RayDistance = 2.8f;

    [Tooltip("是否拿取商品")]
    public bool PickingUp = false;

    [Tooltip("商品是否超過可拿取範圍")]
    public bool PickUpIsOverRange = false;

    [Tooltip("是否按第二次 Gvr 按鈕")]
    public bool SecondClick = false;

    private static CardboardControl cardboard;
    private static CardboardControlGaze gaze;
    /// <summary>
    /// 準心對準的物件名稱
    /// </summary>
    private string GazeObjectName;

    private Transform MainCamera;
    /// <summary>
    /// 商品物件
    /// </summary>
    private Transform Product;
    /// <summary>
    /// 商品物件的 EventTrigger
    /// </summary>
    private EventTrigger ProductGazeSwitch;
    /// <summary>
    /// 射線碰撞參數(PickupRange)
    /// </summary>
    private RaycastHit hit;

    #region 可拿取已丟入購物車內的商品
    /// <summary>
    /// 回傳射線對準的所有物件及訊息
    /// </summary>
    private RaycastHit[] RaycastAllHits;
    /// <summary>
    /// 紀錄第一個射線對準的商品物件
    /// </summary>
    private GameObject AllHitsFirstProductObj;
    /// <summary>
    /// 可紀錄射線對準的商品物件
    /// </summary>
    private bool CanSaveProductObj = true;
    /// <summary>
    /// 計算射線對準的所有商品物件數量
    /// </summary>
    private int CountProductObj = 0;
    #endregion

    void Start() {
        MainCamera = Camera.main.transform;
        cardboard = GameObject.Find("CardboardControlManager").GetComponent<CardboardControl>();

        // 自定準心觀看範圍
        cardboard.gaze.DistanceRange = RayDistance;
        // 準心對準物件時(有範圍限制，利用 cardboard.gaze.DistanceRange 自定準心觀看範圍)
        cardboard.gaze.OnUpdate += CardboardGazeUpdate;
        // 在設定時間內按下並且快速放開 Gvr 按鈕間，會判定為點擊事件觸發
        cardboard.trigger.OnClick += CardboardClick;
    }

    /// <summary>
    /// 設定準心對準的物件名稱、準心是否對準商品
    /// </summary>
    private void SetGaze(object sender) {
        // 準心對準的物件
        gaze = sender as CardboardControlGaze;
        // 如果準心沒有對準任何東西，會設定對準目標名稱 = nothing (無限範圍)
        GazeObjectName = gaze.IsHeld() ? gaze.Object().name : "nothing";
    }

    /// <summary>
    /// 準心是否對準商品 (有限範圍)
    /// </summary>
    private bool GazeObjIsProduct() {
        return gaze.IsHeldRange() && gaze.ObjectRange().name.Contains("ProObj");
    }

    /// <summary>
    /// 檢查商品是否在可拿取範圍內。
    /// Can pickup：準心會放大，Can't pickup：準心不會放大
    /// </summary>
    private void PickUpCheck() {
        // 準心是否對準物件 && 準心是否對準商品 (無限範圍)
        if (gaze.IsHeld() && gaze.Object().name.Contains("ProObj")) {
            // 找到商品物件
            Product = GameObject.Find(GazeObjectName).transform;
            // 找到商品物件的 EventTrigger
            ProductGazeSwitch = Product.GetComponent<EventTrigger>();

            // 準心是否對準物件 && 準心是否對準商品 (有限範圍)
            if (gaze.IsHeldRange() && gaze.ObjectRange().name.Contains("ProObj")) {
                //Debug.Log("Can Pickup");
                // 將 商品是否超過可拿取範圍 狀態改成 false
                PickUpIsOverRange = false;
                // 準心會放大
                ProductGazeSwitch.enabled = true;

            } else {
                //Debug.Log("Cant Pickup");
                // 將 商品是否超過可拿取範圍 狀態改成 true
                PickUpIsOverRange = true;
                // 準心不會放大
                ProductGazeSwitch.enabled = false;
            }
        } else {
            // 將 商品是否超過可拿取範圍 狀態改成 false
            PickUpIsOverRange = false;
        }
    }

    /// <summary>
    /// 準心對準物件時(有範圍限制，利用 cardboard.gaze.DistanceRange 自定準心觀看範圍)
    /// </summary>
    private void CardboardGazeUpdate(object sender) {
        // 檢查準心是否對準商品
        SetGaze(sender);
        // 檢查商品與人物角色的距離是否在設定範圍內，如果在範圍內才可拿取商品
        PickUpCheck();
    }
    
    /// <summary>
    /// 在設定時間內按下並且快速放開 Gvr 按鈕間，會判定為點擊事件觸發
    /// </summary> 
    private void CardboardClick(object sender) {
        // 物理射線 (射線原點, 射線軸向, 射線碰撞參數, 射線距離)
        Physics.Raycast(gaze.Ray(), out hit, RayDistance);

        //紀錄第一個射線對準的商品物件
        SaveRaycastAllHitsFirstProductObj();

        // 按第一次 Gvr 按鈕，商品會跟著玩家頭部方向移動
        // 商品是否超過可拿取範圍 && 準心是否對準商品 (有限範圍) && 按第一次 Gvr 按鈕：拿取商品
        if (!PickUpIsOverRange && (GazeObjIsProduct() || AllHitsFirstProductObj != null) && !SecondClick) {
            //Debug.Log("PickUp：" + hit.transform.name);
            // 將 是否拿取商品 狀態改成 true
            PickingUp = true;
            // 將 是否按第二次 Gvr 按鈕 狀態改成 true
            SecondClick = true;
            // 將 Cart_Collider 物件開啟
            Cart_Collider.SetActive(true);

            // 按第二次 Gvr 按鈕：丟出商品
        } else if (SecondClick) {
            //Debug.Log("Throw");
            // 將 是否拿取商品 狀態改成 false
            PickingUp = false;
            // 將 是否按第二次 Gvr 按鈕 狀態改成 false
            SecondClick = false;
            // 丟出商品
            ThrowProduct();
            // 將 Cart_Collider 物件關閉
            Cart_Collider.SetActive(false);

            // 將 可紀錄射線對準的商品物件 狀態改成 true
            CanSaveProductObj = true;
            // 將第一個射線對準的商品物件紀錄刪除
            AllHitsFirstProductObj = null;
        }

        // 商品與人物角色的距離超過設定範圍
        if (PickUpIsOverRange)
            Debug.Log("商品超過可拿取範圍");
    }

    void FixedUpdate() {
        // 是否按一下 Gvr 按鈕拿取商品
        if (PickingUp) {
            // 商品跟著玩家頭部方向移動
            PickUpProduct();
        }
    }

    /// <summary>
    /// 紀錄第一個射線對準的商品物件
    /// </summary>
    private void SaveRaycastAllHitsFirstProductObj() {
        // 回傳射線對準的所有物件及訊息
        RaycastAllHits = Physics.RaycastAll(gaze.Ray(), RayDistance).OrderBy(h => h.distance).ToArray();
        // 取出的 hits 訊息將會由近至遠排列
        foreach (RaycastHit hits in RaycastAllHits) {
            // 射線是否對準商品
            if (hits.transform.name.Contains("ProObj")) {
                // 計算射線對準的所有商品物件數量
                CountProductObj++;

            } else {
                // 將 可紀錄射線對準的商品物件 狀態改成 true
                CanSaveProductObj = true;
            }

            // 可紀錄射線對準的商品物件 && 射線是否對準商品
            if (CanSaveProductObj && hits.transform.name.Contains("ProObj")) {
                // 紀錄第一個射線對準的商品物件
                AllHitsFirstProductObj = hits.collider.gameObject;
                // 將 可紀錄射線對準的商品物件 狀態改成 false
                CanSaveProductObj = false;
            }

            // 不是 Collider、Ground、Plane 物件才可列印訊息
            if (!hits.transform.name.Contains("Collider") &&
                !hits.transform.name.Contains("Ground") &&
                !hits.transform.name.Contains("Plane")) {
                Debug.Log(hits.transform.name);
            }
        }
        // 如果射線沒有對準商品，將第一個射線對準的商品物件紀錄刪除
        if (CountProductObj == 0) {
            AllHitsFirstProductObj = null;
        }
        // 將 計算射線對準的所有商品物件數量 歸零
        CountProductObj = 0;
        Debug.Log("AllHitsProductObj：" + AllHitsFirstProductObj);
    }


    /// <summary>
    /// 拿取商品
    /// </summary>
    private void PickUpProduct() {
        // 射線是否對準物件
        if (hit.rigidbody) {
            // 射線是否對準商品
            if (hit.transform.name.Contains("ProObj")) {
                // 商品會跟著玩家頭部方向移動
                hit.rigidbody.velocity = (PickupPosition.position -
                        (hit.transform.position + hit.rigidbody.centerOfMass)) * PickPower;
            } else if (AllHitsFirstProductObj != null) {
                hit.rigidbody.velocity = (PickupPosition.position -
                        (hit.transform.position + hit.rigidbody.centerOfMass)) * PickPower;
            }
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