using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using KeyTap.Providers;

namespace KeyTap
{
    public class KeyTapManager
        : INotifyPropertyChanged, IDisposable, IKeyTapEventProvider
    {
        #region ListenState

        private KeyTapListenState _listenState = KeyTapListenState.Off;

        public KeyTapListenState ListenState
        {
            get => _listenState;
            set
            {
                _listenState = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Providers

        private readonly IKeyTapProvider[] Providers = 
        {
            new KeyboardProvider()
        };

        #endregion

        #region Keylist

        public ObservableCollection<TapKey> KeyList { get; }

        #endregion

        #region Constructor

        public KeyTapManager()
        {
            KeyList = new ObservableCollection<TapKey>();
            KeyList.CollectionChanged += (sender, args) => OnPropertyChanged(nameof(KeyList));
            foreach (IKeyTapProvider provider in Providers)
            {
                provider.KeyDown += ProviderOnKeyDown;
                provider.KeyUp += ProviderOnKeyUp;
            }
        }

        #endregion

        #region Key Events

        public event EventHandler<TapKey> KeyDown;

        public event EventHandler<TapKey> KeyUp;

        private void ProviderOnKeyDown(object sender, TapKey e)
        {
            if (ListenState == KeyTapListenState.Off) return;
            if (ListenState == KeyTapListenState.ListOnly &&
                !KeyList.Contains(e)) return;
            KeyDown?.Invoke(this, e);
        }

        private void ProviderOnKeyUp(object sender, TapKey e)
        {
            if (ListenState == KeyTapListenState.Off) return;
            if (ListenState == KeyTapListenState.ListOnly &&
                !KeyList.Contains(e)) return;
            KeyUp?.Invoke(this, e);
        }

        #endregion

        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            foreach (IKeyTapProvider provider in Providers) provider.Dispose();
        }

        #endregion
    }

    public enum KeyTapListenState
    {
        Off = 0,
        ListOnly = 1,
        All = 2
    }
}
