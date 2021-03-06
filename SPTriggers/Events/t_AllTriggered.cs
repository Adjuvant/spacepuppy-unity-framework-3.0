﻿#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy.Events
{

    public sealed class t_AllTriggered : TriggerComponent, IMStartOrEnableReceiver
    {

        #region Fields

        [SerializeField()]
        [ReorderableArray()]
        [DisableOnPlay()]
        private ObservableTargetData[] _observedTargets;

        [SerializeField()]
        [OnChangedInEditor("ResetOnTriggeredChanged", OnlyAtRuntime = true)]
        private bool _resetOnTriggered;


        [System.NonSerialized()]
        private HashSet<ObservableTargetData> _activatedTriggers = new HashSet<ObservableTargetData>();
        [System.NonSerialized()]
        private bool _triggered;

        #endregion

        #region CONSTRUCTOR

        void IMStartOrEnableReceiver.OnStartOrEnable()
        {
            _activatedTriggers.Clear();
            this.RegisterListeners();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            this.UnRegisterListeners();
            _activatedTriggers.Clear();
        }

        #endregion

        #region Properties

        public bool ResetOnTriggered
        {
            get { return _resetOnTriggered; }
            set
            {
                if (_resetOnTriggered == value) return;

                _resetOnTriggered = value;
                this.ResetOnTriggeredChanged();
            }
        }

        #endregion

        #region Methods

        private void ResetOnTriggeredChanged()
        {
            if (_resetOnTriggered && _triggered && this.isActiveAndEnabled)
            {
                _triggered = false;
                this.RegisterListeners();
            }
        }

        private void RegisterListeners()
        {
            if (_triggered) return;

            ObservableTargetData targ;
            var d = new System.Action<ObservableTargetData>(this.OnTriggerActivated);
            for (int i = 0; i < _observedTargets.Length; i++)
            {
                targ = _observedTargets[i];
                targ.AddHandler(d);
            }
        }

        private void UnRegisterListeners()
        {
            ObservableTargetData targ;
            var d = new System.Action<ObservableTargetData>(this.OnTriggerActivated);
            for (int i = 0; i < _observedTargets.Length; i++)
            {
                targ = _observedTargets[i];
                if (targ != null) targ.RemoveHandler(d);
            }
        }

        private void OnTriggerActivated(ObservableTargetData sender)
        {
            if (_triggered) return;

            _activatedTriggers.Add(sender);

            if (_activatedTriggers.SetEquals(_observedTargets))
            {
                _activatedTriggers.Clear();
                if (this._resetOnTriggered)
                {
                    _triggered = false;
                }
                else
                {
                    _triggered = true;
                    this.UnRegisterListeners();
                }
                this.ActivateTrigger();
            }
        }

        #endregion

    }

}
