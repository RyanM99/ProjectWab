using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class empty : MonoBehaviour
{
    // Variables
    public GameObject ObjectToRemove;
    public GameObject panel1;
    public GameObject panel2;
    public float a = 1f, b = 2f, c = 3f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("e") && isActive())
        {
            ObjectToRemove.SetActive(false);

        }

        if (Input.GetKeyDown("f") && !isActive())
        {
            ObjectToRemove.SetActive(true);
        }

    }

    public void panel1SetActive(bool shouldBeActive)
    {
        panel1.SetActive(shouldBeActive);
    }

    public void panel2SetActive(bool shouldBeActive)
    {
        panel2.SetActive(shouldBeActive);
    }


    public bool isActive()
    {
        return ObjectToRemove.activeSelf;
    }


    public void OnButtonClick()
    {
        if (panel1.activeSelf)
        {
            panel1SetActive(false);
            panel2SetActive(true);
            print("panel 1 is active");
        }

        else if (!panel1.activeSelf)
        {
            panel1SetActive(true);
            panel2SetActive(false);
            print("panel 1 is inactive");
        }
    }    
}
