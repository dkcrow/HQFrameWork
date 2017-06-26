using System;
using System.Collections.Generic;
using PVPProtobuf;

namespace PVPSdk.PVP
{
    /// <summary>
    /// 更新自定义数据，在三个地方使用，
    /// 1、应用内的用户自定义信息（永久数据）
    /// 2、房间的公共自定义信息（临时数据）
    /// 3、房间内的用户自定义信息（临时数据）
    /// </summary>
    public class NewCustomData
    {
        public UInt64 memberPlayerId { get; private set; }
        public uint customDataNumber{ get; private set;}
        public int commandId{ get; private set;}
        public double roundServerTime{ get; private set;}

        /// <summary>
        /// server time, 
        /// </summary>
        /// <value>The server time.</value>
        public double serverTime{ get; private set;}
        /// <summary>
        /// 如果更新成功，那么就是更新的内容
        /// 如果验证失败，那么就是验证不通过的内容
        /// </summary>
        /// <value>The new data.</value>
        public Dictionary<string, byte[]> newData {
            get;private set;
        }

        /// <summary>
        /// 如果更新成功，那么就是成功删除的 keys
        /// 如果验证失败，那么就是在服务器中不存在的 keys
        /// </summary>
        /// <value>The deleted data.</value>
        public List<string> deletedKeys { get; private set; }



        /// <summary>
        /// Initializes a new instance of the <see cref="PVPSdk.RoomBroadcastCustomData"/> class.
        /// </summary>
        /// <param name="custom_data_number">Custom data number.</param>
        /// <param name="new_data">New data.</param>
        /// <param name="deleted_data">Deleted data.</param>
		internal NewCustomData (UInt64 memberPlayerId,int commandId, uint customDataNumber, List<Pair> newData, List<string> deletedKeys , double roundServerTime = -1, double serverTime = -1) {
            this.memberPlayerId = memberPlayerId;
            this.commandId = commandId;
            this.customDataNumber = customDataNumber;
            this.roundServerTime = roundServerTime;
            this.serverTime = serverTime;

            this.newData = new Dictionary<string, byte[]> ();
            foreach (Pair item in newData) {
                this.newData [item.key] = item.value;
            }
            this.deletedKeys = new List<string> ();
            this.deletedKeys.AddRange (deletedKeys);
        }
    }
}

