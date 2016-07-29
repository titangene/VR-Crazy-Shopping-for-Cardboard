using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* 讓人物角色以第一人稱視角向前移動，並且購物車會同時移動
 * (購物車與人物角色的距離不變)
 * 準心對準購物車的手把並按住 Gvr 按鈕即可往前移動，放開按鈕可停止移動
 * 玩家同時能透過移動頭部方向來改變移動方位
 */
public class MovingController : MonoBehaviour {
    public GvrViewer GvrMain;
    public Transform Head;
    public Transform Cart;
    public GameObject Range;

    [Tooltip("Cart/InCartProduct 子物件 (拿來放丟入購物車內的所有商品)")]
    public Transform InCartProductObj;

    [Tooltip("找出所有是 InCartProduct Layer 的商品物件")]
    public string LayerName_InCartProduct = "InCartProduct";

    [Tooltip("購物車與人物角色的距離")]
    public float Cart_Player = 1.2f;

    [Tooltip("向前移動速度")]
    public float speed = 6.0f;

    [Tooltip("是否按住 Gvr 按鈕")]
    public bool IsHoldTrigger = false;

    [Tooltip("準心是否對準購物車手把")]
    public bool GazeObjIsHandle = false;

    [Tooltip("是否向前移動")]
    public bool IsMovingForward = false;

    private static CardboardControl cardboard;
    private static CardboardControlGaze gaze;
    /// <summary>
    /// 準心對準的物件名稱
    /// </summary>
    private string GazeObjectName;
    /// <summary>
    /// 角色
    /// </summary>
    private CharacterController controller;
    /// <summary>
    /// 找出 Layer
    /// </summary>
    private FindLayer findLayer;
    // 計算購物車與人物角色的距離
    //private Vector3 Cart_Player_Distance;
    // 紀錄購物車與人物角色的距離
    //private float Cart_Player;
    // 購物車物件的剛體
    //private Rigidbody Cart_rbody;

    void Start() {
        // 找到購物車物件
        Cart = GameObject.Find("Cart").transform;
        // 找到購物車物件的鋼體
        //Cart_rbody = Cart.GetComponent<Rigidbody>();
        // 找到 Cart/InCartProduct 子物件 (拿來放丟入購物車內的所有商品)
        InCartProductObj = GameObject.Find("InCartProduct").transform;
        // 找到 Head 物件
        Head = GvrMain.transform.FindChild("Head");
        // 找到 CharacterController
        controller = GetComponent<CharacterController>();
        // 找出 Layer
        findLayer = gameObject.GetComponent<FindLayer>();

        cardboard = GameObject.Find("CardboardControlManager").GetComponent<CardboardControl>();
        
        // 按住 Gvr 按鈕時
        cardboard.trigger.OnLongClick += CardboardLongClick;
        // 放開 Gvr 按鈕時，玩家和購物車同時停止向前移動
        cardboard.trigger.OnUp += CardboardUp;
        // 準心改變對準的物件時
        cardboard.gaze.OnChange += CardboardGazeChange;
        // 準心持續對準某個物件時
        cardboard.gaze.OnStare += CardboardStare;
    }

    /// <summary>
    /// 準心改變對準的物件時
    /// </summary>
    private void CardboardGazeChange(object sender) {
        // 準心對準的物件
        gaze = sender as CardboardControlGaze;
        // Debug 如果準心沒有對準任何東西，會設定對準目標名稱 = nothing
        GazeObjectName = gaze.IsHeld() ? gaze.Object().name : "nothing";
        // 準心是否對準購物車手把，購物車手把物件名稱為 Handle
        GazeObjIsHandle = (GazeObjectName == "Handle");
    }

    /// <summary>
    /// 準心持續對準某個物件時
    /// </summary>
    private void CardboardStare(object sender) {
        gaze = sender as CardboardControlGaze;
        // Debug 如果準心沒有對準任何東西，會設定對準目標名稱 = nothing
        GazeObjectName = gaze.IsHeld() ? gaze.Object().name : "nothing";
        // 準心是否對準購物車手把，購物車手把物件名稱為 Handle
        GazeObjIsHandle = (GazeObjectName == "Handle");
    }

    /// <summary>
    /// 按下 Gvr 按鈕時
    /// </summary>
    private void CardboardLongClick(object sender) {
        //Debug.Log("按住 Gvr 按鈕");

        // 準心對準購物車手把時 gaze.IsHeld() = true，準心沒有對準時 = false
        if (gaze.IsHeld() && GazeObjIsHandle) {
            //Debug.Log("Moving");
            // 將 向前移動 狀態改成 true，玩家和購物車同時向前移動
            IsMovingForward = true;

            // 防止購物車移動時，商品全部穿透掉光
            // 將所有是 "InCartProduct" Layer 的商品物件放入 Cart/InCartProduct 子物件內
            findLayer.PlacedObjectParent(LayerName_InCartProduct, InCartProductObj);
            // 將 Range 物件開啟
            Range.SetActive(true);
        }
    }

    /// <summary>
    /// 放開 Gvr 按鈕時，玩家和購物車同時停止向前移動
    /// </summary>
    private void CardboardUp(object sender) {
        if (IsMovingForward) {
            //Debug.Log("Stop");
            // 將 向前移動 狀態改成 false，玩家和購物車同時停止向前移動
            IsMovingForward = false;
            // 將 Range 物件關閉
            Range.SetActive(false);
        }
    }

    void Update() {
        // 是否按住 Gvr 按鈕
        IsHoldTrigger = cardboard.trigger.IsLongClick;
        // 向前移動 狀態 = true，玩家和購物車同時向前移動
        if (IsMovingForward) {
            // 玩家向前移動
            PlayerMove();
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
        #region 計算購物車與人物角色的距離(暫時用不到)
        // 更新購物車與人物角色的距離
        //Cart_Player_Distance = Cart.position - transform.position;
        // 紀錄購物車與人物角色的距離 (r 圓型半徑)   畢氏定理 C = sqrt(A^2 + B^2)
        //Cart_Player = Mathf.Sqrt(Mathf.Pow(Cart_Player_Distance.x, 2) + Mathf.Pow(Cart_Player_Distance.z, 2));
        //if (Cart_Player != 1.5f) {
        //    Cart_Player = 1.5f;
        //}
        #endregion

        // 攝影機 Y 軸旋轉角度 (最高只能 90 度，用於theta)
        float Camera_AngleY = Head.transform.eulerAngles.y;
        // Mathf.Deg2Rad 度轉弧度 = (PI * 2) / 360
        float theta = Camera_AngleY * Mathf.Deg2Rad;
        // 紀錄購物車 X 座標：x = r * sin(thita)
        float Cart_X = Cart_Player * Mathf.Sin(theta);
        // 紀錄購物車 Z 座標：z = r * cos(thita)
        float Cart_Z = Cart_Player * Mathf.Cos(theta);

        // 購物車會跟著玩家的視角移動位置
        Cart.position = new Vector3(Cart_X + GvrMain.transform.position.x, 0f,
                                    Cart_Z + GvrMain.transform.position.z);
        // 購物車會跟著玩家的視角旋轉角度
        Cart.rotation = Quaternion.Euler(0, Camera_AngleY, 0);

        #region 剛體移動(暫時用不到)
        //Vector3 CartMove = new Vector3(Cart_X, 0f, Cart_Z);
        //Cart_rbody.AddForce(CartMove);
        // 購物車利用剛體移動
        //Cart_rbody.AddForce(Cart.forward);
        // 購物車移動到的位置
        //Cart.position = new Vector3(Cart.position.x, 0f, Cart.position.z);
        //Debug.Log(Cart.forward);
        #endregion
    }
}
