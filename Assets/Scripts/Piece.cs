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

    [SerializeField] private GameObject[] pawn;
    [SerializeField] private GameObject[] rook;
    [SerializeField] private GameObject[] knight;
    [SerializeField] private GameObject[] bishop;
    [SerializeField] private GameObject[] queen;
    [SerializeField] private GameObject _pieceContainer;
    [SerializeField] private Collider _collider;
    private bool _isSelected;
    private float _moveTimer;

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
        color = (player==1)?Color.blue:Color.red;
        ChangeModel(player);
    }

    private void ChangeModel(int player)
    {
        switch (type) 
        {
            case pieces.PAWN:
                pawn[player-1].SetActive(true);
                break;
            case pieces.ROOK:
                rook[player-1].SetActive(true);
                break;
            case pieces.KNIGHT:
                knight[player-1].SetActive(true);
                break;
            case pieces.BISHOP:
                bishop[player-1].SetActive(true);
                break;
            case pieces.QUEEN:
                queen[player-1].SetActive(true);
                break;
        }
    }

    void OnMouseDown()
    {
        if (tGen.performingMove || inJail) return;
        tGen.SelectPiece(r, c);
    }

    public void SetChosen(bool b) 
    {
        _isSelected = b;
        _moveTimer = 0;
        _pieceContainer.transform.localPosition = Vector3.zero;
        _pieceContainer.transform.localRotation = Quaternion.identity;
    }

    public GameObject SetChosenMovement(bool b)
    {
        _isSelected = b;
        _moveTimer = 0;
        return _pieceContainer;
    }

    public void SetPosition(int r, int c) 
    {
        SetCoords(r,c);
        transform.position = new Vector3(c, 1, r);
    }
    public void SetCoords(int r, int c)
    {
        this.r = r;
        this.c = c;
    }
    public void SetJailPosition(Cell c) 
    {
        SetCoords(-1,-1);
        transform.position = new Vector3(c.transform.position.x, 1, c.transform.position.z);
    }

    void Update()
    {
        _collider.enabled = (TableGenerator.curPlayer == player);
        if (!_isSelected) return;

        _pieceContainer.transform.localPosition = new Vector3(0, 1+Mathf.Sin(_moveTimer), 0);
        _pieceContainer.transform.localEulerAngles = new Vector3(0, Mathf.Rad2Deg*_moveTimer, 0);

        _moveTimer = (_moveTimer+Time.deltaTime)%(2*Mathf.PI);
    }
}
