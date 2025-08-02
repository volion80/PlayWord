using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ResultPopup : MonoBehaviour
{
    private const float LongWordDef = 0.75f;
    private const int AllLettersBonusCoef = 10;
    private const int LongWordsBonus = 10;
    
    
    [SerializeField] private GameObject resultsWordPrefab;
    
    [SerializeField] private GameObject wordsCount;
    [SerializeField] private GameObject allLettersCount;
    [SerializeField] private GameObject longWordsCount;
    [SerializeField] private GameObject totalScore;
    
    [SerializeField] private GameObject wordsCountValue;
    [SerializeField] private GameObject longWordsCountValue;
    [SerializeField] private GameObject allLettersCountValue;
    [SerializeField] private GameObject totalScoreValue;

    [SerializeField] private AudioClip OkResultSound;
    [SerializeField] private AudioClip allLettersSound;
    [SerializeField] private AudioClip longWordsSound;
    [SerializeField] private AudioClip totalScoreSound;
    
    [SerializeField] private AudioClip valueSound;
    [SerializeField] private AudioClip totalScoreValueSound;

    private SceneController _controller;
    private GameObject _resultsContent;

    private float _resultWordHeight = 20f;

    private void Awake()
    {
        GameObject controller = GameObject.Find("SceneController");
        _controller = controller.GetComponent<SceneController>();
        _resultsContent = GameObject.Find("ResultsContent");
    }

    void Start()
    {
        allLettersCount.SetActive(false);
        longWordsCount.SetActive(false);
        totalScore.SetActive(false);
        
        wordsCountValue.SetActive(false);
        longWordsCountValue.SetActive(false);
        allLettersCountValue.SetActive(false);
        totalScoreValue.SetActive(false);
    }
    
    public void Open()
    {
        gameObject.SetActive(true);
        LoadResults();
    }
    
    public void Close()
    {
        Managers.Audio.PlaySound(OkResultSound);
        Hide();
        Messenger.Broadcast(GameEvent.CHALLENGE_RELOAD);
    }
    
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void LoadResults()
    {
        string startWordText = Managers.Challenge.GetStartWordText();
        
        List<Word> words = _controller.GetNonEmptyWords();
        
        int allLetters = words.Count(o => o.GetWordText().Length == startWordText.Length);

        int longWordMinLetters = Convert.ToInt32(Math.Round(startWordText.Length * LongWordDef, 0, MidpointRounding.AwayFromZero));
        int longWords = words.Count(o => o.GetWordText().Length >= longWordMinLetters && o.GetWordText().Length < startWordText.Length);

        int scoreTotal = words.Count;
        scoreTotal += allLetters * (AllLettersBonusCoef * startWordText.Length);
        scoreTotal += longWords * LongWordsBonus;

        int n = Word.MinLetters;
        while (n <= startWordText.Length)
        {
            int cnt = words.Count(o => o.GetWordText().Length == n);

            scoreTotal += cnt * n;
            n++;
        }

        Dictionary<string, object> totals = new Dictionary<string, object>();
        totals.Add("wordsTotal", words.Count);
        totals.Add("allLetters", allLetters);
        totals.Add("longWords", longWords);
        totals.Add("totalScore", scoreTotal);
        
        LoadTotals(totals);
        LoadWords(words);
    }

    public void LoadWords(List<Word> wordList)
    {
        for (int i = 0; i < wordList.Count; i++)
        {
            string wordText = wordList[i].GetWordText();
            
            GameObject resultsWord = Instantiate(resultsWordPrefab);
            resultsWord.GetComponent<Text>().text = wordText;
            float posX = 0;
            float posY = i * _resultWordHeight;
            resultsWord.transform.position = new Vector3(posX, posY, 0);
            
            resultsWord.transform.SetParent(_resultsContent.transform, false);
        }
    }

    public void LoadTotals(Dictionary<string, object> totals)
    {
        Animation wordsCountAnim = wordsCount.GetComponent<Animation>();
        wordsCountAnim.Play("WordCountAnimation");
        
        StartCoroutine(AllLettersCoroutine());
        StartCoroutine(LongWordsCoroutine());

        StartCoroutine(ScoreCountersCoroutine(totals));
        
        StartCoroutine(TotalScoreCounterCoroutine(totals));
        
    }

    IEnumerator AllLettersCoroutine()
    {
        yield return new WaitForSeconds(1.0f);
        allLettersCount.SetActive(true);
        
        Managers.Audio.PlaySound(allLettersSound);
        
        Animation allLettersCountAnim = allLettersCount.GetComponent<Animation>();
        allLettersCountAnim.Play("AllLettersAnimation");
    }

    IEnumerator LongWordsCoroutine()
    {
        yield return new WaitForSeconds(2.5f);
        longWordsCount.SetActive(true);
        
        Managers.Audio.PlaySound(longWordsSound);
        
        Animation longWordsCountAnim = longWordsCount.GetComponent<Animation>();
        longWordsCountAnim.Play("LongWordsCountAnimation");
    }

    IEnumerator ScoreCountersCoroutine(Dictionary<string, object> totals)
    {
        yield return new WaitForSeconds(3.5f);
        
        wordsCountValue.SetActive(true);
        Managers.Audio.PlaySound(valueSound);
        for (int i = 0; i <= (int)totals["wordsTotal"]; i++)
        {
            wordsCountValue.GetComponent<Text>().text = i.ToString();
            yield return new WaitForSeconds(0.1f);
        }
        
        allLettersCountValue.SetActive(true);
        Managers.Audio.PlaySound(valueSound);
        for (int i = 0; i <= (int)totals["allLetters"]; i++)
        {
            allLettersCountValue.GetComponent<Text>().text = i.ToString();
            yield return new WaitForSeconds(0.1f);
        }

        longWordsCountValue.SetActive(true);
        Managers.Audio.PlaySound(valueSound);
        for (int i = 0; i <= (int)totals["longWords"]; i++)
        {
            longWordsCountValue.GetComponent<Text>().text = i.ToString();
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.5f);
        totalScore.SetActive(true);
        Managers.Audio.PlaySound(totalScoreSound);
        
        Animation totalScoreAnim = totalScore.GetComponent<Animation>();
        totalScoreAnim.Play("TotalScoreAnimation");
    }
    
    IEnumerator TotalScoreCoroutine()
    {
        yield return new WaitForSeconds(4.5f);
        totalScore.SetActive(true);
        
        Managers.Audio.PlaySound(totalScoreSound);
        
        Animation totalScoreAnim = totalScore.GetComponent<Animation>();
        totalScoreAnim.Play("TotalScoreAnimation");
    }

    IEnumerator TotalScoreCounterCoroutine(Dictionary<string, object> totals)
    {
        yield return new WaitForSeconds(6.0f);

        totalScoreValue.SetActive(true);
        for (int i = 0; i <= (int)totals["totalScore"]; i++)
        {
            totalScoreValue.GetComponent<Text>().text = i.ToString();
            yield return new WaitForSeconds(0.01f);
        }
        Managers.Audio.PlaySound(totalScoreValueSound);
    }
}
