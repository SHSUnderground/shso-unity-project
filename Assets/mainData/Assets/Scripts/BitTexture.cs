using System.Collections;

public class BitTexture
{
	private BitArray bits;

	private int width;

	private int height;

	public BitArray Bits
	{
		get
		{
			return bits;
		}
	}

	public int Width
	{
		get
		{
			return width;
		}
	}

	public int Height
	{
		get
		{
			return height;
		}
	}

	public BitTexture(int Width, int Height)
		: this(null, Width, Height)
	{
	}

	public BitTexture(BitArray Bits, int Width, int Height)
	{
		bits = ((Bits != null) ? Bits : new BitArray(Width * Height));
		width = Width;
		height = Height;
	}
}
