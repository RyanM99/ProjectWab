using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class TileBehaviour : ScriptableObject
{
    public TileBase[] tiles;

    public bool isWalkable = true;
    public float walkingSpeed = 1f;

    public bool canBeTilled = false;
    public bool canBeSeeded = false;
    public bool canBeFertilised = false;


}
