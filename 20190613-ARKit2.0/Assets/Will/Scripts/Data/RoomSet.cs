using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSet : MonoBehaviour {

	public void SetRoom(int playerType)
    {
        WillData.playerType = (PlayerType)Enum.ToObject(typeof(PlayerType),playerType);
    }
}
