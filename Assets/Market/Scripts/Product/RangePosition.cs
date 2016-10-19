using UnityEngine;

public class RangePosition {
    private GameObject Range;
    private Transform T_Range;
    /// <summary>
    /// 所有 Range Y 軸 高度
    /// </summary>
    private float position_Y = 0.3f;
    /// <summary>
    /// 商品貨架 與 Range 之距離
    /// </summary>
    private float cabinet_range_D = 1.554f;
    /// <summary>
    /// 商品貨架 與 Range 之轉角距離
    /// </summary>
    private float cabinet_range_corner_D = 1.1f;
    /// <summary>
    /// 第一列 X 軸的位置
    /// </summary>
    public float firstAndfinal_X;
    /// <summary>
    /// 第一列 Z 軸的位置
    /// </summary>
    public float first_Z;
    /// <summary>
    /// 第二列 Z 軸的位置
    /// </summary>
    private float Second_Z;
    /// <summary>
    /// 最後一列 Z 軸的位置
    /// </summary>
    private float final_Z;
    /// <summary>
    /// front 與 Corner 距離
    /// </summary>
    private float front_corner_D_X = 0.414f;
    /// <summary>
    /// corner front Z 軸的位置
    /// </summary>
    private float corner_front_Z;
    /// <summary>
    /// corner back Z 軸的位置
    /// </summary>
    private float corner_back_Z;

    /// <summary>
    /// RCorner 角度
    /// </summary>
    private int corner_R = 45;

    private float front_Back_X_Scale = 3.25f;
    private float corner_X_Scale = 1.2f;
    private float Y_Scale = 0.6f;
    private float Z_Scale = 0.1f;

    Vector3 V_Position;
    Quaternion Q_Rotation;
    Vector3 V_Scale;

    /// <summary>
    /// 暫存 array
    /// </summary>
    private System.Collections.ArrayList temp;

    private ProductRandomPosition randomPosition = ProductManager.Instance.randomPosition;

    /// <summary>
    /// 自動產生 Cabinet Range
    /// </summary>
    public void SetCreate() {
        temp = new System.Collections.ArrayList();

        Range = GameObject.FindWithTag("Range");
        T_Range = Range.transform;

        SetRange_Front();
        SetRange_Back();
        SetRange_Row();
        SetRange_Corner_Front();
        SetRange_Corner_Back();
    }

    /// <summary>
    /// 摧毀所有 Range 物件
    /// </summary>
    public void Destroy() {
        // 清空 array (暫存)
        foreach (GameObject child in temp) {
            Object.Destroy(child);
        }

        temp.Clear();
    }

    private void SetRange_Front() {
        GameObject Range_Front = new GameObject();
        temp.Add(Range_Front);
        Range_Front.name = "Range_Front_Test";
        Transform T_Range_Front = Range_Front.transform;

        T_Range_Front.SetParent(T_Range, false);

        first_Z = randomPosition.first_Z - cabinet_range_D;
        V_Position = new Vector3(0, position_Y, first_Z);
        T_Range_Front.position = V_Position;

        Q_Rotation = Quaternion.Euler(0, 180, 0);
        T_Range_Front.rotation = Q_Rotation;

        for (int i = 0; i < 3; i++) {
            GameObject obj = new GameObject();
            obj.name = "Range_Front" + (i + 1);
            obj.transform.SetParent(T_Range_Front, false);

            firstAndfinal_X = randomPosition.twoRow_D + randomPosition.twoArea_D;
            if (i == 0)
                firstAndfinal_X = firstAndfinal_X * -1;
            else if (i == 1)
                firstAndfinal_X = 0;

            V_Position = new Vector3(firstAndfinal_X, position_Y, first_Z);
            V_Scale = new Vector3(front_Back_X_Scale, Y_Scale, Z_Scale);

            obj.transform.position = V_Position;
            obj.transform.localScale = V_Scale;
            obj.AddComponent<BoxCollider>();

            temp.Add(obj);
        }
    }

    private void SetRange_Back() {
        GameObject Range_Back = new GameObject();
        temp.Add(Range_Back);
        Range_Back.name = "Range_Back_Test";
        Transform T_Range_Back = Range_Back.transform;

        T_Range_Back.SetParent(T_Range, false);

        final_Z = randomPosition.final_Z + cabinet_range_D;
        V_Position = new Vector3(0, position_Y, final_Z);
        T_Range_Back.position = V_Position;

        Q_Rotation = Quaternion.Euler(0, 180, 0);
        T_Range_Back.rotation = Q_Rotation;

        for (int i = 0; i < 3; i++) {
            GameObject obj = new GameObject();
            obj.name = "Range_Back" + (i + 1);
            obj.transform.SetParent(T_Range_Back, false);

            firstAndfinal_X = randomPosition.twoRow_D + randomPosition.twoArea_D;
            if (i == 0)
                firstAndfinal_X = firstAndfinal_X * -1;
            else if (i == 1)
                firstAndfinal_X = 0;

            V_Position = new Vector3(firstAndfinal_X, position_Y, final_Z);
            V_Scale = new Vector3(front_Back_X_Scale, Y_Scale, Z_Scale);

            obj.transform.position = V_Position;
            obj.transform.localScale = V_Scale;
            obj.AddComponent<BoxCollider>();

            temp.Add(obj);
        }
    }

    private void SetRange_Row() {
        GameObject Range_Row = new GameObject();
        temp.Add(Range_Row);
        Range_Row.name = "Range_Row_Test";
        Transform T_Range_Row = Range_Row.transform;

        T_Range_Row.SetParent(T_Range, false);

        int count_Z = randomPosition.col / 2;
        bool count_Z_even = randomPosition.col % 2 == 0 ? true : false;
        float count_Z_center;
        if (count_Z_even)
            count_Z_center = (count_Z - 0.5f) * randomPosition.twoCol_D;
        else
            count_Z_center = count_Z * randomPosition.twoCol_D;

        Second_Z = randomPosition.second_Z + count_Z_center;
        V_Position = new Vector3(0, position_Y, Second_Z);
        T_Range_Row.position = V_Position;

        Q_Rotation = Quaternion.Euler(0, 90, 0);
        T_Range_Row.rotation = Q_Rotation;

        for (int i = 0; i < 6; i++) {
            GameObject obj = new GameObject();
            obj.name = "Range_Front" + (i + 1);
            obj.transform.SetParent(T_Range_Row, false);

            if (i == 0)
                firstAndfinal_X = -2 * randomPosition.twoRow_D - randomPosition.twoArea_D - cabinet_range_D;
            else if (i == 1)
                firstAndfinal_X = -1 * randomPosition.twoArea_D + cabinet_range_D;
            else if (i == 2)
                firstAndfinal_X = -1 * randomPosition.twoRow_D - cabinet_range_D;
            else if (i == 3)
                firstAndfinal_X = randomPosition.twoRow_D + cabinet_range_D;
            else if (i == 4)
                firstAndfinal_X = randomPosition.twoArea_D - cabinet_range_D;
            else
                firstAndfinal_X = 2 * randomPosition.twoRow_D + randomPosition.twoArea_D + cabinet_range_D;

            V_Position = new Vector3(firstAndfinal_X, position_Y, Second_Z);
            V_Scale = new Vector3(10.75f, Y_Scale, Z_Scale);

            obj.transform.position = V_Position;
            obj.transform.localScale = V_Scale;
            obj.AddComponent<BoxCollider>();

            temp.Add(obj);
        }
    }

    private void SetRange_Corner_Front() {
        GameObject Range_Corner_Front = new GameObject();
        temp.Add(Range_Corner_Front);
        Range_Corner_Front.name = "Range_Corner_Front_Test";
        Transform T_Range_Corner_Front = Range_Corner_Front.transform;

        T_Range_Corner_Front.SetParent(T_Range, false);

        corner_front_Z = first_Z + front_corner_D_X;
        V_Position = new Vector3(0, position_Y, corner_front_Z);
        T_Range_Corner_Front.position = V_Position;

        Q_Rotation = Quaternion.Euler(0, 0, 0);
        T_Range_Corner_Front.rotation = Q_Rotation;

        for (int i = 0; i < 6; i++) {
            GameObject obj = new GameObject();
            obj.name = "Range_Front" + (i + 1);
            obj.transform.SetParent(T_Range_Corner_Front, false);

            if (i == 0) {
                firstAndfinal_X = -2 * randomPosition.twoRow_D - randomPosition.twoArea_D - cabinet_range_D + front_corner_D_X;
                corner_R = 45;
            } else if (i == 1) {
                firstAndfinal_X = -1 * randomPosition.twoArea_D + cabinet_range_D - front_corner_D_X;
                corner_R = -45;
            } else if (i == 2) {
                firstAndfinal_X = -1 * randomPosition.twoRow_D - cabinet_range_D + front_corner_D_X;
                corner_R = 45;
            } else if (i == 3) {
                firstAndfinal_X = randomPosition.twoRow_D + cabinet_range_D - front_corner_D_X;
                corner_R = -45;
            } else if (i == 4) {
                firstAndfinal_X = randomPosition.twoArea_D - cabinet_range_D + front_corner_D_X;
                corner_R = 45;
            } else {
                firstAndfinal_X = 2 * randomPosition.twoRow_D + randomPosition.twoArea_D + cabinet_range_D - front_corner_D_X;
                corner_R = -45;
            }

            V_Position = new Vector3(firstAndfinal_X, position_Y, corner_front_Z);
            Q_Rotation = Quaternion.Euler(0, corner_R, 0);
            V_Scale = new Vector3(corner_X_Scale, Y_Scale, Z_Scale);

            obj.transform.position = V_Position;
            obj.transform.rotation = Q_Rotation;
            obj.transform.localScale = V_Scale;
            obj.AddComponent<BoxCollider>();

            temp.Add(obj);
        }
    }

    private void SetRange_Corner_Back() {
        GameObject Range_Corner_Back = new GameObject();
        temp.Add(Range_Corner_Back);
        Range_Corner_Back.name = "Range_Corner_Back_Test";
        Transform T_Range_Corner_Back = Range_Corner_Back.transform;

        T_Range_Corner_Back.SetParent(T_Range, false);

        corner_back_Z = final_Z - front_corner_D_X;
        V_Position = new Vector3(0, position_Y, corner_back_Z);
        T_Range_Corner_Back.position = V_Position;

        Q_Rotation = Quaternion.Euler(0, 0, 0);
        T_Range_Corner_Back.rotation = Q_Rotation;

        for (int i = 0; i < 6; i++) {
            GameObject obj = new GameObject();
            obj.name = "Range_Front" + (i + 1);
            obj.transform.SetParent(T_Range_Corner_Back, false);

            if (i == 0) {
                firstAndfinal_X = -2 * randomPosition.twoRow_D - randomPosition.twoArea_D - cabinet_range_D + front_corner_D_X;
                corner_R = -45;
            } else if (i == 1) {
                firstAndfinal_X = -1 * randomPosition.twoArea_D + cabinet_range_D - front_corner_D_X;
                corner_R = 45;
            } else if (i == 2) {
                firstAndfinal_X = -1 * randomPosition.twoRow_D - cabinet_range_D + front_corner_D_X;
                corner_R = -45;
            } else if (i == 3) {
                firstAndfinal_X = randomPosition.twoRow_D + cabinet_range_D - front_corner_D_X;
                corner_R = 45;
            } else if (i == 4) {
                firstAndfinal_X = randomPosition.twoArea_D - cabinet_range_D + front_corner_D_X;
                corner_R = -45;
            } else {
                firstAndfinal_X = 2 * randomPosition.twoRow_D + randomPosition.twoArea_D + cabinet_range_D - front_corner_D_X;
                corner_R = 45;
            }

            V_Position = new Vector3(firstAndfinal_X, position_Y, corner_back_Z);
            Q_Rotation = Quaternion.Euler(0, corner_R, 0);
            V_Scale = new Vector3(corner_X_Scale, Y_Scale, Z_Scale);

            obj.transform.position = V_Position;
            obj.transform.rotation = Q_Rotation;
            obj.transform.localScale = V_Scale;
            obj.AddComponent<BoxCollider>();

            temp.Add(obj);
        }
    }
}
