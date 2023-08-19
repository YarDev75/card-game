using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class tsManager : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider Sfx;
    [SerializeField] private Slider Music;
    [SerializeField] private Animator transition;
    [SerializeField] private RunSaveState saveState;
    [SerializeField] private RoomSaveState mapGenerator;
    [SerializeField] private GameObject character;
    [SerializeField] private GameObject charSwapButton;
    [SerializeField] private GameObject continueButton;
    [SerializeField] private initialDeck initDeck;

    public Sprite[] ListOfCharacters;
    private int currentChar;

    void Start()
    {
        if (saveState.roomNo < 0) continueButton.SetActive(false);
        else continueButton.SetActive(true);

        //if (saveState.firstRun) charSwapButton.SetActive(false);
        //else charSwapButton.SetActive(true);

    }
    public void SetSfx(float value)
    {
        var newValue = Mathf.Log10(value) * 20;
        mixer.SetFloat("sfx", newValue);
        Sfx.value = newValue;
    }

    private void Update()
    {
        if (ind > -2)
        {
            if (timer <= 0)
            {
                ind++;
                if (ind >= KingTalk.Length)
                {
                    Continue();
                    ind = -2;
                }
                else
                {
                    if (ind == 5)
                    {
                        anims[0].SetTrigger("giveCards");
                        anims[2].SetTrigger("giveCards");
                    }
                    anims[1].SetTrigger("talking");
                    KingDialogue.text = KingTalk[ind];
                    timer = 4;
                }
            }
            timer -= Time.deltaTime;
        }
        currentChar = 0;
        character.GetComponent<SpriteRenderer>().sprite = ListOfCharacters[currentChar];
    }

    public void Continue()
    {
        SceneManager.LoadScene(1);
    }

    public void NewGame()
    {
        saveState.character = character.GetComponent<SpriteRenderer>().sprite;
        saveState.Collection = deleteRepeated(initDeck.cards);
        saveState.Deck = initDeck.cards;
        saveState.roomNo = 0;
        mapGenerator.firstTime = true;
        SceneManager.LoadScene("Intro");
    }

    //public void CharSwap()
    //{
    //    currentChar = (currentChar+1) % ListOfCharacters.Length;
    //    character.GetComponent<SpriteRenderer>().sprite = ListOfCharacters[currentChar];
    //}

    private Card[] deleteRepeated(Card[] cards)
    {
        HashSet<Card> result2 = new HashSet<Card>(cards);
        Card[] result = new Card[result2.Count];
        result2.CopyTo(result);
        return result;
    }
}
