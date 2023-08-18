using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "runSS", menuName = "new run_saveState")]
public class RunSaveState : ScriptableObject  //Contains information relevant to the current run/play of the game
{
    public int roomNo = 0;
    //public Sprite character; //If we want to add buffs to certain characters we need to change this //I don't think we'll implement characters
    public Card[] Deck;
    public Card[] Collection;
}
