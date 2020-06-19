namespace Apocalypse.Any.Infrastructure.Server.States.Interfaces
{
    public static class ServerGameSectorNewBook
    {
        public const string CreatePlayerSpaceshipState = nameof(CreatePlayerSpaceshipState);
        public const string CreateEnemySpaceshipState = nameof(CreateEnemySpaceshipState);
        public const string CreateRandomPlanetState = nameof(CreateRandomPlanetState);
        public const string CreateRandomMediumSpaceShipState = nameof(CreateRandomMediumSpaceShipState);
        public const string CreateRandomEnemyState = nameof(CreateRandomEnemyState);
        public const string CreateRandomFogCommand = nameof(CreateRandomFogCommand);
        public const string BuildSingularMechanicsState = nameof(BuildSingularMechanicsState);
        public const string BuildPluralMechanicsState = nameof(BuildPluralMechanicsState);
        public const string BuildDataLayerState = nameof(BuildDataLayerState);
        public const string BuildGameStateDataLayerState = nameof(BuildGameStateDataLayerState);
        public const string BuildFactoriesState = nameof(BuildFactoriesState);
        public const string BuildDefaultSectorState = nameof(BuildDefaultSectorState);
        public const string DropItemsState = nameof(DropItemsState);
        public const string UpdateAllSingularEnemyMechanicsState = nameof(UpdateAllSingularEnemyMechanicsState);
        public const string UpdateGameStateDataState = nameof(UpdateGameStateDataState);
        public const string UpdateProjectileMechanicsState = nameof(UpdateProjectileMechanicsState);
        public const string RemoveDestroyedProjectilesState = nameof(RemoveDestroyedProjectilesState);
        public const string RemoveDeadEnemiesMechanicsState = nameof(RemoveDeadEnemiesMechanicsState);
        public const string ProcessRotationMapsForPlayerMechanicsState = nameof(ProcessRotationMapsForPlayerMechanicsState);
        public const string ProcessShootingForPlayerMechanicsState = nameof(ProcessShootingForPlayerMechanicsState);
        public const string ProcessThrustForPlayerMechanicsState = nameof(ProcessThrustForPlayerMechanicsState);
        public const string ProcessPlayerChooseStatState = nameof(ProcessPlayerChooseStatState);
        public const string ProcessCollisionMechanicState = nameof(ProcessCollisionMechanicState);
        public const string ProcessUseInventoryForPlayerState = nameof(ProcessUseInventoryForPlayerState);
        public const string ProcessInventoryLeftState = nameof(ProcessInventoryLeftState);
        public const string ProcessInventoryRightState = nameof(ProcessInventoryRightState);
        public const string ProcessReleaseStatState = nameof(ProcessReleaseStatState);
        public const string RemoveImagesMechanicsState = nameof(RemoveImagesMechanicsState);
        public const string RunAsDefaultSector = nameof(RunAsDefaultSector);
    }
}