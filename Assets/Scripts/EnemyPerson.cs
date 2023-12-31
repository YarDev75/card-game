using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new enemy", menuName = "enemy person")]
public class EnemyPerson : ScriptableObject
{
    public Card[] Deck;
    public Card[] RewardPool;
    public Sprite[] Art;
    public AudioClip Intro;
    public AudioClip Music;
    public bool IsBoss;
    public bool IsEmpress;
    public string[] ThinkingDialogue;
    public string[] TakenDamageDialogue;
    public string[] EndTurnDialogue;
    public string[] DeckEmptyDialogue;
    public string[] OutOfManaDialogue;
}
