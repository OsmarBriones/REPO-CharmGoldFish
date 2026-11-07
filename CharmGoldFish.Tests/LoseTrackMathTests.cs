using CharmGoldFish.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CharmGoldFish.Tests;

[TestClass]
public class LoseTrackMathTests
{
	[TestMethod]
	public void ExtraDecrease_DoubleSpeedTakesOneMoreStep()
	{
		Assert.AreEqual(9.9f, LoseTrackMath.ExtraDecrease(10f, 0.1f, 2f), 0.0001f);
	}

	[TestMethod]
	public void ExtraDecrease_FiveTimesTakesFourMoreSteps()
	{
		Assert.AreEqual(9.6f, LoseTrackMath.ExtraDecrease(10f, 0.1f, 5f), 0.0001f);
	}

	[TestMethod]
	public void ExtraDecrease_ClampsAtZero()
	{
		Assert.AreEqual(0f, LoseTrackMath.ExtraDecrease(0.05f, 0.1f, 2f));
	}

	[DataTestMethod]
	[DataRow(0f, 0.1f, 2f)]
	[DataRow(-1f, 0.1f, 2f)]
	[DataRow(5f, 0f, 2f)]
	[DataRow(5f, -0.1f, 2f)]
	[DataRow(5f, 0.1f, 1f)]
	[DataRow(5f, 0.1f, 0.5f)]
	public void ExtraDecrease_LeavesTheValueAlone(float timer, float delta, float multiplier)
	{
		Assert.AreEqual(timer, LoseTrackMath.ExtraDecrease(timer, delta, multiplier));
		Assert.IsFalse(LoseTrackMath.Applies(timer, delta, multiplier));
	}

	[TestMethod]
	public void Applies_WhenTheTimerRunsAndTheMultiplierIsAboveOne()
	{
		Assert.IsTrue(LoseTrackMath.Applies(3f, 0.02f, 2f));
	}

	[TestMethod]
	public void ExtraDecrease_SumsToTheScaledDurationOverManyFrames()
	{
		float timer = 10f;
		float seconds = 0f;
		while (timer > 0f)
		{
			timer -= 0.02f; // the game's own decrease
			timer = LoseTrackMath.ExtraDecrease(timer, 0.02f, 2f);
			seconds += 0.02f;
		}

		Assert.AreEqual(5f, seconds, 0.05f);
	}

	[TestMethod]
	public void TakeWear_WaitsForOnePercent()
	{
		float pending = 0f;
		Assert.AreEqual(0f, LoseTrackMath.TakeWear(ref pending, 2f, 0.25f));
		Assert.AreEqual(0.5f, pending, 0.0001f);
		float step = LoseTrackMath.TakeWear(ref pending, 2f, 0.3f);
		Assert.AreEqual(1.1f, step, 0.0001f);
		Assert.AreEqual(0f, pending);
	}

	[TestMethod]
	public void TakeWear_TwoPercentPerSecondWearsTwoPercentInOneSecond()
	{
		float pending = 0f;
		float total = 0f;
		for (int i = 0; i < 50; i++)
		{
			total += LoseTrackMath.TakeWear(ref pending, 2f, 0.02f);
		}

		Assert.AreEqual(2f, total + pending, 0.001f);
	}

	[TestMethod]
	public void TakeWear_ZeroRateOrTimeDoesNothing()
	{
		float pending = 0.4f;
		Assert.AreEqual(0f, LoseTrackMath.TakeWear(ref pending, 0f, 1f));
		Assert.AreEqual(0f, LoseTrackMath.TakeWear(ref pending, 2f, 0f));
		Assert.AreEqual(0.4f, pending, 0.0001f);
	}
}
