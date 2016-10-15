using UnityEngine;

public class Countdown : MonoBehaviour {
    // 倒數時間
    public float TimerStart = 99.0f;
    // 結束時間
    public float TimerEnd = 0.0f;
    // 提醒時間即將結束
    public float TimeReminder = 30.0f;
    // 倒數時間開始
    public bool CounterStart = false;
    // 時間倒數結束
    public bool CounterEnd = false;
    // 時間暫停
    public bool CounterPause = false;
    // 剩下幾秒提醒
    public bool ShowRemaining = false;
    // 遊戲時間文字
    public TextMesh Timer;

    // 按鍵盤 F 鍵 開始時間
    private KeyCode TimeStartKey = KeyCode.F;
    // 按鍵盤 G 鍵 重設時間
    private KeyCode TimeResetKey = KeyCode.G;
    // 按鍵盤 H 鍵 暫停時間
    private KeyCode TimePauseKey = KeyCode.H;
    // 秒
    private float Seconds;
    // 毫秒
    private float MiliSeconds;

    // 初始化時間變數或遊戲狀態
    void Start() {
        // 設定開始倒數時間 EX：99:00
        Timer.text = TimerStart.ToString() + ":00";
    }

    void Update() {
        if (Input.GetKeyDown(TimeStartKey)) {
            // 從遊戲第 1 秒開始，每 0.01 秒重覆執行 "Countdown1" method
            InvokeRepeating("Countdown1", 1.0f, 0.01f);
        }

        /*
        // 按鍵盤 F 鍵 倒數時間開始
        if (Input.GetKeyUp(TimeStartKey)) {
            StartTimer();
        }
        
        // 按鍵盤 G 鍵 重設時間
        if (Input.GetKeyUp(TimeResetKey)) {
            ResetTimer();
        }

        // 更新倒數時間
        UpdateTimer();

        // 提醒時間
        Time_Reminder();

        // 按鍵盤 H 鍵 暫停時間
        if (Input.GetKeyUp(TimePauseKey)) {
            PauseTimer();
        }

        // 時間倒數結束
        EndTimer();
        */
    }

    private void Countdown1() {
        TimerStart -= 0.01f;
        // 毫秒
        float msec = TimerStart - (int) TimerStart;
        // Timer 文字
        string timerStr = string.Format("{0}:{1}", (int) TimerStart, msec.ToString().Substring(2, 2));
        // 套用文字至 Timer
        Timer.text = timerStr;
        // 提醒時間
        string timeReminderStr = TimeReminder + ":00";

        if (timerStr == timeReminderStr) {
            Debug.Log("剩下 " + TimeReminder + " 秒！！！");
        }

        if (TimerStart <= 0) {
            Timer.text = "0:00";
            CancelInvoke("Countdown1");
        }
        
    }

    // 倒數時間開始，初始化時間變數或遊戲狀態
    private void StartTimer() {
        if (CounterStart == false) {
            // 將倒數時間設定至 Seconds 變數
            Seconds = TimerStart;
            // 毫秒重設為 0
            MiliSeconds = 0.0f;
            // 將倒數時間開始改成 true
            CounterStart = !CounterStart;
            // 該改 暫停時間 狀態
            CounterPause = false;
        }
        // 列印幾秒
        Debug.Log(Timer.text);
    }

    // 更新倒數時間
    private void UpdateTimer() {
        // 未暫停時間 or 時間倒數未結束
        if (CounterStart == true & CounterPause == false & CounterEnd == false) {
            // 設定秒、毫秒，EX：1:00 -> 0:99
            if (Seconds > 0 & MiliSeconds <= 0) {
                Seconds--;
                MiliSeconds = 99;
            }
            // 更新時間 至 時間文字，如果毫秒 <= 9 會自動在數字前補一個0
            if ((int) MiliSeconds > 9) {
                Timer.text = string.Format("{0}:{1}", Seconds, (int) MiliSeconds);
            } else {
                Timer.text = string.Format("{0}:0{1}", Seconds, (int) MiliSeconds);
            }

            // 列印幾秒
            Debug.Log(Timer.text);

            // 每毫秒 - 1 毫秒
            MiliSeconds -= Time.deltaTime * 100;

            // 時間倒數結束時，將時間設成 0:00
            if (Seconds <= 0 & (int) MiliSeconds <= 1) {
                Timer.text = "0:00";
            }
        }
    }

    // 提醒時間
    private void Time_Reminder() {
        // 剩下幾秒
        if (ShowRemaining == false & Seconds == TimeReminder & (int) MiliSeconds <= 0) {
            Debug.Log("剩下 " + TimeReminder + " 秒！！！");
            // 剩下幾秒提醒
            ShowRemaining = true;
        }
    }

    // 時間倒數結束
    private void EndTimer() {
        if (CounterStart == true & CounterEnd == false & Seconds == TimerEnd & (int) MiliSeconds <= 0) {
            Debug.Log("時間倒數結束，遊戲回合結束！！！");
            // 將時間倒數結束改成 true
            CounterEnd = true;
            // 將倒數時間設定至 Seconds 變數
            Seconds = TimerStart;
            // 毫秒重設為 0
            MiliSeconds = 0.0f;
        }
    }

    // 按鍵盤 H 鍵 暫停時間
    private void PauseTimer() {
        if (CounterPause == false) Debug.Log("時間暫停");
        if (CounterPause == true) Debug.Log("時間開始");
        // 該改 暫停時間 狀態
        CounterPause = !CounterPause;
    }

    // 按鍵盤 G 鍵 重設時間
    private void ResetTimer() {
        Debug.Log("重設時間");
        // 設定開始倒數時間 EX：99:00
        Timer.text = TimerStart.ToString() + ":00";
        // 將倒數時間設定至 Seconds 變數
        Seconds = TimerStart;
        // 毫秒重設為 0
        MiliSeconds = 0.0f; 
        // 將倒數時間開始改成 false
        CounterStart = false;
        // 將時間倒數結束改成 false
        CounterEnd = false;
        // 該改 暫停時間 狀態
        CounterPause = false;
        // 將時間提醒改成 false
        ShowRemaining = false;
    }
}
