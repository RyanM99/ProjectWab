using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CropManager : MonoBehaviour
{
    public float GrowthTime = 10f;
    public Vector3Int tilePos;

    private float CurrentGrowth = 0f;
    private MapManager mapManager;

    public int currentStage = 0;
    public TileBase stage0;
    public TileBase stage1;
    public TileBase stage2;
    public TileBase stage3;

    public float growthFactor = 1.0f;
    private float fertiliserFactor = 1.0f;

    public bool hasBeenPlanted = false;

    // Start is called before the first frame update
    void Start()
    {
        mapManager = FindObjectOfType<MapManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Grow(float GrowthAmount)
    {
        if (hasBeenPlanted)
        {
            CurrentGrowth += GrowthAmount * growthFactor * fertiliserFactor;

            print(this + " has grown " + CurrentGrowth);

            // Crop has reached maturity
            if (CurrentGrowth >= GrowthTime / 2f && currentStage == 1)
            {
                mapManager.UpdateCropTiles(tilePos, stage2);
                currentStage++;
            }

            // Crop has reached fruition
            if (CurrentGrowth >= GrowthTime && currentStage == 2)
            {
                mapManager.UpdateCropTiles(tilePos, stage3);
                currentStage++;
            }
        }
    }

    public void PlantSeeds()
    {
        hasBeenPlanted = true;
        mapManager.UpdateCropTiles(tilePos, stage1);
        currentStage = 1;
    }

    public void fertilise(float fertFactor)
    {
        fertiliserFactor = fertFactor;
    }


}
