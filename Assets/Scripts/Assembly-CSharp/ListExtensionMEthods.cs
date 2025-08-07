using System.Collections.Generic;

public static class ListExtensionMEthods
{
	public static void AddSorted(this List<screenShake.shake> shakes, screenShake.shake newSHake)
	{
		List<screenShake.shake> list = shakes;
		int num = -1;
		float num2 = -1f;
		for (int num3 = shakes.Count - 1; num3 >= 0; num3--)
		{
			if (shakes[num3].timeLeft < newSHake.timeLeft && shakes[num3].timeLeft > num2)
			{
				num2 = shakes[num3].timeLeft;
				num = num3;
			}
		}
		if (num != -1)
		{
			list.Insert(num, newSHake);
		}
		else
		{
			list.Add(newSHake);
		}
		shakes = list;
	}

	public static void RemoveThingyYao(this List<screenShake.shake> shakes, screenShake.shake shakeToRemove)
	{
		List<screenShake.shake> list = shakes;
		list.Remove(shakeToRemove);
		shakes = list;
	}
}
