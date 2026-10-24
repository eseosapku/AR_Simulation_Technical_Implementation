using System.Collections.Generic;

namespace FirstResponse
{
    public static class ScenarioFactory
    {
        public const int Spurting = 0;
        public const int Oozing = 2;

        public static List<ProcedureStep> Build(ScenarioConfig config)
        {
            bool thigh = config.NeedsTourniquet;
            List<ProcedureStep> steps = new List<ProcedureStep>
            {
                new TapStep("SecureScene", "Secure the scene", "Tap each hazard to make it safe", "ShieldExclamation", 2),
                new ChoiceStep("HandProtection", "Protect yourself", "What goes on your hands?", "HandStop",
                    new[] { "Gloves or plastic bags", "Bare hands" }, 0),
                new ChoiceStep("CallForHelp", "Call for help", "Which number do you dial?", "Volume",
                    new[] { config.emergencyNumber, "911", "Wait and see" }, 0),
                new ChoiceStep("AssessBleeding", "Look at the bleeding", "How is the blood coming out?", "Droplet",
                    new[] { "Spurting", "Pooling", "Oozing" }, thigh ? Spurting : Oozing),
                new ChoiceStep("ChooseIntervention", "Choose your first action", "What do you do first?", "HandStop",
                    new[] { "Direct pressure", "Tourniquet" }, 0, thigh ? -1 : 1),
                new PressureStep()
            };

            if (thigh)
            {
                steps.Add(new TourniquetStep());
                steps.Add(new WindlassStep());
                steps.Add(new TapStep("RecordTime", "Note the time", "Tap to record when the tourniquet went on", "Clock", 1));
            }

            steps.Add(new TapStep("ShockCare", "Treat for shock", "Lay them down and keep them warm", "Bulb", 1));
            return steps;
        }
    }
}
