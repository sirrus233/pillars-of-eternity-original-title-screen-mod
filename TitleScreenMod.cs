using Patchwork.Attributes;
using System.IO;
using System.Linq;
using UnityEngine;

[assembly: PatchAssembly]

namespace TitleScreenMod
{
    [PatchInfo]
    public class ModInfo : IPatchInfo
    {
        public static string Combine(params string[] paths)
        {
            var current = paths.Aggregate(@"", Path.Combine);
            return current;
        }

        public FileInfo GetTargetFile(AppInfo app)
        {
            var file = Combine(app.BaseDirectory.FullName, "PillarsOfEternity_Data", "Managed", "Assembly-CSharp.dll");
            return new FileInfo(file);
        }

        public string CanPatch(AppInfo app)
        {
            return null;
        }

        public string PatchVersion => "1.0.0";

        public string Requirements => "None";

        public string PatchName => "TitleScreenMod";
    }

    [ModifiesType]
    public class FrontEndTitleIntroductionManagerModded : FrontEndTitleIntroductionManager
    {
        [ModifiesMember("Start")]
        private void StartModded()
        {
            this.DeveloperPresents.alpha = 0.0f;
            this.TitleSprite.alpha = 0.0f;
            this.ExpansionTitle.alpha = 0.0f;
            FrontEndTitleIntroductionManager.Instance = this;
            this.BaseGameContent.SetActive(false);
            // Modified lines from here on.
            this.SwitchBackgroundImmediate(MainMenuBackgroundType.BaseGame);
            UISwayMotion component = this.BackgroundCamera.GetComponent<UISwayMotion>();
            if (component)
            {
                component.enabled = true;
            }
        }

        [ModifiesMember("StartFrontEndIntroduction")]
        public void StartFrontEndIntroductionModded()
        {
            this.m_StateVariable = 0.0f;
            this.m_IntroductionState = FrontEndTitleIntroductionManager.IntroductionState.FadeOutMainMenu;
            if (Conditionals.CommandLineArg("e3") && GameUtilities.HasPX1())
                this.m_IntroductionState = FrontEndTitleIntroductionManager.IntroductionState.ExpansionTitleReveal;
            if ((bool)(Object)GameCursor.Instance)
                GameCursor.Instance.SetShowCursor((object)this, false);
            AudioSource component1 = this.GetComponent<AudioSource>();
            if ((Object)component1 == (Object)null)
                return;
            component1.clip = this.TransitionMusic;
            component1.ignoreListenerVolume = true;
            if (component1.enabled)
                component1.Play();
            TweenVolume component2 = this.GetComponent<TweenVolume>();
            component2.to = MusicManager.Instance.FinalMusicVolume;
            component2.Reset();
            component2.Play(true);
            // Removed a fade to black.
            if (!(bool)(Object)BuyWhiteMarchManager.Instance)
                return;
            BuyWhiteMarchManager.Instance.Close();
        }
    }

    [ModifiesType]
    public class CreditsModded : Credits
    {
        [ModifiesMember("HandleCreditsFinished")]
        public void HandleCreditsFinishedModded()
        {
            UIScrollController component = this.ScrollContainer.GetComponent<UIScrollController>();
            if ((UnityEngine.Object)component != (UnityEngine.Object)null)
                component.SetPaused(true);
            FrontEndTitleIntroductionManager.Instance.SwitchBackground(MainMenuBackgroundType.BaseGame); // Modified line.
            this.MainMenu.MenuActive = true;
            this.m_State = Credits.CreditsState.Finished;
        }
    }
}
