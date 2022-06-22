using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject aboutButton;
    [SerializeField] private GameObject addWordButton;
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject stopButton;
    [SerializeField] private GameObject backButton;
    [SerializeField] private GameObject quitButton;
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject okResultButton;

    [SerializeField] private StartPopup startPopup;
    [SerializeField] private ResultPopup resultPopup;
    [SerializeField] private SettingsPopup settingsPopup;
    [SerializeField] private AboutPopup aboutPopup;
    
    [SerializeField] private GameObject wordScore;
    [SerializeField] private GameObject timer;
    
    [SerializeField] private GameObject introText;
    
    [SerializeField] private AudioClip defaultSound;
    [SerializeField] private AudioClip addWordSound;
    [SerializeField] private AudioClip stopGameSound;
    
    
    private SceneController _controller;
    
    private float _timeRemaining;
    private bool _timerIsRunning;
    
    void Awake()
    {
        Messenger.AddListener(GameEvent.WORD_UPDATED, OnWordUpdated);
        Messenger.AddListener(GameEvent.CHALLENGE_INIT, OnChallengeInit);
        Messenger.AddListener(GameEvent.CHALLENGE_SETUP, OnChallengeSetup);
        Messenger.AddListener(GameEvent.CHALLENGE_STARTED, OnChallengeStarted);
        Messenger.AddListener(GameEvent.CHALLENGE_RELOAD, OnChallengeReload);
        Messenger.AddListener(GameEvent.WORD_ADDED, OnWordAdded);
    }
    
    void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.WORD_UPDATED, OnWordUpdated);
        Messenger.RemoveListener(GameEvent.CHALLENGE_INIT, OnChallengeInit);
        Messenger.RemoveListener(GameEvent.CHALLENGE_SETUP, OnChallengeSetup);
        Messenger.RemoveListener(GameEvent.CHALLENGE_STARTED, OnChallengeStarted);
        Messenger.RemoveListener(GameEvent.CHALLENGE_RELOAD, OnChallengeReload);
        Messenger.RemoveListener(GameEvent.WORD_ADDED, OnWordAdded);
    }
    
    void Start()
    {
        GameObject controller = GameObject.Find("SceneController");
        _controller = controller.GetComponent<SceneController>();
    }

    private void Update()
    {
        if (_timerIsRunning)
        {
            if (_timeRemaining > 0)
            {
                _timeRemaining -= Time.deltaTime;
                UpdateTimer(_timeRemaining);
                UpdateTimerProgress(_timeRemaining);
            }
            else
            {
                Debug.Log("Time has run out!");
                OnChallengeStopped();
            }
        }
    }

    public void ToggleSettingsPopup()
    {
        bool isShowing = settingsPopup.gameObject.activeSelf;

        Managers.Audio.PlaySound(defaultSound);
        settingsPopup.gameObject.SetActive(!isShowing);
    }
    
    public void ToggleAboutPopup()
    {
        bool isShowing = aboutPopup.gameObject.activeSelf;

        Managers.Audio.PlaySound(defaultSound);
        aboutPopup.gameObject.SetActive(!isShowing);
    }
    
    public void OnStartSettings()
    {
        Managers.Audio.PlaySound(defaultSound);
        startPopup.Open();
    }

    public void OnAddNewWord()
    {
    Managers.Audio.PlaySound(addWordSound);
        _controller.AddWord();
    }
    
    public void OnStopGame()
    {
        OnChallengeStopped();
    }

    public void OnWordUpdated()
    {
        List <Word> words = _controller.GetNonEmptyWords();
        string score = words.Count.ToString();
        wordScore.GetComponent<Text>().text = score;
    }

    public void OnChallengeInit()
    {
        startPopup.Hide();
        resultPopup.Hide();
        settingsPopup.Hide();
        aboutPopup.Hide();
        
        quitButton.SetActive(true);
        aboutButton.SetActive(true);
        startButton.SetActive(true);
        backButton.SetActive(false);
        playButton.SetActive(false);
        stopButton.SetActive(false);
        addWordButton.SetActive(false);
        okResultButton.SetActive(false);
        
        wordScore.SetActive(false);
        
        introText.SetActive(true);
        
        _timerIsRunning = false;
        _timeRemaining = 0;
        timer.SetActive(false);
        
        Managers.Audio.PlayIntroMusic();

        Managers.Challenge.UpdateChallengeStatus(ChallengeStatus.Inited);
    }

    public void OnChallengeSetup()
    {
        quitButton.SetActive(false);
        aboutButton.SetActive(false);
        startButton.SetActive(false);
        backButton.SetActive(true);
        playButton.SetActive(true);
        stopButton.SetActive(false);
        addWordButton.SetActive(false);
        okResultButton.SetActive(false);
        introText.SetActive(true);
    }

    public void OnChallengeStarted()
    {
        startPopup.Hide();
        settingsPopup.Hide();
        aboutPopup.Hide();
        resultPopup.Hide();
        
        quitButton.SetActive(false);
        aboutButton.SetActive(false);
        startButton.SetActive(false);
        backButton.SetActive(false);
        playButton.SetActive(false);
        stopButton.SetActive(true);
        addWordButton.SetActive(true);
        okResultButton.SetActive(false);
        
        wordScore.SetActive(true);
        
        introText.SetActive(false);

        _timeRemaining = Managers.Challenge.GetTimerValue();
        if (_timeRemaining > 0)
        {
            _timerIsRunning = true;
            timer.SetActive(true);
        }
        else
        {
            timer.SetActive(false);
        }
        
        Managers.Audio.PlayLevelMusic();

        Managers.Challenge.UpdateChallengeStatus(ChallengeStatus.Running);
    }

    public void OnChallengeStopped()
    {
        Managers.Audio.PlaySound(stopGameSound);
        
        _timeRemaining = 0;
        _timerIsRunning = false;
        timer.SetActive(false);
        
        quitButton.SetActive(false);
        aboutButton.SetActive(false);
        startButton.SetActive(false);
        backButton.SetActive(false);
        playButton.SetActive(false);
        stopButton.SetActive(false);
        addWordButton.SetActive(false);
        okResultButton.SetActive(true);
        
        wordScore.SetActive(false);
        
        introText.SetActive(false);
        
        Managers.Audio.StopMusic();
        Managers.Audio.PlayFinalMusic();
        
        Managers.Challenge.UpdateChallengeStatus(ChallengeStatus.Completed);
        
        resultPopup.Open();
    }

    public void OnChallengeReload()
    {
        Managers.Challenge.Reload();
    }
    
    void UpdateTimer(float timeToDisplay)
    {
        timeToDisplay += 1;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timer.GetComponent<Text>().text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void UpdateTimerProgress(float timeRemaining)
    {
        int timerTotal = Managers.Challenge.GetTimerValue();
        float progress = timeRemaining / timerTotal;

        ParticleSystem ps = timer.GetComponent<ParticleSystem>();
        var psMain = ps.main;
        psMain.startLifetime = progress;
    }

    public void OnWordAdded()
    {
        List<Word> words = _controller.GetWords();
        if (words.Count <= 1)
        {
            return;
        }

        Word lastAdded = _controller.GetWord(words.Count - 1);
        
        SnapTo(lastAdded);
    }
    
    public void SnapTo(Word w)
    {
        RectTransform wordRectTransform = w.GetComponent<RectTransform>();
        
        GameObject scroll = GameObject.Find("Scroll");
        ScrollRect scrollRect = scroll.GetComponent<ScrollRect>();
        GameObject contentPanel = GameObject.Find("Panel");
        
        RectTransform scrollRectTransform = scroll.GetComponent<RectTransform>();

        float size = Managers.Challenge.GetLetterSize();
        if ((wordRectTransform.localPosition.y + scrollRectTransform.sizeDelta.y - size) > 0)
        {
            return;
        }

        Canvas.ForceUpdateCanvases();

        contentPanel.GetComponent<VerticalLayoutGroup>().CalculateLayoutInputVertical();
        contentPanel.GetComponent<ContentSizeFitter>().SetLayoutVertical();

        scrollRect.content.GetComponent<VerticalLayoutGroup>().CalculateLayoutInputVertical();
        scrollRect.content.GetComponent<ContentSizeFitter>().SetLayoutVertical();

        scrollRect.verticalNormalizedPosition = 0;
    }

    public void OnQuitGame()
    {
        Application.Quit();
    }
}
