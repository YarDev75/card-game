using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class intro : MonoBehaviour
{
    [SerializeField] private RunSaveState rss;
    [SerializeField] private GameObject player;
    [SerializeField] private TextMeshProUGUI kingsWords;

    public string[] phrases;
    private SFXPlayer sfxPlayer;

    private int convoPlayer;
    private float convoCooldown;

    void Start()
    {
        sfxPlayer = GetComponent<SFXPlayer>();
        convoPlayer = 0;
        player.GetComponent<SpriteRenderer>().sprite = rss.character;
        Invoke("nextPhrase", 2f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            nextPhrase();
        }
        convoCooldown -= Time.deltaTime;
    }

    private void nextPhrase()
    {
        if (convoCooldown <= 0)
        {
            if (convoPlayer == phrases.Length) SceneManager.LoadScene(1);

            StartCoroutine("textSFX",phrases[convoPlayer]);
            kingsWords.text = phrases[convoPlayer++];
            convoCooldown = 2f;
        }
    }
    private IEnumerator textSFX(string s)
    {
        int times = s.Length / 3;
        for(int i = 0; i < times; i++)
        {
            sfxPlayer.play(0);
            yield return new WaitForSeconds(0.15f);
        }
    }
}
