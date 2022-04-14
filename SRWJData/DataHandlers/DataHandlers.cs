namespace SRWJData.DataHandlers
{
    public static class DataHandlers
    {
        private static IModelHandler? modelHandler;
        public static void UseROMHandlers(string filePath)
        {
            if (modelHandler != null && modelHandler is ROMModelHandler)
                ((ROMModelHandler)modelHandler).SetFilePath(filePath);
            else
                modelHandler = new ROMModelHandler(filePath);
        }

        public static IModelHandler? GetModelHandler() => modelHandler;
    }
}
