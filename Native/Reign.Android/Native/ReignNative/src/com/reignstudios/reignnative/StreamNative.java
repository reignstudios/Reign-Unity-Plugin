package com.reignstudios.reignnative;

import java.io.IOException;
import java.io.RandomAccessFile;

import android.content.Intent;
import android.database.Cursor;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.provider.MediaStore;
import android.provider.MediaStore.Images.ImageColumns;

public class StreamNative implements ReignActivityCallbacks
{
	private static StreamNative singleton;
	private static boolean saveImageDone, saveImageSucceeded;
	private static boolean loadImageDone, loadImageSucceeded;
	public static int LoadImageIntentID = 123;
	private static byte[] loadImageData;
	
	public static void Init()
	{
		singleton = new StreamNative();
		ReignUnityActivity.AddCallbacks(singleton);
	}
	
	public static void SaveImage(final byte[] data, final String title, final String desc)
	{
		saveImageSucceeded = false;
		saveImageDone = false;
		
		ReignUnityActivity.ReignContext.runOnUiThread(new Runnable()
		{
			public void run()
			{
				try
				{
					Bitmap image = BitmapFactory.decodeByteArray(data, 0, data.length);
					MediaStore.Images.Media.insertImage(ReignUnityActivity.ReignContext.getContentResolver(), image, title, desc);
					saveImageSucceeded = true;
					saveImageDone = true;
				}
				catch (Exception e)
				{
					saveImageSucceeded = false;
					saveImageDone = true;
				}
			}
		});
	}
	
	public static void LoadImage()
	{
		loadImageSucceeded = false;
		loadImageDone = false;
		
		ReignUnityActivity.ReignContext.runOnUiThread(new Runnable()
		{
			public void run()
			{
				try
				{
					Intent photoPickerIntent = new Intent(Intent.ACTION_PICK);
					photoPickerIntent.setType("image/*");
					ReignUnityActivity.ReignContext.startActivityForResult(photoPickerIntent, LoadImageIntentID);
					
					//Intent intent = new Intent(Intent.ACTION_GET_CONTENT, null);
			        //intent.setType("image/*");
			        //intent.putExtra("return-data", true);
			        //ReignUnityActivity.ReignContext.startActivityForResult(intent, LoadImageIntentID);
				}
				catch (Exception e)
				{
					loadImageSucceeded = false;
					loadImageDone = true;
				}
			}
		});
	}
	
	@Override
	public boolean onActivityResult(int requestCode, int resultcode, Intent intent)
	{
		if (requestCode != LoadImageIntentID) return false;
		
		try
		{
			if (intent != null)
			{
				Cursor cursor = ReignUnityActivity.ReignContext.getContentResolver().query(intent.getData(), null, null, null, null);
				cursor.moveToFirst();
				int idx = cursor.getColumnIndex(ImageColumns.DATA);
				String fileName = cursor.getString(idx);
				
				loadImageData = readFile(fileName);
				loadImageSucceeded = true;
				loadImageDone = true;
				
				// REMEMBER for getting pixel data...
				//Bitmap bitmapPreview = BitmapFactory.decodeFile(fileName);
				//bitmapPreview.copyPixelsToBuffer(dst)// <<< REMEMBER
			}
			else
			{
				loadImageSucceeded = false;
				loadImageDone = true;
			}
		}
		catch (Exception e)
		{
			loadImageSucceeded = false;
			loadImageDone = true;
		}
		
		return true;
	}
	
	@Override
	public void onPause()
	{
		// do nothing...
	}
	
	@Override
	public void onResume()
	{
		// do nothing...
	}
	
	private static byte[] readFile(String fileName) throws IOException
	{
        // Open file
        RandomAccessFile f = new RandomAccessFile(fileName, "r");
        try
        {
            // Get and check length
            long longlength = f.length();
            int length = (int)longlength;
            if (length != longlength) throw new IOException("File size >= 2 GB");
            
            // Read file and return data
            byte[] data = new byte[length];
            f.readFully(data);
            return data;
        }
        catch (Exception e)
        {
        	return null;
        }
        finally
        {
            f.close();
        }
    }
	
	public static boolean CheckSaveImageDone()
	{
		Boolean d = saveImageDone;
		saveImageDone = false;
		return d;
	}
	
	public static boolean CheckSaveImageSucceeded()
	{
		return saveImageSucceeded;
	}
	
	public static boolean CheckLoadImageDone()
	{
		Boolean d = loadImageDone;
		loadImageDone = false;
		return d;
	}
	
	public static boolean CheckLoadImageSucceeded()
	{
		return loadImageSucceeded;
	}
	
	public static byte[] GetLoadedImageData()
	{
		byte[] data = loadImageData;
		loadImageData = null;
		return data;
	}
}
