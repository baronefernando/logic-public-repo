using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

public class Localiser : MonoBehaviour
{
    public string GetLocalisedString(string Key)
    {
        return LocalizationSettings.StringDatabase.GetLocalizedString(Key);;
    }
}
