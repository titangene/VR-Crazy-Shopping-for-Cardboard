using UnityEngine;

public class PlayerAndCartMoveController : MonoBehaviour {
    /// <summary>
    /// 購物車與人物角色的距離
    /// </summary>
    public float Cart_Player = 1f;
    /// <summary>
    /// 向前移動速度
    /// </summary>
    public float speed = 5.5f;
    /// <summary>
    /// 找出所有是 InCartProduct Layer 的商品物件
    /// </summary>
    public string LayerName_InCartProduct = "InCartProduct";
    /// <summary>
    /// 是否向前移動
    /// </summary>
    public bool IsMovingForward = false;

    private GCvrGaze GCvrGaze;
    private GCvrTrigger GCvrTrigger;
    /// <summary>
    /// 找出 Layer / Tag
    /// </summary>
    private Find find;
    private Camera cam;

    /// <summary>
    /// 角色
    /// </summary>
    private CharacterController controller;
    /// <summary>
    /// 找出 Layer / Tag
    /// </summary>
    private Transform Cart;
    /// <summary>
    /// Cart/InCartProduct 子物件 (拿來放丟入購物車內的所有商品)
    /// </summary>
    private Transform InCartProductObj;
    private GameObject OnGroundProduct;
    private Collider Basket_Collider;
    private GameObject Range;

    private bool DebugLogPrint = true;

    void Start () {
        cam = Camera.main;
        GCvrGaze = cam.GetComponent<GCvrGaze>();
        GCvrTrigger = cam.GetComponent<GCvrTrigger>();

        GameObject Player = GameObject.FindWithTag("Player");
        find = Player.GetComponent<Find>();
        controller = Player.GetComponent<CharacterController>();
        Cart = GameObject.FindWithTag("Cart").transform;
        Basket_Collider = GameObject.FindWithTag("Basket").GetComponent<Collider>();
        InCartProductObj = GameObject.FindWithTag("InCartProduct").transform;
        OnGroundProduct = GameObject.FindWithTag("OnGroundProduct");
        Range = GameObject.FindWithTag("Range");

        // Gvr 按鈕事件
        GCvrTrigger.OnLongClick += GCvrLongClick;
    }

	void Update () {
        // 向前移動 狀態 = true，玩家和購物車同時向前移動
        if (IsMovingForward) {
            // 玩家向前移動
            PlayerMove();
            // 購物車跟著玩家移動
            CartMove();
        }
    }

    private void GCvrLongClick(object sender) {
        // 準心是否對準購物車手把
        bool GazeObjIsHandle = GCvrGaze.GetObjName(GCvrGaze.CurrentObj_Infinity()) == "Handle";

        if (GazeObjIsHandle) {
            if (GCvrTrigger.OnLongClickOnlyOnce()) {
                //Debug.Log("Moving");
                // 防止購物車移動時，商品全部穿透掉光
                // 將所有是 "InCartProduct" Layer 的商品物件放入 Cart/InCartProduct 子物件內
                find.PlacedObjectParent(LayerName_InCartProduct, InCartProductObj);
                // 將 Range 物件開啟
                Range.SetActive(true);
                // 將 OnGroundProduct 物件內所有名為 "Pro_Obj" 的子物件 關閉 Collider 和 Rigidbody
                find.Collider_UseGravitySwitch(OnGroundProduct, "Pro_Obj", false);

                // 將 向前移動 狀態改成 true，玩家和購物車同時向前移動
                IsMovingForward = true;
            }
        }
    }

    private void CheckDown() {
        // 開啟 Cart Basket Collider
        Basket_Collider.enabled = true;
    }

    private void CheckUp() {
        if (IsMovingForward) {
            //Debug.Log("MoveStop");
            // 將 Range 物件關閉
            Range.SetActive(false);
            // 關閉 Cart Basket Collider
            Basket_Collider.enabled = false;
            // 將 OnGroundProduct 物件內所有名為 "Pro_Obj" 的子物件 開啟 Collider 和 Rigidbody
            find.Collider_UseGravitySwitch(OnGroundProduct, "Pro_Obj", true);

            // 將 向前移動 狀態改成 false，玩家和購物車同時停止向前移動
            IsMovingForward = false;
        }
    }

    private void CheckClick() {
        // 關閉 Cart Basket Collider
        Basket_Collider.enabled = false;
    }

    /// <summary>
    /// 玩家向前移動
    /// </summary>
    private void PlayerMove() {
        // 找到向前的方向
        Vector3 forward = cam.transform.forward;
        // 讓角色往前
        controller.SimpleMove(forward * speed);
    }

    /// <summary>
    /// 購物車跟著玩家移動
    /// </summary>
    private void CartMove() {
        // 攝影機 Y 軸旋轉角度 (最高只能 90 度，用於theta)
        float Camera_AngleY = cam.transform.eulerAngles.y;
        // Mathf.Deg2Rad 度轉弧度 = (PI * 2) / 360
        float theta = Camera_AngleY * Mathf.Deg2Rad;
        // 紀錄購物車 X 座標：x = r * sin(thita)
        float Cart_X = Cart_Player * Mathf.Sin(theta);
        // 紀錄購物車 Z 座標：z = r * cos(thita)
        float Cart_Z = Cart_Player * Mathf.Cos(theta);

        GvrViewer GvrViewerMain = GvrViewer.Instance;
        // 購物車會跟著玩家的視角移動位置
        Cart.position = new Vector3(Cart_X + GvrViewerMain.transform.position.x, 0f,
                                    Cart_Z + GvrViewerMain.transform.position.z);
        // 購物車會跟著玩家的視角旋轉角度
        Cart.rotation = Quaternion.Euler(0, Camera_AngleY, 0);
    }

    void OnDestroy() {
        // Gvr 按鈕事件
        GCvrTrigger.OnLongClick -= GCvrLongClick;
    }
}
