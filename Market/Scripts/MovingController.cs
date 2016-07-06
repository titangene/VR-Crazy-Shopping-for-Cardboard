using UnityEngine;
using System.Collections;

public class MovingController : MonoBehaviour {
    // For a full explanation of the API, look at ExampleCharacterController.cs
    // This example will assume knowledge of the API to code a moving first-person character
    // 讓第一人稱視角的角色向前移動
    // 準心對準購物車的手把並按住按鈕即可往前移動，放開按鈕可停止移動
    // 玩家同時能透過移動頭部方向來改變移動方位

    // 向前移動速度
    public float speed = 5.0f;
    // 找到 class CardboardControl
    private static CardboardControl cardboard;
    // 角色
    private CharacterController controller;
    // 是否向前移動
    private bool MoveForward = false;

    void Start() {
        // 找到 CharacterController
        controller = GetComponent<CharacterController>();
        // 找到 CardboardControlManager 中的 CardboardControl.cs Script
        cardboard = GameObject.Find("CardboardControlManager").GetComponent<CardboardControl>();
        cardboard.trigger.OnDown += ToggleMove;
        cardboard.trigger.OnUp += ToggleMove;
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
}
