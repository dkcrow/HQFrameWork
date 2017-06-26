using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PVPProtobuf;
using System.IO;
using ProtoBuf;
using PVPProtobuf_Token;

namespace PVPSdk {

    /// <summary>
    /// 房间信息，通过 PVPGlobal.room 获取
    /// </summary>
	public class Room {
        /// <summary>
        /// 房间内所有玩家在房间内的信息
        /// 房间内的用户数据是临时的，当离开房间后，这些数据都会清除
        /// </summary>
        public class MemberInfo {

            /// <summary>
            /// 玩家的UID
            /// </summary>
            public UInt64 playerId {
                get;
                private set;
            }

            /// <summary>
            /// 自定义信息自增量
            /// </summary>
            /// <value>The custom data number.</value>
            internal uint customDataNumber {
                get;
                private set;
            }

            /// <summary>
            /// 自定义信息
            /// 例如存放当前玩家在房间内的 坐标位置、下棋的步骤等
            /// 不要直接修改这个字典内容，请通过 Room.OpUpdateMemberCustomData 方法修改，数据传送到服务器上，由服务器响应修改
            /// </summary>
            public Dictionary<string, byte[]> customData {
                get;
                private set;
            }
                
            public enum Category : int
            {
                /// <summary>
                /// 玩家
                /// </summary>
                Fighter = 1,
                /// <summary>
                /// 观众
                /// </summary>
                Spectator = 2,
            }

            /// <summary>
            /// 类型，玩家还是观众
            /// </summary>
            /// <value>The category.</value>
            public Category category {
                get;
                private set;
            }

            /// <summary>
            /// 是否已经离开房间
            /// </summary>
            /// <value><c>true</c> if is leave; otherwise, <c>false</c>.</value>
            public bool isLeave {
                get;
                internal set;
            }

            internal MemberInfo (PVPProtobuf.RoomInfo.MemberInfo member) {
                this.playerId = member.player_id;
                this.customDataNumber = member.custom_data_number;
                this.category = (Category)member.category;
                this.customData = new Dictionary<string, byte[]>();
                foreach (PVPProtobuf.Pair item in member.custom_data) {
                    customData [item.key] = item.value;
                }

                this.isLeave = member.is_leave == 1;
            }

            /// <summary>
            /// 内部调用
            /// </summary>
            /// <param name="c">C.</param>
            internal void OnUpdateCustomData(PVP.NewCustomData c){
                this.customDataNumber = c.customDataNumber;
                this.customData.Clear ();


                foreach(KeyValuePair<String, byte[]> item in c.newData){// i=0;i<c.custom_data.Count;i++){
                    this.customData[item.Key] = item.Value;
                }
                foreach (string key in c.deletedKeys) {
                    if (this.customData.ContainsKey (key)) {
                        this.customData.Remove (key);
                    }
                }
            }
        }

        /// <summary>
        /// 房间ID
        /// </summary>
        public string roomId {
            get;
            private set;
        }
        /// <summary>
        /// 房间名称
        /// </summary>
        public string name {
            get;
            private set;
        }
        /// <summary>
        /// 最大的玩家数量
        /// </summary>
        /// <value>The max fighter number.</value>
        public int maxFighterNumber {
            get;
            private set;
        }

        /// <summary>
        /// 当前的玩家数量
        /// </summary>
        /// <value>The fighter number.</value>
        public int fighterNumber {
            get;
            private set;
        }
        /// <summary>
        /// 最大的观众数量，暂时不用
        /// </summary>
        /// <value>The max spectator number.</value>
        public int maxSpectatorNumber {
            get;
            private set;
        }

        /// <summary>
        /// 当前的观众数量
        /// </summary>
        /// <value>The spectator number.</value>
        public int spectatorNumber {
            get;
            private set;
        }

        /// <summary>
        /// 暂时无用
        /// </summary>
        /// <value><c>true</c> if is open; otherwise, <c>false</c>.</value>
        public bool isOpen {
            get;
            private set;
        }
        /// <summary>
        /// 暂时无用
        /// </summary>
        /// <value><c>true</c> if is visible; otherwise, <c>false</c>.</value>
        public bool isVisible {
            get;
            private set;
        }

        /// <summary>
        /// 房间内所有玩家的信息
        /// </summary>
        /// <value>The member infos.</value>
        public Dictionary<UInt64, MemberInfo> memberInfos {
            get;
            private set;
        }


        public Dictionary<string, byte[]> customData {
            get;
            private set;
        }

        /// <summary>
        /// 可以忽略
        /// </summary>
        /// <value>The custom data number.</value>
        internal uint customDataNumber {
            get;
            private set;
        }

        /// <summary>
        /// 可以忽略
        /// </summary>
        /// <value>The room data number.</value>
        internal uint roomDataNumber {
            get;
            private set;
        }

		/// <summary>
		/// 回合开始时间
		/// </summary>
		/// <value>The round server time.</value>
		public double roundServerTime {
			get;
			internal set;
		}

		/// <summary>
		/// 最近一次返回的服务器数据回包的服务器时间
		/// 不一定是当前时刻的服务器时间
		/// </summary>
		/// <value>The server time.</value>
		public double serverTime {
			get;
			internal set;
		}

        /// <summary>
        /// 当房间创建完毕后，服务器会根据玩家uid 列表进行一次随机排序
        /// 游戏可以根据这个字段，初始化玩家在比赛中位置排序等
        /// 例如一个麻将游戏，4个玩家的位置信息初始化，可以使用这个字段，当然游戏也可以自行决定位子安排
        /// </summary>
        /// <value>The random sequence.</value>
        public List<UInt64> randomSequence {
            get;
            private set;
        }

		internal Room(PVPProtobuf.RoomInfo roomInfo) {
			this.roomId = roomInfo.room_id;
			this.name = roomInfo.name;
            this.maxFighterNumber = roomInfo.max_fighter_number;
            this.fighterNumber = roomInfo.fighter_number;
            this.maxSpectatorNumber = roomInfo.max_spectator_number;
            this.spectatorNumber = roomInfo.spectator_number;
			this.isOpen = roomInfo.is_open;
			this.isVisible = roomInfo.is_visible;
            this.customData = new Dictionary<string, byte[]> ();
            this.customDataNumber = roomInfo.custom_data_number;
			this.roundServerTime = roomInfo.round_server_time;
			this.serverTime = roomInfo.server_time;
            foreach (PVPProtobuf.Pair item in roomInfo.custom_data) {
                this.customData [item.key] = item.value;
            }
            memberInfos = new Dictionary<UInt64, MemberInfo> ();

            foreach (PVPProtobuf.RoomInfo.MemberInfo member in roomInfo.member_infos) {
                MemberInfo member_info = new MemberInfo (member);
                memberInfos [member.player_id] = member_info;
			}
            this.randomSequence = new List<UInt64> ();
            this.randomSequence.AddRange (roomInfo.random_sequence);
		}

        /// <summary>
        /// 根据key 获取房间内自定义内容
        /// </summary>
        /// <returns>The custom data by key.</returns>
        /// <param name="key">Key.</param>
        public byte[] GetCustomDataByKey(string key){
            if (this.customData != null && this.customData.ContainsKey(key)) {
                return this.customData [key];
            }else{
                return null;
            }
        }

        /// <summary>
        /// 内部使用
        /// </summary>
        /// <param name="c">C.</param>
        internal void OnUpdateCustomData(PVP.NewCustomData c){
            this.customDataNumber = c.customDataNumber;
			this.roundServerTime = c.roundServerTime;
			this.serverTime = c.serverTime;
            foreach(KeyValuePair<String, byte[]> item in c.newData){// i=0;i<c.custom_data.Count;i++){
                this.customData[item.Key] = item.Value;
            }
            for (int i = 0; i < c.deletedKeys.Count; i++) {
                if (this.customData.ContainsKey (c.deletedKeys [i])) {
                    this.customData.Remove (c.deletedKeys [i]);
                }
            }
        }


        /// <summary>
        /// 在房间内发送消息，这个消息会广播到房间内所有其他的玩家
        /// 适合使用在数据比较频繁，而又可以丢失的数据，例如桌球比赛中，玩家的球杆实时移动，即使部分坐标位置数据丢失，也不影响后面的游戏效果
        /// </summary>
        /// <param name="command_id">自定义的消息分类</param>
        /// <param name="message">消息内容</param>
        public bool OpSendNewMessage (int commandId, byte[] message, int timeout=5)
        {
            return PVP.ICM.gameServerClient.SendNewMessage (commandId, message, 1, timeout);
        }

        /// <summary>
        /// 更新房间内的当前玩家的自定义信息，
        /// 服务器更新数据的判断逻辑和 OpUpdateRoomCustomData 一样，只是更新的是在房间内用户的自定义信息
        /// </summary>
        /// <returns><c>true</c>, if update room custom data was oped, <c>false</c> otherwise.</returns>
        /// <param name="updateRange">全量更新，还是增量更新. 全量更新表示把原有的内用全部清空，然后更新为提交的内容，增量更细是在已有的基础上更新</param>
        /// <param name="beingUpdatedData">需要更新的内容.</param>
        /// <param name="beingDeletedData">需要删除的keys.</param>
        /// <param name="checkData">检测的内容</param>
        /// <param name="checkDataNotExits">作用和 checkData 一样，检测要求是checkDataNotExits 的keys 在服务器上都不存在</param>
        /// <param name="timeout">Timeout.</param>
        public bool OpUpdateMemberCustomData (int commandId, PVP.UpdateCustomDataRange updateRange, Dictionary<String, byte[]> beingUpdatedData = null, List<string> beingDeletedData = null, Dictionary<String, byte[]> checkData = null, List<string> checkDataNotExits = null, int timeout = 5)
        {
            return PVP.ICM.gameServerClient.UpdateMemberCustomData (commandId, updateRange, beingUpdatedData, beingDeletedData, checkData, checkDataNotExits, timeout);
        }

        /// <summary>
        /// 更新房间内的公共自定义信息
        /// </summary>
        /// <returns><c>true</c>, if update room custom data was oped, <c>false</c> otherwise.</returns>
        /// <param name="updateRange">全量更新，还是增量更新. 全量更新表示把原有的内用全部清空，然后更新为提交的内容，增量更细是在已有的基础上更新</param>
        /// <param name="beingUpdatedData">需要更新的内容.</param>
        /// <param name="beingDeletedData">需要删除的keys.</param>
        /// <param name="checkData">检测的内容，例如在打怪游戏中，多个玩家提交判定自己消灭怪物的数据更新，
        /// 而如果消灭怪物只能由一个玩家完成，那么可以在 checkData 增加怪物之前的状态（没有死亡），服务器接收到数据后，比较怪物字段，
        /// 最终只有一个玩家可以更新数据成功 </param>
        /// <param name="checkDataNotExits">作用和 checkData 一样，检测要求是checkDataNotExits 的keys 在服务器上都不存在</param>
        /// <param name="timeout">Timeout.</param>
        public bool OpUpdateRoomCustomData (int commandId, PVP.UpdateCustomDataRange updateRange, Dictionary<String, byte[]> beingUpdatedData = null, List<string> beingDeletedData = null, Dictionary<String, byte[]> checkData = null, List<string> checkDataNotExits = null, int updateServerTime = 0, int timeout = 5)
        {
            return PVP.ICM.gameServerClient.UpdateRoomCustomData (commandId, updateRange, beingUpdatedData, beingDeletedData, checkData, checkDataNotExits,updateServerTime, timeout);
        }


        /// <summary>
        /// 离开当前房间，
        /// 注意如果离开房间成功，系统会自动设置 PVPGlobal.room 为 null 值
        /// </summary>
        /// <returns><c>true</c>, if leave room was oped, <c>false</c> otherwise.</returns>
        /// <param name="timeout">Timeout.</param>
        public bool OpLeaveRoom (int timeout = 5){
            return PVP.ICM.gameServerClient.LeaveRoom ();
        }


        internal void OnOtherMemberLeave(UInt64 memberUid){
            this.DelMember (memberUid);
        }


        internal void DelMember(UInt64 memberPlayerId){
            if (this.memberInfos.ContainsKey (memberPlayerId)) {
                MemberInfo m = this.memberInfos[memberPlayerId];
                if (!m.isLeave) {
                    m.isLeave = true;
                    if (m.category == MemberInfo.Category.Fighter) {
                        this.fighterNumber--;
                    } else {
                        this.spectatorNumber--;
                    }
                }
            }
        }
    }
}