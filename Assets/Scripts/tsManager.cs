using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class tsManager : MonoBehaviour
{
    [SerializeField] private Animator transition;
    [SerializeField] private RunSaveState saveState;
    [SerializeField] private RoomSaveState mapGenerator;
    [SerializeField] private TextMeshProUGUI KingDialogue;
    [SerializeField] private GameObject character;
    [SerializeField] private GameObject continueButton;
    [SerializeField] private Animator[] anims;          //hand, mouth, cards, buttons
    [SerializeField] private string[] KingTalk;
    [SerializeField] private Card[] StartingDeck;

    public Sprite[] ListOfCharacters;
    private int currentChar;
    float timer;
    int ind = -2;

    void Start()
    {
        if(saveState.roomNo < 0) continueButton.SetActive(false);
        else continueButton.SetActive(true);

        //currentChar = 0;
        //character.GetComponent<SpriteRenderer>().sprite = ListOfCharacters[currentChar];
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
    }

    public void Continue()
    {
        transition.SetTrigger("go");
        Invoke("Play", 1.5f);
    }

    public void NewGame()
    {
        ind = -1;
        //saveState.character = character.GetComponent<SpriteRenderer>().sprite;
        saveState.Collection = new Card[30];
        saveState.Deck = new Card[8];
        for (int i = 0; i < StartingDeck.Length; i++)
        {
            saveState.Deck[i] = StartingDeck[i];
        }
        saveState.roomNo = 0;
        mapGenerator.firstTime = true;
        anims[3].SetTrigger("Hide");
    }

    //public void CharSwap()
    //{
    //    currentChar = (currentChar+1) % ListOfCharacters.Length;
    //    character.GetComponent<SpriteRenderer>().sprite = ListOfCharacters[currentChar];
    //}

    void Play()
    {
        SceneManager.LoadScene(3);
    }
}
