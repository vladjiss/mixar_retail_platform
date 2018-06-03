using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEngine.XR.iOS
{

	public class UnityRemoteVideo : MonoBehaviour
	{
		public ConnectToEditor connectToEditor;
		public RenderTexture rTexture;

		Texture2D tex;

		private byte[] m_textureBytes;

		private UnityARSessionNativeInterface m_Session;
		private bool bTexturesInitialized;

		private int currentFrameIndex;
		private byte[] m_textureYBytes;
		private byte[] m_textureUVBytes;
		private byte[] m_textureYBytes2;
		private byte[] m_textureUVBytes2;
		private GCHandle m_pinnedYArray;
		private GCHandle m_pinnedUVArray;

		#if !UNITY_EDITOR

		public void Start()
		{
			m_Session = UnityARSessionNativeInterface.GetARSessionNativeInterface ();
			UnityARSessionNativeInterface.ARFrameUpdatedEvent += UpdateCamera;
			currentFrameIndex = 0;
			bTexturesInitialized = false;
		}

		void UpdateCamera(UnityARCamera camera)
		{
			if (!bTexturesInitialized) {
				InitializeTextures (camera);
			}
			UnityARSessionNativeInterface.ARFrameUpdatedEvent -= UpdateCamera;

		}

		void InitializeTextures(UnityARCamera camera)
		{
			int numYBytes = camera.videoParams.yWidth * camera.videoParams.yHeight;
			int numUVBytes = camera.videoParams.yWidth * camera.videoParams.yHeight / 2; //quarter resolution, but two bytes per pixel
										
			tex = new Texture2D(1280, 720, TextureFormat.RGB24, false);
			m_textureBytes = new byte[numYBytes];


			m_textureYBytes = new byte[numYBytes];
			m_textureUVBytes = new byte[numUVBytes];
			m_textureYBytes2 = new byte[numYBytes];
			m_textureUVBytes2 = new byte[numUVBytes];
			m_pinnedYArray = GCHandle.Alloc (m_textureYBytes);
			m_pinnedUVArray = GCHandle.Alloc (m_textureUVBytes);
			bTexturesInitialized = true;
		}

		IntPtr PinByteArray(ref GCHandle handle, byte[] array)
		{
			handle.Free ();
			handle = GCHandle.Alloc (array, GCHandleType.Pinned);
			return handle.AddrOfPinnedObject ();
		}

		byte [] ByteArrayForFrame(int frame,  byte[] array0,  byte[] array1)
		{
			return frame == 1 ? array1 : array0;
		}

		byte [] YByteArrayForFrame(int frame)
		{
			return ByteArrayForFrame (frame, m_textureYBytes, m_textureYBytes2);
		}

		byte [] UVByteArrayForFrame(int frame)
		{
			return ByteArrayForFrame (frame, m_textureUVBytes, m_textureUVBytes2);
		}

		void OnDestroy()
		{
			m_Session.SetCapturePixelData (false, IntPtr.Zero, IntPtr.Zero);

			m_pinnedYArray.Free ();
			m_pinnedUVArray.Free ();
		}
		Texture2D toTexture2D(RenderTexture rTex)
		{
			RenderTexture.active = rTex;
			//camera.videoParams.yWidth, camera.videoParams.yHeight,
			tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
			tex.Apply();
			return tex;
		}
		public void OnPreRender()
		{
			ARTextureHandles handles = m_Session.GetARVideoTextureHandles();
			if (handles.textureY == System.IntPtr.Zero || handles.textureCbCr == System.IntPtr.Zero)
			{
				return;
			}
			if (!bTexturesInitialized)
				return;
			currentFrameIndex = (currentFrameIndex + 1) % 2;
			Resolution currentResolution = Screen.currentResolution;
			m_Session.SetCapturePixelData (true, PinByteArray(ref m_pinnedYArray,YByteArrayForFrame(currentFrameIndex)), PinByteArray(ref m_pinnedUVArray,UVByteArrayForFrame(currentFrameIndex)));
			
			tex = toTexture2D (rTexture) ;
			//tex.Compress (true);
			m_textureBytes = tex.GetRawTextureData();
			connectToEditor.SendToEditor (ConnectionMessageIds.screenCaptureRenTextureMsgId, m_textureBytes);
			//ucl.SendByte (m_textureBytes);
			//connectToEditor.SendToEditor (ConnectionMessageIds.screenCaptureYMsgId, YByteArrayForFrame(1-currentFrameIndex));
			//connectToEditor.SendToEditor (ConnectionMessageIds.screenCaptureUVMsgId, UVByteArrayForFrame(1-currentFrameIndex));
			
		}
		#endif
	}
}
