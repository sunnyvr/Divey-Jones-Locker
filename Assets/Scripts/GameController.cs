using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Text coinText;
    private float coinCount;

    void Start()
    {
        coinCount = 0;
    }

    public void collect()
    {
        coinCount++;
        coinText.text = "Coins Found: " + coinCount + "/5";
    }
}
