using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{

  public TextMeshProUGUI timerText;
  public int framesForUpdate;
  

  private int timeUpdate;
  private int frames;
  private int timeOffsetInSec;
  private float startTime;

  // Start is called before the first frame update
  void Start()
  {
    SetTimeOffset(0);
    Init();
  }

  public void Init()
  {
    timeOffsetInSec = 0;
    startTime       = Time.time;
  }

  // Update is called once per frame
  void LateUpdate()
  {
    RefreshTimer();

    if (frames < framesForUpdate)
    {
      PrintUpdateTime();
      frames++;
    }
  }

  public void SetTimeOffset(int secs)
  {
    timeOffsetInSec = secs;
  }

  public void IncreaseTimeOffset(int deltaSecs)
  {
    timeOffsetInSec += deltaSecs;
    frames = 0;
    timeUpdate = deltaSecs;
  }

  private void RefreshTimer() {
    int time = Mathf.Max(0 , ((int)(Time.time - startTime)) + timeOffsetInSec);
    timerText.text = ((int)time/60).ToString("00") + ":" + ((int)time%60).ToString("00");
  }

  private void PrintUpdateTime() {
    timerText.text += " (" + timeUpdate.ToString("00") + "s)";
  }
}
