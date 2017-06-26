namespace PVPSdk.PVP
{
    public class ErrorCode
    {
        public const int SUCCESS = 0;

        public const int UNKNOWN = 1;
        public const int SERVICE_ERROR = 2;
        public const int METHOD_UNKNOWN = 3;
        public const int TOO_MANY_CALLS = 4;
        public const int BAD_IP = 5;
        public const int DB_ERROR = 6;
        public const int IS_CHEAT = 7;
        public const int NEED_LOGIN = 8;
        public const int ACCOUNT_OR_PASSWORD_ERROR = 9;
        public const int DATA_ERROR = 11;
        public const int CP_LIMIT = 12;
        public const int VERSION_IS_OLD = 13;
        public const int APP_NOT_EXIST = 14;
        public const int CHECK_SUM_ERROR = 15;
        public const int CHECK_TOKEN_ERROR = 16;

        public const int PARAM_ERROR = 100;
        public const int PARAM_TOKEN = 102;
        public const int INVALID_MOBILE = 103;
        public const int INVALID_EMAIL = 104;
        public const int CAN_NOT_REQUEST = 106;

        public const int LOBBY_GET_LIST_ERROR = 200;
        public const int LOBBY_NOT_IN_LOBBY_ERROR = 201;
        public const int LOBBY_ID_NOT_EXIST_ERROR = 202;
        public const int LOBBY_MATCH_OPPONENT_TIMEOUT = 203;
        public const int LOBBY_MATCH_OPPONENT_ALREADY_IN_ROOM = 204;

        public const int USER_NO_EXIST = 10001;
        public const int USER_PASSWORD_ERROR = 10002;
        public const int USER_ACCOUNT_REPEATED = 10003;
        public const int USER_STATE_NOT_NORMAL = 10004;
        public const int USER_PASSWORD_FORMAT_ERROR = 10005;

        /**
         * 登陆方式错误，不支持
         */
        public const int USER_LOGIN_TYPE_ERROR = 10006;
        public const int USER_LOGIN_FACEBOOK_ACCESS_TOKEN_ERROR = 10007;
        public const int USER_REGISTER_FAILED = 10008;
        public const int USER_LOGIN_GUEST_ACCESS_TOKEN_ERROR = 10009;

        public const int ROOM_NOT_EXIST = 11001;
        public const int ROOM_CACHE_MESSAGE_NUMBER_NOT_EXIST = 11002;
        public const int ROOM_NOT_IN_ROOM_ERROR = 11003;
        public const int ROOM_TARGET_UID_NOT_IN_ROOM_ERROR = 11004;

        /// <summary>
        /// 不用
        /// </summary>
        public const int ROOM_NO_MORE_ROOM_ERROR = 11005;
        public const int ROOM_UPDATE_MEMBER_CHECK_DATA_NOT_PASS = 11006;
        public const int ROOM_UPDATE_ROOM_CHECK_DATA_NOT_PASS = 11007;

        public const int APPUSER_CHECK_NOT_PASS = 12001;


        public const int LEADERBOARD_ID_NOT_EXIST = 13001;
        public const int LEADERBOARD_ID_SUBMIT_REPEATEDLY = 13002;


        public const int SOCIAL_NOT_FACEBOOK_USER = 13101;
        public const int SOCIAL_NO_CHALLENGE = 13102;
        public const int SOCIAL_REFUSE_CHALLENGE = 13103;
        public const int SOCIAL_TARGET_PLAYER_NOT_EXIST = 13104;
        public const int SOCIAL_IS_FRIENDED = 13105;
        public const int SOCIAL_YOU_CAN_NOT_BE_YOUR_TARGET = 13106;

        //客户端直接反馈的错误

        /// <summary>
        /// 业务层数据响应超时
        /// </summary>
        public const int RESPONSE_TIME_OUT = 90000;

        /// <summary>
        /// 网络层错误，连接失败
        /// </summary>
        public const int NETWORK_ERROR = 90001;
    }
}

