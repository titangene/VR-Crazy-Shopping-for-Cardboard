using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

    public Text ScoreText;
    /// <summary>
    /// 商品 Tag
    /// </summary>
    public string ProductTag = "Product";

    private int score = 0;

    void Awake() {
        ScoreText = GameObject.FindWithTag("Score").GetComponent<Text>();
    }

    /// <summary>
    /// 商品放入購物車時，將商品價格加入分數
    /// </summary>
    void OnTriggerEnter(Collider other) {
        if (other.tag == ProductTag) {
            int productID = int.Parse(other.name.Substring(7, 4));
            int json_ID = productID - 1;
            int Price = int.Parse(ProductManager.Instance.json["product"][json_ID]["price"].ToString());
            Debug.Log("IN：" + productID + "：" + Price);
            score += Price;
            ScoreText.text = "$" + score.ToString();
        }
    }

    /// <summary>
    /// 商品離開購物車時，會將分數減去商品價格
    /// </summary>
    void OnTriggerExit(Collider other) {
        if (other.tag == ProductTag) {
            int productID = int.Parse(other.name.Substring(7, 4));
            int json_ID = productID - 1;
            int Price = int.Parse(ProductManager.Instance.json["product"][json_ID]["price"].ToString());
            Debug.Log("IN：" + productID + "：" + Price);
            score -= Price;
            ScoreText.text = "$" + score.ToString();
        }
    }
}
