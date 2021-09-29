using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System.Linq;
using System;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{

    public Sound[] sounds;

    public static AudioManager instance = null;
    public AudioSource source;
    public AudioMixerGroup masterMixer;
    public AudioMixer musicMixer;
    public AudioMixer sfxMixer;
    public Coroutine fadeCoroutine;
    [HideInInspector] public string musicPlaying; 
    
    // Start is called before the first frame update
    void Awake()
    {
        
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(gameObject);
        
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.outputAudioMixerGroup = s.mixerGroup;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.ignoreListenerPause = s.ignoreListenerPause;
        }   
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name.Equals("MenuSong"))
        {
            PlaySong("MenuSong");  
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        
        s.source.Play();
    }

    public void PlayClipAtGameObject(string name, GameObject gameObject, bool loop, float minDist, float maxDist)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        s.source = gameObject.AddComponent<AudioSource>();
        s.source.clip = s.clip;
        s.source.volume = s.volume;
        s.source.loop = loop;
        s.source.spatialBlend = 1;
        s.source.rolloffMode = AudioRolloffMode.Linear;
        s.source.minDistance = minDist;
        s.source.maxDistance = maxDist;
        s.source.Play();
    }
    
    public void PlayOneShot(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        
        s.source.PlayOneShot(s.clip);
    }

    public void PlaySong(string name)
    {
        if (musicPlaying != "")
        {
            if (IfSoundIsPlaying(musicPlaying) && !musicPlaying.Equals(name)) //if song is playing and it's not the same song
            {
                Stop(musicPlaying);
            }  
        }

        musicPlaying = name;
        Play(name);
    }
    
    public void FadeInSound(string name, float duration)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        if (fadeCoroutine != null) //if it's not the first time
        {
            StopCoroutine(fadeCoroutine);  
        }
        else
        {
            s.source.volume = 0;
        }

        float targetVolume = s.volume;

        s.source.Play();
        fadeCoroutine = StartCoroutine(StartFade(s.source, duration, targetVolume));
    }
    
    public void FadeOutSound(string name, float duration)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        StopCoroutine(fadeCoroutine);
        float targetVolume = 0;

        s.source.Play();
        fadeCoroutine = StartCoroutine(StartFade(s.source, duration, targetVolume));
    }
    
    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Stop();
    }

    public void PlayRandomBetweenSounds(string[] names)
    {
        Play(names[Random.Range(0, names.Length)]);
    }
    
    public void PlayOneShotRandomBetweenSounds(string[] names)
    {
        PlayOneShot(names[Random.Range(0, names.Length)]);
    }
    
    public bool IfSoundIsPlaying(string soundName)
    {
        Sound s = Array.Find(sounds, sound => sound.name == soundName);

        return s.source.isPlaying;
    }

    public void SetSoundVolume(string soundName, float volume)
    {
        Sound s = Array.Find(sounds, sound => sound.name == soundName);

        s.source.volume = volume;
    }

    public float GetAudioLength(string soundName)
    {
        Sound s = Array.Find(sounds, sound => sound.name == soundName);

        return s.source.clip.length;
    }

    public void PlaySoundAfterAnother(string firstSoundName, string secondSoundName, float pauseDuration)
    {
        Play(firstSoundName);
        StartCoroutine(PlaySoundAfterSeconds(secondSoundName, pauseDuration));
    }
    
    public static IEnumerator StartFade(AudioSource audioSource, float duration, float targetVolume)
    {
        float currentTime = 0;
        float start = audioSource.volume;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        yield break;
    }

    public static IEnumerator PlaySoundAfterSeconds(string soundName, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        instance.Play(soundName);
    }
}
