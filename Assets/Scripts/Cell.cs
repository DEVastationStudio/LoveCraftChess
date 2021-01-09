using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Cell : MonoBehaviourPunCallbacks
{

    [SerializeField] private GameObject obstacle;
    [SerializeField] private Renderer renderer;
    [SerializeField] private Color _glowColor;
    [SerializeField] private Material OneRevive;
    [SerializeField] private Material TwoRevive;
    [SerializeField] private Material ThreeRevive;
    [SerializeField] private Material[] tableTextures;
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private Gradient _freeCellGradient, _occupiedCellGradient;

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
                renderer.material = tableTextures[0];
                break;
            case TableObj.pieceType.P2ZONE:
                renderer.material = tableTextures[1];
                break;
            case TableObj.pieceType.P1KEY:
                renderer.material = tableTextures[2];
                break;
            case TableObj.pieceType.P2KEY:
                renderer.material = tableTextures[3];
                break;
            case TableObj.pieceType.P1REVIVE1:
            case TableObj.pieceType.P2REVIVE1:
                renderer.material = tableTextures[4];
                break;
            case TableObj.pieceType.P1REVIVE2:
            case TableObj.pieceType.P2REVIVE2:
                renderer.material = tableTextures[5];
                break;
            case TableObj.pieceType.P1REVIVE3:
            case TableObj.pieceType.P2REVIVE3:
                renderer.material = tableTextures[6];
                break;
            case TableObj.pieceType.P11JAIL:
            case TableObj.pieceType.P21JAIL:
                renderer.material = tableTextures[7];
                break;
            case TableObj.pieceType.P12JAIL:
            case TableObj.pieceType.P22JAIL:
                renderer.material = tableTextures[8];
                break;
            case TableObj.pieceType.P13JAIL:
            case TableObj.pieceType.P23JAIL:
                renderer.material = tableTextures[9];
                break;
            case TableObj.pieceType.BASIC:
                renderer.material = tableTextures[10];
                break;
            case TableObj.pieceType.BASIC1:
                renderer.material = tableTextures[11];
                break;
            case TableObj.pieceType.BASIC2:
                renderer.material = tableTextures[12];
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
        ParticleSystem.MainModule main = _particleSystem.main;
        main.startColor = (p!=null)?_occupiedCellGradient:_freeCellGradient;
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
        if(selected) renderer.material.SetColor("_Color", _glowColor);
        else renderer.material.SetColor("_Color", color);

        if (selected) _particleSystem.Play();
        else _particleSystem.Stop();
    }

    public void SetClickable(bool click) 
    {
        isClickable = click;
        SetSelected(isClickable);
    }
}
