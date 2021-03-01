using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Feature : MonoBehaviour
{
    private EFeatureType featureType;
    private MapIconContainer iconContainer;
    private SpriteRenderer sRenderer;
    public SpriteRenderer backgroundSRenderer; 

    private MapRegions homeRegion;

    private float timeSinceLastClick = 0;
    private int clicks;

    private GameController controller;

    [SerializeField] private GameObject tradeRoutePrefab;
    [SerializeField] private GameObject aqueductPrefab; 

    public EFeatureType FeatureType
    {
        get { return featureType; }
        set
        {
            featureType = value;

            switch (featureType)
            {
                case EFeatureType.Quarry: 
                    sRenderer.sprite = Sprite.Create(iconContainer.QuarryIcon, 
                        new Rect(0, 0, iconContainer.QuarryIcon.width, iconContainer.QuarryIcon.height), 
                        new Vector2(0.5f, 0.5f));
                    break;
                case EFeatureType.LumberMill:                    
                    sRenderer.sprite = Sprite.Create(iconContainer.LumberMillIcon,
                        new Rect(0, 0, iconContainer.LumberMillIcon.width, iconContainer.LumberMillIcon.height),
                        new Vector2(0.5f, 0.5f));
                    break;
                case EFeatureType.Mine:
                    sRenderer.sprite = Sprite.Create(iconContainer.MineIcon,
                        new Rect(0, 0, iconContainer.MineIcon.width, iconContainer.MineIcon.height),
                        new Vector2(0.5f, 0.5f));
                    break;
                case EFeatureType.Field:
                    sRenderer.sprite = Sprite.Create(iconContainer.FieldIcon,
                        new Rect(0, 0, iconContainer.FieldIcon.width, iconContainer.FieldIcon.height),
                        new Vector2(0.5f, 0.5f));
                    break;
                case EFeatureType.LivestockFarm:
                    sRenderer.sprite = Sprite.Create(iconContainer.LivestockFarmIcon,
                        new Rect(0, 0, iconContainer.LivestockFarmIcon.width, iconContainer.LivestockFarmIcon.height),
                        new Vector2(0.5f, 0.5f));
                    break;
                case EFeatureType.Well:
                    sRenderer.sprite = Sprite.Create(iconContainer.WellIcon,
                        new Rect(0, 0, iconContainer.WellIcon.width, iconContainer.WellIcon.height),
                        new Vector2(0.5f, 0.5f));
                    break;
                case EFeatureType.Village:
                    sRenderer.sprite = Sprite.Create(iconContainer.VillageIcon,
                        new Rect(0, 0, iconContainer.VillageIcon.width, iconContainer.VillageIcon.height),
                        new Vector2(0.5f, 0.5f));
                    break;
                case EFeatureType.Port:
                    sRenderer.sprite = Sprite.Create(iconContainer.PortIcon,
                        new Rect(0, 0, iconContainer.PortIcon.width, iconContainer.PortIcon.height),
                        new Vector2(0.5f, 0.5f));
                    break;
                case EFeatureType.Town:
                    sRenderer.sprite = Sprite.Create(iconContainer.TownIcon,
                        new Rect(0, 0, iconContainer.VillageIcon.width, iconContainer.VillageIcon.height),
                        new Vector2(0.5f, 0.5f));
                    break;
                case EFeatureType.City:
                    sRenderer.sprite = Sprite.Create(iconContainer.CityIcon,
                        new Rect(0, 0, iconContainer.VillageIcon.width, iconContainer.VillageIcon.height),
                        new Vector2(0.5f, 0.5f));
                    break;
            }
        }
    }

    public MapIconContainer IconContainer { set { iconContainer = value; } }

    public MapRegions HomeRegion
    {
        get { return homeRegion; }
    }

    public GameController Controller { get { return controller; } set { controller = value; } }

    // Start is called before the first frame update
    void Start()
    {
        iconContainer = FindObjectOfType<GameController>().IconContainer;
        sRenderer = GetComponent<SpriteRenderer>();
        backgroundSRenderer = transform.Find("Background").gameObject.GetComponent<SpriteRenderer>();
        backgroundSRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (clicks != 0)
            timeSinceLastClick += Time.deltaTime;
    }
    
    public void SetUpFeature(MapRegions home, EFeatureType type, GameController controller)
    {
        Start();
        homeRegion = home;
        FeatureType = type;
        this.controller = controller;

        BuildFeature();
    }

    public void BuildFeature()
    {
        if (featureType == EFeatureType.Village)
        {
            homeRegion.ModifyResources(EResources.Population, 5);
        }
        else if (featureType == EFeatureType.Town)
        {
            homeRegion.ModifyResources(EResources.Population, 7);
        }
        else if (featureType == EFeatureType.City)
        {
            homeRegion.ModifyResources(EResources.Population, 13);
        }

        homeRegion.FeatureCosts[featureType.ToString()].BuildFeature(homeRegion);
    }

    public void OnMouseEnter()
    {
        backgroundSRenderer.enabled = true;

        if (Camera.main.orthographicSize == 1 && (featureType == EFeatureType.Village || featureType == EFeatureType.Town))
        {
            controller.ShowUpgradeButton(this);
        }
    }

    public void OnMouseExit()
    {
        backgroundSRenderer.enabled = false;

        controller.HideUpgradeButton();
    }

    public void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (clicks == 1 && timeSinceLastClick < 0.5f && Camera.main.GetComponent<Zoom>().targetOrtho != 1)
            {
                clicks++;
                Camera.main.GetComponent<Zoom>().targetOrtho = 1;
                StartCoroutine(Camera.main.GetComponent<Zoom>().MoveCameraToPoint(transform.position));

                //Debug.Log("Double Click");
            }
            else// if ((clicks == 2 || clicks == 0) && Camera.main.GetComponent<Zoom>().targetOrtho != 1)
            {
                clicks = 1;

                //Debug.Log("Single Click");

                if (controller.CurrentFeature == EFeatureType.Aqueduct || controller.CurrentFeature == EFeatureType.TradeRoute)
                {
                    if (!controller.BuildingRoad)
                    {
                        if (controller.CurrentFeature == EFeatureType.Aqueduct)
                        {
                            if (featureType == EFeatureType.Port || featureType == EFeatureType.Well)
                            {
                                // Instantiate a new aqueduct prefab and give control of it to the game controller
                                // Game controller needs to update it each frame so it's at the same position as the mouse
                                // Until either a different feature is clicked that can accept the aqueduct or until it is 
                                // cancelled by clicking somewhere else or hitting enter or backspace or something
                                GameObject aqueduct = Instantiate(aqueductPrefab);
                                aqueduct.GetComponent<TradeRoute>().StartRoad(this);
                            }
                            else
                            {
                                StartCoroutine(controller.FlashCursor());
                            }
                        }
                        else if (controller.CurrentFeature == EFeatureType.TradeRoute)
                        {
                            if (featureType == EFeatureType.Port || featureType == EFeatureType.Well)
                            {
                                StartCoroutine(controller.FlashCursor());
                            }
                            else
                            {
                                GameObject tradeRoute = Instantiate(tradeRoutePrefab);
                                tradeRoute.GetComponent<TradeRoute>().StartRoad(this);
                            }
                        }
                    }
                    else
                    {
                        if (featureType == EFeatureType.Port || featureType == EFeatureType.Village || featureType == EFeatureType.Town || featureType == EFeatureType.City)
                        {
                            GameObject tradeRoute = controller.CurrentRoad;
                            tradeRoute.GetComponent<TradeRoute>().ConnectedFeatures[1] = this;

                            if ((controller.CurrentFeature == EFeatureType.Aqueduct && controller.FeatureCosts["Aqueduct"].VerifyRoadCosts(tradeRoute.GetComponent<TradeRoute>())) ||
                                controller.CurrentFeature == EFeatureType.TradeRoute && controller.FeatureCosts["TradeRoute"].VerifyRoadCosts(tradeRoute.GetComponent<TradeRoute>()))
                            {
                                // Finish the Trade Route or Aqueduct and connect it to both features
                                tradeRoute.GetComponent<TradeRoute>().EndRoad(this);
                            }
                            else
                            {
                                StartCoroutine(controller.FlashCursor());
                            }
                        }
                    }
                }
            }

            timeSinceLastClick = 0;
        }
    }

    public void EndTurn()
    {
        switch (featureType)
        {
            case EFeatureType.Mine:
                homeRegion.ModifyResources(EResources.Iron, 5);
                break;
            case EFeatureType.Port:
                homeRegion.ModifyResources(EResources.Meat, 2);
                homeRegion.ModifyResources(EResources.Water, 5);
                break;
            case EFeatureType.LumberMill:
                homeRegion.ModifyResources(EResources.Wood, 5);
                break;
            case EFeatureType.Quarry:
                homeRegion.ModifyResources(EResources.Stone, 5);
                break;
            case EFeatureType.Field:
                homeRegion.ModifyResources(EResources.Grain, 5);
                break;
            case EFeatureType.LivestockFarm:
                homeRegion.ModifyResources(EResources.Meat, 5);
                break;
            case EFeatureType.Well:
                homeRegion.ModifyResources(EResources.Water, 5);
                break;
            case EFeatureType.Village:
                homeRegion.ModifyResources(EResources.Gold, 2);
                homeRegion.ModifyResources(EResources.Grain, -2);
                homeRegion.ModifyResources(EResources.Meat, -2);
                homeRegion.ModifyResources(EResources.Water, -2);
                break;
            case EFeatureType.Town:
                homeRegion.ModifyResources(EResources.Gold, 5);
                homeRegion.ModifyResources(EResources.Grain, -5);
                homeRegion.ModifyResources(EResources.Meat, -5);
                homeRegion.ModifyResources(EResources.Water, -5);
                break;
            case EFeatureType.City:
                homeRegion.ModifyResources(EResources.Gold, 12);
                homeRegion.ModifyResources(EResources.Grain, -12);
                homeRegion.ModifyResources(EResources.Meat, -12);
                homeRegion.ModifyResources(EResources.Water, -12);
                break;
        }
    }

    public void Upgrade()
    {
        EFeatureType upgradedFeature = EFeatureType.None;

        switch (featureType)
        {
            case EFeatureType.Village:
                upgradedFeature = EFeatureType.Town;
                break;
            case EFeatureType.Town:
                upgradedFeature = EFeatureType.City;
                break;
        }

        if (upgradedFeature != EFeatureType.None)
        {
            FeatureCosts selectedFeatureCost = homeRegion.FeatureCosts[upgradedFeature.ToString()];

            if (selectedFeatureCost.VerifyCosts(homeRegion))
            {
                FeatureType = upgradedFeature;
                BuildFeature();
            }
            else
                StartCoroutine(controller.FlashCursor());
        }
    }

    private void UpdateFeatureType(EFeatureType featureType)
    {

    }
}
