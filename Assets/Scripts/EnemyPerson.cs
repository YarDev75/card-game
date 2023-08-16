using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new enemy", menuName = "enemy person")]
public class EnemyPerson : ScriptableObject
{
    public Card[] Deck;
    public Sprite[] Art;
    public string[] ThinkingDialogue;
    public string[] TakenDamageDialogue;
    public string[] EndTurnDialogue;
    public string[] DeckEmptyDialogue;
    public string[] OutOfManaDialogue;
}
