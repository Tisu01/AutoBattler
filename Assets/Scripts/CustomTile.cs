using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class CustomTile : Tile
{
    public TileData tileData;

#if UNITY_EDITOR
    // The following is a helper that adds a menu item to create a RoadTile Asset
    [MenuItem("Assets/Create/CustomTile")]
    public static void CreateCustomTile()
    {
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<CustomTile>(), "Assets/NewCustomTile.asset");
    }
#endif
}

[System.Serializable]
public class TileData
{
    public bool unitCanEnter = true;
    public TileType tileType;
    public TeamSlot teamSlot;
}

public enum TileType
{
    none,
    desert,
    forest,
    castle,
    water,
    grass,
    plains
}

public enum TeamSlot
{
    none,
    team1,
    team2,
    both
}