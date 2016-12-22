using System.Net;
using GoogleMusicApi.UWP.Sessions;

namespace GoogleMusicApi.UWP.Requests.Data
{
    public class SearchGetRequest : GetRequest
    {
        public int NumberOfResults { get; set; }

        public string Query { get; set; }

        public int[] ReturnTypes { get; set; }

        public SearchGetRequest(Session session, string query, int type) : base(session)
        {
            Query = query;
            NumberOfResults = 100;
            ReturnTypes = new[] {type};
        }

        //TODO (Low): Get types and turn to a flag or enum array
        public override WebRequestHeaders GetUrlContent()
        {
            UrlData.Add(new WebRequestHeader("ct", WebUtility.UrlEncode(string.Join(",", ReturnTypes))));
            UrlData.Add(new WebRequestHeader("q", WebUtility.UrlEncode(Query)));
            UrlData.Add(new WebRequestHeader("max-results", NumberOfResults.ToString()));
            return base.GetUrlContent();
        }
    }
}