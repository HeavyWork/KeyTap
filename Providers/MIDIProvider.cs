using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Midi;

namespace KeyTap.Providers
{
    public sealed class MIDIProvider : IKeyTapProvider
    {
        #region Static Data

        public string Name { get; } = "MIDI";

        public string Icon { get; } = "ConnectionZone";

        #endregion

        #region Core Data

        private Dictionary<string, MidiIn> _midiInDevices;
        private Dictionary<string, MidiOut> _midiOutDevices;

        private KeyTapManager _manager;

        #endregion

        #region Constructors

        public MIDIProvider(KeyTapManager manager)
        {
            _manager = manager;

            for (int device = 0; device < MidiIn.NumberOfDevices; device++)
            {
                MidiIn midiIn = new MidiIn(device);
                _midiInDevices.Add(MidiIn.DeviceInfo(device).ProductName, midiIn);
                midiIn.MessageReceived += MidiInOnMessageReceived;
                midiIn.Start();
            }

            for (int device = 0; device < MidiOut.NumberOfDevices; device++)
            {
                MidiOut midiOut = new MidiOut(device);
                _midiOutDevices.Add(MidiOut.DeviceInfo(device).ProductName, midiOut);
            }

            _manager.KeyList.CollectionChanged += RefreshListenKeys;
            RefreshListenKeys(null, null);
        }

        #endregion

        #region Core

        private void MidiInOnMessageReceived(object sender, MidiInMessageEventArgs e)
        {
            if (_manager.ListenState == KeyTapListenState.Off) return;
            try
            {
                MidiIn midiIn = sender as MidiIn;
                if (midiIn is null) return;

                bool isOnEvent;
                if (e.MidiEvent.CommandCode == MidiCommandCode.NoteOn) isOnEvent = true;
                else if (e.MidiEvent.CommandCode == MidiCommandCode.NoteOff) isOnEvent = false;
                else return;

                NoteEvent noteEvent = e.MidiEvent as NoteEvent;
                if (noteEvent is null) return;

                if (noteEvent.Velocity == 0) isOnEvent = false;

                TapKey tapKey = new TapKey(
                    Name,
                    Icon,
                    _midiInDevices.FirstOrDefault(pair => pair.Value == midiIn).Key,
                    $"{noteEvent.Channel}|{noteEvent.NoteNumber}",
                    $"Channel {noteEvent.Channel} - {noteEvent.NoteNumber}");

                if (_manager.ListenState == KeyTapListenState.ListOnly &&
                    !_manager.KeyList.Contains(tapKey)) return;

                if (isOnEvent) KeyDown?.Invoke(this, tapKey);
                else KeyUp?.Invoke(this, tapKey);
            }
            catch (Exception)
            {
                // Ignore
            }
        }

        private NoteEvent ParseTapKey(TapKey tapKey)
        {
            try
            {
                string[] s = tapKey.Id.Split('|');
                return new NoteEvent(
                    0,
                    int.Parse(s[0]),
                    MidiCommandCode.NoteOn,
                    int.Parse(s[1]),
                    100);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void RefreshListenKeys(object sender, NotifyCollectionChangedEventArgs e)
        {
            try
            {
                foreach (KeyValuePair<string, MidiOut> midiOut in _midiOutDevices)
                {
                    for (int channel = 0; channel < 16; channel++)
                    {
                        for (int noteNumber = 0; noteNumber < 128; noteNumber++)
                            midiOut.Value.Send(
                                new NoteEvent(0, channel, MidiCommandCode.NoteOff, noteNumber, 100)
                                    .GetAsShortMessage());
                    }
                }
            }
            catch (Exception)
            {
                // Ignore
            }

            Collection<TapKey> keysToClear = new Collection<TapKey>();

            foreach (TapKey tapKey in _manager.KeyList.Where(key => key.Name == Name))
            {
                try
                {
                    if (!_midiInDevices.Keys.Contains(tapKey.Device))
                    {
                        keysToClear.Add(tapKey);
                        continue;
                    }

                    _midiOutDevices[tapKey.Device].Send(ParseTapKey(tapKey).GetAsShortMessage());
                }
                catch (Exception)
                {
                    // Ignore
                }
            }

            foreach (TapKey tapKey in keysToClear)
                while (true)
                    if (!_manager.KeyList.Remove(tapKey))
                        break;
        }

        #endregion

        #region Key Events

        public event EventHandler<TapKey> KeyDown;
        public event EventHandler<TapKey> KeyUp;

        #endregion

        #region Dispose

        public void Dispose()
        {
            foreach (KeyValuePair<string, MidiIn> midiIn in _midiInDevices)
            {
                midiIn.Value.Stop();
                midiIn.Value.Dispose();
            }

            foreach (KeyValuePair<string, MidiOut> midiOut in _midiOutDevices) midiOut.Value.Dispose();

            _manager.KeyList.CollectionChanged -= RefreshListenKeys;
        }

        #endregion
    }
}
