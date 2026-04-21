using UnityEngine;

namespace ManagmentScripts.SoundScripts
{
    public class AudioPool : SimplePool<AudioPool, AudioSourcePoolable>
    {
        public static new AudioPool Instance
        {
            get
            {
                return (AudioPool)SimplePool<AudioPool, AudioSourcePoolable>.Instance;
            }
        }
    }
}