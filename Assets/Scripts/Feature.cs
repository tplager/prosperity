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
    }

    public void OnMouseEnter()
    {
        backgroundSRenderer.enabled = true;

        if (Camera.main.orthographicSize == 1)
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

                Debug.Log("Double Click");
            }
            else// ((clicks == 2 || clicks == 0) && Camera.main.GetComponent<Zoom>().targetOrtho != 1)
            {
                clicks = 1;

                Debug.Log("Single Click");
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
                homeRegion.ModifyResources(EResources.Meat, -2);
                homeRegion.ModifyResources(EResources.Water, -2);
                break;
            case EFeatureType.Town:
                homeRegion.ModifyResources(EResources.Gold, 5);
                homeRegion.ModifyResources(EResources.Meat, -5);
                homeRegion.ModifyResources(EResources.Water, -5);
                break;
            case EFeatureType.City:
                homeRegion.ModifyResources(EResources.Gold, 12);
                homeRegion.ModifyResources(EResources.Meat, -12);
                homeRegion.ModifyResources(EResources.Water, -12);
                break;
        }
    }

    public void Upgrade()
    {
        switch (featureType)
        {
            //case EFeatureType.Mine:
            //    homeRegion.ModifyResources(EResources.Iron, 5);
            //    break;
            //case EFeatureType.Port:
            //    homeRegion.ModifyResources(EResources.Meat, 2);
            //    homeRegion.ModifyResources(EResources.Water, 5);
            //    break;
            //case EFeatureType.LumberMill:
            //    homeRegion.ModifyResources(EResources.Wood, 5);
            //    break;
            //case EFeatureType.Quarry:
            //    homeRegion.ModifyResources(EResources.Stone, 5);
            //    break;
            //case EFeatureType.Field:
            //    homeRegion.ModifyResources(EResources.Grain, 5);
            //    break;
            //case EFeatureType.LivestockFarm:
            //    homeRegion.ModifyResources(EResources.Meat, 5);
            //    break;
            //case EFeatureType.Well:
            //    homeRegion.ModifyResources(EResources.Water, 5);
            //    break;
            case EFeatureType.Village:
                FeatureType = EFeatureType.Town;
                break;
            case EFeatureType.Town:
                FeatureType = EFeatureType.City;
                break;
        }
    }

    private void UpdateFeatureType(EFeatureType featureType)
    {

    }
}
