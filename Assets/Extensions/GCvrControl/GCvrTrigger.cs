using UnityEngine;
using GCvrDelegates;

public class GCvrTrigger : MonoBehaviour {
    /// <summary>
    /// 設定按下並且快速放開 Gvr 按鈕間的時間，在範圍內會判定為 OnClick 事件觸發，
    /// 超過範圍會判定為 OnLongClick 事件觸發
    /// </summary>
    public float clickHoldTimeRange = 0.4f;
    /// <summary>
    /// 按下 Gvr 按鈕時裝置是否震動
    /// </summary>
    public bool vibrateOnDown = false;
    /// <summary>
    /// 放開 Gvr 按鈕時裝置是否震動
    /// </summary>
    public bool vibrateOnUp = false;
    /// <summary>
    /// 點擊 Gvr 按鈕時裝置是否會震動
    /// </summary>
    public bool vibrateOnClick = false;

    /// <summary>
    /// 開始按住 Gvr 按鈕時間
    /// </summary>
    private float clickStartTime = 0f;
    /// <summary>
    /// 在長按事件用來偵測是否能只做一次
    /// </summary>
    private bool onlyOnce = true;

    public GCvrDelegate OnUp = delegate { };
    public GCvrDelegate OnDown = delegate { };
    public GCvrDelegate OnClick = delegate { };
    public GCvrDelegate OnLongClick = delegate { };

    void Update() {
        CheckKey();
    }

    private void CheckKey() {
        if (Input.GetMouseButtonDown(0))
            ReportDown();
        if (Input.GetMouseButtonUp(0)) {
            ReportUp();
            ReportClick();
        }
        if (Input.GetMouseButton(0))
            ReportLongClick();
    }

    private void ReportDown() {
        //Debug.Log("按一下按鍵");
        OnDown(this);
        // 紀錄開始按住的時間
        clickStartTime = Time.time;
        // 是否讓裝置震動
        if (vibrateOnDown)
            Handheld.Vibrate();
    }

    private void ReportUp() {
        //Debug.Log("放開按鍵");
        OnUp(this);

        onlyOnce = true;
        // 是否讓裝置震動
        if (vibrateOnUp)
            Handheld.Vibrate();
    }

    private void ReportClick() {
        // 是否為點擊事件
        bool IsOnClick = ClickTime() <= clickHoldTimeRange;
        // 重設 開始按住的時間
        clickStartTime = 0f;

        if (IsOnClick) {
            //Debug.Log("按一下按鍵");
            OnClick(this);
            // 是否讓裝置震動
            if (vibrateOnClick)
                Handheld.Vibrate();
        }
    }

    private void ReportLongClick() {
        // 是否為長按事件
        bool IsOnLongClick = ClickTime() > clickHoldTimeRange;

        if (IsOnLongClick) {
            OnLongClick(this);
            // 在長按事件用來偵測是否能只做一次
            if (onlyOnce) {
                //Debug.Log("按住按鍵");
                onlyOnce = false;
            }
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

    /// <summary>
    /// 在長按事件用來偵測是否能只做一次
    /// </summary>
    public bool OnLongClickOnlyOnce() {
        return onlyOnce;
    }

    /// <summary>
    /// 讓裝置震動
    /// </summary>
    public void Vibrate() {
        Handheld.Vibrate();
    }
}
