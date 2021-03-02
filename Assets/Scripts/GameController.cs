using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private MapIconContainer iconContainer;
    [SerializeField] private MapIconContainer cursorContainer;

    [SerializeField] private Texture2D hazardCursorTexture; 
    private Texture2D currentCursorTexture;
    private EFeatureType currentFeatureSelection;

    [SerializeField] private Text notEnoughText; 

    [SerializeField] private List<MapRegions> regions = new List<MapRegions>();
    [SerializeField] private List<ResourceRegion> resourceRegionObjects = new List<ResourceRegion>();
    private Dictionary<string, ResourceRegion> resourceRegions = new Dictionary<string, ResourceRegion>(); 
    private List<GameObject> features = new List<GameObject>();
    [SerializeField] private List<Text> resourceTexts = new List<Text>();
    [SerializeField] private List<Text> resourceChangedTexts = new List<Text>();
    [SerializeField] private Text turnCounter; 

    private bool buildingRoad;
    private GameObject currentRoad = null; 

    [SerializeField] private RectTransform canvasRect; 
    [SerializeField] private GameObject upgradeButton;

    [SerializeField] private List<string> featureCostPairNames;
    [SerializeField] private List<FeatureCosts> featureCostsPairCosts;
    private Dictionary<string, FeatureCosts> featureCosts = new Dictionary<string, FeatureCosts>();

    [SerializeField] private int populationLimit;

    private Dictionary<EResources, int> previousResources = new Dictionary<EResources, int>();

    #region Properties
    public MapIconContainer IconContainer { get { return iconContainer; } }
    public Texture2D CurrentCursorTexture { get { return currentCursorTexture; } }
    public EFeatureType CurrentFeature { get { return currentFeatureSelection; } }
    public Dictionary<string, ResourceRegion> ResourceRegions { get { return resourceRegions; } }
    public bool BuildingRoad { get { return buildingRoad; } set { buildingRoad = value; } }
    public GameObject CurrentRoad 
    { 
        get { return currentRoad; } 
        set 
        { 
            currentRoad = value;
            if (currentRoad == null)
                buildingRoad = false;
            else
                buildingRoad = true;
        }
    }
    public Dictionary<string, FeatureCosts> FeatureCosts { get { return featureCosts; } }
    public Text NotEnoughText { get { return notEnoughText; } }
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

        for (int i = 0; i < featureCostPairNames.Count; i++)
        {
            featureCosts.Add(featureCostPairNames[i], featureCostsPairCosts[i]);
        }

        previousResources.Add(EResources.Stone, 0);
        previousResources.Add(EResources.Wood, 0);
        previousResources.Add(EResources.Iron, 0);
        previousResources.Add(EResources.Gold, 0);
        previousResources.Add(EResources.Grain, 0);
        previousResources.Add(EResources.Meat, 0);
        previousResources.Add(EResources.Water, 0);
        previousResources.Add(EResources.Population, 0);
        previousResources.Add(EResources.UncountedPopulation, 0);

        TotalResourceTexts();
        TotalPreviousResourceTexts();
    }

    // Update is called once per frame
    void Update()
    {
        if (buildingRoad)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentRoad.GetComponent<LineRenderer>().SetPosition(1, new Vector3(mousePosition.x, mousePosition.y, 0));

            Vector3[] roadPositions = new Vector3[currentRoad.GetComponent<LineRenderer>().positionCount];
            currentRoad.GetComponent<LineRenderer>().GetPositions(roadPositions);
            currentRoad.GetComponent<TradeRoute>().Length = (int)Vector3.Distance(roadPositions[0], roadPositions[1]);

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                Destroy(CurrentRoad);
                CurrentRoad = null; 
            }
        }
    }

    public void FeatureDropdownChanged(int currentIndex)
    {
        Vector2 cursorOffset = new Vector2(250, 250);
        DisableAllResourceRegionRenderers();

        if (currentIndex == 0 || currentIndex == 9|| currentIndex == 10)
        {
            foreach (GameObject g in features)
            {
                g.GetComponent<BoxCollider>().enabled = true;
            }
            
            if (currentIndex == 0)
            {
                currentCursorTexture = null;
                currentFeatureSelection = EFeatureType.None;
                cursorOffset = new Vector2(0, 0);
            }
            else if (currentIndex == 9)
            {
                currentCursorTexture = cursorContainer.TradeRouteIcon;
                currentFeatureSelection = EFeatureType.TradeRoute;
            }
            else if (currentIndex == 10)
            {
                currentCursorTexture = cursorContainer.AqeductIcon;
                currentFeatureSelection = EFeatureType.Aqueduct;
            }
        }
        else
        {
            foreach (GameObject g in features)
            {
                g.GetComponent<BoxCollider>().enabled = false; 
            }
            
            if (currentIndex == 1)
            {
                currentCursorTexture = cursorContainer.QuarryIcon;
                currentFeatureSelection = EFeatureType.Quarry;
                resourceRegions["MineAndQuarry"].gameObject.GetComponent<MeshRenderer>().enabled = true;
                resourceRegions["MineAndQuarry"].gameObject.GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0, 0.25f);
            }
            else if (currentIndex == 2)
            {
                currentCursorTexture = cursorContainer.LumberMillIcon;
                currentFeatureSelection = EFeatureType.LumberMill;
                resourceRegions["LumberMill"].gameObject.GetComponent<MeshRenderer>().enabled = true;
                resourceRegions["LumberMill"].gameObject.GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0, 0.25f);
            }
            else if (currentIndex == 3)
            {
                currentCursorTexture = cursorContainer.MineIcon;
                currentFeatureSelection = EFeatureType.Mine;
                resourceRegions["MineAndQuarry"].gameObject.GetComponent<MeshRenderer>().enabled = true;
                resourceRegions["MineAndQuarry"].gameObject.GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0, 0.25f);
            }
            else if (currentIndex == 4)
            {
                currentCursorTexture = cursorContainer.FieldIcon;
                currentFeatureSelection = EFeatureType.Field;
                resourceRegions["LivestockFarmAndField"].gameObject.GetComponent<MeshRenderer>().enabled = true;
                resourceRegions["LivestockFarmAndField"].gameObject.GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0, 0.25f);
            }
            else if (currentIndex == 5)
            {
                currentCursorTexture = cursorContainer.LivestockFarmIcon;
                currentFeatureSelection = EFeatureType.LivestockFarm;
                resourceRegions["LivestockFarmAndField"].gameObject.GetComponent<MeshRenderer>().enabled = true;
                resourceRegions["LivestockFarmAndField"].gameObject.GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0, 0.25f);
            }
            else if (currentIndex == 6)
            {
                currentCursorTexture = cursorContainer.WellIcon;
                currentFeatureSelection = EFeatureType.Well;
                resourceRegions["Well"].gameObject.GetComponent<MeshRenderer>().enabled = true;
                resourceRegions["Well"].gameObject.GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0, 0.25f);
            }
            else if (currentIndex == 7)
            {
                currentCursorTexture = cursorContainer.VillageIcon;
                currentFeatureSelection = EFeatureType.Village;
            }
            else if (currentIndex == 8)
            {
                currentCursorTexture = cursorContainer.PortIcon;
                currentFeatureSelection = EFeatureType.Port;
                resourceRegions["Port"].gameObject.GetComponent<MeshRenderer>().enabled = true;
                resourceRegions["Port"].gameObject.GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0, 0.25f);
            }
        }

        Cursor.SetCursor(currentCursorTexture, cursorOffset, CursorMode.Auto);
    }

    public IEnumerator FlashCursor()
    {
        Cursor.SetCursor(hazardCursorTexture, new Vector2(250, 250), CursorMode.Auto);
        yield return new WaitForSeconds(0.5f);
        Cursor.SetCursor(currentCursorTexture, new Vector2(250, 250), CursorMode.Auto);
    }

    public void AddFeature(Feature newFeature)
    {
        features.Add(newFeature.gameObject);
        SpecifyResourceTexts(newFeature.HomeRegion);
    }

    public void EndTurn()
    {
        previousResources.Clear();

        foreach (MapRegions region in regions)
        {
            region.PreviousResources.Clear();

            foreach (KeyValuePair<EResources, int> resource in region.Resources)
            {
                int resourceValue = resource.Value;

                if (region.TradeRoutes[resource.Key].Count != 0)
                {
                    foreach (MapRegions otherRegion in region.TradeRoutes[resource.Key])
                    {
                        resourceValue += otherRegion.Resources[resource.Key];
                    }
                }

                region.PreviousResources.Add(resource.Key, resourceValue);

                if (!previousResources.ContainsKey(resource.Key))
                    previousResources.Add(resource.Key, 0);

                previousResources[resource.Key] += resource.Value;
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
            SceneManager.LoadScene("WinScene");
        }

        turnCounter.text = (int.Parse(turnCounter.text) + 1).ToString(); 

        TotalResourceTexts();
        TotalPreviousResourceTexts();

        foreach (MapRegions region in regions)
        {
            foreach (KeyValuePair<EResources, int> resource in region.Resources)
            {
                if (resource.Value < 0)
                {
                    SceneManager.LoadScene("LoseScene");
                }
            }
        }
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
            
            int resourceValue = resourcePair.Value;

            if (region.TradeRoutes[resourcePair.Key].Count != 0)
            {
                foreach (MapRegions otherRegion in region.TradeRoutes[resourcePair.Key])
                {
                    resourceValue += otherRegion.Resources[resourcePair.Key];
                }
            }

            resourceTexts[(int)resourcePair.Key].text = resourceValue.ToString();
        }
    }

    public void SpecifyPreviousResourceText(MapRegions region)
    {
        if (region == null)
        {
            TotalPreviousResourceTexts();
            return;
        }

        foreach (KeyValuePair<EResources, int> resourcePair in region.Resources)
        {
            if (resourcePair.Key == EResources.UncountedPopulation)
                continue;

            int resourceValue = resourcePair.Value - region.PreviousResources[resourcePair.Key];

            if (resourceValue < 0)
                resourceChangedTexts[(int)resourcePair.Key].color = Color.red;
            else if (resourceValue > 0)
                resourceChangedTexts[(int)resourcePair.Key].color = Color.green;
            else
                resourceChangedTexts[(int)resourcePair.Key].color = resourceTexts[(int)resourcePair.Key].color;

            resourceChangedTexts[(int)resourcePair.Key].text = resourceValue.ToString();
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


        foreach (KeyValuePair<EResources, int> resourcePair in previousResources)
        {
            if (resourcePair.Key == EResources.UncountedPopulation)
                continue;

            int resourceValue = 0;

            foreach (MapRegions region in regions)
            {
                resourceValue += region.Resources[resourcePair.Key];
            }

            resourceTexts[(int)resourcePair.Key].text = resourceValue.ToString();
        }
    }

    private void TotalPreviousResourceTexts()
    {
        foreach (Text resourceText in resourceChangedTexts)
        {
            resourceText.text = 0.ToString();
        }

        foreach (KeyValuePair<EResources, int> resourcePair in previousResources)
        {
            if (resourcePair.Key == EResources.UncountedPopulation)
                continue;

            int resourceValue = 0; 

            foreach (MapRegions region in regions)
            {
                resourceValue += region.Resources[resourcePair.Key];
            }

            resourceValue = int.Parse(resourceTexts[(int)resourcePair.Key].text) - resourcePair.Value;

            if (resourceValue < 0)
                resourceChangedTexts[(int)resourcePair.Key].color = Color.red;
            else if (resourceValue > 0)
                resourceChangedTexts[(int)resourcePair.Key].color = Color.green;
            else
                resourceChangedTexts[(int)resourcePair.Key].color = resourceTexts[(int)resourcePair.Key].color;

            resourceChangedTexts[(int)resourcePair.Key].text = resourceValue.ToString();
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
        if (region.Resources[EResources.Population] < populationLimit)
            return 0;

        foreach (KeyValuePair<EResources, int> resource in region.Resources)
        {
            int resourceValue = resource.Value;

            if (region.TradeRoutes[resource.Key].Count != 0)
            {
                foreach (MapRegions otherRegion in region.TradeRoutes[resource.Key])
                {
                    resourceValue += otherRegion.Resources[resource.Key];
                }
            }

            if (region.PreviousResources[resource.Key] > resourceValue)
                return 0;
        }

        return 1;
    }

    public IEnumerator FlashNotEnoughText(string resource)
    {
        NotEnoughText.text = notEnoughText.text.Substring(0, 11) + resource;

        notEnoughText.transform.parent.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        notEnoughText.transform.parent.gameObject.SetActive(false);
    }
}
