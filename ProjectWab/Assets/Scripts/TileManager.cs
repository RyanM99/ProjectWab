using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public bool isWalkable = true;
    public Sprite Sprite;
    public int X;
    public int Y;
    public int id;


    // Start is called before the first frame update
    void Awake()
    {
        GetComponent<SpriteRenderer>().sortingOrder = -1; 
        GetComponent<SpriteRenderer>().sprite = Sprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
