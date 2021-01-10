using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviourPunCallbacks
{

    [SerializeField] private GameObject obstacle;
    [SerializeField] private Renderer renderer;
    [SerializeField] private Renderer _PitRendererL;
    [SerializeField] private Renderer _PitRendererR;
    [SerializeField] private Color _glowColor;
    [SerializeField] private Material OneRevive;
    [SerializeField] private Material TwoRevive;
    [SerializeField] private Material ThreeRevive;
    [SerializeField] private Material[] tableTextures;
    [SerializeField] private Material[] _p1FloorMaterials;
    [SerializeField] private Material[] _p2FloorMaterials;
    [SerializeField] private Material _barrierFloorMaterial;
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private Gradient _freeCellGradient, _occupiedCellGradient;
    [SerializeField] private Sprite[] _minimapSprites;
    public Image minimapCell;
    public Image minimapPiece;

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
    [SerializeField] private Material barrierButtonTexture;

    public bool isPit;
    public GameObject pit;
    public GameObject pitHingeL, pitHingeR;
    public bool pitOpen;
    public bool barrierDown;
    [SerializeField] private Material pitButtonTexture;

    public void Init(TableObj.pieceType type, int r, int c, TableGenerator tGen)
    {
        minimapCell.sprite = _minimapSprites[0];
        minimapPiece.sprite = _minimapSprites[0];
        color = renderer.material.GetColor("_Color");
        this.tGen = tGen;
        this.type = type;
        transform.eulerAngles = new Vector3(0f, 180f, 0f);
        switch (type) 
        {
            case TableObj.pieceType.OBSTACLE:
                isObstacle = true;
                obstacle.SetActive(isObstacle);
                minimapCell.sprite = _minimapSprites[30];
                break;
            case TableObj.pieceType.P1ZONE:
                renderer.material = tableTextures[0];
                minimapCell.sprite = _minimapSprites[14];
                break;
            case TableObj.pieceType.P2ZONE:
                renderer.material = tableTextures[1];
                minimapCell.sprite = _minimapSprites[18];
                break;
            case TableObj.pieceType.P1KEY:
                renderer.material = tableTextures[2];
                minimapCell.sprite = _minimapSprites[25];
                break;
            case TableObj.pieceType.P2KEY:
                renderer.material = tableTextures[3];
                minimapCell.sprite = _minimapSprites[25];
                break;
            case TableObj.pieceType.P1REVIVE1:
            case TableObj.pieceType.P2REVIVE1:
                renderer.material = tableTextures[4];
                minimapCell.sprite = _minimapSprites[19];
                break;
            case TableObj.pieceType.P1REVIVE2:
            case TableObj.pieceType.P2REVIVE2:
                renderer.material = tableTextures[5];
                minimapCell.sprite = _minimapSprites[20];
                break;
            case TableObj.pieceType.P1REVIVE3:
            case TableObj.pieceType.P2REVIVE3:
                renderer.material = tableTextures[6];
                minimapCell.sprite = _minimapSprites[21];
                break;
            case TableObj.pieceType.P11JAIL:
            case TableObj.pieceType.P21JAIL:
                renderer.material = tableTextures[7];
                minimapCell.sprite = _minimapSprites[22];
                break;
            case TableObj.pieceType.P12JAIL:
            case TableObj.pieceType.P22JAIL:
                renderer.material = tableTextures[8];
                minimapCell.sprite = _minimapSprites[23];
                break;
            case TableObj.pieceType.P13JAIL:
            case TableObj.pieceType.P23JAIL:
                renderer.material = tableTextures[9];
                minimapCell.sprite = _minimapSprites[24];
                break;
            case TableObj.pieceType.BASIC:
                renderer.material = tableTextures[10];
                minimapCell.sprite = _minimapSprites[32-(r+c)%2]; //
                break;
            case TableObj.pieceType.BASIC1:
                renderer.material = _p1FloorMaterials[Random.Range(0,_p1FloorMaterials.Length)];
                minimapCell.sprite = _minimapSprites[12-(r+c)%2]; //
                break;
            case TableObj.pieceType.BASIC2:
                renderer.material = _p2FloorMaterials[Random.Range(0,_p2FloorMaterials.Length)];
                minimapCell.sprite = _minimapSprites[16-(r+c)%2]; //
                break;
            case TableObj.pieceType.BARRIER:
                isBarrier = true;
                renderer.material = _barrierFloorMaterial;
                barrier.SetActive(true);
                minimapCell.sprite = _minimapSprites[29];
                break;
            case TableObj.pieceType.BARRIERBTN:
                renderer.material = barrierButtonTexture;
                minimapCell.sprite = _minimapSprites[28];
                break;
            case TableObj.pieceType.PIT:
                isPit = true;
                renderer.enabled = false;
                pit.SetActive(true);
                minimapCell.sprite = _minimapSprites[27];
                break;
            case TableObj.pieceType.PITBTN:
                renderer.material = pitButtonTexture;
                minimapCell.sprite = _minimapSprites[26];
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
        if (piece == null)
        {
            minimapPiece.sprite = _minimapSprites[0];
        }
        else
        {
            int pieceOffset = (piece.player==2)?5:0;
            switch (piece.type)
            {
                case Piece.pieces.PAWN:
                    minimapPiece.sprite = _minimapSprites[1+pieceOffset];
                break;
                case Piece.pieces.ROOK:
                    minimapPiece.sprite = _minimapSprites[4+pieceOffset];
                break;
                case Piece.pieces.BISHOP:
                    minimapPiece.sprite = _minimapSprites[2+pieceOffset];
                break;
                case Piece.pieces.KNIGHT:
                    minimapPiece.sprite = _minimapSprites[3+pieceOffset];
                break;
                case Piece.pieces.QUEEN:
                    minimapPiece.sprite = _minimapSprites[5+pieceOffset];
                break;
            }
        }
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
        if(selected) 
        {
            renderer.material.SetColor("_Color", _glowColor);
            _PitRendererL.material.SetColor("_Color", _glowColor);
            _PitRendererR.material.SetColor("_Color", _glowColor);
        }
        else 
        {
            renderer.material.SetColor("_Color", color);
            _PitRendererL.material.SetColor("_Color", color);
            _PitRendererR.material.SetColor("_Color", color);
        }

        if (selected) _particleSystem.Play();
        else _particleSystem.Stop();
    }

    public void SetClickable(bool click) 
    {
        isClickable = click;
        SetSelected(isClickable);
    }
}
