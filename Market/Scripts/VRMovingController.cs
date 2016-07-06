using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class VRMovingController : MonoBehaviour {

    //走動速度
    public float speed = 5.0f;
    //是否向前走
    public bool moveForward;

    private CharacterController controller;
    private GvrViewer gvrViewer;
    private Transform vrHead;
    // 按滑鼠左鍵 = Gvr按鈕
    private KeyCode triggerKey = KeyCode.Mouse0;

    void Start () {
        // 找到CharacterController
        controller = GetComponent<CharacterController>();
        // 找到GvrViewer
        gvrViewer = GameObject.Find("GvrMain").GetComponent<GvrViewer>();
        Debug.Log(gvrViewer);
        // 找到VR Head
        vrHead = Camera.main.transform;
	}
	
	void Update () {
        // 如果按Gvr按鈕
        if (Input.GetKeyDown(triggerKey)) {
            // 更改向前走的狀態
            moveForward = !moveForward;
        }

        // 如果往前走
        if (moveForward) {
            // 找到向前的方向
            Vector3 forward = vrHead.TransformDirection(Vector3.forward);
            // 讓角色往前
            controller.SimpleMove(forward * speed);
        }
	}
}
