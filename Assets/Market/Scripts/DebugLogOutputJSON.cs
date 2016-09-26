using UnityEngine;
using System;
using System.IO;
using System.Text;
using LitJson;

/// <summary>
/// 將執行階段的所有 Debug Log 訊息 Output 成 Json 檔，
/// 會在每個小時分別產生 Json 檔
/// </summary>
public class DebugLogOutputJSON : MonoBehaviour {
    /// <summary>
    /// Log 訊息 的完整目錄
    /// </summary>
    private string FullPath;

    /// <summary>
    /// 開始執行時間
    /// </summary>
    private string StartNowTime;

    /// <summary>
    /// 上一次執行時間
    /// </summary>
    private string PreviousTime;

    /// <summary>
    /// 開始裝態，用來寫入 --執行開始時間-- 作為區隔
    /// </summary>
    private bool IsStart;

    private JsonData json;

    // 簡化觀看 JsonData 用的
    // json["debug"][DebugID]
    private JsonData JsonDebug;
    // JsonDebug["log"][LogID]
    private JsonData JsonDebug_Log;
    // JsonDebug_Log["content"][LogContentID]
    private JsonData JsonDebug_Log_Content;

    // Json 內的編號 ( json["debug"][DebugID]["log"][LogID]["content"][LogContentID] )
    private int DebugID = 0;
    private int LogID = -1;
    private int LogContentID = 0;

    /// <summary>
    /// DebugLog ID，每次執行都從 1 開始編號
    /// </summary>
    private int DebugLogID = 0;

    void Start() {
        // 目錄
        string Path = Application.dataPath + "/DebugLog/";

        // 建立目錄
        CreateDirectory(Path);

        // 現在時間
        DateTime now = DateTime.Now;
        // 設定時間格式 (開始執行時間，用於 DebugOutput.log 檔案名稱)
        string StartTimePathName = string.Format("{0:yyyy-MM-dd_HH}", now);

        // 設定 DebugOutput 的檔案位置
        // D:\YourProject\Assets\DebugLog\Year-Month-Day_Hour_DebugOutput.json
        FullPath = Path + StartTimePathName + "_DebugOutput.json";

        // 設定時間格式 (用於寫入 執行開始時間)
        StartNowTime = string.Format("{0:yyyy/MM/dd HH:mm:ss}", now);

        // 檔案是否存在
        if (!File.Exists(FullPath)) {
            // 新建 JsonData
            json = new JsonData();
            // 寫入 Log 訊息
            json["debug"] = new JsonData();
            json["debug"].SetJsonType(JsonType.Array);
            //Debug.Log("新建 JsonData");

        } else {
            // 讀取 Json 檔
            ReadJson(LoadJsonFile());
            // 設定 DebugID
            DebugID = json["debug"].Count;
            //Debug.Log("讀取 Json File");
        }

        DeleteFile();

        // Log 的監聽，事件如果接收到 Log 訊息 會被觸發 (只會在 Main Thread 觸發)
        Application.logMessageReceived += HandleLog;

        // 將 開始 狀態 開啟
        IsStart = true;
    }

    /// <summary>
    /// 建立目錄
    /// </summary>
    /// <param name="path">目錄</param>
    private void CreateDirectory(string path) {
        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }
    }

    /// <summary>
    /// 讀取 Json 檔
    /// </summary>
    /// <param name="jsonStr">搭配 LoadJsonFile() 使用：ReadJson(LoadJsonFile())</param>
    private void ReadJson(string jsonStr) {
        json = JsonMapper.ToObject(jsonStr);
    }

    /// <summary>
    /// 讀入 Json 檔的內容
    /// </summary>
    private string LoadJsonFile() {
        //用來儲存讀入的Json內容
        StringBuilder sbJson = new StringBuilder();
        using (StreamReader sr = new StreamReader(FullPath)) {
            //一次性將資料全部讀入
            sbJson.Append(sr.ReadToEnd());
        }

        return sbJson.ToString();
    }

    /// <summary>
    /// 刪除 Json 檔
    /// </summary>
    private void DeleteFile() {
        if (File.Exists(FullPath)) {
            File.Delete(FullPath);
        }
    }

    /// <summary>
    /// 寫入 Json 資料 && 美化 Json
    /// </summary>
    private string WriteJsonAndPrettyPrint() {
        JsonWriter jsonwriter = new JsonWriter();
        // 美化 Json
        jsonwriter.PrettyPrint = true;
        // 縮排大小
        jsonwriter.IndentValue = 2;
        JsonMapper.ToJson(json, jsonwriter);

        string str = jsonwriter.ToString();
        return str;
    }

    /// <summary>
    /// 將資料寫入 Json 檔
    /// </summary>
    private void OutputJsonFile() {
        using (StreamWriter sw = new StreamWriter(FullPath, true, Encoding.UTF8)) {
            sw.WriteLine(WriteJsonAndPrettyPrint());
        }
    }

    /// <summary>
    /// 結束執行時：紀錄結束執行時間、將資料寫入 Json 檔
    /// </summary>
    void OnDisable() {
        int Log_Count;

        // 有 Log 訊息時才會計算 Log 訊息總數，沒有則設為 1 (列印訊息數量本身也是 Log 訊息)
        if (DebugLogID > 0) {
            Log_Count = JsonDebug["log"].Count + 1;
        } else {
            DebugLogID++;
            Log_Count = DebugLogID;
        }

        int LogContent_Count = DebugLogID;

        Debug.Log("執行結束！DebugID:" + DebugID + ", Log_Count:" + Log_Count + ", LogContent_Count:" + LogContent_Count);

        // 現在時間
        DateTime now = DateTime.Now;
        // 設定時間格式 (接收到 Log 訊息 時間)
        string NowTime = string.Format("{0:yyyy/MM/dd HH:mm:ss}", now);
        JsonDebug["endtime"] = NowTime;

        // 將資料寫入 Json 檔
        OutputJsonFile();
    }
    
    /*
    void OnApplicationQuit() {
        Debug.Log("OnApplicationQuit");
    }
    */

    /// <summary>
    /// 寫入 Log 訊息：執行開始時間、訊息出現的時間、訊息、詳細追蹤內容
    /// </summary>
    private void HandleLog(string condition, string stackTrace, LogType type) {
        // 每次執行開始時，要寫入 Log 訊息：執行開始時間、訊息
        if (IsStart) {
            JsonAdd_Debug(condition, stackTrace, type);
            // 將 開始 狀態 關閉
            IsStart = false;
        }

        // 現在時間
        DateTime now = DateTime.Now;
        // 設定時間格式 (接收到 Log 訊息 時間)
        string NowTime = string.Format("{0:yyyy/MM/dd HH:mm:ss}", now);

        // 第一次接收到 Log 訊息 或 在同一時間接收到 Log 訊息 時，才會紀錄現在時間
        if (PreviousTime == null || NowTime != PreviousTime) {
            // 寫入 Log 訊息：出現的時間、訊息詳細內容
            JsonAdd_Debug_Log(NowTime, condition, stackTrace, type);
        }

        // 寫入 Log 訊息詳細內容：ID、型態、內容、詳細追蹤內容
        JsonAdd_Debug_Log_Content(condition, stackTrace, type);

        // 紀錄現在時間，用來檢察下一次 Log 訊息 的時間是否跟這次一樣
        PreviousTime = NowTime;
    }

    /// <summary>
    /// 寫入 Log 訊息：執行開始時間、Log 訊息
    /// </summary>
    private void JsonAdd_Debug(string condition, string stackTrace, LogType type) {
        json["debug"].Add(new JsonData());
        JsonDebug = json["debug"][DebugID];

        //JsonDebug["DebugID"] = DebugID;
        JsonDebug["runtime"] = StartNowTime;
        JsonDebug["endtime"] = "";
        JsonDebug["log"] = new JsonData();
        JsonDebug["log"].SetJsonType(JsonType.Array);
    }

    /// <summary>
    /// 寫入 Log 訊息：Log 訊息出現的時間、Log 訊息詳細內容
    /// </summary>
    private void JsonAdd_Debug_Log(string NowTime, string condition, string stackTrace, LogType type) {
        JsonDebug["log"].Add(new JsonData());
        LogID++;
        JsonDebug_Log = JsonDebug["log"][LogID];

        //JsonDebug_Log["LogID"] = LogID;
        JsonDebug_Log["time"] = NowTime;
        JsonDebug_Log["content"] = new JsonData();
        JsonDebug_Log["content"].SetJsonType(JsonType.Array);

        LogContentID = 0;
    }

    /// <summary>
    /// 寫入 Log 訊息詳細內容：ID、型態、內容、詳細追蹤內容
    /// </summary>
    private void JsonAdd_Debug_Log_Content(string condition, string stackTrace, LogType type) {
        JsonDebug_Log["content"].Add(new JsonData());
        JsonDebug_Log_Content = JsonDebug_Log["content"][LogContentID];

        // 設定下一個訊息 ID
        DebugLogID++;

        //JsonDebug_Log_Content["LogContentID"] = LogContentID;
        JsonDebug_Log_Content["id"] = DebugLogID;
        JsonDebug_Log_Content["logtype"] = type.ToString();
        JsonDebug_Log_Content["message"] = condition;
        JsonDebug_Log_Content["stackTrace"] = stackTrace;

        LogContentID++;
    }
}
