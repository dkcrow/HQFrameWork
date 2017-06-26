using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using ProtoBuf;
using System.Text;
using System.IO;
using System;


namespace PVPSdk.PVP{
    /// <summary>
    /// 公用工具类
    /// </summary>
    public class Toolkit 
    {

    	/// <summary>
    	/// 返回向量r旋转角度degA后的向量
    	/// </summary>
    	/// <returns>The rotate a.</returns>
    	/// <param name="r">The red component.</param>
    	/// <param name="degA">Deg a.</param>
    	internal static Vector2 RotateA(Vector2 r, float degA){
    		float radA = Mathf.Deg2Rad * degA;
    		float sinRadA = Mathf.Sin (radA);
    		float cosRadA = Mathf.Cos (radA);
    		return new Vector2 (r.x * cosRadA - r.y*sinRadA, r.x*sinRadA + r.y * cosRadA);
    	}

        /// <summary>
        /// 返回向量r旋转角度degA后的向量
        /// </summary>
        /// <returns>The rotate a.</returns>
        /// <param name="r">The red component.</param>
        /// <param name="degA">Deg a.</param>
        internal static Vector3 RotateA3(Vector2 r, float degA){
            float radA = Mathf.Deg2Rad * degA;
            float sinRadA = Mathf.Sin (radA);
            float cosRadA = Mathf.Cos (radA);
            //Debug.LogError (string.Format ("{0} {1} {2} {3} {4} ",r.x,r.y,degA, r.x * cosRadA - r.y * sinRadA, r.x * sinRadA + r.y * cosRadA));
            return new Vector3 (r.x * cosRadA - r.y*sinRadA, r.x*sinRadA + r.y * cosRadA, 0);
        }

        /// <summary>
        /// 只对X Y 生效
        /// </summary>
        /// <returns>The a3.</returns>
        /// <param name="r">The red component.</param>
        /// <param name="degA">Deg a.</param>
        internal static Vector3 RotateA3(Vector3 r, float degA){
            float radA = Mathf.Deg2Rad * degA;
            float sinRadA = Mathf.Sin (radA);
            float cosRadA = Mathf.Cos (radA);
            return new Vector3 (r.x * cosRadA - r.y*sinRadA, r.x*sinRadA + r.y * cosRadA, 0);
        }

        internal static float GetAngle(Vector2 _from , Vector2 _to){
            if (Vector3.Cross (new Vector3 (_from.x, _from.y, 0), new Vector3 (_to.x, _to.y)).z > 0) {
                return Vector2.Angle (_from, _to);
            } else {
                return -Vector2.Angle (_from, _to);
            }


        }

        /// <summary>
        /// 忽略Z轴
        /// </summary>
        /// <returns>The angle.</returns>
        /// <param name="_from">From.</param>
        /// <param name="_to">To.</param>
        internal static float GetAngle(Vector3 _from, Vector3 _to){
            return GetAngle (new Vector2 (_from.x, _from.y), new Vector2 (_to.x, _to.y));
        }

        internal static Vector2 GetVector2(float angle){
            float radA = Mathf.Deg2Rad * angle;
            return new Vector2 (Mathf.Cos (radA), Mathf.Sin (radA));
        }

        /// <summary>
        /// Gets the md5 hash.
        /// </summary>
        /// <returns>The md5 hash.</returns>
        /// <param name="input">Input.</param>
        /// <param name="encoding">Encoding.</param>
    	internal static string GetMd5Hash(string input, string encoding)
    	{
    		MD5 md5Hash = MD5.Create ();
    		// Convert the input string to a byte array and compute the hash.
    		byte[] data = md5Hash.ComputeHash(Encoding.GetEncoding(encoding).GetBytes(input));
    		StringBuilder sBuilder = new StringBuilder();
    		for (int i = 0; i < data.Length; i++)
    		{
    			sBuilder.Append(data[i].ToString("x2"));
    		}
    			
    		return sBuilder.ToString();
    	}

        /// <summary>
        /// Gets the child.
        /// </summary>
        /// <returns>The child.</returns>
        /// <param name="parent">Parent.</param>
        /// <param name="name">Name.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
    	internal static GameObject GetChild<T>(GameObject parent, string name) where T : Component{
    		T[] gs = parent.GetComponentsInChildren<T> ();
    		foreach (T g in gs) {
    			if (g.name.CompareTo(name) == 0) {
    				return g.gameObject;
    			}
    		}
    		return null;
    	}

        /// <summary>
        /// Gets the child t.
        /// </summary>
        /// <returns>The child t.</returns>
        /// <param name="parent">Parent.</param>
        /// <param name="name">Name.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
    	internal static T GetChildT<T>(GameObject parent, string name) where T : Component{
    		T[] gs = parent.GetComponentsInChildren<T> ();
    		foreach (T g in gs) {
    			if (g.name.CompareTo(name) == 0) {
    				return g;
    			}
    		}
    		return null;
    	}

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <returns>The children.</returns>
        /// <param name="parent">Parent.</param>
        /// <param name="names">Names.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
    	internal static Dictionary<String, GameObject> GetChildren<T>(GameObject parent, List<String> names) where T : Component{
    		T[] gs = parent.GetComponentsInChildren<T> ();
    		Dictionary<String, GameObject> r = new  Dictionary<String, GameObject>  ();
    		foreach (T g in gs) {
    			if (names.Contains(g.name)) {
    				r [g.name] = g.gameObject;
     			}
    		}
    		return r;
    	}

        /// <summary>
        /// Gets the writable path.
        /// </summary>
        /// <returns>The writable path.</returns>
        internal static string GetWritableRoot()
        {
            string path = null;
            if (Application.platform == RuntimePlatform.IPhonePlayer) {
                path = Application.dataPath.Substring (0, Application.dataPath.Length - 5);
                path = path.Substring (0, path.LastIndexOf ('/')) + "/Documents"; 

            } else if (Application.platform == RuntimePlatform.Android) {

                path = Application.persistentDataPath;

            }
            else {

                path = Application.dataPath;
            }

            return path + "/StreamingAssets/";
        }

//        internal static string GetBattleJsonPath()
//        {
//            string path = null;
//            if (Application.platform == RuntimePlatform.IPhonePlayer) {
//                path = Application.dataPath.Substring (0, Application.dataPath.Length - 5);
//                path = path.Substring (0, path.LastIndexOf ('/')) + "/Documents" + GlobalData.battle_json_file_path_section; 
//
//
//            } else if (Application.platform == RuntimePlatform.Android) {
//
//                path = Application.persistentDataPath + GlobalData.battle_json_file_path_section;
//
//            }
//            else {
//
//                path = Application.dataPath + GlobalData.battle_json_file_path_section;
//            }
//
//            return path;
//        }
//

		private static string _path = null;
        /// <summary>
        /// Gets the writable path.
        /// </summary>
        /// <returns>The writable path.</returns>
        internal static string GetWritablePath()
        {
//            string path = null;
			if(_path == null){
            	if (Application.platform == RuntimePlatform.IPhonePlayer) {
					_path = Application.persistentDataPath + "/Documents/";                 
            	} else if (Application.platform == RuntimePlatform.Android) {
					_path = Application.persistentDataPath + "/Documents/";
            	} else {
					_path = Application.dataPath+ "/Documents/";
            	}
			}

            return _path;
        }


		/// <summary>
		/// Saves the file.
		/// </summary>
		/// <param name="file_name">File name.</param>
		/// <param name="data">Data.</param>
		/// <param name="save_path">Save path.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		internal static string SaveFile(string file_name, string data, string save_path = "")
		{
			if (string.IsNullOrEmpty(save_path))
				save_path = Toolkit.GetWritablePath ();

			string save_file_path = save_path + file_name;
			string temp_file_path = save_path + file_name + ".tmp";

			if (!Directory.Exists(save_path))
				Directory.CreateDirectory(save_path);

			if (File.Exists(temp_file_path))
				File.Delete(temp_file_path);
			File.WriteAllText(temp_file_path, data);
			if (File.Exists(save_file_path))
				File.Delete(save_file_path);
			File.Move (temp_file_path, save_file_path);

			return save_file_path;
		}

		internal static string AppendFile(string file_name, string data, string save_path = ""){
			if (string.IsNullOrEmpty(save_path))
				save_path = Toolkit.GetWritablePath ();

			string save_file_path = save_path + file_name;

			if (!Directory.Exists(save_path))
				Directory.CreateDirectory(save_path);

			File.AppendAllText(save_file_path, data);

			return save_file_path;
		}


		/// <summary>
		/// Loads the file.
		/// </summary>
		/// <returns>The file.</returns>
		/// <param name="file_name">File name.</param>
		/// <param name="save_path">Save path.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		internal static string LoadFile(string file_name, string savePath = null)
		{
			if ( string.IsNullOrEmpty(savePath) ) 
			{
				savePath = Toolkit.GetWritablePath ();
			}
			string save_file_path = savePath + file_name;
			string data = "";
			try{
				data = File.ReadAllText (save_file_path);
			}catch(Exception){
				return "";
			}
			return data;
		}

		internal static int GetFileSize(string file_name, string savePath = null)
		{
			if ( string.IsNullOrEmpty(savePath) ) 
			{
				savePath = Toolkit.GetWritablePath ();
			}
			string save_file_path = savePath + file_name;
			try{
				return File.ReadAllText (save_file_path).Length;
			}catch(Exception){
				return 0;
			}
			return 0;
		}


        /// <summary>
        /// Saves the file.
        /// </summary>
        /// <param name="file_name">File name.</param>
        /// <param name="data">Data.</param>
        /// <param name="save_path">Save path.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        internal static string SaveFile<T>(string file_name, T data, string save_path = "") where T : global::ProtoBuf.IExtensible
        {
            if (string.IsNullOrEmpty(save_path))
                save_path = Toolkit.GetWritablePath ();

            string save_file_path = save_path + file_name;
            string temp_file_path = save_path + file_name + ".tmp";

            if (!Directory.Exists(save_path))
                Directory.CreateDirectory(save_path);

            if (File.Exists(temp_file_path))
                File.Delete(temp_file_path);
            MemoryStream stream = new MemoryStream();
            ProtoBuf.Serializer.Serialize<T> (stream, data);
            stream.Position = 0;
            Byte[] b = new byte[stream.Length];
            stream.Read (b, 0, (int)stream.Length);
            File.WriteAllBytes(temp_file_path, b);
            if (File.Exists(save_file_path))
                File.Delete(save_file_path);
            File.Move (temp_file_path, save_file_path);

            return save_file_path;
        }

        

        /// <summary>
        /// Loads the file.
        /// </summary>
        /// <returns>The file.</returns>
        /// <param name="file_name">File name.</param>
        /// <param name="save_path">Save path.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        internal static T LoadFile<T>(string file_name, string savePath = null)  where T : global::ProtoBuf.IExtensible
        {
            if ( string.IsNullOrEmpty(savePath) ) 
    		{
    			savePath = Toolkit.GetWritablePath ();
    		}
            string save_file_path = savePath + file_name;
            byte[] data = null;
            try{
                data = File.ReadAllBytes (save_file_path);
            }catch(Exception){
                return default(T);
            }
            if (data != null && data.Length > 0) {
                MemoryStream resStream = new MemoryStream ();
                resStream.Write (data, 0, data.Length);
                resStream.Position = 0;
                try{
                    T t = ProtoBuf.Serializer.Deserialize<T> (resStream);
                    if(t != null){
                        return t;
                    }else{
                        return default(T);
                    }
                }catch(Exception){
                    return default(T);
                }
            }

            return default(T);
        }

        internal static bool FileExists(string file_name, string path = "") {
            if (string.IsNullOrEmpty(path))
                path = GetWritablePath ();
            string save_path = path + file_name;
            return File.Exists (save_path);
        }

        internal static void DeleteFile(string file_name, string path = "") {

            if (string.IsNullOrEmpty(path))
                path = GetWritablePath ();
                
            string save_path = path + file_name;
//            Debug.Log ("========Toolkit::DeleteFile " + save_path);
            if (File.Exists(save_path))
                File.Delete(save_path);
        }

        private static DateTime _starTime = new DateTime(1970, 1, 1);
    	/// <summary>
        /// Unix时间戳格式
        /// </summary>
        /// <returns>The unix time.</returns>
    	public static int GetUnixTime() 
    	{ 
            TimeSpan t = (DateTime.UtcNow - _starTime);
            return (int)t.TotalSeconds;
    	}

        /// <summary>
        /// 获取 从 1970-1-1 0:0:0  开始到datetime 的秒数，Unix时间戳格式
        /// </summary>
        /// <returns>The unix time.</returns>
        /// <param name="dt">Dt.</param>
		public static int GetUnixTime(DateTime dt) 
        { 
            TimeSpan t = (dt - _starTime);
            return (int)t.TotalSeconds;
        }

        /// <summary>
        /// Strings the convert list.
        /// </summary>
        /// <returns>The convert list.</returns>
        /// <param name="separator">Separator.</param>
        /// <param name="str">String.</param>
        /// <param name="list">List.</param>
        internal static ArrayList StringConvertList (char separator, string str, ArrayList list)
    	{
            list.Clear();
            if (!string.IsNullOrEmpty(str))
    		{
                string[] ss = str.Split (separator);
    			foreach(string s in ss)
    				list.Add (s);
    		}

    		return list;
    	}

/// <summary>
/// 秒数转换成 System.DateTime
/// </summary>
/// <returns>The int date time.</returns>
/// <param name="seconds">从 1970-01-01 0:0:0 开始的秒数，可以有小数点</param>
        public static System.DateTime ConvertIntDateTime(double seconds)
        {
            System.DateTime time = System.DateTime.MinValue;
            System.DateTime startTime = System.TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            time = startTime.AddSeconds(seconds);
            return time;
        }

        /// <summary>
        /// Shuffles the list.
        /// </summary>
        /// <returns>The list.</returns>
        /// <param name="list">List.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        internal static List<T> ShuffleList<T>(List<T> list)
        {
            List<T> randomizedList = new List<T>();
            System.Random rnd = new System.Random();
            while (list.Count > 0)
            {
                int index = rnd.Next(0, list.Count); //pick a random item from the master list
                randomizedList.Add(list[index]); //place it at the end of the randomized list
                list.RemoveAt(index);
            }
            return randomizedList;
        }

        /// <summary>
        /// Gets the local streaming assets path.
        /// </summary>
        /// <returns>The local streaming assets path.</returns>
        /// <param name="file_name">File name.</param>
        static internal string GetLocalStreamingAssetsPath(string file_name)
        {
            string url_path = System.IO.Path.Combine(Application.streamingAssetsPath, file_name);
            
            if (Application.platform != RuntimePlatform.Android)
            {
                if (!url_path.Contains ("://")) {
                    
                    url_path = "file://" + url_path;
                }
            }
            
            return url_path;
        }


        internal static int CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[2048];
            int totoal_read = 0;
            int read;
            while ((read = input.Read (buffer, 0, buffer.Length)) > 0) {
                totoal_read += read;
                output.Write (buffer, 0, read);
            }

            return totoal_read;
        }

    	internal static int ConvertVerToInt(string ver) {
    		string str = ver.Replace( ".", "" );
    		int assetVer = 0;
    		if (!int.TryParse(str.Trim(), out assetVer)) {
				Debug.LogError("ConvertAssetVerToInt error ver = " + ver);
    		}
    		return assetVer;
    	}

        internal static string GetDateTime(){
			return DateTime.UtcNow.ToLongTimeString();
        }

        internal static string GetString(PVPClient.AutoLoginInfo autoLoginInfo){
            return string.Format("autoLoginInfo.auto_login {0}\n, autoLoginInfo.login_type {1}\n, autoLoginInfo.facebook_access_token {2}\n," +
                " autoLoginInfo.facebook_expires {3}\n, autoLoginInfo.facebook_fb_uid {4}\n, autoLoginInfo.facebook_player_id {5}\n, autoLoginInfo.guest_access_token {6}\n, autoLoginInfo.guest_player_id {7}\n", 
                autoLoginInfo.auto_login, autoLoginInfo.login_type, autoLoginInfo.facebook_access_token, autoLoginInfo.facebook_expires, autoLoginInfo.facebook_fb_uid, autoLoginInfo.facebook_player_id, autoLoginInfo.guest_access_token, autoLoginInfo.guest_player_id);
        }

		private static DateTime zeroDateTime = new DateTime (1970, 1, 1, 0, 0, 0, 0);

		/// <summary>
		/// 获取时间戳，精确到毫秒
		/// </summary>
		/// <returns>The time tikct.</returns>
		internal static Int64 GetTimeStamp(){
			return (Int64)((DateTime.UtcNow - zeroDateTime).TotalMilliseconds);
		}

	}
}