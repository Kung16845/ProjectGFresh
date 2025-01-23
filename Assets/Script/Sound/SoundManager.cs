using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    BGSound,
    MusicSound,
    VFXSound,
    Gunshot
}

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    public SoundType soundType;
    public float cooldown; // Minimum time between plays
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Sound List")]
    public List<Sound> sounds = new List<Sound>();
    private Dictionary<string, float> soundCooldowns = new Dictionary<string, float>();

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float bgVolume = 0.5f;
    [Range(0f, 1f)] public float musicVolume = 0.5f;
    [Range(0f, 1f)] public float vfxVolume = 0.5f;
    [Range(0f, 1f)] public float gunshotVolume = 0.5f;

    [Header("Audio Source Pool")]
    public int poolSize = 30;
    public int GunpoolSize = 30;
    private List<AudioSource> audioSourcePool;
    private List<AudioSource> vfxAudioSourcePool;
    private int currentSourceIndex = 0;
    private int vfxCurrentSourceIndex = 0;
    private AudioSource bgAudioSource;
    private AudioSource musicAudioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Dedicated sources for BG and Music
            bgAudioSource = gameObject.AddComponent<AudioSource>();
            musicAudioSource = gameObject.AddComponent<AudioSource>();

            // Initialize audio source pools
            audioSourcePool = new List<AudioSource>();
            for (int i = 0; i < GunpoolSize; i++)
            {
                AudioSource source = gameObject.AddComponent<AudioSource>();
                audioSourcePool.Add(source);
            }

            vfxAudioSourcePool = new List<AudioSource>();
            for (int i = 0; i < poolSize; i++)
            {
                AudioSource source = gameObject.AddComponent<AudioSource>();
                vfxAudioSourcePool.Add(source);
            }
        }
        else
        {
            Destroy(gameObject);
        }

        UpdateVolumes();
    }

    public void PlaySound(string name, float cooldownOverride = 0f)
    {
        Sound sound = sounds.Find(s => s.name == name);
        if (sound != null)
        {
            // Use either the defined cooldown or an override (like fire rate)
            float cooldown = cooldownOverride > 0f ? cooldownOverride : sound.cooldown;

            // Check cooldown
            if (soundCooldowns.TryGetValue(name, out float lastPlayedTime))
            {
                if (Time.time - lastPlayedTime < cooldown)
                {
                    return; // Too soon to play this sound again
                }
            }

            soundCooldowns[name] = Time.time; // Update last played time

            switch (sound.soundType)
            {
                case SoundType.BGSound:
                    bgAudioSource.clip = sound.clip;
                    bgAudioSource.volume = bgVolume;
                    bgAudioSource.Play();
                    break;
                case SoundType.MusicSound:
                    musicAudioSource.clip = sound.clip;
                    musicAudioSource.volume = musicVolume;
                    musicAudioSource.loop = true;
                    musicAudioSource.Play();
                    break;
                case SoundType.VFXSound:
                    PlayFromVFXPool(sound.clip, vfxVolume);
                    break;
                case SoundType.Gunshot:
                    PlayFromPool(sound.clip, gunshotVolume);
                    break;
            }
        }
        else
        {
            Debug.LogWarning($"Sound '{name}' not found!");
        }
    }


    public void UpdateVolumes()
    {
        bgAudioSource.volume = bgVolume;
        musicAudioSource.volume = musicVolume;

        foreach (var source in vfxAudioSourcePool)
        {
            source.volume = vfxVolume;
        }

        foreach (var source in audioSourcePool)
        {
            source.volume = gunshotVolume;
        }
    }

    private void PlayFromPool(AudioClip clip, float volume)
    {
        for (int i = 0; i < audioSourcePool.Count; i++)
        {
            int index = (currentSourceIndex + i) % audioSourcePool.Count;
            AudioSource source = audioSourcePool[index];

            if (!source.isPlaying)
            {
                source.clip = clip;
                source.volume = volume;
                source.Play();

                currentSourceIndex = (index + 1) % audioSourcePool.Count;
                return;
            }
        }

        Debug.LogWarning("All audio sources are busy. Sound could not be played.");
    }


    private void PlayFromVFXPool(AudioClip clip, float volume)
    {
        for (int i = 0; i < vfxAudioSourcePool.Count; i++)
        {
            int index = (vfxCurrentSourceIndex + i) % vfxAudioSourcePool.Count;
            AudioSource source = vfxAudioSourcePool[index];

            if (!source.isPlaying)
            {
                source.clip = clip;
                source.volume = volume;
                source.Play();

                vfxCurrentSourceIndex = (index + 1) % vfxAudioSourcePool.Count;
                return;
            }
        }

        Debug.LogWarning("All VFX audio sources are busy. Sound could not be played.");
    }


    public AudioSource GetAudioSourceForType(SoundType type)
    {
        switch (type)
        {
            case SoundType.VFXSound:
            case SoundType.Gunshot:
                return audioSourcePool[currentSourceIndex];
            case SoundType.BGSound:
                return bgAudioSource;
            case SoundType.MusicSound:
                return musicAudioSource;
            default:
                return null;
        }
    }
}


   
