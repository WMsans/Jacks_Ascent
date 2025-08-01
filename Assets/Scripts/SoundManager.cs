using UnityEngine;
using FMODUnity;
using FMOD.Studio;

[CreateAssetMenu(fileName = "SoundManager", menuName = "Managers/SoundManager")]
public class SoundManager : ScriptableObject
{
    private static SoundManager instance;

    public static SoundManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<SoundManager>("SoundManager");
            }
            return instance;

        }
    }

    // Serialized EventReferences for one-shots and persistent sounds
    [Header("Player SFX")]
    [SerializeField] private EventReference grabRef;
    [SerializeField] private EventReference releaseRef;
    [SerializeField] private EventReference pullRef;
    [SerializeField] private EventReference inRef;
    [SerializeField] private EventReference fireRef;
    [SerializeField] private EventReference landingRef;

    [Header("Environment & Ambience")]
    [SerializeField] private EventReference backgroundRef;
    [SerializeField] private EventReference eggBurstRef;
    [SerializeField] private EventReference introRef;

    [Header("Creature SFX")]
    [SerializeField] private EventReference centipedeBiteRef;
    [SerializeField] private EventReference centipedeDeathRef;
    [SerializeField] private EventReference centipedeFlyRef;
    [SerializeField] private EventReference lanternFlyRef;
    [SerializeField] private EventReference mouthAttackRef;
    [SerializeField] private EventReference mouthDetectRef;
    [SerializeField] private EventReference mouthIdleRef;
    [SerializeField] private EventReference mouthImpactRef;
    [SerializeField] private EventReference spiderCrawlRef;
    [SerializeField] private EventReference spiderCrawlWebRef;
    [SerializeField] private EventReference spiderHitRef;

    // EventInstances for persistent sounds
    private EventInstance ambienceEvent;
    private EventInstance flyEvent;
    private EventInstance lanternFlyEvent;
    private EventInstance spiderCrawlEvent;
    private EventInstance spiderCrawlWebEvent;
    private EventInstance introEvent;

    // Helper method to create an instance if it's not valid
    private void CreateInstanceIfNeeded(ref EventInstance eventInstance, EventReference eventReference)
    {
        if (!eventInstance.isValid())
        {
            eventInstance = RuntimeManager.CreateInstance(eventReference);
        }
    }

    public void SpiderCrawlStart()
    {
        CreateInstanceIfNeeded(ref spiderCrawlEvent, spiderCrawlRef);
        spiderCrawlEvent.start();
    }
    public void SpiderCrawlStop()
    {
        if (spiderCrawlEvent.isValid())
        {
            spiderCrawlEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    public void SpiderCrawlWebStart()
    {
        CreateInstanceIfNeeded(ref spiderCrawlWebEvent, spiderCrawlWebRef);
        spiderCrawlWebEvent.start();
    }

    public void SpiderCrawlWebStop()
    {
        if (spiderCrawlWebEvent.isValid())
        {
            spiderCrawlWebEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    public void PlaySpiderHitSFX() => RuntimeManager.PlayOneShot(spiderHitRef);
    public void MouthImpact() => RuntimeManager.PlayOneShot(mouthImpactRef);
    public void MouthIdle() => RuntimeManager.PlayOneShot(mouthIdleRef);
    public void MouthDetect() => RuntimeManager.PlayOneShot(mouthDetectRef);
    public void MouthAttackSFX() => RuntimeManager.PlayOneShot(mouthAttackRef);
    public void LandingSFX() => RuntimeManager.PlayOneShot(landingRef);

    public void LanternFlyStart()
    {
        CreateInstanceIfNeeded(ref lanternFlyEvent, lanternFlyRef);
        lanternFlyEvent.start();
    }

    public void LanternFlyEnd()
    {
        if (lanternFlyEvent.isValid())
        {
            lanternFlyEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    public void PlayEggBurstSFX() => RuntimeManager.PlayOneShot(eggBurstRef);
    public void PlayGrabSFX() => RuntimeManager.PlayOneShot(grabRef);
    public void PlayReleaseSFX() => RuntimeManager.PlayOneShot(releaseRef);
    public void PlayFireSFX() => RuntimeManager.PlayOneShot(fireRef);
    public void PlayPullSFX() => RuntimeManager.PlayOneShot(pullRef);
    public void PlayInSFX() => RuntimeManager.PlayOneShot(inRef);

    public void BackgroundStart()
    {
        CreateInstanceIfNeeded(ref ambienceEvent, backgroundRef);
        ambienceEvent.start();
        ambienceEvent.setParameterByName("IsChasing", 0);
        ambienceEvent.setParameterByName("IsDetected", 0);
        ambienceEvent.setParameterByName("IsFalling", 0);
    }

    public void BackgroundStop()
    {
        if (ambienceEvent.isValid())
        {
            ambienceEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    public void FlyStart()
    {
        CreateInstanceIfNeeded(ref flyEvent, centipedeFlyRef);
        flyEvent.start();
    }

    public void FlyStop()
    {
        if (flyEvent.isValid())
        {
            flyEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    public void PlayCentipedeBiteSFX() => RuntimeManager.PlayOneShot(centipedeBiteRef);
    public void PlayCentipedeDeathSFX() => RuntimeManager.PlayOneShot(centipedeDeathRef);
    public void IntroPlay()
    {
        CreateInstanceIfNeeded(ref introEvent, introRef);
        introEvent.start(); 
    }
    public void IntroStop()
    {
        if (introEvent.isValid())
        {
            introEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    // Call this when the ScriptableObject is unloaded to release FMOD resources
    private void OnDisable()
    {
        ambienceEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        ambienceEvent.release();
        flyEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        flyEvent.release();
        lanternFlyEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        lanternFlyEvent.release();
        spiderCrawlEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        spiderCrawlEvent.release();
        spiderCrawlWebEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        spiderCrawlWebEvent.release();
    }
}