using UnityEngine;
using TMPro;

public class TwoColorText : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    void Start()
    {
        titleText.text = "<color=#357A71>ALHAI</color><color=#B9512F>KAVEH</color>";
        // titleText.text = "<color=#5D9582>AL</color><color=#357A71>HAI</color><color=#B9512F>KA</color><color=#D47E4B>V</color><color=#E4E0BE>EH</color>";
    }
}
