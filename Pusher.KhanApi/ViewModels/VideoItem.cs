using System;
using System.Runtime.Serialization;

#if WINDOWS_PHONE
using Microsoft.Phone.Tasks;
#endif

namespace Pusher.KhanApi
{
    [DataContract]
    public class VideoItem : Item
    {
        [DataMember]
        public string Parent { get; set; }

        [DataMember]
        public string YoutubeId { get; set; }

        [DataMember]
        public Uri VideoUri { get; set; }

        [DataMember]
        public Uri VideoScreenshotUri { get; set; }

        [DataMember]
        public Uri VideoFileUri { get; set; }

        public void Navigate()
        {
            bool noVideoFileUri = this.VideoFileUri == null || string.IsNullOrWhiteSpace(this.VideoFileUri.ToString());
#if !WINDOWS_PHONE
            if (noVideoFileUri)
            {
                Windows.System.Launcher.LaunchUriAsync(this.VideoUri);
            }
            else
            {
                Windows.System.Launcher.LaunchUriAsync(this.VideoFileUri);
            }
#else

            if (this.VideoFileUri != null && !string.IsNullOrEmpty(this.VideoFileUri.ToString()))
            {
                var launcher = new MediaPlayerLauncher
                {
                    Controls = MediaPlaybackControls.All,
                    Media = this.VideoFileUri
                };
                launcher.Show();


            }
            else
            {
                WebBrowserTask browser = new WebBrowserTask();
                browser.URL = this.VideoUri.ToString();
                browser.Show();
            }
#endif
        }
    }
}
