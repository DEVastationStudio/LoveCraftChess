using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Cell : MonoBehaviourPunCallbacks
{

    [SerializeField] private GameObject obstacle;
    [SerializeField] private Renderer renderer;
    [SerializeField] private Material OneRevive;
    [SerializeField] private Material TwoRevive;
    [SerializeField] private Material ThreeRevive;

    private Piece piece;
    public bool isObstacle;
    private Color color;
    private TableGenerator tGen;

    public int r = 0;
    public int c = 0;
    public bool isClickable;
    public TableObj.pieceType type;

    public bool isBarrier;
    public GameObject barrier;
    public GameObject barrierBtn;

    public bool isPit;
    public GameObject pitBtn;
    public GameObject pit;
    public void Init(TableObj.pieceType type, int r, int c, TableGenerator tGen)
    {
        color = renderer.material.GetColor("_Color");
        this.tGen = tGen;
        this.type = type;
        transform.eulerAngles = new Vector3(0f, 180f, 0f);
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
            case TableObj.pieceType.P1REVIVE1:
                renderer.material = OneRevive;
                color = Color.cyan;
                renderer.material.SetColor("_Color", color);
                break;
            case TableObj.pieceType.P1REVIVE2:
                renderer.material = TwoRevive;
                color = Color.cyan;
                renderer.material.SetColor("_Color", color);
                break;
            case TableObj.pieceType.P1REVIVE3:
                renderer.material = ThreeRevive;
                color = Color.cyan;
                renderer.material.SetColor("_Color", color);
                break;
            case TableObj.pieceType.P2REVIVE1:
                renderer.material = OneRevive;
                color = Color.magenta;
                renderer.material.SetColor("_Color", color);
                break;
            case TableObj.pieceType.P2REVIVE2:
                renderer.material = TwoRevive;
                color = Color.magenta;
                renderer.material.SetColor("_Color", color);
                break;
            case TableObj.pieceType.P2REVIVE3:
                renderer.material = ThreeRevive;
                color = Color.magenta;
                renderer.material.SetColor("_Color", color);
                break;
            case TableObj.pieceType.P1JAIL:
            case TableObj.pieceType.P2JAIL:
                color = Color.grey;
                renderer.material.SetColor("_Color", color);
                break;
            case TableObj.pieceType.BARRIER:
                isBarrier = true;
                barrier.SetActive(true);
                break;
            case TableObj.pieceType.BARRIERBTN:
                barrierBtn.SetActive(true);
                break;
            case TableObj.pieceType.PIT:
                isPit = true;
                pit.SetActive(true);
                break;
            case TableObj.pieceType.PITBTN:
                pitBtn.SetActive(true);
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
            int[] _movePieceParams = new int[4];
            _movePieceParams[0] = r;
            _movePieceParams[1] = c;
            _movePieceParams[2] = tGen.curPiece.r;
            _movePieceParams[3] = tGen.curPiece.c;
            tGen.ShowConfirmMove(_movePieceParams);
            SetSelected(true);
            if (tGen.initialTurn) {
                tGen.ConfirmMove();
            }
        }
    }
    public void SetSelected(bool selected)
    {
        if(selected) renderer.material.SetColor("_Color", Color.yellow);
        else renderer.material.SetColor("_Color", color);
    }

    public void SetClickable(bool click) 
    {
        isClickable = click;
        SetSelected(isClickable);
    }
}
