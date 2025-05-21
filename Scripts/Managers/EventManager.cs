using System;
using UnityEngine;


public class EventManager : MonoBehaviour
{

    #region Debug

    private void DebugSubscribers(Delegate eventDelegate, string eventName)
    {
        if (eventDelegate == null)
        {
            Debug.Log($"[EventManager Debug] Event '{eventName}' has no subscribers.");
            return;
        }

        var subscribers = eventDelegate.GetInvocationList();
        Debug.Log($"[EventManager Debug] Event '{eventName}' has {subscribers.Length} subscriber(s):");

        foreach (var subscriber in subscribers)
        {
            Debug.Log($"  - {subscriber.Target} in method {subscriber.Method.Name}");
        }
    }

    #endregion

    private static EventManager instance;
    public static EventManager Instance
    {
        get { return instance; }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        instance = null;
    }


    [SerializeField] private ProfileChangeEvents profileChange;
    public ProfileChangeEvents ProfileChange => profileChange;

    [SerializeField] private CombatHitEvents combatHits;
    public CombatHitEvents CombatHits => combatHits;





    [SerializeField] private ObjectPoolingEvents objectPooling;
    public ObjectPoolingEvents ObjectPooling => objectPooling;



    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        profileChange = new ProfileChangeEvents();
        combatHits = new CombatHitEvents();
        objectPooling = new ObjectPoolingEvents();
    }




    [System.Serializable]
    public class ProfileChangeEvents
    {
        public event System.Action<CharacterProfile> OnGlobalProfileChanged; // -> Don't use when you want to change a single unit's stats, use it to change the profile globally (it'll affect all actors that have the specified character profile)

        public void TriggerGlobalProfileChanged(CharacterProfile characterProfile)
        {
            OnGlobalProfileChanged?.Invoke(characterProfile);
        }

    }

    [System.Serializable]
    public class CombatHitEvents
    {
        [SerializeField] private bool debugCombatHitEvents;
        [SerializeField] private bool debugDamageTakeEvents;


        public event Action<DamageSource, GameObject> OnCombatHit;
        public event Action<BuffAndDebuffSource, GameObject> OnCombatBuffDebuffHit;
        public event Action<DamageableCharacter, DamageSource> OnCharacterTookDamage;
        public event Action<DamageableCharacter, DamageSource> OnCharacterPushedBack;

        public void TriggerCombatHit(DamageSource source, GameObject target)
        {
            OnCombatHit?.Invoke(source, target);

            if (debugCombatHitEvents)
                Instance.DebugSubscribers(OnCombatHit, "OnCombatHit");
        }

        public void TriggerBuffDebuffHit(BuffAndDebuffSource source, GameObject target)
        {
            OnCombatBuffDebuffHit?.Invoke(source, target);

            if (debugCombatHitEvents)
                Instance.DebugSubscribers(OnCombatBuffDebuffHit, "OnCombatBuffDebuffHit");
        }

        public void TriggerCharacterTookDamage(DamageableCharacter character, DamageSource damageSource)
        {
            OnCharacterTookDamage?.Invoke(character, damageSource);

            if (debugDamageTakeEvents)
                Instance.DebugSubscribers(OnCharacterTookDamage, "OnCharacterTookDamage");
        }

        public void TriggerCharacterPushback(DamageableCharacter character, DamageSource damageSource)
        {
            OnCharacterPushedBack?.Invoke(character, damageSource);

            if (debugDamageTakeEvents)
                Instance.DebugSubscribers(OnCharacterPushedBack, "OnCharacterPushback");
        }
    }
    


    [System.Serializable]
    public class ObjectPoolingEvents
    {
        public event Action<DamageSource> OnDamageSourceCreated;
        public void TriggerDamageSourceCreated(DamageSource damageSource)
        {
            OnDamageSourceCreated?.Invoke(damageSource);
        }
    }
}
