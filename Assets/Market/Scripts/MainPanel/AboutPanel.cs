using UnityEngine;

public class AboutPanel : MonoBehaviour {
    public void OnClickOpenAboutPanel() {
        gameObject.SetActive(true);
    }

    public void OnClickCloseAboutPanel() {
        gameObject.SetActive(false);
    }
}
