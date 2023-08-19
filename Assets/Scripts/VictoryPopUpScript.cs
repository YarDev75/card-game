using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class VictoryPopUpScript : MonoBehaviour
{
    //takes 4 cards from enemy reward pool, lets you choose 2
    [SerializeField] private bool Chests;
    [SerializeField] private Animator Transition;
    [SerializeField] private RunSaveState RunSS;
    [SerializeField] private Transform[] CardSpots;
    [SerializeField] private GameObject CardPrefab;
    [SerializeField] private TextMeshProUGUI ChooseText;
    int CardsLeft = 2;
    List<int> Inds;

    private void Start()
    {
        var AvailableInds = new List<int>();
        Inds = new List<int>();
        for (int i = 0; i < EnenemyAI.person.RewardPool.Length; i++)
        {
            AvailableInds.Add(i);
        }
        for (int i = 0; i < 4; i++)
        {
            Inds.Add(AvailableInds[Random.Range(0, AvailableInds.Count)]);
            var card = Instantiate(CardPrefab, CardSpots[i]).GetComponent<CardObjectScript>();
            card.sr.sortingOrder = 204;
            card.canvas.overrideSorting = true;
            card.canvas.sortingOrder = 205;
            card.transform.localPosition = Vector3.zero;
            card.transform.localScale = new Vector3(150, 150, 0);
            card.decorative = true;
            card.content = EnenemyAI.person.RewardPool[Inds[i]];
        }
    }

    public void ChooseCard(int ind)
    {
        CardsLeft--;
        for (int i = 0; i < RunSS.Collection.Length; i++)
        {
            if(RunSS.Collection[i] == null)
            {
                RunSS.Collection[i] = EnenemyAI.person.RewardPool[Inds[ind]];
                break;
            }
        }
        if (CardsLeft <= 0)
        {
            for (int i = 0; i < CardSpots.Length; i++)
            {
                CardSpots[i].gameObject.SetActive(false);
            }
        }
        else
        {
            CardSpots[ind].gameObject.SetActive(false);
        }
        ChooseText.text = $"choose {CardsLeft}:";
    }

    public void QuitToMap()
    {
        if (Chests)
        {
            gameObject.SetActive(false);
            CardsLeft = 2; 
            for (int i = 0; i < CardSpots.Length; i++)
            {
                CardSpots[i].gameObject.SetActive(true);
            }
        }
        else
        {
            Transition.SetTrigger("go");
            Invoke("LoadMap", 1.5f);
        }
    }

    void LoadMap()
    {
        SceneManager.LoadScene(3 + RunSS.roomNo);
    }
}
