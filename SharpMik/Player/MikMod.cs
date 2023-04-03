﻿using SharpMik.Interfaces;
using System;
using System.IO;

namespace SharpMik.Player
{
	public class MikMod : IDisposable
	{
		String m_Error;
		String m_CommandLine;

		public event SharpMik.Player.ModPlayer.PlayerStateChangedEvent PlayerStateChangeEvent;

		public bool HasError
		{
			get { return !String.IsNullOrEmpty(m_Error); }
		}

		public String ErrorMessage
		{
			get
			{
				String error = m_Error;
				m_Error = null;
				return error;
			}
		}


		public MikMod()
		{
			ModPlayer.PlayStateChangedHandle += new ModPlayer.PlayerStateChangedEvent(ModPlayer_PlayStateChangedHandle);
		}

		void ModPlayer_PlayStateChangedHandle(ModPlayer.PlayerState state)
		{
			if (PlayerStateChangeEvent != null)
			{
				PlayerStateChangeEvent(state);
			}
		}

		public float GetProgress()
		{
			if (ModPlayer.mod != null)
			{
				float current = (ModPlayer.mod.sngpos * ModPlayer.mod.numrow) + ModPlayer.mod.patpos;
				float total = ModPlayer.mod.numpos * ModPlayer.mod.numrow;

				return current / total;
			}
			return 0.0f;
		}

		private object loaded_Driver;

		public bool Init<T>(Drivers.NaudioDriverAdvanced.NaudioDriverAdvacedOptions naudioDriverOptions) where T : IModDriver, new()
		{
			loaded_Driver = ModDriver.LoadDriver<T>(naudioDriverOptions);
			return ModDriver.MikMod_Init("");
		}

		public bool Init<T>(Drivers.WavDriver.WavDriverOptions wavDriverOptions) where T : IModDriver, new()
		{
			loaded_Driver = ModDriver.LoadDriver<T>(wavDriverOptions);
			return ModDriver.MikMod_Init("");
		}

		public bool Init<T>() where T : IModDriver, new()
		{
			return Init<T>("");
		}

		public bool Init<T>(String command) where T : IModDriver, new()
		{
			m_CommandLine = command;
			ModDriver.LoadDriver<T>();

			return ModDriver.MikMod_Init(command);
		}

		public T Init<T>(String command, out bool result) where T : IModDriver, new()
		{
			m_CommandLine = command;
			T driver = ModDriver.LoadDriver<T>();

			result = ModDriver.MikMod_Init(command);

			return driver;
		}

		public void Reset()
		{
			ModDriver.MikMod_Reset(m_CommandLine);
		}

		public void Exit()
		{
			ModDriver.MikMod_Exit();
		}

		public Module LoadModule(String fileName)
		{
			m_Error = null;
			if (ModDriver.Driver != null)
			{
				try
				{
					return ModuleLoader.Load(fileName);
				}
				catch (System.Exception ex)
				{
					m_Error = ex.Message;
				}
			}
			else
			{
				m_Error = "A Driver needs to be set before loading a module";
			}

			return null;
		}

		public Module LoadModule(Stream stream)
		{
			m_Error = null;
			if (ModDriver.Driver != null)
			{
				try
				{
					return ModuleLoader.Load(stream, 128, 0);
				}
				catch (System.Exception ex)
				{
					m_Error = ex.Message;
				}
			}
			else
			{
				m_Error = "A Driver needs to be set before loading a module";
			}

			return null;
		}

		public void UnLoadModule(Module mod)
		{
			// Make sure the mod is stopped before unloading.
			Stop();
			ModuleLoader.UnLoad(mod);
		}

		public void UnLoadCurrent()
		{
			if (ModPlayer.mod != null)
			{
				ModuleLoader.UnLoad(ModPlayer.mod);
			}
		}

		public Module Play(String name)
		{
			Module mod = LoadModule(name);

			if (mod != null)
			{
				Play(mod);
			}

			return mod;
		}


		public Module Play(Stream stream)
		{
			Module mod = LoadModule(stream);

			if (mod != null)
			{
				Play(mod);
			}

			return mod;
		}

		public void Play(Module mod)
		{
			ModPlayer.Player_Start(mod);
		}

		public bool IsPlaying()
		{
			return ModPlayer.Player_Active();
		}

		public void Stop()
		{
			ModPlayer.Player_Stop();
		}

		public void TogglePause()
		{
			ModPlayer.Player_TogglePause();
		}


		public void SetPosition(int position)
		{
			ModPlayer.Player_SetPosition((ushort)position);
		}

		// Fast forward will mute all the channels and mute the driver then update mikmod till it reaches the song position that is requested
		// then it will unmute and unpause the audio after.
		// this makes sure that no sound is heard while fast forwarding.
		// the bonus of fast forwarding over setting the position is that it will know the real state of the mod.
		public void FastForwardTo(int position)
		{
			ModPlayer.Player_Mute_Channel(SharpMik.SharpMikCommon.MuteOptions.MuteAll, null);
			ModDriver.Driver_Pause(true);
			while (ModPlayer.mod.sngpos != position)
			{
				ModDriver.MikMod_Update();
			}
			ModDriver.Driver_Pause(false);
			ModPlayer.Player_UnMute_Channel(SharpMik.SharpMikCommon.MuteOptions.MuteAll, null);
		}

		public void MuteChannel(int channel)
		{
			ModPlayer.Player_Mute_Channel(channel);
		}

		public void MuteChannel(SharpMikCommon.MuteOptions option, params int[] list)
		{
			ModPlayer.Player_Mute_Channel(option, list);
		}

		public void UnMuteChannel(int channel)
		{
			ModPlayer.Player_UnMute_Channel(channel);
		}

		public void UnMuteChannel(SharpMikCommon.MuteOptions option, params int[] list)
		{
			ModPlayer.Player_UnMute_Channel(option, list);
		}

		/// <summary>
		/// Depending on the driver this might need to be called, it should be safe to call even if the driver is auto updating.
		/// </summary>
		public void Update()
		{
			if (ModDriver.Driver != null && !ModDriver.Driver.AutoUpdating)
			{
				ModDriver.MikMod_Update();
			}
		}

		public void Dispose()
		{
			if (loaded_Driver!=null)
			{
				((Drivers.VirtualSoftwareDriver)loaded_Driver).Dispose();
			}
		}
	}
}
