namespace ShopEgypt.Application.Utilities;

/// <summary>
/// Provides random unique integer ID generation for entities whose PK is int
/// but needs to be set manually (without relying on DB identity auto-increment).
/// </summary>
public static class IdHelper
{
    /// <summary>
    /// Generates a random positive int suitable for use as a DB primary key.
    /// Combines the last 6 digits of UTC ticks with 3 random digits to form
    /// a 9-digit integer, reducing collision probability under normal load.
    /// </summary>
    public static int GenerateRandomId()
    {
        int timePart   = (int)(DateTime.UtcNow.Ticks % 1_000_000); // last 6 digits of ticks
        int randomPart = Random.Shared.Next(100, 999);               // 3 random digits
        return Math.Abs(timePart * 1000 + randomPart);               // guaranteed positive
    }
}
