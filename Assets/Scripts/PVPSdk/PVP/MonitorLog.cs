
using System;
using BestHTTP;
using System.Text;
using UnityEngine;

namespace PVPSdk.PVP
{
	internal class MonitorLog
	{
		internal enum State{
			None = 0,
			Submiting = 1,
		}

		private static string MONITOR_LOG_FILE_NAME = "monitorLog";
		private static string MONITOR_LOG_FILE_NAME_TMP = "monitorLogTmp";

		private static MonitorLog _instance = null;
		internal static MonitorLog GetInstance(){
			if(_instance == null){
				_instance = new MonitorLog();
			}
			return _instance;
		}

		private MonitorLog(){
			this._state = State.None;
		}

		private State _state = State.None;
//		private StringBuilder _sb = new StringBuilder();

		internal void AddLog(string log){
//			_sb.Append(string.Format("{0}, {1} |", Toolkit.GetTimeStamp(), log));
			Toolkit.AppendFile(MONITOR_LOG_FILE_NAME, string.Format("{0}, {1} |", Toolkit.GetTimeStamp(), log));
		}

		internal void _OnSubmited(HTTPRequest req, HTTPResponse resp) {
			string log = Toolkit.LoadFile(MONITOR_LOG_FILE_NAME_TMP);
			//默认 网络层异常
			if (resp != null && resp.IsSuccess) {
				Toolkit.SaveFile(MONITOR_LOG_FILE_NAME, log);
			}else{
				Toolkit.AppendFile(MONITOR_LOG_FILE_NAME, log);
			}
			Toolkit.DeleteFile(MONITOR_LOG_FILE_NAME_TMP);
			this._state = State.None;
		}

		internal void Submit(){
			Debug.Log(this._state.ToString());
			if(this._state == State.Submiting){
				return;
			}
			string log = Toolkit.LoadFile(MONITOR_LOG_FILE_NAME);

			if(log.Length == 0){
				return;
			}
			this._state = State.Submiting;
			Toolkit.DeleteFile(MONITOR_LOG_FILE_NAME_TMP);
			Debug.Log( Config.HttpUri + Config.ReceiveUri);
			HTTPRequest request = new HTTPRequest(new Uri( Config.HttpUri + Config.ReceiveUri), HTTPMethods.Post, true, true, _OnSubmited);
			request.ConnectTimeout = TimeSpan.FromSeconds (5);
			request.Timeout = TimeSpan.FromSeconds (10);

			request.RawData = System.Text.Encoding.ASCII.GetBytes(string.Format("{0}|{1}|{2}", Config.appKey.Substring(0,6), ( PVPGlobal.player != null ) ? PVPGlobal.player.playerId : 0,  log));
			HTTPManager.SendRequest(request);
		}
	}
}

