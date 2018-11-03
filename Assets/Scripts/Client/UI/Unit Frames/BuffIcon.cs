using UnityEngine;
using UnityEngine.UI;

public class BuffIcon : MonoBehaviour 
{
    public Image IconImage { get; set; }
    public Text TimerText { get; set; }

    public void Initialize()
    {
        IconImage = transform.Find("Icon Button").GetComponent<Image>();
        TimerText = transform.Find("Timer").GetComponent<Text>();
    }
}
