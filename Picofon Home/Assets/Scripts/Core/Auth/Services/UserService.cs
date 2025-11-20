using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class UserService
{
    private const string BaseUrl = "https://api.example.com/users";

    public async Task<List<ChildListItemDTO>> GetUserChildren(
        string userId,
        CancellationTokenSource cts
    )
    {
        string url = $"{BaseUrl}/{userId}?active=true";

        // Simulate list
        await Task.Delay(5000, cts.Token);
        List<ChildListItemDTO> simulatedList = new()
        {
            new ChildListItemDTO { Id = "child1", Name = "Alice" },
            new ChildListItemDTO { Id = "child2", Name = "Bob" },
        };

        return simulatedList;
    }
}
