﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapRegions : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{ 
    private GameController controller;
    private List<GameObject> features = new List<GameObject>(); 
    private MeshRenderer meshRend; 
    [SerializeField] private GameObject featurePrefab;
    [SerializeField] private Resources resourceCap;

    //[SerializeField] private List<string> startingResourceNames = new List<string>();
    [SerializeField] private List<int> startingResourceValues = new List<int>(); 
    private Dictionary<EResources, int> resources = new Dictionary<EResources, int>();
    private Dictionary<EResources, int> previousResources = new Dictionary<EResources, int>();

    private Dictionary<string, FeatureCosts> featureCosts = new Dictionary<string, FeatureCosts>();

    private Dictionary<EResources, List<MapRegions>> tradeRoutes = new Dictionary<EResources, List<MapRegions>>();

    #region Properties
    public Dictionary<EResources, int> Resources { get { return resources; } }
    public Dictionary<EResources, int> PreviousResources { get { return previousResources; } set { previousResources = value; } }
    public Dictionary<string, FeatureCosts> FeatureCosts { get { return featureCosts; } }
    public Dictionary<EResources, List<MapRegions>> TradeRoutes { get { return tradeRoutes; } set { tradeRoutes = value; } }
    public GameController Controller { get { return controller; } }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        AddPhysicsRaycaster();

        meshRend = GetComponent<MeshRenderer>();
        meshRend.enabled = false;

        controller = FindObjectOfType<GameController>();

        resources.Add(EResources.Stone, startingResourceValues[0]);
        resources.Add(EResources.Wood, startingResourceValues[1]);
        resources.Add(EResources.Iron, startingResourceValues[2]);
        resources.Add(EResources.Gold, startingResourceValues[3]);
        resources.Add(EResources.Grain, startingResourceValues[4]);
        resources.Add(EResources.Meat, startingResourceValues[5]);
        resources.Add(EResources.Water, startingResourceValues[6]);
        resources.Add(EResources.Population, 0);
        resources.Add(EResources.UncountedPopulation, 0);

        foreach (KeyValuePair<EResources, int> resource in resources)
        {
            tradeRoutes.Add(resource.Key, new List<MapRegions>());
            previousResources.Add(resource.Key, 0);
        }

        featureCosts = controller.FeatureCosts;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void AddPhysicsRaycaster()
    {
        PhysicsRaycaster physicsRaycaster = FindObjectOfType<PhysicsRaycaster>();
        if (physicsRaycaster == null)
        {
            Camera.main.gameObject.AddComponent<PhysicsRaycaster>();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            //Debug.Log("Clicked: " + gameObject.name);

            if (controller.CurrentFeature == EFeatureType.TradeRoute || controller.CurrentFeature == EFeatureType.Aqueduct)
            {
                StartCoroutine(controller.FlashCursor());
            }
            else if (controller.CurrentFeature != EFeatureType.None)
            {
                FeatureCosts selectedFeatureCost = featureCosts[controller.CurrentFeature.ToString()];

                Vector2 clickWorldPosition = Camera.main.ScreenToWorldPoint(eventData.position);

                RaycastHit hit;
                Physics.Raycast(clickWorldPosition, new Vector3(0, 0, 1), out hit, 1);

                if (selectedFeatureCost.VerifyCosts(this))
                { 
                    if (controller.CurrentFeature != EFeatureType.Village && !hit.collider.name.Contains(controller.CurrentFeature.ToString()))
                    {
                        StartCoroutine(controller.FlashCursor());
                        return;
                    }
                    GameObject newFeature = Instantiate(featurePrefab,
                        new Vector3(clickWorldPosition.x, clickWorldPosition.y, 0),
                        Quaternion.identity);
                    newFeature.name = controller.CurrentFeature.ToString();
                    newFeature.GetComponent<Feature>().SetUpFeature(this, controller.CurrentFeature, controller);
                    newFeature.GetComponent<BoxCollider>().enabled = false;

                    features.Add(newFeature.gameObject);
                    controller.AddFeature(newFeature.GetComponent<Feature>());
                }
                else
                {
                    StartCoroutine(controller.FlashCursor()); 
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("Pointer Enter: " + gameObject.name);

        meshRend.enabled = true;
        meshRend.material.color = new Color(0, 0, 1, 0.25f);

        controller.SpecifyResourceTexts(this);
        controller.SpecifyPreviousResourceText(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("Pointer Exit: " + gameObject.name);

        meshRend.enabled = false;

        controller.SpecifyResourceTexts(null);
        controller.SpecifyPreviousResourceText(null);
    }

    public void ModifyResources(EResources resource, int amountToModify)
    {
        resources[resource] += amountToModify;

        if (resource == EResources.Population)
        {
            resources[EResources.UncountedPopulation] += amountToModify; 
        }

        controller.SpecifyResourceTexts(this);
        controller.SpecifyPreviousResourceText(this);
    }
}
