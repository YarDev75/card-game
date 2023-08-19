using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

public class tsManager : MonoBehaviour
{
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
        if(saveState.roomNo < 0) continueButton.SetActive(false);
        else continueButton.SetActive(true);

        if(saveState.firstRun) charSwapButton.SetActive(false);
        else charSwapButton.SetActive(true);

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

    public void CharSwap()
    {
        currentChar = (currentChar+1) % ListOfCharacters.Length;
        character.GetComponent<SpriteRenderer>().sprite = ListOfCharacters[currentChar];
    }

    private Card[] deleteRepeated(Card[] cards)
    {
        HashSet<Card> result2 = new HashSet<Card>(cards);
        Card[] result = new Card[result2.Count];
        result2.CopyTo(result);
        return result;
    }
}
