namespace LinkUp.Helpers;

public static class VideoUrlHelper
{
    public static string? NormalizeYouTubeUrl(string? url)
    {
        if (string.IsNullOrEmpty(url)) return null;

        if (url.Contains("youtube.com/watch?v="))
        {
            var videoId = url.Split("watch?v=").Last().Split('&').First();
            return $"https://www.youtube.com/embed/{videoId}";
        }
        else if (url.Contains("youtu.be/"))
        {
            var videoId = url.Split("youtu.be/").Last().Split('?').First();
            return $"https://www.youtube.com/embed/{videoId}";
        }

        return url; // si no es YouTube, se deja tal cual
    }
}
