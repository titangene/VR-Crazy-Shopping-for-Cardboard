using UnityEngine;
using System.Collections;

public class MovingController : MonoBehaviour {
    // For a full explanation of the API, look at ExampleCharacterController.cs
    // This example will assume knowledge of the API to code a moving first-person character
    // 讓第一人稱視角的角色向前移動
    // 準心對準購物車的手把並按住按鈕即可往前移動，放開按鈕可停止移動
    // 玩家同時能透過移動頭部方向來改變移動方位

    // 找到 Head 物件
    public Transform Head;
    // 找到 GvrViewer
    public GvrViewer GvrMain;
    // 找到購物車
    public GameObject Cart;
    // 向前移動速度
    public float speed = 5.0f;
    // 是否按住 Gvr 按鈕
    public bool GvrTrigger = false;
    // 準心是否對準購物車手把
    public bool GazeCart = false;
    // 找到 class CardboardControl
    private static CardboardControl cardboard;
    // 目光盯住的物體
    private static CardboardControlGaze gaze;
    // 角色
    private CharacterController controller;
    // 按住 Gvr 按鈕時間
    private float TriggerTime = 0.0f;
    // 是否向前移動
    private bool MoveForward = false;
    // 用來計算購物車與人物角色的距離
    private Vector3 Offset;
    

    void Start() {
        // 找到購物車
        Cart = GameObject.Find("Cart");
        // 找到 Head 物件
        Head = GvrMain.transform.FindChild("Head");
        // 找到 CharacterController
        controller = GetComponent<CharacterController>();
        // 找到 CardboardControlManager 中的 CardboardControl.cs Script
        cardboard = GameObject.Find("CardboardControlManager").GetComponent<CardboardControl>();
        // 放開 Gvr 按鈕時，玩家和購物車同時停止向前移動
        cardboard.trigger.OnUp += StopMove;
        // 改變目光(gaze)看的東西
        cardboard.gaze.OnChange += CardboardGazeChange;
    }

    // 放開 Gvr 按鈕時，玩家和購物車同時停止向前移動
    private void CardboardGazeChange(object sender) {
        // 目光盯住的物體
        gaze = sender as CardboardControlGaze;
    }

    // 檢查是否按住 Gvr 按鈕
    private void CheckTrigger() {
        if (cardboard.trigger.TiggerHold() == true) {
            Debug.Log("有按住 Gvr 按鈕");
            // 將 按住 Gvr 按鈕 狀態改成 true
            GvrTrigger = true;
            // 檢查準心是否對準購物車手把
            CheckGaze();
        } else {
            Debug.Log("沒有按住 Gvr 按鈕");
            // 將 按住 Gvr 按鈕 狀態改成 false
            GvrTrigger = false;
            // 將 向前移動 狀態改成 false，玩家和購物車同時停止向前移動
            MoveForward = false;
        }
    }

    // 檢查準心是否對準購物車手把
    private void CheckGaze() {
        // 是否按住 Gvr 按鈕
        if (GvrTrigger) {
            // 準心對準購物車手把時 gaze.IsHeld() = true，沒有準心對準時 = false
            if (gaze.IsHeld() && gaze.Object().name == "Handle") {
                // 將 向前移動 狀態改成 true，玩家和購物車同時向前移動
                MoveForward = true;
                // 將購物車放在 Head 物件內(子類別)
                //Cart.transform.parent = Head;
            }
        }
    }

    // 玩家和購物車同時停止向前移動
    private void StopMove(object sender) {
        // 將 向前移動 狀態改成 false，玩家和購物車同時停止向前移動
        MoveForward = false;
        // 解除購物車會跟著頭部方向移動的鎖定
        //Cart.transform.parent = null;
    }

    void Update() {
        // 是否按下 Gvr 按鈕
        if (cardboard.trigger.IsHeld()) {
            // 檢查是否按住 Gvr 按鈕
            CheckTrigger();
        }
        // 準心對準購物車的手把並按住按鈕即可往前移動，放開按鈕可停止移動
        // 玩家同時能透過移動頭部方向來改變移動方位
        if (MoveForward) {
            // 找到向前的方向
            Vector3 forward = Camera.main.transform.forward;
            // 讓角色往前
            controller.SimpleMove(forward * speed);
            //transform.position += forward * speed * Time.deltaTime;
        }
    }
}
