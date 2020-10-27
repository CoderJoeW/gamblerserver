public enum ServerPackages
{
    SConnected = 1,
    SAccountExists,
    SAccountDoesNotExist,
    SAccountCreated,
    SAccountLoaded,
    SLobbyJoinError,
}

public enum ClientPackages
{
    CCheckAccountExist = 1,
    CCreateAccount,
    CLoadAccountInfo,
    CJoinLobby,
}