using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

public class Setting : MonoBehaviour
{
    public Slider sliderBrightness;
    public Slider sliderVolume;

    public Light2D[] lights; // Mảng để chứa nhiều Light2D
    public AudioSource[] audioSources; // Mảng để chứa nhiều AudioSource

    void Start()
    {
        // Thiết lập giá trị tối đa cho sliderBrightness
        sliderBrightness.maxValue = 5f;

        // Thiết lập giá trị cho sliderVolume
        sliderVolume.maxValue = 1f;

        // Lấy giá trị brightness và volume đã lưu hoặc đặt giá trị mặc định
        float savedBrightness = PlayerPrefs.GetFloat("Brightness", 2.5f); // Giá trị mặc định là 2.5
        float savedVolume = PlayerPrefs.GetFloat("Volume", 1f);

        // Đặt giá trị slider
        sliderBrightness.value = savedBrightness;
        sliderVolume.value = savedVolume;

        // Cập nhật giá trị ánh sáng và âm lượng dựa trên giá trị đã lưu
        foreach (Light2D light in lights)
        {
            light.intensity = sliderBrightness.value;
        }

        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.volume = sliderVolume.value;
        }

        // Đăng ký sự kiện thay đổi giá trị cho slider
        sliderBrightness.onValueChanged.AddListener(delegate { OnBrightnessChange(); });
        sliderVolume.onValueChanged.AddListener(delegate { OnVolumeChange(); });
    }

    void OnEnable()
    {
        // Thiết lập giá trị cho các slider dựa trên giá trị đã lưu khi mở menu settings
        sliderBrightness.value = PlayerPrefs.GetFloat("Brightness", 2.5f); // Giá trị mặc định là 2.5
        sliderVolume.value = PlayerPrefs.GetFloat("Volume", 1f);
    }

    void OnBrightnessChange()
    {
        // Điều chỉnh cường độ ánh sáng của tất cả Light2D và lưu lại giá trị
        foreach (Light2D light in lights)
        {
            light.intensity = sliderBrightness.value;
        }
        PlayerPrefs.SetFloat("Brightness", sliderBrightness.value);
    }

    void OnVolumeChange()
    {
        // Điều chỉnh âm lượng của tất cả AudioSource và lưu lại giá trị
        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.volume = sliderVolume.value;
        }
        PlayerPrefs.SetFloat("Volume", sliderVolume.value);

        // Log giá trị của slider volume
        Debug.Log("Volume value: " + sliderVolume.value);
    }
}
