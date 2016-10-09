using UnityEngine;

public class PickUpAndThrowController : MonoBehaviour {
    /// <summary>
    /// 購物車的碰撞體
    /// </summary>
    public GameObject Cart_Collider;
    /// <summary>
    /// Debug gazeObj：拿取商品時，準心會永遠放大，丟出商品後，準心會縮小
    /// </summary>
    public GameObject FixGazeObj;
    /// <summary>
    /// 是否拿取商品
    /// </summary>
    public bool PickingUp = false;
    /// <summary>
    /// 是否能按第二次 Gvr 按鈕
    /// </summary>
    public bool CanSecondClick = false;
    /// <summary>
    /// 拿取商品時的力
    /// </summary>
    public float PickUp_Power = 15.0f;
    /// <summary>
    /// 丟出商品時的力
    /// </summary>
    public float Throw_Power = 3.0f;

    private GCvrGaze GCvrGaze;
    /// <summary>
    /// 目標物件 (商品)
    /// </summary>
    private Transform TargetObj = null;
    /// <summary>
    /// 目標物件之剛體 (商品)
    /// </summary>
    private Rigidbody HitRB = null;
    /// <summary>
    /// 商品被拿取時的位置
    /// </summary>
    private Transform PickupObj;

    public static PickUpAndThrowController Instance {
        get {
#if UNITY_EDITOR
            if (instance == null && !Application.isPlaying) {
                instance = UnityEngine.Object.FindObjectOfType<PickUpAndThrowController>();
            }
#endif
            if (instance == null) {
                Debug.LogError("No PickUpAndThrowController instance found.  Ensure one exists in the scene, or call "
                    + "PickUpAndThrowController.Create() at startup to generate one.\n"
                    + "If one does exist but hasn't called Awake() yet, "
                    + "then this error is due to order-of-initialization.\n"
                    + "In that case, consider moving "
                    + "your first reference to PickUpAndThrowController.Instance to a later point in time.\n"
                    + "If exiting the scene, this indicates that the PickUpAndThrowController object has already "
                    + "been destroyed.");
            }
            return instance;
        }
    }
    private static PickUpAndThrowController instance = null;

    public static void Create() {
        if (instance == null && UnityEngine.Object.FindObjectOfType<PickUpAndThrowController>() == null) {
            Debug.Log("Creating PickUpAndThrowController object");
            var go = new GameObject("PickUpAndThrowController", typeof(PickUpAndThrowController));
            go.transform.localPosition = Vector3.zero;
            // sdk will be set by Awake().
        }
    }

    void Awake() {
        if (instance == null) {
            instance = this;
        }
        if (instance != this) {
            Debug.LogError("There must be only one PickUpAndThrowController object in a scene.");
            UnityEngine.Object.DestroyImmediate(this);
            return;
        }
    }

    void Start() {
        GCvrGaze = Camera.main.GetComponent<GCvrGaze>();
        PickupObj = GameObject.Find("PickupObject").transform;
    }

    void LateUpdate() {
        // 是否按一下 Gvr 按鈕拿取商品
        if (PickingUp) {
            // 商品跟著玩家頭部方向移動
            PickUpProduct();
        }
    }

    /// <summary>
    /// 在設定時間內按下並且快速放開 Gvr 按鈕間，會判定為點擊事件觸發
    /// </summary>
    public void CheckClick() {
        // 按第一次 Gvr 按鈕：拿取商品
        if (!CanSecondClick) {
            // 設定目標物件 (商品)
            TargetObj = GCvrGaze.GetCurrentObj_Range().transform;
            // 設定目標物件之剛體 (商品)
            HitRB = GCvrGaze.GetHit().rigidbody;
            Debug.Log("PickUp：" + TargetObj.name);

            // 將 是否拿取商品 狀態改成 true
            PickingUp = true;
            // 將 是否能按第二次 Gvr 按鈕 狀態改成 true
            CanSecondClick = true;
            // 將 Cart_Collider、FixGazeObj 物件開啟
            Cart_Collider.SetActive(true);
            FixGazeObj.SetActive(true);

            // 按第二次 Gvr 按鈕：丟出商品
        } else {
            Debug.Log("Throw");

            // 將 是否拿取商品 狀態改成 false
            PickingUp = false;
            // 將 是否能按第二次 Gvr 按鈕 狀態改成 false
            CanSecondClick = false;
            // 丟出商品
            ThrowProduct();
            // 將 Cart_Collider、FixGazeObj 物件關閉
            Cart_Collider.SetActive(false);
            FixGazeObj.SetActive(false);

            // 設定目標物件 (商品) 為 null
            TargetObj = null;
            // 設定目標物件之剛體 (商品) 為 null
            HitRB = null;
        }
    }

    /// <summary>
    /// 拿取商品
    /// </summary>
    private void PickUpProduct() {
        // 商品會跟著玩家頭部方向移動
        HitRB.velocity = (PickupObj.position - (TargetObj.position +
            HitRB.centerOfMass)) * PickUp_Power;
    }

    /// <summary>
    /// 丟出商品
    /// </summary>
    private void ThrowProduct() {
        HitRB.velocity = Camera.main.transform.forward * Throw_Power;
    }

    void OnDestroy() {
        if (instance == this) {
            instance = null;
        }
    }
}
