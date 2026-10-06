using Picofon.Utils;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Picofon.Core.MapPath
{
    public class VideoManager : MonoBehaviour
    {
        [SerializeField]
        private SimpleButton _playButton;

        [SerializeField]
        private RawImage _videoImage;

        [SerializeField]
        private VideoPlayer _videoPlayer;

        [SerializeField]
        private Image _videoFrame;

        [SerializeField]
        private Image _fade;

        public void Awake()
        {
            _playButton.OnClick += PlayVideo;
        }

        private void PlayVideo()
        {
            _playButton.gameObject.SetActive(false);

            _videoImage.gameObject.SetActive(true);

            _videoFrame.gameObject.SetActive(true);

            _videoImage.transform.localScale = Vector3.one * 0.3f;
            _videoFrame.transform.localScale = Vector3.one * 0.3f;

            Tween.Scale(_videoImage.transform, Vector3.one, 0.5f, ease: Ease.OutBack);
            Tween.Scale(_videoFrame.transform, Vector3.one, 0.5f, ease: Ease.OutBack);

            _fade.gameObject.SetActive(true);

            _fade.color = new Color(0, 0, 0, 0);

            Tween.Alpha(_fade, 1, 0.5f);

            _videoPlayer.gameObject.SetActive(true);
            _videoPlayer.Play();
        }
    }
}
