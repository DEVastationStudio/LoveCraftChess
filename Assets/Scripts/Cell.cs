using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{

    [SerializeField] private GameObject obstacle;
    [SerializeField] private Renderer renderer;

    private Piece piece;
    public bool isObstacle;
    private Color color;
    private TableGenerator tGen;

    public int r = 0;
    public int c = 0;
    public bool isClickable;
    public TableObj.pieceType type;

    public void Init(TableObj.pieceType type, int r, int c, TableGenerator tGen)
    {
        color = renderer.material.GetColor("_Color");
        this.tGen = tGen;
        this.type = type;
        switch (type) 
        {
            case TableObj.pieceType.OBSTACLE:
                isObstacle = true;
                obstacle.SetActive(isObstacle);
                break;
            case TableObj.pieceType.P1ZONE:
                color = Color.blue;
                renderer.material.SetColor("_Color", color);
                break;
            case TableObj.pieceType.P2ZONE:
                color = Color.red;
                renderer.material.SetColor("_Color", color);
                break;
            case TableObj.pieceType.P1KEY:
                color = Color.cyan;
                renderer.material.SetColor("_Color", color);
                break;
            case TableObj.pieceType.P2KEY:
                color = Color.magenta;
                renderer.material.SetColor("_Color", color);
                break;
        }
        this.r = r;
        this.c = c;
    }

    public void ChangePiece(Piece p) 
    {
        piece = p;
    }

    public Piece getPiece() 
    {
        return piece;    
    }
    void OnMouseDown()
    {
        if (isClickable) 
        {
            tGen.MovePiece(r, c);
        }
    }

    public void SetClickable(bool click) 
    {
        isClickable = click;
        if(isClickable) renderer.material.SetColor("_Color", Color.yellow);
        else renderer.material.SetColor("_Color", color);
    }
}
