using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class Instantanea 
{
    
}
public class MinMax : MonoBehaviour
{

    [SerializeField] private TableGenerator TBinit;
    [SerializeField] private bool DEBUG_MODE = true;

    private float alpha = -Mathf.Infinity;
    private float beta = Mathf.Infinity;
    private Stack<TableGenerator> gameStates = new Stack<TableGenerator>();


    private void CreateTB(TableGenerator TB) 
    {
        gameStates.Push(TB.Clone_TableGenerator(
            TB.cells, TB.p1Start, TB.p2Start, TB.p1Jail, TB.p2Jail, TB.p1Keys, TB.p2Keys,
            TB.p1Revives, TB.p2Revives, TB.p1Pieces, TB.p2Pieces, TB.moves, TB.curPiece,
            TB.curPlayer, TB.cellPos, TB.initialTurn, TB.gameOver));
        //Debug.Log(gameStates.Count());
    }

    public float alphaBeta(int profundidad, int player, float alpha, float beta) 
    {
        gameStates.Peek().curPlayer = player;
        if (profundidad == 0) return heuristica(player);
        if (player == 1) 
        {
            Debug.Log("Entro a player 1");
            for(int i = 0; i< gameStates.Peek().p1Pieces.Count;i++)
            {
                Piece p = gameStates.Peek().p1Pieces[i];
                if (!p.inJail) 
                {
                    if (DEBUG_MODE) Debug.Log(profundidad+" del jugador "+player+" de la pieza "+p.type);
                    gameStates.Peek().curPiece = p;
                    gameStates.Peek().GetMoves(p.type, p.r, p.c);
                    foreach (Vector2 v in gameStates.Peek().moves)
                    {
                        gameStates.Peek().curPiece = p;
                        if (DEBUG_MODE) Debug.Log("Movimiento "+v);
                        CreateTB(gameStates.Peek());
                        if (DEBUG_MODE) Debug.Log(gameStates.Peek().curPiece.inJail);
                        gameStates.Peek().MovePiece((int)v.x, (int)v.y, true);
                        alpha = Math.Max(alpha, alphaBeta(profundidad - 1, 2, alpha, beta));
                        if (DEBUG_MODE) Debug.Log("[MAX] Alpha: " + alpha + " -- Beta: " + beta);
                        if (beta <= alpha) return alpha;
                    }
                }
            }
            return alpha;
        }
        else
        {
            Debug.Log("Entro a player 2");
            for (int i = 0; i < gameStates.Peek().p2Pieces.Count; i++)
            {
                Piece p = gameStates.Peek().p2Pieces[i];
                if (!p.inJail)
                {
                    if (DEBUG_MODE) Debug.Log(profundidad + " del jugador " + player + " de la pieza " + p.type);
                    gameStates.Peek().curPiece = p;
                    gameStates.Peek().GetMoves(p.type, p.r, p.c);
                    foreach (Vector2 v in gameStates.Peek().moves)
                    {
                        gameStates.Peek().curPiece = p;
                        if (DEBUG_MODE) Debug.Log("Movimiento " + v);
                        CreateTB(gameStates.Peek());
                        if(DEBUG_MODE) Debug.Log(gameStates.Peek().curPiece.inJail);
                        gameStates.Peek().MovePiece((int)v.x, (int)v.y, true);
                        beta = Math.Min(beta, alphaBeta(profundidad - 1, 1, alpha, beta));
                        if (DEBUG_MODE) Debug.Log("[MIN] Alpha: " + alpha+" -- Beta: "+beta);
                        if (beta <= alpha) return beta;
                    }
                }
            }
            return beta;
        }
    }

    public void startMinMax() 
    {
        CreateTB(TBinit);
        Debug.Log(alphaBeta(3,1,alpha, beta));
        //colocarPiezas(TBinit);
    }

    private void colocarPiezas(TableGenerator TB)
    {
        foreach (Piece p in TB.p1Pieces) 
        {
            if (!p.inJail)
                p.SetPosition(p.r, p.c, false);
            else
                p.SetJailPosition(TB.p1Jail[p.jailPos], p.jailPos, false);
        }

        foreach (Piece p in TB.p2Pieces)
        {
            if (!p.inJail)
                p.SetPosition(p.r, p.c, false);
            else
                p.SetJailPosition(TB.p2Jail[p.jailPos], p.jailPos, false);
        }
    }

    private int heuristica(int player)
    {
        TableGenerator TB = gameStates.Pop();
        if (player == 1) 
        {
            if (DEBUG_MODE) Debug.Log("Heuristica => "+ (TB.p1Pieces.Count - TB.p2Pieces.Count));
            return TB.p1Pieces.Count - TB.p2Pieces.Count;
        }
        else
        {
            if (DEBUG_MODE) Debug.Log("Heuristica => " + (TB.p2Pieces.Count - TB.p1Pieces.Count));
            return TB.p2Pieces.Count - TB.p1Pieces.Count;
        }
    }
}
