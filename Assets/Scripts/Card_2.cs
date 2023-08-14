using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Card_2 : ScriptableObject //This is the approach with arrays don't have time to figure out the file one
{
    //Same fields as OG Card
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

    public int ColID;
    public string Name;
    public Sprite Pic;
    public int damage;
    public bool Primary;
    public elements element;
    public int cost;
    public directions direction;

    static private string[] names = { "Perjury", "Devour", "Penance", "Abyss Look" };
    //static private Sprite[] pics <- Okay, we have a problem with this one, not gonna lie, maybe use Resources.Load() I don't know
    static private int[] damages = { 4, 6, 0, 3 };
    static private bool[] primaryList = { true, true, false, true }; //We can just add the primary cards first in the collection and only use true/false
    static private int[] els = { 0, 1, 0, 1 };
    static private int[] costs = { 2, 0, 0, 4 };
    static private int[] dirs = { 0, 1, 2, 3 };
    
    public Card_2(int colID)
    {
        ColID = colID;
        Name = names[colID];
        // Pic = pics[colID];
        damage = damages[colID];
        Primary = primaryList[colID];
        element = (elements)els[colID];         //This way the one who will retain the info will be the object instance by using Card_2
        cost = costs[colID];                    //This also let us convert colection and deck into a int[] so the cards are created in runtime
        direction = (directions)dirs[colID];    //It would also simplify the enemies as they would only need to carry an int[] with cards IDs
    }
}
