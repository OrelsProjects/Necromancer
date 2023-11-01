public class OnboardingTutorial : TutorialBehaviour
{
    #region Step Specific Functions
    public void SelectSmallZombieForRaid() =>
        RaidAssembleController.Instance.SelectZombie(ZombieType.Small);

    public void SelectSmallZombieForSpawn() => ZombieSpawner.Instance.SelectZombie(ZombieType.Small);

    public void SpawnZombie() => ZombieSpawner.Instance.SpawnSelectedZombie();

    internal override void SetType()
    {
        _type = TutorialType.Onboarding;
    }

    #endregion
}