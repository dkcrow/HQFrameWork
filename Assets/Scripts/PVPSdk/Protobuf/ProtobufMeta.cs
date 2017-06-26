using System;
using UnityEngine;
using ProtoBuf;

namespace PVPSdk
{
	internal class ProtobufMeta<T> : AbstractProtobufMeta where T: global::ProtoBuf.IExtensible
	{
		internal int error_code;
		internal T protbufMeta;
	}
}

