
using HQFrameWork;
using UnityEngine;

namespace HQFrameWork
{
    [System.Serializable]
    public class UISoundInfo
    {
        [CsvTitle("音效类型")] public string type = "";
        [CsvTitle("声音文件")] public string soundPath = "";
    }

    public enum AudioBusEnum
    {
        [StringValue("NO BUS")] NoBus,

        [StringValue("Partner")] Partner,
        [StringValue("PartnerSkill")] PartnerSkill,
        [StringValue("PartnerSkillHit")] PartnerSkillHit,
        [StringValue("PartnerRun")] PartnerRun,
        [StringValue("PartnerDeath")] PartnerDeath,

        [StringValue("Monster")] Monster,
        [StringValue("MonsterSkill")] MonsterSkill,
        [StringValue("MonsterSkillHit")] MonsterSkillHit,

        [StringValue("Boss")] Boss,

        [StringValue("Global")] Global,
    }
    /// <summary>
    /// 所有音效放在这里
    /// </summary>
    public enum SoundEnum
    {
        [StringValue("0")] NONE,
        [StringValue("1")] CONFIRM,//确定
        [StringValue("2")] CANCEL,//取消
        [StringValue("3")] SUCCESS,
        [StringValue("4")] FAIL, 
        
    }

    public enum ELevelBGM
    {
        [StringValue("BGM_Login")] //没有实体场景
        STAGE_DOWNLOAD,
        [StringValue("BGM_Login")] INIT,
        [StringValue("BGM_Login")] LOGIN,
        [StringValue("BGM_Login")] CREATE_OR_SELECT_HERO,
        [StringValue("BGM_GameScene")] Battle,
    }

    public class HQAudioPlayer : MBSingleton<HQAudioPlayer>
    {
        // UI界面音效
        public void Play(SoundEnum soundType)
        {
            string soundID = soundType.GetStringValue();
            if (string.IsNullOrEmpty(soundID) || soundID.Equals("0"))
            {
                return;
            }

            UISoundInfo uiSoundInfo = null;
            if (GameDBConfig.Instance.m_dicUISound.TryGetValue(soundID, out uiSoundInfo))
            {
                SoundManager.PlaySFX(uiSoundInfo.soundPath);
            }
            else
            {
                Debug.Log("未找到音频文件 soundID == " + soundID);
            }
        }

        // 技能音效
        public void PlaySkill(uint soundId, Transform effTrm)
        {
            AudioData data = null;
            GameDBConfig.Instance.m_dicAudioData.TryGetValue(soundId, out data);
            if (data != null)
            {
                PlayAudio(data.Name, effTrm);
            }
        }

        // 播放声音
        // 从sounds_list中读取音效文件
        public void PlaySoundById(uint soundId, Transform trm)
        {
            AudioData data = null;
            GameDBConfig.Instance.m_dicAudioData.TryGetValue(soundId, out data);
            if (data != null)
            {
                if (trm != null)
                {
                    PlayAudio(data.Name, trm);
                }
                else
                    PlayAudio(data.Name, null, false);
            }
        }

        /// <summary>
        /// 播放游戏音效，区分是否是3D音效
        /// </summary>
        /// <param name="audioName"></param>
        /// <param name="effTrm"></param>
        /// <param name="is3D"></param>
        public void PlayAudio(string audioName, Transform effTrm = null, bool is3D = true)
        {
            if (!string.IsNullOrEmpty(audioName))
            {
                if (effTrm != null)
                {
                    SoundManager.PlaySFX(effTrm.gameObject, audioName);
                }
                else
                {
                    SoundManager.PlaySFX(audioName);
                }

            }
        }


        /// <summary>
        /// 播放背景音乐
        /// </summary>
        /// <param name="levelName"></param>
        public void PlayBGM(int soundId)
        {
            return;
            string levelName = soundId.ToString();
            if (SoundManager.GetCurrentSoundConnection() != null)
            {
                if (!SoundManager.GetCurrentSoundConnection().level.Equals(levelName))
                {
                    SoundManager.PlayConnection(levelName);
                }

            }
            else
            {
                SoundManager.PlayConnection(levelName);
            }
        }

        // 开启或关闭背景音乐
        public void ToggleBGM(bool isPlay)
        {
            SoundManager.MuteMusic(!isPlay);
        }

        // 开启或关闭SFX
        public void ToggleSFX(bool isPlay)
        {
            SoundManager.MuteSFX(!isPlay);
        }

        // 开启或关闭所有音效
        public void ToggleSound(bool value)
        {
            SoundManager.Mute(value);
            //SoundManager.Pause();
            //SoundManager.UnPause();


        }


        // 设置SFX音量
        public void SetVolumeSFX(float value)
        {
            SoundManager.SetVolumeSFX(value);
        }

        // 设置背景音乐音量
        public void SetVolumeBGM(float value)
        {
            SoundManager.SetVolumeMusic(value);
        }

    }
}


