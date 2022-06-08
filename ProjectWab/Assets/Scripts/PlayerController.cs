using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    // Movement
    public float baseSpeed = 5f;
    public Transform movePoint;
    public LayerMask isWalkable;
    public LayerMask Collisions;

    // Map
    [SerializeField]
    private Tilemap map;
    [SerializeField]
    private Tilemap UI;
    public TileBase SelectedTile;
    public TileBase tilledSoil;
    public TileBase grass;
    private Vector3Int prevTilePos;
    private MapManager mapManager;

    // Equipment
    public int EquipmentID;
    public GameObject EquipmentSlots;

    public GameObject _object;


    // Start is called before the first frame update
    void Start()
    {
        movePoint.parent = null;
        mapManager = FindObjectOfType<MapManager>();
        EquipmentSlots.transform.Find("Item Slot 1").transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
    }

    // Update is called once per frame
    void Update()
    {
        float moveSpeed = baseSpeed * mapManager.GetTileWalkingSpeed(transform.position);

        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);


        // Movement
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            if (Vector3.Distance(transform.position, movePoint.position) <= 0.5f)
            {

                if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f)
                {
                    if (Physics2D.OverlapCircle(movePoint.position + new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f), .2f, isWalkable) && !Physics2D.OverlapCircle(movePoint.position + new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f), .2f, Collisions))
                    {
                        movePoint.position += new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);

                        if (Input.GetAxisRaw("Horizontal") == 1f) { GetComponent<SpriteRenderer>().flipX = true; }
                        if (Input.GetAxisRaw("Horizontal") == -1f) { GetComponent<SpriteRenderer>().flipX = false; }
                    }
                }

                if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f)
                {
                    if (Physics2D.OverlapCircle(movePoint.position + new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f), .2f, isWalkable) && !Physics2D.OverlapCircle(movePoint.position + new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f), .2f, Collisions))
                    {
                        movePoint.position += new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f);
                    }
                }
            }
        }


        // Display UI

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int gridPosition = map.WorldToCell(mousePosition);

        if (map.GetTile(gridPosition) != SelectedTile && map.GetTile(gridPosition) != null)
        {
            UI.SetTile(prevTilePos, null);
            UI.SetTile(gridPosition, SelectedTile);
            prevTilePos = gridPosition;
        }


        // Controls

        // Selecting Equipment
        if (Input.GetKeyDown("1"))
        {
            EquipmentID = 1;
            foreach (Transform child in EquipmentSlots.transform)
            {
                child.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
            EquipmentSlots.transform.GetChild(EquipmentID - 1).transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        }
        if (Input.GetKeyDown("2"))
        {
            EquipmentID = 2;
            foreach (Transform child in EquipmentSlots.transform)
            {
                child.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
            EquipmentSlots.transform.GetChild(EquipmentID - 1).transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        }
        if (Input.GetKeyDown("3"))
        {
            EquipmentID = 3;
            foreach (Transform child in EquipmentSlots.transform)
            {
                child.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
            EquipmentSlots.transform.GetChild(EquipmentID - 1).transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        }



        if (Input.GetMouseButtonDown(0))
        {
            TileBase clickedTile = map.GetTile(gridPosition);
            switch (EquipmentID)
            {
                // Hoe
                case 1:
                    if (clickedTile == grass)
                    {
                        map.SetTile(gridPosition, tilledSoil);
                    }
                    break;

                // Seeds
                case 2:
                    mapManager.PlantSeeds(mousePosition);
                    break;

                // Fertiliser Spell
                case 3:
                    break;
            }



        }
    }
}
