using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class DebugLogOutput : MonoBehaviour {
    /// <summary>
    /// Log 訊息 的完整目錄
    /// </summary>
    string FullPath;
    static List<string> WriteStr = new List<string>();

    /// <summary>
    /// 開始執行時間
    /// </summary>
    string StartNowTime;

    /// <summary>
    /// 上一次執行時間
    /// </summary>
    string PreviousTime;

    /// <summary>
    /// 開始裝態，用來寫入 --執行開始時間-- 作為區隔
    /// </summary>
    bool IsStart;

    void Start() {
        // 目錄
        string Path = Application.dataPath + "/DebugLog/";

        // 建立目錄
        CreateDirectory(Path);

        // 現在時間
        DateTime now = DateTime.Now;
        // 設定時間格式 (開始執行時間，用於 DebugOutput.log 檔案名稱)
        string StartTimePathName = string.Format("{0:yyyy-MM-dd_H}", now);

        // 設定 DebugOutput 的檔案位置 (D:\YourProject\Assets\DebugLog\Year-Month-Day_Hour_DebugOutput.log)
        FullPath = Path + StartTimePathName + "_DebugOutput.log";

        // 設定時間格式 (用於寫入 --執行開始時間-- 作為區隔)
        StartNowTime = string.Format("{0:yyyy/MM/dd H:mm:ss}", now);

        // 每次刪除之前保存的 Log
        /*
        if (File.Exists(fullPath))
            File.Delete(fullPath);
        */

        // Log 的監聽，事件如果接收到 Log 訊息 會被觸發 (只會在 Main Thread 觸發)
        Application.logMessageReceived += HandleLog;
        // 舊方法
        //Application.RegisterLogCallback(HandleLog);

        // 將 開始 狀態 開啟
        IsStart = true;
    }

    /// <summary>
    /// 建立目錄
    /// </summary>
    /// <param name="path">目錄</param>
    private static void CreateDirectory(string path) {
        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }
    }

    void Update() {
        // 因為寫入文件的操作必須在 Main Thread 中完成，所以在 Update 中寫入文件
        if (WriteStr.Count > 0) {
            string[] temp = WriteStr.ToArray();
            foreach (string t in temp) {
                using (StreamWriter writer = new StreamWriter(FullPath, true, Encoding.UTF8))
                    writer.WriteLine(t);
                WriteStr.Remove(t);
            }
        }
    }

    /// <summary>
    /// 寫入 時間、Log 訊息：
    /// 1. 執行開始時間，
    /// 2. Log 訊息 出現的時間，
    /// 3. Log 訊息，
    /// 4. Log 訊息 的詳細追蹤內容
    /// </summary>
    private void HandleLog(string condition, string stackTrace, LogType type) {
        // 每次執行開始時，要寫入 --執行開始時間-- 作為區隔
        if (IsStart) {
            WriteStr.Add("");
            WriteStr.Add("---------------------------- " + StartNowTime + " ----------------------------");
            // 將 開始 狀態 關閉
            IsStart = false;
        }

        // 現在時間
        DateTime now = DateTime.Now;
        // 設定時間格式 (接收到 Log 訊息 時間)
        string NowTime = string.Format("[ {0:yyyy/MM/dd H:mm:ss} ]", now);

        // 第一次接收到 Log 訊息 或 在同一時間接收到 Log 訊息 時，才會紀錄現在時間
        if (PreviousTime == null || (PreviousTime != null && NowTime != PreviousTime))
            WriteStr.Add(NowTime);

        // 接收到 Error 或 Exception 類型的 Log 訊息時，加強註明
        if (type == LogType.Error || type == LogType.Exception)
            WriteStr.Add("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");

        // 寫入 Log 訊息
        WriteStr.Add(condition);

        // 寫入 Log 訊息 的詳細追蹤內容
        WriteStr.Add(stackTrace);

        // 紀錄現在時間，用來檢察下一次 Log 訊息 的時間是否跟這次一樣
        PreviousTime = NowTime;
    }
}
