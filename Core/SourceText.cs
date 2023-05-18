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
    public override string ToString()
    {
        return new string(_text, 0, Length);
    }
    public string ToString(int offset, int length) 
    {
        return new string(_text, offset, length);
    }
    public static SourceText FromString(string source) 
    {
        fixed(char* ptr = source) 
        {
            return new SourceText(ptr, source.Length);
        }
    }
    public static SourceText FromString(string source, int offset, int length) 
    {
        fixed(char* roptr = source) 
        {
            char* ptr = roptr;
            ptr += offset;
            return new SourceText(ptr, length);
        }
    }
}