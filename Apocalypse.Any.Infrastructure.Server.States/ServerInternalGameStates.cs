namespace Apocalypse.Any.Infrastructure.Server.States
{
    public enum ServerInternalGameStates : byte
    {
        /// <summary>
        /// This only appears if the byte given is a default (0). Means that something went wrong or was undefined
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// Default start state. 
        /// </summary>
        Initial = 1,

        /// <summary>
        /// Internal login request state. This should be the first state if login into a restricted system
        /// </summary>
        Login = 2,

        /// <summary>
        /// Self explanatory.
        /// </summary>
        LoginSuccessful = 3,

        /// <summary>
        /// Throws a game server exception if something went wrong
        /// </summary>
        Error = 4,

        /// <summary>
        /// Starts the update command. This basically fires update commands to the client
        /// </summary>
        Update = 5,

        /// <summary>
        /// Starts the CLI "process"
        /// </summary>
        CLI = 6,

        /// <summary>
        /// Passes through the information of a CLI user to the game server
        /// </summary>
        CLIPassthrough = 7,

        /// <summary>
        /// Singalizes that 
        /// </summary>
        ReceiveWork = 8,

        /// <summary>
        /// User role gateway tha routes messages to the corresponding game state
        /// </summary>
        UserRoleGateWay = 9,
        UpdateDelta = 10,


    };
}