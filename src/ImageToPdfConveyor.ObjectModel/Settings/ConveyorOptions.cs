namespace ImageToPdfConveyor.ObjectModel.Settings
{
    public sealed class ConveyorOptions
    {
        public string PathToImages { get; set; } = "./";

        public int AmountImagesInPdf { get; set; } = 3;

        public string BasePdfDocumentName { get; set; } = "result";

        public string OutputDirectory { get; set; } = "./";

        public string ImageFormat { get; set; } = "jpg";
    }
}
