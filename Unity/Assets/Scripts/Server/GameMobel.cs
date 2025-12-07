using System; 
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class LoginRequest
{
    public string useremail;
    public string userpassword;
}

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
public class LoginResponse
{
    public bool success;
    public string message;
    //public string accessToken;
    //public string refreshToken;
    public UserData user;
}

[System.Serializable]
public class MembershipRequest
{
    public string useremail;
    public string userpassword;
    public string username;
}

[System.Serializable]
public class MembershipResponse
{
    public bool success;
    public string message;
    public int userId;
}