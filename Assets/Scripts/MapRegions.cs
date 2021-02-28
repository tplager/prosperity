using System;
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
    private Dictionary<EResources, int> resources = new Dictionary<EResources, int>();

    [SerializeField] private List<string> featureCostPairNames;
    [SerializeField] private List<FeatureCosts> featureCostsPairCosts;
    private Dictionary<string, FeatureCosts> featureCosts = new Dictionary<string, FeatureCosts>(); 

    public Dictionary<EResources, int> Resources { get { return resources; } }
    // Start is called before the first frame update
    void Start()
    {
        AddPhysicsRaycaster();

        meshRend = GetComponent<MeshRenderer>();
        meshRend.enabled = false;

        controller = FindObjectOfType<GameController>();

        resources.Add(EResources.Stone, 0);
        resources.Add(EResources.Wood, 0);
        resources.Add(EResources.Iron, 0);
        resources.Add(EResources.Gold, 0);
        resources.Add(EResources.Grain, 0);
        resources.Add(EResources.Meat, 0);
        resources.Add(EResources.Water, 0); 

        for (int i = 0; i < featureCostPairNames.Count; i++)
        {
            featureCosts.Add(featureCostPairNames[i], featureCostsPairCosts[i]); 
        }
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
            Debug.Log("Clicked: " + gameObject.name);

            if (controller.CurrentFeature != EFeatureType.None)
            {
                FeatureCosts selectedFeatureCost = featureCosts[controller.CurrentFeature.ToString()];

                if (selectedFeatureCost.VerifyCosts(Resources))
                { 
                    Vector2 clickWorldPosition = Camera.main.ScreenToWorldPoint(eventData.position);
                    GameObject newFeature = Instantiate(featurePrefab,
                        new Vector3(clickWorldPosition.x, clickWorldPosition.y, 0),
                        Quaternion.identity);
                    newFeature.name = controller.CurrentFeature.ToString();
                    newFeature.GetComponent<Feature>().Start();
                    newFeature.GetComponent<Feature>().HomeRegion = this;
                    newFeature.GetComponent<Feature>().FeatureType = controller.CurrentFeature;
                    newFeature.GetComponent<BoxCollider>().enabled = false;

                    features.Add(newFeature.gameObject);
                    controller.AddFeature(newFeature.GetComponent<Feature>());
                }
                else
                {
                    StartCoroutine(controller.FlashCursor()); 
                    // Change color of cursor or something indicating that it can't be built
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Pointer Enter: " + gameObject.name);

        meshRend.enabled = true;
        meshRend.material.color = new Color(0, 0, 1, 0.25f);

        controller.SpecifyResourceTexts(this); 
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Pointer Exit: " + gameObject.name);

        meshRend.enabled = false;

        controller.SpecifyResourceTexts(null);
    }

    public void ModifyResources(EResources resource, int amountToModify)
    {
        resources[resource] += amountToModify;
    }
}
