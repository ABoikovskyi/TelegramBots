using System.Net;

namespace PostPublisher
{
	class Program
	{
		static void Main(string[] args)
		{
			var request = WebRequest.Create($"https://playzone.ua/popcorn/PublishPost?postId={args[0]}");
			request.Method = "GET";
			using (var response = (HttpWebResponse)request.GetResponse())
			{
				response.GetResponseStream();
			}
		}
	}
}
