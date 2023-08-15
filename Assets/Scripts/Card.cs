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
        left,
        none
    }

    public enum PassiveEffects //not implemented
    {
        Leeching,
    }

    public enum SpecialEffects
    {
        RecoverLux,
        RecoverUmbra,
        DamageOwner,
        HealOwner,
        BoostPrimary,
    }

    public enum DeathEffects //not implemented
    {
        none,
        DrainLux,
        DrainUmbra,
    }



    public string Name;
    public Sprite Pic;
    public int damage;
    public bool Primary;
    public elements element;
    public int cost;
    public directions direction;
    public SpecialEffects[] effects;
    public DeathEffects deathEffect;
    public PassiveEffects[] passives;
    public string description;
}
