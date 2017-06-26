using System;

namespace PVPSdk.PVP
{
	internal class RequestIdCreater
	{
		private static UInt16 _requestId = 0;
		internal static UInt16 GetOne ()
		{
			++ _requestId ;
			if(_requestId >= 65500){
				_requestId = 1;
			}
			return _requestId;
		}
	}
}

