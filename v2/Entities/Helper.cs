namespace v2.Entities
{
    public static class Helper
    {
        public static bool AllowedExtension(this string extension)
        {
            List<string> allowedExtensions = new List<string> { ".png", ".jpg" };

            return allowedExtensions.Contains(extension.ToLower());
        }

        // Max accepted image size from user
        public static long AcceptedImageSizeBytes = 5 * 1024 * 1024; // 5 MB

        // Resized to image size
        public static long MaxResizedImageSizeBytes = 10 * 1024; // 10 KB

        // Displa picture size
        public static int DpWidth = 150;
        public static int DpHeight = 150;
    }
}
