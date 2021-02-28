using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Feature : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler//, IPointerClickHandler
{
    private EFeatureType featureType;
    private MapIconContainer iconContainer;
    private SpriteRenderer sRenderer;
    public SpriteRenderer backgroundSRenderer; 

    private MapRegions homeRegion; 

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
            }
        }
    }

    public MapIconContainer IconContainer { set { iconContainer = value; } }

    public MapRegions HomeRegion
    {
        get { return homeRegion; }
        set { homeRegion = value; }
    }

    // Start is called before the first frame update
    public void Start()
    {
        iconContainer = FindObjectOfType<GameController>().IconContainer;
        sRenderer = GetComponent<SpriteRenderer>();
        backgroundSRenderer = transform.Find("Background").gameObject.GetComponent<SpriteRenderer>();
        backgroundSRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        backgroundSRenderer.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        backgroundSRenderer.enabled = false;
    }
}
