﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapIconData", menuName = "ScriptableObjects/SpawnMapIconData", order = 1)]
public class MapIconContainer : ScriptableObject
{
    [SerializeField] private Texture2D quarryIcon;
    [SerializeField] private Texture2D lumberMillIcon;
    [SerializeField] private Texture2D mineIcon;
    [SerializeField] private Texture2D fieldIcon;
    [SerializeField] private Texture2D livestockFarmIcon;
    [SerializeField] private Texture2D wellIcon;
    [SerializeField] private Texture2D villageIcon;
    [SerializeField] private Texture2D portIcon;
    [SerializeField] private Texture2D roadIcon;
    [SerializeField] private Texture2D aqeductIcon;

    public Texture2D QuarryIcon { get => quarryIcon; set => quarryIcon = value; }
    public Texture2D LumberMillIcon { get => lumberMillIcon; set => lumberMillIcon = value; }
    public Texture2D MineIcon { get => mineIcon; set => mineIcon = value; }
    public Texture2D FieldIcon { get => fieldIcon; set => fieldIcon = value; }
    public Texture2D LivestockFarmIcon { get => livestockFarmIcon; set => livestockFarmIcon = value; }
    public Texture2D WellIcon { get => wellIcon; set => wellIcon = value; }
    public Texture2D VillageIcon { get => villageIcon; set => villageIcon = value; }
    public Texture2D PortIcon { get => portIcon; set => portIcon = value; }
    public Texture2D RoadIcon { get => roadIcon; set => roadIcon = value; }
    public Texture2D AqeductIcon { get => aqeductIcon; set => aqeductIcon = value; }
}