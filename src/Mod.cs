using System.Reflection;
using ColossalFramework;
using ColossalFramework.UI;
using ICities;

[assembly:AssemblyVersion("1.0.*")]

namespace SkylinesMod
{
    public class Mod : LoadingExtensionBase, IUserMod
    {
        public string Name => "Your very own mod";
        public string Description => "Does something interesting... honest!";

        private void Show()
        {
            UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage("Your Mod", "It's working", false);
        }

        public void OnEnabled()
        {
            if (Singleton<LoadingManager>.instance.m_currentlyLoading || (UnityEngine.Object)UIView.library == (UnityEngine.Object)null)
            {
                Singleton<LoadingManager>.instance.m_introLoaded += new LoadingManager.IntroLoadedHandler(OnIntroLoaded);
                Singleton<LoadingManager>.instance.m_levelLoaded += new LoadingManager.LevelLoadedHandler(OnLevelLoaded);
            }
            else
                Show();
        }

        private void OnIntroLoaded()
        {
            Singleton<LoadingManager>.instance.m_introLoaded -= new LoadingManager.IntroLoadedHandler(OnIntroLoaded);
            Show();
        }

        private void OnLevelLoaded(SimulationManager.UpdateMode updateMode)
        {
            Singleton<LoadingManager>.instance.m_levelLoaded -= new LoadingManager.LevelLoadedHandler(OnLevelLoaded);
            Show();
        }
    }
}
