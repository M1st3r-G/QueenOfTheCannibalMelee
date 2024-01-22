using System.Collections;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(AudioSource))]
    public class MainMenuFunction : MonoBehaviour
    {
        //ComponentReferences
        private AudioSource effects;
        [SerializeField] private AudioClip[] clips;
        [SerializeField] private SettingsMenu settings;
        //Params
        //Temps
        //Public

        private void Awake()
        {
            effects = GetComponent<AudioSource>();
        
            float effectVolume = PlayerPrefs.GetFloat(SettingsMenu.EffectVolumeKey, 0.75f);
            float generalVolume = PlayerPrefs.GetFloat(SettingsMenu.GeneralVolumeKey, 0.75f);
            float musicVolume = PlayerPrefs.GetFloat(SettingsMenu.MusicVolumeKey, 0.75f);

            effects.volume = effectVolume * generalVolume;
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>().volume = musicVolume * generalVolume;
        }

        public void OnSettingsClick()
        {
            effects.PlayOneShot(clips[1]);
            settings.ToggleMenu();
        }

        public void OnPlayClick()
        {
            effects.PlayOneShot(clips[1]);
            StartCoroutine(DelayStart(clips[1].length));
        }

        public void OnQuitClick()
        {
            effects.PlayOneShot(clips[1]);
            StartCoroutine(DelayQuit(clips[1].length));
        }
        
        private static IEnumerator DelayStart(float delay)
        {
            yield return new WaitForSeconds(delay);
            SceneController.LoadFirstLoadingScreen();
        }
        
        private static IEnumerator DelayQuit(float delay)
        {
            yield return new WaitForSeconds(delay);
            Application.Quit();
        }
        
        public void PlayButtonHover() => effects.PlayOneShot(clips[0]);
    }
}
