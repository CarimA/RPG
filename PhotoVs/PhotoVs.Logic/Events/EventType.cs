namespace PhotoVs.Logic.Events
{
    public static class EventType
    {
        public const string GAME_START = "GAME_START";

        // events which would be nice
        /*
         * OnSaveLoaded
         * OnSaveCreated
         * OnStep
         * OnWalkStart
         * OnWalkEnd
         * OnRunStart
         * OnRunEnd
         * OnStandStart
         * OnStandEnd
         *
         * ZoneLoad
         * ZoneUnload
         * MapLoad
         * MapUnload
         */

        public const string INTERACT_ACTION = "INTERACT_ACTION";
        public const string INTERACT_AREA_ENTER = "INTERACT_AREA_ENTER";
        public const string INTERACT_AREA_EXIT = "INTERACT_AREA_EXIT";
        public const string INTERACT_AREA_STAND = "INTERACT_AREA_STAND";
        public const string INTERACT_AREA_WALK = "INTERACT_AREA_WALK";
        public const string INTERACT_AREA_RUN = "INTERACT_AREA_RUN";

        public const string COLLISION = "COLLISION";
    }
}
