using System;

namespace PVPSdk.Http
{
	internal delegate void Callback();
	internal delegate void Callback<T>(T arg1);
	internal delegate void Callback<T, U>(T arg1, U arg2);
	internal delegate void Callback<T, U, V>(T arg1, U arg2, V arg3);
}

