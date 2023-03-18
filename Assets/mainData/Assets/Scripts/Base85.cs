using System;
using System.IO;
using System.Text;

internal class Base85
{
	private static int _asciiOffset = 33;

	private static int encodeBlockSize = 5;

	private static int decodeBlockSize = 4;

	private static uint[] pow85 = new uint[5]
	{
		52200625u,
		614125u,
		7225u,
		85u,
		1u
	};

	public static string ToBase85String(byte[] inArray, int offset, int length)
	{
		int capacity = (length - offset) * (encodeBlockSize / decodeBlockSize);
		StringBuilder stringBuilder = new StringBuilder(capacity);
		byte[] encodedBlock = new byte[encodeBlockSize];
		int num = 0;
		uint num2 = 0u;
		for (int i = offset; i < length; i++)
		{
			byte b = inArray[i];
			if (num >= decodeBlockSize - 1)
			{
				num2 |= b;
				if (num2 == 0)
				{
					stringBuilder.Append('z');
				}
				else
				{
					EncodeBlock(num2, encodeBlockSize, encodedBlock, stringBuilder);
				}
				num = 0;
				num2 = 0u;
			}
			else
			{
				num2 = (uint)((int)num2 | (b << 24 - num * 8));
				num++;
			}
		}
		if (num > 0)
		{
			EncodeBlock(num2, num + 1, encodedBlock, stringBuilder);
		}
		return stringBuilder.ToString();
	}

	public static byte[] FromBase85String(string s)
	{
		MemoryStream memoryStream = new MemoryStream();
		byte[] array = new byte[decodeBlockSize];
		int num = 0;
		uint num2 = 0u;
		bool flag = false;
		foreach (char c in s)
		{
			switch (c)
			{
			case 'z':
				if (num != 0)
				{
					throw new Exception("The character 'z' is invalid inside an ASCII85 block.");
				}
				array[0] = 0;
				array[1] = 0;
				array[2] = 0;
				array[3] = 0;
				memoryStream.Write(array, 0, decodeBlockSize);
				flag = false;
				break;
			case '\0':
			case '\b':
			case '\t':
			case '\n':
			case '\f':
			case '\r':
				flag = false;
				break;
			default:
				if (c < '!' || c > 'u')
				{
					throw new Exception("Bad character '" + c + "' found. ASCII85 only allows characters '!' to 'u'.");
				}
				flag = true;
				break;
			}
			if (flag)
			{
				num2 = (uint)((int)num2 + (c - _asciiOffset) * (int)pow85[num]);
				num++;
				if (num == encodeBlockSize)
				{
					DecodeBlock(num2, decodeBlockSize, array);
					memoryStream.Write(array, 0, decodeBlockSize);
					num2 = 0u;
					num = 0;
				}
			}
		}
		switch (num)
		{
		case 1:
			throw new Exception("The last block of ASCII85 data cannot be a single byte.");
		default:
			num--;
			num2 += pow85[num];
			DecodeBlock(num2, num, array);
			memoryStream.Write(array, 0, num);
			break;
		case 0:
			break;
		}
		return memoryStream.ToArray();
	}

	private static void EncodeBlock(uint tuple, int count, byte[] encodedBlock, StringBuilder sb)
	{
		for (int num = encodeBlockSize - 1; num >= 0; num--)
		{
			encodedBlock[num] = (byte)(tuple % 85u + _asciiOffset);
			tuple /= 85u;
		}
		for (int i = 0; i < count; i++)
		{
			sb.Append((char)encodedBlock[i]);
		}
	}

	private static void DecodeBlock(uint tuple, int bytes, byte[] decodedBlock)
	{
		for (int i = 0; i < bytes; i++)
		{
			decodedBlock[i] = (byte)(tuple >> 24 - i * 8);
		}
	}
}
