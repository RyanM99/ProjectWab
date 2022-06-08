using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;




public class MapManager : MonoBehaviour
{
    // Tile Maps
    [SerializeField]
    private Tilemap map;
    [SerializeField]
    private Tilemap collisions;
    [SerializeField]
    private Tilemap Details;

    // Tile Behaviour (For all tiles of a type)
    [SerializeField]
    private List<TileBehaviour> tileBehaviours;
    private Dictionary<TileBase, TileBehaviour> behaviourFromTiles;


    // Tile Data (Individual Tiles)


    // Crops
    public TileBase Seeded;
    public CropManager CropPrefab;
    public float GrowthInterval = 1f;
    private Dictionary<Vector3Int, CropManager> Crops = new Dictionary<Vector3Int, CropManager>();

    private void Awake()
    {
        behaviourFromTiles = new Dictionary<TileBase, TileBehaviour>();

        foreach (var tileBehaviour in tileBehaviours)
        {
            foreach (var tile in tileBehaviour.tiles)
            {
                behaviourFromTiles.Add(tile, tileBehaviour);
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GrowCrops());
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridPosition = map.WorldToCell(mousePosition);

            TileBase clickedTile = map.GetTile(gridPosition);

            float walkingSpeed = behaviourFromTiles[clickedTile].walkingSpeed;
        }
    }

    public float GetTileWalkingSpeed(Vector2 worldPosition)
    {
        Vector3Int gridPosition = map.WorldToCell(worldPosition);

        TileBase tile = map.GetTile(gridPosition);

        if (tile == null)
            return 1f;

        float walkingSpeed = behaviourFromTiles[tile].walkingSpeed;

        return walkingSpeed;
    }

    public void PlantSeeds(Vector2 worldPosition)
    {
        Vector3Int gridPosition = map.WorldToCell(worldPosition);

        TileBase tile = map.GetTile(gridPosition);

        if (tile == null || !behaviourFromTiles[tile].canBeSeeded)
            return;

        if (!Crops.ContainsKey(gridPosition))
        {
            Details.SetTile(gridPosition, Seeded);
            CropManager newCrop = Instantiate(CropPrefab);
            newCrop.tilePos = gridPosition;
            Crops.Add(gridPosition, newCrop);

            print("new crop added");
        }
    }

    public void UpdateCropTiles(Vector3Int tilePos, TileBase tile)
    {
        Details.SetTile(tilePos, tile);
    }

    private IEnumerator GrowCrops()
    {
        while (true)
        {
            foreach (var crop in Crops)
            {
                crop.Value.Grow(1f);
                
            }

            yield return new WaitForSeconds(GrowthInterval);
        }    
    }
}
