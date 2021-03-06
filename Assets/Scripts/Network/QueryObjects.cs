﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

//Z tego co ludzie pisali w internecie, nie można serializować tablic za pomocą JsonUtility, więc trzeba uważać.

[Serializable]
public abstract class Q_OBJECT
{
    public abstract void executeQuery(QueuePack queuePack);
    public static Q_OBJECT Deserialize(string json, string type)
    {
        if (type == "Q_SERVER_INFO_REQUEST"){ return JsonUtility.FromJson<Q_SERVER_INFO_REQUEST>(json); }
        if (type == "Q_SERVER_INFO"){ return JsonUtility.FromJson<Q_SERVER_INFO>(json); }
        if (type == "Q_HELLO") { return JsonUtility.FromJson<Q_HELLO>(json); }
        if (type == "Q_JOIN_REQUEST") { return JsonUtility.FromJson<Q_JOIN_REQUEST>(json); }
        if (type == "Q_JOIN_OK") { return JsonUtility.FromJson<Q_JOIN_OK>(json); }
        if (type == "Q_IM_ALIVE") { return JsonUtility.FromJson<Q_IM_ALIVE>(json); }
        if (type == "Q_IM_ALIVE_RESPONSE") { return JsonUtility.FromJson<Q_IM_ALIVE_RESPONSE>(json); }
        if (type == "Q_SHOOT_PHOTON") { return JsonUtility.FromJson<Q_SHOOT_PHOTON>(json); }
        if (type == "Q_SPAWN_BLACKHOLE") { return JsonUtility.FromJson<Q_SPAWN_BLACKHOLE>(json); }
        if (type == "Q_SET_PLAYER_AVATAR") { return JsonUtility.FromJson<Q_SET_PLAYER_AVATAR>(json); }
        if (type == "Q_SET_CANNON_POSITION") { return JsonUtility.FromJson<Q_SET_CANNON_POSITION>(json); }
        if (type == "Q_RUN_ROUND") { return JsonUtility.FromJson<Q_RUN_ROUND>(json); }
        if (type == "Q_SEND_SCORE") { return JsonUtility.FromJson<Q_SEND_SCORE>(json); }
        
        //na wypadek błędu
        Debug.Log("Q_OBJECT ERROR, Nieznany typ "+type.ToString());
        Debug.Log("Zapomniałeś dopisać tą linię kodu w Q_OBJECT");
        throw new Exception("Q_OBJECT ERROR, Nieznany typ "+type.ToString());
    }
}
[Serializable]
public class Q_SERVER_INFO_REQUEST : Q_OBJECT   //obiekt oznaczający, że ktoś chce się dowiedzieć coś o serwerze
{
    public override void executeQuery(QueuePack queuePack)
    {
        Debug.Log("Q_SERVER_INFO_REQUEST execute.");        
        if (NetworkManager.instance.getNetworkState() == NetworkState.NET_SERVER)
        {
            Q_SERVER_INFO info = new Q_SERVER_INFO();
            info.numberOfPlayers = 0;
            info.serverName = "test komunikatu";
            NetworkManager.instance.sendToComputer(info, queuePack.endpoint);
        }
    }
}
[Serializable]
public class Q_SERVER_INFO : Q_OBJECT   //obiekt zawierający dane o serwerze
{
    public string serverName;
    public int numberOfPlayers;
    public override void executeQuery(QueuePack queuePack)
    {
        Debug.Log("Q_SERVER_INFO: "+ serverName+"\t"+ numberOfPlayers);
        GameObject gameObject = GameObject.Find("MenuScriptObject");
        MenuScript test = gameObject.GetComponent<MenuScript>();
        if (test != null)
            test.ip = queuePack.endpoint;
    }
}
/*[Serializable]
public class Q_SERVER_INFO : Q_OBJECT   //obiekt zawierający dane o serwerze
{
    public string serverName;
    public int numberOfPlayers;
    public override void executeQuery(QueuePack queuePack)
    {
        Debug.Log("Q_SERVER_INFO: " + serverName + "\t" + numberOfPlayers);
        GameObject gameObject = GameObject.Find("Tester");
        Test test = gameObject.GetComponent<Test>();
        if (test != null)
            test.ip = queuePack.endpoint;
    }
}*/
[Serializable]
public class Q_HELLO : Q_OBJECT   //obiekt do testowania
{
    public string text;
    public override void executeQuery(QueuePack queuePack)
    {
        Debug.Log("HELLO "+text);
    }
}
[Serializable]
public class Q_JOIN_REQUEST : Q_OBJECT   //obiekt oznaczający chęć dołączenia do gry
{
    public override void executeQuery(QueuePack queuePack)
    {
        Debug.Log("Q_JOIN_REQUEST execute.");
        if (NetworkManager.instance.getNetworkState() == NetworkState.NET_SERVER)
        {
            if (NetworkManager.instance.isKnownComputer(queuePack.endpoint))//może dołączyć nawet w trakcie gry, jeżeli na chwilę go wywali
            {
                NetworkManager.instance.addComputer(queuePack.endpoint);
                NetworkManager.instance.sendToComputer(new Q_JOIN_OK(), queuePack.endpoint);
            }
            if (true /*jakis warunek typu, jak gra jest w toku false*/) {
                NetworkManager.instance.addComputer(queuePack.endpoint);
                NetworkManager.instance.sendToComputer(new Q_JOIN_OK(), queuePack.endpoint);
                Debug.Log("Q_JOIN_REQUEST done.");
            } else
            {
                Debug.Log("Q_JOIN_REQUEST fail.");
            }  
        }
    }
}
[Serializable]
public class Q_JOIN_OK : Q_OBJECT   //obiekt oznaczający fakt dołączenia do gry
{
    public override void executeQuery(QueuePack queuePack)
    {
        Debug.Log("Q_JOIN_OK execute.");
        if (NetworkManager.instance.getNetworkState() == NetworkState.NET_ENABLED)
        {
            IPEndPoint tmp = NetworkManager.instance.getJoinIp();
            bool res = IPEndPoint.Equals(tmp, queuePack.endpoint);
            if (res)
            {
                IPEndPoint ip = queuePack.endpoint;
                NetworkManager.instance.acceptJoin(ip);
                Debug.Log("Q_JOIN_OK done.");
            }
        }
    }
}
[Serializable]
public class Q_IM_ALIVE : Q_OBJECT   //obiekt oznaczający że komputer nie umarł
{
    public override void executeQuery(QueuePack queuePack)
    {
        Debug.Log("Q_IM_ALIVE done.");
        NetworkManager.instance.setComputerTimeZero(queuePack.endpoint);
        NetworkManager.instance.sendToComputer(new Q_IM_ALIVE_RESPONSE(), queuePack.endpoint);
    }
}
[Serializable]
public class Q_IM_ALIVE_RESPONSE : Q_OBJECT   //obiekt oznaczający że komputer nie umarł
{
    public override void executeQuery(QueuePack queuePack)
    {
        Debug.Log("Q_IM_ALIVE_RESPONSE done.");
        NetworkManager.instance.setServerTimeZero();
    }
}
[Serializable]
public class Q_SHOOT_PHOTON : Q_OBJECT   //obiekt oznaczający że komputer nie umarł
{
    public Vector3 position;
    public Vector3 direction;
    public override void executeQuery(QueuePack queuePack)
    {
        Debug.Log("Q_SHOOT_PHOTON done.");
        PhotonEmitter.emitPhoton(position, direction);
    }
}
[Serializable]
public class Q_SPAWN_BLACKHOLE : Q_OBJECT   //obiekt oznaczający że komputer nie umarł
{
    public Vector3 position;
    public float radius;
    public override void executeQuery(QueuePack queuePack)
    {
        Debug.Log("Q_SPAWN_BLACKHOLE done.");
        GameManager.instance.placeBlackHole(position,radius);
    }
}
[Serializable]
public class Q_SET_PLAYER_AVATAR : Q_OBJECT   //obiekt oznaczający że komputer nie umarł
{
    public Vector3 position;
    public Quaternion qa;
    public int type;
    public override void executeQuery(QueuePack queuePack)
    {
        try
        {
            if (type == 1)
            {
                GameManager.instance.pos1 = position;
                GameManager.instance.qa1 = qa;
            }
            if (type == 2)
            {
                GameManager.instance.pos2 = position;
                GameManager.instance.qa2 = qa;
            }
        } catch
        {

        }
    }
}
[Serializable]
public class Q_SET_CANNON_POSITION : Q_OBJECT   //obiekt oznaczający że komputer nie umarł
{
    public Vector3 position;
    public Quaternion qa;
    public override void executeQuery(QueuePack queuePack)
    {
        try
        {

            GameManager.instance.setCannonPosition(position, qa);
            
        }
        catch
        {

        }
    }
}
[Serializable]
public class Q_RUN_ROUND : Q_OBJECT   //obiekt oznaczający że komputer nie umarł
{
    public override void executeQuery(QueuePack queuePack)
    {
        try
        {
            GameManager.instance.startCannonRemote();
        }
        catch
        {

        }
    }
}
[Serializable]
public class Q_SEND_SCORE : Q_OBJECT   //obiekt oznaczający że komputer nie umarł
{
    public int a;
    public int b;
    public override void executeQuery(QueuePack queuePack)
    {
        try
        {
            GameManager.instance.setScore(a,b);
        }
        catch
        {

        }
    }
}