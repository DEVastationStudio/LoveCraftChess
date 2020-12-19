using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public enum pieces 
    {
        PAWN, //X3
        ROOK, //X2
        KNIGHT, //X1
        BISHOP, //X2
        QUEEN //X1
    }

    [SerializeField] private Renderer renderer;

    private TableGenerator tGen;
    public int player;
    public int r;
    public int c;
    public pieces type;
    private Color color;
    public bool inJail;

    public void Init(pieces piece, int r, int c, TableGenerator tGen, int player) 
    {
        this.r = r;
        this.c = c;
        type = piece;
        this.tGen = tGen;
        this.player = player;
        color = renderer.material.GetColor("_Color");
        ChangeModel();
    }

    private void ChangeModel()
    {
        
    }

    void OnMouseDown()
    {
        if (inJail) return;
        tGen.SelectPiece(r, c);
    }

    public void SetChosen(bool b) 
    {
        if (b)
            renderer.material.SetColor("_Color", Color.green);
        else
            renderer.material.SetColor("_Color", color);
    }

    public void SetPosition(int r, int c) 
    {
        this.r = r;
        this.c = c;
        transform.position = new Vector3(c, 1, r);
    }
}
