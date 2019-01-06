namespace Math.Vectors
{
    public interface IVector
    {
        Vector GetNormalized();
        float GetMagnitude();
        Vector GetReverse();
    }
}