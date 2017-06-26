using System;
using System.IO;

namespace PVPSdk{
	/// <summary>
	/// 
	/// </summary>
	internal abstract class AbstractHttpProtocol {
	    protected string objName { get; set; }
	    
	    protected string method { get; set; }
	    
	    internal string tValue { get; set; }
	    
	    internal int error_code = -1;
	    internal bool show_loading = true;
	    internal bool handle_error = true;
        internal string token = "";
        internal ushort requestId = 0;
	    internal Byte[] protobufBytes ;
	    
	    internal MemoryStream resStream = null;

	    internal abstract string GetRequestUrl();
	    internal abstract void SetResMsg(Stream stream, long length);
	    internal abstract void SetResMsg(byte[] stream, long length);

	    public override string ToString () {
	        return objName + "_" + method + "_Msg";
	    }
	    
	    internal string GetBroadcastMsg () {
	        return this.ToString();
	    }
	}

}