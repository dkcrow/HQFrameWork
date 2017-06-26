using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System;
using ProtoBuf;
using System.Text.RegularExpressions;


namespace PVPSdk{
	/// <summary>
	/// http 的请求协议，http 一定有响应报
	/// </summary>
	internal class HttpProtocol<T1, T2> : AbstractHttpProtocol where T1:global::ProtoBuf.IExtensible where T2: global::ProtoBuf.IExtensible  {
	    private const string TAG = "[Protocol]";

	    internal HttpProtocol(){
	        string reqName = typeof(T1).ToString();

	        if (!string.IsNullOrEmpty(reqName)) {
	            string reqNameStr = reqName.Substring (reqName.IndexOf ('.') + 1);
	            string[] reqNameSplit = reqNameStr.Split ('_');
	            if (reqNameSplit != null && reqNameSplit.Length != 0) {
	                this.objName = reqNameSplit [0];
	                this.method = reqNameSplit [1];
	            } else {
//	                Debug.LogError (TAG + " reqName split error!");
	            }
	        } else {
//	            Debug.LogError (TAG + " reqName is null!");
	        }
	        this.tValue = PVP.Toolkit.GetUnixTime().ToString();
	    }


	    internal void SetReqMsg( T1 obj )  {
	        MemoryStream stream = new MemoryStream();
	        ProtoBuf.Serializer.Serialize<T1>(stream, obj);
	        stream.Position = 0;
	        this.protobufBytes = new byte[stream.Length];
            stream.Read (this.protobufBytes, 0, (int)stream.Length);
	    }

	    internal override string GetRequestUrl () {
			string request_url = PVP.Config.HttpUri + objName + "/" + method;
//	        Debug.Log ("Protocol GetRequestUrl = " + request_url);
	        return request_url;
	    }

	    internal T2 GetResMsg (){
	        if (resStream != null && resStream.Length > 0) {
//				byte[] buf = resStream.GetBuffer ();
//				int error_code = BitConverter.ToInt32 (buf, 0);
				resStream.Position = 4;
	            return ProtoBuf.Serializer.Deserialize<T2>( resStream );
	        } else {
                return default(T2);
	        }
	    }

	    internal override void SetResMsg(byte[] stream, long length){
            if (resStream != null) {
	            resStream.Close ();
	            resStream = null;
	        }

	        resStream = new MemoryStream ();
	        resStream.Write (stream, 0, (int)length);

	        if (length >= 4) {
	            base.error_code = System.BitConverter.ToInt32(stream, 0);
//	            Debug.Log ("SetResMsg error_code = " + error_code + ", stream read = " + length);
	        }
	    }

	    internal override void SetResMsg(Stream stream, long length){

	        if (resStream != null) {
	            resStream.Close ();
	            resStream = null;
	        }

	        resStream = new MemoryStream ();
	        byte[] buffer = new byte[length];
	        int numBytesToRead = (int)length;
	        int numBytesRead = 0;
	        do {
	            int read = stream.Read (buffer, numBytesRead, numBytesToRead);
	            numBytesRead += read;
	            numBytesToRead -= read;
//	            Debug.Log ("SetResMsg length = " + length);
	        }while (numBytesToRead > 0);
	            
	        if (numBytesRead >= 4) {
	            base.error_code = System.BitConverter.ToInt32(buffer, 0);
//	            Debug.Log ("SetResMsg error_code = " + error_code + ", stream read = " + numBytesRead);
	//            read = Toolkit.CopyStream (stream, resStream);
	//            Debug.Log ("SetResMsg resStream len = " + resStream.Length + ", stream read = " + read);
	        }
	        if (numBytesRead > 4) {

	            resStream.Write (buffer, 0, numBytesRead);
	        }
	    }
	}
}