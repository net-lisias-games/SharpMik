﻿using SharpMik.DSP;
using System;

namespace SharpMik.Interfaces
{
    public abstract class IModDriver : IDisposable
    {
        #region protected variables
        protected IModDriver m_Next;
        protected String m_Name;
        protected String m_Version;

        protected byte m_HardVoiceLimit;
        protected byte m_SoftVoiceLimit;

        protected bool m_AutoUpdating;

        protected Idsp m_DspProcessor = null;
        #endregion


        #region public accessors
        public IModDriver NextDriver
        {
            get { return m_Next; }
        }

        public String Name
        {
            get { return m_Name; }
        }

        public String Version
        {
            get { return m_Version; }
        }

        public byte HardVoiceLimit
        {
            get { return m_HardVoiceLimit; }
        }

        public byte SoftVoiceLimit
        {
            get { return m_SoftVoiceLimit; }
        }

        public bool AutoUpdating
        {
            get { return m_AutoUpdating; }
        }

        public Idsp DspProcessor
        {
            get { return m_DspProcessor; }
            set { m_DspProcessor = value; }
        }
        #endregion


        #region abstract functions
        public abstract void CommandLine(string command);
        public abstract bool IsPresent();
        public abstract short SampleLoad(SAMPLOAD sample, int type);
        public abstract void SampleUnload(short handle);

        public abstract short[] GetSample(short handle);
        public abstract short SetSample(short[] sample);

        public abstract uint FreeSampleSpace(int value);
        public abstract uint RealSampleLength(int value, SAMPLE sample);
        public abstract bool Init();
        public abstract void Exit();
        public abstract bool Reset();
        public abstract bool SetNumVoices();
        public abstract bool PlayStart();
        public abstract void PlayStop();
        public abstract void Update();

        public abstract void Pause();
        public abstract void Resume();

        public abstract void VoiceSetVolume(byte voice, ushort volume);
        public abstract ushort VoiceGetVolume(byte voice);
        public abstract void VoiceSetFrequency(byte voice, uint freq);
        public abstract uint VoiceGetFrequency(byte voice);
        public abstract void VoiceSetPanning(byte voice, uint panning);
        public abstract uint VoiceGetPanning(byte voice);
        public abstract void VoicePlay(byte voice, short handle, uint start, uint size, uint reppos, uint repend, ushort flags);
        public abstract void VoiceStop(byte voice);
        public abstract bool VoiceStopped(byte voice);
        public abstract int VoiceGetPosition(byte voice);
        public abstract uint VoiceRealVolume(byte voice);

        public abstract void Dispose();
        #endregion
    }
}
