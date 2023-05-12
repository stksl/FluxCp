namespace Fluxcp;
public unsafe sealed class SourceText
{
    public char this[int index] 
    {
        get 
        {
            if (index >= Length || index < 0) throw new IndexOutOfRangeException();
            return *(_text + index);
        }
    }
    private int _length;
    public int Length  => _length;
    private char* _text;
    public SourceText(char* start, int length)
    {
        _text = start;
        _length = length;
    }
    public static SourceText FromString(string source) 
    {
        fixed(char* ptr = source) 
        {
            return new SourceText(ptr, source.Length);
        }
    }
}