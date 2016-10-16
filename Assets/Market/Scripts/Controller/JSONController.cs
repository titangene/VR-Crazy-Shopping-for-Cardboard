using LitJson;
using System.IO;
using System.Text;

public class JSONController {
    /// <summary>
    /// JSON 目錄
    /// </summary>
    private string path;
    /// <summary>
    /// JSON 完整目錄
    /// </summary>
    private string fullPath;


    public string SetPath() {
#if UNITY_EDITOR
        path = UnityEngine.Application.dataPath;
#endif

#if UNITY_ANDROID
        path = UnityEngine.Application.persistentDataPath;
#endif

#if UNITY_IOS
        path = UnityEngine.Application.persistentDataPath;
#endif
        return path;
    }

    public string SetAllPath(string path, string filePath) {
        fullPath = path + filePath;
        return fullPath;
    }

    /// <summary>
    /// 建立目錄
    /// </summary>
    public void CreateDirectory(string path) {
        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }
    }

    /// <summary>
    /// 合併 ReadJson 與 LoadJsonFile 功能。
    /// ReadJson：讀取 Json 檔，
    /// LoadJsonFile：讀入 Json 檔的內容
    /// </summary>
    public JsonData ReadJson_LoadJsonFile(string fullPath) {
        string jsonStr = LoadJsonFile(fullPath);
        return JsonMapper.ToObject(jsonStr);
    }

    /// <summary>
    /// 讀取 Json 檔，需搭配 LoadJsonFile() 使用：ReadJson(LoadJsonFile())
    /// </summary>
    public JsonData ReadJson(string jsonStr) {
        return JsonMapper.ToObject(jsonStr);
    }

    /// <summary>
    /// 讀入 Json 檔的內容
    /// </summary>
    public string LoadJsonFile(string fullPath) {
        //用來儲存讀入的Json內容
        StringBuilder sbJson = new StringBuilder();
        using (StreamReader sr = new StreamReader(fullPath)) {
            //一次性將資料全部讀入
            sbJson.Append(sr.ReadToEnd());
        }

        return sbJson.ToString();
    }

    /// <summary>
    /// 刪除 Json 檔
    /// </summary>
    public void DeleteFile(string fullPath) {
        if (File.Exists(fullPath)) {
            File.Delete(fullPath);
        }
    }

    /// <summary>
    /// 寫入 Json 資料 && 美化 Json
    /// </summary>
    public string WriteJsonAndPrettyPrint(JsonData json) {
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
    public void OutputJsonFile(JsonData json, string fullPath) {
        using (StreamWriter sw = new StreamWriter(fullPath, true, Encoding.UTF8)) {
            string jsonStr = WriteJsonAndPrettyPrint(json);
            sw.WriteLine(jsonStr);
        }
    }
}
