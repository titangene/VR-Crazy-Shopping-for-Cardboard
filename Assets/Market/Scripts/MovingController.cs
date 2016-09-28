using UnityEngine;

/* 
 * 讓人物角色以第一人稱視角向前移動，並且購物車會同時移動
 * (購物車與人物角色的距離不變)
 * 準心對準購物車的手把並按住 Gvr 按鈕即可往前移動，放開按鈕可停止移動
 * 玩家同時能透過移動頭部方向來改變移動方位
 */
public class MovingController : MonoBehaviour {
    public GvrViewer GvrViewerMain;
    public GameObject Range;

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

    private Transform Cart;
    private Collider Basket_Collider;

    /// <summary>
    /// Cart/InCartProduct 子物件 (拿來放丟入購物車內的所有商品)
    /// </summary>
    private Transform InCartProductObj;
    /// <summary>
    /// 準心對準的物件名稱
    /// </summary>
    private string GazeObjectName;
    /// <summary>
    /// 角色
    /// </summary>
    private CharacterController controller;
    /// <summary>
    /// 找出 Layer / Tag
    /// </summary>
    private Find find;

    private bool DebugLogPrint = true;

    #region 計算購物車與人物角色的距離(暫時用不到)
    /*
    // 計算購物車與人物角色的距離
    private Vector3 Cart_Player_Distance;
    // 紀錄購物車與人物角色的距離
    private float Cart_Player;
    // 購物車物件的剛體
    private Rigidbody Cart_rbody;
    */
    #endregion

    void Start() {
        Cart = GameObject.Find("Cart").transform;
        //Cart_rbody = Cart.GetComponent<Rigidbody>();
        Basket_Collider = GameObject.Find("Basket").GetComponent<Collider>();

        // 找到 Cart/InCartProduct 子物件 (拿來放丟入購物車內的所有商品)
        InCartProductObj = GameObject.Find("InCartProduct").transform;
        // 找到 CharacterController
        controller = gameObject.GetComponent<CharacterController>();
        // 找出 Layer / Tag
        find = gameObject.GetComponent<Find>();

        cardboard = GameObject.Find("CardboardControlManager").GetComponent<CardboardControl>();

        // 按下 Gvr 按鈕時，開啟Cast Basket Collider
        cardboard.trigger.OnDown += CardboardDown;
        // 放開 Gvr 按鈕時，玩家和購物車同時停止向前移動
        cardboard.trigger.OnUp += CardboardUp;
        // 在設定時間內按下並且快速放開 Gvr 按鈕間，會判定為點擊事件觸發
        // 按一下 Gvr 按紐時
        cardboard.trigger.OnClick += CardboardClick;
        // 按住 Gvr 按鈕時
        cardboard.trigger.OnLongClick += CardboardLongClick;
        // 準心改變對準的物件時
        cardboard.gaze.OnChange += CardboardGazeChange;
        // 準心持續對準某個物件時
        cardboard.gaze.OnStare += CardboardStare;
    }

    /// <summary>
    /// 準心改變對準的物件時：
    /// 1. 紀錄準心對準的物件名稱，
    /// 2. 紀錄準心是否對準購物車手把
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
    /// 準心持續對準某個物件時：
    /// 1. 紀錄準心對準的物件名稱，
    /// 2. 紀錄準心是否對準購物車手把
    /// </summary>
    private void CardboardStare(object sender) {
        gaze = sender as CardboardControlGaze;
        // Debug 如果準心沒有對準任何東西，會設定對準目標名稱 = nothing
        GazeObjectName = gaze.IsHeld() ? gaze.Object().name : "nothing";
        // 準心是否對準購物車手把，購物車手把物件名稱為 Handle
        GazeObjIsHandle = (GazeObjectName == "Handle");
    }

    /// <summary>
    /// 準心對準購物車手把時，按下 Gvr 按鈕：
    /// 開啟 Cart Basket Collider
    /// </summary>
    private void CardboardDown(object sender) {
        // 準心是否對準購物車手把
        if (GazeObjIsHandle) {
            // 開啟 Cart Basket Collider
            Basket_Collider.enabled = true;
        }
    }

    /// 在設定時間內按下並且快速放開 Gvr 按鈕間，會判定為點擊事件觸發
    /// <summary>
    /// 準心對準購物車手把時，按一下 Gvr 按鈕：
    /// 關閉 Cart Basket Collider
    /// </summary>
    private void CardboardClick(object sender) {
        // 準心是否對準購物車手把
        if (GazeObjIsHandle) {
            // 關閉 Cart Basket Collider
            Basket_Collider.enabled = false;
        }
    }

    /// <summary>
    /// 準心對準購物車手把時，按住 Gvr 按鈕：
    /// 1. 玩家和購物車可同時向前移動，
    /// 2. 將購物車內的所有放入 Cart/InCartProduct 子物件內，
    /// 3. 開啟 Range 物件
    /// </summary>
    private void CardboardLongClick(object sender) {
        //Debug.Log("按住 Gvr 按鈕");

        // 準心是否對準購物車手把
        if (GazeObjIsHandle) {

            if (DebugLogPrint) {
                Debug.Log("Moving");
                DebugLogPrint = false;
            }

            // 將 向前移動 狀態改成 true，玩家和購物車同時向前移動
            IsMovingForward = true;

            // 防止購物車移動時，商品全部穿透掉光
            // 將所有是 "InCartProduct" Layer 的商品物件放入 Cart/InCartProduct 子物件內
            find.PlacedObjectParent(LayerName_InCartProduct, InCartProductObj);
            // 將 Range 物件開啟

            Range.SetActive(true);
        }
    }

    /// <summary>
    /// 放開 Gvr 按鈕時：
    /// 1. 玩家和購物車同時停止向前移動，
    /// 2. 關閉 Range 物件，
    /// 3. 關閉 Cart Basket Collider
    /// </summary>
    private void CardboardUp(object sender) {
        if (IsMovingForward) {
            Debug.Log("MoveStop");
            DebugLogPrint = true;

            // 將 向前移動 狀態改成 false，玩家和購物車同時停止向前移動
            IsMovingForward = false;
            // 將 Range 物件關閉
            Range.SetActive(false);
            // 關閉 Cart Basket Collider
            Basket_Collider.enabled = false;
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
        /*
        // 更新購物車與人物角色的距離
        Cart_Player_Distance = Cart.position - transform.position;
        // 紀錄購物車與人物角色的距離 (r 圓型半徑)   畢氏定理 C = sqrt(A^2 + B^2)
        Cart_Player = Mathf.Sqrt(Mathf.Pow(Cart_Player_Distance.x, 2) + Mathf.Pow(Cart_Player_Distance.z, 2));
        if (Cart_Player != 1.5f) {
            Cart_Player = 1.5f;
        }
        */
        #endregion

        // 攝影機 Y 軸旋轉角度 (最高只能 90 度，用於theta)
        float Camera_AngleY = Camera.main.transform.eulerAngles.y;
        // Mathf.Deg2Rad 度轉弧度 = (PI * 2) / 360
        float theta = Camera_AngleY * Mathf.Deg2Rad;
        // 紀錄購物車 X 座標：x = r * sin(thita)
        float Cart_X = Cart_Player * Mathf.Sin(theta);
        // 紀錄購物車 Z 座標：z = r * cos(thita)
        float Cart_Z = Cart_Player * Mathf.Cos(theta);
        
        // 購物車會跟著玩家的視角移動位置
        Cart.position = new Vector3(Cart_X + GvrViewerMain.transform.position.x, 0f,
                                    Cart_Z + GvrViewerMain.transform.position.z);
        // 購物車會跟著玩家的視角旋轉角度
        Cart.rotation = Quaternion.Euler(0, Camera_AngleY, 0);
        
        #region 剛體移動(暫時用不到)
        /*
        Vector3 CartMove = new Vector3(Cart_X, 0f, Cart_Z);
        Cart_rbody.AddForce(CartMove);
        // 購物車利用剛體移動
        Cart_rbody.AddForce(Cart.forward);
        // 購物車移動到的位置
        Cart.position = new Vector3(Cart.position.x, 0f, Cart.position.z);
        Debug.Log(Cart.forward);
        */
        #endregion
    }

    // 當掛載該 Script 的物件被銷毀時，可清理一切
    void OnDestroy() {
        cardboard.trigger.OnDown -= CardboardDown;
        cardboard.trigger.OnUp -= CardboardUp;
        cardboard.trigger.OnClick -= CardboardClick;
        cardboard.trigger.OnLongClick -= CardboardLongClick;
        cardboard.gaze.OnChange -= CardboardGazeChange;
        cardboard.gaze.OnStare -= CardboardStare;
    }
}
