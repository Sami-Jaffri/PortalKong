using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] private SoundGroup[] soundGroups;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource soundfxSource;
    private Dictionary<string, List<AudioClip>> soundLibrary;

    [System.Serializable]
    public struct SoundGroup
    {
        public string name;
        public List<AudioClip> audioClips;
    }

    void Awake()
    {
        musicSource.volume = PlayerPrefs.GetFloat("musicVolume");
        soundfxSource.volume = PlayerPrefs.GetFloat("soundfxVolume");
        InitializeLibrary();
        PlayMusic("BackgroundMusic");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeLibrary()
    {
        soundLibrary = new Dictionary<string, List<AudioClip>>();
        foreach (SoundGroup soundGroup in soundGroups)
        {
            soundLibrary[soundGroup.name] = soundGroup.audioClips;
        }
    }

    public AudioClip GetRandomAudio(string soundName)
    {
        if (soundLibrary.ContainsKey(soundName))
        {
            List<AudioClip> soundList = soundLibrary[soundName];
            if (soundList.Count > 0)
            {
                return soundList[Random.Range(0, soundList.Count)];
            }
        }
        return null;
    }

    public void PlayMusic(string name)
    {
        AudioClip audioClip = GetRandomAudio(name);
        if (audioClip != null)
        {
            musicSource.PlayOneShot(audioClip);
        }
    }
    public void PlayAudioClip(string name)
    {
        AudioClip audioClip = GetRandomAudio(name);
        if (audioClip != null)
        {
            soundfxSource.PlayOneShot(audioClip);
        }
    }
}
