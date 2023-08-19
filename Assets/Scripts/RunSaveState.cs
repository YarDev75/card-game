using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "runSS", menuName = "new run_saveState")]
public class RunSaveState : ScriptableObject  //Contains information relevant to the current run/play of the game
{
    public int roomNo = -1;
    public Sprite character;
    public Card[] Deck;
    public Card[] Collection;
    public bool firstRun;
    public bool HaveRecover;        //the second life thing
}
