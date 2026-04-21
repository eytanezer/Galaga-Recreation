using System;

namespace State.Models
{
    public class MovementData
    {
        private bool _isEntering;
        private bool _isDiving;
        private bool _isWaiting;
        private bool _isIdle;
        private bool _isDancing;

        public bool IsWaiting
        {
            get => _isWaiting;
            set
            {
                if (_isWaiting == value) return;
                _isWaiting = value;
                OnDataChanged?.Invoke();
            }
        }

        public bool IsEntering 
        { 
            get => _isEntering;
            set
            {
                if (_isEntering == value) return;

                _isEntering = value;
                OnDataChanged?.Invoke();
            } 
        }
        
        public bool IsDiving
        { 
            get => _isDiving;
            set
            {
                if (_isDiving == value) return;

                _isDiving = value;
                OnDataChanged?.Invoke();
            } 
        }

        public bool IsIdle
        {
            get => _isIdle;
            set
            {
                if (_isIdle == value) return;
                _isIdle = value;
            }
        }
        
        
        public bool IsDancing
        {
            get => _isDancing;
            set
            {
                if (_isDancing == value) return;
                _isDancing = value;
                OnDataChanged?.Invoke();
            }
        }
        
        public event Action OnDataChanged;

        public override string ToString() => $"Waiting: {IsWaiting}, Entering: {IsEntering}, Diving: {IsDiving}";
    }
}