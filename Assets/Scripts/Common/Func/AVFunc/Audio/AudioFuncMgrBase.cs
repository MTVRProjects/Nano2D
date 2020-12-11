namespace HMLFramwork.AV
{
    public class AudioFuncMgrBase : AVFuncMgrBase
    {
        protected override void Awake()
        {
            base.Awake();
        }
        /// <summary>
        /// 静音
        /// </summary>
        public override void Mute()
        {
            foreach (var item in targets)
            {
                item.Value.Mute();
            }
        }

        public override void Pause()
        {
            base.Pause();
        }

        public override void UnPause()
        {
            base.UnPause();
        }
        public override void Init()
        {
            base.Init();
        }

        public override void Stop()
        {
            base.Stop();
        }

        public override void RecoverVoice()
        {
            base.RecoverVoice();
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
        }
        
    }
}