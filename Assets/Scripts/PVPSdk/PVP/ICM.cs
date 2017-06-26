using System;
using UnityEngine;

namespace PVPSdk.PVP
{
    internal class ICM
    {
        private static GameServerClient _gameServerClient = null;
        internal static GameServerClient gameServerClient{
            get{
                if (ICM._gameServerClient == null) {
                    GameObject go = new GameObject ();
                    ICM._gameServerClient = go.AddComponent<GameServerClient> ();
                    go.name = "Socket client";
                }
                return ICM._gameServerClient;
            }
        }
        private static HandlerRegister _handlerRegister = null;

        internal static HandlerRegister handlerRegister {
            get {
                if (_handlerRegister == null) {
                    _handlerRegister = new HandlerRegister ();
                }
                return _handlerRegister;
            }
        }
    }
}

