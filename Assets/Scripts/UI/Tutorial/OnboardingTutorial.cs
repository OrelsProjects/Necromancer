public class OnboardingTutorial : TutorialBehaviour
{

    #region Step Specific Functions
    public void SelectSmallZombieForRaid() =>
        RaidAssembleController.Instance.SelectZombie(ZombieType.Small);

    public void SelectSmallZombieForSpawn() => ZombieSpawner.Instance.SelectZombie(ZombieType.Small);

    public void SpawnZombie() => ZombieSpawner.Instance.SpawnSelectedZombie();

    public void AcquireSmallZombie() => InventoryManager.Instance.AcquireZombie(ZombieType.Small);

    #endregion
}