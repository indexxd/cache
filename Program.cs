namespace HelloWorld
{
	class Program
	{
		static async Task Main(string[] args)
		{
			Console.WriteLine("Started");

			var refreshPeriod = TimeSpan.FromMinutes(10);
			var validityOfCachedData = TimeSpan.FromMinutes(20);

			var cache = new SelfRefreshingCache<string>(refreshPeriod, validityOfCachedData, SendRequestA);

			var res = await cache.GetOrCreate();

			Console.WriteLine(res);
		}


		static async Task<string> SendRequestA()
		{
			var client = new HttpClient();
			var response = await client.GetAsync("https://api.agify.io/?name=meelad");

			return await response.Content.ReadAsStringAsync();
		}
	}
}