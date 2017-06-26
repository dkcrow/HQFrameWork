using System;
//using TcpClient = SocketEx.TcpClient;
using UnityEngine;
using System.IO;
using System.Net;
using PVPProtobuf;
using System.Security.Cryptography;
using System.Threading;
using System.Collections.Generic;
using System.Net.Sockets;

/**
 * 网络需求，当还处在网络链接状态时，状态改变了，那么直接重连
 * 
 */

#if NETFX_CORE || BUILD_FOR_WP8
using System.Threading.Tasks;
using Windows.Networking.Sockets;

using TcpClient = BestHTTP.PlatformSupport.TcpClient.WinRT.TcpClient;

//Disable CD4014: Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
#pragma warning disable 4014
#elif UNITY_WP8 && !UNITY_EDITOR
using TcpClient = BestHTTP.PlatformSupport.TcpClient.WP8.TcpClient;
#else
using TcpClient = BestHTTP.PlatformSupport.TcpClient.General.TcpClient;
#endif

namespace PVPSdk.PVP
{

    /// <summary>
    /// Socket client state.
    /// </summary>
	internal enum SocketClientState
    {
        None = 0,

		waitForConnectThread,

        startConnect,
        /// <summary>
        /// 网络层连接成功，但是没有验证token
        /// </summary>
        Connected,

        /// <summary>
        /// 验证了 token
        /// </summary>
        CheckTokened,

        Closed,
    }

	internal class ReceivedProtoEventArgs : System.EventArgs
    {
        internal UInt16 requestId{ get; set; }

        internal int errorCode{ get; set; }

        internal int messageTypeId{ get; set; }

		internal UInt16 socketId{ get; set;}

        internal byte[] bytes { get; set; }
    }

	internal delegate void ReceivedProtoEventHandler (ReceivedProtoEventArgs e);

	internal delegate void NetworkErrorEventHandler ();

	internal delegate void StartConnectEventHandler ();

    /// <summary>
    /// 访问服务器的socket 客户端，支持自动重连,支持自动接收数据包
    /// </summary>
	internal class SocketClient
    { 
		static UInt16 __staticSocketId = 1;
		static UInt16 __CreateSocketId(){
			__staticSocketId ++;
			return __staticSocketId;
		}

		private UInt16 _socketId = 1;

		private UInt16 _checkedSocketId = 0;
		public UInt16 checkedSocketId{
			set{
				_checkedSocketId = value;
			}
		}

		private string _ip = "" ;
        private int _port = 0;
        private string _token = "";
        private byte[] _secretKeyBytes;
		private TcpClient _tcpClient = null;
		private bool _keepConnecting = false;
        private DateTime _lastReceive;

		internal bool keepConnecting {
            get {
                return this._keepConnecting;
            }
        }

        private SocketClientState _state = SocketClientState.None;

		internal SocketClientState state {
            get {
                return _state;
            }
        }

		enum _NetworkReachability{
			NotReachable= 1,
			Wifi = 2,
			CarrierData = 3
		}

        private Thread _runConnectThread = null;
//        private Thread _sendDataThread = null;
        private Thread _readThread = null;
		private Thread _pingThread = null;
        private int _bufferSize = 2048;


		/// <summary>
		/// 40K 大小的缓存池
		/// </summary>
        private int _bufferWaitForProcessSize = 40960;
        private byte[] _buffer;
        private byte[] _bufferWaitForProcess;
        private int _bufferWaitForProcessPosition = 0;
        private int _bufferWaitForProcessEnd = 0;
        private int _threadNum = 0;
		private bool _outOfMaxMessageLength = false;

        private int _headerSize = 14;
//        bool _writing = false;
        /// <summary>
        /// 优先发送的数据包
        /// </summary>  
//        private Queue<ReceivedProtoEventArgs> _socketWriteQueue = new Queue<ReceivedProtoEventArgs> ();
//        private Queue<ReceivedProtoEventArgs> _writeQueue = new Queue<ReceivedProtoEventArgs> ();

        internal event ReceivedProtoEventHandler receiveProtoEventHandler;
        internal event NetworkErrorEventHandler networkErrorHandler;
        internal event StartConnectEventHandler startConnectEventHandler;
//        private bool _isc = false;
//        private bool _isCCed =false;
//		private int _NNN = 0;
        internal bool isConnected {
            get {
				if(this._networkReachability == _NetworkReachability.NotReachable){
					return false;
				}

				if (this._tcpClient == null || this._tcpClient.Client == null) {
                    return false;
                }
//                if(_isc != this._tcpClient.IsConnected() || _isCCed != this._tcpClient.Connected){
//                    Debug.Log(String.Format("LOGLOGLOG end   {0} {1} {2} {3}", Toolkit.GetDateTime(),  _isc, _isCCed, _NNN));
//                    _NNN = 1;
//                    _isc = this._tcpClient.IsConnected() ;
//                    _isCCed = this._tcpClient.Connected;
//                    Debug.Log(String.Format("LOGLOGLOG start {0} {1} {2}", Toolkit.GetDateTime(),  _isc, _isCCed));
//                }else{
//                    _NNN ++;
//                }
				return this._tcpClient.IsConnected();
            }
        }

		internal bool IsAlive(){
			switch(this._state){
			case SocketClientState.waitForConnectThread:
			case SocketClientState.startConnect:
			case SocketClientState.Connected:
			case SocketClientState.CheckTokened:
				return true;
			default:
				return false;

			}
			return false;
		}

		internal bool IsSame(string ip, int port, string token, byte[] secretKeyBytes){
			return this._ip == ip && this._port == port && this._token == token  && this._secretKeyBytes == secretKeyBytes;
		}

        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="ip">Ip.</param>
        /// <param name="port">Port.</param>
        internal void Connect (string ip, int port, string token, byte[] secretKeyBytes)
        {
			this._ip = ip;
            this._port = port;
            this._token = token;
            this._secretKeyBytes = secretKeyBytes;
			this._StartNewConnectThread();
		}

		private void _StartNewConnectThread(){
			++ this._threadNum ;
			this._FrozenSocket();
			this._keepConnecting = true;
			_runConnectThread = new Thread (_ConnectThread);
			_runConnectThread.Start ();
			this._state = SocketClientState.waitForConnectThread;
        }

        /// <summary>
        /// 连续重连次数
        /// </summary>
        internal const int Max_Retry_Times = 3;
        private int _retry_times = 0;

        internal bool IsNormal{
            get{
                return this.isConnected && this._state == SocketClientState.CheckTokened ;
            }
        }

        /// <summary>
        /// 这里面机会同时接受数据并解析
        /// 这段代码
        /// </summary>
        private void _ConnectThread ()
        {
			this._retry_times = 0;
			while (this._keepConnecting) {
				//判断正常状态里面单信息长度过长
				if(this._outOfMaxMessageLength){
					this._keepConnecting = false;
					if (this.networkErrorHandler != null) {
						this.networkErrorHandler ();
					}
					break;
				}

				//如果已经连接就等待50 毫秒，再次检测
                if (this.isConnected && this._state == SocketClientState.CheckTokened && (DateTime.UtcNow.Subtract( this._lastReceive).TotalSeconds) < 5) {
                    Thread.Sleep (20);
                } else {
					string log = String.Format( "pvpsc {0} {1} {2} {3}",
						Toolkit.GetDateTime(), this.isConnected , this._state == SocketClientState.CheckTokened ,
					 (DateTime.UtcNow.Subtract( this._lastReceive).TotalSeconds) < 5);

					MonitorLog.GetInstance().AddLog(log);
					Debug.Log (log);

                    if (this.startConnectEventHandler != null) {
                        this.startConnectEventHandler ();
                    }

                    //清掉已有的线程 
                    ++_threadNum ;
					this._socketId = __CreateSocketId();
                    if (this._tcpClient != null) {
                        try{
							this._tcpClient.Close();
                        }catch(Exception){
                        }
                    }
					this._outOfMaxMessageLength = false;
                    this._state = SocketClientState.startConnect;
                    //如果没有就连接端口，并且获取数据流
                    try {
						this._tcpClient = new TcpClient ();
						this._tcpClient.ConnectTimeout = new TimeSpan(0,0,5);
						IPAddress[] addresses = Dns.GetHostAddresses(this._ip);
						Thread.Sleep (300);
						Debug.Log("PVPSdk startConnect " + DateTime.Now.ToString() );
						this._tcpClient.Connect(addresses, this._port);
						if (!this._tcpClient.IsConnected())
                        {
							MonitorLog.GetInstance().AddLog(string.Format("cf {0} {1}", this._ip, this._port));
                            throw new Exception("Failed to connect.");
                        }
                    } catch (Exception e) {
						Debug.LogWarning (string.Format ("PVPSdk message {0} stackTrace {1}", e.Message, e.StackTrace));
                    }

                    if (this.isConnected) {
                        this._state = SocketClientState.Connected;
                        this._buffer = new byte[this._bufferSize];
                        this._bufferWaitForProcess = new byte[this._bufferWaitForProcessSize];
                        this._bufferWaitForProcessPosition = this._bufferWaitForProcessEnd = 0;

						_readThread = new Thread(this._Read);
                        _readThread.Start();
						_pingThread = new Thread(this._Ping);
						_pingThread.Start();

                        int times = 0;
                        if (this._tcpClient.Connected && this._state != SocketClientState.CheckTokened) {
                            times++;
                            Socket_CheckToken_Request t = new Socket_CheckToken_Request ();
                            t.proto_version = Config.PROTO_VERSION;
                            t.token = this._token;

                            if (PVPGlobal.appPlayerInfos != null) {
                                t.app_member_player_ids.AddRange (PVPGlobal.appPlayerInfos.Keys);
                            }

                            this.SendData<Socket_CheckToken_Request> (0, MessageTypeId.Socket_CheckToken_Request, t, true);

                        }
                        //再等待5秒
                        times = 0;
                        while (times < 50) {
                            times++;

                            if (!this._tcpClient.Connected) {//网络断了，直接离开吧
                                break;
                            }
                            if (this._state == SocketClientState.CheckTokened) {//网络验证通过了，直接离开吧
                                break;
                            }
                            //每隔0.1 秒 检查一次
                            Thread.Sleep (100);
                        }
                        if (!this._tcpClient.Connected || this._state != SocketClientState.CheckTokened) {
                            //问题来了，要重来
                            this._retry_times += 1;
                            ///抛出异常
                            if (this._retry_times >= Max_Retry_Times) {
                                this._keepConnecting = false;
                                if (this.networkErrorHandler != null) {
                                    this.networkErrorHandler ();
                                }
                            }
                        } else {
                            this._retry_times = 0;
                        }
                    } else {
                        this._retry_times += 1;
                        ///抛出异常
                        if (this._retry_times >= Max_Retry_Times) {
                            this._keepConnecting = false;
                            if (this.networkErrorHandler != null) {
                                this.networkErrorHandler ();
                            }
                        }
                    }
                }
            }
//			Debug.Log("PVPSdk Close 12: _ConnectThread close");
			MonitorLog.GetInstance().AddLog("cl 12");
			++_threadNum ;
            this._tcpClient.Close ();
            this._state = SocketClientState.Closed;
        }
		/// <summary>
		/// 当前接收缓冲区的数据长度
		/// </summary>
        private int _bufferWaitForProsessLength ;

        private void _Read ()
        {
            int bytesRead = 0;
			UInt16 socketId = this._socketId;
            int threadNum = this._threadNum;
            SocketError socketErrorCode = SocketError.IsConnected;
            while (this.keepConnecting && this.isConnected && threadNum == this._threadNum) {
                try {
                    // Begin receiving the data from the remote device.
                    bytesRead = this._tcpClient.Client.Receive (
                        this._buffer, 
                        0, 
                        this._bufferSize, 
                        SocketFlags.None,
						out socketErrorCode);
                    if(bytesRead <=0){
                        continue;
                    }
                    if(threadNum != this._threadNum){
                        break;
                    }

					if (bytesRead + this._bufferWaitForProcessEnd >= this._bufferWaitForProcess.Length) {
                        for (int i = this._bufferWaitForProcessPosition; i<this._bufferWaitForProcessEnd; i++) {
                            this._bufferWaitForProcess [i - this._bufferWaitForProcessPosition] = this._bufferWaitForProcess [i];
                        }
                        this._bufferWaitForProcessEnd -= this._bufferWaitForProcessPosition;
                        this._bufferWaitForProcessPosition = 0;
					
						if(bytesRead + _bufferWaitForProcessEnd >= _bufferWaitForProcess.Length) {
							Debug.LogError("Game message size too large!!!!");
							Array.Resize<byte>(ref _bufferWaitForProcess, _bufferWaitForProcess.Length + _bufferWaitForProcessSize);
						}
                    }

					//copy 
                    for (int i = 0; i<bytesRead; i++) {
                        this._bufferWaitForProcess [this._bufferWaitForProcessEnd + i] = this._buffer [i];
                    }

                    this._bufferWaitForProcessEnd += bytesRead;
                    _bufferWaitForProsessLength = this._bufferWaitForProcessEnd - this._bufferWaitForProcessPosition;
                    while (_bufferWaitForProsessLength >= this._headerSize) {
                        byte[] b = this._bufferWaitForProcess;
                        int position = this._bufferWaitForProcessPosition;
                        int protoLength = BitConverter.ToInt32 (b.SubArray (position + 10, 4).CheckBigEndian (), 0);

						if(protoLength >= 61440){ // 60K
							Debug.LogError(String.Format("message Length {0} too Long  ", protoLength));
							this._outOfMaxMessageLength = true;
							return;
						}


                        if (_bufferWaitForProsessLength >= this._headerSize + protoLength) {
                            UInt16 requestId = BitConverter.ToUInt16 (b.SubArray (position, 2).CheckBigEndian (), 0);
                            int errorCode = BitConverter.ToInt32 (b.SubArray (position + 2, 4).CheckBigEndian (), 0);
                            int messageTypeId = BitConverter.ToInt32 (b.SubArray (position + 6, 4).CheckBigEndian (), 0);
                            if(messageTypeId != MessageTypeId.Socket_HeartBeat_Pong){
                                ReceivedProtoEventArgs r = new ReceivedProtoEventArgs ();
                                r.bytes = null;
                                r.messageTypeId = messageTypeId;
                                r.errorCode = errorCode;
                                r.requestId = requestId;
								r.socketId = socketId;
                                if (r.messageTypeId == MessageTypeId.Socket_CheckToken_Response && r.errorCode == ErrorCode.SUCCESS) {
                                    this._state = SocketClientState.CheckTokened;
                                }

                                r.bytes = this._bufferWaitForProcess.SubArray (this._headerSize + position, protoLength);
                                if (receiveProtoEventHandler != null) {
                                    receiveProtoEventHandler (r);
                                }
							}else{
								this._lastPongTime = Toolkit.GetTimeStamp();
								this._lastPongRequestId = requestId;
								if(this._lastPongRequestId == this._lastPingRequestId){
									this._PingPongTimeDifference = 0;
								}else{
									this._PingPongTimeDifference --;
								}
//								Debug.Log(string.Format("pong {0} {1} {2}", this.PingPongTime, this._lastPingRequestId, this._lastPongRequestId));
							}

                            this._bufferWaitForProcessPosition += this._headerSize + protoLength;
                            this._lastReceive = DateTime.UtcNow ;
                        }else{
                            break;
                        }
                        _bufferWaitForProsessLength = this._bufferWaitForProcessEnd - this._bufferWaitForProcessPosition;
                    }
                } catch (Exception e) {
					MonitorLog.GetInstance().AddLog("read close");
					Debug.LogWarning("PVPSdk Read Close");
					Debug.LogWarning (e.Message);
					Debug.LogWarning (e.StackTrace);
                    if(threadNum == this._threadNum){
                        this._state = SocketClientState.Closed;
                    }
                    return;
                }
            }
        }


		private void _Ping(){
			int threadNum = this._threadNum;
			while (this._keepConnecting && this.isConnected && threadNum == this._threadNum) {
				this._lastPingTime = Toolkit.GetTimeStamp();
				this._lastPingRequestId = RequestIdCreater.GetOne();
				this._PingPongTimeDifference ++;
				this.SendData (this._lastPingRequestId, MessageTypeId.Socket_HeartBeat_Ping);
//				Debug.Log(string.Format("ping {0} {1} {2}", this.PingPongTime, this._lastPingRequestId, this._lastPongRequestId));
				Thread.Sleep (1000);
			}
		}

//		private Int64 _lastSendTime = 0;

		//心跳包的时间
		private Int64 _lastPingTime = 0;
		private Int64 _lastPongTime = 0;
		private int _PingPongTimeDifference = 0;
		private UInt16 _lastPingRequestId = 0;
		private UInt16 _lastPongRequestId = 0;


		/// <summary>
		/// 网络延时的计算
		/// 如果 _lastPingRequestId == _lastPongRequestId
		/// 那么 _lastPongTime - _lastPingTime
		/// 如果 _lastPingRequestId == _lastPongRequestId + 1， 就是 发了一个ping，还没受到回报
		/// 那么 _lastPongTime - _lastPingTime + 1
		/// 其他情况，那么网络状态就很不好
		/// 
		/// 统一的网络公式： _lastPongTime - _lastPingTime + (_lastPingRequestId - _lastPongRequestId) * 发送间隔时间1
		/// </summary>
		/// <value>The socket delayed.</value>
		internal Int64 PingPongTime{
			get{
				if(_lastPongTime == 0){
					return Toolkit.GetTimeStamp() - _lastPingTime;
				}else{
					return _lastPongTime - _lastPingTime + _PingPongTimeDifference * 1000;
				}
			}
		}

        


		private void _DoSendData (byte[] buffer)
		{
//			Int64 t = Toolkit.GetTimeStamp ();
			try {
				int length = 0;
				int offset = 0;
				int maxLenth = buffer.Length;
				Socket socket = this._tcpClient.Client;
				do{
					length = socket.Send (buffer, offset, maxLenth - offset, SocketFlags.None);
					if (length <= 0) {
						this._state = SocketClientState.Closed;
						break;
					}
					offset += length;
				}while(offset + 1 < maxLenth);
			} catch (Exception ex) {
				Debug.LogWarning (ex.Message);
				Debug.LogWarning (ex.StackTrace);
				Debug.LogWarning ("PVPSdk Close DoSendData Exception");
				MonitorLog.GetInstance().AddLog("send close " + ex.Message);
				this._state = SocketClientState.Closed;
			}
		}

        /// <summary>
        /// 发心跳包也放在这里面了
        /// </summary>
//        private void _DoSendData ()
//        {
//            int threadNum = this._threadNum;
//            while (this._keepConnecting && this.isConnected && threadNum == this._threadNum) {
//                double t = Toolkit.GetTimeStamp ();
//
//                if (_socketWriteQueue.Count == 0 && _writeQueue.Count == 0) {
//                    if ((t - this._lastSendTime) >= 1000) {
//                        this._lastSendTime = t;
//
//						this.SendData (RequestIdCreater.GetOne(), MessageTypeId.Socket_HeartBeat_Ping);
//                    }
//                } else {
//                    try {
//                        int length = 0;
//                        Socket socket = this._tcpClient.Client;
//                        if (_socketWriteQueue.Count > 0) {
//                            while (_socketWriteQueue.Count > 0) {
//                                ReceivedProtoEventArgs item = _socketWriteQueue.Dequeue ();
//
//                                length = socket.Send (item.bytes);
//
//                                if (length <= 0) {
//									Debug.LogWarning ("PVPSdk Close DoSendData");
//                                    if(threadNum == this._threadNum){
//                                        this._state = SocketClientState.Closed;
//                                    }
//                                    break;
//                                }
//                            }
//                        }
//                        if (this.state == SocketClientState.CheckTokened) {
//                            if (_writeQueue.Count > 0) {
//                                while (_writeQueue.Count > 0) {
//                                    ReceivedProtoEventArgs item = _writeQueue.Dequeue ();
//                                    length = socket.Send (item.bytes);
//                                    if (length <= 0) {
//										Debug.LogWarning ("PVPSdk Close DoSendData CheckTokened");
//                                        if(threadNum == this._threadNum){
//                                            this._state = SocketClientState.Closed;
//                                        }
//                                        break;
//                                    }
//                                }
//                            }
//                        }
//                    } catch (Exception ex) {
//						Debug.LogWarning (ex.Message);
//						Debug.LogWarning (ex.StackTrace);
//						Debug.LogWarning ("PVPSdk Close DoSendData Exception");
//                        if(threadNum == this._threadNum){
//                            this._state = SocketClientState.Closed;
//                        }
//                    }
//                }
//                Thread.Sleep (15);
//            }
//        }

        internal bool SendData (UInt16 requestId, int messageTypeId)
        {
            if (!this.isConnected) {
                return false;
            }
			if(this._checkedSocketId != _socketId){
				return false;
			}
            byte[] bf = new byte[12];//+ b.Length];
            Array.Copy (SocketClient.UInt162ByteArray (requestId).CheckBigEndian (), bf, 2);
            Array.Copy (SocketClient.Int2ByteArray (messageTypeId).CheckBigEndian (), 0, bf, 2, 4);
            Array.Copy (SocketClient.Int2ByteArray (0).CheckBigEndian (), 0, bf, 6, 4);
            Array.Copy (this._GetCheckSum (bf), 0, bf, 10, 2);

//            ReceivedProtoEventArgs item = new ReceivedProtoEventArgs ();
//            item.messageTypeId = messageTypeId;
//			item.requestId = requestId;
//            item.bytes = bf;
//            _writeQueue.Enqueue (item);
			_DoSendData(bf);
            return true;
        }

        /// <summary>
        /// 计算校验和
        /// </summary>
        /// <returns>The check sum.</returns>
        /// <param name="b">要计算的部分 ，注意最后两位用来存放校验位，不一起计算 .</param>
        /// <param name="secretKey">Secret key.</param>
        private byte[] _GetCheckSum (byte[] b)
        {
            int length = b.Length;

            byte[] bb = new byte[length - 2 + this._secretKeyBytes.Length];
            Array.Copy (b, 0, bb, 0, length - 2);
            Array.Copy (this._secretKeyBytes, 0, bb, length - 2, this._secretKeyBytes.Length);

            MD5 md5Hash = MD5.Create ();
            byte[] data = md5Hash.ComputeHash (bb);
            UInt32 sum = 0;

            if (BitConverter.IsLittleEndian) {
                byte t;
                for (int i = 0; i < data.Length; i += 2) {
                    t = data [i];
                    data [i] = data [i + 1];
                    data [i + 1] = t;
                }
            }
            for (int i = 0; i < data.Length; i += 2) {
                UInt16 n = BitConverter.ToUInt16 (data, i);
                sum += n;
            }
            while (sum > 0xffff) {
                Console.WriteLine (Convert.ToString (sum, 2));
                sum = (sum >> 16) + (sum & 0xffff);
            }
            return SocketClient.UInt162ByteArray (Convert.ToUInt16 (sum)).CheckBigEndian ();
        }

        internal bool SendData<T> (UInt16 requestId, int messageTypeId, T t, bool is_token = false) where T:global::ProtoBuf.IExtensible
        {
            if (!this.isConnected) {
                return false;
            }
			if(!is_token && this._checkedSocketId != _socketId){
				Debug.Log(string.Format("{0} {1} {2}", this._checkedSocketId, _socketId, messageTypeId));
				Debug.LogWarning("you should not pass data before connectedToGameServerEventHandler");
				MonitorLog.GetInstance().AddLog("sendData cId!=sID");
				return false;
			}

            byte[] b = this._Serialize<T> (t);
            byte[] bf = new byte[12 + b.Length];
            Array.Copy (SocketClient.UInt162ByteArray (requestId).CheckBigEndian (), bf, 2);
            Array.Copy (SocketClient.Int2ByteArray (messageTypeId).CheckBigEndian (), 0, bf, 2, 4);
            Array.Copy (SocketClient.Int2ByteArray (b.Length).CheckBigEndian (), 0, bf, 6, 4);

            // 设置校验和
            Array.Copy (b, 0, bf, 10, b.Length);
            Array.Copy (this._GetCheckSum (bf), 0, bf, bf.Length - 2, 2);

//            ReceivedProtoEventArgs item = new ReceivedProtoEventArgs ();
//			item.requestId = requestId;
//            item.messageTypeId = messageTypeId;
//            item.bytes = bf;
            if (is_token) {
				this._DoSendData(bf);
            } else {
				if(this._state == SocketClientState.CheckTokened){
					this._DoSendData (bf);
				}else{
					return false;
				}
            }
            return true;
        }


        /// <summary>
        /// Int2s the byte array.
        /// </summary>
        /// <returns>The byte array.</returns>
        /// <param name="intValue">Int value.</param>
        internal static byte[] Int2ByteArray (int intValue)
        {
            return BitConverter.GetBytes (intValue);
        }

        internal static byte[] UInt162ByteArray (UInt16 value)
        {
            return BitConverter.GetBytes (value);
        }

        /// <summary>
        /// Serialize the specified t.
        /// </summary>
        /// <param name="t">T.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        private byte[] _Serialize<T> (T t) where T:global::ProtoBuf.IExtensible
        {
            MemoryStream ms = new MemoryStream ();
            ProtoBuf.Serializer.Serialize<T> (ms, t);

            return ms.GetBuffer ().SubArray (0, Convert.ToInt32 (ms.Length));
        }

		/// <summary>
		/// 纯粹的冻结网络，不修改各个值
		/// </summary>
		internal void _FrozenSocket(){
			this._state = SocketClientState.Closed;
			++ this._threadNum ;
			if (this._runConnectThread != null) {
				try {
					this._runConnectThread.Abort ();
				} catch (Exception) {
				}
				this._runConnectThread = null;
			}

			if(this._readThread != null){
				try{
					this._readThread.Abort();
				}catch(Exception){
				}
				this._readThread = null;
			}

			if(this._pingThread != null){
				try{
					this._pingThread.Abort();
				}catch(Exception){
				}
				this._pingThread = null;
			}

			if (this._tcpClient != null) {
				try {
					MonitorLog.GetInstance().AddLog("c11");
					this._tcpClient.Close ();
				} catch (Exception) {
				}
				this._tcpClient = null;
			}
		}

		/// <summary>
		/// 这个是网络外层中断
		/// </summary>
		internal void Close ()
        {
            this._keepConnecting = false;
			this._FrozenSocket();
        }

		private _NetworkReachability _networkReachability;

		private _NetworkReachability _GetNetworkReachability(NetworkReachability n){
			switch(n){
			case NetworkReachability.NotReachable:
				return _NetworkReachability.NotReachable;
				break;
			case NetworkReachability.ReachableViaCarrierDataNetwork:
				return _NetworkReachability.CarrierData;
				break;
			case NetworkReachability.ReachableViaLocalAreaNetwork:
				return _NetworkReachability.CarrierData;
				break;
			default:
				return _NetworkReachability.NotReachable;
				break;
			}
		}

		/// <summary>
		/// 在主线程被调用
		/// </summary>
		internal void OnStateChange(NetworkReachability networkReachability){
			this._networkReachability = _GetNetworkReachability(networkReachability);
			this._state = SocketClientState.Closed;
			if(this._keepConnecting && this._ip.Length > 0 && this._token.Length > 0 ){
				if( this._networkReachability != _NetworkReachability.NotReachable ){
					this._StartNewConnectThread();
				}else{
					this._FrozenSocket();
					if (this.networkErrorHandler != null) {
						this.networkErrorHandler ();
					}
				}
			}
		}
    }
}