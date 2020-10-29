public enum ServerPackages
{
    SConnected = 1,
    SAccountExists,
    SAccountDoesNotExist,
    SAccountCreated,
    SAccountLoaded,
    SLobbyJoinError,
    SLobbyStart,
    SPlayerDisconnected,
}

public enum ClientPackages
{
    CAbortConnection = 1,
    CCheckAccountExist,
    CCreateAccount,
    CLoadAccountInfo,
    CJoinLobby,
    CLeaveLobby,
}