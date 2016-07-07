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
    // 向前移動速度
    public float speed = 5.0f;
    // 找到 class CardboardControl
    private static CardboardControl cardboard;
    // 角色
    private CharacterController controller;
    // 是否向前移動
    private bool MoveForward = false;

    void Start() {
        // 找到 Head 物件
        Head = GvrMain.transform.FindChild("Head");

        // 找到 CharacterController
        controller = GetComponent<CharacterController>();

        // 找到 CardboardControlManager 中的 CardboardControl.cs Script
        cardboard = GameObject.Find("CardboardControlManager").GetComponent<CardboardControl>();

        // 按下 Gvr 按鈕時
        cardboard.trigger.OnDown += CardboardDown;

        // 放開 Gvr 按鈕時
        cardboard.trigger.OnUp += CardboardUp;

        // 在設定時間內按下並且快速放開 Gvr 按鈕間，會判定為點擊事件觸發
        cardboard.trigger.OnClick += CardboardClick;

        // 改變目光(gaze)看的東西
        cardboard.gaze.OnChange += CardboardGazeChange;

        // 持續盯著某個物體
        cardboard.gaze.OnStare += CardboardStare;
    }

    // 按下 Gvr 按鈕時
    private void CardboardDown(object sender) {
        Debug.Log("偵測到按下 Gvr 按鈕");
    }

    // 放開 Gvr 按鈕時
    private void CardboardUp(object sender) {
        Debug.Log("偵測到放開 Gvr 按鈕");
    }

    // 在設定時間內按下並且快速放開 Gvr 按鈕間，會判定為點擊事件觸發
    private void CardboardClick(object sender) {
        // 找到 SphereClick 內的 Counter 的文字物件
        TextMesh Counter = GameObject.Find("SphereClick/Counter").GetComponent<TextMesh>();
        // 預設是 0，如果偵測是點擊事件就會加 1
        int increment = int.Parse(Counter.text) + 1;
        // 將加 1 的數字設定至 Counter 文字物件上
        Counter.text = increment.ToString();

        // If you need more raycast data from cardboard.gaze, the RaycastHit is exposed as gaze.Hit()
    }

    // 改變目光(gaze)看的東西
    private void CardboardGazeChange(object sender) {
        // You can grab the data from the sender instead of the CardboardControl object
        // 目光盯住的物體
        CardboardControlGaze gaze = sender as CardboardControlGaze;
        // We can access to the object we're looking at
        // gaze.IsHeld will make sure the gaze.Object() isn't null
        // 目光盯住的物體時 gaze.IsHeld() = true，目光沒有盯住的物體時 = false
        if (gaze.IsHeld() && gaze.Object().name.Contains("Cube")) {

            // 碰到 HighlightCube 這個方塊準心(cardboard.reticle)會變成紅色
            if (gaze.Object().name == "HighlightCube") {

            }
        }
        // We also can access to the last object we looked at
        // gaze.WasHeld() will make sure the gaze.PreviousObject() isn't null
        // 存取上一個目光盯住的物體在 gaze.WasHeld()
        if (gaze.WasHeld() && gaze.PreviousObject().name.Contains("Cube")) {
            // Use these to undo reticle hiding and highlighting
            // 顯示準心
            cardboard.reticle.Show();
        }

        // Be sure to set the Reticle Layer Mask on the CardboardControlManager
        // to grow the reticle on the objects you want. The default is everything.

        // Not used here are gaze.Forward(), gaze.Right(), and gaze.Rotation()
        // which are useful for things like checking the view angle or shooting projectiles
    }

    // 持續盯著某個物體
    private void CardboardStare(object sender) {
        CardboardControlGaze gaze = sender as CardboardControlGaze;
        if (gaze.IsHeld() && gaze.Object().name.Contains("Cube")) {

        }
    }

    // 按 Gvr按鈕 會改變 向前走的狀態
    void ToggleMove(object sender) {
        // 改變向前走的狀態
        MoveForward = !MoveForward;
    }

    void Update() {
        // If you don't need as much control over what happens when moving is toggled,
        // you can replace this with cardboard.trigger.IsHeld() and remove ToggleMove()
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

    /*
    * Be sure to unsubscribe before this object is destroyed
    * so the garbage collector can clean everything up
    * 當掛載該 Script 的物件被銷毀時，使垃圾回收器可以清理一切 ??? 看不懂
    */
    void OnDestroy() {
        cardboard.trigger.OnDown -= CardboardDown;
        cardboard.trigger.OnUp -= CardboardUp;
        cardboard.trigger.OnClick -= CardboardClick;
        cardboard.gaze.OnChange -= CardboardGazeChange;
        cardboard.gaze.OnStare -= CardboardStare;
    }
}
