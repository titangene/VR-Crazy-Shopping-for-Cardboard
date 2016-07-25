using UnityEngine;
using System.Collections;
/// <summary>
/// 找出 Layer
/// </summary>
public class FindLayer : MonoBehaviour {

    /// <summary>
    /// 要找出的 Layer 名稱
    /// </summary>
    //public string FindLayerName;

    /// <summary>
    /// 要放入某子物件內
    /// </summary>
    //public Transform ObjectParent;

    /// <summary>
    /// 找出所有物件
    /// </summary>
    private GameObject[] AllGameObjArray;

    /// <summary>
    /// 將所有是某 Layer 的物件放入某子物件內
    /// </summary>
    /// <param name="LayerName">要找出的 Layer 名稱</param>
    /// <param name="ObjParent">要放入某子物件內</param>
    public void PlacedObjectParent(string FindLayerName, Transform ObjectParent) {
        // 找出所有物件
        AllGameObjArray = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        foreach (GameObject GameObj in AllGameObjArray) {
            // 找出所有是某 Layer 的物件
            if (GameObj.layer == LayerMask.NameToLayer(FindLayerName)) {
                // 將所有是某 Layer 的物件放入某子物件內
                GameObj.transform.parent = ObjectParent;
            }
        }
    }
}
