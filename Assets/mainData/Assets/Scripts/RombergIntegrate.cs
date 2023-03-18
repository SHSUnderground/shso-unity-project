internal class RombergIntegrate
{
	public delegate float Function(float x);

	public static float Integrate(float a, float b, Function F)
	{
		float[,] array = new float[2, 5];
		float num = b - a;
		array[0, 0] = num * (F(a) + F(b)) / 2f;
		int num2 = 2;
		int num3 = 1;
		while (num2 <= 5)
		{
			float num4 = 0f;
			for (int i = 1; i <= num3; i++)
			{
				num4 += F(a + num * ((float)i - 0.5f));
			}
			array[1, 0] = (array[0, 0] + num * num4) / 2f;
			int num5 = 1;
			int num6 = 4;
			while (num5 < num2)
			{
				array[1, num5] = ((float)num6 * array[1, num5 - 1] - array[0, num5 - 1]) / ((float)num6 - 1f);
				num5++;
				num6 *= 4;
			}
			for (int j = 0; j < num2; j++)
			{
				array[0, j] = array[1, j];
			}
			num2++;
			num3 *= 2;
			num /= 2f;
		}
		return array[0, 4];
	}
}
