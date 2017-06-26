using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using BestHTTP;
using BestHTTP.Caching;
using System.Text;
using PVPSdk.PVP;
using System.Security.Cryptography;



namespace PVPSdk.Http{

	/// <summary>
	/// http 协议是有响应包，可以进行
	/// </summary>
	internal class HttpRequestHandler
	{
	    // http 响应报 ，在哪个级别返回数据
		// 网络返回消息类型
	    public enum NetworkMsgType {
	        network = 1,
	        server,
	        protocol,
	    };


		private static System.Random _r = new System.Random();

	    AbstractHttpProtocol protoBuffer;
	    Callback<NetworkMsgType, string, AbstractHttpProtocol> onFinish;
//		int time_out = PVP.Config.protocol_request_timeout;
//		int connect_time_out = PVP.Config.connect_timeout;

		internal void PostRequest ( AbstractHttpProtocol proto, Callback<NetworkMsgType, string, AbstractHttpProtocol> onFinish, byte[] secret, int connectTimeout, int timeout ) {
	        this.protoBuffer = proto;
			this.onFinish = onFinish;

	        Uri uri = new Uri (proto.GetRequestUrl());
	        HTTPRequest request = new HTTPRequest (uri, HTTPMethods.Post, true, true, OnDownloaded);
			request.ConnectTimeout = TimeSpan.FromSeconds (connectTimeout);
			request.Timeout = TimeSpan.FromSeconds (timeout);
//            request.AddField ("token", proto.token);
//            request.AddField ("req", BitConverter.ToString proto.postBody.ToString());
            ///有一个验证码
//            byte[] rawData = new byte[proto.protobufBytes.Length + proto.token.Length + 14];
            //version_id  request_id  protobuf_length protobuf token checkSum
//            Array.Copy(rawData, 0, SocketClient.Int2ByteArray( PVPGlobal.VERSION ).CheckBigEndian());
//            Array.Copy(rawData, 4, SocketClient.UInt162ByteArray( proto.requestId ).CheckBigEndian());
//            Array.Copy (rawData, 6, SocketClient.Int2ByteArray (proto.protobufBytes.Length).CheckBigEndian ());
//            Array.Copy (rawData, 10, proto.protobufBytes);
////            Array.Copy ();
//            request.RawData = rawData;
            request.AddField ("appKey", Config.appKey);
			string postStr = Convert.ToBase64String(proto.protobufBytes, Base64FormattingOptions.None);
			request.AddField ("protoStr", postStr);
            request.AddField ("requestId", proto.requestId.ToString());
			request.AddField ("version", Config.PROTO_VERSION.ToString());
            request.AddField ("token", proto.token);
			string once = Toolkit.GetUnixTime().ToString() + _r.Next(0, 1000000 ).ToString();
			request.AddField ("once", once);
			request.DisableCache = true;
			request.DisableRetry = false;
			StringBuilder sb = new StringBuilder();
			sb.Append("appKey=");
			sb.Append(Config.appKey);
			sb.Append("&once=");
			sb.Append(once);
			sb.Append("&protoStr=");
			sb.Append(postStr);
			sb.Append("&requestId=");
			sb.Append(proto.requestId.ToString());
			sb.Append("&token");
			sb.Append(proto.token);
			sb.Append("&version=");
			sb.Append(Config.PROTO_VERSION.ToString());
			sb.Append(Encoding.ASCII.GetString(secret));
//			Debug.Log(sb.ToString());
			using( MD5 md5Hash = MD5.Create () ){
				byte[] data = md5Hash.ComputeHash (Encoding.ASCII.GetBytes(sb.ToString()));

				sb.Remove(0, sb.Length);
				foreach (var b in data) 
				{ 
					sb.Append(b.ToString("x2").ToLower()); 
				}

				request.AddField ("checkSum", sb.ToString());
			}
//			request.ConnectTimeout = new TimeSpan( 10 );
//			request.Timeout = new TimeSpan(10);
	        HTTPManager.SendRequest(request);
	    }

	    private void OnDownloaded(HTTPRequest req, HTTPResponse resp) {
            //默认 网络层异常

            NetworkMsgType error;
            if (req != null && (req.State == HTTPRequestStates.Aborted || req.State == HTTPRequestStates.ConnectionTimedOut || req.State == HTTPRequestStates.TimedOut)) {
                error = NetworkMsgType.network;
            } else {
                error = NetworkMsgType.server;
            }
	        string message = "未知错误";
	        if (resp != null) {
	            if (resp.IsSuccess) {
	                byte[] data = resp.Data;
	                protoBuffer.SetResMsg (data, data.Length);
	                error = NetworkMsgType.protocol;
	                message = "请求成功";
	            } else {
                    if (resp.StatusCode == 500 ||
	                    resp.StatusCode == 501 ||
	                    resp.StatusCode == 503 ||
	                    resp.StatusCode == 403) {
	                    message = resp.DataAsText;
	                    error = NetworkMsgType.server;
	                }
	            }
	        }
	        onFinish (error, message, protoBuffer);
	    }
	}

}
