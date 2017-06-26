using System;
using System.Collections.Generic;

/// <summary>
/// 消息类型
/// </summary>
public class MessageEventType
{
    public const string OnChangePlaylistToggle = "OnChangePlaylistToggle";
    
    public const string OnEnterScene = "OnEnterScene";//进入场景后第一个调用

    public const string GAME_BEGIN = "GAME_BEGIN";
    public const string GAME_OVER = "GAME_OVER";
    public const string BATTLE_SCENE_INIT_OK = "BATTLE_SCENE_INIT_OK";
    public const string PlayerSelect = "PlayerSelect";
    public const string PlayerSwerve = "PlayerSwerve";
    public const string PlayerAccelerate = "PlayerAccelerate";
    public const string CancelAccelerate = "CancelAccelerate";

    public const string EntityCollide = "EntityCollide";


    public const string OnBuffDestroy = "OnBuffDestroy";

#region photon use
    public const string OnPlayerNumChange = "OnPlayerNumChange";//玩家数量改变
    public const string OnPlayerCreated = "OnPlayerCreated";//玩家数量改变
    public const string OnPlayerDestroyed = "OnPlayerDestroyed";//玩家数量改变

    public const string ConnectInfo = "ConnectInfo";
    public const string OnFailedToConnectToPhoton = "OnFailedToConnectToPhoton";
    public const string OnCreatedRoom = "OnCreatedRoom";
    public const string OnJoinedRoom = "OnJoinedRoom";
    public const string OnPhotonCreateRoomFailed = "OnPhotonCreateRoomFailed";
    public const string OnPhotonJoinRoomFailed = "OnPhotonJoinRoomFailed";
    public const string OnPhotonRandomJoinFailed = "OnPhotonRandomJoinFailed";
    public const string OnDisconnectedFromPhoton = "OnDisconnectedFromPhoton";
    public const string OnMasterClientSwitched = "OnMasterClientSwitched";

    public const string OnLeftRoom = "OnLeftRoom";
    public const string OnPhotonInstantiate = "OnPhotonInstantiate";
    public const string OnPhotonPlayerConnected = "OnPhotonPlayerConnected";
    public const string OnPhotonPlayerDisconnected = "OnPhotonPlayerDisconnected";
#endregion
  
    
}

public class BroadcastException : Exception
{
    public BroadcastException(string msg) : base(msg)
    {
    }
}

public class ListenerException : Exception
{
    public ListenerException(string msg) : base(msg)
    {
    }
}

//=====================================================================================/
/// <summary>
/// weding
/// 消息类 全局类消息需要自已处理删除添加 ,全部使用这个
/// </summary>
//=====================================================================================.
public class Messenger : NormalSingleton<Messenger>
{
    public delegate void Callback();

    public delegate void Callback<T>(T arg1);

    public delegate void Callback<T, U>(T arg1, U arg2);

    public delegate void Callback<T, U, V>(T arg1, U arg2, V arg3);

    public Dictionary<string, Delegate> m_eventDictionary = new Dictionary<string, Delegate>();

    ~Messenger()
    {
        Cleanup();
    }

    #region AddEventListener

    //委托字典的方式派发
    public void AddEventListener(string eventType, Callback handler)
    {
        OnListenerAdding(eventType, handler);
        m_eventDictionary[eventType] = (Callback)m_eventDictionary[eventType] + handler;//+handler来添加派发消息时接收的对象
    }

    //一个参数 parameter
    public void AddEventListener<T>(string eventType, Callback<T> handler)
    {
        OnListenerAdding(eventType, handler);
        m_eventDictionary[eventType] = (Callback<T>)m_eventDictionary[eventType] + handler;
    }

    //两个参数 parameter
    public void AddEventListener<T, U>(string eventType, Callback<T, U> handler)
    {
        OnListenerAdding(eventType, handler);
        m_eventDictionary[eventType] = (Callback<T, U>)m_eventDictionary[eventType] + handler;
    }

    //三个参数 parameter
    public void AddEventListener<T, U, V>(string eventType, Callback<T, U, V> handler)
    {
        OnListenerAdding(eventType, handler);
        m_eventDictionary[eventType] = (Callback<T, U, V>)m_eventDictionary[eventType] + handler;
    }

    #endregion AddEventListener

    #region RemoveEventListener

    public void RemoveEventListener(string eventType, Callback handler)
    {
        OnListenerRemoving(eventType, handler);
        m_eventDictionary[eventType] = (Callback)m_eventDictionary[eventType] - handler;
        OnListenerRemoved(eventType);
    }

    public void RemoveEventListener<T>(string eventType, Callback<T> handler)
    {
        OnListenerRemoving(eventType, handler);
        m_eventDictionary[eventType] = (Callback<T>)m_eventDictionary[eventType] - handler;
        OnListenerRemoved(eventType);
    }

    public void RemoveEventListener<T, U>(string eventType, Callback<T, U> handler)
    {
        OnListenerRemoving(eventType, handler);
        m_eventDictionary[eventType] = (Callback<T, U>)m_eventDictionary[eventType] - handler;
        OnListenerRemoved(eventType);
    }

    public void RemoveEventListener<T, U, V>(string eventType, Callback<T, U, V> handler)
    {
        OnListenerRemoving(eventType, handler);
        m_eventDictionary[eventType] = (Callback<T, U, V>)m_eventDictionary[eventType] - handler;
        OnListenerRemoved(eventType);
    }

    #endregion RemoveEventListener

    #region OnListenerAdding OnListenerRemoving

    private void OnListenerAdding(string eventType, Delegate listenerBeingAdded)
    {
        if (!m_eventDictionary.ContainsKey(eventType))
        {
            m_eventDictionary.Add(eventType, null);
        }

        Delegate d = m_eventDictionary[eventType];

        if (d != null && d.GetType() != listenerBeingAdded.GetType())
        {
            throw new ListenerException(string.Format("Attempting to add listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being added has type {2}", eventType, d.GetType().Name, listenerBeingAdded.GetType().Name));
        }
    }

    private void OnListenerRemoving(string eventType, Delegate listenerBeingRemoved)
    {
        if (m_eventDictionary.ContainsKey(eventType))
        {
            Delegate d = m_eventDictionary[eventType];

            if (d == null)
            {
                throw new ListenerException(string.Format("Attempting to remove listener with for event type \"{0}\" but current listener is null.", eventType));
            }
            else if (d.GetType() != listenerBeingRemoved.GetType())
            {
                throw new ListenerException(string.Format("Attempting to remove listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being removed has type {2}", eventType, d.GetType().Name, listenerBeingRemoved.GetType().Name));
            }
        }
        else
        {
            throw new ListenerException(string.Format("Attempting to remove listener for type \"{0}\" but Messenger doesn't know about this event type.", eventType));
        }
    }

    private void OnListenerRemoved(string eventType)
    {
        if (m_eventDictionary[eventType] == null)
        {
            m_eventDictionary.Remove(eventType);
        }
    }

    #endregion OnListenerAdding OnListenerRemoving

    #region BroadCastEventMsg

    public void BroadCastEventMsg(string eventType)
    {
        Delegate d;
        if (m_eventDictionary.TryGetValue(eventType, out d))
        {
            Callback callback = d as Callback;

            if (callback != null)
            {
                callback();
            }
            else
            {
                throw CreateBroadcastSignatureException(eventType);
            }
        }
    }

    public void BroadCastEventMsg<T>(string eventType, T arg1)
    {
        Delegate d;
        if (m_eventDictionary.TryGetValue(eventType, out d))
        {
            Callback<T> callback = d as Callback<T>;

            if (callback != null)
            {
                callback(arg1);
            }
            else
            {
                throw CreateBroadcastSignatureException(eventType);
            }
        }
    }

    public void BroadCastEventMsg<T, U>(string eventType, T arg1, U arg2)
    {
        Delegate d;
        if (m_eventDictionary.TryGetValue(eventType, out d))
        {
            Callback<T, U> callback = d as Callback<T, U>;

            if (callback != null)
            {
                callback(arg1, arg2);
            }
            else
            {
                throw CreateBroadcastSignatureException(eventType);
            }
        }
    }

    public void BroadCastEventMsg<T, U, V>(string eventType, T arg1, U arg2, V arg3)
    {
        Delegate d;
        if (m_eventDictionary.TryGetValue(eventType, out d))
        {
            Callback<T, U, V> callback = d as Callback<T, U, V>;

            if (callback != null)
            {
                callback(arg1, arg2, arg3);
            }
            else
            {
                throw CreateBroadcastSignatureException(eventType);
            }
        }
    }

    #endregion BroadCastEventMsg

    #region CheckEventListener

    public bool CheckEventListener(string eventType, Callback handler)
    {
        if (m_eventDictionary.ContainsKey(eventType))
        {
            Delegate d = m_eventDictionary[eventType];

            if (d == null)
            {
                return false;
            }
            else if (d.GetType() != handler.GetType())
            {
                return false;
            }

            return true;
        }

        return false;
    }

    //Single parameter
    public bool CheckEventListener<T>(string eventType, Callback<T> handler)
    {
        if (m_eventDictionary.ContainsKey(eventType))
        {
            Delegate d = m_eventDictionary[eventType];

            if (d == null)
            {
                return false;
            }
            else if (d.GetType() != handler.GetType())
            {
                return false;
            }

            return true;
        }

        return false;
    }

    //Two parameters
    public bool CheckEventListener<T, U>(string eventType, Callback<T, U> handler)
    {
        if (m_eventDictionary.ContainsKey(eventType))
        {
            Delegate d = m_eventDictionary[eventType];

            if (d == null)
            {
                return false;
            }
            else if (d.GetType() != handler.GetType())
            {
                return false;
            }

            return true;
        }

        return false;
    }

    //Three parameters
    public bool CheckEventListener<T, U, V>(string eventType, Callback<T, U, V> handler)
    {
        if (m_eventDictionary.ContainsKey(eventType))
        {
            Delegate d = m_eventDictionary[eventType];

            if (d == null)
            {
                return false;
            }
            else if (d.GetType() != handler.GetType())
            {
                return false;
            }

            return true;
        }

        return false;
    }

    public BroadcastException CreateBroadcastSignatureException(string eventType)
    {
        return new BroadcastException(string.Format("Broadcasting message \"{0}\" but listeners have a different signature than the broadcaster.", eventType));
    }

    #endregion CheckEventListener

    public void Cleanup()
    {
        m_eventDictionary.Clear();
    }

  
}