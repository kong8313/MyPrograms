namespace Confirmit.CATI.Supervisor.Controls
{
    public class ImageProvider
    {
        public string GetSvg(string commandImage, string title = "")
        {
            if (commandImage == "empty")
            {
                return string.Empty;
            }

            try
            {
                var imageContent = SvgIcons.ResourceManager.GetObject(commandImage).ToString();
                return string.Format(imageContent, $"{commandImage}Icon", title);
            }
            catch
            {
                return string.Format(SvgIcons.attention, "IconNotFound", title);
            }
        }
    }
}