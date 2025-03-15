using UnityEngine;
using Zenject;

namespace Buildings
{
    public class BuildingInstaller : MonoInstaller
    {
        [SerializeField] private BuildingManager buildingManagerPrefab;

        public override void InstallBindings()
        {
            Debug.Log("Билдинг инсталлер");
            Container.Bind<BuildingManager>().FromComponentInNewPrefab(buildingManagerPrefab).AsSingle().NonLazy();

            Container.Bind<IBuildingManager>().To<BuildingManager>().FromResolve().AsSingle();
        }
    }
}