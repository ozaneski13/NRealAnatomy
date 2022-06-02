using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Unity.Collections;

public class CaptureTest : MonoBehaviour
{
    [SerializeField] private RawImage _rawImage = null;

    private Texture2D _texture = null;

    private int count = 0;

    private void Start()
    {
        StartCoroutine(CaptureRoutine());
    }

    private Texture2D TextureToTexture2D(Texture texture)
    {
        Texture2D texture2D = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture renderTexture = RenderTexture.GetTemporary(texture.width, texture.height, 32);
        Graphics.Blit(texture, renderTexture);

        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();

        RenderTexture.active = currentRT;
        RenderTexture.ReleaseTemporary(renderTexture);
        return texture2D;
    }

    private void OnDestroy()
    {
        //RecordMovie();
    }

    private List<Texture2D> textureArray = null;

    private IEnumerator CaptureRoutine()
    {
        textureArray = new List<Texture2D>();

        while (true)
        {
            if (_rawImage.texture != null)
            {
                _texture = TextureToTexture2D(_rawImage.texture);

                byte[] bytes = _texture.EncodeToPNG();
                var dirPath = Path.Combine(Application.persistentDataPath, "Captures");
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                File.WriteAllBytes(dirPath + "Image" + count.ToString() + ".png", bytes);

                count++;

                textureArray.Add(_texture);

                yield return new WaitForSeconds(1/24f);
            }

            yield return null;
        }
    }
    /*

    private MediaEncoder encoder;
    private NativeArray<float> audioBuffer;

    public void RecordMovie()
    {
        var videoAttr = new VideoTrackAttributes
        {
            frameRate = new MediaRational(24),
            width = 1280,
            height = 720,
            includeAlpha = false
        };

        var audioAttr = new AudioTrackAttributes
        {
            sampleRate = new MediaRational(48000),
            channelCount = 2,
            language = "fr"
        };

        int sampleFramesPerVideoFrame = audioAttr.channelCount *
            audioAttr.sampleRate.numerator / videoAttr.frameRate.numerator;

        var encodedFilePath = Path.Combine(Application.persistentDataPath + "/Images");

        encoder = new MediaEncoder(encodedFilePath, videoAttr, audioAttr);
        audioBuffer = new NativeArray<float>(sampleFramesPerVideoFrame, Allocator.Temp);
            for (int i = 0; i < textureArray.Count; ++i)
            {
            // Fill 'tex' with the video content to be encoded into the file for this frame.
            // ...
            encoder.AddFrame(textureArray[i]);

                // Fill 'audioBuffer' with the audio content to be encoded into the file for this frame.
                // ...
            }

        encoder.Dispose();
    }

    public void Encode(Texture2D tex)
    {
        encoder.AddFrame(tex);
    }*/
}