using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Collectables : MonoBehaviour
{
    public TextMeshProUGUI collectedText;

    private int totalCollectables;
    private int collected;

    private void Awake() {
        Init();
    }

    public void SetTotalCollectables(int total)
    {
        totalCollectables = total;
        RefreshText();
    }

    public void Init()
    {
        collected = 0;
        totalCollectables = 0;
        RefreshText();
    }

    public void IncrementCollected()
    {
        collected++;
        RefreshText();
    }

    private void RefreshText()
    {
        collectedText.text = collected.ToString("00") + "/" + totalCollectables.ToString("00");
    }

}
