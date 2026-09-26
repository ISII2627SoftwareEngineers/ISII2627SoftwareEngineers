public class Reposición
{
    public int Id { get; set; }
    public DateTime FechaReposicion { get; set; }
    public decimal PrecioTotal { get; set; }
    public string? Comentario { get; set; }

    public Reposición(){}
    public override bool Equals(object? obj)
    {
        if (obj is not Reposicion other)
            return false;

        return Id == other.Id;
    }
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}public class Reposición
{
    public int Id { get; set; }
    public DateTime FechaReposicion { get; set; }
    public decimal PrecioTotal { get; set; }
    public string? Comentario { get; set; }

    public Reposición(){}
    public override bool Equals(object? obj)
    {
        if (obj is not Reposicion other)
            return false;

        return Id == other.Id;
    }
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}