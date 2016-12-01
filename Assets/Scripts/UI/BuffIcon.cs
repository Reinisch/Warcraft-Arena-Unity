using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BuffIcon : MonoBehaviour 
{
    public Image IconImage { get; set; }
    public Text TimerText { get; set; }

    public void Initialize()
    {
        IconImage = transform.FindChild("Icon Button").GetComponent<Image>();
        TimerText = transform.FindChild("Timer").GetComponent<Text>();
    }
}
