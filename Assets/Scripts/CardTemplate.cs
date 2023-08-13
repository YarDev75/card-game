using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new card", menuName = "card")]
public class CardTemplate : ScriptableObject
{
    public enum elements 
    {
        light,
        dark,
    }

    public enum directions 
    {
        front,
        threeway,
        fullBoard,
        mantis
    }


    public int damage;
    public elements element;
    public int cost;
    public directions direction;

}
