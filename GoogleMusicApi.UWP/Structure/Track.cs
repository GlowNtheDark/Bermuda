using System.Collections.Generic;
using GoogleMusicApi.UWP.Structure.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Windows.UI.Xaml.Media;
using Windows.UI;

namespace GoogleMusicApi.UWP.Structure
{
    [JsonObject]
    public class Track
    {
        [JsonProperty("album")]
        public string Album { get; set; }

        [JsonProperty("albumArtist")]
        public string AlbumArtist { get; set; }

        [JsonProperty("albumArtRef")]
        public List<ArtReference> AlbumArtReference { get; set; }

        [JsonProperty("albumAvailableForPurchase")]
        public bool AlbumAvailableForPurchase { get; set; }

        [JsonProperty("albumId")]
        public string AlbumId { get; set; }

        [JsonProperty("artist")]
        public string Artist { get; set; }

        [JsonProperty("artistArtRef")]
        public ArtReference[] ArtistArtRef { get; set; }

        [JsonProperty("artistId")]
        public string[] ArtistIds { get; set; }

        [JsonProperty("composer")]
        public string Composer { get; set; }

        [JsonProperty("discNumber")]
        public int DiscNumber { get; set; }

        [JsonProperty("durationMillis")]
        public float DurationMillis { get; set; }

        [JsonProperty("estimatedSize")]
        public float EstimatedSize { get; set; }

        [JsonProperty("explicitType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ExplicitType ExplicitType { get; set; }

        [JsonProperty("genre")]
        public string Genre { get; set; }

        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("lastRatingChangeTimestamp")]
        public string LastRatingChangeTimestamp { get; set; }

        [JsonProperty("lastModifiedTimestamp")]
        public string LastModifiedTimestamp { get; set; }

        [JsonProperty("creationTimestamp")]
        public string CreationTimestamp { get; set; }

        [JsonProperty("recentTimestamp")]
        public string RecentTimestamp { get; set; }

        [JsonProperty("deleted")]
        public bool Deleted { get; set; }

        [JsonProperty("nid")]
        public string Nid { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("clientId")]
        public string ClientId { get; set; }

        [JsonProperty("playCount")]
        public int PlayCount { get; set; }

        [JsonProperty("primaryVideo")]
        public Video PrimaryVideo { get; set; }

        [JsonProperty("rating")]
        public int Rating { get; set; }

        [JsonProperty("storeId")]
        public string StoreId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("trackAvailableForPurchase")]
        public bool TrackAvailableForPurchase { get; set; }

        [JsonProperty("trackAvailableForSubscription")]
        public bool TrackAvailableForSubscription { get; set; }

        [JsonProperty("trackNumber")]
        public int TrackNumber { get; set; }

        [JsonProperty("trackType")]
        public int TrackType { get; set; }

        [JsonProperty("totalTrackCount")]
        public int TotalTrackCount { get; set; }

        [JsonProperty("totalDiscCount")]
        public int TotalDiskCount { get; set; }

        [JsonProperty("year")]
        public int Year { get; set; }

        public SolidColorBrush tileColor { get; set; } = new SolidColorBrush(Colors.Transparent);

        public override string ToString()
        {
            return string.Join(" ", "Title:", Title, "Album:", Album, "Genre:", Genre);
        }
    }
}