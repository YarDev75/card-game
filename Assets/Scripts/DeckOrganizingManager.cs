using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeckOrganizingManager : MonoBehaviour
{
    [SerializeField] private Animator Transition;
    [SerializeField] private RunSaveState RunSave;
    [SerializeField] private RoomSaveState MaybeThisWillHelp;
    [SerializeField] private Transform[] CollectionFrames;
    [SerializeField] private Transform[] DeckFrames;
    [SerializeField] private GameObject CardPrefab;
    [SerializeField] private float SnapDistance;
    private DeckScreenCardObject[] CollectionCards;
    private DeckScreenCardObject[] DeckCards;

    private void Awake()
    {
        CollectionCards = new DeckScreenCardObject[CollectionFrames.Length];
        DeckCards = new DeckScreenCardObject[DeckFrames.Length];

        for (int i = 0; i < RunSave.Collection.Length; i++)
        {
            if (RunSave.Collection[i] != null)
            {
                var card = Instantiate(CardPrefab, CollectionFrames[i].position, Quaternion.identity).GetComponent<DeckScreenCardObject>();
                card.content = RunSave.Collection[i];
                card.TargetPos = CollectionFrames[i].position;
                CollectionCards[i] = card;
            }
        }
        for (int i = 0; i < RunSave.Deck.Length; i++)
        {
            if (RunSave.Deck[i] != null)
            {
                var card = Instantiate(CardPrefab, DeckFrames[i].position, Quaternion.identity).GetComponent<DeckScreenCardObject>();
                card.content = RunSave.Deck[i];
                card.TargetPos = DeckFrames[i].position;
                DeckCards[i] = card;
            }
        }
    }

    public void Confirm()
    {
        var collectionCards = new List<Card>();
        for (int i = 0; i < CollectionCards.Length; i++)
        {
            if (CollectionCards[i] != null) collectionCards.Add(CollectionCards[i].content);
        }
        for (int i = collectionCards.Count; i < CollectionCards.Length; i++)
        {
            collectionCards.Add(null);
        }
        for (int i = 0; i < collectionCards.Count; i++)
        {
            RunSave.Collection[i] = collectionCards[i];
        }
        var deckCards = new List<Card>();
        for (int i = 0; i < DeckCards.Length; i++)
        {
            if (DeckCards[i] != null) deckCards.Add(DeckCards[i].content);
        }
        for (int i = deckCards.Count; i < DeckCards.Length; i++)
        {
            deckCards.Add(null);
        }
        for (int i = 0; i < deckCards.Count; i++)
        {
            RunSave.Deck[i] = deckCards[i];
        }
        Back();
    }

    public void Back()
    {
        Transition.SetTrigger("go");
        Invoke("LoadPrev", 1.5f);
    }

    void LoadPrev()
    {
        SceneManager.LoadScene(3 + RunSave.roomNo);
    }

    //I know it's bad, but we don't have much time left :P
    public void Place(DeckScreenCardObject card)
    {
        float Min = SnapDistance;
        int ind = -1;
        int CardInd = -1;
        bool LastInDeck = false;
        //band aid code, looking for the card in deck to remove later
        for (int i = 0; i < DeckCards.Length; i++)
        {
            if (DeckCards[i] == card)
            {
                LastInDeck = true;
                CardInd = i;
            }
        }

        for (int i = 0; i < CollectionFrames.Length; i++)
        {
            if (CollectionCards[i] == card) CardInd = i;
            if (Vector2.Distance(card.transform.position, CollectionFrames[i].position) < Min && CollectionCards[i] == null)
            {
                Min = Vector2.Distance(card.transform.position, CollectionFrames[i].position);
                ind = i;
            }
        }
        if (ind >= 0)
        {
            CollectionCards[ind] = card;
            card.TargetPos = CollectionFrames[ind].position;
            if (LastInDeck) DeckCards[CardInd] = null;
            else CollectionCards[CardInd] = null;
            return;
        }

        for (int i = 0; i < DeckFrames.Length; i++)
        {
            if (DeckCards[i] == card) CardInd = i;
            if (Vector2.Distance(card.transform.position, DeckFrames[i].position) < Min && DeckCards[i] == null)
            {
                Min = Vector2.Distance(card.transform.position, DeckFrames[i].position);
                ind = i;
            }
        }
        if (ind >= 0)
        {
            DeckCards[ind] = card;
            card.TargetPos = DeckFrames[ind].position;
            if (LastInDeck) DeckCards[CardInd] = null;
            else CollectionCards[CardInd] = null;
            return;
        }
    }
}
