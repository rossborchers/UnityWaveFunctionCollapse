using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace UnityWFC
{
	public class Bitmap
	{
		public int Height { get; private set; }
		public int Width { get; private set; }

		private Texture2D _texture2D;

		public Bitmap() { }
		  
		public Bitmap(int width, int height)
		{ 
			Width = width; 
			Height = height;
			CreateTexture();
		}

		private void CreateTexture()
		{
			if (_texture2D != null)
			{
				if (Application.isPlaying)
				{
					Object.Destroy(_texture2D);
				}
				else
				{
					Object.DestroyImmediate(_texture2D);
				}
			}
			_texture2D = new Texture2D(Width, Height, TextureFormat.RGBA32, false);
		}
		  
		public Color32 GetPixel(int x, int y)
		{ 
			return _texture2D.GetPixel(x, y);
		}

		public void Copy(int[] bitmapIntData)
		{
			if (bitmapIntData.Length == 0)
			{
				Debug.LogError("[Bitmap] Empty int array passed to Copy(...).");
				return;
			}

			try
			{
				CreateTexture();
				
				//https://stackoverflow.com/questions/5896680/converting-an-int-to-byte-in-c-sharp
				byte[] bitmapData = new byte[bitmapIntData.Length * sizeof(int)];
				Buffer.BlockCopy(bitmapIntData, 0, bitmapData, 0, bitmapData.Length);
				_texture2D.LoadRawTextureData(bitmapData);
			}
			catch (Exception e)
			{
				Debug.LogError($"[Bitmap] {e.Message}");
			}
		}

		  public async Task Save(string fullPath)
		  {
			  //Read from StreamingAssets
			  /*if (!Path.HasExtension(fullPath))
			  {
				  fullPath += ".png";
			  }

			  byte[] data = _texture2D.EncodeToPNG();
			  using UnityWebRequest request = UnityWebRequest.Put(fullPath, data);
			  
			  await request.SendWebRequest();
	    
			  if (request.result != UnityWebRequest.Result.Success)
			  {
				  Debug.LogError($"[Bitmap] Failed to save image! {request.error}");
			  }
			  else
			  {
				  //Success
			  }*/
			  
			  byte[] data = _texture2D.EncodeToPNG();
			  File.WriteAllBytes(fullPath, data);
		  }

		  public async Task Load(string fullPath)
		  {
			  //Read from StreamingAssets
			  if (!Path.HasExtension(fullPath))
			  {
				  fullPath += ".png";
			  }

			  using UnityWebRequest request = UnityWebRequest.Get(fullPath);
			  await request.SendWebRequest();
			  if (request.result != UnityWebRequest.Result.Success)
			  {
				  Debug.LogError($"[Bitmap] Failed to import image! {request.error}");
			  }
			  else
			  {
				  byte[] data = request.downloadHandler.data;
				  
				  _texture2D = new Texture2D(2, 2);
				  _texture2D.LoadImage(data, false);
				  Width = _texture2D.width;
				  Height = _texture2D.height;
			  }
		  }
	}
	
	public static class BitmapExtensions
	{
		public static TaskAwaiter GetAwaiter(this AsyncOperation asyncOp)
		{
			var tcs = new TaskCompletionSource<object>();
			asyncOp.completed += obj => { tcs.SetResult(null); };
			return ((Task)tcs.Task).GetAwaiter();
		}
	}
	
}