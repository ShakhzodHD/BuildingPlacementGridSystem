using UnityEngine;
using Zenject;

namespace Buildings
{
    public class BuildingInstaller : MonoInstaller
    {
        [SerializeField] private BuildingManager buildingManagerPrefab;

        public override void InstallBindings()
        {
            Container.Bind<BuildingManager>()
                .FromComponentInNewPrefab(buildingManagerPrefab)
                .AsSingle()
                .NonLazy();
        }
    }
}