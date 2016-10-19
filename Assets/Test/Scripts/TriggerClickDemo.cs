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
        GCvrTrigger.OnDown      += GCvrDown;
        GCvrTrigger.OnUp        += GCvrUp;
        GCvrTrigger.OnClick     += GCvrClick;
        GCvrTrigger.OnLongClick += GCvrLongClick;
    }

    void LateUpdate() {
        // 目前準心對準的物件
        PrintGazeObj();
        // 按住 Gvr 按紐時會顯示並計算按了多少時間
        PrintLongClickTime();
    }

    private void GCvrDown(object sender) {
        ChangeObjectColor("SphereDown");
        /*
        Debug.Log(GCvrGaze.CurrentObj_Range());
        Debug.Log("目標物件名稱：" + GCvrGaze.CurrentObjName_Range());
        Debug.Log("目前準心對準物件的碰撞位置：" + GCvrGaze.GetIntersectPosition_Range());
        Debug.Log("玩家與目標物件的距離：" + GCvrGaze.GetTargetObj_Player_Distance());
        Debug.Log("是否超過範圍：" + GCvrGaze.IsOverRange());
        */
        //Debug.Log("目標物件名稱：" + GCvrGaze.GetObjName(GCvrGaze.CurrentObj_Range()));
        //Debug.Log("玩家與目標物件的距離：" + GCvrGaze.GetTargetObj_Player_Distance());
        // TODO 按下按鍵事件
    }

    private void GCvrUp(object sender) {
        ChangeObjectColor("SphereUp");

        // TODO 放開按鍵事件
    }

    private void GCvrClick(object sender) {
        ChangeObjectColor("SphereClick");

        // 預設是 0，如果偵測是點擊事件就會加 1
        int count = int.Parse(SphereClick_Counter.text) + 1;
        // 將加 1 的數字設定至 Counter 文字物件上
        SphereClick_Counter.text = count.ToString();

        // TODO 按一下按鍵事件
    }

    private void GCvrLongClick(object sender) {
        // TODO 按住按鍵事件
    }

    /// <summary>
    /// 按住 Gvr 按紐時會顯示：計算按住 Gvr 多少時間
    /// </summary>
    private void PrintLongClickTime() {
        Renderer SphereDown_Counter_Renderer = SphereDown_Counter.GetComponent<Renderer>();
        // 按下 Gvr 按鈕時 trigger.IsHeld() = true，放開 Gvr 按鈕時 = false
        if (Input.GetKey(KeyCode.Mouse0)) {
            // 顯示SphereDown 內的 Counter 的文字物件
            SphereDown_Counter_Renderer.enabled = true;
            // trigger.SecondsHeld() is the number of seconds we've held the trigger down
            SphereDown_Counter.text = GCvrTrigger.ClickTime().ToString("0.0000");
        } else
            // 閃爍 SphereDown 內的 Counter 文字物件
            SphereDown_Counter_Renderer.enabled = Time.time % 1 < 0.5;
    }

    /// <summary>
    /// Gaze 任何物件時會顯示：(Gaze 某物件：持續 Gaze 某物件多少時間)
    /// </summary>
    private void PrintGazeObj() {
        // 持續 Gaze 某物件多少時間
        float count = GCvrGaze.GazeTime();
        // 將 持續 Gaze 某物件多少時間 資料設定至 TimeCounter 文字物件上：Object：1.2345
        SphereClick_TimeCounter.text = 
            GCvrGaze.GetObjName(GCvrGaze.CurrentObj_Infinity()) + "：" + count.ToString("0.0000");
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

    void OnDestroy() {
        // Gvr 按鈕事件
        GCvrTrigger.OnDown -= GCvrDown;
        GCvrTrigger.OnUp -= GCvrUp;
        GCvrTrigger.OnClick -= GCvrClick;
        GCvrTrigger.OnLongClick -= GCvrLongClick;
    }
}
