using System;

namespace PVPSdk.PVP
{
	internal class Config
	{
        internal const int PROTO_VERSION = 2;

		//internal const string QdevHttpUri = "http://qdev2.pvp.monthurs.com:83/";// test version
        internal const string QdevHttpUri = "http://qdev2.pvp.monthurs.com:83/"; // online version
		internal const string AWSHttpUri = "https://aws.pvp.magiplay.com/";
		internal const string ALIHttpUri = "https://ali.pvp.magiplay.com/";

		internal const string IP_Uri = "ip.php";
		internal const string ReceiveUri = "receiveTimeLog.php";

        internal static string HttpUri ;

		//internal const string LoginRequest = "User/Login";
        
		internal static int protocol_request_timeout = 30;
		internal static int connect_timeout = 30;

        internal static string appKey = "";
	}
}