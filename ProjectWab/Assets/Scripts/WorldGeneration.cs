using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGeneration : MonoBehaviour
{
    public int rows = 16;
    public int columns = 16;
    static public int tileSize = 16;

    public GameObject TilePrefab;
    public GameObject Camera;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] Tiles = new GameObject[rows * columns];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                int id = j + i * columns;
                Tiles[id] = Instantiate(TilePrefab, new Vector2(j, -i), Quaternion.identity);

                // Decide Tile type here
                Tiles[id].GetComponent<TileManager>().thisTileType = TileManager.tileType.Grass;
                Tiles[id].GetComponentInChildren<UnityEngine.UI.Text>().text = id.ToString();

                //tile.GetComponent<SpriteRenderer>().size = new Vector2(tileSize, tileSize);
            }
        }

        Camera.transform.position = new Vector3(columns / 2, -rows / 2, -10);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
