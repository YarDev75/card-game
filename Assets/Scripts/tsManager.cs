using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using TMPro;

public class tsManager : MonoBehaviour
{
    [SerializeField] private Animator[] anims;     //hand mouth cards buttons
    [SerializeField] private TextMeshProUGUI KingDialogue;
    [SerializeField] private string[] KingTalk;
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider Sfx;
    [SerializeField] private Slider Music;
    [SerializeField] private Slider Diff;
    [SerializeField] private Animator transition;
    [SerializeField] private RunSaveState saveState;
    [SerializeField] private RoomSaveState mapGenerator;
    //[SerializeField] private GameObject character;
    //[SerializeField] private GameObject charSwapButton;
    [SerializeField] private GameObject continueButton;
    [SerializeField] private Card[] initDeck;

    public Sprite[] ListOfCharacters;
    //private int currentChar;
    int ind = -2;
    float timer;
    private SFXPlayer sfxPlayer;

    void Start()
    {
        sfxPlayer = GetComponent<SFXPlayer>();
        if (saveState.roomNo < 0) continueButton.SetActive(false);
        else continueButton.SetActive(true);

        //if (saveState.firstRun) charSwapButton.SetActive(false);
        //else charSwapButton.SetActive(true);

    }
    public void SetSfx(float value)
    {
        var newValue = value;
        mixer.SetFloat("sfx", newValue);
        Sfx.value = newValue;
    }
    public void SetMusic(float value)
    {
        var newValue = value;
        mixer.SetFloat("music", newValue);
        Music.value = newValue;
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
                    StartCoroutine("textSFX", KingTalk[ind]);
                    timer = 4;
                }
            }
            timer -= Time.deltaTime;
        }
        //currentChar = 0;
        //character.GetComponent<SpriteRenderer>().sprite = ListOfCharacters[currentChar];
    }

    public void Continue()
    {
        transition.SetTrigger("go");
        Invoke("LoadMap", 1.5f);
    }

    public void NewGame()
    {
        //saveState.character = character.GetComponent<SpriteRenderer>().sprite;
        saveState.Collection = new Card[30];
        saveState.Deck = new Card[8];
        for (int i = 0; i < initDeck.Length; i++)
        {
            saveState.Deck[i] = initDeck[i];
        }
        saveState.difficulty = (int)Diff.value;
        saveState.Collection = deleteRepeated(saveState.Deck);
        saveState.roomNo = 0;
        mapGenerator.firstTime = true;
        ind = -1;
        anims[3].SetTrigger("Hide");
    }

    void LoadMap()
    {
        SceneManager.LoadScene(3);
    }

    private IEnumerator textSFX(string s)
    {
        int times = s.Length / 3;
        for (int i = 0; i < times; i++)
        {
            sfxPlayer.play(0);
            yield return new WaitForSeconds(0.15f);
        }
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
