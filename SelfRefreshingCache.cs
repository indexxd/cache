public class SelfRefreshingCache<TResult>
{
	private Func<Task<TResult>> _refreshFunc;
	private TResult? _currentValue;
	private TimeSpan _validityOfCachedData;
	private DateTime _lastRefreshed;
	private TimeSpan _refreshPeriod;
	private bool _isInitialized = false;
	static SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

	public SelfRefreshingCache(
		TimeSpan refreshPeriod,
		TimeSpan validityOfCachedData,
		Func<Task<TResult>> createdCachedItem)
	{
		_refreshFunc = createdCachedItem;
		_validityOfCachedData = validityOfCachedData;
		_refreshPeriod = refreshPeriod;

	}

	public async Task<TResult> GetOrCreate()
	{

		await _semaphore.WaitAsync();

		try
		{

			if (!_isInitialized)
			{
				try
				{
					_currentValue = await _refreshFunc();
					_lastRefreshed = DateTime.Now;
					_isInitialized = true;

					return _currentValue;
				}
				catch (System.Exception)
				{
					throw new Exception("The provided refresh function could not retrieve the value.");
				}

			}


			if (refreshNeeded)
			{
				try
				{
					_currentValue = await _refreshFunc();
					_lastRefreshed = DateTime.Now;
				}
				catch (System.Exception)
				{
					if (!dataIsValid)
					{
						throw new Exception("Validity has expired and the value could not be retrieved.");
					}
				}
			}

			return _currentValue;

		}
		finally
		{
			_semaphore.Release();
		}
	}

	private bool dataIsValid
	{
		get
		{
			return _lastRefreshed.Add(_validityOfCachedData) < DateTime.Now;
		}
	}

	private bool refreshNeeded
	{
		get
		{
			return _lastRefreshed.Add(_refreshPeriod) > DateTime.Now;
		}
	}
}
