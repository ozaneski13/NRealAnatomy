using NRKernal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerScript : MonoBehaviour
{
    [SerializeField] private List<VideoClip> videoClipList;
    [SerializeField] private MeshRenderer meshRenderer;
    private GameObject playerCam;

    private List<VideoPlayer> videoPlayerList = null;
    private int videoIndex = 0;

    private int nextIndex = 0;

    private bool canStartAgain = false;

    void Awake()
    {
        playerCam = FindObjectOfType<NRMultiDisplayManager>().gameObject;
    }

    void Start()
    {
        StartCoroutine(playVideo());
    }

    private void Update()
    {
        Vector3 tempVector = playerCam.transform.position - transform.position;
        tempVector.y = 0;

        Quaternion rot = Quaternion.LookRotation(tempVector);
        transform.rotation = rot;

        if(canStartAgain)
            StartCoroutine(playVideo());
    }

    IEnumerator playVideo(bool firstRun = true)
    {
        if (videoClipList == null || videoClipList.Count <= 0)
        {
            Debug.LogError("Assign VideoClips from the Editor");
            yield break;
        }

        if (firstRun)
        {
            if (videoPlayerList != null)
                foreach (VideoPlayer vp in videoPlayerList)
                 Destroy(vp.gameObject);

            canStartAgain = false;
            videoIndex = 0;
            nextIndex = 0;
        }

        //Init videoPlayerList first time this function is called
        if (firstRun)
        {
            videoPlayerList = new List<VideoPlayer>();

            for (int i = 0; i < videoClipList.Count; i++)
            {
                //Create new Object to hold the Video and the sound then make it a child of this object
                GameObject vidHolder = new GameObject("VP" + i);

                vidHolder.transform.SetParent(transform);
                vidHolder.transform.position = transform.position;
                vidHolder.transform.rotation = transform.rotation;

                //Add VideoPlayer to the GameObject
                VideoPlayer videoPlayer = vidHolder.AddComponent<VideoPlayer>();
                videoPlayerList.Add(videoPlayer);

                //Add AudioSource to  the GameObject
                AudioSource audioSource = vidHolder.AddComponent<AudioSource>();

                //Disable Play on Awake for both Video and Audio
                videoPlayer.playOnAwake = false;
                audioSource.playOnAwake = false;

                //We want to play from video clip not from url
                videoPlayer.source = VideoSource.VideoClip;

                //Set Audio Output to AudioSource
                videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;

                //Assign the Audio from Video to AudioSource to be played
                videoPlayer.EnableAudioTrack(0, true);
                videoPlayer.SetTargetAudioSource(0, audioSource);

                //Set video Clip To Play 
                videoPlayer.clip = videoClipList[i];

                videoPlayer.renderMode = VideoRenderMode.MaterialOverride;
                videoPlayer.targetMaterialRenderer = meshRenderer;
            }
        }

        //Make sure that the NEXT VideoPlayer index is valid
        if (videoIndex >= videoPlayerList.Count)
            yield break;

        //Prepare video
        videoPlayerList[videoIndex].Prepare();

        //Wait until this video is prepared
        while (!videoPlayerList[videoIndex].isPrepared)
        {
            Debug.Log("Preparing Video");
            //Prepare/Wait for 5 sceonds only
            yield return new WaitForSeconds(1f);
            //Break out of the while loop after 5 seconds wait
            break;
        }

        Debug.LogWarning("Done Preparing current Video Index: " + videoIndex);

        if (nextIndex == videoPlayerList.Count) 
        {
            Debug.LogWarning("End of All Videos: " + videoIndex);
            videoIndex = 0;
            nextIndex = 0;
            canStartAgain = true;

            yield break;
        }

        //Play first video
        videoPlayerList[videoIndex].Play();

        //Wait while the current video is playing
        nextIndex = (videoIndex + 1);

        while (videoPlayerList[videoIndex].isPlaying)
        {
            //Debug.Log("Playing time: " + videoPlayerList[videoIndex].time + " INDEX: " + videoIndex);

            //(Check if we have reached half way)
            if (videoPlayerList[videoIndex].time >= (videoPlayerList[videoIndex].clip.length)) 
            {
                //Make sure that the NEXT VideoPlayer index is valid. Othereise Exit since this is the end
                if (nextIndex >= videoPlayerList.Count)
                {
                    Debug.LogWarning("End of All Videos: " + videoIndex);
                    videoIndex = 0;
                    nextIndex = 0;
                    canStartAgain = true;

                    yield break;
                }

                //Prepare the NEXT video
                Debug.LogWarning("Ready to Prepare NEXT Video Index: " + nextIndex);
                videoPlayerList[nextIndex].Prepare();
            }
            yield return null;
        }
        Debug.Log("Done Playing current Video Index: " + videoIndex);

        if (nextIndex == videoPlayerList.Count)
        {
            Debug.LogWarning("End of All Videos: " + videoIndex);
            videoIndex = 0;
            nextIndex = 0;
            canStartAgain = true;

            yield break;
        }

        //Wait until NEXT video is prepared
        while (!videoPlayerList[nextIndex].isPrepared)
        {
            Debug.Log("Preparing NEXT Video Index: " + nextIndex);

            yield return new WaitForSeconds(1f);
            //Break out of the while loop after 5 seconds wait
            break;
        }

        Debug.LogWarning("Done Preparing NEXT Video Index: " + videoIndex);

        //Increment Video index
        videoIndex++;

        //Play next prepared video. Pass false to it so that some codes are not executed at-all
        StartCoroutine(playVideo(false));
    }
}