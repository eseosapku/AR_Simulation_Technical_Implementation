using NUnit.Framework;

namespace FirstResponse.Tests
{
    public class SetupScreenTests
    {
        [Test]
        public void NoButtonUntilTheSceneIsPlaced()
        {
            Assert.AreEqual(SetupAction.None, SetupScreen.PrimaryAction(PlacementState.Scanning));
            Assert.AreEqual(SetupAction.None, SetupScreen.PrimaryAction(PlacementState.Aiming));
        }

        [Test]
        public void PlacedSceneAsksForConfirmThenBegin()
        {
            Assert.AreEqual(SetupAction.Confirm, SetupScreen.PrimaryAction(PlacementState.Previewing));
            Assert.AreEqual(SetupAction.Begin, SetupScreen.PrimaryAction(PlacementState.Confirmed));
        }

        [Test]
        public void EveryStateHasClearInstructions()
        {
            Assert.AreEqual("Looking for the floor", SetupScreen.Describe(PlacementState.Scanning).status);
            Assert.AreEqual("Tap the floor", SetupScreen.Describe(PlacementState.Aiming).title);
            Assert.AreEqual("Happy with this spot?", SetupScreen.Describe(PlacementState.Previewing).title);
            Assert.AreEqual("Scene anchored", SetupScreen.Describe(PlacementState.Confirmed).status);
        }
    }
}
