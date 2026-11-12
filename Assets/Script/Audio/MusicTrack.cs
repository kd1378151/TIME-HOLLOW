using UnityEngine;

namespace TimeHollow.Audio
{
    [CreateAssetMenu(menuName = "TimeHollow/Music Track")]
    public sealed class MusicTrack : ScriptableObject
    {
        [SerializeField] private AudioClip clip;

        public AudioClip Clip => clip;
    }
}
