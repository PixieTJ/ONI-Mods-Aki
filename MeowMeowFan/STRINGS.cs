namespace MeowMeowFan
{
    public static class STRINGS
    {
        public static class BUILDINGS
        {
            public static class PREFABS
            {
                public static class MEOWMEOWSELECTIVEGASFAN
                {
                    public static LocString NAME = "Selective Ventilation Fan";
                    public static LocString DESC = "A compact ventilation fan with very particular tastes. Affectionately known as the Meow Meow Fan.";
                    public static LocString EFFECT = "Draws only the selected gas from nearby tiles and sends it into a Gas Pipe.\n\nThe selected gas is copied with building settings and the fan can be controlled by Automation.";
                }

                // Hidden legacy prefab names retained so existing saves from the original
                // Ventilating Fan mod can load without rebuilding every fan.
                public static class O2GASPUMP
                {
                    public static LocString NAME = "Oxygen Ventilation Fan (Legacy)";
                    public static LocString DESC = "An existing fan migrated from the original Ventilating Fan mod.";
                    public static LocString EFFECT = "Functions as a Selective Ventilation Fan and initially targets Oxygen.";
                }

                public static class O22GASPUMP
                {
                    public static LocString NAME = "Polluted Oxygen Ventilation Fan (Legacy)";
                    public static LocString DESC = "An existing fan migrated from the original Ventilating Fan mod.";
                    public static LocString EFFECT = "Functions as a Selective Ventilation Fan and initially targets Polluted Oxygen.";
                }

                public static class CO2GASPUMP
                {
                    public static LocString NAME = "Carbon Dioxide Ventilation Fan (Legacy)";
                    public static LocString DESC = "An existing fan migrated from the original Ventilating Fan mod.";
                    public static LocString EFFECT = "Functions as a Selective Ventilation Fan and initially targets Carbon Dioxide.";
                }

                public static class CL2GASPUMP
                {
                    public static LocString NAME = "Chlorine Ventilation Fan (Legacy)";
                    public static LocString DESC = "An existing fan migrated from the original Ventilating Fan mod.";
                    public static LocString EFFECT = "Functions as a Selective Ventilation Fan and initially targets Chlorine.";
                }

                public static class H2GASPUMP
                {
                    public static LocString NAME = "Hydrogen Ventilation Fan (Legacy)";
                    public static LocString DESC = "An existing fan migrated from the original Ventilating Fan mod.";
                    public static LocString EFFECT = "Functions as a Selective Ventilation Fan and initially targets Hydrogen.";
                }

                public static class CH4GASPUMP
                {
                    public static LocString NAME = "Natural Gas Ventilation Fan (Legacy)";
                    public static LocString DESC = "An existing fan migrated from the original Ventilating Fan mod.";
                    public static LocString EFFECT = "Functions as a Selective Ventilation Fan and initially targets Natural Gas.";
                }
            }
        }
    }
}
