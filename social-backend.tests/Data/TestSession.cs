using Microsoft.AspNetCore.Http;

public class TestSession : ISession
{
    private readonly Dictionary<string, byte[]> _sessionStorage = new();
    public IEnumerable<string> Keys => _sessionStorage.Keys;
    public string Id => Guid.NewGuid().ToString();
    public bool IsAvailable => true;
    public void Clear() => _sessionStorage.Clear();
    public void Remove(string key) => _sessionStorage.Remove(key);
    public void Set(string key, byte[] value) => _sessionStorage[key] = value;
    public bool TryGetValue(string key, out byte[] value) => _sessionStorage.TryGetValue(key, out value!);
    public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}