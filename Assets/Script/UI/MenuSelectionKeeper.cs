using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace TimeHollow.UI
{
    [DefaultExecutionOrder(-1000)]
    public sealed class MenuSelectionKeeper : MonoBehaviour
    {
        [SerializeField] private EventSystem eventSystem;
        [SerializeField] private InputSystemUIInputModule inputModule;
        [SerializeField] private Selectable primaryButton;
        private bool waitForRelease;
        private bool selectionPending;
        private bool navigationSuppressed;
        private bool previousSendNavigationEvents;

        private void OnEnable()
        {
            waitForRelease = true;
            selectionPending = true;
            FocusPrimary();
        }

        public void FocusPrimary()
        {
            if (!CanSelectPrimary())
                return;

            eventSystem.SetSelectedGameObject(primaryButton.gameObject);
            selectionPending = false;
            waitForRelease = true;
            SuppressNavigation();
        }

        private bool CanSelectPrimary() => eventSystem != null && inputModule != null &&
            primaryButton != null && primaryButton.isActiveAndEnabled && primaryButton.IsInteractable();

        private void Update()
        {
            if (!CanSelectPrimary())
                return;

            if (selectionPending)
            {
                FocusPrimary();
                return;
            }

            var move = inputModule.move != null ? inputModule.move.action : null;
            var submit = inputModule.submit != null ? inputModule.submit.action : null;
            var hasInput = (move != null && move.ReadValue<Vector2>().sqrMagnitude > 0.01f) ||
                (submit != null && (submit.IsPressed() || submit.WasPerformedThisFrame()));

            if (waitForRelease)
            {
                SuppressNavigation();
                if (!hasInput)
                    waitForRelease = false;
                return;
            }

            if (eventSystem.currentSelectedGameObject == null && hasInput)
                FocusPrimary();
        }

        private void SuppressNavigation()
        {
            if (!navigationSuppressed)
            {
                previousSendNavigationEvents = eventSystem.sendNavigationEvents;
                navigationSuppressed = true;
            }
            // Run before EventSystem.Update. Pointer processing stays enabled.
            eventSystem.sendNavigationEvents = false;
        }

        private void RestoreNavigation()
        {
            if (navigationSuppressed && eventSystem != null)
                eventSystem.sendNavigationEvents = previousSendNavigationEvents;
            navigationSuppressed = false;
        }

        private void LateUpdate() => RestoreNavigation();
        private void OnDisable() => RestoreNavigation();
    }
}
