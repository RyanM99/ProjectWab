using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGeneration : MonoBehaviour
{
    static int rows = 16;
    static int columns = 24;
    static int tileSize = 16;

    public Sprite GrassSprite;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = -rows / 2; i < rows / 2; i++)
        {
            for (int j = -columns / 2; j < columns / 2; j++)
            {
                GameObject tile = new GameObject();
                tile.AddComponent<SpriteRenderer>().sprite = GrassSprite;
                tile.GetComponent<SpriteRenderer>().size = new Vector2(tileSize, tileSize);
                tile.transform.position = new Vector2(j, i);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
