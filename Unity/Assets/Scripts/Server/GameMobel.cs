using System;
using UnityEngine;

[System.Serializable]
public class UserData
{
    public int playerId;
    public string playerName;
    public int playerLevel;
    public int currentMoney;
    public bool isDead;
    public string lastLoginTime;
}

[System.Serializable]
public class AuthRequest
{
    public string useremail;
    public string userpassword;
}

[System.Serializable]
public class MembershipRequest
{
    public string useremail;
    public string userpassword;
    public string username;
}

[System.Serializable]
public class LoginResponse
{
    public bool success;
    public string message;
    public UserData user;
}

[System.Serializable]
public class MembershipResponse
{
    public bool success;
    public string message;
    public int userId;
}

[System.Serializable]
public class NpcKillRequest
{
    public string useremail;
    public int npcTypeId;
}

[System.Serializable]
public class NpcKillResponse
{
    public bool success;
    public string message;
    public int rewardAmount;
    public int newMoney;
}

[System.Serializable]
public class PlayerInfoResponse
{
    public bool success;
    public UserData user;
    public string message;
}