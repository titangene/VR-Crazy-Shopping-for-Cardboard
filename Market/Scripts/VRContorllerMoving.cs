using UnityEngine;
using System.Collections;

public class VRContorllerMoving : MonoBehaviour {
    [Tooltip("商品被拿取時的位置")]
    public Transform PickupPosition;

    [Tooltip("找到 Head 物件")]
    public Transform Head;

    [Tooltip("找到 GvrViewer")]
    public GvrViewer GvrMain;

    [Tooltip("找到玩家角色物件")]
    public Transform Player;

    [Tooltip("找到購物車物件")]
    public Transform Cart;

    [Tooltip("找到購物車手把物件")]
    public Transform Handle;

    [Tooltip("找到 Cart/InCartProduct 子物件 (拿來放所有放在購物車內的商品)")]
    public Transform InCartProduct;

    [Tooltip("購物車與人物角色射線的距離")]
    public float RayDistance = 0.7f;

    [Tooltip("向前移動速度")]
    public float speed = 4.0f;

    [Tooltip("是否按住 Gvr 按鈕")]
    public bool HoldTrigger = false;

    [Tooltip("準心是否對準購物車手把")]
    public bool GazeCart = false;

    [Tooltip("是否向前移動")]
    public bool MoveForward = false;

    private static CardboardControl cardboard;
    private static CardboardControlGaze gaze;
    // 準心對準的物體名稱
    private string GazeObjectName;
    // 角色
    private CharacterController controller;

    private Transform MainCamera;
    // 射線碰撞參數
    private RaycastHit hit;
    // 射線碰撞的物體s
    private Transform HitObject;
    // 購物車剛體
    private Rigidbody CartRbody;
    // 是否可以產生射線
    private bool ProduceRay = true;

    void Start() {
        // 找到 MainCamera
        MainCamera = Camera.main.transform;
        // 找到 CharacterController
        controller = GetComponent<CharacterController>();
        // 找到 CardboardControlManager 中的 CardboardControl.cs Script
        cardboard = GameObject.Find("CardboardControlManager").GetComponent<CardboardControl>();

        // 按住 Gvr 按鈕時
        cardboard.trigger.OnLongClick += CardboardLongClick;
        // 放開 Gvr 按鈕時，玩家和購物車同時停止向前移動
        cardboard.trigger.OnUp += CardboardUp;
        // 準心改變對準的物體時
        cardboard.gaze.OnChange += CardboardGazeChange;
        // 準心持續對準某個物體時
        cardboard.gaze.OnStare += CardboardStare;
    }

    /// <summary>
    /// 準心改變對準的物體時
    /// </summary>
    private void CardboardGazeChange(object sender) {
        // 準心對準的物體
        gaze = sender as CardboardControlGaze;
        // Debug 如果準心沒有對準任何東西，會設定對準目標名稱 = nothing
        GazeObjectName = gaze.IsHeld() ? gaze.Object().name : "nothing";
    }

    /// <summary>
    /// 準心持續對準某個物體時
    /// </summary>
    private void CardboardStare(object sender) {
        // 準心對準的物體
        gaze = sender as CardboardControlGaze;
        // Debug 如果準心沒有對準任何東西，會設定對準目標名稱 = nothing
        GazeObjectName = gaze.IsHeld() ? gaze.Object().name : "nothing";
    }

    /// <summary>
    /// 按下 Gvr 按鈕時
    /// </summary>
    private void CardboardLongClick(object sender) {
        // 將 按住 Gvr 按鈕 狀態改成 true
        HoldTrigger = true;

        Debug.Log("按住 Gvr 按鈕");

        // 是否可以產生射線 && 準心是否對準購物車手把 && 準心是否對準購物車手把
        if (ProduceRay && gaze.IsHeld() && GazeObjectName.Contains(Handle.name)) {
            // 物理射線 (射線原點, 射線軸向, 射線碰撞參數, 射線距離)
            Physics.Raycast(MainCamera.position, MainCamera.forward, out hit, RayDistance);
            // 將 是否可以產生射線 狀態改成 false
            ProduceRay = false;
            // 準心對準購物車手把 狀態改成 true
            GazeCart = true;

            if (hit.rigidbody) {
                // 將 向前移動 狀態改成 true，玩家和購物車同時向前移動
                MoveForward = true;
            }
        }
    }

    /// <summary>
    /// 放開 Gvr 按鈕時，玩家和購物車同時停止向前移動
    /// </summary>
    private void CardboardUp(object sender) {
        if (MoveForward) {
            Debug.Log("Stop");
            // 將 按住 Gvr 按鈕 狀態改成 false
            HoldTrigger = false;
            // 準心對準購物車手把 狀態改成 false
            GazeCart = false;
            // 將 向前移動 狀態改成 false，玩家和購物車同時停止向前移動
            MoveForward = false;
        }
    }

    void Update() {
        Debug.DrawRay(MainCamera.position, MainCamera.forward * RayDistance);

        // 向前移動 狀態 = true，玩家和購物車同時向前移動
        if (MoveForward) {
            // 玩家向前移動
            //PlayerMove();
            // 購物車跟著玩家移動
            CartMove();
        }
    }

    /// <summary>
    /// 玩家向前移動
    /// </summary>
    private void PlayerMove() {
        // 找到向前的方向
        Vector3 forward = Camera.main.transform.forward;
        // 讓角色往前
        controller.SimpleMove(forward * speed);
    }

    /// <summary>
    /// 購物車跟著玩家移動
    /// </summary>
    private void CartMove() {

        //Rigidbody HitObjectRbody = HitObject.GetComponent<Rigidbody>();

        // 攝影機 Y 軸旋轉角度 (最高只能 90 度，用於theta)
        float Camera_AngleY = Head.transform.eulerAngles.y;
        // Mathf.Deg2Rad 度轉弧度 = (PI * 2) / 360
        float theta = Camera_AngleY * Mathf.Deg2Rad;
        // 紀錄購物車 X 座標：x = r * sin(thita)
        float Cart_X = 1.2f * Mathf.Sin(theta);
        // 紀錄購物車 Z 座標：z = r * cos(thita)
        float Cart_Z = 1.2f * Mathf.Cos(theta);

        //if (HitObjectRbody) {
        //    CartRbody = Cart.GetComponent<Rigidbody>();
        //    CartRbody.velocity = new Vector3(Cart_X + GvrMain.transform.position.x, 0f,
        //                            Cart_Z + GvrMain.transform.position.z);
        //}
        //Debug.Log(hit.transform.name);

        Debug.Log(hit.rigidbody);


        if (hit.rigidbody) {
            CartRbody = Cart.GetComponent<Rigidbody>();

            CartRbody.constraints = RigidbodyConstraints.FreezeAll;

            CartRbody.velocity = (PickupPosition.position -
                    (hit.transform.position + CartRbody.centerOfMass));
        }
    }
}
