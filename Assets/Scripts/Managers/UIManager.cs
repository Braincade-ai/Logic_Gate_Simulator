﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject componentPrefab;
    public GameObject UIPrefab;
    public Transform UIParent;

    private List<GameManager> components = new List<GameManager>();


    private Camera cam;

    private Transform moveComponent;
    private LevelManager currentSelectedComponent;
    private LevelManager currNodeConnector;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();

        components.AddRange(Resources.LoadAll<GameManager>("ComponentData/"));

        foreach (GameManager comp in components)
        {
            //Create UI
            GameObject newButton = Instantiate(UIPrefab, transform.position, Quaternion.identity);
            newButton.transform.SetParent(UIParent);

            //Change Button
            if (newButton.GetComponent<Button>())
            {
                Button buttonComp = newButton.GetComponent<Button>();

                buttonComp.onClick.AddListener(delegate { CreateComponent(comp); });

                var colors = buttonComp.colors;
                colors.normalColor = comp.componentColor;

                buttonComp.colors = colors;
            }

            //Change Text
            if (newButton.GetComponentInChildren<TextMeshProUGUI>())
            {
                TextMeshProUGUI buttonText = newButton.GetComponentInChildren<TextMeshProUGUI>();
                buttonText.text = comp.componentName;

                float avgColor = (comp.componentColor.r + comp.componentColor.g + comp.componentColor.b) / 3;

                buttonText.color = avgColor > 127 ? Color.black : Color.white;
            }

            newButton.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) //Left mouse button
        {
            //Check to see if object is at position
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 1000);

            if (hit.collider != null)
            {
                if (hit.collider.GetComponentInParent<LevelManager>()) //Hit node
                {
                    currNodeConnector = hit.collider.GetComponentInParent<LevelManager>();
                    if (currNodeConnector.inputNodes.Contains(hit.transform.gameObject))
                    {
                        currNodeConnector = null;
                    }
                }
                if (hit.collider.GetComponent<LevelManager>()) //Hit component
                {
                    currentSelectedComponent = hit.collider.GetComponent<LevelManager>();
                    moveComponent = hit.transform;
                }
            }
        }

        if (Input.GetMouseButtonDown(1)) //Right Mouse Button - Interact with inputs
        {
            //Check to see if object is at position
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 1000);

            if (hit.collider != null)
            {
                if (hit.collider.GetComponentInParent<LevelManager>()) //Hit node
                {
                    LevelManager comp = hit.collider.GetComponentInParent<LevelManager>();
                    if (comp.componentData.isInput)
                    {
                        switch (comp.componentData.componentType)
                        {
                            case GameManager.ComponentType.SWITCH:
                                comp.componentActive = !comp.componentActive;
                                break;
                        }
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Delete))
        {
            if (currentSelectedComponent != null)
            {
                currentSelectedComponent.DeleteComponent();
                currentSelectedComponent = null;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (currNodeConnector != null)
            {
                //Check to see if object is at position
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 1000);

                if (hit.collider != null) //Hit something
                {
                    if (hit.collider.GetComponentInParent<LevelManager>()) //Hit node
                    {
                        LevelManager parentComponent = hit.collider.GetComponentInParent<LevelManager>();
                        if (parentComponent != currNodeConnector)
                        {
                            //DRAW LINE BETWEEN NODES
                            parentComponent.connections.Add(new Connection(parentComponent,currNodeConnector));
                            Debug.Log("Connected nodes");
                        }
                    }
                }
            }
            currNodeConnector = null;
            moveComponent = null;
        }

        if (moveComponent != null)
        {
            moveComponent.position = (Vector2)cam.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    public void CreateComponent(GameManager newComp)
    {
        Vector2 mousePos = cam.ScreenToWorldPoint(new Vector2(Screen.width / 2, Screen.height / 2)); //Convert screen center sto world pos

        GameObject newComponent = Instantiate(componentPrefab, transform.position, Quaternion.identity);
        newComponent.transform.position = (Vector2)mousePos;

        if (newComponent.GetComponent<LevelManager>())
        {
            LevelManager comp = newComponent.GetComponent<LevelManager>();
            comp.componentData = newComp;
            comp.canvasUI.worldCamera = cam;
            comp.compUIText.text = comp.componentData.componentName;
        }
    }
}
