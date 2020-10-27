public enum ServerPackages
{
    SConnected = 1,
    SAccountExists,
    SAccountDoesNotExist,
    SAccountCreated,
    SAccountLoaded,
    SLobbyJoinError,
    SLobbyStart,
}

public enum ClientPackages
{
    CCheckAccountExist = 1,
    CCreateAccount,
    CLoadAccountInfo,
    CJoinLobby,
    CLeaveLobby,
}