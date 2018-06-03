using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;


/// <summary>
/// Unity VideoPlayer Script for Unity 5.6 (currently in beta 0b11 as of March 15, 2017)
/// Blog URL: http://justcode.me/unity2d/how-to-play-videos-on-unity-using-new-videoplayer/
/// YouTube Video Link: https://www.youtube.com/watch?v=nGA3jMBDjHk
/// StackOverflow Disscussion: http://stackoverflow.com/questions/41144054/using-new-unity-videoplayer-and-videoclip-api-to-play-video/
/// Code Contiburation: StackOverflow - Programmer
/// </summary>



public class video : MonoBehaviour
{

    public RawImage image;
	public string url;
    public VideoClip videoToPlay;

    private VideoPlayer videoPlayer;
    private VideoSource videoSource;

    private AudioSource audioSource;
    public Material planeMaterial;
    // Use this for initialization
    void Start()
    {
        Application.runInBackground = true;
        StartCoroutine(playVideo());
        planeMaterial = GetComponent<Renderer>().material;

    }

    IEnumerator playVideo()
    {
        //Add VideoPlayer to the GameObject
        videoPlayer = gameObject.AddComponent<VideoPlayer>();

        //Add AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();

        //Disable Play on Awake for both Video and Audio
        videoPlayer.playOnAwake = false;
        
        audioSource.playOnAwake = false;
        audioSource.Pause();

        //We want to play from video clip not from url

        videoPlayer.source = VideoSource.VideoClip;

        // Vide clip from Url
        videoPlayer.source = VideoSource.Url;
        //videoPlayer.url = "https://r7---sn-poqvn5u-jb36.googlevideo.com/videoplayback?mm=31%2C29&sparams=dur%2Cei%2Cid%2Cip%2Cipbits%2Citag%2Clmt%2Cmime%2Cmm%2Cmn%2Cms%2Cmv%2Cpl%2Cratebypass%2Crequiressl%2Csource%2Cexpire&mn=sn-poqvn5u-jb36%2Csn-npoeen7r&ipbits=0&signature=4079F69BE204FEB655BA0E9AEFE086338FCD7811.6954B5DC5D4E138B29ECEDD66374A52EF657CBDC&mime=video%2Fmp4&lmt=1507357906870552&requiressl=yes&mt=1527944793&expire=1527966504&dur=507.564&mv=m&ei=yJYSW7_oMNSNoAP1woyQBQ&ms=au%2Crdu&source=youtube&pl=24&ratebypass=yes&key=yt6&ip=101.50.1.2&fvip=3&c=WEB&itag=22&id=o-AAOD6Slh9lDcnhTEcabPHMmpkYi_xd-MVtlGwGA-FLop&video_id=blsvq-YtjNg&title=Forza+Motorsport+7+-+Gameplay+%28HD%29+%5B1080p60FPS%5D";
        //"https://r3---sn-jgxqvapo3-axqe.googlevideo.com/videoplayback?c=WEB&id=o-ACGE9lTXGBnlIfQnXyqGfcxePT4Ls8BOATU70H2ZucUj&fvip=16&requiressl=yes&ip=77.247.161.182&signature=16434F8A90290BBDD9929DE1160C90622B309515.41DD608E06C1CCE4241C7BF2547999CE8393E44D&dur=216.711&ratebypass=yes&source=youtube&lmt=1497611614602453&key=yt6&initcwndbps=1212500&nh=%2CIgphcjAxLmxlZDAzKgkxMjcuMC4wLjE&ipbits=0&expire=1527980379&pl=20&mime=video%2Fmp4&mv=m&mt=1527958653&ms=au%2Crdu&itag=22&ei=-8wSW_7sG8_Z7ATEqKT4AQ&sparams=dur%2Cei%2Cid%2Cinitcwndbps%2Cip%2Cipbits%2Citag%2Clmt%2Cmime%2Cmm%2Cmn%2Cms%2Cmv%2Cnh%2Cpl%2Cratebypass%2Crequiressl%2Csource%2Cexpire&mn=sn-jgxqvapo3-axqe%2Csn-axq7sn7z&mm=31%2C29&video_id=V81xOk_MtFg&title=Forza+7+-+Gameplay+E3+2017";
		videoPlayer.url = url;//"http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4";


        //Set Audio Output to AudioSource
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;

        //Assign the Audio from Video to AudioSource to be played
        videoPlayer.EnableAudioTrack(0, true);
        videoPlayer.SetTargetAudioSource(0, audioSource);

        //Set video To Play then prepare Audio to prevent Buffering
        videoPlayer.clip = videoToPlay;
        videoPlayer.Prepare();

        //Wait until video is prepared
        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }

        Debug.Log("Done Preparing Video");

        //Assign the Texture from Video to RawImage to be displayed
        //image.texture = videoPlayer.texture;
        planeMaterial.SetTexture("_MainTex", videoPlayer.texture);

        //Play Video
        videoPlayer.Play();

        //Play Sound
        audioSource.Play();

        Debug.Log("Playing Video");
        while (videoPlayer.isPlaying)
        {
            Debug.LogWarning("Video Time: " + Mathf.FloorToInt((float)videoPlayer.time));
            yield return null;
        }

        Debug.Log("Done Playing Video");
    }

    // Update is called once per frame
    void Update()
    {

    }
}

