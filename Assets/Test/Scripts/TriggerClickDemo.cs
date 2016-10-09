using UnityEngine;

public class TriggerClickDemo : MonoBehaviour {

    public TextMesh SphereDown_Counter;
    public TextMesh SphereClick_Counter;
    public TextMesh SphereClick_TimeCounter;

    private GCvrGaze GCvrGaze;
    private GCvrTrigger GCvrTrigger;

    void Start() {
        Camera MainCamera = Camera.main;
        GCvrGaze = MainCamera.GetComponent<GCvrGaze>();
        GCvrTrigger = MainCamera.GetComponent<GCvrTrigger>();

        // Gvr 按鈕事件
        GCvrTrigger.OnDown      += TiggerDown;
        GCvrTrigger.OnUp        += TiggerUp;
        GCvrTrigger.OnClick     += TiggerClick;
        GCvrTrigger.OnLongClick += TiggerLongClick;
    }

    void LateUpdate() {
        // 目前準心對準的物件
        SphereClick_TimeCounter.text = GCvrGaze.GetCurrentObjName_Unlimited();
        // 按住 Gvr 按紐時會顯示並計算按了多少時間
        PrintLongClickTime();
    }

    private void TiggerDown(object sender) {
        ChangeObjectColor("SphereDown");
        //Debug.Log("按下按鍵");
    }

    private void TiggerUp(object sender) {
        ChangeObjectColor("SphereUp");
        //Debug.Log("放開按鍵");
    }

    private void TiggerClick(object sender) {
        ChangeObjectColor("SphereClick");
        // 預設是 0，如果偵測是點擊事件就會加 1
        int count = int.Parse(SphereClick_Counter.text) + 1;
        // 將加 1 的數字設定至 Counter 文字物件上
        SphereClick_Counter.text = count.ToString();
        //Debug.Log("按一下按鍵");
    }

    private void TiggerLongClick(object sender) {
        //Debug.Log("按住按鍵");
    }

    /// <summary>
    /// 按住 Gvr 按紐時會顯示並計算按了多少時間
    /// </summary>
    private void PrintLongClickTime() {
        Renderer SphereDown_Counter_Renderer = SphereDown_Counter.GetComponent<Renderer>();
        // 按下 Gvr 按鈕時 trigger.IsHeld() = true，放開 Gvr 按鈕時 = false
        if (Input.GetKey(KeyCode.Mouse0)) {
            // 顯示SphereDown 內的 Counter 的文字物件
            SphereDown_Counter_Renderer.enabled = true;
            // trigger.SecondsHeld() is the number of seconds we've held the trigger down
            SphereDown_Counter.text = GCvrTrigger.ClickTime().ToString("0.0000");
        } else {
            // 閃爍SphereDown 內的 Counter 的文字物件
            SphereDown_Counter_Renderer.enabled = Time.time % 1 < 0.5;
        }
    }

    /// <summary>
    /// 改變方塊顏色(隨機)
    /// </summary>
    /// <param name="name">要改變的物件名稱</param>
    private void ChangeObjectColor(string name) {
        GameObject obj = GameObject.Find(name);
        Color newColor = RandomColor();
        obj.GetComponent<Renderer>().material.color = newColor;
    }

    /// <summary>
    /// 產生隨機顏色
    /// </summary>
    private Color RandomColor() {
        return new Color(Random.value, Random.value, Random.value);
    }

    // 當掛載該 Script 的物件被銷毀時，可清理一切
    void OnDestroy() {
        // Gvr 按鈕事件
        GCvrTrigger.OnDown -= TiggerDown;
        GCvrTrigger.OnUp -= TiggerUp;
        GCvrTrigger.OnClick -= TiggerClick;
        GCvrTrigger.OnLongClick -= TiggerLongClick;
    }
}
