using UnityEngine;
using System.Collections;
using CardboardControlDelegates;

/**
* Creating a vision raycast and handling the data from it
* Relies on Google Cardboard SDK API's
*/
public class CardboardControlTrigger : MonoBehaviour {
    // 設定按下並且快速放開 Gvr 按鈕間的時間，會判定為點擊事件觸發
    public float clickSpeedThreshold = 0.4f;
    // 使用事件冷卻時間
    public bool useEventCooldowns = true;
    // 按下 Gvr 按鈕時是否裝置震動
    public bool vibrateOnDown = false;
    // 放開 Gvr 按鈕時是否裝置震動
    public bool vibrateOnUp = false;
    // 在設定時間內按下並且快速放開 Gvr 按鈕間，會判定為點擊事件觸發
    // 點擊事件觸發時是否裝置震動
    public bool vibrateOnClick = true;
    // 使用磁鐵
    public bool useMagnet = true;
    // 使用觸碰
    public bool useTouch = true;
    // 滑鼠左鍵 = Gvr 按鈕
    public KeyCode triggerKey = KeyCode.Mouse0;
    // 列印 Debug 訊息
    public bool printDebugInfo = false;

    // 分析磁鐵資料
    private ParsedMagnetData magnet;
    // 分析觸碰資料
    private ParsedTouchData touch;
    // enum 是列舉，預設第一個為 0，依序類推
    // Gvr 按鈕狀態
    private enum TriggerState {
        Up, Down
    }
    // 目前 Gvr 按鈕狀態，預設為放開 Gvr 按鈕
    private TriggerState currentTriggerState = TriggerState.Up;
    // 按住 Gvr 按鈕時間，預設為 0 秒
    private float clickStartTime = 0f;
    // debug 次數，預設為 0 次 ??? 看不懂
    private int debugThrottle = 0;
    // 每 FPS debug 次數，預設為 5 次 ??? 看不懂
    private int FRAMES_PER_DEBUG = 5;

    private CardboardControl cardboard;
    public CardboardControlDelegate OnUp = delegate { };
    public CardboardControlDelegate OnDown = delegate { };
    public CardboardControlDelegate OnClick = delegate { };


    public void Start() {
        cardboard = gameObject.GetComponent<CardboardControl>();
        magnet = new ParsedMagnetData();
        touch = new ParsedTouchData();
    }

    public void Update() {
        magnet.Update();
        touch.Update();
        if (useTouch)
            CheckTouch();
        if (useMagnet)
            CheckMagnet();
        CheckKey();
    }

    public void FixedUpdate() {
        if (printDebugInfo)
            PrintDebug();
    }

    private bool KeyFor(string direction) {
        switch (direction) {
            case "down":
            return Input.GetKeyDown(triggerKey);
            case "up":
            return Input.GetKeyUp(triggerKey);
            default:
            return false;
        }
    }

    private void CheckKey() {
        if (KeyFor("down") && cardboard.EventReady("OnDown"))
            ReportDown();
        if (KeyFor("up") && cardboard.EventReady("OnUp"))
            ReportUp();
    }

    private void CheckMagnet() {
        if (magnet.IsDown() && cardboard.EventReady("OnDown"))
            ReportDown();
        if (magnet.IsUp() && cardboard.EventReady("OnUp"))
            ReportUp();
    }

    private void CheckTouch() {
        if (touch.IsDown() && cardboard.EventReady("OnDown"))
            ReportDown();
        if (touch.IsUp() && cardboard.EventReady("OnUp"))
            ReportUp();
    }

    private bool IsTouching() {
        return Input.touchCount > 0;
    }

    private void ReportDown() {
        if (currentTriggerState == TriggerState.Up) {
            currentTriggerState = TriggerState.Down;
            OnDown(this);
            if (vibrateOnDown)
                Handheld.Vibrate();
            clickStartTime = Time.time;
        }
    }

    private void ReportUp() {
        if (currentTriggerState == TriggerState.Down) {
            currentTriggerState = TriggerState.Up;
            OnUp(this);
            if (vibrateOnUp)
                Handheld.Vibrate();
            CheckForClick();
        }
    }

    private void CheckForClick() {
        bool withinClickThreshold = SecondsHeld() <= clickSpeedThreshold;
        clickStartTime = 0f;
        if (withinClickThreshold && cardboard.EventReady("OnClick"))
            ReportClick();
    }

    private void ReportClick() {
        OnClick(this);
        if (vibrateOnClick)
            Handheld.Vibrate();
    }

    public float SecondsHeld() {
        return Time.time - clickStartTime;
    }

    public bool IsHeld() {
        return (currentTriggerState == TriggerState.Down);
    }

    public void ResetMagnetState() {
        magnet.ResetState();
    }

    private void PrintDebug() {
        debugThrottle++;
        if (debugThrottle >= FRAMES_PER_DEBUG) {
            magnet.PrintDebug();
            touch.PrintDebug();
            debugThrottle = 0;
        }
    }
}
