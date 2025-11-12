namespace Sandbox;

public static class Program
{
    public static void Main(params string[] args)
    {
        const string path = @"d:/temp/SVB/SVBWWB65PLUS00002_kapot.dat.txt";
        // const string path = @"d:/temp/SVB/SVBWWB65PLUS00002_goed.dat.txt";

        var handler = new Handler();
        handler.Execute(path);
    }
}
