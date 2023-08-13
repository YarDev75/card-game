using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new card", menuName = "card")]
public class Card : ScriptableObject
{
    public enum elements 
    {
        light,
        dark,
    }

    public enum directions 
    {
        front,
        fork,
        right,
        left
    }


    public string Name;
    public Sprite Pic;
    public int damage;
    public bool Primary;
    public elements element;
    public int cost;
    public directions direction;
}
