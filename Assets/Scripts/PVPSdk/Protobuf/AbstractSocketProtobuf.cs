using System;
using System.IO;

namespace PVPSdk
{
	/// <summary>
	/// socket 的协议，这里面的纯粹封装了协议，请求不一定有响应报，
	/// 或者说协议只是一个信息报
	/// </summary>
	internal abstract class AbstractSocketProtobuf{

		internal string tValue { get; set; }

		internal int error_code = -1;
		internal bool show_loading = true;
		internal bool handle_error = true;

		internal Byte[] postBody ;

		internal MemoryStream resStream = null;

		internal abstract string GetRequestUrl();
		internal abstract void SetResMsg(Stream stream, long length);
		internal abstract void SetResMsg(byte[] stream, long length);

		public override string ToString () {
			return  "_Msg";
		}

		internal string GetBroadcastMsg () {
			return this.ToString();
		}
	}
}

