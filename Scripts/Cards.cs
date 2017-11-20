public enum DirectionType
{
    H,
    R,
    L,
    MU,
    MD
}

public enum CardType
{
    Normal,
    SkyWheel,
    Vaccum
}

public class Cards {
    public readonly int Value;
    public readonly DirectionType DT;
    public readonly CardType CT;
    public readonly string ImgName;

    public Cards(int v, DirectionType dt, CardType ct, string img)
    {
        this.Value = v;
        this.DT = dt;
        this.CT = ct;
        this.ImgName = img;
    }
}



