using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private MapIconContainer iconContainer;

    [SerializeField] private Texture2D hazardCursorTexture; 
    private Texture2D currentCursorTexture;
    private EFeatureType currentFeatureSelection;

    [SerializeField] private List<MapRegions> regions = new List<MapRegions>();
    private List<GameObject> features = new List<GameObject>();
    [SerializeField] private List<Text> resourceTexts = new List<Text>();

    private bool cursorFlashing;

    #region Properties
    public MapIconContainer IconContainer { get { return iconContainer; } }
    public Texture2D CurrentCursorTexture { get { return currentCursorTexture; } }
    public EFeatureType CurrentFeature { get { return currentFeatureSelection; } }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FeatureDropdownChanged(int currentIndex)
    {
        Vector2 cursorOffset = new Vector2(0, 0); 

        if (currentIndex == 0)
        {
            currentCursorTexture = null;
            currentFeatureSelection = EFeatureType.None;

            foreach (GameObject g in features)
            {
                g.GetComponent<BoxCollider>().enabled = true;
            }
        }
        else
        {
            foreach (GameObject g in features)
            {
                g.GetComponent<BoxCollider>().enabled = false; 
            }

            cursorOffset = new Vector2(250, 250);
            
            if (currentIndex == 1)
            {
                currentCursorTexture = iconContainer.QuarryIcon;
                currentFeatureSelection = EFeatureType.Quarry;
            }
            else if (currentIndex == 2)
            {
                currentCursorTexture = iconContainer.LumberMillIcon;
                currentFeatureSelection = EFeatureType.LumberMill;
            }
            else if (currentIndex == 3)
            {
                currentCursorTexture = iconContainer.MineIcon;
                currentFeatureSelection = EFeatureType.Mine;
            }
            else if (currentIndex == 4)
            {
                currentCursorTexture = iconContainer.FieldIcon;
                currentFeatureSelection = EFeatureType.Field;
            }
            else if (currentIndex == 5)
            {
                currentCursorTexture = iconContainer.LivestockFarmIcon;
                currentFeatureSelection = EFeatureType.LivestockFarm;
            }
            else if (currentIndex == 6)
            {
                currentCursorTexture = iconContainer.WellIcon;
                currentFeatureSelection = EFeatureType.Well;
            }
            else if (currentIndex == 7)
            {
                currentCursorTexture = iconContainer.VillageIcon;
                currentFeatureSelection = EFeatureType.Village;
            }
            else if (currentIndex == 8)
            {
                currentCursorTexture = iconContainer.PortIcon;
                currentFeatureSelection = EFeatureType.Port;
            }
        }

        Cursor.SetCursor(currentCursorTexture, cursorOffset, CursorMode.Auto);
    }

    public IEnumerator FlashCursor()
    {
        Cursor.SetCursor(hazardCursorTexture, new Vector2(250, 250), CursorMode.Auto);
        yield return new WaitForSeconds(0.25f);
        Cursor.SetCursor(currentCursorTexture, new Vector2(250, 250), CursorMode.Auto);
    }

    public void AddFeature(Feature newFeature)
    {
        features.Add(newFeature.gameObject);
    }

    public void EndTurn()
    {
        foreach (GameObject g in features)
        {
            Feature gFeature = g.GetComponent<Feature>();

            switch (gFeature.FeatureType)
            {
                case EFeatureType.Mine:
                    gFeature.HomeRegion.ModifyResources(EResources.Iron, 5); 
                    break;
                case EFeatureType.Village:
                    gFeature.HomeRegion.ModifyResources(EResources.Gold, 2);
                    gFeature.HomeRegion.ModifyResources(EResources.Meat, -2);
                    gFeature.HomeRegion.ModifyResources(EResources.Water, -2);
                    break;
                case EFeatureType.Port:
                    gFeature.HomeRegion.ModifyResources(EResources.Meat, 2);
                    break;
                case EFeatureType.LumberMill:
                    gFeature.HomeRegion.ModifyResources(EResources.Wood, 5);
                    break;
                case EFeatureType.Quarry:
                    gFeature.HomeRegion.ModifyResources(EResources.Stone, 5);
                    break;
                case EFeatureType.Field:
                    gFeature.HomeRegion.ModifyResources(EResources.Grain, 5);
                    break;
                case EFeatureType.LivestockFarm:
                    gFeature.HomeRegion.ModifyResources(EResources.Meat, 5);
                    break;
                case EFeatureType.Well:
                    gFeature.HomeRegion.ModifyResources(EResources.Water, 5);
                    break;
            }
        }

        TotalResourceTexts(); 
    }

    public void SpecifyResourceTexts(MapRegions region)
    {
        if (region == null)
        {
            TotalResourceTexts();
            return; 
        }

        foreach (KeyValuePair<EResources, int> resourcePair in region.Resources)
        {
            int resourceValue = int.Parse(resourceTexts[(int)resourcePair.Key].text);
            resourceValue = resourcePair.Value;
            resourceTexts[(int)resourcePair.Key].text = resourceValue.ToString();
        }
    }

    private void TotalResourceTexts()
    {
        foreach (Text resourceText in resourceTexts)
        {
            resourceText.text = 0.ToString();
        }

        foreach (MapRegions region in regions)
        {
            foreach (KeyValuePair<EResources, int> resourcePair in region.Resources)
            {
                int resourceValue = int.Parse(resourceTexts[(int)resourcePair.Key].text);
                resourceValue += resourcePair.Value;
                resourceTexts[(int)resourcePair.Key].text = resourceValue.ToString();
            }
        }
    }
}
