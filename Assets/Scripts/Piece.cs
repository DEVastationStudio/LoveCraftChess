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
    [SerializeField] private GameObject pawn;
    [SerializeField] private GameObject rook;
    [SerializeField] private GameObject knight;
    [SerializeField] private GameObject bishop;
    [SerializeField] private GameObject queen;

    private Renderer renderer2;

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
        color = (player==1)?Color.blue:Color.red;//renderer.material.GetColor("_Color");
        ChangeModel();
        renderer.material.SetColor("_Color", color);
        renderer2.material.SetColor("_Color", color);
    }

    private void ChangeModel()
    {
        switch (type) 
        {
            case pieces.PAWN:
                pawn.SetActive(true);
                renderer2 = pawn.GetComponent<Renderer>();
                break;
            case pieces.ROOK:
                rook.SetActive(true);
                renderer2 = rook.GetComponent<Renderer>();
                break;
            case pieces.KNIGHT:
                knight.SetActive(true);
                renderer2 = knight.GetComponent<Renderer>();
                break;
            case pieces.BISHOP:
                bishop.SetActive(true);
                renderer2 = bishop.GetComponent<Renderer>();
                break;
            case pieces.QUEEN:
                queen.SetActive(true);
                renderer2 = queen.GetComponent<Renderer>();
                break;
        }
    }

    void OnMouseDown()
    {
        if (inJail) return;
        tGen.SelectPiece(r, c);
    }

    public void SetChosen(bool b) 
    {
        if (b)
        {
            renderer.material.SetColor("_Color", Color.green);
            renderer2.material.SetColor("_Color", Color.green);
        }
        else
        {
            renderer.material.SetColor("_Color", color);
            renderer2.material.SetColor("_Color", color);
        }
    }

    public void SetPosition(int r, int c) 
    {
        this.r = r;
        this.c = c;
        transform.position = new Vector3(c, 1, r);
    }
    public void SetJailPosition(Cell c) 
    {
        this.r = -1;
        this.c = -1;
        transform.position = new Vector3(c.transform.position.x, 1, c.transform.position.z);
    }
}
