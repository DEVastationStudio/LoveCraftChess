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
        P2KEY,
        BASIC1,
        BASIC2,
        P11JAIL,
        P12JAIL,
        P13JAIL,
        P21JAIL,
        P22JAIL,
        P23JAIL,
        P1REVIVE1,
        P1REVIVE2,
        P1REVIVE3,
        P2REVIVE1,
        P2REVIVE2,
        P2REVIVE3,
        BARRIER,
        BARRIERBTN,
        PIT,
        PITBTN, 
        P1GOD,
        P2GOD
    }

    public string name;
    public Sprite preview;
    public int numRows = 3;
    public int numCols = 3;
    public Arr[] rows;
    public JailCell[] p1JailCells;
    public JailCell p1GodCell;
    public JailCell[] p2JailCells;
    public JailCell p2GodCell;

    void OnValidate()
    {
        if (rows.Length != numRows) Array.Resize(ref rows, numRows);
        for (int i = 0; i<numRows; i++) 
        {
            if (rows[i].cols.Length != numCols) Array.Resize(ref rows[i].cols, numCols);
        }
        if (p1JailCells.Length != 3) Array.Resize(ref p1JailCells, 3);
        if (p2JailCells.Length != 3) Array.Resize(ref p2JailCells, 3);
    }
}

[System.Serializable]
public class Arr 
{
    public TableObj.pieceType[] cols;
}

[System.Serializable]
public class JailCell 
{
    public float row;
    public float col;
}