using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newTable", menuName ="ScriptableObjects/Table", order = 1)]

public class TableObj : ScriptableObject
{
    public enum pieceType 
    {
        BASIC,
        OBSTACLE,
        P1ZONE,
        P2ZONE,
        P1KEY,
        P2KEY
    }

    public int numRows;
    public int numCols;
    public string name;
    public Arr[] rows;

    void OnValidate()
    {
        if (rows.Length != numRows) Array.Resize(ref rows, numRows);
        for (int i = 0; i<numRows; i++) 
        {
            if (rows[i].cols.Length != numCols) Array.Resize(ref rows[i].cols, numCols);
        }
    }

}

[System.Serializable]
public class Arr 
{
    public TableObj.pieceType[] cols;
}