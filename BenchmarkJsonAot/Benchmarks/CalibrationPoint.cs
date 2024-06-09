namespace BenchmarkJsonAot.Benchmarks;

public class CalibrationPoint
{
    public int ScreenX { get; set; }
    public int ScreenY { get; set; }
    public int RawX { get; set; }
    public int RawY { get; set; }

    public CalibrationPoint()
    {
    }

    public CalibrationPoint(int screenX, int screenY, int rawX, int rawY)
    {
        ScreenX = screenX;
        ScreenY = screenY;
        RawX = rawX;
        RawY = rawY;
    }
}
