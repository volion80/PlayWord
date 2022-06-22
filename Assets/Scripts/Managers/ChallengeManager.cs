using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChallengeManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status {get; private set;}

    private ChallengeStatus ChallengeStatus {get; set;}
    
    private NetworkService _network;
    
    private string StartWordText { get; set; }
    private int TimerValue { get; set; }

    private float _letterSize;
    public float defaultLetterSize = 64f;
    
    public void Startup(NetworkService service)
    {
        Debug.Log("Challenge manager starting...");
        _network = service;

        status = ManagerStatus.Started;
    }

    public void SetupNew(string wordText, string timerText)
    {
        StartWordText = wordText.Replace(" ", "");
        TimerValue = Convert.ToInt32(timerText);
        DoStart();
    }
    
    public void DoStart()
    {
        Restart();
    }
    
    public void Restart()
    {
        string sceneName = "Challenge";
        Debug.Log("Loading " + sceneName);
        SceneManager.LoadScene(sceneName);
    }

    public string GetStartWordText()
    {
        return StartWordText;
    }

    public int GetTimerValue()
    {
        return TimerValue;
    }

    public bool IsSetStartWord()
    {
        return !string.IsNullOrEmpty(StartWordText);
    }

    public void UpdateChallengeStatus(ChallengeStatus st)
    {
        ChallengeStatus = st;
    }

    public void Reload()
    {
        StartWordText = null;
        Restart();
    }

    public void CalculateLetterSize(float maxWidth)
    {
        _letterSize = defaultLetterSize;
        
        float startWordLength = GetStartWordText().Length;
        
        while (startWordLength * _letterSize > maxWidth)
        {
            _letterSize -= 4f;
        }
    }

    public float GetLetterSize()
    {
        return _letterSize;
    }
}
