using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradeRoute : MonoBehaviour
{
    private Feature[] connectedFeatures = new Feature[2];
    private GameController controller;

    private List<EResources> movingResources = new List<EResources>();
    private FeatureCosts costOfRoad;

    private int length;

    public int Length { get { return length; } set { length = value; } }
    public Feature[] ConnectedFeatures { get { return connectedFeatures; } }
    public GameController Controller { get { return controller; } }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartRoad(Feature feature)
    {
        RoadBuildHelper(0, feature);

        controller = feature.Controller;
        controller.CurrentRoad = gameObject;

        costOfRoad = controller.FeatureCosts["TradeRoute"];

        switch (feature.FeatureType)
        {
            case EFeatureType.Mine:
                movingResources.Add(EResources.Iron);
                break;
            case EFeatureType.Port:
                movingResources.Add(EResources.Water);
                movingResources.Add(EResources.UncountedPopulation);
                costOfRoad = controller.FeatureCosts["Aqueduct"];
                break;
            case EFeatureType.LumberMill:
                movingResources.Add(EResources.Wood);
                break;
            case EFeatureType.Quarry:
                movingResources.Add(EResources.Stone);
                break;
            case EFeatureType.Field:
                movingResources.Add(EResources.Grain);
                break;
            case EFeatureType.LivestockFarm:
                movingResources.Add(EResources.Meat);
                break;
            case EFeatureType.Well:
                movingResources.Add(EResources.Water);
                costOfRoad = controller.FeatureCosts["Aqueduct"];
                break;
            case EFeatureType.Village:
                movingResources.Add(EResources.Gold);
                movingResources.Add(EResources.UncountedPopulation);
                break;
            case EFeatureType.Town:
                movingResources.Add(EResources.Gold);
                movingResources.Add(EResources.UncountedPopulation);
                break;
            case EFeatureType.City:
                movingResources.Add(EResources.Gold);
                movingResources.Add(EResources.UncountedPopulation);
                break;
        }
    }

    public void EndRoad(Feature feature)
    {
        RoadBuildHelper(1, feature);
        controller.CurrentRoad = null;

        costOfRoad.BuildRoad(this);

        foreach (EResources resource in movingResources)
        {
            connectedFeatures[0].HomeRegion.TradeRoutes[resource].Add(connectedFeatures[1].HomeRegion);
            connectedFeatures[1].HomeRegion.TradeRoutes[resource].Add(connectedFeatures[0].HomeRegion);
        }
    }

    private void RoadBuildHelper(int index, Feature feature)
    {
        GetComponent<LineRenderer>().SetPosition(index, feature.transform.position);
        connectedFeatures[index] = feature;
    }
}
