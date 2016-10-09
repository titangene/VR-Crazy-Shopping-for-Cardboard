using UnityEngine;

public class GCvrTrigger : MonoBehaviour {
    /// <summary>
    /// 設定按下並且快速放開 Gvr 按鈕間的時間，
    /// 在範圍內會判定為點擊事件觸發，
    /// 超過範圍會判定為按住事件觸發
    /// </summary>
    public const float ClickHoldTimeRange = 0.4f;

    // 按住 Gvr 按鈕時間，預設為 0 秒
    private float clickStartTime = 0f;
    // 是否按住 Gvr 按鈕
    private bool IsLongClick = false;

    public delegate void GCvrDelegate(object sender);

    public GCvrDelegate OnUp = delegate { };
    public GCvrDelegate OnDown = delegate { };
    public GCvrDelegate OnClick = delegate { };
    public GCvrDelegate OnLongClick = delegate { };

    void Update() {
        CheckKey();
    }

    private void CheckKey() {
        if (Input.GetMouseButtonDown(0))
            CheckDown();
        if (Input.GetMouseButtonUp(0))
            CheckUp();
        if (Input.GetMouseButton(0))
            CheckLongClick();
    }

    private void CheckDown() {
        OnDown(this);
        // 紀錄開始按住的時間
        clickStartTime = Time.time;
    }

    private void CheckUp() {
        OnUp(this);
        CheckClick();
    }

    private void CheckClick() {
        // 是否為點擊事件
        bool IsOnClick = ClickTime() <= ClickHoldTimeRange;
        // 重設 開始按住的時間
        clickStartTime = 0f;

        if (IsOnClick)
            OnClick(this);
    }

    private void CheckLongClick() {
        if (ClickTime() > ClickHoldTimeRange) {
            OnLongClick(this);
        }
    }

    /// <summary>
    /// 取得按住多少時間
    /// </summary>
    public float ClickTime() {
        return Time.time - clickStartTime;
    }

    /// <summary>
    /// 是否按下 Gvr 按鈕
    /// </summary>
    public bool IsOnClick() {
        return GvrViewer.Instance.Triggered;
    }
}
