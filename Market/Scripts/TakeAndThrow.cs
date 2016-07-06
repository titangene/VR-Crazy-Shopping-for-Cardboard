using UnityEngine;
using System.Collections;

public class TakeAndThrow : MonoBehaviour, IGvrGazeResponder {

    private Vector3 startingPosition;
    public Transform Head;
    public GvrViewer CB;
    Rigidbody RB;
    public bool holding = false;
    [Range(1.0f, 10.0f)]
    public float speed = 8.0f;

    void Start() {
        startingPosition = transform.localPosition;
        // 一開始物體會變成紅色
        SetGazedAt(false);
        // 找到頭部視角鏡頭
        Head = CB.transform.FindChild("Head");
        // 找到當前物體的鋼體
        RB = GetComponent<Rigidbody>();
    }

    void LateUpdate() {
        GvrViewer.Instance.UpdateState();
        if (GvrViewer.Instance.BackButtonPressed) {
            Application.Quit();
        }
    }

    void Update() {

    }

    #region IGvrGazeResponder implementation

    /// Called when the user is looking on a GameObject with this script,
    /// as long as it is set to an appropriate layer (see GvrGaze).
    public void OnGazeEnter() {
        // 準心對準物體會變成綠色
        SetGazedAt(true);
    }

    /// Called when the user stops looking on the GameObject, after OnGazeEnter
    /// was already called.
    public void OnGazeExit() {
        // 準心沒有對準物體會變成紅色
        SetGazedAt(false);
    }

    /// Called when the viewer's trigger is used, between OnGazeEnter and OnGazeExit.
    public void OnGazeTrigger() {
        TeleportRandomly();
    }

    #endregion

    public void SetGazedAt(bool gazedAt) {
        GetComponent<Renderer>().material.color = gazedAt ? Color.blue : Color.red;
    }

    public void Reset() {
        transform.localPosition = startingPosition;
    }

    public void ToggleVRMode() {
        GvrViewer.Instance.VRModeEnabled = !GvrViewer.Instance.VRModeEnabled;
    }

    public void ToggleDistortionCorrection() {
        switch (GvrViewer.Instance.DistortionCorrection) {
            case GvrViewer.DistortionCorrectionMethod.Unity:
            GvrViewer.Instance.DistortionCorrection = GvrViewer.DistortionCorrectionMethod.Native;
            break;
            case GvrViewer.DistortionCorrectionMethod.Native:
            GvrViewer.Instance.DistortionCorrection = GvrViewer.DistortionCorrectionMethod.None;
            break;
            case GvrViewer.DistortionCorrectionMethod.None:
            default:
            GvrViewer.Instance.DistortionCorrection = GvrViewer.DistortionCorrectionMethod.Unity;
            break;
        }
    }

    public void ToggleDirectRender() {
        GvrViewer.Controller.directRender = !GvrViewer.Controller.directRender;
    }

    public void TeleportRandomly() {
        Vector3 direction = Random.onUnitSphere;
        direction.y = Mathf.Clamp(direction.y, 0.5f, 1f);
        float distance = 2 * Random.value + 1.5f;
        transform.localPosition = direction * distance;
    }

    public void GetObject() {

        if (holding == false) {                                 // 按一下 Cardboard 按鈕
            transform.parent = Head;                            // 物體會跟著頭部方向移動
            holding = true;
            RB.useGravity = false;                              // 關閉物體的重力
            RB.constraints = RigidbodyConstraints.FreezeAll;    // 鎖定物理效果影響物體的旋轉和移動
        } else {                                                // 放開 Cardboard 按鈕
            transform.parent = null;                            // 解除物體會跟著頭部方向移動的鎖定
            holding = false;
            RB.useGravity = true;                               // 開啟物體的重力
            RB.constraints = RigidbodyConstraints.None;         // 解除物理效果影響物體旋轉和移動的鎖定
            RB.velocity = Head.forward * speed;                 // 往視角的方向丟出物體
        }
    }
}
