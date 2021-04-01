using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{

  public TextMeshProUGUI timerText;

  private int timeOffsetInSec;

  // Start is called before the first frame update
  void Start()
  {
    SetTimeOffset(0);
  }

  // Update is called once per frame
  void LateUpdate()
  {
    RefreshTimer();
  }

  public void SetTimeOffset(int secs)
  {
    timeOffsetInSec = secs;
  }

  public void IncreaseTimeOffset(int deltaSecs)
  {
    timeOffsetInSec += deltaSecs;
  }

  private void RefreshTimer() {
    int time = Mathf.Max(0 , ((int)Time.time) + timeOffsetInSec);
    timerText.text = ((int)time/60).ToString("00") + ":" + ((int)time%60).ToString("00");
  }
}
