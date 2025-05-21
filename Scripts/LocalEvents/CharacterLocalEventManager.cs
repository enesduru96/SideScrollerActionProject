using UnityEngine;
using System;

public class CharacterLocalEventManager : MonoBehaviour
{

    [SerializeField] private CharacterInputEvents characterInput;
    public CharacterInputEvents CharacterInput => characterInput;


    [SerializeField] private CharacterActionEvents characterActions;
    public CharacterActionEvents CharacterActions => characterActions;


    [SerializeField] private PlayerCombatHandlerEvents playerCombatHandler;
    public PlayerCombatHandlerEvents PlayerCombatHandler => playerCombatHandler;


    [SerializeField] private CombatHandlerEvents combatHandler;
    public CombatHandlerEvents CombatHandler => combatHandler;


    [SerializeField] private CharacterStateEvents characterStateEvents;
    public CharacterStateEvents _CharacterStateEvents => characterStateEvents;


    [SerializeField] private CharacterBusyStateEvents characterBusyState;
    public CharacterBusyStateEvents CharacterBusyState => characterBusyState;

    private void Awake()
    {
        characterInput = new CharacterInputEvents();
        characterActions = new CharacterActionEvents();
        playerCombatHandler = new PlayerCombatHandlerEvents();
        combatHandler = new CombatHandlerEvents();
        characterStateEvents = new CharacterStateEvents();
        characterBusyState = new CharacterBusyStateEvents();
    }

    [System.Serializable]
    public class CharacterInputEvents
    {
        public event Action OnRunPerformed;
        public event Action OnLedgeDropPerformed;

        [SerializeField] private bool debugInputEvents;

        private void DebugSubscribers(Delegate eventDelegate, string eventName)
        {
            if (eventDelegate == null)
            {
                Debug.Log($"[Character Local EventManager Debug] PlayerInputEvent '{eventName}' has no subscribers.");
                return;
            }

            var subscribers = eventDelegate.GetInvocationList();
            Debug.Log($"[Character Local EventManager Debug] PlayerInputEvent '{eventName}' has {subscribers.Length} subscriber(s):");

            foreach (var subscriber in subscribers)
            {
                Debug.Log($"  - {subscriber.Target} in method {subscriber.Method.Name}");
            }
        }

        public void TriggerRunButtonPressed()
        {
            OnRunPerformed?.Invoke();

            if (debugInputEvents)
                DebugSubscribers(OnRunPerformed, "OnRunPerformed");
        }
        public void TriggerLedgeDropButtonPressed()
        {
            OnLedgeDropPerformed?.Invoke();

            if (debugInputEvents)
                DebugSubscribers(OnLedgeDropPerformed, "OnLedgeDropPerformed");
        }


        public event Action OnInteractButtonPressed;
        public event Action OnInteractLiteButtonPressed;

        public void TriggerInteractButtonPressed()
        {
            OnInteractButtonPressed?.Invoke();

            if (debugInputEvents)
                DebugSubscribers(OnInteractButtonPressed, "OnInteractPerformed");
        }
        public void TriggerInteractLiteButtonPressed()
        {
            OnInteractLiteButtonPressed?.Invoke();

            if (debugInputEvents)
                DebugSubscribers(OnInteractLiteButtonPressed, "OnInteractPerformed");
        }



        public event Action OnJumpButtonPressed;
        public event Action OnCrouchButtonPressed;
        public event Action OnJetpackButtonPressed;

        public void TriggerJumpButtonPressed()
        {
            OnJumpButtonPressed?.Invoke();

            if (debugInputEvents)
                DebugSubscribers(OnJumpButtonPressed, "OnJumpPerformed");
        }

        public event Action OnPlayerDashStarted;
        public void TriggerPlayerDashStarted()
        {
            OnPlayerDashStarted?.Invoke();

            if (debugInputEvents)
                DebugSubscribers(OnPlayerDashStarted, "OnPlayerDashStarted");
        }
        public void TriggerCrouchButtonPressed()
        {
            OnCrouchButtonPressed?.Invoke();

            if (debugInputEvents)
                DebugSubscribers(OnCrouchButtonPressed, "OnCrouchPerformed");
        }
        public void TriggerJetpackButtonPressed()
        {
            OnJetpackButtonPressed?.Invoke();

            if (debugInputEvents)
                DebugSubscribers(OnJetpackButtonPressed, "OnJetpackPerformed");
        }



        public event Action OnHeavyAttackButtonPressed;


        public void TriggerHeavyAttackButtonPressed()
        {
            OnHeavyAttackButtonPressed?.Invoke();

            if (debugInputEvents)
                DebugSubscribers(OnHeavyAttackButtonPressed, "OnHeavyAttackPerformed");

        }

    }


    [System.Serializable]
    public class CharacterActionEvents
    {

        public event Action OnDrawSheatheStarted;
        public event Action OnGroundLightAttackStarted;
        public event Action OnAirLightAttackStarted;

        public event Action OnBlockStarted;
        public event Action OnBlockStopped;

        public event Action<float> OnTurnBackRunningStarted;

        [SerializeField] private bool debugCharacterActionEvents;


        private void DebugSubscribers(Delegate eventDelegate, string eventName)
        {
            if (eventDelegate == null)
            {
                Debug.Log($"[Player Local EventManager Debug] PlayerActionEvent '{eventName}' has no subscribers.");
                return;
            }

            var subscribers = eventDelegate.GetInvocationList();
            Debug.Log($"[Player Local EventManager Debug] PlayerActionEvent '{eventName}' has {subscribers.Length} subscriber(s):");

            foreach (var subscriber in subscribers)
            {
                Debug.Log($"  - {subscriber.Target} in method {subscriber.Method.Name}");
            }
        }


        public void T_OnGroundLightAttackStarted()
        {
            OnGroundLightAttackStarted?.Invoke();

            if (debugCharacterActionEvents)
                DebugSubscribers(OnGroundLightAttackStarted, "OnPlayerGroundLightAttackStarted");

        }
        public void T_OnAirLightAttackStarted()
        {
            OnAirLightAttackStarted?.Invoke();

            if (debugCharacterActionEvents)
                DebugSubscribers(OnAirLightAttackStarted, "OnPlayerAirLightAttackStarted");

        }
        public void T_OnDrawSheatheStarted()
        {
            OnDrawSheatheStarted?.Invoke();

            if (debugCharacterActionEvents)
                DebugSubscribers(OnDrawSheatheStarted, "OnDrawSheatheStarted");
        }

        public void T_OnBlockStarted()
        {
            OnBlockStarted?.Invoke();
            if(debugCharacterActionEvents)
                DebugSubscribers(OnBlockStarted, "OnPlayerBlockStarted");
        }

        public void T_OnBlockStopped()
        {
            OnBlockStopped?.Invoke();
            if(debugCharacterActionEvents)
                DebugSubscribers(OnBlockStopped, "OnPlayerBlockStopped");
        }


        public void T_OnTurnBackRunningStarted(float duration)
        {
            OnTurnBackRunningStarted?.Invoke(duration);
            if(debugCharacterActionEvents)
                DebugSubscribers(OnTurnBackRunningStarted, "OnTurnBackRunningStarted");
        }
    }


    [System.Serializable]
    public class PlayerCombatHandlerEvents
    {
        public event Action<AttackElement> OnSetAttackLocks;
        public event Action<float> OnSetCombatTransitionLock;

        public event Action<AttackElement> OnPlayerAttackPerformed;

        public event Action OnPlayerWeaponsDisabled;
        public event Action OnPlayerWeaponsEnabled;


        public event Action OnPlayerTookDamage;


        [SerializeField] private bool debugPlayerCombatHandlerEvents;


        private void DebugSubscribers(Delegate eventDelegate, string eventName)
        {
            if (eventDelegate == null)
            {
                Debug.Log($"[Player Local EventManager Debug] PlayerActionEvent '{eventName}' has no subscribers.");
                return;
            }

            var subscribers = eventDelegate.GetInvocationList();
            Debug.Log($"[Player Local EventManager Debug] PlayerActionEvent '{eventName}' has {subscribers.Length} subscriber(s):");

            foreach (var subscriber in subscribers)
            {
                Debug.Log($"  - {subscriber.Target} in method {subscriber.Method.Name}");
            }
        }


        public void T_OnPlayerAttackPerformed(AttackElement attackElement)
        {
            OnPlayerAttackPerformed?.Invoke(attackElement);

            if (debugPlayerCombatHandlerEvents)
                DebugSubscribers(OnPlayerAttackPerformed, "OnPlayerAttackPerformed");
        }

        public void T_OnPlayerWeaponsDisabled()
        {
            OnPlayerWeaponsDisabled?.Invoke();

            if (debugPlayerCombatHandlerEvents)
                DebugSubscribers(OnPlayerWeaponsDisabled, "OnPlayerWeaponsDisabled");
        }

        public void T_OnPlayerWeaponsEnabled()
        {
            OnPlayerWeaponsEnabled?.Invoke();

            if (debugPlayerCombatHandlerEvents)
                DebugSubscribers(OnPlayerWeaponsEnabled, "OnPlayerWeaponsEnabled");
        }

        public void T_OnPlayerTookDamage()
        {
            OnPlayerTookDamage?.Invoke();
            if(debugPlayerCombatHandlerEvents)
                DebugSubscribers(OnPlayerTookDamage, "OnPlayerTookDamage");
        }
    }


    [System.Serializable]
    public class CombatHandlerEvents
    {
        [SerializeField] private bool debugCombatHandlerEvents;
        private void DebugSubscribers(Delegate eventDelegate, string eventName)
        {
            if (eventDelegate == null)
            {
                Debug.Log($"[Character Local EventManager Debug] CombatHandlerEvents '{eventName}' has no subscribers.");
                return;
            }

            var subscribers = eventDelegate.GetInvocationList();
            Debug.Log($"[Character Local EventManager Debug] CombatHandlerEvents '{eventName}' has {subscribers.Length} subscriber(s):");

            foreach (var subscriber in subscribers)
            {
                Debug.Log($"  - {subscriber.Target} in method {subscriber.Method.Name}");
            }
        }



        public event Action<AttackElement> OnSetAttackLocks;
        public void T_OnSetAttackLocks(AttackElement attackElement)
        {
            OnSetAttackLocks?.Invoke(attackElement);

            if (debugCombatHandlerEvents)
                DebugSubscribers(OnSetAttackLocks, "OnSetAttackLocks");
        }


        public event Action<float> OnSetCombatTransitionLock;
        public void T_OnSetCombatTransitionLock(float duration)
        {
            OnSetCombatTransitionLock?.Invoke(duration);

            if (debugCombatHandlerEvents)
                DebugSubscribers(OnSetCombatTransitionLock, "OnSetCombatTransitionLock");
        }

    }


    [System.Serializable]
    public class CharacterStateEvents
    {
        public event Action<CharacterState, CharacterState> OnPlayerCharacterStateChanged;

        public void T_OnPlayerCharacterStateChanged(CharacterState fromState, CharacterState toState)
        {
            OnPlayerCharacterStateChanged?.Invoke(fromState, toState);
        }
    }


    [System.Serializable]
    public class CharacterBusyStateEvents
    {
        public event Action OnComboLockReleased;
        public event Action OnInputLockReleased;
        public event Action OnAnimStateLockReleased;
        public event Action OnReturnToIdleLockReleased;
        public event Action OnCombatStateTransitionReleased;

        public event Action OnRunningTurnBackAnimReleased;


        [SerializeField] private bool debugPlayerBusyStateControllerEvents;

        private void DebugSubscribers(Delegate eventDelegate, string eventName)
        {
            if (eventDelegate == null)
            {
                Debug.Log($"[Character Local EventManager Debug] CharacterBusyStateEvents '{eventName}' has no subscribers.");
                return;
            }

            var subscribers = eventDelegate.GetInvocationList();
            Debug.Log($"[Character Local EventManager Debug] CharacterBusyStateEvents '{eventName}' has {subscribers.Length} subscriber(s):");

            foreach (var subscriber in subscribers)
            {
                Debug.Log($"  - {subscriber.Target} in method {subscriber.Method.Name}");
            }
        }


        public void T_OnComboLockReleased()
        {
            OnComboLockReleased?.Invoke();

            if (debugPlayerBusyStateControllerEvents)
                DebugSubscribers(OnComboLockReleased, "OnComboLockReleased");
        }

        public void T_OnInputLockReleased()
        {
            OnInputLockReleased?.Invoke();

            if (debugPlayerBusyStateControllerEvents)
                DebugSubscribers(OnInputLockReleased, "OnInputLockReleased");
        }

        public void T_OnAnimStateLockReleased()
        {
            OnAnimStateLockReleased?.Invoke();

            if (debugPlayerBusyStateControllerEvents)
                DebugSubscribers(OnAnimStateLockReleased, "OnAnimStateLockReleased");
        }

        public void T_OnReturnToIdleLockReleased()
        {
            OnReturnToIdleLockReleased?.Invoke();

            if (debugPlayerBusyStateControllerEvents)
                DebugSubscribers(OnReturnToIdleLockReleased, "OnReturnToIdleLockReleased");
        }

        public void T_OnCombatStateTransitionReleased()
        {
            OnCombatStateTransitionReleased?.Invoke();

            if (debugPlayerBusyStateControllerEvents)
                DebugSubscribers(OnCombatStateTransitionReleased, "OnCombatStateTransitionReleased");
        }

        public void T_OnRunningTurnBackAnimReleased()
        {
            OnRunningTurnBackAnimReleased?.Invoke();

            if (debugPlayerBusyStateControllerEvents)
                DebugSubscribers(OnRunningTurnBackAnimReleased, "OnRunningTurnBackAnimReleased");
        }
    }


}
