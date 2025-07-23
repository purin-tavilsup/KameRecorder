namespace KameRecorder.Extensions;

public static class BitmapExtensions
{
	public static Bitmap DrawCursor(this Bitmap bitmap, int x, int y)
	{
		using var graphics = Graphics.FromImage(bitmap);
		Cursors.Default.Draw(graphics, new Rectangle(x, y, Cursors.Default.Size.Width, Cursors.Default.Size.Height));
		
		return bitmap;
	}
}