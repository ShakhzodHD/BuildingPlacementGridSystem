using UnityEngine;
using Zenject;

namespace UIManager
{
    public class UIInstaller : MonoInstaller
    {
        [SerializeField] private UIManager uiManagerPrefab;

        public override void InstallBindings()
        {
            Debug.Log("UI ���������");
            Container.Bind<UIManager>()
                .FromComponentInNewPrefab(uiManagerPrefab)
                .AsSingle()
                .NonLazy();
        }
    }
}