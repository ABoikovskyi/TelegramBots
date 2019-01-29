using System;
using System.Collections.Generic;

namespace DataLayer.Models
{
    public class Concert
    {
		public int Id { get; set; }
		public string Artist { get; set; }
		public string ShortDescription { get; set; }
		public string FullDescription { get; set; }
		public string FacebookLink { get; set; }
		public string Poster { get; set; }
		public DateTime EventDate { get; set; }
		public string Venue { get; set; }
		public string TicketsLink { get; set; }
        public string AppleMusicLink { get; set; }
        public string YoutubeMusicLink { get; set; }
        public string GoogleMusicLink { get; set; }
        public string SpotifyMusicLink { get; set; }
        public string DeezerMusicLink { get; set; }
        public string VideoInfo { get; set; }
		public string PhotoReport { get; set; }
		public virtual List<ConcertVisit> ConcertVisits { get; set; }
		public virtual List<UserSubscription> Subscriptions { get; set; }
		public virtual List<Post> Posts { get; set; }
	}

	public class ShortConcertInfo
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public DateTime EventDate { get; set; }
	}
}