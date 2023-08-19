using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class tsManager : MonoBehaviour
{
    [SerializeField] private RunSaveState saveState;
    [SerializeField] private RoomSaveState mapGenerator;
    [SerializeField] private GameObject character;
    [SerializeField] private GameObject charSwapButton;
    [SerializeField] private GameObject continueButton;

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
        SceneManager.LoadScene(3);
    }

    public void NewGame()
    {
        saveState.character = character.GetComponent<SpriteRenderer>().sprite;
        saveState.Collection = null;
        saveState.Deck = null;
        saveState.roomNo = 0;
        mapGenerator.firstTime = true;
        SceneManager.LoadScene(3);
    }

    public void CharSwap()
    {
        currentChar = (currentChar+1) % ListOfCharacters.Length;
        character.GetComponent<SpriteRenderer>().sprite = ListOfCharacters[currentChar];
    }
}
