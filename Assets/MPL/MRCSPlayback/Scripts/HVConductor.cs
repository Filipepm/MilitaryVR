using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Keeps track of HoloVideoObjects that are potentially going to play soon
/// and loads them asynchronously.
/// An object pool is not used to allow user to use the editor to control
/// the transforms of the objects. User should be wary of reusing
/// HoloVideoObjects between adjacent sequences.
/// </summary>
public class HVConductor : MonoBehaviour
{
    [System.Serializable]
    public struct HVSequence
    {
        public List<HoloVideoObject> VideosToLoad;
    }

    public bool ShouldLoopSequences;
    [SerializeField]
    private List<HVSequence> sequenceList;
    [SerializeField]
    private int currSequence = 0;

    void Start()
    {
        foreach (var video in sequenceList[currSequence].VideosToLoad)
        {
            if (video)
            {
                if (!video.ShouldAutoPlay)
                {
                    video.Open(video.Url);
                    video.Play();
                }
            }
        }
        LoadNextSequence();
    }

    void LoadNextSequence()
    {
        if (currSequence < sequenceList.Count - 1 || ShouldLoopSequences)
        {
            foreach (var video in sequenceList[(currSequence + 1) % sequenceList.Count].VideosToLoad)
            {
                if (video)
                {
#if (NET_4_6 || USE_ASYNC)
                    video.OpenAsync();
#else
                    video.Open(video.Url);
#endif
                }
            }
        }
    }

    public void GoToNextSequence()
    {
        if (currSequence >= sequenceList.Count && !ShouldLoopSequences)
        {
            Debug.LogWarning("End of sequence list reached; index: " + currSequence);
            return;
        }

        foreach (var video in sequenceList[currSequence].VideosToLoad)
        {
            if (video)
            {
#if (NET_4_6 || USE_ASYNC)
                video.CloseAsync();
#else
                video.Close();
#endif
            }
        }

        ++currSequence;
        if (ShouldLoopSequences)
        {
            currSequence %= sequenceList.Count;
        }
        if (currSequence >= sequenceList.Count && !ShouldLoopSequences)
        {
            return;
        }
        foreach (var video in sequenceList[currSequence].VideosToLoad)
        {
            if (video)
            {
                video.Play();
            }
        }
        LoadNextSequence();
    }

    void Update()
    {
    }

    public void AddVideoToSequence(int sequence, HoloVideoObject video)
    {
        sequenceList[sequence].VideosToLoad.Add(video);
        if (sequence == currSequence + 1 ||
            (ShouldLoopSequences && sequence == 0 && currSequence == sequenceList.Count - 1))
        {
#if (NET_4_6 || USE_ASYNC)
            video.OpenAsync();
#endif
        }
    }
}
