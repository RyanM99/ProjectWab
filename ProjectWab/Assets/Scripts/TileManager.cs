using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public bool isWalkable = true;
    public Sprite GrassSprite;

    public enum tileType
    {
        None,
        Grass,
        Water,
        Wall
    }

    public tileType thisTileType = tileType.None;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().sortingOrder = -1;

        switch (thisTileType)
        {
            case tileType.None:
                break;
            case tileType.Grass:
                GetComponent<SpriteRenderer>().sprite = GrassSprite;
                break;
            case tileType.Water:
                break;
            case tileType.Wall:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
