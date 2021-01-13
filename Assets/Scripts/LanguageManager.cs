using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageManager : MonoBehaviour
{
    public static bool isSpanish;
    [SerializeField] private Image _image;
    [SerializeField] private SpriteState _button;
    [SerializeField] private Sprite _english;
    [SerializeField] private Sprite _spanish;
    [SerializeField] private Sprite _englishOn;
    [SerializeField] private Sprite _spanishOn;
    private bool _isSpanish;

    void Update()
    {
        if (_isSpanish == isSpanish) return;
        _isSpanish = isSpanish;
        if (_image == null) return;

        if (isSpanish)
        {
            _image.sprite = _spanish;
        }
        else
        {
            _image.sprite = _english;
        }
        if (!_button.Equals(null))
        {
            if (isSpanish)
            {
                _button.highlightedSprite = _spanishOn;
            }
            else
            {
                _button.highlightedSprite = _englishOn;
            }
        }
    }

    #region translated strings
    public const string roomCode_en = "Room code: ";
    public const string roomCode_es = "Código de la sala: ";
    public static string RoomCode() {return (isSpanish?roomCode_es:roomCode_en);}
    public const string connectServerSuccess_en = "Connecting to the server";
    public const string connectServerSuccess_es = "Conectándose al servidor";
    public const string connectServerFailure_en = "Error while connecting to the server";
    public const string connectServerFailure_es = "Error al conectarse al servidor";
    public static string ConnectServer(bool success) {return ((success)?(isSpanish?connectServerSuccess_es:connectServerSuccess_en):(isSpanish?connectServerFailure_es:connectServerFailure_en));}
    public const string connectedToRegion_1_en = "Connected to ";
    public const string connectedToRegion_2_en = " server";
    public const string connectedToRegion_1_es = "Conectado al servidor ";
    public static string ConnectedToRegion(string region) {return (isSpanish?connectedToRegion_1_es+region:connectedToRegion_1_en+region+connectedToRegion_2_en);}
    public const string connecting_en = "Connecting...";
    public const string connecting_es = "Conectando...";
    public static string Connecting() {return (isSpanish?connecting_es:connecting_en);}
    public const string joinRoomFail_en = "Failed to join room.";
    public const string joinRoomFail_es = "Error al unirse a la sala.";
    public static string JoinRoomFail() {return (isSpanish?joinRoomFail_es:joinRoomFail_en);}
    public const string enterRoomName_en = "Please enter a full code.";
    public const string enterRoomName_es = "Introduce un código completo.";
    public static string EnterRoomName() {return (isSpanish?enterRoomName_es:enterRoomName_en);}
    public const string noNewRooms_en = "There are no rooms. Creating a new room...";
    public const string noNewRooms_es = "No hay ninguna sala. Creando una nueva sala...";
    public static string NoNewRooms() {return (isSpanish?noNewRooms_es:noNewRooms_en);}
    public const string roomCreateSuccess_en = "Room created successfully.";
    public const string roomCreateSuccess_es = "Sala creada con éxito.";
    public static string RoomCreateSuccess() {return (isSpanish?roomCreateSuccess_es:roomCreateSuccess_en);}
    public const string roomCreateFailure_en = "Failed to create room.";
    public const string roomCreateFailure_es = "No se pudo crear la sala.";
    public static string RoomCreateFailure() {return (isSpanish?roomCreateFailure_es:roomCreateFailure_en);}
    public const string joinedRoom_en = "Joined room.";
    public const string joinedRoom_es = "Te uniste a una sala.";
    public static string JoinedRoom() {return (isSpanish?joinedRoom_es:joinedRoom_en);}
    public const string loadingRoom_en = "Loading...\n";
    public const string loadingRoom_es = "Cargando...\n";
    public static string LoadingRoom() {return (isSpanish?loadingRoom_es:loadingRoom_en);}
    
    public const string stats_1_en = "Local Multiplayer Stats:\n";
    public const string stats_1_es = "Estadísticas del Multijugador Local:\n";
    public static string Stats_1() {return (isSpanish?stats_1_es:stats_1_en);}
    public const string stats_2_en = "Deep One victories: ";
    public const string stats_2_es = "Victorias (Profundos): ";
    public static string Stats_2() {return (isSpanish?stats_2_es:stats_2_en);}
    public const string stats_3_en = "Corrupted One victories: ";
    public const string stats_3_es = "Victorias (Corruptos): ";
    public static string Stats_3() {return (isSpanish?stats_3_es:stats_3_en);}
    public const string stats_4_en = "\nOnline Multiplayer Stats:\n";
    public const string stats_4_es = "\nEstadísticas del Multijugador Online:\n";
    public static string Stats_4() {return (isSpanish?stats_4_es:stats_4_en);}
    public const string stats_5_en = "Games played: ";
    public const string stats_5_es = "Partidas jugadas: ";
    public static string Stats_5() {return (isSpanish?stats_5_es:stats_5_en);}
    public const string stats_6_en = "Victories: ";
    public const string stats_6_es = "Victorias: ";
    public static string Stats_6() {return (isSpanish?stats_6_es:stats_6_en);}
    public const string stats_7_en = "Victories (Conqueror): ";
    public const string stats_7_es = "Victorias (Conquistador): ";
    public static string Stats_7() {return (isSpanish?stats_7_es:stats_7_en);}
    public const string stats_8_en = "Victories (Devastator): ";
    public const string stats_8_es = "Victorias (Devastador): ";
    public static string Stats_8() {return (isSpanish?stats_8_es:stats_8_en);}
    public const string stats_9_en = "Victories (Surrender): ";
    public const string stats_9_es = "Victorias (Rendición): ";
    public static string Stats_9() {return (isSpanish?stats_9_es:stats_9_en);}
    public const string stats_10_en = "Defeats: ";
    public const string stats_10_es = "Derrotas: ";
    public static string Stats_10() {return (isSpanish?stats_10_es:stats_10_en);}
    public const string stats_11_en = "Defeats (Conqueror): ";
    public const string stats_11_es = "Derrotas (Conquistador): ";
    public static string Stats_11() {return (isSpanish?stats_11_es:stats_11_en);}
    public const string stats_12_en = "Defeats (Devastator): ";
    public const string stats_12_es = "Derrotas (Devastador): ";
    public static string Stats_12() {return (isSpanish?stats_12_es:stats_12_en);}
    public const string stats_13_en = "Defeats (Surrender): ";
    public const string stats_13_es = "Derrotas (Rendición): ";
    public static string Stats_13() {return (isSpanish?stats_13_es:stats_13_en);}
    public const string stats_14_en = "Eaten pieces: ";
    public const string stats_14_es = "Piezas comidas: ";
    public static string Stats_14() {return (isSpanish?stats_14_es:stats_14_en);}
    public const string stats_15_en = "Resurrections: ";
    public const string stats_15_es = "Resurrecciones: ";
    public static string Stats_15() {return (isSpanish?stats_15_es:stats_15_en);}
    public const string stats_16_en = "Traps triggered: ";
    public const string stats_16_es = "Trampas activadas: ";
    public static string Stats_16() {return (isSpanish?stats_16_es:stats_16_en);}
    public const string stats_17_en = "Pieces destroyed by traps: ";
    public const string stats_17_es = "Piezas destruidas por trampas: ";
    public static string Stats_17() {return (isSpanish?stats_17_es:stats_17_en);}
    public const string stats_18_en = "Turns taken: ";
    public const string stats_18_es = "Turnos totales: ";
    public static string Stats_18() {return (isSpanish?stats_18_es:stats_18_en);}

    #endregion
}
