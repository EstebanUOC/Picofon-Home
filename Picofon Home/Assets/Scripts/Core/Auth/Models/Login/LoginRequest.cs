public struct LoginFirebaseRequest
{
    public string FirebaseIdToken { get; set; }
}

public readonly struct FirebaseRequest
{
    public readonly bool ReturnSecureToken { get; init; }

    public readonly string Email { get; init; }

    public readonly string Password { get; init; }
}
