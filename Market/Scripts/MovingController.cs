using UnityEngine;
using System.Collections;

/* 讓人物角色以第一人稱視角向前移動，並且購物車會同時移動
 * (購物車與人物角色的距離不變)
 * 準心對準購物車的手把並按住 Gvr 按鈕即可往前移動，放開按鈕可停止移動
 * 玩家同時能透過移動頭部方向來改變移動方位
 */
public class MovingController : MonoBehaviour {
    // 找到 Head 物件
    public Transform Head;
    // 找到 GvrViewer
    public GvrViewer GvrMain;
    // 找到購物車物件
    public Transform Cart;
    // 購物車與人物角色的距離
    public float Cart_Player = 1.2f;
    // 向前移動速度
    public float speed = 6.0f;
    // 是否按住 Gvr 按鈕
    public bool HoldTrigger = false;
    // 準心是否對準購物車手把
    public bool GazeCart = false;
    // 是否向前移動
    public bool MoveForward = false;

    // 找到 class CardboardControl
    private static CardboardControl cardboard;
    // 準心對準的物體
    private static CardboardControlGaze gaze;
    // 準心對準的物體名稱
    private string GazeObjectName;
    // 角色
    private CharacterController controller;
    // 按住 Gvr 按鈕時間
    private float TriggerTime = 0.0f;
    // 計算購物車與人物角色的距離
    //private Vector3 Cart_Player_Distance;
    // 紀錄購物車與人物角色的距離
    //private float Cart_Player;
    // 紀錄購物車 X 座標
    private float Cart_X;
    // 紀錄購物車 Z 座標
    private float Cart_Z;

    void Start() {
        // 找到購物車物件
        Cart = GameObject.Find("Cart").transform;
        // 找到 Head 物件
        Head = GvrMain.transform.FindChild("Head");
        // 找到 CharacterController
        controller = GetComponent<CharacterController>();
        // 找到 CardboardControlManager 中的 CardboardControl.cs Script
        cardboard = GameObject.Find("CardboardControlManager").GetComponent<CardboardControl>();

        // 放開 Gvr 按鈕時，玩家和購物車同時停止向前移動
        cardboard.trigger.OnUp += PlayerCartStopMove;
        // 準心改變對準的物體時
        cardboard.gaze.OnChange += CardboardGazeChange;
        // 準心持續對準某個物體時
        cardboard.gaze.OnStare += CardboardStare;
    }

    // 準心改變對準的物體時
    private void CardboardGazeChange(object sender) {
        // 準心對準的物體
        gaze = sender as CardboardControlGaze;
        // Debug 如果準心沒有對準任何東西，會設定對準目標名稱 = nothing
        GazeObjectName = gaze.IsHeld() ? gaze.Object().name : "nothing";
        //Debug.Log(GazeObjectName);
    }

    // 準心持續對準某個物體時
    private void CardboardStare(object sender) {
        gaze = sender as CardboardControlGaze;
        // Debug 如果準心沒有對準任何東西，會設定對準目標名稱 = nothing
        GazeObjectName = gaze.IsHeld() ? gaze.Object().name : "nothing";
        //Debug.Log(GazeObjectName);
    }

    void Update() {
        // 是否按下 Gvr 按鈕
        if (cardboard.trigger.IsHeld()) {
            // 檢查是否按住 Gvr 按鈕
            CheckTrigger();
        } else {
            // 準心對準購物車手把 狀態改成 false
            GazeCart = false;
            // 將 按住 Gvr 按鈕 狀態改成 false
            HoldTrigger = false;
        }

        // 向前移動 狀態 = true，玩家和購物車同時向前移動
        if (MoveForward) {
            // 玩家向前移動
            PlayerMove();
            // 購物車跟著玩家移動
            CartMove();
        }
    }

    // 檢查是否按住 Gvr 按鈕
    private void CheckTrigger() {
        if (cardboard.trigger.TiggerHold() == true) {
            //Debug.Log("有按住 Gvr 按鈕");
            // 將 按住 Gvr 按鈕 狀態改成 true
            HoldTrigger = true;
            // 檢查準心是否對準購物車手把
            CheckGaze();
        } else {
            //Debug.Log("沒有按住 Gvr 按鈕");
            // 將 按住 Gvr 按鈕 狀態改成 false
            HoldTrigger = false;
            // 將 向前移動 狀態改成 false，玩家和購物車同時停止向前移動
            MoveForward = false;
        }
    }

    // 檢查準心是否對準購物車手把
    private void CheckGaze() {
        // 是否按住 Gvr 按鈕
        if (HoldTrigger) {
            // 準心對準購物車手把時 gaze.IsHeld() = true，準心沒有對準時 = false
            // 購物車手把物件名稱為 Handle
            if (gaze.IsHeld() && GazeObjectName == "Handle") {
                //Debug.Log("Moving");
                // 準心對準購物車手把 狀態改成 true
                GazeCart = true;
                // 將 向前移動 狀態改成 true，玩家和購物車同時向前移動
                MoveForward = true;
            }
        }
    }

    // 玩家和購物車同時停止向前移動
    private void PlayerCartStopMove(object sender) {
        //Debug.Log("Stop");
        // 將 向前移動 狀態改成 false，玩家和購物車同時停止向前移動
        MoveForward = false;
    }

    // 玩家向前移動
    private void PlayerMove() {
        // 找到向前的方向
        Vector3 forward = Camera.main.transform.forward;
        // 讓角色往前
        controller.SimpleMove(forward * speed);
    }

    // 購物車跟著玩家移動
    private void CartMove() {
        // 更新購物車與人物角色的距離
        //Cart_Player_Distance = Cart.position - transform.position;
        // 紀錄購物車與人物角色的距離 (r 圓型半徑)   畢氏定理 C = sqrt(A^2 + B^2)
        //Cart_Player = Mathf.Sqrt(Mathf.Pow(Cart_Player_Distance.x, 2) + Mathf.Pow(Cart_Player_Distance.z, 2));
        //if (Cart_Player != 1.5f) {
        //    Cart_Player = 1.5f;
        //}
        // 攝影機 Y 軸旋轉角度 (最高只能 90 度，用於theta)
        float Camera_AngleY = Head.transform.eulerAngles.y;
        // Mathf.Deg2Rad 度轉弧度 = (PI * 2) / 360
        float theta = Camera_AngleY * Mathf.Deg2Rad;
        // 紀錄購物車 X 座標：x = r * sin(thita)
        Cart_X = Cart_Player * Mathf.Sin(theta);
        // 紀錄購物車 Z 座標：z = r * cos(thita)
        Cart_Z = Cart_Player * Mathf.Cos(theta);
        // 購物車會跟著玩家的視角移動位置
        Cart.position = new Vector3(Cart_X + GvrMain.transform.position.x, 0f, 
                                    Cart_Z + GvrMain.transform.position.z);
        // 購物車會跟著玩家的視角旋轉角度
        Cart.rotation = Quaternion.Euler(0, Camera_AngleY, 0);
    }
}
