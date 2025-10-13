using System;
using System.Collections.Generic;
using System.Reflection;
using EditorAttributes;
using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_EDITOR
using UnityEditor;
#endif

[DisallowMultipleComponent]
public class FullScenarioBlender : MonoBehaviour
{
    [Header("APV scenarios (noms exacts)")]
    public string scenarioA = "ScenarioClean";
    public string scenarioB = "ScenarioStorm";
    [Min(1)] public int numberOfCellsBlendedPerFrame = 10;

    [Header("Directional lights (drag & drop)")]
    public Light lightA; // clean / sunny
    public Light lightB; // storm

    [Header("Global Volume (optional, auto-findsi null)")]
    public Volume globalVolume; // si null le script cherche un Global Volume dans la scène

    [Header("Blend settings")]
    [Tooltip("Durée complète d'une transition (secondes). 600 = 10 minutes")]
    public float blendDuration = 600f;
    public bool autoLoop = true;

    [ReadOnly] public float debugBlendFactor = 0f;

    // runtime
    private ProbeReferenceVolume probeRefVolume;
    private Volume volumeA;
    private Volume volumeB;
    private VolumeProfile profileA;
    private VolumeProfile profileB;
    private VolumeProfile runtimeProfile; // clone appliqué sur globalVolume.profile

    // cache de types/fields pour interpolation volume
    private List<Type> componentTypes = new List<Type>();
    private Dictionary<Type, FieldInfo[]> paramFieldCache = new Dictionary<Type, FieldInfo[]>();

    // valeurs lights sauvegardées
    private Color colorA, colorB;
    private float intensityA, intensityB;
    private Quaternion rotA, rotB;
    private float shadowA, shadowB;
    private float timer = 0f;
    private bool toB = true;

    void Start()
    {
        // ---- APV setup ----
        probeRefVolume = ProbeReferenceVolume.instance;
        if (probeRefVolume == null)
            Debug.LogWarning("[FullScenarioBlender] ProbeReferenceVolume.instance est null (APV pas encore initialisé).");

        if (probeRefVolume != null)
        {
            probeRefVolume.lightingScenario = scenarioA;
            probeRefVolume.numberOfCellsBlendedPerFrame = Mathf.Max(1, numberOfCellsBlendedPerFrame);
        }

        // ---- store initial light values ----
        if (lightA != null)
        {
            colorA = lightA.color;
            intensityA = lightA.intensity;
            rotA = lightA.transform.rotation;
            shadowA = lightA.shadowStrength;
        }
        if (lightB != null)
        {
            colorB = lightB.color;
            intensityB = lightB.intensity;
            rotB = lightB.transform.rotation;
            shadowB = lightB.shadowStrength;
            // start B at 0 intensity so crossfade is visible
            lightB.intensity = 0f;
        }

        // ---- find volumes associated to each light (optional) ----
        volumeA = FindAssociatedVolume(lightA);
        volumeB = FindAssociatedVolume(lightB);

        profileA = volumeA != null ? (volumeA.sharedProfile ?? volumeA.profile) : null;
        profileB = volumeB != null ? (volumeB.sharedProfile ?? volumeB.profile) : null;

        // ---- global volume target ----
        if (globalVolume == null)
            globalVolume = FindGlobalVolume();

        if (globalVolume == null && profileA == null && profileB == null)
        {
            Debug.LogWarning("[FullScenarioBlender] Aucun Volume trouvé pour lire les paramètres de sky/fog. Le script continuera mais ne blendera pas les Volumes.");
        }
        else if (globalVolume != null)
        {
            // Clone le profile pour ne pas modifier l'asset (modifs reset à la sortie du Play)
            runtimeProfile = UnityEngine.Object.Instantiate(globalVolume.profile != null ? globalVolume.profile : globalVolume.sharedProfile);
            if (runtimeProfile == null)
            {
                // si globalVolume n'avait pas de profile -> crée en runtime
                runtimeProfile = ScriptableObject.CreateInstance<VolumeProfile>();
            }
            globalVolume.profile = runtimeProfile;
        }

        // ---- build list of component types to handle (union de A et B) ----
        BuildComponentTypeList();

        // pré-cache des fields des paramètres des VolumeComponents
        CacheParameterFields();
    }

    void Update()
    {
        if (blendDuration <= 0f) blendDuration = 0.0001f;
        timer += Time.deltaTime;
        float factor = Mathf.Clamp01(timer / blendDuration);
        debugBlendFactor = factor;
        float t = toB ? factor : 1f - factor;

        // ---- APV blend ----
        if (probeRefVolume != null)
        {
            probeRefVolume.numberOfCellsBlendedPerFrame = Mathf.Max(1, numberOfCellsBlendedPerFrame);
            probeRefVolume.BlendLightingScenario(scenarioB, t); // API officielle APV
        }

        // ---- Lights blending (couverture large des propriétés utiles) ----
        BlendLights(t);

        // ---- VolumeProfile blending (interpolation générique de tous les VolumeParameter) ----
        if (runtimeProfile != null && (profileA != null || profileB != null))
        {
            BlendVolumeProfiles(t);
        }

        // loop / reset
        if (timer >= blendDuration)
        {
            if (autoLoop)
            {
                toB = !toB;
                timer = 0f;
            }
            else
            {
                timer = blendDuration;
            }
        }
    }

    // ------------------ Helpers ------------------

    // Cherche un Volume sur la light (ou dans son enfant/parent). Retourne null si rien
    Volume FindAssociatedVolume(Light l)
    {
        if (l == null) return null;
        var v = l.GetComponent<Volume>();
        if (v != null) return v;
        v = l.GetComponentInChildren<Volume>(true);
        if (v != null) return v;
        v = l.GetComponentInParent<Volume>(true);
        if (v != null) return v;
        return null;
    }

    // Cherche un global volume dans la scène si tu n'en a pas assigné un
    Volume FindGlobalVolume()
    {
        Volume[] all = FindObjectsOfType<Volume>(true);
        foreach (var v in all)
            if (v.isGlobal) return v;
        return null;
    }

    void BlendLights(float t)
    {
        // si les deux lights sont présentes -> crossfade propre
        if (lightA != null && lightB != null)
        {
            // intensities
            lightA.intensity = Mathf.Lerp(intensityA, 0f, t);
            lightB.intensity = Mathf.Lerp(0f, intensityB, t);

            // couleurs
            lightA.color = Color.Lerp(colorA, colorB, t);
            lightB.color = colorB;

            // rotation (A vers B), B garde sa rotation target
            lightA.transform.rotation = Quaternion.Slerp(rotA, rotB, t);
            lightB.transform.rotation = rotB;

            // shadows
            lightA.shadowStrength = Mathf.Lerp(shadowA, 0f, t);
            lightB.shadowStrength = Mathf.Lerp(0f, shadowB, t);

            // quelques autres propriétés utiles — safe - on essaye de copier si possible
            try
            {
                lightB.renderingLayerMask = lightB.renderingLayerMask; // laisse B tel quel
            }
            catch { /* ignore si n'existe pas */ }
        }
        else if (lightA != null) // only A
        {
            lightA.intensity = Mathf.Lerp(intensityA, intensityA * 0.25f, t);
            lightA.color = Color.Lerp(colorA, new Color(0.8f, 0.85f, 0.9f), t);
            lightA.transform.rotation = Quaternion.Slerp(rotA, rotA * Quaternion.Euler(10f, 0f, 0f), t);
        }
        else if (lightB != null) // only B
        {
            lightB.intensity = Mathf.Lerp(0f, intensityB, t);
            lightB.color = colorB;
            lightB.transform.rotation = rotB;
        }
    }

    // Construit la liste des types de VolumeComponent à prendre en compte (union A,B)
    void BuildComponentTypeList()
    {
        componentTypes.Clear();
        if (profileA != null)
            AddProfileComponentTypes(profileA);
        if (profileB != null)
            AddProfileComponentTypes(profileB);
    }

    void AddProfileComponentTypes(VolumeProfile prof)
    {
        if (prof == null) return;
        // récupère la liste interne "components" par reflection (compatible versions)
        FieldInfo fi = typeof(VolumeProfile).GetField("components", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        if (fi == null) return;
        var list = fi.GetValue(prof) as System.Collections.IList;
        if (list == null) return;
        foreach (var comp in list)
        {
            if (comp == null) continue;
            Type t = comp.GetType();
            if (!componentTypes.Contains(t))
                componentTypes.Add(t);
        }
    }

    void CacheParameterFields()
    {
        paramFieldCache.Clear();
        foreach (var t in componentTypes)
        {
            var fields = t.GetFields(BindingFlags.Instance | BindingFlags.Public);
            // garder seulement ceux qui sont VolumeParameter
            List<FieldInfo> paramFields = new List<FieldInfo>();
            foreach (var f in fields)
            {
                if (typeof(VolumeParameter).IsAssignableFrom(f.FieldType))
                    paramFields.Add(f);
            }
            paramFieldCache[t] = paramFields.ToArray();
        }
    }

    // Interpole tous les VolumeParameter entre profileA et profileB, écrit dans runtimeProfile
    void BlendVolumeProfiles(float t)
    {
        // s'assure que runtimeProfile contient tous les components nécessaires
        foreach (var compType in componentTypes)
        {
            if (!runtimeProfile.Has(compType))
            {
                runtimeProfile.Add(compType, true);
            }

            // récupère instances
            var compRuntime = GetComponentFromProfile(runtimeProfile, compType);
            var compA = profileA != null ? GetComponentFromProfile(profileA, compType) : null;
            var compB = profileB != null ? GetComponentFromProfile(profileB, compType) : null;

            if (compRuntime == null) continue;

            // pour chaque param (cached fields)
            if (!paramFieldCache.TryGetValue(compType, out FieldInfo[] fields) || fields == null) continue;
            foreach (var f in fields)
            {
                try
                {
                    var dstParam = f.GetValue(compRuntime) as VolumeParameter;
                    var aParam = compA != null ? f.GetValue(compA) as VolumeParameter : null;
                    var bParam = compB != null ? f.GetValue(compB) as VolumeParameter : null;
                    InterpParameter(dstParam, aParam, bParam, t);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[FullScenarioBlender] erreur en interpolant param {f.Name} of {compType.Name}: {e.Message}");
                }
            }
        }
        // mark dirty to force update in editor (optionnel)
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(runtimeProfile);
#endif
    }

    // Récupère un component spécifique depuis un VolumeProfile (via TryGetSubclassOf)
    VolumeComponent GetComponentFromProfile(VolumeProfile prof, Type type)
    {
        if (prof == null) return null;
        // use TryGetSubclassOf via reflection because generic is inconvenient here
        MethodInfo mi = typeof(VolumeProfile).GetMethod("TryGetSubclassOf", BindingFlags.Instance | BindingFlags.Public);
        if (mi != null)
        {
            object[] args = new object[] { type, null };
            bool ok = (bool)mi.MakeGenericMethod(type).Invoke(prof, args);
            // note : above MakeGenericMethod(type) will fail because TryGetSubclassOf<T> expects T : VolumeComponent compile-time type.
            // fallback: we search the internal list via reflection:
        }

        // fallback: read internal components list and find by type
        FieldInfo fi = typeof(VolumeProfile).GetField("components", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        if (fi == null) return null;
        var list = fi.GetValue(prof) as System.Collections.IList;
        if (list == null) return null;
        foreach (var comp in list)
        {
            if (comp == null) continue;
            if (comp.GetType() == type || comp.GetType().IsSubclassOf(type))
                return comp as VolumeComponent;
        }
        return null;
    }

    // Interpole un VolumeParameter 'dst' à partir de 'a' et 'b'
    void InterpParameter(VolumeParameter dst, VolumeParameter a, VolumeParameter b, float t)
    {
        if (dst == null) return;

        // s'assurer que la valeur est utilisée
        dst.overrideState = true;

        // Try to call Interp(fromVal, toVal, t) on the dst parameter (works for VolumeParameter<T>)
        MethodInfo interpMI = dst.GetType().GetMethod("Interp", BindingFlags.Instance | BindingFlags.Public);
        try
        {
            object fromVal = null;
            object toVal = null;

            // get underlying values (value property) if available
            PropertyInfo valPropDst = dst.GetType().GetProperty("value", BindingFlags.Instance | BindingFlags.Public);
            if (a != null)
            {
                PropertyInfo pa = a.GetType().GetProperty("value", BindingFlags.Instance | BindingFlags.Public);
                fromVal = pa != null ? pa.GetValue(a) : null;
            }
            else if (valPropDst != null)
                fromVal = valPropDst.GetValue(dst);

            if (b != null)
            {
                PropertyInfo pb = b.GetType().GetProperty("value", BindingFlags.Instance | BindingFlags.Public);
                toVal = pb != null ? pb.GetValue(b) : null;
            }
            else if (valPropDst != null)
                toVal = valPropDst.GetValue(dst);

            if (interpMI != null)
            {
                // safe invoke: if fromVal or toVal are null, the method may still handle it or fallback will occur
                interpMI.Invoke(dst, new object[] { fromVal, toVal, t });
                dst.overrideState = true;
                return;
            }
        }
        catch
        {
            // ignore and fallback to SetValue below
        }

        // fallback simple: snap at 0.5 or prefer existing
        try
        {
            if (t < 0.5f && a != null)
                dst.SetValue(a);
            else if (b != null)
                dst.SetValue(b);
        }
        catch { /* silent */ }
    }

    void OnValidate()
    {
        if (numberOfCellsBlendedPerFrame < 1) numberOfCellsBlendedPerFrame = 1;
        if (probeRefVolume == null)
            probeRefVolume = ProbeReferenceVolume.instance;
        if (probeRefVolume != null)
            probeRefVolume.numberOfCellsBlendedPerFrame = Mathf.Max(1, numberOfCellsBlendedPerFrame);
    }
}
