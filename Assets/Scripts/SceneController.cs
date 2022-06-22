using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class SceneController : MonoBehaviour
{
    [SerializeField] private Word word;
    
    [SerializeField] private AudioClip startWordLetterSound;
    [SerializeField] private AudioClip selectWordSound;
    [SerializeField] private AudioClip resetLetterSound;

    private static Random rnd;

    void Start()
    {
        rnd = new Random();
        SetupStartWord();
    }

    private void SetupStartWord()
    {
        if (Managers.Challenge.IsSetStartWord())
        {
            GameObject scroll = GameObject.Find("Scroll");
            RectTransform scrollRectTransform = scroll.GetComponent<RectTransform>();
            
            Managers.Challenge.CalculateLetterSize(scrollRectTransform.sizeDelta.x);
            AddWord();
            
            Messenger.Broadcast(GameEvent.CHALLENGE_STARTED);
        }
        else
        {
            Messenger.Broadcast(GameEvent.CHALLENGE_INIT);
        }
    }
    
    public void AddWord()
    {
        int id = DefineNewWordId();
        
        Word newWord = Instantiate(word);
        newWord.SetWord(id);

        if (!newWord.IsStartWord())
        {
            Word activeWord = GetActiveWord();
            if (activeWord != null)
                activeWord.SetActive(false);
            newWord.SetActive(true);

            Word startWord = GetWord(0); 
            startWord.RefreshHighlightStart();
            
            Messenger.Broadcast(GameEvent.WORD_ADDED);
        }
        else
        {
            newWord.InitHighlightStart();
        }
    }

    private int DefineNewWordId()
    {
        return GetWords().Count;
    }

    public void LetterClicked(Letter letter)
    {
        Word startWord = GetWord(0);
        Word activeWord = GetActiveWord();
        Word clickedWord = GetWord(letter.WordId);
        
        if (clickedWord.IsStartWord())
        {
            if (activeWord != null && !activeWord.IsStartWord() && clickedWord.IsLetterAvailable(letter))
            {
                if (activeWord.PopulateLetter(letter))
                {
                    Managers.Audio.PlaySound(startWordLetterSound);
                    
                    Messenger.Broadcast(GameEvent.WORD_UPDATED);
                }
                
                clickedWord.RefreshHighlightStart();
            }
        }
        else
        {
            if (activeWord == null || activeWord.ID != letter.WordId)
            {
                Managers.Audio.PlaySound(selectWordSound);
                
                if (activeWord != null)
                    activeWord.SetActive(false);
                clickedWord.SetActive(true);
                startWord.RefreshHighlightStart();
            }
            else
            {
                if (!letter.IsEmpty() && (letter.IsLast() || clickedWord.IsEmptyNextLetter(letter)))
                {
                    Managers.Audio.PlaySound(resetLetterSound);
                    
                    letter.Reset();
                    Messenger.Broadcast(GameEvent.WORD_UPDATED);
                    startWord.RefreshHighlightStart();
                }
            }
        }
    }

    public Word GetActiveWord()
    {
        List<Word> words = GetWords();
        Word w = words.FirstOrDefault(o => o.IsActive);

        return w;
    }

    public List<Word> GetWords()
    {
        List<Word> words = FindObjectsOfType<Word>().OrderBy(o => o.ID).ToList();

        return words;
    }

    public List<Word> GetNonEmptyWords()
    {
        List<Word> words = FindObjectsOfType<Word>().Where(o => o.ID > 0 && o.GetWordText().Length >= Word.MinLetters).ToList();

        return words;
    }
    
    public Word GetWord(int wordId)
    {
        return GetWords().First(o => o.ID == wordId);
    }

    void BlinkStartWord()
    {

        StartCoroutine(BlinkStartWordCoroutine());
    }

    IEnumerator BlinkStartWordCoroutine()
    {
        try
        {
            Word w = GetWord(0);
            List<Letter> letters = w.GetLetters().Where(o => !o.IsStartUsed()).ToList();
            int r = rnd.Next(letters.Count);
            letters[r].Blink("start");
        }
        catch (Exception ex)
        {
            Debug.Log("Start Word not set yet!");
        }
        yield return new WaitForSeconds(1.0f);
    }
    
    private void Todo()
    {
        List<string> todo = new List<string>();
        
        todo.Add("Do not allow create multiple empty words?");
        todo.Add("Replace Stop Game button?");
        
        todo.Add("Update score calculation for All Letters depending on Start Word length applying a coef");
        todo.Add("Add About popup content");
    }
}
