using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject NPCPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space)) 
        {
            GameObject NPC = Instantiate(NPCPrefab, new Vector2(0, 0), Quaternion.identity, GameObject.Find("NPCs").transform);
        }
    }
}
