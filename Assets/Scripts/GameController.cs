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
    [SerializeField] private List<ResourceRegion> resourceRegionObjects = new List<ResourceRegion>();
    private Dictionary<string, ResourceRegion> resourceRegions = new Dictionary<string, ResourceRegion>(); 
    private List<GameObject> features = new List<GameObject>();
    [SerializeField] private List<Text> resourceTexts = new List<Text>();
    [SerializeField] private Text turnCounter; 

    private bool buildingRoad;

    [SerializeField] private RectTransform canvasRect; 
    [SerializeField] private GameObject upgradeButton; 

    #region Properties
    public MapIconContainer IconContainer { get { return iconContainer; } }
    public Texture2D CurrentCursorTexture { get { return currentCursorTexture; } }
    public EFeatureType CurrentFeature { get { return currentFeatureSelection; } }
    public Dictionary<string, ResourceRegion> ResourceRegions { get { return resourceRegions; } }

    public bool BuildingRoad { get { return buildingRoad; } set { buildingRoad = value; } }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        foreach (ResourceRegion resReg in resourceRegionObjects)
        {
            string resourceRegionName = resReg.name.Remove(resReg.name.Length - 9);
            resourceRegionName = resourceRegionName[0].ToString().ToUpper() + resourceRegionName.Substring(1);
            resourceRegions.Add(resourceRegionName, resReg); 
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FeatureDropdownChanged(int currentIndex)
    {
        Vector2 cursorOffset = new Vector2(0, 0);
        DisableAllResourceRegionRenderers();

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
                resourceRegions["MineAndQuarry"].gameObject.GetComponent<MeshRenderer>().enabled = true;
                resourceRegions["MineAndQuarry"].gameObject.GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0, 0.25f);
            }
            else if (currentIndex == 2)
            {
                currentCursorTexture = iconContainer.LumberMillIcon;
                currentFeatureSelection = EFeatureType.LumberMill;
                resourceRegions["LumberMill"].gameObject.GetComponent<MeshRenderer>().enabled = true;
                resourceRegions["LumberMill"].gameObject.GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0, 0.25f);
            }
            else if (currentIndex == 3)
            {
                currentCursorTexture = iconContainer.MineIcon;
                currentFeatureSelection = EFeatureType.Mine;
                resourceRegions["MineAndQuarry"].gameObject.GetComponent<MeshRenderer>().enabled = true;
                resourceRegions["MineAndQuarry"].gameObject.GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0, 0.25f);
            }
            else if (currentIndex == 4)
            {
                currentCursorTexture = iconContainer.FieldIcon;
                currentFeatureSelection = EFeatureType.Field;
                resourceRegions["LivestockFarmAndField"].gameObject.GetComponent<MeshRenderer>().enabled = true;
                resourceRegions["LivestockFarmAndField"].gameObject.GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0, 0.25f);
            }
            else if (currentIndex == 5)
            {
                currentCursorTexture = iconContainer.LivestockFarmIcon;
                currentFeatureSelection = EFeatureType.LivestockFarm;
                resourceRegions["LivestockFarmAndField"].gameObject.GetComponent<MeshRenderer>().enabled = true;
                resourceRegions["LivestockFarmAndField"].gameObject.GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0, 0.25f);
            }
            else if (currentIndex == 6)
            {
                currentCursorTexture = iconContainer.WellIcon;
                currentFeatureSelection = EFeatureType.Well;
                resourceRegions["Well"].gameObject.GetComponent<MeshRenderer>().enabled = true;
                resourceRegions["Well"].gameObject.GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0, 0.25f);
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
                resourceRegions["Port"].gameObject.GetComponent<MeshRenderer>().enabled = true;
                resourceRegions["Port"].gameObject.GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0, 0.25f);
            }
            else if (currentIndex == 9)
            {
                currentCursorTexture = iconContainer.TradeRouteIcon;
                currentFeatureSelection = EFeatureType.TradeRoute;
                DisableAllResourceRegionRenderers();
            }
            else if (currentIndex == 10)
            {
                currentCursorTexture = iconContainer.AqeductIcon;
                currentFeatureSelection = EFeatureType.Aqueduct;
                DisableAllResourceRegionRenderers();
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
        foreach (MapRegions region in regions)
        {
            region.PreviousResources.Clear();

            foreach (KeyValuePair<EResources, int> resource in region.Resources)
            {
                region.PreviousResources.Add(resource.Key, resource.Value);
            }
        }

        foreach (GameObject g in features)
        {
            Feature gFeature = g.GetComponent<Feature>();

            gFeature.EndTurn();
        }

        int prosperityCounter = 0; 
        foreach (MapRegions region in regions)
        {
            prosperityCounter += CheckForRegionProsperity(region);
        }

        if (prosperityCounter == regions.Count)
        {
            // You Win!
        }

        turnCounter.text = (int.Parse(turnCounter.text) + 1).ToString(); 

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
            if (resourcePair.Key == EResources.UncountedPopulation)
                continue;

            int resourceValue = int.Parse(resourceTexts[(int)resourcePair.Key].text);
            resourceValue = resourcePair.Value;
            resourceTexts[(int)resourcePair.Key].text = resourceValue.ToString();
        }
    }

    public void ShowUpgradeButton(Feature feature)
    {
        foreach (Image i in upgradeButton.GetComponentsInChildren<Image>())
        {
            i.enabled = true;
        }

        Vector2 viewportPosition = Camera.main.WorldToViewportPoint(feature.transform.position);
        Vector2 worldObjectScreenPosition = new Vector2(
        ((viewportPosition.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f) + 40),
        ((viewportPosition.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)) + 40);

        upgradeButton.GetComponent<RectTransform>().anchoredPosition = worldObjectScreenPosition;

        upgradeButton.GetComponent<Button>().onClick.AddListener(delegate { feature.Upgrade(); });
    }

    public void HideUpgradeButton()
    {
        upgradeButton.GetComponent<Button>().onClick.RemoveAllListeners();

        foreach (Image i in upgradeButton.GetComponentsInChildren<Image>())
        {
            i.enabled = false;
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
                if (resourcePair.Key == EResources.UncountedPopulation)
                    continue;

                int resourceValue = int.Parse(resourceTexts[(int)resourcePair.Key].text);
                resourceValue += resourcePair.Value;
                resourceTexts[(int)resourcePair.Key].text = resourceValue.ToString();
            }
        }
    }

    private void DisableAllResourceRegionRenderers()
    {
        foreach (ResourceRegion resReg in resourceRegionObjects)
        {
            resReg.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    private int CheckForRegionProsperity(MapRegions region)
    {
        if (region.Resources[EResources.Population] < 200)
            return 0;

        foreach (KeyValuePair<EResources, int> resource in region.Resources)
        {
            if (region.PreviousResources[resource.Key] > resource.Value)
                return 0;
        }

        return 1;
    }
}
