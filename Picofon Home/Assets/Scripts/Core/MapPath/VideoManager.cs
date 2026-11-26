using PrimeTween;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Picofon.Core.MapPath
{
    public class VideoManager : MonoBehaviour
    {
        [SerializeField]
        private RawImage _videoImage;

        [SerializeField]
        private VideoPlayer _videoPlayer;

        [SerializeField]
        private Image _videoFrame;

        [SerializeField]
        private Image _fade;

        public void PlayVideo()
        {
            if (gameObject.activeInHierarchy == false)
            {
                gameObject.SetActive(true);
            }

            _videoImage.gameObject.SetActive(true);

            _videoFrame.gameObject.SetActive(true);

            _videoImage.transform.localScale = Vector3.one * 0.3f;
            _videoFrame.transform.localScale = Vector3.one * 0.3f;

            Tween.Scale(_videoImage.transform, Vector3.one, 0.5f, ease: Ease.OutBack);
            Tween.Scale(_videoFrame.transform, Vector3.one, 0.5f, ease: Ease.OutBack);

            _fade.gameObject.SetActive(true);

            _fade.color = new Color(0, 0, 0, 0);

            Tween.Alpha(_fade, 0.7f, 0.5f);

            _videoPlayer.gameObject.SetActive(true);
            _videoPlayer.Play();
        }

        public void StopVideo()
        {
            _videoPlayer.Stop();

            Tween
                .Alpha(_fade, 0f, 0.5f)
                .OnComplete(() =>
                {
                    _fade.gameObject.SetActive(false);
                });

            Tween
                .Scale(_videoImage.transform, Vector3.one * 0.3f, 0.5f, ease: Ease.InBack)
                .OnComplete(() =>
                {
                    _videoImage.gameObject.SetActive(false);
                });

            Tween
                .Scale(_videoFrame.transform, Vector3.one * 0.3f, 0.5f, ease: Ease.InBack)
                .OnComplete(() =>
                {
                    _videoFrame.gameObject.SetActive(false);
                });
        }

        public bool IsVideoPlaying()
        {
            return _videoPlayer.isPlaying;
        }
    }
}
