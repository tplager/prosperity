using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FeatureCostData", menuName = "ScriptableObjects/SpawnFeatureCostData", order = 3)]
public class FeatureCosts : ScriptableObject
{
    [SerializeField] private int stone;
    [SerializeField] private int wood;
    [SerializeField] private int iron;
    [SerializeField] private int gold;
    [SerializeField] private int grain;
    [SerializeField] private int meat;
    [SerializeField] private int water;
    [SerializeField] private int population;

    public int Stone { get => stone; set => stone = value; }
    public int Wood { get => wood; set => wood = value; }
    public int Iron { get => iron; set => iron = value; }
    public int Gold { get => gold; set => gold = value; }
    public int Grain { get => grain; set => grain = value; }
    public int Meat { get => meat; set => meat = value; }
    public int Water { get => water; set => water = value; }
    public int Population { get => population; set => population = value; }

    public bool VerifyCosts(MapRegions region)
    {
        Dictionary<EResources, int> resources = region.Resources; 

        foreach (KeyValuePair<EResources, int> resource in resources)
        {
            int resourceCost = SetResourceCostHelper(resource.Key);

            int resourceValue = resource.Value;

            if (region.TradeRoutes[resource.Key].Count != 0)
            {
                foreach (MapRegions otherRegion in region.TradeRoutes[resource.Key])
                {
                    resourceValue += otherRegion.Resources[resource.Key];
                }
            }

            if (resourceCost > resourceValue)
            {
                region.StartCoroutine(region.Controller.FlashNotEnoughText(resource.Key == EResources.UncountedPopulation ? EResources.Population.ToString() : resource.Key.ToString()));
                return false;
            }
        }

        return true;
    }

    public bool VerifyRoadCosts(TradeRoute road)
    {
        Dictionary<EResources, int> resources1 = road.ConnectedFeatures[0].HomeRegion.Resources;
        Dictionary<EResources, int> resources2 = road.ConnectedFeatures[1].HomeRegion.Resources;

        foreach (KeyValuePair<EResources, int> resource in resources1)
        {
            int resourceCost = SetResourceCostHelper(resource.Key);

            int trueCost = resourceCost * road.Length;

            if (trueCost > (resource.Value + resources2[resource.Key]))
            {
                road.ConnectedFeatures[1] = null;
                road.StartCoroutine(road.Controller.FlashNotEnoughText(resource.Key == EResources.UncountedPopulation ? EResources.Population.ToString() : resource.Key.ToString()));
                return false;
            }
        }

        return true;
    }

    public void BuildFeature(MapRegions region)
    {
        Dictionary<EResources, int> resources = region.Resources;

        Dictionary<MapRegions, Dictionary<EResources, int>> updatedResources = new Dictionary<MapRegions, Dictionary<EResources, int>>();
        updatedResources.Add(region, new Dictionary<EResources, int>());

        foreach (KeyValuePair<EResources, int> resource in resources)
        {
            int resourceCost = SetResourceCostHelper(resource.Key);

            List<MapRegions> countingRegions = region.TradeRoutes[resource.Key].ConvertAll(new System.Converter<MapRegions, MapRegions>((MapRegions m) => { return m; })); 

            while (resourceCost > resource.Value)
            {
                int startingCostPerRegion = (resourceCost - resource.Value) / region.TradeRoutes[resource.Key].Count;

                for (int i = 0; i < countingRegions.Count; i++)
                {
                    MapRegions otherRegion = countingRegions[i];

                    if (!updatedResources.ContainsKey(otherRegion))
                        updatedResources.Add(otherRegion, new Dictionary<EResources, int>());

                    int regionCost = startingCostPerRegion;

                    if (regionCost > otherRegion.Resources[resource.Key])
                    {
                        regionCost = otherRegion.Resources[resource.Key];
                        countingRegions.Remove(otherRegion);
                        i--;
                    }

                    if (!updatedResources[otherRegion].ContainsKey(resource.Key))
                        updatedResources[otherRegion].Add(resource.Key, 0);

                    updatedResources[otherRegion][resource.Key] = otherRegion.Resources[resource.Key] - regionCost;
                    resourceCost -= regionCost; 
                }
            }

            updatedResources[region].Add(resource.Key, resource.Value - resourceCost);
        }

        foreach (KeyValuePair<MapRegions, Dictionary<EResources, int>> updatedRegion in updatedResources)
        {
            foreach (KeyValuePair<EResources, int> updatedResource in updatedRegion.Value)
            {
                updatedRegion.Key.Resources[updatedResource.Key] = updatedResource.Value;
            }

            updatedRegion.Key.Controller.SpecifyResourceTexts(updatedRegion.Key);
            updatedRegion.Key.Controller.SpecifyPreviousResourceText(updatedRegion.Key);
        }
    }

    public void BuildRoad(TradeRoute road)
    {
        Dictionary<EResources, int> resources1 = road.ConnectedFeatures[0].HomeRegion.Resources;
        Dictionary<EResources, int> resources2 = road.ConnectedFeatures[1].HomeRegion.Resources;

        Dictionary<EResources, int> updatedResources1 = new Dictionary<EResources, int>();
        Dictionary<EResources, int> updatedResources2 = new Dictionary<EResources, int>();

        foreach (KeyValuePair<EResources, int> resource in resources1)
        {
            int resourceCost = SetResourceCostHelper(resource.Key);

            int trueCost = resourceCost * road.Length;

            int region1Cost = 0;
            int region2Cost = 0; 

            if (trueCost > resource.Value)
            {
                region2Cost = trueCost - resource.Value;
                trueCost -= region2Cost;
            }
            region1Cost = trueCost; 

            updatedResources1.Add(resource.Key, resources1[resource.Key] - region1Cost);
            updatedResources2.Add(resource.Key, resources2[resource.Key] - region2Cost);
        }

        foreach (KeyValuePair<EResources, int> updatedResource in updatedResources1)
        {
            resources1[updatedResource.Key] = updatedResources1[updatedResource.Key];
            resources2[updatedResource.Key] = updatedResources2[updatedResource.Key];
        }

        road.ConnectedFeatures[1].HomeRegion.Controller.SpecifyResourceTexts(road.ConnectedFeatures[0].HomeRegion);
        road.ConnectedFeatures[1].HomeRegion.Controller.SpecifyPreviousResourceText(road.ConnectedFeatures[0].HomeRegion);
    }

    private int SetResourceCostHelper(EResources resource)
    {
        int resourceCost = 0;

        switch (resource)
        {
            case EResources.Stone:
                resourceCost = stone;
                break;
            case EResources.Wood:
                resourceCost = wood;
                break;
            case EResources.Iron:
                resourceCost = iron;
                break;
            case EResources.Gold:
                resourceCost = gold;
                break;
            case EResources.Grain:
                resourceCost = grain;
                break;
            case EResources.Meat:
                resourceCost = meat;
                break;
            case EResources.Water:
                resourceCost = water;
                break;
            case EResources.Population:
                resourceCost = 0;
                break;
            case EResources.UncountedPopulation:
                resourceCost = population;
                break;
        }

        return resourceCost;
    }
}
