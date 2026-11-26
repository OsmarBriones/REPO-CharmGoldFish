using CharmGoldFish.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CharmGoldFish.Tests;

[TestClass]
public class LuckyCatchMathTests
{
	[TestMethod]
	public void Wins_BelowTheChanceOnly()
	{
		Assert.IsTrue(LuckyCatchMath.Wins(10f, 0.0999));
		Assert.IsFalse(LuckyCatchMath.Wins(10f, 0.1));
		Assert.IsFalse(LuckyCatchMath.Wins(10f, 0.9));
	}

	[TestMethod]
	public void Wins_ZeroNeverAndHundredAlways()
	{
		Assert.IsFalse(LuckyCatchMath.Wins(0f, 0d));
		Assert.IsTrue(LuckyCatchMath.Wins(100f, 0.999999));
	}

	[TestMethod]
	public void CopyValue_IsTheWholeDollarsOfTheCurrentValue()
	{
		Assert.AreEqual(900, LuckyCatchMath.CopyValue(900f));
		Assert.AreEqual(899, LuckyCatchMath.CopyValue(899.7f));
	}

	[TestMethod]
	public void CopyValue_RefusesWorthlessValuables()
	{
		Assert.AreEqual(0, LuckyCatchMath.CopyValue(0.5f));
		Assert.AreEqual(0, LuckyCatchMath.CopyValue(-10f));
		Assert.AreEqual(0, LuckyCatchMath.CopyValue(float.NaN));
	}

	[TestMethod]
	public void WearForCopy_TakesExactlyTheCopyValue()
	{
		// A $900 copy costs a $1500 fish 60% of its original value, which is $900.
		Assert.AreEqual(60f, LuckyCatchMath.WearForCopy(900, 1500f, 100f), 0.001f);
	}

	[TestMethod]
	public void WearForCopy_FollowsTheChargeShare()
	{
		Assert.AreEqual(30f, LuckyCatchMath.WearForCopy(900, 1500f, 50f), 0.001f);
		Assert.AreEqual(0f, LuckyCatchMath.WearForCopy(900, 1500f, 0f));
	}

	[TestMethod]
	public void WearForCopy_IsCappedAtTheWholeFish()
	{
		Assert.AreEqual(100f, LuckyCatchMath.WearForCopy(1500, 1500f, 100f), 0.001f);
		Assert.AreEqual(100f, LuckyCatchMath.WearForCopy(10000, 1500f, 100f));
	}

	[TestMethod]
	public void WearForCopy_IgnoresImpossibleInput()
	{
		Assert.AreEqual(0f, LuckyCatchMath.WearForCopy(0, 1500f, 100f));
		Assert.AreEqual(0f, LuckyCatchMath.WearForCopy(900, 0f, 100f));
		Assert.AreEqual(0f, LuckyCatchMath.WearForCopy(900, 1500f, -5f));
	}

	[TestMethod]
	public void RolledValuables_GiveOneChanceEach()
	{
		RolledValuables rolled = new RolledValuables();
		Assert.IsTrue(rolled.TryClaim(7));
		Assert.IsFalse(rolled.TryClaim(7));
		Assert.IsTrue(rolled.TryClaim(-3));
		rolled.Clear();
		Assert.AreEqual(0, rolled.Count);
		Assert.IsTrue(rolled.TryClaim(7));
	}
}
